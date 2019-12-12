using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.util.test
{
    public partial class TestForm : Form
    {
        private TradeTestCase testTrade = new TradeTestCase();
        private List<Account> accounts;
        public TestForm(List<Account> accounts )
        {
            InitializeComponent();
            this.accounts = accounts;
        }

        private void btnTestBuy_Click(object sender, EventArgs e)
        {
            testTrade.TestBuyStrategy(accounts );
        }

        private void button1_Click(object sender, EventArgs e)
        {
            testTrade.TestSellStrategy(accounts);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Order> todayTransactions = TradeAPI.QueryTodayTransaction(accounts[0].TradeSessionId);
        }
    }
}
