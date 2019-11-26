using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.api;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.forms
{
    public partial class BuyForm : Form
    {
        private float positionRatio = 1 / 3;
        private List<Account> accounts;
        private Quotes quotes;
        public BuyForm(List<Account> accounts)
        {
            InitializeComponent();
            this.accounts = accounts;
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            Order order = new Order();
            order.Code = tbCode.Text;
            order.Price = float.Parse(tbPrice.Text);

            BuyStrategy.CancelOrdersCanCancel(accounts, quotes, null);
            foreach (Account account in accounts)
            {
                order.ClientId = account.ClientId;
                account.Funds = TradeAPI.QueryFunds(account.ClientId);
                BuyStrategy.SetShareholderAcct(account, quotes, order);
                //数量是整百整百的
                double money = account.Funds.AvailableAmt * positionRatio;
                order.Quantity = ((int)(money / (order.Price * 100))) * 100;
                int rspCode = TradeAPI.Buy(order);
                string opLog
                    = account.FundAcct + "窗口买入" + quotes.Name + "->"
                    + order.Quantity * order.Price + "元";
                Logger.log(opLog);
            }
        }

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            if (tbCode.TextLength == 6)
            {
                quotes = TradeAPI.QueryQuotes(accounts[0].ClientId, tbCode.Text);
                if (quotes.Name.Length > 0)
                {
                    tbName.Text = quotes.Name + "[" + quotes.Sell1 + "]";
                }
            }
        }

        private void rbtnOneThird_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1 / 3;
        }

        private void rbtnOneSecond_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1 / 2;
        }

        private void rbtnAllIn_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1;
        }
    }
}
