using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.persistence;
using NineSunScripture.trade.structApi.api;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.forms
{
    partial class TradeForm : Form
    {
        private float positionRatio = 1 / 3f;
        private List<Account> accounts;
        private Quotes quotes;
        private short opDirection;  //buy or sell, @Order const
        private ITrade callback;

        public TradeForm(
            List<Account> accounts, ITrade callback, short opDirection = Order.CategoryBuy)
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
            QueryPrice();
        }

        private void SetSellView()
        {
            Text = "卖出";
            btnBuy.BackgroundImage = Properties.Resources.btn_green;
            btnBuy.Text = "卖出";
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
        }

        private void Buy()
        {
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("账户不能为空！");
                return;
            }

            float price = 0;
            try
            {
                price = float.Parse(tbPrice.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("价格格式错误");
                return;
            }
            Close();

            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks.Add(Task.Run(() =>
                {
                    Order order = new Order();
                    order.Code = tbCode.Text;
                    order.Price = price;
                    order.TradeSessionId = account.TradeSessionId;
                    account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
                    ApiHelper.SetShareholderAcct(account, quotes, order);
                    //数量是整百整百的
                    double money = account.Funds.TotalAsset * positionRatio;
                    if (money > account.Funds.AvailableAmt)
                    {
                        money = account.Funds.AvailableAmt;
                    }
                    order.Quantity = ((int)(money / (order.Price * 100))) * 100;
                    if (order.Quantity == 0)
                    {
                        return;
                    }
                    int rspCode = TradeAPI.Buy(order);
                    string opLog
                        = "资金账号【" + account.FundAcct + "】" + "窗口买入【" + quotes.Name + "】"
                          + Math.Round(order.Quantity * order.Price / account.Funds.TotalAsset * 100) + "%仓位";
                    Logger.Log(opLog);
                    lock (failAccts)
                    {
                        if (rspCode <= 0)
                        {
                            failAccts.Add(account);
                        }
                        if (rspCode != 888)
                        {
                            successCnt++;
                        }
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (successCnt > 0)
            {
                quotes.Operation = Quotes.OperationSell;
                quotes.StockCategory = Quotes.CategoryBand;
                JsonDataHelper.Instance.AddStock(quotes);
            }
            if (null != callback && (successCnt + failAccts.Count) > 0)
            {
                string tradeResult = "【" + quotes.Name + "】窗口买入结果：成功账户"
                    + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(
                    MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }

        private void Sell()
        {
            try
            {
                quotes.Buy2 = float.Parse(tbPrice.Text);
            }
            catch (Exception e)
            {
                MessageBox.Show("价格格式错误");
                Logger.Exception(e);
                return;
            }
            Close();
            AccountHelper.SellByRatio(quotes, accounts, callback, positionRatio);
            JsonDataHelper.Instance.DelStockByCode(quotes.Code, Quotes.OperationSell);
        }

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            if (tbCode.TextLength == 6)
            {
                QueryPrice();
            }
        }

        private void QueryPrice()
        {
            try
            {
                quotes = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, tbCode.Text);
            }
            catch (Exception ex)
            {
                Logger.Exception(ex);
                return;
            }
            if (null != quotes && !string.IsNullOrEmpty(quotes.Name))
            {
                tbName.Text = quotes.Name + "[" + quotes.Buy1 + "]";
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
