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
        private int category;
        public AddStockForm()
        {
            InitializeComponent();
        }

        public AddStockForm(MainForm mainForm, int category)
        {
            InitializeComponent();
            this.mainForm = mainForm;
            this.category = category;
        }

        private void btnAddStcok_Click(object sender, EventArgs e)
        {
            Quotes quotes = new Quotes();
            quotes.Code = tbCode.Text;
            quotes.Name = tbName.Text;
            quotes.positionCtrl = float.Parse(tbPosition.Text);
            quotes.moneyCtrl = int.Parse(tbMoney.Text);
            mainForm.AddStock(category, quotes);
            Close();
        }
    }
}
