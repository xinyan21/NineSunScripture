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

        public MainForm()
        {
            InitializeComponent();
            stockDbHelper = new StockDbHelper();
            stocks = new List<Quotes>();
            mainStrategy = new MainStrategy(stocks, this);
            InitializeListViews();
            BindStocksData();
            mainStrategy.setFundListener(this);
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

            lvLongTermStocks.Columns.Add(stock);
            lvLongTermStocks.Columns.Add(position);
            lvLongTermStocks.Columns.Add(money);
            lvLongTermStocks.MultiSelect = false;
            lvLongTermStocks.GridLines = true;
            lvLongTermStocks.View = View.Details;

            stock = (ColumnHeader)stock.Clone();
            position = (ColumnHeader)position.Clone();
            money = (ColumnHeader)money.Clone();
            lvDragonLeaders.Columns.Add(stock);
            lvDragonLeaders.Columns.Add(position);
            lvDragonLeaders.Columns.Add(money);
            lvDragonLeaders.MultiSelect = false;
            lvDragonLeaders.GridLines = true;
            lvDragonLeaders.View = View.Details;

            stock = (ColumnHeader)stock.Clone();
            position = (ColumnHeader)position.Clone();
            money = (ColumnHeader)money.Clone();
            lvTomorrowStocks.Columns.Add(stock);
            lvTomorrowStocks.Columns.Add(position);
            lvTomorrowStocks.Columns.Add(money);
            lvTomorrowStocks.MultiSelect = false;
            lvTomorrowStocks.GridLines = true;
            lvTomorrowStocks.View = View.Details;

            lvPositions.Columns.Add("股票", 70, HorizontalAlignment.Center);
            lvPositions.Columns.Add("持仓数量", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("可用", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("盈亏比例", 60, HorizontalAlignment.Center);
            lvPositions.Columns.Add("市值", 100, HorizontalAlignment.Center);
            lvPositions.Columns.Add("仓位", 70, HorizontalAlignment.Center);
        }

        private void BindStocksData()
        {
            ListViewItem lvi;
            List<Quotes> quotes = stockDbHelper.GetStocksBy(Quotes.CategoryDragonLeader);
            dragonLeaders = quotes;
            if (quotes.Count > 0)
            {
                lvDragonLeaders.BeginUpdate();
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvDragonLeaders.Items.Add(lvi);
                }
                lvDragonLeaders.EndUpdate();
            }
            longTermStocks = quotes = stockDbHelper.GetStocksBy(Quotes.CategoryLongTerm);
            if (quotes.Count > 0)
            {
                lvLongTermStocks.BeginUpdate();
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvLongTermStocks.Items.Add(lvi);
                }
                lvLongTermStocks.EndUpdate();
            }
            tomorrowStocks = quotes = stockDbHelper.GetStocksBy(Quotes.CategoryTomorrow);
            if (quotes.Count > 0)
            {
                lvTomorrowStocks.BeginUpdate();
                foreach (Quotes item in quotes)
                {
                    lvi = new ListViewItem(item.Name);
                    lvi.SubItems.Add(item.PositionCtrl + "");
                    lvi.SubItems.Add(item.MoneyCtrl + "");
                    lvTomorrowStocks.Items.Add(lvi);
                }
                lvTomorrowStocks.EndUpdate();
            }
        }

        public void AddStock(Quotes quotes)
        {
            stockDbHelper.AddStock(quotes);
            ListViewItem lvi = new ListViewItem(quotes.Name);
            lvi.SubItems.Add(quotes.PositionCtrl + "");
            lvi.SubItems.Add(quotes.MoneyCtrl + "");
            lvi.Tag = quotes.Code;
            switch (quotes.StockCategory)
            {
                case Quotes.CategoryDragonLeader:
                    dragonLeaders.Add(quotes);
                    lvDragonLeaders.Items.Add(lvi);
                    break;
                case Quotes.CategoryLongTerm:
                    longTermStocks.Add(quotes);
                    lvLongTermStocks.Items.Add(lvi);
                    break;
                case Quotes.CategoryTomorrow:
                    tomorrowStocks.Add(quotes);
                    lvTomorrowStocks.Items.Add(lvi);
                    break;

                default:
                    break;
            }
        }

        private void PutStocksTogether()
        {
            stocks.Clear();
            stocks.AddRange(longTermStocks);
            stocks.AddRange(tomorrowStocks);
            stocks.AddRange(dragonLeaders);
            stocks = stocks.Distinct().ToList();
            mainStrategy.updateStocks(stocks);
        }

        public void OnTradeResult(int rspCode, string msg, string errInfo)
        {
            if (rspCode > 0)
            {
                AddRuntimeInfo(msg);
            }
            else
            {
                AddRuntimeInfo(errInfo);
            }
        }
        private void AddRuntimeInfo(string text)
        {
            tbRuntimeInfo.Text = DateTime.Now.ToString("HH:mm:ss") + " " + text + "\r\n"
                + tbRuntimeInfo.Text;
        }
        private void BtnStart_Click(object sender, EventArgs e)
        {
            PutStocksTogether();
            mainStrategy.updateStocks(stocks);
            if (isProgramStarted)
            {
                mainStrategy.Stop();
                BtnStart.Text = "启  动";
                BtnStart.BackColor = Color.Red;
                isProgramStarted = false;
                AddRuntimeInfo("策略已停止");
                return;
            }
            bool isStarted = mainStrategy.Start();
            if (!isStarted)
            {
                return;
            }
            BtnStart.Text = "停  止";
            BtnStart.BackColor = Color.DarkGray;
            isProgramStarted = true;
            AddRuntimeInfo("策略已启动");
        }

        private void MenuItemManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm().Show();
        }
        private void BtnSellAll_Click(object sender, EventArgs e)
        {
            mainStrategy.SellAll(this);
        }

        private void tspClearLongTerm_Click(object sender, EventArgs e)
        {
            lvLongTermStocks.Items.Clear();
            stockDbHelper.DelAllBy(Quotes.CategoryLongTerm);
        }

        private void tsmClearDragonLeaders_Click(object sender, EventArgs e)
        {
            lvDragonLeaders.Items.Clear();
            stockDbHelper.DelAllBy(Quotes.CategoryDragonLeader);
        }

        private void tsmClearTomorrowStocks_Click(object sender, EventArgs e)
        {
            lvTomorrowStocks.Items.Clear();
            stockDbHelper.DelAllBy(Quotes.CategoryTomorrow);
        }

        private void tspDelLongTerm_Click(object sender, EventArgs e)
        {
            if (lvLongTermStocks.SelectedItems.Count > 0)
            {
                stockDbHelper.DelStockBy(Quotes.CategoryLongTerm,
                   lvTomorrowStocks.SelectedItems[0].Tag.ToString());
                lvLongTermStocks.Items.Remove(lvLongTermStocks.SelectedItems[0]);
            }
        }
        private void tspDelDragonLeaders_Click(object sender, EventArgs e)
        {
            if (lvDragonLeaders.SelectedItems.Count > 0)
            {
                stockDbHelper.DelStockBy(Quotes.CategoryDragonLeader,
                   lvTomorrowStocks.SelectedItems[0].Tag.ToString());
                lvDragonLeaders.Items.Remove(lvDragonLeaders.SelectedItems[0]);
            }
        }
        private void tspDelTomorrowStocks_Click(object sender, EventArgs e)
        {
            if (lvTomorrowStocks.SelectedItems.Count > 0)
            {
                stockDbHelper.DelStockBy(Quotes.CategoryTomorrow,
                    lvTomorrowStocks.SelectedItems[0].Tag.ToString());
                lvTomorrowStocks.Items.Remove(lvTomorrowStocks.SelectedItems[0]);
            }
        }

        private void tspAddLongTermStock_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this, Quotes.CategoryLongTerm);
            addStockForm.Show();
        }

        private void tsmAddDragonLeader_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this, Quotes.CategoryDragonLeader);
            addStockForm.Show();
        }

        private void tsmAddTomorrowStocks_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this, Quotes.CategoryTomorrow);
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
    }

    public interface IAcctInfoListener
    {
        void onAcctInfoListen(Account account);
    }
}
