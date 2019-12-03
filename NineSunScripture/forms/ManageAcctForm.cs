using NineSunScripture.db;
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
    public partial class ManageAcctForm : Form
    {
        private AcctDbHelper dbHelper;
        private List<Account> accounts;
        public ManageAcctForm()
        {
            dbHelper = new AcctDbHelper();
            InitializeComponent();
            InitListView();
        }

        private void InitListView()
        {
            lvAccounts.Columns.Add("券商", 150, HorizontalAlignment.Center);
            lvAccounts.Columns.Add("资金账号", 350, HorizontalAlignment.Center);
            lvAccounts.MultiSelect = false;
            lvAccounts.View = View.Details;
            ImageList imgList = new ImageList();
            imgList.ImageSize = new Size(1, 32);//分别是宽和高
            lvAccounts.SmallImageList = imgList;

            lvAccounts.BeginUpdate();
            accounts = dbHelper.GetAccounts();

            ListViewItem item = null;
            foreach (Account account in accounts)
            {
                item = new ListViewItem(account.BrokerName);
                item.SubItems.Add(account.FundAcct);
                lvAccounts.Items.Add(item);
            }
            lvAccounts.EndUpdate();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (null != dbHelper.GetAccountByFundAcct(tbAccout.Text))
            {
                MessageBox.Show("该资金账号已经存在，不能重复添加！");
                return;
            }
            if (!checkInput())
            {
                return;
            }

            Account account = new Account();
            //默认自动判断
            account.AcctType = 0x20;
            account.BrokerId = int.Parse(tbBrokerId.Text);
            account.BrokerName = tbBrokerName.Text;
            account.BrokerServerIP = tbIP.Text;
            account.BrokerServerPort = short.Parse(tbPort.Text);
            account.VersionOfTHS = tbTHSVersion.Text;
            account.FundAcct = tbAccout.Text;
            account.Password = tbPassword.Text;
            account.CommPwd = tbCommPwd.Text;

            bool result = dbHelper.AddAccount(account);
            if (result)
            {
                MessageBox.Show("添加成功");
                ListViewItem item = new ListViewItem(account.BrokerName);
                item.SubItems.Add(account.FundAcct);
                lvAccounts.Items.Add(item);
            }
            else
            {
                MessageBox.Show("添加失败");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (null == dbHelper.GetAccountByFundAcct(tbAccout.Text))
            {
                MessageBox.Show("不能修改资金账号，请删除该账号重新添加！");
                return;
            }
            Account account = new Account();
            account.BrokerId = int.Parse(tbBrokerId.Text);
            account.BrokerName = tbBrokerName.Text;
            account.BrokerServerIP = tbIP.Text;
            account.BrokerServerPort = short.Parse(tbPort.Text);
            account.VersionOfTHS = tbTHSVersion.Text;
            account.FundAcct = tbAccout.Text;
            account.Password = tbPassword.Text;
            account.CommPwd = tbCommPwd.Text;

            bool result = dbHelper.EditAccount(account);
            if (result)
            {
                MessageBox.Show("修改成功！");
                int editIndex = lvAccounts.SelectedIndices[0];
                accounts[editIndex] = account;
                ListViewItem item = new ListViewItem(account.BrokerName);
                item.SubItems.Add(account.FundAcct);
                lvAccounts.Items[editIndex] = item;
            }
            else
            {
                MessageBox.Show("修改失败！");
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            bool result = dbHelper.DelAccount(tbAccout.Text);
            if (result)
            {
                MessageBox.Show("删除成功");
                lvAccounts.Items.Remove(lvAccounts.SelectedItems[0]);
            }
            else
            {
                MessageBox.Show("删除失败");
            }
        }

        private void lvAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvAccounts.SelectedItems.Count == 0)
            {
                return;
            }
            string fundAcct = lvAccounts.SelectedItems[0].SubItems[1].Text;
            Account acct = null;
            foreach (Account account in accounts)
            {
                if (fundAcct == account.FundAcct)
                {
                    acct = account;
                    break;
                }
            }
            if (null == acct)
            {
                return;
            }
            tbAccout.Text = acct.FundAcct;
            tbBrokerId.Text = acct.BrokerId + "";
            tbBrokerName.Text = acct.BrokerName;
            tbCommPwd.Text = acct.CommPwd;
            tbIP.Text = acct.BrokerServerIP;
            tbPassword.Text = acct.Password;
            tbPort.Text = acct.BrokerServerPort + "";
            tbTHSVersion.Text = acct.VersionOfTHS;
        }

        private bool checkInput()
        {
            if (tbAccout.Text.Length == 0)
            {
                MessageBox.Show("资金账号不能为空！");
                return false;
            }
            if (tbBrokerId.Text.Length == 0)
            {
                MessageBox.Show("券商ID不能为空！");
                return false;
            }
            if (tbBrokerName.Text.Length == 0)
            {
                MessageBox.Show("券商名称不能为空！");
                return false;
            }
            if (tbIP.Text.Length == 0)
            {
                MessageBox.Show("IP不能为空");
                return false;
            }
            if (tbPassword.Text.Length == 0)
            {
                MessageBox.Show("密码不能为空！");
                return false;
            }
            if (tbPort.Text.Length == 0)
            {
                MessageBox.Show("端口不能为空！");
                return false;
            }
            if (tbTHSVersion.Text.Length == 0)
            {
                MessageBox.Show("同花顺版本号不能为空！");
                return false;
            }
            return true;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            tbAccout.Text = "";
            tbBrokerId.Text = "";
            tbBrokerName.Text = "";
            tbCommPwd.Text = "";
            tbIP.Text = "";
            tbPassword.Text = "";
            tbPort.Text = "";
            tbTHSVersion.Text = "";
        }
    }
}
