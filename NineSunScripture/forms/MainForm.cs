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

namespace NineSunScripture
{
    public partial class MainForm : Form, ITrade
    {
        public const int CategoryLongTermStocks = 0;
        public const int CategoryDragonLeaders = 1;
        public const int CategoryTomorrowStocks = 2;
        private List<Account> accounts;
        private MainStrategy mainStrategy;
        private List<Quotes> stocks;
        private List<Quotes> longTermStocks;
        private List<Quotes> dragonLeaders;
        private List<Quotes> tomorrowStocks;
        private bool isProgramStarted = false;

        public MainForm()
        {
            InitializeComponent();
            accounts = new List<Account>();
            stocks = new List<Quotes>();
            longTermStocks = new List<Quotes>();
            dragonLeaders = new List<Quotes>();
            tomorrowStocks = new List<Quotes>();
            mainStrategy = new MainStrategy(accounts, stocks, this);
            InitializeListViews();
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
            lvLongTermStocks.HoverSelection = true;
            lvLongTermStocks.View = View.Details;

            stock = (ColumnHeader)stock.Clone();
            position = (ColumnHeader)position.Clone();
            money = (ColumnHeader)money.Clone();
            lvDragonLeaders.Columns.Add(stock);
            lvDragonLeaders.Columns.Add(position);
            lvDragonLeaders.Columns.Add(money);
            lvDragonLeaders.MultiSelect = false;
            lvDragonLeaders.GridLines = true;
            lvDragonLeaders.HoverSelection = true;
            lvDragonLeaders.View = View.Details;

            stock = (ColumnHeader)stock.Clone();
            position = (ColumnHeader)position.Clone();
            money = (ColumnHeader)money.Clone();
            lvTomorrowStocks.Columns.Add(stock);
            lvTomorrowStocks.Columns.Add(position);
            lvTomorrowStocks.Columns.Add(money);
            lvTomorrowStocks.MultiSelect = false;
            lvTomorrowStocks.GridLines = true;
            lvTomorrowStocks.HoverSelection = true;
            lvTomorrowStocks.View = View.Details;
        }

        public void AddStock(int category, Quotes quotes)
        {
            ListViewItem lvi = new ListViewItem(quotes.Name);
            lvi.SubItems.Add(quotes.positionCtrl + "");
            lvi.SubItems.Add(quotes.moneyCtrl + "");
            switch (category)
            {
                case CategoryDragonLeaders:
                    dragonLeaders.Add(quotes);
                    lvDragonLeaders.Items.Add(lvi);
                    break;
                case CategoryLongTermStocks:
                    longTermStocks.Add(quotes);
                    lvLongTermStocks.Items.Add(lvi);
                    break;
                case CategoryTomorrowStocks:
                    tomorrowStocks.Add(quotes);
                    lvTomorrowStocks.Items.Add(lvi);
                    break;

                default:
                    break;
            }
        }

        public void AddAccount(Account account)
        {
            accounts.Add(account);
        }

        private void TestDll()
        {
            int Qsid = 59;
            string Host = "101.89.64.194";
            short Port = 8003;
            string Version = "E065.20.92";
            short AccountType = 0x30;
            //string Account = "49758842";
            //string password = "13231178";
            string Account = "321019194496";
            string password = "198921";
            string comm_password = "";

            byte[] ErrInfo = new byte[256];
            byte[] Result = new byte[1024 * 1024];//  不写大点内存溢出会崩溃
            Console.WriteLine("\nstart to login");
            int clientId = TradeAPI.Logon(Qsid, Host, Port, Version, AccountType, Account,
                password, comm_password, false, ErrInfo);
            if (clientId > 0)
            {
                //Funds funds = TradeAPI.QueryFunds(clientId);
                //TradeAPI.QueryQuotes(clientId, "600686");
                int rspCode = TradeAPI.QueryData(clientId, 4, Result, ErrInfo);
                if (rspCode > 0)
                {
                }
                else
                {
                    MessageBox.Show(Encoding.Default.GetString(ErrInfo).TrimEnd('\0'));
                }
            }
            else
            {
            }
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
            tbRuntimeInfo.Text = DateTime.Now.ToString("HH:mm:ss") + " " + text+ "\r\n"
                + tbRuntimeInfo.Text;
        }
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (isProgramStarted)
            {
                mainStrategy.Stop();
                BtnStart.Text = "启  动";
                BtnStart.BackColor = Color.Red;
                isProgramStarted = false;
                AddRuntimeInfo("策略已停止");
                return;
            }
            mainStrategy.Start();
            BtnStart.Text = "停  止";
            BtnStart.BackColor = Color.DarkGray;
            isProgramStarted = true;
            AddRuntimeInfo("策略已启动");
        }

        private void MenuItemManageAcct_Click(object sender, EventArgs e)
        {
            new ManageAcctForm(this).Show();
        }
        private void BtnSellAll_Click(object sender, EventArgs e)
        {
            SellStrategy.SellAll(accounts, this);
        }

        private void tspClearLongTerm_Click(object sender, EventArgs e)
        {
            lvLongTermStocks.Items.Clear();
        }

        private void tsmClearDragonLeaders_Click(object sender, EventArgs e)
        {
            lvDragonLeaders.Items.Clear();
        }

        private void tsmClearTomorrowStocks_Click(object sender, EventArgs e)
        {
            lvTomorrowStocks.Items.Clear();
        }

        private void tspDelLongTerm_Click(object sender, EventArgs e)
        {
            if (lvLongTermStocks.SelectedItems.Count > 0)
            {
                lvLongTermStocks.Items.Remove(lvLongTermStocks.SelectedItems[0]);
            }
        }
        private void tspDelDragonLeaders_Click(object sender, EventArgs e)
        {
            if (lvDragonLeaders.SelectedItems.Count > 0)
            {
                lvDragonLeaders.Items.Remove(lvDragonLeaders.SelectedItems[0]);
            }
        }
        private void tspDelTomorrowStocks_Click(object sender, EventArgs e)
        {
            if (lvTomorrowStocks.SelectedItems.Count > 0)
            {
                lvTomorrowStocks.Items.Remove(lvTomorrowStocks.SelectedItems[0]);
            }
        }

        private void tspAddLongTermStock_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this, CategoryLongTermStocks);
            addStockForm.Show();
        }

        private void tsmAddDragonLeader_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this, CategoryDragonLeaders);
            addStockForm.Show();
        }

        private void tsmAddTomorrowStocks_Click(object sender, EventArgs e)
        {
            AddStockForm addStockForm = new AddStockForm(this, CategoryTomorrowStocks);
            addStockForm.Show();
        }
    }
}
