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
            int Qsid = 59;
            string Host = "101.89.64.194";
            short Port = 8003;
            string Version = "E065.20.92";
            short AccountType = 0x30;
            //string Account = "49758842";
            //string password = "13231178";
            string Account = "321019194496";
            string password = "198921";
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
                int rspCode = TradeAPI.QueryData(clientId, 4, Result, ErrInfo);
                if (rspCode>0)
                {
                }
                else
                {
                    MessageBox.Show(Encoding.Default.GetString(ErrInfo).TrimEnd('\0'));
                }
            }
            else
            {
            }
        }

        private void BtnAddLongTermSock_Click(object sender, EventArgs e)
        {

        }

        private void MenuItemManageAcct_Click(object sender, EventArgs e)
        {

        }
    }
}
