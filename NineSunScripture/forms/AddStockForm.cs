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
        private short category;
        public AddStockForm()
        {
            InitializeComponent();
        }

        public AddStockForm(MainForm mainForm, short category)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.category = category;
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
    }
}
