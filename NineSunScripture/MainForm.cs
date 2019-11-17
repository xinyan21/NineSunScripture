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
using NineSunScripture.model;

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
            int clientId = TradeAPI.Logon(Qsid, Host, Port, Version, AccountType, Account,
                password, comm_password, false, ErrInfo);
            if (clientId > 0)
            {
                //Funds funds = TradeAPI.QueryFunds(clientId);
                //TradeAPI.QueryQuotes(clientId, "600686");
                int rspCode = TradeAPI.QueryData(clientId, 3, Result, ErrInfo);
                if (rspCode>0)
                {
                    label1.Text = Encoding.Default.GetString(Result).TrimEnd('\0');
                }
                else
                {
                    MessageBox.Show(Encoding.Default.GetString(ErrInfo).TrimEnd('\0'));
                }
            }
            else
            {
                label1.Text = Encoding.Default.GetString(Result).TrimEnd('\0');
            }
        }

        private void Label1_Click(object sender, EventArgs e)
        {

        }

        private void 添加账号ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
