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

namespace NineSunScripture
{
    public partial class MainForm : Form, ITrade, IAcctInfoListener
    {
        private MainStrategy mainStrategy;
        private List<Quotes> stocks;
        private List<Quotes> longTermStocks;
        private List<Quotes> dragonLeaders;
        private List<Quotes> tomorrowStocks;
        private bool isProgramStarted = false;
        private StockDbHelper stockDbHelper;
        private Account account;
        private string runtimeInfo;

        public MainForm()
        {
            InitializeComponent();
            stockDbHelper = new StockDbHelper();
            stocks = new List<Quotes>();
            mainStrategy = new MainStrategy();
            InitializeListViews();
            BindStocksData();
            mainStrategy.setFundListener(this);
            mainStrategy.setTradeCallback(this);
            Thread.Sleep(3000);
            //程序启动自动启动策略
            tsmiSwitch_Click(null, null);
        }

        private void InitializeListViews()
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

            stock = (ColumnHeader)stock.Clone();
            position = (ColumnHeader)position.Clone();
            money = (ColumnHeader)money.Clone();
            lvStocks.Columns.Add(stock);
            lvStocks.Columns.Add(position);
            lvStocks.Columns.Add(money);
            lvStocks.MultiSelect = false;
            lvStocks.View = View.Details;

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
                    lvi.Tag = item.Code;
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
                    lvi.Tag = item.Code;
                    lvStocks.Items.Add(lvi);
                }
            }
            tomorrowStocks = quotes = stockDbHelper.GetStocksBy(Quotes.CategoryLatest);
            if (quotes.Count > 0)
            {
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name, lvgTomorrow);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvi.Tag = item.Code;
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
            if (tomorrowStocks.Count > 0)
            {
                stocks.AddRange(tomorrowStocks);
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

            mainStrategy.updateStocks(stocks);
        }

        public void OnTradeResult(int rspCode, string msg, string errInfo)
        {
            if (rspCode > 0)
            {
                runtimeInfo = msg + ">成功";
            }
            else
            {
                runtimeInfo = msg + ">失败，错误信息：" + errInfo;
            }
            Invoke(new MethodInvoker(AddRuntimeInfo));
        }
        private void AddRuntimeInfo()
        {
            tbRuntimeInfo.AppendText(DateTime.Now.ToString("HH:mm:ss") + " " + runtimeInfo + "\r\n");
            tbRuntimeInfo.ScrollToCaret();
        }

        private void MenuItemManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm().Show();
        }

        private void tsmClearStocks_Click(object sender, EventArgs e)
        {
            lvStocks.Items.Clear();
            stockDbHelper.DelAllBy(Quotes.CategoryLatest);
            stockDbHelper.DelAllBy(Quotes.CategoryLongTerm);
            stockDbHelper.DelAllBy(Quotes.CategoryDragonLeader);
        }

        private void tsmDelStock_Click(object sender, EventArgs e)
        {
            if (lvStocks.SelectedItems.Count <= 0)
            {
                return;
            }
            string strCategory = lvStocks.SelectedItems[0].Group.Header;
            short category = Quotes.CategoryLatest;
            if (strCategory == "龙头")
            {
                category = Quotes.CategoryDragonLeader;
            }
            else if (strCategory == "常驻")
            {
                category = Quotes.CategoryLongTerm;
            }
            stockDbHelper.DelStockBy(category, lvStocks.SelectedItems[0].Tag.ToString());
            lvStocks.Items.Remove(lvStocks.SelectedItems[0]);
        }

        private void tsmAddStock_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this);
            addStockForm.Show();
        }

        public void onAcctInfoListen(Account account)
        {
            this.account = account;
            Invoke(new MethodInvoker(UpdateAcctInfo));
        }
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
                lvi.SubItems.Add(position.QuantityBalance + "");
                lvi.SubItems.Add(position.AvailableQuantity + "");
                lvi.SubItems.Add(position.ProfitAndLoss + "");
                lvi.SubItems.Add(position.ProfitAndLossPct + "%");
                lvi.SubItems.Add(position.QuantityBalance * position.Price + "");
                int positionRatio = (int)(position.QuantityBalance * position.Price
                    / account.Funds.TotalAsset * 100);
                lvi.SubItems.Add(positionRatio + "%");

                lvPositions.Items.Add(lvi);
            }
            lvPositions.EndUpdate();
        }

        private void tspManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm().Show();
        }

        private void tsmiSwitch_Click(object sender, EventArgs e)
        {
            runtimeInfo = "策略开始启动";
            AddRuntimeInfo();
            Logger.log(runtimeInfo);
            if (isProgramStarted)
            {
                mainStrategy.Stop();
                tsmiSwitch.Text = "启动";
                isProgramStarted = false;
                runtimeInfo = "策略已停止";
                AddRuntimeInfo();
                return;
            }
            PutStocksTogether();
            mainStrategy.updateStocks(stocks);
            bool isStarted = mainStrategy.Start();
            if (!isStarted)
            {
                string log = "策略启动失败";
                runtimeInfo = log;
                AddRuntimeInfo();
                Logger.log(log);
                return;
            }
            tsmiSwitch.Text = "停止";
            isProgramStarted = true;
            runtimeInfo = "策略已启动";
            AddRuntimeInfo();
        }

        private void tsmiClearPositions_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确认要清仓吗？", "警告",
                MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                mainStrategy.SellAll(this);
            }
        }
    }

    public interface IAcctInfoListener
    {
        void onAcctInfoListen(Account account);
    }
}
