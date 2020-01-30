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
using System.Windows.Forms;

namespace NineSunScripture
{
    public partial class MainForm : Form, ITrade, IAcctInfoListener, IShowWorkingSatus
    {
        private MainStrategy mainStrategy;
        private List<Quotes> stocks;
        private List<Quotes> longTermStocks;
        private List<Quotes> dragonLeaders;
        private List<Quotes> latestStocks;
        private List<Quotes> weakTurnStrongStocks;
        private List<Quotes> bandStocks;
        //持仓股票，这个列表是用来存放卖出计划的，不是底部的那个持仓
        private List<Quotes> positionStocks;
        private Account account;//用来临时保存总账户信息
        private Image imgTaiJi;
        private bool isStrategyStarted = false;
        private int rotateDegree = 0;

        private delegate void SetTextCallback(string text);

        public MainForm()
        {
            InitializeComponent();
            stocks = new List<Quotes>();
            mainStrategy = new MainStrategy();
            InitializeListViews();
            BindStocksData();
            mainStrategy.SetFundListener(this);
            mainStrategy.SetTradeCallback(this);
            mainStrategy.SetShowWorkingStatus(this);
            imgTaiJi = Properties.Resources.taiji;

            //Start strategy
            bool isStarted = mainStrategy.Start(stocks);
            if (!isStarted)
            {
                return;
            }
            tsmiSwitch.Text = "停止";
            isStrategyStarted = true;
        }

        private void InitializeListViews()
        {
            InitLvStocks();
            InitLvPositions();
            InitLvCancelOrders();

            ImageList imgList = new ImageList
            {
                ImageSize = new Size(1, 32)//分别是宽和高
            };
            lvStocks.SmallImageList = imgList;
            lvPositions.SmallImageList = imgList;
            lvCancelOrders.SmallImageList = imgList;
        }

