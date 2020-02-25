using NineSunScripture.model;
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
        private Quotes quotes;

        public AddStockForm(List<Account> accounts, MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.accounts = accounts;
            quotes = new Quotes();
            //默认值
            quotes.Operation = Quotes.OperationBuy;
            quotes.StockCategory = Quotes.CategoryHitBoard;
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
            try
            {
                if (tbPosition.Text.Length > 0)
                {
                    quotes.PositionCtrl = (float)(float.Parse(tbPosition.Text) / 10 + 0.01);
                }
                if (tbMoney.Text.Length > 0)
                {
                    quotes.MoneyCtrl = int.Parse(tbMoney.Text);
                }
                if (quotes.StockCategory == Quotes.CategoryBand)
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
                if (tbContBoards.Text.Length > 0)
                {
                    quotes.ContBoards = short.Parse(tbContBoards.Text);
                }
                if (tbAvgCost.Text.Length > 0)
                {
                    quotes.AvgCost = float.Parse(tbAvgCost.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入格式有误，请检查后重新输入，错误信息：" + ex.Message);
                return;
            }
            mainForm.AddStock(quotes);
            Close();
        }

        private void RbtnDragonLeader_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryDragonLeader;
        }

        private void RbtnLongTerm_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryLongTerm;
        }

        private void RbtnLatest_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryHitBoard;
        }

        private void RbWeakTurnStrong_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryWeakTurnStrong;
        }

        private void RbtnBand_CheckedChanged(object sender, EventArgs e)
        {
            quotes.StockCategory = Quotes.CategoryBand;
        }

        private void TbCode_TextChanged(object sender, EventArgs e)
        {
            Quotes stock;  //别把成员变量覆盖了
            if (tbCode.TextLength == 6)
            {
                try
                {
                    stock = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, tbCode.Text);
                }
                catch
                {
                    return;
                }
                if (null != stock && !string.IsNullOrEmpty(stock.Name))
                {
                    tbName.Text = stock.Name + "[" + stock.Buy1 + "]";
                    quotes.Name = stock.Name;
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

        private void RbtnYes_CheckedChanged(object sender, EventArgs e)
        {
            quotes.IsBuyBackWhenReboard = true;
        }

        private void RbtnNo_CheckedChanged(object sender, EventArgs e)
        {
            quotes.IsBuyBackWhenReboard = false;
        }
    }
}