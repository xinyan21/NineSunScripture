using NineSunScripture.model;
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
        private short category = Quotes.CategoryLatest;
        public AddStockForm()
        {
            InitializeComponent();
        }

        public AddStockForm(MainForm mainForm)
        {
            InitializeComponent();
            this.mainForm = mainForm;
        }

        private void btnAddStcok_Click(object sender, EventArgs e)
        {
            if (tbMoney.Text.Contains("."))
            {
                MessageBox.Show("成交额只能输入整数！");
                return;
            }
            Quotes quotes = new Quotes();
            quotes.Code = tbCode.Text;
            quotes.Name = tbName.Text;
            quotes.PositionCtrl = float.Parse(tbPosition.Text);
            quotes.MoneyCtrl = int.Parse(tbMoney.Text);
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
    }
}