        private void InitLvStocks()
        {
            ColumnHeader stock = new ColumnHeader();
            ColumnHeader position = new ColumnHeader();
            ColumnHeader money = new ColumnHeader();

            stock.Text = "股票";
            stock.Width = 100;
            stock.TextAlign = HorizontalAlignment.Center;
            position.Text = "仓位";
            position.Width = 100;
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

        private void InitLvPositions()
        {
            lvPositions.Columns.Add("股票", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("持仓数量", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("可用", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏比例", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("市值", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("仓位", 100, HorizontalAlignment.Center);
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

        /// <summary>
        /// 绑定股票到listview
        /// </summary>
        private void BindStocksData()
        {
            ListViewGroup lvgDragonLeader = new ListViewGroup("龙头");
            ListViewGroup lvgLongTerm = new ListViewGroup("常驻");
            ListViewGroup lvgTomorrow = new ListViewGroup("最新");
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
            List<Quotes> quotes = JsonDataHelper.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryDragonLeader);
            dragonLeaders = quotes;
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgDragonLeader);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            longTermStocks = quotes = JsonDataHelper.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryLongTerm);
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgLongTerm);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            bandStocks = quotes = JsonDataHelper.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryBand);
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgBand);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            weakTurnStrongStocks = quotes = JsonDataHelper.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryWeakTurnStrong);
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgWeakTurnStrong);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            latestStocks = quotes = JsonDataHelper.GetStocksByCatgory(
                Quotes.OperationBuy, Quotes.CategoryLatest);
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgTomorrow);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }
            positionStocks = quotes = JsonDataHelper.GetStocksByCatgory(
               Quotes.OperationBuy, Quotes.CategoryPosition);
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgPositions);
                    lvi.SubItems.Add("卖策略1");
                    lvi.SubItems.Add("卖策略2");
                    lvi.Tag = item;
                    lvStocks.Items.Add(lvi);
                }
            }

            stocks.AddRange(dragonLeaders);
            stocks.AddRange(latestStocks);
            stocks.AddRange(weakTurnStrongStocks);
            stocks.AddRange(bandStocks);
            stocks.AddRange(longTermStocks);
            stocks.AddRange(positionStocks);
        }

        private void BindPositionsData()
        {
            if (null == account.Positions)
            {
                return;
            }
            float totalProfit = 0;
            lvPositions.BeginUpdate();
            lvPositions.Items.Clear();
            int totalPositionRatio = 0;
            foreach (Position position in account.Positions)
            {
                ListViewItem lvi = new ListViewItem(position.Name);
                lvi.SubItems.Add(position.StockBalance + "");
                lvi.SubItems.Add(position.AvailableBalance + "");
                lvi.SubItems.Add(position.ProfitAndLoss + "");
                lvi.SubItems.Add(position.ProfitAndLossPct + "%");
                lvi.SubItems.Add(position.StockBalance * position.Price + "");
                int positionRatio = (int)(position.MarketValue / account.Funds.TotalAsset * 100);
                lvi.SubItems.Add(positionRatio + "%");
                lvi.Tag = position;
                totalProfit += position.ProfitAndLoss;
                totalPositionRatio += positionRatio;

                lvPositions.Items.Add(lvi);
            }
            lvPositions.EndUpdate();
            lblTotalProfit.Text = "总盈亏：" + Math.Round(totalProfit / 10000, 2) + "万";
            lblTotalPositionRatio.Text = "总仓位：" + totalPositionRatio + "%";
        }

        private void BindCancelOrdersData()
        {
            if (null == account.CancelOrders)
            {
                return;
            }
            lvCancelOrders.BeginUpdate();
            lvCancelOrders.Items.Clear();
            foreach (Order order in account.CancelOrders)
            {
                ListViewItem lvi = new ListViewItem(order.Name);
                lvi.SubItems.Add(order.Operation + "");
                lvi.SubItems.Add(order.Quantity + "");
                lvi.SubItems.Add(order.TransactionQuantity + "");
                lvi.SubItems.Add(order.Price + "");
                lvi.SubItems.Add(order.TransactionPrice + "");
                lvi.SubItems.Add(order.CanceledQuantity + "");
                lvi.Tag = order;

                lvCancelOrders.Items.Add(lvi);
            }
            lvCancelOrders.EndUpdate();
        }

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
            JsonDataHelper.AddStock(quotes);
            string runtimeInfo = "新增股票【" + quotes.Name + "】";
            AddRuntimeInfo(runtimeInfo);
            RefreshStocksListView();
            if (!stocks.Contains(quotes))
            {
                stocks.Add(quotes);
            }
            mainStrategy.EditStockSub(quotes, true);
        }

        private void RefreshStocksListView()
        {
            lvStocks.BeginUpdate();
            lvStocks.Items.Clear();
            BindStocksData();
            lvStocks.EndUpdate();
        }

        private void InvokeRebootStrategy()
        {
            Invoke(new MethodInvoker(RebootStrategy));
        }

        /// <summary>
        /// 重启策略
        /// </summary>
        private void RebootStrategy()
        {
            if (isStrategyStarted)
            {
                string runtimeInfo = "重启策略中...";
                AddRuntimeInfo(runtimeInfo);
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
        }

        /// <summary>
        /// 交易结果回调（这里调用了成员变量，要加锁或者改成同步方法）
        /// </summary>
        /// <param name="rspCode">接口返回响应码</param>
        /// <param name="msg">正确消息</param>
        /// <param name="errInfo">错误消息</param>
        /// <param name="needReboot">是否需要重启策略</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void OnTradeResult(int rspCode, string msg, string errInfo, bool needReboot)
        {
            Logger.Log(rspCode + ">" + msg);
            if (rspCode > 0)
            {
                //mainStrategy.UpdateTotalAccountInfo(false);
                string runtimeInfo = msg + ">成功";
                AddRuntimeInfo(runtimeInfo);
            }
            else
            {
                if (string.IsNullOrEmpty(errInfo) && !string.IsNullOrEmpty(msg))
                {
                    errInfo = msg;
                }
                string runtimeInfo = msg + ">失败，错误信息：" + errInfo;
                AddRuntimeInfo(runtimeInfo);
                if (needReboot)
                {
                    InvokeRebootStrategy();
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void OnAcctInfoListen(Account account)
        {
            if (null == account)
            {
                return;
            }
            this.account = account;
            Invoke(new MethodInvoker(UpdateAcctInfo));
        }

        /// <summary>
        /// 更新总账户信息
        /// </summary>
        private void UpdateAcctInfo()
        {
            Logger.Log("Mainform UpdateAcctInfo");
            lblTotalAsset.Text
                = "总资产：" + Math.Round(account.Funds.TotalAsset / 10000, 2) + "万";
            lblMoneyAvailable.Text
                = "可   用：" + Math.Round(account.Funds.AvailableAmt / 10000, 2) + "万";
            if (account.Positions.Count > 0)
            {
                BindPositionsData();
            }
            else
            {
                lvPositions.Items.Clear();
            }
            if (account.CancelOrders.Count > 0)
            {
                BindCancelOrdersData();
            }
            else
            {
                lvCancelOrders.Items.Clear();
            }
        }

        /// <summary>
        /// 添加运行时信息
        /// </summary>
        private void AddRuntimeInfo(string text)
        {
            Logger.Log("AddRuntimeInfo-------" + text);
            if (tbRuntimeInfo.InvokeRequired)//如果调用控件的线程和创建创建控件的线程不是同一个则为True
            {
                Logger.Log("AddRuntimeInfo----InvokeRequired---");
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
                    Logger.Log("AddRuntimeInfo---- tbRuntimeInfo.Invoke---");
                }
                catch (Exception e)
                {
                    Logger.Log("AddRuntimeInfo---- tbRuntimeInfo.Invoke--Exception-" + e.Message);
                    Logger.Exception(e);
                }
            }
            else
            {
                Logger.Log(text);
                tbRuntimeInfo.AppendText(
                    DateTime.Now.ToString("HH:mm:ss") + " " + text + "\r\n");
                tbRuntimeInfo.ScrollToCaret();
            }
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
                JsonDataHelper.DelStocksByCategory(Quotes.OperationBuy, Quotes.CategoryLatest);
                JsonDataHelper.DelStocksByCategory(Quotes.OperationBuy, Quotes.CategoryLongTerm);
                JsonDataHelper.DelStocksByCategory(Quotes.OperationBuy, Quotes.CategoryBand);
                JsonDataHelper.DelStocksByCategory(
                    Quotes.OperationBuy, Quotes.CategoryWeakTurnStrong);
                JsonDataHelper.DelStocksByCategory(
                    Quotes.OperationBuy, Quotes.CategoryDragonLeader);

                stocks.Clear();
                lvStocks.Clear();
                latestStocks.Clear();
                longTermStocks.Clear();
                dragonLeaders.Clear();

                string runtimeInfo = "清空股票池";
                AddRuntimeInfo(runtimeInfo);
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

            lvStocks.Items.Remove(lvStocks.SelectedItems[0]);
            string runtimeInfo = "删除股票【" + quotes.Name + "】";
            AddRuntimeInfo(runtimeInfo);
            RefreshStocksListView();
            mainStrategy.EditStockSub(quotes, false);
            if (stocks.Contains(quotes))
            {
                stocks.Remove(quotes);
            }
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
        /// 账号管理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm().Show();
        }

        /// <summary>
        /// 策略开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiSwitch_Click(object sender, EventArgs e)
        {
            string runtimeInfo = "";
            if (isStrategyStarted)
            {
                runtimeInfo = "策略开始停止";
                AddRuntimeInfo(runtimeInfo);
                mainStrategy.Stop();
                tsmiSwitch.Text = "启动";
                isStrategyStarted = false;
                runtimeInfo = "策略已停止";
                AddRuntimeInfo(runtimeInfo);
                return;
            }
            runtimeInfo = "策略开始启动";
            AddRuntimeInfo(runtimeInfo);
            bool isStarted = mainStrategy.Start(stocks);
            if (!isStarted)
            {
                string log = "策略启动失败";
                runtimeInfo = log;
                AddRuntimeInfo(runtimeInfo);
                return;
            }
            tsmiSwitch.Text = "停止";
            isStrategyStarted = true;
            runtimeInfo = "策略已启动";
            AddRuntimeInfo(runtimeInfo);
        }

        /// <summary>
        /// 清仓
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiClearPositions_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认要清仓吗？", "警告",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                mainStrategy.SellAll(this);
            }
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
        /// 持仓列表右键一键卖出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TsmiSell_Click(object sender, EventArgs e)
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
                //mainStrategy.UpdateTotalAccountInfo(false);
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

        private void TsmiTest_Click(object sender, EventArgs e)
        {
            new TestForm(this, mainStrategy.GetAccounts()).Show();
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

        private void TspExit_Click(object sender, EventArgs e)
        {
            MainForm_Closing(null, null);
        }

        public void RotateStatusImg(int degree)
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

        private void BtnSwtichPositions_Click(object sender, EventArgs e)
        {
            lvCancelOrders.Visible = false;
            lvPositions.Visible = true;
            mainStrategy.UpdateTotalAccountInfo(false);
        }

        private void BtnSwitchCancelOrders_Click(object sender, EventArgs e)
        {
            lvCancelOrders.Visible = true;
            lvPositions.Visible = false;
            mainStrategy.UpdateTotalAccountInfo(false);
        }

        private void TspiPrivacyMode_Click(object sender, EventArgs e)
        {
            if (tspiPrivacyMode.Text.Equals("隐私模式"))
            {
                tspiPrivacyMode.Text = "隐私模式【开】";
                flpStockPool.Visible = false;
                panelFundInfo.Visible = false;
            }
            else
            {
                tspiPrivacyMode.Text = "隐私模式";
                flpStockPool.Visible = true;
                panelFundInfo.Visible = true;
            }
        }
    }

    public interface IAcctInfoListener
    {
        void OnAcctInfoListen(Account account);
    }

    public interface IShowWorkingSatus
    {
        void RotateStatusImg(int degree);
    }
}