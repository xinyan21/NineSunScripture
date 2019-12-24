using NineSunScripture.model;
using NineSunScripture.trade.api;
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
    public partial class AddStockForm : Form
    {
        private MainForm mainForm;
        private List<Account> accounts;
        private short category = Quotes.CategoryLatest;
        Quotes quotes = new Quotes();

        public AddStockForm(List<Account> accounts, MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.accounts = accounts;
        }

        private void BtnAddStcok_Click(object sender, EventArgs e)
        {
            if (tbMoney.Text.Contains("."))
            {
                MessageBox.Show("成交额只能输入整数！");
                return;
            }

            quotes.Code = tbCode.Text;
            quotes.PositionCtrl = float.Parse(tbPosition.Text);
            if (tbMoney.Text.Length > 0)
            {
                quotes.MoneyCtrl = int.Parse(tbMoney.Text);
            }
            quotes.StockCategory = category;
            mainForm.AddStock(quotes);
            Close();
        }

        private void RbtnDragonLeader_CheckedChanged(object sender, EventArgs e)
        {
            this.category = Quotes.CategoryDragonLeader;
        }

        private void RbtnLongTerm_CheckedChanged(object sender, EventArgs e)
        {
            this.category = Quotes.CategoryLongTerm;
        }

        private void RbtnTomorrow_CheckedChanged(object sender, EventArgs e)
        {
            this.category = Quotes.CategoryLatest;
        }

        private void TbCode_TextChanged(object sender, EventArgs e)
        {
            if (tbCode.TextLength == 6)
            {
                try
                {
                    quotes = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, tbCode.Text);
                }
                catch (Exception exce)
                {

                    MessageBox.Show(exce.Message);
                }
                if (null != quotes.Name && quotes.Name.Length > 0)
                {
                    tbName.Text = quotes.Name + "[" + quotes.LatestPrice + "]";
                }
            }
        }

        private void RbHitBoard_CheckedChanged(object sender, EventArgs e)
        {
            this.quotes.TradeStrategy = Strategy.HitBoard;
        }

        private void RbWeakTurnStrong_CheckedChanged(object sender, EventArgs e)
        {
            this.quotes.TradeStrategy = Strategy.WeakTurnStrong;
        }
    }
}
