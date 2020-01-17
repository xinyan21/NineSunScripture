﻿using NineSunScripture.model;
using NineSunScripture.trade.structApi.api;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace NineSunScripture.forms
{
    public partial class AddStockForm : Form
    {
        private MainForm mainForm;
        private List<Account> accounts;
        private short category = Quotes.CategoryLatest;
        private Quotes quotes = new Quotes();

        public AddStockForm(List<Account> accounts, MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.accounts = accounts;
            panelBandParam.Visible = false;
        }

        private void BtnAddStcok_Click(object sender, EventArgs e)
        {
            if (tbMoney.Text.Contains("."))
            {
                MessageBox.Show("成交额只能输入整数！");
                return;
            }

            quotes.Code = tbCode.Text;
            if (!tbName.Text.Contains("["))
            {
                quotes.Name = tbName.Text; 
            }
            quotes.PositionCtrl = float.Parse(tbPosition.Text);
            if (tbMoney.Text.Length > 0)
            {
                quotes.MoneyCtrl = int.Parse(tbMoney.Text);
            }
            quotes.StockCategory = category;
            if (category == Quotes.CategoryBand)
            {
                if (tbStopLossPrice.Text.Length > 0)
                {
                    quotes.StopLossPrice = float.Parse(tbStopLossPrice.Text);
                }
                if (tbStopWinPrice.Text.Length > 0)
                {
                    quotes.StopWinPrice = float.Parse(tbStopWinPrice.Text);
                }
            }
            mainForm.AddStock(quotes);
            Close();
        }

        private void RbtnDragonLeader_CheckedChanged(object sender, EventArgs e)
        {
            category = Quotes.CategoryDragonLeader;
            panelBandParam.Visible = false;
        }

        private void RbtnLongTerm_CheckedChanged(object sender, EventArgs e)
        {
            category = Quotes.CategoryLongTerm;
            panelBandParam.Visible = false;
        }

        private void RbtnLatest_CheckedChanged(object sender, EventArgs e)
        {
            category = Quotes.CategoryLatest;
            panelBandParam.Visible = false;
        }

        private void RbWeakTurnStrong_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryWeakTurnStrong;
            panelBandParam.Visible = false;
        }

        private void RbtnBand_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryBand;
            panelBandParam.Visible = true;
        }

        private void TbCode_TextChanged(object sender, EventArgs e)
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

        private void RbtnBuy_CheckedChanged(object sender, EventArgs e)
        {
            quotes.Operation = Quotes.OperationBuy;
        }

        private void RbtnSell_CheckedChanged(object sender, EventArgs e)
        {
            quotes.Operation = Quotes.OperationSell;
        }
    }
}