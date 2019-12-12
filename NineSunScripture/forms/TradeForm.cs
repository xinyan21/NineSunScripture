using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
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
    public partial class TradeForm : Form
    {
        private float positionRatio = 1 / 3f;
        private List<Account> accounts;
        private Quotes quotes;
        private short opDirection;  //buy or sell, @Order const
        private ITrade callback;

        public TradeForm(List<Account> accounts, ITrade callback, short opDirection = Order.CategoryBuy)
        {
            InitializeComponent();
            this.opDirection = opDirection;
            this.accounts = accounts;
            this.callback = callback;
            if (opDirection == Order.CategorySell)
            {
                SetSellView();
            }
        }
        public TradeForm(List<Account> accounts, Quotes quotes,
            ITrade callback, short opDirection = Order.CategoryBuy)
        {
            InitializeComponent();
            this.opDirection = opDirection;
            this.accounts = accounts;
            this.quotes = quotes;
            tbCode.Text = quotes.Code;
            tbName.Text = quotes.Name;
            this.callback = callback;
            if (opDirection == Order.CategorySell)
            {
                SetSellView();
            }
        }

        private void SetSellView()
        {
            this.Text = "卖出";
            this.btnBuy.BackgroundImage = Properties.Resources.btn_green;
            this.btnBuy.Text = "卖出";
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (opDirection == Order.CategorySell)
            {
                Sell();
            }
            else
            {
                Buy();
            }
            Close();
        }

        private void Buy()
        {
            Order order = new Order();
            order.Code = tbCode.Text;
            order.Price = float.Parse(tbPrice.Text);

            BuyStrategy.CancelOrdersCanCancel(accounts, quotes, null);
            foreach (Account account in accounts)
            {
                order.TradeSessionId = account.TradeSessionId;
                account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
                ApiHelper.SetShareholderAcct(account, quotes, order);
                //数量是整百整百的
                double money = account.Funds.AvailableAmt * positionRatio;
                order.Quantity = ((int)(money / (order.Price * 100))) * 100;
                int rspCode = TradeAPI.Buy(order);
                string opLog
                    = "资金账号【" + account.FundAcct + "】" + "窗口买入【" + quotes.Name + "】"
                    + order.Quantity * order.Price + "元";
                Logger.log(opLog);
                if (null != callback)
                {
                    callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(account.ErrorInfo));
                }
            }
        }

        private void Sell()
        {
            quotes.Buy2 = float.Parse(tbPrice.Text);
            SellStrategy.SellByRatio(quotes, accounts, callback, positionRatio);
        }

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            if (tbCode.TextLength == 6)
            {
                quotes = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, tbCode.Text);
                if (quotes.Name.Length > 0)
                {
                    tbName.Text = quotes.Name + "[" + quotes.LatestPrice + "]";
                }
            }
        }

        private void rbtnOneThird_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1 / 3f;
        }

        private void rbtnOneSecond_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1 / 2f;
        }

        private void rbtnAllIn_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1;
        }

        private void rbtnOneFourth_CheckedChanged(object sender, EventArgs e)
        {
            positionRatio = 1 / 4f;
        }
    }
}
