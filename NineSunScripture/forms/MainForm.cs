using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NineSunScripture.trade.api;
using NineSunScripture.model;
using NineSunScripture.forms;
using NineSunScripture.trade.helper;
using NineSunScripture.strategy;
using NineSunScripture.db;
using NineSunScripture.util.log;
using System.Threading;
using NineSunScripture.util.test;
using NineSunScripture.util;

namespace NineSunScripture
{
    public partial class MainForm : Form, ITrade, IAcctInfoListener, IShowWorkingSatus
    {
        private MainStrategy mainStrategy;
        private List<Quotes> stocks;
        private List<Quotes> longTermStocks;
        private List<Quotes> dragonLeaders;
        private List<Quotes> latestStocks;
        private bool isStrategyStarted = false;
        private StockDbHelper stockDbHelper;
        private Account account;//用来临时保存总账户信息
        private string runtimeInfo;
        private Image imgTaiJi;
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

            new Thread(InvokeRebootStrategy).Start();
        }

        private void InitializeListViews()
        {
            InitLvStocksHeader();

            lvPositions.Columns.Add("股票", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("持仓数量", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("可用", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏比例", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("市值", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("仓位", 100, HorizontalAlignment.Center);

            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 32);//分别是宽和高
            lvStocks.SmallImageList = imgList;
            lvPositions.SmallImageList = imgList;
        }

        private void InitLvStocksHeader()
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

        /// <summary>
        /// 添加股票
        /// </summary>
        /// <param name="quotes">股票对象</param>
        public void AddStock(Quotes quotes)
        {
            stockDbHelper.AddStock(quotes);
            refreshStocksListView();
            RebootStrategy();
        }

        private void refreshStocksListView()
        {
            lvStocks.BeginUpdate();
            lvStocks.Items.Clear();
            BindStocksData();
            lvStocks.EndUpdate();
        }

        /// <summary>
        /// 汇总股票池=常驻+最新，龙头不需要加进去，因为龙头是用来指导卖点的，只有持仓里有才会设置
        /// </summary>
        private void PutStocksTogether()
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

            mainStrategy.UpdateStocks(stocks);
        }

        private void InvokeAddRunInfo()
        {
            Invoke(new MethodInvoker(AddRuntimeInfo));
        }

        private void InvokeRebootStrategy()
        {
            Thread.Sleep(1000);
            Invoke(new MethodInvoker(RebootStrategy));
        }

        private void RebootStrategy()
        {
            InvokeAddRunInfo();
            if (isStrategyStarted)
            {
                runtimeInfo = "股票池变更，重启策略中...";
                tsmiSwitch_Click(null, null);
                Thread.Sleep(1000);
                tsmiSwitch_Click(null, null);
            }
            else
            {
                tsmiSwitch_Click(null, null);
            }
        }

        public void OnTradeResult(int rspCode, string msg, string errInfo)
        {
            if (rspCode > 0)
            {
                mainStrategy.UpdateFundsInfo(false);
                runtimeInfo = msg + ">成功";
            }
            else
            {
                runtimeInfo = msg + ">失败，错误信息：" + errInfo;
                if (errInfo.Contains("超时"))
                {
                    InvokeRebootStrategy();
                }
            }
            InvokeAddRunInfo();
        }

        /// <summary>
        /// 更新总账户信息
        /// </summary>
        private void UpdateAcctInfo()
        {
            lblTotalAsset.Text = "总资产：\n" + account.Funds.TotalAsset;
            lblMoneyAvailable.Text = "可用金额：\n" + account.Funds.AvailableAmt;
            if (account.Positions.Count == 0)
            {
                return;
            }
            lvPositions.BeginUpdate();
            lvPositions.Items.Clear();
            foreach (Position position in account.Positions)
            {
                //TODO 模拟交易代码
                if (MainStrategy.ReserveStocks.Contains(position.Code))
                {
                    continue;
                }
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

                lvPositions.Items.Add(lvi);
            }
            lvPositions.EndUpdate();
        }

        /// <summary>
        /// 添加运行时信息
        /// </summary>
        private void AddRuntimeInfo()
        {
            Logger.log(runtimeInfo);
            tbRuntimeInfo.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + runtimeInfo + "\r\n");
            tbRuntimeInfo.ScrollToCaret();
        }

        /// <summary>
        /// 清空股票池
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmClearStocks_Click(object sender, EventArgs e)
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

                InitLvStocksHeader();
                refreshStocksListView();
                RebootStrategy();
            }
        }

        /// <summary>
        /// 删除股票池股票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmDelStock_Click(object sender, EventArgs e)
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
            refreshStocksListView();
            RebootStrategy();
        }

        /// <summary>
        /// 添加股票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmAddStock_Click(object sender, EventArgs e)
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
        private void tsmiManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm().Show();
        }

        /// <summary>
        /// 策略开关
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiSwitch_Click(object sender, EventArgs e)
        {
            if (isStrategyStarted)
            {
                runtimeInfo = "策略开始停止";
                InvokeAddRunInfo();
                Logger.log(runtimeInfo);
                mainStrategy.Stop();
                tsmiSwitch.Text = "启动";
                isStrategyStarted = false;
                runtimeInfo = "策略已停止";
                InvokeAddRunInfo();
                return;
            }
            runtimeInfo = "策略开始启动";
            InvokeAddRunInfo();
            Logger.log(runtimeInfo);
            PutStocksTogether();
            mainStrategy.UpdateStocks(stocks);
            bool isStarted = mainStrategy.Start();
            if (!isStarted)
            {
                string log = "策略启动失败";
                runtimeInfo = log;
                InvokeAddRunInfo();
                Logger.log(log);
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
        private void tsmiClearPositions_Click(object sender, EventArgs e)
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
        private void tsmiBuyStock_Click(object sender, EventArgs e)
        {
            new TradeForm(mainStrategy.GetAccounts(), this).Show();
        }
        /// <summary>
        /// 股票池列表右键菜单买入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiBuy_Click(object sender, EventArgs e)
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
        /// 持仓列表右键卖出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiSell_Click(object sender, EventArgs e)
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
            }
        }

        /// <summary>
        /// 持仓列表窗口卖出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiSellInForm_Click(object sender, EventArgs e)
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

        private void tsmiTest_Click(object sender, EventArgs e)
        {
            new TestForm().Show();
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

        private void tspExit_Click(object sender, EventArgs e)
        {
            MainForm_Closing(null, null);
        }

        public void RotateStatusImg()
        {
            Image image = pbWorkStatus.Image;
            pbWorkStatus.Image = Utils.RotateImage(imgTaiJi, rotateDegree++);
            if (null != image)
            {
                image.Dispose();
            }
        }
    }

    public interface IAcctInfoListener
    {
        void OnAcctInfoListen(Account account);
    }

    public interface IShowWorkingSatus
    {
        void RotateStatusImg();
    }
}
