using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.util.test
{
    public partial class TestForm : Form
    {
        private TestTrade testTrade = new TestTrade();
        public TestForm()
        {
            InitializeComponent();
        }

        private void btnTestBuy_Click(object sender, EventArgs e)
        {
            testTrade.TestBuyStrategy();
        }
    }
}
