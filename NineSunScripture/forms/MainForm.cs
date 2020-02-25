using NineSunScripture.forms;
using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.persistence;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using NineSunScripture.util.test;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture
{
    public interface IWorkListener
    {
        void OnAcctInfoUpdate(Account account);

        void OnPriceChange(Quotes quotes);

        void OnImgRotate(int degree);
    }

    public partial class MainForm : Form, ITrade, IWorkListener
    {
        private bool isRebooting = false;
        private bool isStrategyStarted = false;
        private int rotateDegree = 0;
        private Image imgTaiJi;
        //用来临时保存总账户信息
        private Account account;
        private MainStrategy mainStrategy;
        private UpdatePrice updatePrice;

        private List<Quotes> latestStocks;
        private List<Quotes> longTermStocks;
        private List<Quotes> bandStocks;
        private List<Quotes> dragonLeaders;
        //持仓股票，这个列表是用来存放卖出计划的，不是底部的那个持仓
        private List<Quotes> positionStocks;
        private List<Quotes> stocks;
        private List<Quotes> weakTurnStrongStocks;

        public MainForm()
        {
            stocks = new List<Quotes>();
            mainStrategy = new MainStrategy();
            InitializeComponent();
            InitPeriod();
            InitializeListViews();
            BindStocksData();
            mainStrategy.SetWorkListener(this);
            mainStrategy.SetTradeCallback(this);
            imgTaiJi = Properties.Resources.taiji;
            updatePrice = new UpdatePrice(UpdateStocksPrice);

            //Start strategy
            Task.Run(() =>
            {
                Thread.Sleep(5000);
                RebootStrategy();
            });
        }

        private delegate void SetTextCallback(string text);

        private delegate void UpdatePrice(Quotes quotes);

        /// <summary>
        /// 添加股票
        /// </summary>
        /// <param name="quotes">股票对象</param>
        public void AddStock(Quotes quotes)
        {
            if (null == quotes)
            {
                return;
            }
            JsonDataHelper.Instance.AddStock(quotes);
            AddRuntimeInfo("新增股票【" + quotes.Name + "】");
            RefreshStocksListView();
            if (!stocks.Contains(quotes))
            {
                stocks.Add(quotes);
            }
            mainStrategy.AddStock(quotes);
        }

        public void OnAcctInfoUpdate(Account account)
        {
            if (null == account)
            {
                return;
            }
            this.account = account;
            Invoke(new MethodInvoker(UpdateAcctInfo));
        }

        /// <summary>
        /// 交易结果回调（这里调用了成员变量，要加锁或者改成同步方法）
        /// </summary>
        /// <param name="rspCode">接口返回响应码</param>
        /// <param name="msg">正确消息</param>
        /// <param name="errInfo">错误消息</param>
        /// <param name="needReboot">是否需要重启策略</param>
        public void OnTradeResult(int rspCode, string msg, string errInfo, bool needReboot)
        {
            Logger.Log("OnTradeResult：" + rspCode + ">" + msg);
            if (rspCode > 0)
            {
                AddRuntimeInfo(msg + ">成功");
                if (MainStrategy.RspCodeOfUpdateAcctInfo == rspCode)
                {
                    Utils.PlaySuccessSoundHint();
                    mainStrategy.UpdateTotalAccountInfo(false);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(errInfo) && !string.IsNullOrEmpty(msg))
                {
                    errInfo = msg;
                }
                AddRuntimeInfo(msg + ">失败，错误信息：" + errInfo);
                //多线程同时调用，必须加锁，而整个方法加锁的话会导致方法顺序执行，导致多次重启
                //只有这样加锁才能解决问题
                lock (mainStrategy)
                {
                    if (needReboot && !isRebooting)
                    {
                        InvokeRebootStrategy();
                    }
                }
            }
        }

        private void InitPeriod()
        {
            Dictionary<string, string> settings = JsonDataHelper.Instance.Settings;
            if (null != settings)
            {
                if (!settings.ContainsKey("period"))
                {
                    settings.Add("period", "up");
                    JsonDataHelper.Instance.SaveSettings(settings);
                }
                if (settings["period"].Equals("up"))
                {
                    btnPeriod.Text = "涨潮期";
                    btnPeriod.BackColor = Color.Red;
                }
                else
                {
                    btnPeriod.Text = "退潮期";
                    btnPeriod.BackColor = Color.Green;
                }
            }
            else
            {
                settings = new Dictionary<string, string>();
                settings.Add("period", "up");
                JsonDataHelper.Instance.SaveSettings(settings);
            }
        }

        /// <summary>
        /// 添加运行时信息
        /// </summary>
        private void AddRuntimeInfo(string text)
        {
            if (tbRuntimeInfo.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                while (!tbRuntimeInfo.IsHandleCreated)
                {
                    //解决窗体关闭时出现“访问已释放句柄“的异常
                    if (tbRuntimeInfo.Disposing || tbRuntimeInfo.IsDisposed)
                        return;
                }
                try
                {
                    SetTextCallback d = new SetTextCallback(AddRuntimeInfo);
                    tbRuntimeInfo.Invoke(d, new object[] { text });
                }
                catch (Exception e)
                {
                    Logger.Log("AddRuntimeInfo---- tbRuntimeInfo.Invoke--Exception-" + e.Message);
                    Logger.Exception(e);
                }
            }
            else
            {
                tbRuntimeInfo.AppendText(
                    DateTime.Now.ToString("HH:mm:ss") + " " + text + "\r\n");
                tbRuntimeInfo.ScrollToCaret();
            }
        }

        private void BindCancelOrdersData()
        {
            lvCancelOrders.BeginUpdate();
            lvCancelOrders.Items.Clear();
            foreach (Order order in account.CancelOrders)
            {
                ListViewItem lvi = new ListViewItem(order.Name);
                lvi.SubItems.Add(order.Operation + "");
                lvi.SubItems.Add(order.Quantity + "");
                lvi.SubItems.Add(order.TransactionQuantity + "");
                lvi.SubItems.Add(Math.Round(order.Price, 2) + "");
                lvi.SubItems.Add(order.TransactionPrice + "");
                lvi.SubItems.Add(order.CanceledQuantity + "");
                lvi.Tag = order;

                lvCancelOrders.Items.Add(lvi);
            }
            lvCancelOrders.EndUpdate();
        }

        private void BindPositionsData()
        {
            float totalProfit = 0;
            lvPositions.BeginUpdate();
            lvPositions.Items.Clear();
            int totalPositionRatio = 0;
            foreach (Position position in account.Positions)
            {
                ListViewItem lvi = new ListViewItem(position.Name);
                lvi.SubItems.Add(position.StockBalance + "");
                lvi.SubItems.Add(position.AvailableBalance + "");
                lvi.SubItems.Add(Math.Round(position.AvgCost, 2) + "");
                lvi.SubItems.Add(Math.Round(position.ProfitAndLoss / 10000, 2) + "万");
                lvi.SubItems.Add(Math.Round(position.ProfitAndLossPct, 1) + "%");
                lvi.SubItems.Add(Math.Round(position.MarketValue / 10000, 2) + "万");
                if (null != account.Funds && account.Funds.TotalAsset > 0)
                {
                    int positionRatio = (int)(position.MarketValue / account.Funds.TotalAsset * 100);
                    lvi.SubItems.Add(positionRatio + "%");
                    lvi.Tag = position;
                    totalProfit += position.ProfitAndLoss;
                    totalPositionRatio += positionRatio;
                }

                lvPositions.Items.Add(lvi);
            }
            lvPositions.EndUpdate();
            lblTotalProfit.Text = "总盈亏：" + Math.Round(totalProfit / 10000, 2) + "万";
            lblTotalPositionRatio.Text = "总仓位：" + totalPositionRatio + "%";
        }

        /// <summary>
        /// 绑定股票到listview
        /// </summary>
        private void BindStocksData()
        {
            ListViewGroup lvgDragonLeader = new ListViewGroup("龙头");
            ListViewGroup lvgLongTerm = new ListViewGroup("常驻打板");
            ListViewGroup lvgTomorrow = new ListViewGroup("打板");
            ListViewGroup lvgWeakTurnStrong = new ListViewGroup("弱转强");
            ListViewGroup lvgBand = new ListViewGroup("波段");
            ListViewGroup lvgPositions = new ListViewGroup("持仓");

            lvStocks.Groups.Add(lvgDragonLeader);
            lvStocks.Groups.Add(lvgLongTerm);
            lvStocks.Groups.Add(lvgTomorrow);
            lvStocks.Groups.Add(lvgWeakTurnStrong);
            lvStocks.Groups.Add(lvgBand);
            lvStocks.Groups.Add(lvgPositions);

            ListViewItem lvi;
            List<Quotes> quotes = JsonDataHelper.Instance.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryDragonLeader);
            dragonLeaders = quotes;
            if (!Utils.IsListEmpty(quotes))
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgDragonLeader);
                    lvi.SubItems.Add(Utils.FormatPositionForShow(item.PositionCtrl));
                    lvi.SubItems.Add(Utils.FormatMoneyForShow(item.MoneyCtrl));
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            longTermStocks = quotes = JsonDataHelper.Instance.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryLongTerm);
            if (!Utils.IsListEmpty(quotes))
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgLongTerm);
                    lvi.SubItems.Add(Utils.FormatPositionForShow(item.PositionCtrl));
                    lvi.SubItems.Add(Utils.FormatMoneyForShow(item.MoneyCtrl));
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            bandStocks = quotes = JsonDataHelper.Instance.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryBand);
            if (!Utils.IsListEmpty(quotes))
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgBand);
                    lvi.SubItems.Add(Utils.FormatPositionForShow(item.PositionCtrl));
                    lvi.SubItems.Add(Utils.FormatMoneyForShow(item.MoneyCtrl));
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            weakTurnStrongStocks = quotes = JsonDataHelper.Instance.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryWeakTurnStrong);
            if (!Utils.IsListEmpty(quotes))
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgWeakTurnStrong);
                    lvi.SubItems.Add(Utils.FormatPositionForShow(item.PositionCtrl));
                    lvi.SubItems.Add(Utils.FormatMoneyForShow(item.MoneyCtrl));
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            latestStocks = quotes = JsonDataHelper.Instance.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryHitBoard);
            if (!Utils.IsListEmpty(quotes))
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgTomorrow);
                    lvi.SubItems.Add(Utils.FormatPositionForShow(item.PositionCtrl));
                    lvi.SubItems.Add(Utils.FormatMoneyForShow(item.MoneyCtrl));
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            positionStocks = quotes
                = JsonDataHelper.Instance.GetStocksByOperation(Quotes.OperationSell);
            if (!Utils.IsListEmpty(quotes))
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgPositions);
                    InitPositionItems(lvi, item);
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            PutStocksTogether();
        }

        private void BtnSwitchCancelOrders_Click(object sender, EventArgs e)
        {
            lvCancelOrders.Visible = true;
            lvPositions.Visible = false;
            mainStrategy.UpdateTotalAccountInfo(false);
        }

        private void BtnSwtichPositions_Click(object sender, EventArgs e)
        {
            lvCancelOrders.Visible = false;
            lvPositions.Visible = true;
            mainStrategy.UpdateTotalAccountInfo(false);
        }

        private void BtnUpPeriod_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> settings = JsonDataHelper.Instance.Settings;
            if (!settings.ContainsKey("period"))
            {
                settings.Add("period", "up");
            }
            if (btnPeriod.Text.Equals("上升期"))
            {
                btnPeriod.Text = "下降期";
                btnPeriod.BackColor = Color.Green;
                settings["period"] = "down";
            }
            else
            {
                btnPeriod.Text = "上升期";
                btnPeriod.BackColor = Color.Red;
                settings["period"] = "up";
            }
            JsonDataHelper.Instance.SaveSettings(settings);
        }

        private void InitializeListViews()
        {
            InitLvStocks();
            InitLvPositions();
            InitLvCancelOrders();

            ImageList imgList = new ImageList
            {
                ImageSize = new Size(1, 24)//分别是宽和高
            };
            lvStocks.SmallImageList = imgList;
            lvPositions.SmallImageList = imgList;
            lvCancelOrders.SmallImageList = imgList;
        }

        /// <summary>
        /// 可撤单
        /// </summary>
        private void InitLvCancelOrders()
        {
            lvCancelOrders.Columns.Add("股票", 100, HorizontalAlignment.Center);
            lvCancelOrders.Columns.Add("操作", 100, HorizontalAlignment.Center);
            lvCancelOrders.Columns.Add("委托数量", 100, HorizontalAlignment.Center);
            lvCancelOrders.Columns.Add("成交数量", 100, HorizontalAlignment.Center);
            lvCancelOrders.Columns.Add("委托价格", 100, HorizontalAlignment.Center);
            lvCancelOrders.Columns.Add("成交均价", 100, HorizontalAlignment.Center);
            lvCancelOrders.Columns.Add("撤单数量", 100, HorizontalAlignment.Center);
        }

        private void InitLvPositions()
        {
            lvPositions.Columns.Add("股票", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("持仓数量", 86, HorizontalAlignment.Center);
            lvPositions.Columns.Add("可用", 86, HorizontalAlignment.Center);
            lvPositions.Columns.Add("成本价", 85, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏", 86, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏比例", 85, HorizontalAlignment.Center);
            lvPositions.Columns.Add("市值", 85, HorizontalAlignment.Center);
            lvPositions.Columns.Add("仓位", 85, HorizontalAlignment.Center);
        }

        private void InitLvStocks()
        {
            ColumnHeader stock = new ColumnHeader();
            ColumnHeader position = new ColumnHeader();
            ColumnHeader money = new ColumnHeader();

            stock.Text = "股票";
            stock.Width = 120;
            stock.TextAlign = HorizontalAlignment.Center;
            position.Text = "仓位";
            position.Width = 80;
            position.TextAlign = HorizontalAlignment.Center;
            money.Text = "成交额";
            money.Width = 100;
            money.TextAlign = HorizontalAlignment.Center;

            lvStocks.Columns.Add(stock);
            lvStocks.Columns.Add(position);
            lvStocks.Columns.Add(money);
            lvStocks.MultiSelect = false;
            lvStocks.View = View.Details;
        }

        private void InitPositionItems(ListViewItem lvi, Quotes item)
        {
            if (item.IsDragonLeader)
            {
                lvi.SubItems.Add("龙头");
            }
            else if (item.ContBoards > 0)
            {
                lvi.SubItems.Add(item.ContBoards + "板");
            }
            else
            {
                if (item.StopWinPrice > 0)
                {
                    lvi.SubItems.Add("止盈：" + item.StopWinPrice);
                }
                else if (item.StopWinPct > 0)
                {
                    lvi.SubItems.Add("止盈：" + item.StopWinPct + "%");
                }
                else
                {
                    lvi.SubItems.Add(Utils.GetNameByStockCategory(item.StockCategory));
                }
            }
            if (item.AvgCost > 0)
            {
                lvi.SubItems.Add("成本：" + item.AvgCost);
            }
        }

        private void InvokeRebootStrategy()
        {
            Invoke(new MethodInvoker(RebootStrategy));
        }

        private void MainForm_Closing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认要退出策略吗？", "警告",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                mainStrategy.Stop();
                Environment.Exit(0);
            }
        }

        private void PutStocksTogether()
        {
            if (null != dragonLeaders && dragonLeaders.Count > 0)
            {
                stocks.AddRange(dragonLeaders);
            }
            if (null != latestStocks && latestStocks.Count > 0)
            {
                stocks.AddRange(latestStocks);
            }
            if (null != weakTurnStrongStocks && weakTurnStrongStocks.Count > 0)
            {
                stocks.AddRange(weakTurnStrongStocks);
            }
            if (null != bandStocks && bandStocks.Count > 0)
            {
                stocks.AddRange(bandStocks);
            }
            if (null != longTermStocks && longTermStocks.Count > 0)
            {
                stocks.AddRange(longTermStocks);
            }
            if (null != positionStocks && positionStocks.Count > 0)
            {
                stocks.AddRange(positionStocks);
            }
        }
        /// <summary>
        /// 重启策略
        /// </summary>
        private void RebootStrategy()
        {
            isRebooting = true;
            if (isStrategyStarted)
            {
                AddRuntimeInfo("重启策略中...");
                //Stop
                TsmiSwitch_Click(null, null);
                Thread.Sleep(1000);
                //Start
                TsmiSwitch_Click(null, null);
            }
            else
            {
                TsmiSwitch_Click(null, null);
            }
            isRebooting = false;
        }

        private void RefreshStocksListView()
        {
            lvStocks.BeginUpdate();
            lvStocks.Items.Clear();
            BindStocksData();
            lvStocks.EndUpdate();
        }
        /// <summary>
        /// 添加股票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmAddStock_Click(object sender, EventArgs e)
        {
            new AddStockForm(mainStrategy.GetAccounts(), this).Show();
        }

        /// <summary>
        /// 清空股票池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmClearStocks_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认要清空股票池吗？", "警告",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                lvStocks.Items.Clear();
                JsonDataHelper.Instance.ClearStocks();
                mainStrategy.ClearStocks();

                stocks.Clear();
                lvStocks.Clear();
                latestStocks.Clear();
                longTermStocks.Clear();
                dragonLeaders.Clear();
                bandStocks.Clear();
                weakTurnStrongStocks.Clear();

                AddRuntimeInfo("清空股票池");
                InitLvStocks();
                RefreshStocksListView();
            }
        }

        /// <summary>
        /// 删除股票池股票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmDelStock_Click(object sender, EventArgs e)
        {
            if (lvStocks.SelectedItems.Count <= 0)
            {
                return;
            }
            Quotes quotes = (Quotes)lvStocks.SelectedItems[0].Tag;
            latestStocks.Remove(quotes);
            weakTurnStrongStocks.Remove(quotes);
            bandStocks.Remove(quotes);
            dragonLeaders.Remove(quotes);
            longTermStocks.Remove(quotes);
            stocks.Remove(quotes);
            JsonDataHelper.Instance.DelStockByCode(quotes.Code, quotes.Operation);
            mainStrategy.DelStock(quotes);

            lvStocks.Items.Remove(lvStocks.SelectedItems[0]);
            AddRuntimeInfo("删除股票【" + quotes.Name + "】");
            mainStrategy.EditStockSub(quotes, false);
        }

        /// <summary>
        /// 股票池列表右键菜单买入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiBuy_Click(object sender, EventArgs e)
        {
            if (lvStocks.SelectedItems.Count <= 0)
            {
                return;
            }
            Quotes quotes = (Quotes)lvStocks.SelectedItems[0].Tag;
            new TradeForm(mainStrategy.GetAccounts(), quotes, this).Show();
        }

        /// <summary>
        /// 管理菜单买入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiBuyStock_Click(object sender, EventArgs e)
        {
            new TradeForm(mainStrategy.GetAccounts(), this).Show();
        }

        private void TsmiCancel_Click(object sender, EventArgs e)
        {
            if (lvCancelOrders.SelectedItems.Count <= 0)
            {
                return;
            }
            Order order = (Order)lvCancelOrders.SelectedItems[0].Tag;
            AccountHelper.CancelTotalOrders(mainStrategy.GetAccounts(), order, this);
            mainStrategy.UpdateTotalAccountInfo(false);
        }

        /// <summary>
        /// 清仓
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiClearPositions_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认要清空所有仓位吗？", "警告",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                mainStrategy.SellAll(this);
            }
        }

        private void TsmiDelGroup_Click(object sender, EventArgs e)
        {
            if (lvStocks.SelectedItems.Count <= 0)
            {
                return;
            }
            Quotes quotes = (Quotes)lvStocks.SelectedItems[0].Tag;
            JsonDataHelper.Instance.DelStocksByCategory(quotes.Operation, quotes.StockCategory);
            RefreshStocksListView();
        }

        /// <summary>
        /// 账号管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm().Show();
        }

        /// <summary>
        /// 持仓列表右键一键卖出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiOneClickSell_Click(object sender, EventArgs e)
        {
            if (lvPositions.SelectedItems.Count <= 0)
            {
                return;
            }
            Position position = (Position)lvPositions.SelectedItems[0].Tag;
            Quotes quotes = new Quotes
            {
                Code = position.Code,
                Name = position.Name
            };
            DialogResult dr = MessageBox.Show("确认要全部卖出【" + quotes.Name + "】吗？", "警告",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                mainStrategy.SellStock(quotes, this);
                mainStrategy.UpdateTotalAccountInfo(false);
            }
        }

        /// <summary>
        /// 持仓列表窗口卖出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiSellInForm_Click(object sender, EventArgs e)
        {
            if (lvPositions.SelectedItems.Count <= 0)
            {
                return;
            }
            Position position = (Position)lvPositions.SelectedItems[0].Tag;
            Quotes quotes = new Quotes
            {
                Code = position.Code,
                Name = position.Name
            };
            new TradeForm(mainStrategy.GetAccounts(), quotes, this, Order.CategorySell).Show();
        }

        /// <summary>
        /// 策略开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiSwitch_Click(object sender, EventArgs e)
        {
            if (isStrategyStarted)
            {
                AddRuntimeInfo("策略开始停止");
                tsmiSwitch.Text = "启动";
                mainStrategy.Stop();
                isStrategyStarted = false;
                AddRuntimeInfo("策略已停止");
                return;
            }
            AddRuntimeInfo("策略开始启动");
            bool isStarted = mainStrategy.Start(stocks);
            if (!isStarted)
            {
                AddRuntimeInfo("策略启动失败");
                return;
            }
            tsmiSwitch.Text = "停止";
            isStrategyStarted = true;
            AddRuntimeInfo("策略已启动");
        }

        private void TsmiTest_Click(object sender, EventArgs e)
        {
            new TestForm(this, mainStrategy.GetAccounts()).Show();
        }

        private void TspExit_Click(object sender, EventArgs e)
        {
            MainForm_Closing(null, null);
        }

        private void TspiPrivacyMode_Click(object sender, EventArgs e)
        {
            if (tspiPrivacyMode.Text.Equals("隐私模式【关】"))
            {
                tspiPrivacyMode.Text = "隐私模式【开】";
                lblMoneyAvailable.Text = "可用：***";
                lblTotalAsset.Text = "总资产：***";
                lblTotalProfit.Text = "总盈亏：***";
                int[] positionHideIndexs = new int[] { 1, 2, 4, 6 };
                foreach (ListViewItem item in lvPositions.Items)
                {
                    foreach (int i in positionHideIndexs)
                    {
                        item.SubItems[i].Text = "***";
                    }
                }
                int[] orderHideIndexs = new int[] { 1, 2, 4, 6 };
                foreach (ListViewItem item in lvCancelOrders.Items)
                {
                    foreach (int i in orderHideIndexs)
                    {
                        item.SubItems[i].Text = "***";
                    }
                }
            }
            else
            {
                tspiPrivacyMode.Text = "隐私模式【关】";
                mainStrategy.UpdateTotalAccountInfo(false);
            }
        }

        /// <summary>
        /// 更新总账户信息
        /// </summary>
        private void UpdateAcctInfo()
        {
            if (tspiPrivacyMode.Text.Equals("隐私模式【开】"))
            {
                return;
            }
            if (null != account.Funds)
            {
                lblTotalAsset.Text
                       = "总资产：" + Math.Round(account.Funds.TotalAsset / 10000, 2) + "万";
                lblMoneyAvailable.Text
                    = "可   用：" + Math.Round(account.Funds.AvailableAmt / 10000, 2) + "万";
            }
            if (!Utils.IsListEmpty(account.Positions))
            {
                BindPositionsData();
            }
            else
            {
                lvPositions.Items.Clear();
            }
            if (!Utils.IsListEmpty(account.CancelOrders))
            {
                BindCancelOrdersData();
            }
            else
            {
                lvCancelOrders.Items.Clear();
            }
        }

        private void UpdateStocksPrice(Quotes quotes)
        {
            if (null == quotes)
            {
                return;
            }
            foreach (ListViewItem item in lvStocks.Items)
            {
                if (item.Text.Contains(quotes.Name))
                {
                    double changeRatio = Math.Round((quotes.Buy1 / quotes.PreClose - 1) * 100, 1);
                    item.Text = quotes.Name + "[" + changeRatio + "%]";
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void OnPriceChange(Quotes quotes)
        {
            lvStocks.Invoke(updatePrice, quotes);
        }

        public void OnImgRotate(int degree)
        {
            try
            {
                Image image = pbWorkStatus.Image;
                if (degree > 0)
                {
                    rotateDegree += degree;
                    //RotateImage会发生异常
                    pbWorkStatus.Image = Utils.RotateImage(imgTaiJi, rotateDegree);
                }
                else
                {
                    pbWorkStatus.Image = imgTaiJi;
                }
                //imgTaiJi是原图，而且会不停地赋值给控件，不能dispose
                if (null != image && image != imgTaiJi)
                {
                    image.Dispose();
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }
    }
}