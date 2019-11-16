using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using NineSunScripture.trade.api;

namespace NineSunScripture
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            int Qsid = 0;
            string Host = "trade.10jqka.com.cn";
            short Port = 8002;
            string Version = "E065.18.77";
            short AccountType = 0x30;
            //string Account = "49758842";
            //string password = "13231178";
            string Account = "13534068934";
            string password = "3594035x";
            string comm_password = "";

            byte[] ErrInfo = new byte[256];
            byte[] Result = new byte[1024 * 1024];//  不写大点内存溢出会崩溃
            Console.WriteLine("\nstart to login");
            int id = TradeAPI.Logon(Qsid, Host, Port, Version, AccountType, Account,
                password, comm_password, false, ErrInfo);
            if (id > 0)
            {
                label1.Text = "login success" + id;
            }
            else
            {
                label1.Text = Encoding.Default.GetString(Result).TrimEnd('\0');
            }
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
