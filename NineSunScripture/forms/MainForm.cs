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
        private StockDbHelper stockDbHelper;
        private Account account;//用来临时保存总账户信息
        private Image imgTaiJi;
        private bool isStrategyStarted = false;
        private string runtimeInfo;
        private int rotateDegree = 0;

        public MainForm()
        {
            InitializeComponent();
            /* if (!Utils.DetectTHSDll())
             {
                 MessageBox.Show("dll不存在，不能启动策略->" + System.Environment.CurrentDirectory);
                 return;
             }*/

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
        }

        private void InitializeListViews()
        {
            InitLvStocks();
            InitLvPositions();
            InitLvCancelOrders();

            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 32);//分别是宽和高
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
        /// 可撤单和持仓公用一个ListView
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
            lvStocks.Groups.Add(lvgDragonLeader);
            lvStocks.Groups.Add(lvgLongTerm);
            lvStocks.Groups.Add(lvgTomorrow);
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
            foreach (Position position in account.Positions)
            {
                ListViewItem lvi = new ListViewItem(position.Name);
                lvi.SubItems.Add(position.StockBalance + "");
                lvi.SubItems.Add(position.AvailableBalance + "");
                lvi.SubItems.Add(position.ProfitAndLoss + "");
                lvi.SubItems.Add(position.ProfitAndLossPct + "%");
                lvi.SubItems.Add(position.StockBalance * position.Price + "");
                int positionRatio = (int)(position.StockBalance * position.Price
                    / account.Funds.TotalAsset * 100);
                lvi.SubItems.Add(positionRatio + "%");
                lvi.Tag = position;
                totalProfit += position.ProfitAndLoss;

                lvPositions.Items.Add(lvi);
            }
            lvPositions.EndUpdate();
            lblTotalProfit.Text = "总盈亏\n" + ((int)totalProfit).ToString();
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
            if (mainStrategy.IsTradeTime())
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

            mainStrategy.UpdateStocks(stocks);
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
            lblTotalAsset.Text = "总资产\n" + account.Funds.TotalAsset;
            lblMoneyAvailable.Text = "可用金额\n" + account.Funds.AvailableAmt;
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
                if (mainStrategy.IsTradeTime())
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
            if (mainStrategy.IsTradeTime())
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
            mainStrategy.UpdateStocks(stocks);
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
            string code = quotes.Code;
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
            Quotes quotes = new Quotes();
            quotes.Code = position.Code;
            quotes.Name = position.Name;
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
            Quotes quotes = new Quotes();
            quotes.Code = position.Code;
            quotes.Name = position.Name;
            string code = quotes.Code;
            new TradeForm(mainStrategy.GetAccounts(), quotes, this, Order.CategorySell).Show();
        }

        private void TsmiTest_Click(object sender, EventArgs e)
        {
            new TestForm(mainStrategy.GetAccounts()).Show();
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
        void OnAcctInfoListen(Account account);
    }

    public interface IShowWorkingSatus
    {
        void RotateStatusImg(int degree);
    }
}
