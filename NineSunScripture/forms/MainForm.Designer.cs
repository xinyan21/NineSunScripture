namespace NineSunScripture
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.lvStocks = new System.Windows.Forms.ListView();
            this.cmsStocks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmAddTomorrowStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBuy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelTomorrowStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmClearTomorrowStocks = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTotalAsset = new System.Windows.Forms.Label();
            this.lblMoneyAvailable = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lvPositions = new System.Windows.Forms.ListView();
            this.cmsPositionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSell = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSellInForm = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemManageAcct = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuMain = new System.Windows.Forms.MenuStrip();
            this.管理菜单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSwitch = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBuyStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tspManageAcct = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearPositions = new System.Windows.Forms.ToolStripMenuItem();
            this.tspExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbRuntimeInfo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tsmiTest = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel2.SuspendLayout();
            this.cmsStocks.SuspendLayout();
            this.panel1.SuspendLayout();
            this.cmsPositionsMenu.SuspendLayout();
            this.MenuMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.lvStocks);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(875, 186);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(310, 757);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18.33962F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Gold;
            this.label2.Location = new System.Drawing.Point(4, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 39);
            this.label2.TabIndex = 0;
            this.label2.Text = "股票池";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvStocks
            // 
            this.lvStocks.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.lvStocks.BackgroundImageTiled = true;
            this.lvStocks.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvStocks.ContextMenuStrip = this.cmsStocks;
            this.lvStocks.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvStocks.ForeColor = System.Drawing.Color.Yellow;
            this.lvStocks.Location = new System.Drawing.Point(3, 42);
            this.lvStocks.Name = "lvStocks";
            this.lvStocks.Size = new System.Drawing.Size(305, 712);
            this.lvStocks.TabIndex = 1;
            this.lvStocks.UseCompatibleStateImageBehavior = false;
            this.lvStocks.View = System.Windows.Forms.View.Details;
            // 
            // cmsStocks
            // 
            this.cmsStocks.AutoSize = false;
            this.cmsStocks.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsStocks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddTomorrowStock,
            this.tsmiBuy,
            this.tsmDelTomorrowStock,
            this.tsmClearTomorrowStocks});
            this.cmsStocks.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.cmsStocks.Name = "cmsTomorrow";
            this.cmsStocks.Size = new System.Drawing.Size(193, 142);
            this.cmsStocks.Text = "股票池管理";
            // 
            // tsmAddTomorrowStock
            // 
            this.tsmAddTomorrowStock.Name = "tsmAddTomorrowStock";
            this.tsmAddTomorrowStock.Size = new System.Drawing.Size(134, 24);
            this.tsmAddTomorrowStock.Text = "添加股票";
            this.tsmAddTomorrowStock.Click += new System.EventHandler(this.tsmAddStock_Click);
            // 
            // tsmiBuy
            // 
            this.tsmiBuy.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmiBuy.Name = "tsmiBuy";
            this.tsmiBuy.Size = new System.Drawing.Size(134, 24);
            this.tsmiBuy.Text = "买入";
            this.tsmiBuy.Click += new System.EventHandler(this.tsmiBuy_Click);
            // 
            // tsmDelTomorrowStock
            // 
            this.tsmDelTomorrowStock.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmDelTomorrowStock.Name = "tsmDelTomorrowStock";
            this.tsmDelTomorrowStock.Size = new System.Drawing.Size(134, 24);
            this.tsmDelTomorrowStock.Text = "删除";
            this.tsmDelTomorrowStock.Click += new System.EventHandler(this.tsmDelStock_Click);
            // 
            // tsmClearTomorrowStocks
            // 
            this.tsmClearTomorrowStocks.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmClearTomorrowStocks.Name = "tsmClearTomorrowStocks";
            this.tsmClearTomorrowStocks.Size = new System.Drawing.Size(134, 24);
            this.tsmClearTomorrowStocks.Text = "清空";
            this.tsmClearTomorrowStocks.Click += new System.EventHandler(this.tsmClearStocks_Click);
            // 
            // lblTotalAsset
            // 
            this.lblTotalAsset.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalAsset.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalAsset.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotalAsset.Location = new System.Drawing.Point(4, 10);
            this.lblTotalAsset.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTotalAsset.Name = "lblTotalAsset";
            this.lblTotalAsset.Size = new System.Drawing.Size(209, 45);
            this.lblTotalAsset.TabIndex = 0;
            this.lblTotalAsset.Text = "总资产：";
            // 
            // lblMoneyAvailable
            // 
            this.lblMoneyAvailable.BackColor = System.Drawing.Color.Transparent;
            this.lblMoneyAvailable.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMoneyAvailable.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMoneyAvailable.Location = new System.Drawing.Point(2, 69);
            this.lblMoneyAvailable.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMoneyAvailable.Name = "lblMoneyAvailable";
            this.lblMoneyAvailable.Size = new System.Drawing.Size(211, 52);
            this.lblMoneyAvailable.TabIndex = 1;
            this.lblMoneyAvailable.Text = "可用金额：";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.LightCoral;
            this.panel1.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lvPositions);
            this.panel1.Controls.Add(this.lblTotalAsset);
            this.panel1.Controls.Add(this.lblMoneyAvailable);
            this.panel1.Location = new System.Drawing.Point(112, 1010);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(986, 133);
            this.panel1.TabIndex = 8;
            // 
            // lvPositions
            // 
            this.lvPositions.BackColor = System.Drawing.Color.LightCoral;
            this.lvPositions.BackgroundImageTiled = true;
            this.lvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvPositions.ContextMenuStrip = this.cmsPositionsMenu;
            this.lvPositions.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvPositions.ForeColor = System.Drawing.SystemColors.Info;
            this.lvPositions.Location = new System.Drawing.Point(283, -2);
            this.lvPositions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvPositions.MultiSelect = false;
            this.lvPositions.Name = "lvPositions";
            this.lvPositions.Scrollable = false;
            this.lvPositions.Size = new System.Drawing.Size(701, 136);
            this.lvPositions.TabIndex = 2;
            this.lvPositions.UseCompatibleStateImageBehavior = false;
            this.lvPositions.View = System.Windows.Forms.View.Details;
            // 
            // cmsPositionsMenu
            // 
            this.cmsPositionsMenu.BackColor = System.Drawing.SystemColors.Control;
            this.cmsPositionsMenu.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsPositionsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSell,
            this.tsmiSellInForm});
            this.cmsPositionsMenu.Name = "cmsPositionsMenu";
            this.cmsPositionsMenu.Size = new System.Drawing.Size(144, 94);
            this.cmsPositionsMenu.Text = "持仓菜单";
            // 
            // tsmiSell
            // 
            this.tsmiSell.AutoSize = false;
            this.tsmiSell.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsmiSell.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tsmiSell.Name = "tsmiSell";
            this.tsmiSell.Size = new System.Drawing.Size(214, 40);
            this.tsmiSell.Text = "一键卖出";
            this.tsmiSell.Click += new System.EventHandler(this.tsmiSell_Click);
            // 
            // tsmiSellInForm
            // 
            this.tsmiSellInForm.AutoSize = false;
            this.tsmiSellInForm.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsmiSellInForm.Name = "tsmiSellInForm";
            this.tsmiSellInForm.Size = new System.Drawing.Size(192, 40);
            this.tsmiSellInForm.Text = "窗口卖出";
            this.tsmiSellInForm.Click += new System.EventHandler(this.tsmiSellInForm_Click);
            // 
            // Menu
            // 
            this.Menu.AutoSize = false;
            this.Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemManageAcct,
            this.MenuItemExit});
            this.Menu.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Menu.ForeColor = System.Drawing.Color.White;
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(86, 40);
            this.Menu.Text = "管理菜单";
            // 
            // MenuItemManageAcct
            // 
            this.MenuItemManageAcct.Name = "MenuItemManageAcct";
            this.MenuItemManageAcct.Size = new System.Drawing.Size(150, 26);
            this.MenuItemManageAcct.Text = "账号管理";
            this.MenuItemManageAcct.Click += new System.EventHandler(this.MenuItemManageAcct_Click);
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(150, 26);
            this.MenuItemExit.Text = "Exit";
            // 
            // MenuMain
            // 
            this.MenuMain.AllowDrop = true;
            this.MenuMain.AutoSize = false;
            this.MenuMain.BackColor = System.Drawing.Color.Transparent;
            this.MenuMain.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.MenuMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.MenuMain.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.管理菜单ToolStripMenuItem});
            this.MenuMain.Location = new System.Drawing.Point(0, 0);
            this.MenuMain.Name = "MenuMain";
            this.MenuMain.Size = new System.Drawing.Size(1211, 40);
            this.MenuMain.TabIndex = 7;
            this.MenuMain.Text = "菜单";
            // 
            // 管理菜单ToolStripMenuItem
            // 
            this.管理菜单ToolStripMenuItem.AutoSize = false;
            this.管理菜单ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSwitch,
            this.tsmiBuyStock,
            this.tspManageAcct,
            this.tsmiClearPositions,
            this.tsmiTest,
            this.tspExit});
            this.管理菜单ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.管理菜单ToolStripMenuItem.ForeColor = System.Drawing.Color.DarkGreen;
            this.管理菜单ToolStripMenuItem.Name = "管理菜单ToolStripMenuItem";
            this.管理菜单ToolStripMenuItem.Size = new System.Drawing.Size(86, 36);
            this.管理菜单ToolStripMenuItem.Text = "管理菜单";
            // 
            // tsmiSwitch
            // 
            this.tsmiSwitch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsmiSwitch.AutoSize = false;
            this.tsmiSwitch.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tsmiSwitch.Name = "tsmiSwitch";
            this.tsmiSwitch.Padding = new System.Windows.Forms.Padding(0, 10, 0, 1);
            this.tsmiSwitch.Size = new System.Drawing.Size(150, 40);
            this.tsmiSwitch.Text = "启动";
            this.tsmiSwitch.Click += new System.EventHandler(this.tsmiSwitch_Click);
            // 
            // tsmiBuyStock
            // 
            this.tsmiBuyStock.AutoSize = false;
            this.tsmiBuyStock.Name = "tsmiBuyStock";
            this.tsmiBuyStock.Size = new System.Drawing.Size(198, 40);
            this.tsmiBuyStock.Text = "买入";
            this.tsmiBuyStock.Click += new System.EventHandler(this.tsmiBuyStock_Click);
            // 
            // tspManageAcct
            // 
            this.tspManageAcct.AutoSize = false;
            this.tspManageAcct.Name = "tspManageAcct";
            this.tspManageAcct.Size = new System.Drawing.Size(150, 40);
            this.tspManageAcct.Text = "账号管理";
            this.tspManageAcct.Click += new System.EventHandler(this.tspManageAcct_Click);
            // 
            // tsmiClearPositions
            // 
            this.tsmiClearPositions.AutoSize = false;
            this.tsmiClearPositions.Name = "tsmiClearPositions";
            this.tsmiClearPositions.Size = new System.Drawing.Size(150, 40);
            this.tsmiClearPositions.Text = "一键清仓";
            this.tsmiClearPositions.Click += new System.EventHandler(this.tsmiClearPositions_Click);
            // 
            // tspExit
            // 
            this.tspExit.AutoSize = false;
            this.tspExit.Name = "tspExit";
            this.tspExit.Size = new System.Drawing.Size(150, 40);
            this.tspExit.Text = "退出";
            // 
            // tbRuntimeInfo
            // 
            this.tbRuntimeInfo.BackColor = System.Drawing.Color.LightCoral;
            this.tbRuntimeInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRuntimeInfo.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbRuntimeInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.tbRuntimeInfo.Location = new System.Drawing.Point(0, 42);
            this.tbRuntimeInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbRuntimeInfo.Multiline = true;
            this.tbRuntimeInfo.Name = "tbRuntimeInfo";
            this.tbRuntimeInfo.ReadOnly = true;
            this.tbRuntimeInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbRuntimeInfo.Size = new System.Drawing.Size(300, 712);
            this.tbRuntimeInfo.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 18.33962F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Gold;
            this.label3.Location = new System.Drawing.Point(4, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(304, 39);
            this.label3.TabIndex = 15;
            this.label3.Text = "运行日志";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.tbRuntimeInfo);
            this.panel2.Location = new System.Drawing.Point(25, 186);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(306, 757);
            this.panel2.TabIndex = 16;
            // 
            // tsmiTest
            // 
            this.tsmiTest.AutoSize = false;
            this.tsmiTest.Name = "tsmiTest";
            this.tsmiTest.Size = new System.Drawing.Size(198, 40);
            this.tsmiTest.Text = "测试";
            this.tsmiTest.Click += new System.EventHandler(this.tsmiTest_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._6;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(1211, 1214);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.MenuMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuMain;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "九阳真经策略";
            this.flowLayoutPanel2.ResumeLayout(false);
            this.cmsStocks.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.cmsPositionsMenu.ResumeLayout(false);
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.ComponentResourceManager resources;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalAsset;
        private System.Windows.Forms.Label lblMoneyAvailable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem Menu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemManageAcct;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.MenuStrip MenuMain;
        private System.Windows.Forms.ContextMenuStrip cmsStocks;
        private System.Windows.Forms.ToolStripMenuItem tsmAddTomorrowStock;
        private System.Windows.Forms.ToolStripMenuItem tsmDelTomorrowStock;
        private System.Windows.Forms.ToolStripMenuItem tsmClearTomorrowStocks;
        private System.Windows.Forms.TextBox tbRuntimeInfo;
        private System.Windows.Forms.ListView lvPositions;
        private System.Windows.Forms.ToolStripMenuItem 管理菜单ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tspManageAcct;
        private System.Windows.Forms.ToolStripMenuItem tspExit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwitch;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearPositions;
        private System.Windows.Forms.ToolStripMenuItem tsmiBuy;
        private System.Windows.Forms.ListView lvStocks;
        private System.Windows.Forms.ToolStripMenuItem tsmiBuyStock;
        private System.Windows.Forms.ContextMenuStrip cmsPositionsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiSell;
        private System.Windows.Forms.ToolStripMenuItem tsmiSellInForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiTest;
    }
}

