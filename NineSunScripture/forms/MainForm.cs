using NineSunScripture.db;
using NineSunScripture.forms;
using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.helper;
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
        private StockDbHelper stockDbHelper;
        private Account account;//用来临时保存总账户信息
        private Image imgTaiJi;
        private bool isStrategyStarted = false;
        private string runtimeInfo;
        private int rotateDegree = 0;

        public MainForm()
        {
            InitializeComponent();
            stockDbHelper = new StockDbHelper();
            stocks = new List<Quotes>();
            mainStrategy = new MainStrategy();
            InitializeListViews();
            BindStocksData();
            mainStrategy.SetFundListener(this);
            mainStrategy.SetTradeCallback(this);
            mainStrategy.SetShowWorkingStatus(this);
            imgTaiJi = Properties.Resources.taiji;

            //Start strategy
            UpdateStrategyStocks();
            bool isStarted = mainStrategy.Start();
            if (!isStarted)
            {
                return;
            }
            tsmiSwitch.Text = "停止";
            isStrategyStarted = true;
            //默认开启隐私模式
            tspiPrivacyMode.Text = "隐私模式【开】";
            flpStockPool.Visible = false;
            panelFundInfo.Visible = false;
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
            lvCancelOrders.Columns.Add("撤销数量", 100, HorizontalAlignment.Center);
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
            lvStocks.Groups.Add(lvgDragonLeader);
            lvStocks.Groups.Add(lvgLongTerm);
            lvStocks.Groups.Add(lvgTomorrow);
            lvStocks.Groups.Add(lvgWeakTurnStrong);
            lvStocks.Groups.Add(lvgBand);
            ListViewItem lvi;
            List<Quotes> quotes = stockDbHelper.GetStocksBy(Quotes.CategoryDragonLeader);
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
            longTermStocks = quotes = stockDbHelper.GetStocksBy(Quotes.CategoryLongTerm);
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
            bandStocks = quotes = stockDbHelper.GetStocksBy(Quotes.CategoryBand);
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
            weakTurnStrongStocks =
                quotes = stockDbHelper.GetStocksBy(Quotes.CategoryWeakTurnStrong);
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
            latestStocks = quotes = stockDbHelper.GetStocksBy(Quotes.CategoryLatest);
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
            stockDbHelper.AddStock(quotes);
            runtimeInfo = "新增股票【" + quotes.Name + "】";
            InvokeAddRunInfo();
            RefreshStocksListView();
            if (isStrategyStarted && mainStrategy.IsTradeTime())
            {
                RebootStrategy();
            }
            else
            {
                UpdateStrategyStocks();
            }
        }

        private void RefreshStocksListView()
        {
            lvStocks.BeginUpdate();
            lvStocks.Items.Clear();
            BindStocksData();
            lvStocks.EndUpdate();
        }

        /// <summary>
        /// 汇总股票池
        /// </summary>
        private void UpdateStrategyStocks()
        {
            stocks.Clear();
            if (latestStocks.Count > 0)
            {
                stocks.AddRange(latestStocks);
                if (longTermStocks.Count > 0)
                {
                    foreach (Quotes quotes in longTermStocks)
                    {
                        if (!stocks.Contains(quotes))
                        {
                            stocks.Add(quotes);
                        }
                    }
                }
            }
            else if (longTermStocks.Count > 0)
            {
                stocks.AddRange(longTermStocks);
            }
            if (dragonLeaders.Count > 0)
            {
                mainStrategy.SetDragonLeaders(dragonLeaders);
            }

            mainStrategy.UpdateStocks(stocks, weakTurnStrongStocks, bandStocks);
        }

        private void InvokeAddRunInfo()
        {
            Invoke(new MethodInvoker(AddRuntimeInfo));
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
                runtimeInfo = "重启策略中...";
                InvokeAddRunInfo();
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
            if (rspCode > 0)
            {
                mainStrategy.UpdateTotalAccountInfo(false);
                runtimeInfo = msg + ">成功";
                InvokeAddRunInfo();
            }
            else
            {
                if (string.IsNullOrEmpty(errInfo) && !string.IsNullOrEmpty(msg))
                {
                    errInfo = msg;
                }
                runtimeInfo = msg + ">失败，错误信息：" + errInfo;
                InvokeAddRunInfo();
                if (needReboot)
                {
                    InvokeRebootStrategy();
                }
            }
        }

        /// <summary>
        /// 更新总账户信息
        /// </summary>
        private void UpdateAcctInfo()
        {
            lblTotalAsset.Text = "总资产：" + Math.Round(account.Funds.TotalAsset / 10000, 2) + "万";
            lblMoneyAvailable.Text
                = "可  用：" + Math.Round(account.Funds.AvailableAmt / 10000, 2) + "万";
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
        private void AddRuntimeInfo()
        {
            Logger.Log(runtimeInfo);
            tbRuntimeInfo.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + runtimeInfo + "\r\n");
            tbRuntimeInfo.ScrollToCaret();
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
                stockDbHelper.DelAllBy(Quotes.CategoryLatest);
                stockDbHelper.DelAllBy(Quotes.CategoryLongTerm);
                stockDbHelper.DelAllBy(Quotes.CategoryDragonLeader);

                stocks.Clear();
                lvStocks.Clear();
                latestStocks.Clear();
                longTermStocks.Clear();
                dragonLeaders.Clear();

                runtimeInfo = "清空股票池";
                InvokeAddRunInfo();
                InitLvStocks();
                RefreshStocksListView();
                if (isStrategyStarted && mainStrategy.IsTradeTime())
                {
                    RebootStrategy();
                }
                else
                {
                    UpdateStrategyStocks();
                }
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
            string strCategory = lvStocks.SelectedItems[0].Group.Header;
            short category = Quotes.CategoryLatest;
            Quotes quotes = (Quotes)lvStocks.SelectedItems[0].Tag;
            if (strCategory == "龙头")
            {
                category = Quotes.CategoryDragonLeader;
                dragonLeaders.Remove(quotes);
            }
            else if (strCategory == "常驻")
            {
                category = Quotes.CategoryLongTerm;
                longTermStocks.Remove(quotes);
            }
            else
            {
                latestStocks.Remove(quotes);
            }
            stockDbHelper.DelStockBy(category, quotes.Code);
            lvStocks.Items.Remove(lvStocks.SelectedItems[0]);
            runtimeInfo = "删除股票【" + quotes.Name + "】";
            InvokeAddRunInfo();
            RefreshStocksListView();
            if (isStrategyStarted && mainStrategy.IsTradeTime())
            {
                RebootStrategy();
            }
            else
            {
                UpdateStrategyStocks();
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

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void OnAcctInfoListen(Account account)
        {
            this.account = account;
            Invoke(new MethodInvoker(UpdateAcctInfo));
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
            if (isStrategyStarted)
            {
                runtimeInfo = "策略开始停止";
                InvokeAddRunInfo();
                mainStrategy.Stop();
                tsmiSwitch.Text = "启动";
                isStrategyStarted = false;
                runtimeInfo = "策略已停止";
                InvokeAddRunInfo();
                return;
            }
            runtimeInfo = "策略开始启动";
            InvokeAddRunInfo();
            UpdateStrategyStocks();
            mainStrategy.UpdateStocks(stocks, weakTurnStrongStocks, bandStocks);
            bool isStarted = mainStrategy.Start();
            if (!isStarted)
            {
                string log = "策略启动失败";
                runtimeInfo = log;
                InvokeAddRunInfo();
                return;
            }
            tsmiSwitch.Text = "停止";
            isStrategyStarted = true;
            runtimeInfo = "策略已启动";
            InvokeAddRunInfo();
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
                Environment.Exit(0);
            }
        }

        private void TspExit_Click(object sender, EventArgs e)
        {
            MainForm_Closing(null, null);
        }

        public void RotateStatusImg(int degree)
        {
            Image image = pbWorkStatus.Image;
            if (degree > 0)
            {
                rotateDegree += degree;
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
        [MethodImpl(MethodImplOptions.Synchronized)]
        void OnAcctInfoListen(Account account);
    }

    public interface IShowWorkingSatus
    {
        void RotateStatusImg(int degree);
    }
}
