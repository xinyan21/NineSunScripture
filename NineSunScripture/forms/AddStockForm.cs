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

        private void btnAddStcok_Click(object sender, EventArgs e)
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

        private void rbtnDragonLeader_CheckedChanged(object sender, EventArgs e)
        {
            this.category = Quotes.CategoryDragonLeader;
        }

        private void rbtnLongTerm_CheckedChanged(object sender, EventArgs e)
        {
            this.category = Quotes.CategoryLongTerm;
        }

        private void rbtnTomorrow_CheckedChanged(object sender, EventArgs e)
        {
            this.category = Quotes.CategoryLatest;
        }

        private void tbCode_TextChanged(object sender, EventArgs e)
        {
            if (tbCode.TextLength == 6)
            {
                quotes = TradeAPI.QueryQuotes(accounts[0].SessionId, tbCode.Text);
                if (quotes.Name.Length > 0)
                {
                    tbName.Text = quotes.Name + "[" + quotes.LatestPrice + "]";
                }
            }
        }
    }
}
