﻿using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
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
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("账户不能为空！");
                return;
            }
            Order order = new Order();
            order.Code = tbCode.Text;
            order.Price = float.Parse(tbPrice.Text);

            Task[] tasks = new Task[accounts.Count];
            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                tasks[i] = Task.Run(() =>
                {
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
                        + Math.Round(order.Quantity * order.Price / 10000, 2) + "万元";
                    Logger.Log(opLog);
                    if (null != callback)
                    {
                        string errInfo = ApiHelper.ParseErrInfo(order.ErrorInfo);
                        callback.OnTradeResult(rspCode, opLog, errInfo, false);
                    }
                });
            }
            Task.WaitAll(tasks);
        }

        private void Sell()
        {
            quotes.Buy2 = float.Parse(tbPrice.Text);
            ContBoardSellStrategy.SellByRatio(quotes, accounts, callback, positionRatio);
        }

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            if (tbCode.TextLength == 6)
            {
                try
                {
                    quotes = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, tbCode.Text);
                }
                catch
                {
                    return;
                }
                if (null != quotes && !string.IsNullOrEmpty(quotes.Name))
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
