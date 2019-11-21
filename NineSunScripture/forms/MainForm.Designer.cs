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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.BtnStart = new System.Windows.Forms.Button();
            this.BtnSellAll = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.LvDragonLeaders = new System.Windows.Forms.ListView();
            this.BtnAddDragonLeader = new System.Windows.Forms.Button();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.label3 = new System.Windows.Forms.Label();
            this.LvLongTermStocks = new System.Windows.Forms.ListView();
            this.BtnAddLongTermSock = new System.Windows.Forms.Button();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.LvTomorrowStocks = new System.Windows.Forms.ListView();
            this.BtnAddTomorrowStock = new System.Windows.Forms.Button();
            this.LbRuntimeInfo = new System.Windows.Forms.Label();
            this.LbTotalAsset = new System.Windows.Forms.Label();
            this.LbMoneyAvailable = new System.Windows.Forms.Label();
            this.LbStock1 = new System.Windows.Forms.Label();
            this.LbStock2 = new System.Windows.Forms.Label();
            this.MenuMain = new System.Windows.Forms.MenuStrip();
            this.Menu = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemManageAcct = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.MenuMain.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.BtnStart.Text = "启动";
            this.BtnStart.UseVisualStyleBackColor = false;
            this.BtnStart.Click += new System.EventHandler(this.Button1_Click);
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
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.LvDragonLeaders);
            this.flowLayoutPanel1.Controls.Add(this.BtnAddDragonLeader);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(645, 55);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(308, 252);
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
            // LvDragonLeaders
            // 
            this.LvDragonLeaders.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LvDragonLeaders.Location = new System.Drawing.Point(3, 53);
            this.LvDragonLeaders.Name = "LvDragonLeaders";
            this.LvDragonLeaders.Size = new System.Drawing.Size(305, 120);
            this.LvDragonLeaders.TabIndex = 1;
            this.LvDragonLeaders.UseCompatibleStateImageBehavior = false;
            // 
            // BtnAddDragonLeader
            // 
            this.BtnAddDragonLeader.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnAddDragonLeader.Location = new System.Drawing.Point(3, 179);
            this.BtnAddDragonLeader.Name = "BtnAddDragonLeader";
            this.BtnAddDragonLeader.Size = new System.Drawing.Size(305, 66);
            this.BtnAddDragonLeader.TabIndex = 2;
            this.BtnAddDragonLeader.Text = "添加";
            this.BtnAddDragonLeader.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel3.Controls.Add(this.label3);
            this.flowLayoutPanel3.Controls.Add(this.LvLongTermStocks);
            this.flowLayoutPanel3.Controls.Add(this.BtnAddLongTermSock);
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(88, 55);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(313, 346);
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
            // LvLongTermStocks
            // 
            this.LvLongTermStocks.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LvLongTermStocks.Location = new System.Drawing.Point(3, 42);
            this.LvLongTermStocks.Name = "LvLongTermStocks";
            this.LvLongTermStocks.Size = new System.Drawing.Size(307, 222);
            this.LvLongTermStocks.TabIndex = 1;
            this.LvLongTermStocks.UseCompatibleStateImageBehavior = false;
            // 
            // BtnAddLongTermSock
            // 
            this.BtnAddLongTermSock.BackColor = System.Drawing.Color.Gainsboro;
            this.BtnAddLongTermSock.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnAddLongTermSock.Location = new System.Drawing.Point(3, 270);
            this.BtnAddLongTermSock.Name = "BtnAddLongTermSock";
            this.BtnAddLongTermSock.Size = new System.Drawing.Size(307, 63);
            this.BtnAddLongTermSock.TabIndex = 2;
            this.BtnAddLongTermSock.Text = "添加";
            this.BtnAddLongTermSock.UseVisualStyleBackColor = false;
            this.BtnAddLongTermSock.Click += new System.EventHandler(this.BtnAddLongTermSock_Click);
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.label2);
            this.flowLayoutPanel2.Controls.Add(this.LvTomorrowStocks);
            this.flowLayoutPanel2.Controls.Add(this.BtnAddTomorrowStock);
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(645, 371);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(308, 382);
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
            // LvTomorrowStocks
            // 
            this.LvTomorrowStocks.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LvTomorrowStocks.Location = new System.Drawing.Point(3, 42);
            this.LvTomorrowStocks.Name = "LvTomorrowStocks";
            this.LvTomorrowStocks.Size = new System.Drawing.Size(305, 261);
            this.LvTomorrowStocks.TabIndex = 1;
            this.LvTomorrowStocks.UseCompatibleStateImageBehavior = false;
            // 
            // BtnAddTomorrowStock
            // 
            this.BtnAddTomorrowStock.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BtnAddTomorrowStock.Location = new System.Drawing.Point(3, 309);
            this.BtnAddTomorrowStock.Name = "BtnAddTomorrowStock";
            this.BtnAddTomorrowStock.Size = new System.Drawing.Size(305, 64);
            this.BtnAddTomorrowStock.TabIndex = 2;
            this.BtnAddTomorrowStock.Text = "添加";
            this.BtnAddTomorrowStock.UseVisualStyleBackColor = true;
            // 
            // LbRuntimeInfo
            // 
            this.LbRuntimeInfo.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LbRuntimeInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.LbRuntimeInfo.Location = new System.Drawing.Point(85, 442);
            this.LbRuntimeInfo.Name = "LbRuntimeInfo";
            this.LbRuntimeInfo.Size = new System.Drawing.Size(313, 311);
            this.LbRuntimeInfo.TabIndex = 5;
            this.LbRuntimeInfo.Text = "RuntimeInfo";
            // 
            // LbTotalAsset
            // 
            this.LbTotalAsset.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LbTotalAsset.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.LbTotalAsset.Location = new System.Drawing.Point(3, 0);
            this.LbTotalAsset.Name = "LbTotalAsset";
            this.LbTotalAsset.Size = new System.Drawing.Size(209, 100);
            this.LbTotalAsset.TabIndex = 0;
            this.LbTotalAsset.Text = "TotalAsset";
            // 
            // LbMoneyAvailable
            // 
            this.LbMoneyAvailable.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LbMoneyAvailable.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.LbMoneyAvailable.Location = new System.Drawing.Point(218, 0);
            this.LbMoneyAvailable.Name = "LbMoneyAvailable";
            this.LbMoneyAvailable.Size = new System.Drawing.Size(193, 100);
            this.LbMoneyAvailable.TabIndex = 1;
            this.LbMoneyAvailable.Text = "MoneyAvailable";
            // 
            // LbStock1
            // 
            this.LbStock1.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LbStock1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.LbStock1.Location = new System.Drawing.Point(477, 0);
            this.LbStock1.Name = "LbStock1";
            this.LbStock1.Size = new System.Drawing.Size(202, 100);
            this.LbStock1.TabIndex = 1;
            this.LbStock1.Text = "Stock1";
            // 
            // LbStock2
            // 
            this.LbStock2.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LbStock2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.LbStock2.Location = new System.Drawing.Point(685, 0);
            this.LbStock2.Name = "LbStock2";
            this.LbStock2.Size = new System.Drawing.Size(177, 100);
            this.LbStock2.TabIndex = 2;
            this.LbStock2.Text = "Stock2";
            // 
            // MenuMain
            // 
            this.MenuMain.AllowDrop = true;
            this.MenuMain.ImageScalingSize = new System.Drawing.Size(18, 18);
            this.MenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu});
            this.MenuMain.Location = new System.Drawing.Point(0, 0);
            this.MenuMain.Name = "MenuMain";
            this.MenuMain.Size = new System.Drawing.Size(1062, 28);
            this.MenuMain.TabIndex = 7;
            this.MenuMain.Text = "菜单";
            // 
            // Menu
            // 
            this.Menu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuItemManageAcct,
            this.MenuItemExit});
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(77, 24);
            this.Menu.Text = "管理菜单";
            // 
            // MenuItemManageAcct
            // 
            this.MenuItemManageAcct.Name = "MenuItemManageAcct";
            this.MenuItemManageAcct.Size = new System.Drawing.Size(140, 24);
            this.MenuItemManageAcct.Text = "账号管理";
            this.MenuItemManageAcct.Click += new System.EventHandler(this.MenuItemManageAcct_Click);
            // 
            // MenuItemExit
            // 
            this.MenuItemExit.Name = "MenuItemExit";
            this.MenuItemExit.Size = new System.Drawing.Size(140, 24);
            this.MenuItemExit.Text = "Exit";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.LbTotalAsset);
            this.panel1.Controls.Add(this.LbStock2);
            this.panel1.Controls.Add(this.LbMoneyAvailable);
            this.panel1.Controls.Add(this.LbStock1);
            this.panel1.Location = new System.Drawing.Point(88, 787);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(865, 100);
            this.panel1.TabIndex = 8;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkCyan;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.ClientSize = new System.Drawing.Size(1062, 1031);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.LbRuntimeInfo);
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
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.MenuMain.ResumeLayout(false);
            this.MenuMain.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStart;
        private System.Windows.Forms.Button BtnSellAll;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView LvDragonLeaders;
        private System.Windows.Forms.Button BtnAddDragonLeader;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListView LvLongTermStocks;
        private System.Windows.Forms.Button BtnAddLongTermSock;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView LvTomorrowStocks;
        private System.Windows.Forms.Button BtnAddTomorrowStock;
        private System.Windows.Forms.Label LbRuntimeInfo;
        private System.Windows.Forms.Label LbTotalAsset;
        private System.Windows.Forms.Label LbMoneyAvailable;
        private System.Windows.Forms.Label LbStock1;
        private System.Windows.Forms.Label LbStock2;
        private System.Windows.Forms.MenuStrip MenuMain;
        private System.Windows.Forms.ToolStripMenuItem Menu;
        private System.Windows.Forms.ToolStripMenuItem MenuItemManageAcct;
        private System.Windows.Forms.ToolStripMenuItem MenuItemExit;
        private System.Windows.Forms.Panel panel1;
    }
}

