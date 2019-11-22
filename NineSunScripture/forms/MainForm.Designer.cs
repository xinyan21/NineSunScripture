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
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnSellAll = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.lvDragonLeaders = new System.Windows.Forms.ListView();
            this.cmsDragonLeader = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmAddDragonLeader = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDelDragonLeader = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmClearDragonLeaders = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.lvLongTermStocks = new System.Windows.Forms.ListView();
            this.cmsLongTerm = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tspAddLongTermStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tspDelLongTerm = new System.Windows.Forms.ToolStripMenuItem();
            this.tspClearLongTerm = new System.Windows.Forms.ToolStripMenuItem();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.lvTomorrowStocks = new System.Windows.Forms.ListView();
            this.cmsTomorrowStocks = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmAddTomorrowStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmpDelTomorrowStock = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmClearTomorrowStocks = new System.Windows.Forms.ToolStripMenuItem();
            this.lblTotalAsset = new System.Windows.Forms.Label();
            this.lblMoneyAvailable = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvPositions = new System.Windows.Forms.DataGridView();
            this.Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemManageAcct = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuMain = new System.Windows.Forms.MenuStrip();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbRuntimeInfo = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.cmsDragonLeader.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.cmsLongTerm.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.cmsTomorrowStocks.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPositions)).BeginInit();
            this.MenuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BtnStart
            // 
            this.BtnStart.BackColor = System.Drawing.Color.Red;
            this.BtnStart.Font = new System.Drawing.Font("微软雅黑", 14.26415F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnStart.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BtnStart.Location = new System.Drawing.Point(645, 918);
            this.BtnStart.Name = "BtnStart";
            this.BtnStart.Size = new System.Drawing.Size(308, 74);
            this.BtnStart.TabIndex = 0;
            this.BtnStart.Text = "启  动";
            this.BtnStart.UseVisualStyleBackColor = false;
            this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // BtnSellAll
            // 
            this.BtnSellAll.BackColor = System.Drawing.Color.Green;
            this.BtnSellAll.Font = new System.Drawing.Font("微软雅黑", 14.26415F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnSellAll.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.BtnSellAll.Location = new System.Drawing.Point(88, 918);
            this.BtnSellAll.Name = "BtnSellAll";
            this.BtnSellAll.Size = new System.Drawing.Size(310, 74);
            this.BtnSellAll.TabIndex = 1;
            this.BtnSellAll.Text = "一键清仓";
            this.BtnSellAll.UseVisualStyleBackColor = false;
            this.BtnSellAll.Click += new System.EventHandler(this.BtnSellAll_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.lvDragonLeaders);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(645, 55);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(308, 274);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("微软雅黑", 18.33962F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Gold;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(305, 50);
            this.label1.TabIndex = 0;
            this.label1.Text = "龙头";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvDragonLeaders
            // 
            this.lvDragonLeaders.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.lvDragonLeaders.ContextMenuStrip = this.cmsDragonLeader;
            this.lvDragonLeaders.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvDragonLeaders.Location = new System.Drawing.Point(3, 53);
            this.lvDragonLeaders.Name = "lvDragonLeaders";
            this.lvDragonLeaders.Size = new System.Drawing.Size(305, 211);
            this.lvDragonLeaders.TabIndex = 1;
            this.lvDragonLeaders.UseCompatibleStateImageBehavior = false;
            // 
            // cmsDragonLeader
            // 
            this.cmsDragonLeader.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsDragonLeader.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddDragonLeader,
            this.tsmDelDragonLeader,
            this.tsmClearDragonLeaders});
            this.cmsDragonLeader.Name = "cmsDragonLeader";
            this.cmsDragonLeader.Size = new System.Drawing.Size(135, 76);
            this.cmsDragonLeader.Text = "股票池管理";
            // 
            // tsmAddDragonLeader
            // 
            this.tsmAddDragonLeader.Name = "tsmAddDragonLeader";
            this.tsmAddDragonLeader.Size = new System.Drawing.Size(134, 24);
            this.tsmAddDragonLeader.Text = "添加龙头";
            this.tsmAddDragonLeader.Click += new System.EventHandler(this.tsmAddDragonLeader_Click);
            // 
            // tsmDelDragonLeader
            // 
            this.tsmDelDragonLeader.Name = "tsmDelDragonLeader";
            this.tsmDelDragonLeader.Size = new System.Drawing.Size(134, 24);
            this.tsmDelDragonLeader.Text = "删除";
            // 
            // tsmClearDragonLeaders
            // 
            this.tsmClearDragonLeaders.Name = "tsmClearDragonLeaders";
            this.tsmClearDragonLeaders.Size = new System.Drawing.Size(134, 24);
            this.tsmClearDragonLeaders.Text = "清空";
            this.tsmClearDragonLeaders.Click += new System.EventHandler(this.tsmClearDragonLeaders_Click);
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Controls.Add(this.lvLongTermStocks);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(88, 55);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(310, 274);
            this.flowLayoutPanel3.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("微软雅黑", 16.30189F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.Color.Yellow;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(307, 39);
            this.label3.TabIndex = 0;
            this.label3.Text = "常驻股票池";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvLongTermStocks
            // 
            this.lvLongTermStocks.BackColor = System.Drawing.Color.PeachPuff;
            this.lvLongTermStocks.ContextMenuStrip = this.cmsLongTerm;
            this.lvLongTermStocks.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvLongTermStocks.Location = new System.Drawing.Point(3, 42);
            this.lvLongTermStocks.Name = "lvLongTermStocks";
            this.lvLongTermStocks.Size = new System.Drawing.Size(307, 222);
            this.lvLongTermStocks.TabIndex = 1;
            this.lvLongTermStocks.UseCompatibleStateImageBehavior = false;
            // 
            // cmsLongTerm
            // 
            this.cmsLongTerm.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsLongTerm.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tspAddLongTermStock,
            this.tspDelLongTerm,
            this.tspClearLongTerm});
            this.cmsLongTerm.Name = "cmsLongTerm";
            this.cmsLongTerm.Size = new System.Drawing.Size(135, 76);
            this.cmsLongTerm.Text = "股票池管理";
            // 
            // tspAddLongTermStock
            // 
            this.tspAddLongTermStock.Name = "tspAddLongTermStock";
            this.tspAddLongTermStock.Size = new System.Drawing.Size(134, 24);
            this.tspAddLongTermStock.Text = "添加股票";
            this.tspAddLongTermStock.Click += new System.EventHandler(this.tspAddLongTermStock_Click);
            // 
            // tspDelLongTerm
            // 
            this.tspDelLongTerm.Name = "tspDelLongTerm";
            this.tspDelLongTerm.Size = new System.Drawing.Size(134, 24);
            this.tspDelLongTerm.Text = "删除";
            this.tspDelLongTerm.Click += new System.EventHandler(this.tspDelLongTerm_Click);
            // 
            // tspClearLongTerm
            // 
            this.tspClearLongTerm.Name = "tspClearLongTerm";
            this.tspClearLongTerm.Size = new System.Drawing.Size(134, 24);
            this.tspClearLongTerm.Text = "清空";
            this.tspClearLongTerm.Click += new System.EventHandler(this.tspClearLongTerm_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.lvTomorrowStocks);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(645, 371);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(308, 372);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("微软雅黑", 18.33962F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.Color.Yellow;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(305, 39);
            this.label2.TabIndex = 0;
            this.label2.Text = "明日股票池";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lvTomorrowStocks
            // 
            this.lvTomorrowStocks.BackColor = System.Drawing.Color.PeachPuff;
            this.lvTomorrowStocks.ContextMenuStrip = this.cmsTomorrowStocks;
            this.lvTomorrowStocks.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvTomorrowStocks.Location = new System.Drawing.Point(3, 42);
            this.lvTomorrowStocks.Name = "lvTomorrowStocks";
            this.lvTomorrowStocks.Size = new System.Drawing.Size(303, 319);
            this.lvTomorrowStocks.TabIndex = 1;
            this.lvTomorrowStocks.UseCompatibleStateImageBehavior = false;
            // 
            // cmsTomorrowStocks
            // 
            this.cmsTomorrowStocks.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.cmsTomorrowStocks.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddTomorrowStock,
            this.tsmpDelTomorrowStock,
            this.tsmClearTomorrowStocks});
            this.cmsTomorrowStocks.Name = "cmsTomorrow";
            this.cmsTomorrowStocks.Size = new System.Drawing.Size(163, 76);
            this.cmsTomorrowStocks.Text = "股票池管理";
            // 
            // tsmAddTomorrowStock
            // 
            this.tsmAddTomorrowStock.Name = "tsmAddTomorrowStock";
            this.tsmAddTomorrowStock.Size = new System.Drawing.Size(162, 24);
            this.tsmAddTomorrowStock.Text = "添加明日股票";
            this.tsmAddTomorrowStock.Click += new System.EventHandler(this.tsmAddTomorrowStocks_Click);
            // 
            // tsmpDelTomorrowStock
            // 
            this.tsmpDelTomorrowStock.Name = "tsmpDelTomorrowStock";
            this.tsmpDelTomorrowStock.Size = new System.Drawing.Size(162, 24);
            this.tsmpDelTomorrowStock.Text = "删除";
            // 
            // tsmClearTomorrowStocks
            // 
            this.tsmClearTomorrowStocks.Name = "tsmClearTomorrowStocks";
            this.tsmClearTomorrowStocks.Size = new System.Drawing.Size(162, 24);
            this.tsmClearTomorrowStocks.Text = "清空";
            this.tsmClearTomorrowStocks.Click += new System.EventHandler(this.tsmClearTomorrowStocks_Click);
            // 
            // lblTotalAsset
            // 
            this.lblTotalAsset.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTotalAsset.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblTotalAsset.Location = new System.Drawing.Point(3, 0);
            this.lblTotalAsset.Name = "lblTotalAsset";
            this.lblTotalAsset.Size = new System.Drawing.Size(209, 103);
            this.lblTotalAsset.TabIndex = 0;
            this.lblTotalAsset.Text = "TotalAsset";
            // 
            // lblMoneyAvailable
            // 
            this.lblMoneyAvailable.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMoneyAvailable.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblMoneyAvailable.Location = new System.Drawing.Point(218, 0);
            this.lblMoneyAvailable.Name = "lblMoneyAvailable";
            this.lblMoneyAvailable.Size = new System.Drawing.Size(193, 103);
            this.lblMoneyAvailable.TabIndex = 1;
            this.lblMoneyAvailable.Text = "MoneyAvailable";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.dgvPositions);
            this.panel1.Controls.Add(this.lblTotalAsset);
            this.panel1.Controls.Add(this.lblMoneyAvailable);
            this.panel1.Location = new System.Drawing.Point(85, 780);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(865, 107);
            this.panel1.TabIndex = 8;
            // 
            // dgvPositions
            // 
            this.dgvPositions.BackgroundColor = System.Drawing.Color.Sienna;
            this.dgvPositions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPositions.GridColor = System.Drawing.Color.Sienna;
            this.dgvPositions.Location = new System.Drawing.Point(417, 0);
            this.dgvPositions.Name = "dgvPositions";
            this.dgvPositions.RowHeadersWidth = 45;
            this.dgvPositions.RowTemplate.Height = 24;
            this.dgvPositions.Size = new System.Drawing.Size(446, 105);
            this.dgvPositions.TabIndex = 2;
            // 
            // Menu
            // 
            this.Menu.AutoSize = false;
            this.Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemManageAcct,
            this.MenuItemExit});
            this.Menu.Font = new System.Drawing.Font("Microsoft YaHei UI", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
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
            this.MenuMain.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu});
            this.MenuMain.Location = new System.Drawing.Point(0, 0);
            this.MenuMain.Name = "MenuMain";
            this.MenuMain.Size = new System.Drawing.Size(1062, 35);
            this.MenuMain.TabIndex = 7;
            this.MenuMain.Text = "菜单";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pictureBox1.BackgroundImage")));
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.Location = new System.Drawing.Point(465, 908);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(125, 111);
            this.pictureBox1.TabIndex = 12;
            this.pictureBox1.TabStop = false;
            // 
            // tbRuntimeInfo
            // 
            this.tbRuntimeInfo.BackColor = System.Drawing.Color.Sienna;
            this.tbRuntimeInfo.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbRuntimeInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.tbRuntimeInfo.Location = new System.Drawing.Point(93, 371);
            this.tbRuntimeInfo.Multiline = true;
            this.tbRuntimeInfo.Name = "tbRuntimeInfo";
            this.tbRuntimeInfo.ReadOnly = true;
            this.tbRuntimeInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbRuntimeInfo.Size = new System.Drawing.Size(305, 363);
            this.tbRuntimeInfo.TabIndex = 14;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Sienna;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1062, 1031);
            this.Controls.Add(this.tbRuntimeInfo);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Controls.Add(this.flowLayoutPanel3);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.BtnSellAll);
            this.Controls.Add(this.BtnStart);
            this.Controls.Add(this.MenuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.MenuMain;
            this.Name = "MainForm";
            this.Text = "九阳真经";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.cmsDragonLeader.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.cmsLongTerm.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.cmsTomorrowStocks.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPositions)).EndInit();
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnSellAll;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvDragonLeaders;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView lvLongTermStocks;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView lvTomorrowStocks;
        private System.Windows.Forms.Label lblTotalAsset;
        private System.Windows.Forms.Label lblMoneyAvailable;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem Menu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemManageAcct;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.MenuStrip MenuMain;
        private System.Windows.Forms.DataGridView dgvPositions;
        private System.Windows.Forms.ContextMenuStrip cmsDragonLeader;
        private System.Windows.Forms.ContextMenuStrip cmsLongTerm;
        private System.Windows.Forms.ToolStripMenuItem tspAddLongTermStock;
        private System.Windows.Forms.ToolStripMenuItem tspDelLongTerm;
        private System.Windows.Forms.ToolStripMenuItem tspClearLongTerm;
        private System.Windows.Forms.ContextMenuStrip cmsTomorrowStocks;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem tsmAddDragonLeader;
        private System.Windows.Forms.ToolStripMenuItem tsmDelDragonLeader;
        private System.Windows.Forms.ToolStripMenuItem tsmClearDragonLeaders;
        private System.Windows.Forms.ToolStripMenuItem tsmAddTomorrowStock;
        private System.Windows.Forms.ToolStripMenuItem tsmpDelTomorrowStock;
        private System.Windows.Forms.ToolStripMenuItem tsmClearTomorrowStocks;
        private System.Windows.Forms.TextBox tbRuntimeInfo;
    }
}

