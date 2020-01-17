using Newtonsoft.Json;
using NineSunScripture.model;
using NineSunScripture.trade.structApi.api;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.util.test
{
    public partial class TestForm : Form
    {
        private TradeTestCase testTrade = new TradeTestCase();
        private List<Account> accounts;
        Quotes quotes;
        private MainForm mainForm;
        public TestForm(MainForm main, List<Account> accounts)
        {
            InitializeComponent();
            this.accounts = accounts;
            this.mainForm = main;
        }

        private void TestBuyStrategy()
        {
            testTrade.TestBuyStrategy(accounts, mainForm);
        }

        private void btnTestBuy_Click(object sender, EventArgs e)
        {
            new Thread(TestBuyStrategy).Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            testTrade.TestSellStrategy(accounts);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Order> todayTransactions = TradeAPI.QueryTodayTransaction(accounts[0].TradeSessionId);
        }

        private void btnTenthGearPrice_Click(object sender, EventArgs e)
        {
            new Thread(Work).Start();
        }

        private void Work()
        {
            try
            {
                /* for (int i = 0; i < 1; i++)
                 {
                     quotes = PriceAPI.QueryTenthGearPrice(accounts[0].PriceSessionId, "002071");
                     Invoke(new MethodInvoker(UpdatePrice));
                     Thread.Sleep(200);
                 }*/
                quotes = PriceAPI.QueryTenthGearPrice(accounts[0].PriceSessionId, "002071");
                Invoke(new MethodInvoker(UpdatePrice));

                quotes = PriceAPI.QueryTenthGearPrice(accounts[0].PriceSessionId, "002071");
                Invoke(new MethodInvoker(UpdatePrice));
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        private void UpdatePrice()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("卖三 ").Append(quotes.Sell3).Append(" ").Append(quotes.Sell3Vol).Append("\n");
            sb.Append("卖二 ").Append(quotes.Sell2).Append(" ").Append(quotes.Sell2Vol).Append("\n");
            sb.Append("卖一 ").Append(quotes.Sell1).Append(" ").Append(quotes.Sell1Vol).Append("\n");
            sb.Append("买一 ").Append(quotes.Buy1).Append(" ").Append(quotes.Buy1Vol).Append("\n");
            sb.Append("买二 ").Append(quotes.Buy2).Append(" ").Append(quotes.Buy2Vol).Append("\n");
            sb.Append("买三 ").Append(quotes.Buy3).Append(" ").Append(quotes.Buy3Vol).Append("\n");
            lblTenthGearPrice.Text = sb.ToString();
        }

        private void btnTestRRB_Click(object sender, EventArgs e)
        {
            testTrade.TestSell(accounts);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                mainForm.OnTradeResult(0, "Test reboot1", "Test reboot", true);
            });
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                mainForm.OnTradeResult(0, "Test reboot2", "Test reboot", false);
            });
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                mainForm.OnTradeResult(0, "Test reboot3", "Test reboot", false);
            });
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                mainForm.OnTradeResult(0, "Test reboot4", "Test reboot", false);
            });
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                mainForm.OnTradeResult(0, "Test reboot5", "Test reboot", false);
            });
            Thread.Sleep(2000);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            List<Account> accounts = new List<Account>();
            accounts.Add(AccountHelper.GetTestMainAccount());
            accounts.Add(AccountHelper.GetTestMainAccount());
            accounts.Add(AccountHelper.GetTestMainAccount());
            Task.Run(() =>
            {
                string dir = Environment.CurrentDirectory + @"\Json";
                string filePath = dir + @"\test.json";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, accounts);
                }
                accounts= JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(filePath));
                accounts.RemoveAt(0);
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, accounts);
                }
                MessageBox.Show(accounts.ToString());
            });

        }
    }
}
