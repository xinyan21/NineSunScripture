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
            this.flpStockPool = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.lvStocks = new System.Windows.Forms.ListView();
            this.cmsStocks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiAddStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBuy = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelGroup = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiClearStocks = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTotalAsset = new System.Windows.Forms.Label();
            this.lblMoneyAvailable = new System.Windows.Forms.Label();
            this.panelFundInfo = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalProfit = new System.Windows.Forms.Label();
            this.lblTotalPositionRatio = new System.Windows.Forms.Label();
            this.btnSwitchCancelOrders = new System.Windows.Forms.Button();
            this.btnSwtichPositions = new System.Windows.Forms.Button();
            this.lvCancelOrders = new System.Windows.Forms.ListView();
            this.cmsCancelOrders = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.lvPositions = new System.Windows.Forms.ListView();
            this.cmsPositions = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSell = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSellInForm = new System.Windows.Forms.ToolStripMenuItem();
            this.pbWorkStatus = new System.Windows.Forms.PictureBox();
            this.Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemManageAcct = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuMain = new System.Windows.Forms.MenuStrip();
            this.管理菜单ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSwitch = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiBuyStock = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiManageAcct = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiClearPositions = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tspiPrivacyMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTest = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tspExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tbRuntimeInfo = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnPeriod = new System.Windows.Forms.Button();
            this.flpStockPool.SuspendLayout();
            this.cmsStocks.SuspendLayout();
            this.panelFundInfo.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.cmsCancelOrders.SuspendLayout();
            this.cmsPositions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbWorkStatus)).BeginInit();
            this.MenuMain.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // flpStockPool
            // 
            this.flpStockPool.BackColor = System.Drawing.Color.Transparent;
            this.flpStockPool.Controls.Add(this.label2);
            this.flpStockPool.Controls.Add(this.lvStocks);
            this.flpStockPool.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flpStockPool.Location = new System.Drawing.Point(888, 71);
            this.flpStockPool.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.flpStockPool.Name = "flpStockPool";
            this.flpStockPool.Size = new System.Drawing.Size(305, 718);
            this.flpStockPool.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18.33962F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
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
            this.lvStocks.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvStocks.ForeColor = System.Drawing.Color.Yellow;
            this.lvStocks.HideSelection = false;
            this.lvStocks.Location = new System.Drawing.Point(3, 42);
            this.lvStocks.MultiSelect = false;
            this.lvStocks.Name = "lvStocks";
            this.lvStocks.Size = new System.Drawing.Size(300, 669);
            this.lvStocks.TabIndex = 1;
            this.lvStocks.UseCompatibleStateImageBehavior = false;
            this.lvStocks.View = System.Windows.Forms.View.Details;
            // 
            // cmsStocks
            // 
            this.cmsStocks.AutoSize = false;
            this.cmsStocks.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cmsStocks.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsStocks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddStock,
            this.tsmiBuy,
            this.tsmiDelStock,
            this.tsmiDelGroup,
            this.tsmiClearStocks});
            this.cmsStocks.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.cmsStocks.Name = "cmsTomorrow";
            this.cmsStocks.Size = new System.Drawing.Size(193, 200);
            this.cmsStocks.Text = "股票池管理";
            // 
            // tsmiAddStock
            // 
            this.tsmiAddStock.AutoSize = false;
            this.tsmiAddStock.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tsmiAddStock.Name = "tsmiAddStock";
            this.tsmiAddStock.Size = new System.Drawing.Size(192, 30);
            this.tsmiAddStock.Text = "添加股票";
            this.tsmiAddStock.Click += new System.EventHandler(this.TsmAddStock_Click);
            // 
            // tsmiBuy
            // 
            this.tsmiBuy.AutoSize = false;
            this.tsmiBuy.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmiBuy.Name = "tsmiBuy";
            this.tsmiBuy.Size = new System.Drawing.Size(192, 30);
            this.tsmiBuy.Text = "买入";
            this.tsmiBuy.Click += new System.EventHandler(this.TsmiBuy_Click);
            // 
            // tsmiDelStock
            // 
            this.tsmiDelStock.AutoSize = false;
            this.tsmiDelStock.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmiDelStock.Name = "tsmiDelStock";
            this.tsmiDelStock.Size = new System.Drawing.Size(192, 30);
            this.tsmiDelStock.Text = "删除股票";
            this.tsmiDelStock.Click += new System.EventHandler(this.TsmDelStock_Click);
            // 
            // tsmiDelGroup
            // 
            this.tsmiDelGroup.AutoSize = false;
            this.tsmiDelGroup.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmiDelGroup.Name = "tsmiDelGroup";
            this.tsmiDelGroup.Size = new System.Drawing.Size(180, 30);
            this.tsmiDelGroup.Text = "删除该组";
            this.tsmiDelGroup.Click += new System.EventHandler(this.TsmiDelGroup_Click);
            // 
            // tsmiClearStocks
            // 
            this.tsmiClearStocks.AutoSize = false;
            this.tsmiClearStocks.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.tsmiClearStocks.Name = "tsmiClearStocks";
            this.tsmiClearStocks.Size = new System.Drawing.Size(192, 30);
            this.tsmiClearStocks.Text = "清空股票池";
            this.tsmiClearStocks.Click += new System.EventHandler(this.TsmClearStocks_Click);
            // 
            // lblTotalAsset
            // 
            this.lblTotalAsset.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalAsset.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalAsset.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotalAsset.Location = new System.Drawing.Point(0, 0);
            this.lblTotalAsset.Margin = new System.Windows.Forms.Padding(0);
            this.lblTotalAsset.Name = "lblTotalAsset";
            this.lblTotalAsset.Padding = new System.Windows.Forms.Padding(5, 2, 0, 0);
            this.lblTotalAsset.Size = new System.Drawing.Size(151, 42);
            this.lblTotalAsset.TabIndex = 0;
            this.lblTotalAsset.Text = "总资产：";
            this.lblTotalAsset.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMoneyAvailable
            // 
            this.lblMoneyAvailable.BackColor = System.Drawing.Color.Transparent;
            this.lblMoneyAvailable.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMoneyAvailable.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMoneyAvailable.Location = new System.Drawing.Point(0, 42);
            this.lblMoneyAvailable.Margin = new System.Windows.Forms.Padding(0);
            this.lblMoneyAvailable.Name = "lblMoneyAvailable";
            this.lblMoneyAvailable.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblMoneyAvailable.Size = new System.Drawing.Size(151, 42);
            this.lblMoneyAvailable.TabIndex = 1;
            this.lblMoneyAvailable.Text = "可用：";
            this.lblMoneyAvailable.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panelFundInfo
            // 
            this.panelFundInfo.BackColor = System.Drawing.Color.LightCoral;
            this.panelFundInfo.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.panelFundInfo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelFundInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelFundInfo.Controls.Add(this.flowLayoutPanel1);
            this.panelFundInfo.Controls.Add(this.btnSwitchCancelOrders);
            this.panelFundInfo.Controls.Add(this.btnSwtichPositions);
            this.panelFundInfo.Controls.Add(this.lvCancelOrders);
            this.panelFundInfo.Controls.Add(this.lvPositions);
            this.panelFundInfo.Location = new System.Drawing.Point(128, 831);
            this.panelFundInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelFundInfo.Name = "panelFundInfo";
            this.panelFundInfo.Size = new System.Drawing.Size(986, 178);
            this.panelFundInfo.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.lblTotalAsset);
            this.flowLayoutPanel1.Controls.Add(this.lblMoneyAvailable);
            this.flowLayoutPanel1.Controls.Add(this.lblTotalProfit);
            this.flowLayoutPanel1.Controls.Add(this.lblTotalPositionRatio);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(-2, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(155, 168);
            this.flowLayoutPanel1.TabIndex = 22;
            // 
            // lblTotalProfit
            // 
            this.lblTotalProfit.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalProfit.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalProfit.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotalProfit.Location = new System.Drawing.Point(0, 84);
            this.lblTotalProfit.Margin = new System.Windows.Forms.Padding(0);
            this.lblTotalProfit.Name = "lblTotalProfit";
            this.lblTotalProfit.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblTotalProfit.Size = new System.Drawing.Size(155, 42);
            this.lblTotalProfit.TabIndex = 20;
            this.lblTotalProfit.Text = "总盈亏：";
            this.lblTotalProfit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTotalPositionRatio
            // 
            this.lblTotalPositionRatio.BackColor = System.Drawing.Color.Transparent;
            this.lblTotalPositionRatio.Font = new System.Drawing.Font("微软雅黑", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalPositionRatio.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotalPositionRatio.Location = new System.Drawing.Point(0, 126);
            this.lblTotalPositionRatio.Margin = new System.Windows.Forms.Padding(0);
            this.lblTotalPositionRatio.Name = "lblTotalPositionRatio";
            this.lblTotalPositionRatio.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblTotalPositionRatio.Size = new System.Drawing.Size(155, 42);
            this.lblTotalPositionRatio.TabIndex = 21;
            this.lblTotalPositionRatio.Text = "总仓位：";
            this.lblTotalPositionRatio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSwitchCancelOrders
            // 
            this.btnSwitchCancelOrders.BackColor = System.Drawing.Color.Green;
            this.btnSwitchCancelOrders.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.btnSwitchCancelOrders.FlatAppearance.BorderSize = 2;
            this.btnSwitchCancelOrders.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwitchCancelOrders.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSwitchCancelOrders.Location = new System.Drawing.Point(159, 106);
            this.btnSwitchCancelOrders.Name = "btnSwitchCancelOrders";
            this.btnSwitchCancelOrders.Size = new System.Drawing.Size(126, 42);
            this.btnSwitchCancelOrders.TabIndex = 19;
            this.btnSwitchCancelOrders.Text = "撤单";
            this.btnSwitchCancelOrders.UseVisualStyleBackColor = false;
            this.btnSwitchCancelOrders.Click += new System.EventHandler(this.BtnSwitchCancelOrders_Click);
            // 
            // btnSwtichPositions
            // 
            this.btnSwtichPositions.BackColor = System.Drawing.Color.Red;
            this.btnSwtichPositions.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.btnSwtichPositions.FlatAppearance.BorderSize = 2;
            this.btnSwtichPositions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSwtichPositions.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSwtichPositions.Location = new System.Drawing.Point(159, 26);
            this.btnSwtichPositions.Name = "btnSwtichPositions";
            this.btnSwtichPositions.Size = new System.Drawing.Size(126, 42);
            this.btnSwtichPositions.TabIndex = 18;
            this.btnSwtichPositions.Text = "持仓";
            this.btnSwtichPositions.UseVisualStyleBackColor = false;
            this.btnSwtichPositions.Click += new System.EventHandler(this.BtnSwtichPositions_Click);
            // 
            // lvCancelOrders
            // 
            this.lvCancelOrders.BackColor = System.Drawing.Color.LightCoral;
            this.lvCancelOrders.BackgroundImageTiled = true;
            this.lvCancelOrders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvCancelOrders.ContextMenuStrip = this.cmsCancelOrders;
            this.lvCancelOrders.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvCancelOrders.ForeColor = System.Drawing.SystemColors.Info;
            this.lvCancelOrders.HideSelection = false;
            this.lvCancelOrders.Location = new System.Drawing.Point(283, -2);
            this.lvCancelOrders.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvCancelOrders.MultiSelect = false;
            this.lvCancelOrders.Name = "lvCancelOrders";
            this.lvCancelOrders.Size = new System.Drawing.Size(701, 178);
            this.lvCancelOrders.TabIndex = 3;
            this.lvCancelOrders.UseCompatibleStateImageBehavior = false;
            this.lvCancelOrders.View = System.Windows.Forms.View.Details;
            this.lvCancelOrders.Visible = false;
            // 
            // cmsCancelOrders
            // 
            this.cmsCancelOrders.BackColor = System.Drawing.SystemColors.Control;
            this.cmsCancelOrders.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsCancelOrders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiCancel});
            this.cmsCancelOrders.Name = "cmsPositionsMenu";
            this.cmsCancelOrders.Size = new System.Drawing.Size(106, 49);
            this.cmsCancelOrders.Text = "持仓菜单";
            // 
            // tsmiCancel
            // 
            this.tsmiCancel.AutoSize = false;
            this.tsmiCancel.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsmiCancel.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tsmiCancel.Name = "tsmiCancel";
            this.tsmiCancel.Size = new System.Drawing.Size(214, 35);
            this.tsmiCancel.Text = "撤单";
            this.tsmiCancel.Click += new System.EventHandler(this.TsmiCancel_Click);
            // 
            // lvPositions
            // 
            this.lvPositions.BackColor = System.Drawing.Color.LightCoral;
            this.lvPositions.BackgroundImageTiled = true;
            this.lvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvPositions.ContextMenuStrip = this.cmsPositions;
            this.lvPositions.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvPositions.ForeColor = System.Drawing.SystemColors.Info;
            this.lvPositions.HideSelection = false;
            this.lvPositions.Location = new System.Drawing.Point(283, -2);
            this.lvPositions.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.lvPositions.MultiSelect = false;
            this.lvPositions.Name = "lvPositions";
            this.lvPositions.Size = new System.Drawing.Size(701, 178);
            this.lvPositions.TabIndex = 2;
            this.lvPositions.UseCompatibleStateImageBehavior = false;
            this.lvPositions.View = System.Windows.Forms.View.Details;
            // 
            // cmsPositions
            // 
            this.cmsPositions.BackColor = System.Drawing.SystemColors.Control;
            this.cmsPositions.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsPositions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSell,
            this.tsmiSellInForm});
            this.cmsPositions.Name = "cmsPositionsMenu";
            this.cmsPositions.Size = new System.Drawing.Size(134, 94);
            this.cmsPositions.Text = "持仓菜单";
            // 
            // tsmiSell
            // 
            this.tsmiSell.AutoSize = false;
            this.tsmiSell.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsmiSell.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tsmiSell.Name = "tsmiSell";
            this.tsmiSell.Size = new System.Drawing.Size(214, 40);
            this.tsmiSell.Text = "一键卖出";
            this.tsmiSell.Click += new System.EventHandler(this.TsmiSell_Click);
            // 
            // tsmiSellInForm
            // 
            this.tsmiSellInForm.AutoSize = false;
            this.tsmiSellInForm.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tsmiSellInForm.Name = "tsmiSellInForm";
            this.tsmiSellInForm.Size = new System.Drawing.Size(192, 40);
            this.tsmiSellInForm.Text = "窗口卖出";
            this.tsmiSellInForm.Click += new System.EventHandler(this.TsmiSellInForm_Click);
            // 
            // pbWorkStatus
            // 
            this.pbWorkStatus.BackColor = System.Drawing.Color.Transparent;
            this.pbWorkStatus.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbWorkStatus.Image = global::NineSunScripture.Properties.Resources.taiji;
            this.pbWorkStatus.InitialImage = null;
            this.pbWorkStatus.Location = new System.Drawing.Point(360, 229);
            this.pbWorkStatus.Name = "pbWorkStatus";
            this.pbWorkStatus.Size = new System.Drawing.Size(517, 510);
            this.pbWorkStatus.TabIndex = 3;
            this.pbWorkStatus.TabStop = false;
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
            this.MenuItemManageAcct.Size = new System.Drawing.Size(134, 24);
            this.MenuItemManageAcct.Text = "账号管理";
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(134, 24);
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
            this.toolStripSeparator1,
            this.tsmiBuyStock,
            this.toolStripSeparator2,
            this.tsmiManageAcct,
            this.toolStripSeparator3,
            this.tsmiClearPositions,
            this.toolStripSeparator4,
            this.tspiPrivacyMode,
            this.toolStripSeparator5,
            this.tsmiTest,
            this.toolStripSeparator6,
            this.tspExit,
            this.toolStripSeparator7});
            this.管理菜单ToolStripMenuItem.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.管理菜单ToolStripMenuItem.ForeColor = System.Drawing.Color.DarkGreen;
            this.管理菜单ToolStripMenuItem.Name = "管理菜单ToolStripMenuItem";
            this.管理菜单ToolStripMenuItem.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.管理菜单ToolStripMenuItem.Size = new System.Drawing.Size(134, 36);
            this.管理菜单ToolStripMenuItem.Text = "管理菜单";
            // 
            // tsmiSwitch
            // 
            this.tsmiSwitch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsmiSwitch.AutoSize = false;
            this.tsmiSwitch.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tsmiSwitch.Name = "tsmiSwitch";
            this.tsmiSwitch.Padding = new System.Windows.Forms.Padding(0, 10, 0, 1);
            this.tsmiSwitch.Size = new System.Drawing.Size(150, 30);
            this.tsmiSwitch.Text = "启动";
            this.tsmiSwitch.Click += new System.EventHandler(this.TsmiSwitch_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(131, 6);
            // 
            // tsmiBuyStock
            // 
            this.tsmiBuyStock.AutoSize = false;
            this.tsmiBuyStock.Name = "tsmiBuyStock";
            this.tsmiBuyStock.Padding = new System.Windows.Forms.Padding(0, 10, 0, 1);
            this.tsmiBuyStock.Size = new System.Drawing.Size(198, 30);
            this.tsmiBuyStock.Text = "买入";
            this.tsmiBuyStock.Click += new System.EventHandler(this.TsmiBuyStock_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(131, 6);
            // 
            // tsmiManageAcct
            // 
            this.tsmiManageAcct.AutoSize = false;
            this.tsmiManageAcct.Name = "tsmiManageAcct";
            this.tsmiManageAcct.Padding = new System.Windows.Forms.Padding(0, 10, 0, 1);
            this.tsmiManageAcct.Size = new System.Drawing.Size(150, 30);
            this.tsmiManageAcct.Text = "账号管理";
            this.tsmiManageAcct.Click += new System.EventHandler(this.TsmiManageAcct_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(131, 6);
            // 
            // tsmiClearPositions
            // 
            this.tsmiClearPositions.AutoSize = false;
            this.tsmiClearPositions.Name = "tsmiClearPositions";
            this.tsmiClearPositions.Size = new System.Drawing.Size(150, 30);
            this.tsmiClearPositions.Text = "一键清仓";
            this.tsmiClearPositions.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.tsmiClearPositions.Click += new System.EventHandler(this.TsmiClearPositions_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(131, 6);
            // 
            // tspiPrivacyMode
            // 
            this.tspiPrivacyMode.AutoSize = false;
            this.tspiPrivacyMode.Name = "tspiPrivacyMode";
            this.tspiPrivacyMode.Size = new System.Drawing.Size(150, 30);
            this.tspiPrivacyMode.Text = "隐私模式";
            this.tspiPrivacyMode.Click += new System.EventHandler(this.TspiPrivacyMode_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(131, 6);
            // 
            // tsmiTest
            // 
            this.tsmiTest.AutoSize = false;
            this.tsmiTest.Name = "tsmiTest";
            this.tsmiTest.Size = new System.Drawing.Size(198, 30);
            this.tsmiTest.Text = "测试";
            this.tsmiTest.Click += new System.EventHandler(this.TsmiTest_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(131, 6);
            // 
            // tspExit
            // 
            this.tspExit.AutoSize = false;
            this.tspExit.Name = "tspExit";
            this.tspExit.Size = new System.Drawing.Size(150, 30);
            this.tspExit.Text = "退出";
            this.tspExit.Click += new System.EventHandler(this.TspExit_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(131, 6);
            // 
            // tbRuntimeInfo
            // 
            this.tbRuntimeInfo.BackColor = System.Drawing.Color.LightCoral;
            this.tbRuntimeInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbRuntimeInfo.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbRuntimeInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.tbRuntimeInfo.Location = new System.Drawing.Point(2, 45);
            this.tbRuntimeInfo.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tbRuntimeInfo.Multiline = true;
            this.tbRuntimeInfo.Name = "tbRuntimeInfo";
            this.tbRuntimeInfo.ReadOnly = true;
            this.tbRuntimeInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbRuntimeInfo.Size = new System.Drawing.Size(300, 666);
            this.tbRuntimeInfo.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 18.33962F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Yellow;
            this.label3.Location = new System.Drawing.Point(-2, 0);
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
            this.panel2.Controls.Add(this.tbRuntimeInfo);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Location = new System.Drawing.Point(20, 71);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(306, 718);
            this.panel2.TabIndex = 16;
            // 
            // btnPeriod
            // 
            this.btnPeriod.BackColor = System.Drawing.Color.Red;
            this.btnPeriod.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.btnPeriod.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPeriod.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPeriod.Location = new System.Drawing.Point(555, 68);
            this.btnPeriod.Name = "btnPeriod";
            this.btnPeriod.Size = new System.Drawing.Size(126, 42);
            this.btnPeriod.TabIndex = 23;
            this.btnPeriod.Text = "上升期";
            this.btnPeriod.UseVisualStyleBackColor = false;
            this.btnPeriod.Click += new System.EventHandler(this.BtnUpPeriod_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightCoral;
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1211, 1014);
            this.Controls.Add(this.btnPeriod);
            this.Controls.Add(this.pbWorkStatus);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelFundInfo);
            this.Controls.Add(this.flpStockPool);
            this.Controls.Add(this.MenuMain);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("宋体", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuMain;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "九阳真经策略";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Closing);
            this.flpStockPool.ResumeLayout(false);
            this.cmsStocks.ResumeLayout(false);
            this.panelFundInfo.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.cmsCancelOrders.ResumeLayout(false);
            this.cmsPositions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbWorkStatus)).EndInit();
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.ComponentModel.ComponentResourceManager resources;
        private System.Windows.Forms.FlowLayoutPanel flpStockPool;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotalAsset;
        private System.Windows.Forms.Label lblMoneyAvailable;
        private System.Windows.Forms.Panel panelFundInfo;
        private System.Windows.Forms.ToolStripMenuItem Menu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemManageAcct;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.MenuStrip MenuMain;
        private System.Windows.Forms.ContextMenuStrip cmsStocks;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddStock;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelStock;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearStocks;
        private System.Windows.Forms.TextBox tbRuntimeInfo;
        private System.Windows.Forms.ListView lvPositions;
        private System.Windows.Forms.ToolStripMenuItem 管理菜单ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiManageAcct;
        private System.Windows.Forms.ToolStripMenuItem tspExit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem tsmiSwitch;
        private System.Windows.Forms.ToolStripMenuItem tsmiClearPositions;
        private System.Windows.Forms.ToolStripMenuItem tsmiBuy;
        private System.Windows.Forms.ListView lvStocks;
        private System.Windows.Forms.ToolStripMenuItem tsmiBuyStock;
        private System.Windows.Forms.ContextMenuStrip cmsPositions;
        private System.Windows.Forms.ToolStripMenuItem tsmiSell;
        private System.Windows.Forms.ToolStripMenuItem tsmiSellInForm;
        private System.Windows.Forms.ToolStripMenuItem tsmiTest;
        private System.Windows.Forms.PictureBox pbWorkStatus;
        private System.Windows.Forms.ListView lvCancelOrders;
        private System.Windows.Forms.ContextMenuStrip cmsCancelOrders;
        private System.Windows.Forms.ToolStripMenuItem tsmiCancel;
        private System.Windows.Forms.Button btnSwitchCancelOrders;
        private System.Windows.Forms.Button btnSwtichPositions;
        private System.Windows.Forms.Label lblTotalProfit;
        private System.Windows.Forms.ToolStripMenuItem tspiPrivacyMode;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.Label lblTotalPositionRatio;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnPeriod;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelGroup;
    }
}

