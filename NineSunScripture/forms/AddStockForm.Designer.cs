﻿namespace NineSunScripture.forms
{
    partial class AddStockForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddStockForm));
            this.btnAddStcok = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCode = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbMoney = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.tbPosition = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnBand = new System.Windows.Forms.RadioButton();
            this.rbWeakTurnStrong = new System.Windows.Forms.RadioButton();
            this.rbtnTomorrow = new System.Windows.Forms.RadioButton();
            this.rbtnLongTerm = new System.Windows.Forms.RadioButton();
            this.rbtnDragonLeader = new System.Windows.Forms.RadioButton();
            this.lblStopWinPrice = new System.Windows.Forms.Label();
            this.lblStopLossPrice = new System.Windows.Forms.Label();
            this.panelBandParam = new System.Windows.Forms.Panel();
            this.tbStopLossPrice = new System.Windows.Forms.TextBox();
            this.tbStopWinPrice = new System.Windows.Forms.TextBox();
            this.panelContBoards = new System.Windows.Forms.Panel();
            this.tbAvgCost = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbContBoards = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnSell = new System.Windows.Forms.RadioButton();
            this.rbtnBuy = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbtnNo = new System.Windows.Forms.RadioButton();
            this.rbtnYes = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.panelBandParam.SuspendLayout();
            this.panelContBoards.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddStcok
            // 
            this.btnAddStcok.BackColor = System.Drawing.Color.Red;
            this.btnAddStcok.BackgroundImage = global::NineSunScripture.Properties.Resources.btn_red2;
            this.btnAddStcok.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnAddStcok.FlatAppearance.BorderSize = 0;
            this.btnAddStcok.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddStcok.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAddStcok.ForeColor = System.Drawing.Color.Snow;
            this.btnAddStcok.Location = new System.Drawing.Point(221, 468);
            this.btnAddStcok.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddStcok.Name = "btnAddStcok";
            this.btnAddStcok.Size = new System.Drawing.Size(305, 72);
            this.btnAddStcok.TabIndex = 5;
            this.btnAddStcok.Text = "添  加";
            this.btnAddStcok.UseVisualStyleBackColor = false;
            this.btnAddStcok.Click += new System.EventHandler(this.BtnAddStcok_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(90, 107);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "代码：";
            // 
            // tbCode
            // 
            this.tbCode.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCode.Location = new System.Drawing.Point(172, 101);
            this.tbCode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbCode.MaxLength = 6;
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(147, 29);
            this.tbCode.TabIndex = 1;
            this.tbCode.TextChanged += new System.EventHandler(this.TbCode_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(475, 162);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(78, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "成交额：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(90, 162);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 23);
            this.label3.TabIndex = 10;
            this.label3.Text = "仓位：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(486, 104);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 23);
            this.label4.TabIndex = 11;
            this.label4.Text = "名称：";
            // 
            // tbMoney
            // 
            this.tbMoney.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbMoney.Location = new System.Drawing.Point(565, 156);
            this.tbMoney.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbMoney.MaxLength = 5;
            this.tbMoney.Name = "tbMoney";
            this.tbMoney.Size = new System.Drawing.Size(147, 29);
            this.tbMoney.TabIndex = 4;
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.Location = new System.Drawing.Point(565, 104);
            this.tbName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbName.MaxLength = 4;
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(147, 29);
            this.tbName.TabIndex = 11;
            // 
            // tbPosition
            // 
            this.tbPosition.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPosition.Location = new System.Drawing.Point(172, 159);
            this.tbPosition.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbPosition.MaxLength = 4;
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(147, 29);
            this.tbPosition.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(90, 220);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 23);
            this.label5.TabIndex = 12;
            this.label5.Text = "分类：";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.rbtnBand);
            this.groupBox1.Controls.Add(this.rbWeakTurnStrong);
            this.groupBox1.Controls.Add(this.rbtnTomorrow);
            this.groupBox1.Controls.Add(this.rbtnLongTerm);
            this.groupBox1.Controls.Add(this.rbtnDragonLeader);
            this.groupBox1.Location = new System.Drawing.Point(172, 198);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(540, 56);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // rbtnBand
            // 
            this.rbtnBand.AutoSize = true;
            this.rbtnBand.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnBand.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnBand.Location = new System.Drawing.Point(97, 19);
            this.rbtnBand.Name = "rbtnBand";
            this.rbtnBand.Size = new System.Drawing.Size(62, 27);
            this.rbtnBand.TabIndex = 5;
            this.rbtnBand.Text = "波段";
            this.rbtnBand.UseVisualStyleBackColor = true;
            this.rbtnBand.CheckedChanged += new System.EventHandler(this.RbtnBand_CheckedChanged);
            // 
            // rbWeakTurnStrong
            // 
            this.rbWeakTurnStrong.AutoSize = true;
            this.rbWeakTurnStrong.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbWeakTurnStrong.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbWeakTurnStrong.Location = new System.Drawing.Point(182, 19);
            this.rbWeakTurnStrong.Name = "rbWeakTurnStrong";
            this.rbWeakTurnStrong.Size = new System.Drawing.Size(79, 27);
            this.rbWeakTurnStrong.TabIndex = 0;
            this.rbWeakTurnStrong.Text = "弱转强";
            this.rbWeakTurnStrong.UseVisualStyleBackColor = true;
            this.rbWeakTurnStrong.CheckedChanged += new System.EventHandler(this.RbWeakTurnStrong_CheckedChanged);
            // 
            // rbtnTomorrow
            // 
            this.rbtnTomorrow.AutoSize = true;
            this.rbtnTomorrow.Checked = true;
            this.rbtnTomorrow.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnTomorrow.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnTomorrow.Location = new System.Drawing.Point(22, 19);
            this.rbtnTomorrow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnTomorrow.Name = "rbtnTomorrow";
            this.rbtnTomorrow.Size = new System.Drawing.Size(62, 27);
            this.rbtnTomorrow.TabIndex = 5;
            this.rbtnTomorrow.TabStop = true;
            this.rbtnTomorrow.Text = "打板";
            this.rbtnTomorrow.UseVisualStyleBackColor = true;
            this.rbtnTomorrow.CheckedChanged += new System.EventHandler(this.RbtnLatest_CheckedChanged);
            // 
            // rbtnLongTerm
            // 
            this.rbtnLongTerm.AutoSize = true;
            this.rbtnLongTerm.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnLongTerm.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnLongTerm.Location = new System.Drawing.Point(280, 19);
            this.rbtnLongTerm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnLongTerm.Name = "rbtnLongTerm";
            this.rbtnLongTerm.Size = new System.Drawing.Size(96, 27);
            this.rbtnLongTerm.TabIndex = 1;
            this.rbtnLongTerm.Text = "常驻打板";
            this.rbtnLongTerm.UseVisualStyleBackColor = true;
            this.rbtnLongTerm.CheckedChanged += new System.EventHandler(this.RbtnLongTerm_CheckedChanged);
            // 
            // rbtnDragonLeader
            // 
            this.rbtnDragonLeader.AutoSize = true;
            this.rbtnDragonLeader.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnDragonLeader.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnDragonLeader.Location = new System.Drawing.Point(384, 18);
            this.rbtnDragonLeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnDragonLeader.Name = "rbtnDragonLeader";
            this.rbtnDragonLeader.Size = new System.Drawing.Size(62, 27);
            this.rbtnDragonLeader.TabIndex = 0;
            this.rbtnDragonLeader.Text = "龙头";
            this.rbtnDragonLeader.UseVisualStyleBackColor = true;
            this.rbtnDragonLeader.CheckedChanged += new System.EventHandler(this.RbtnDragonLeader_CheckedChanged);
            // 
            // lblStopWinPrice
            // 
            this.lblStopWinPrice.AutoSize = true;
            this.lblStopWinPrice.BackColor = System.Drawing.Color.Transparent;
            this.lblStopWinPrice.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStopWinPrice.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblStopWinPrice.Location = new System.Drawing.Point(6, 21);
            this.lblStopWinPrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStopWinPrice.Name = "lblStopWinPrice";
            this.lblStopWinPrice.Size = new System.Drawing.Size(78, 23);
            this.lblStopWinPrice.TabIndex = 14;
            this.lblStopWinPrice.Text = "止盈价：";
            this.lblStopWinPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblStopLossPrice
            // 
            this.lblStopLossPrice.AutoSize = true;
            this.lblStopLossPrice.BackColor = System.Drawing.Color.Transparent;
            this.lblStopLossPrice.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStopLossPrice.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.lblStopLossPrice.Location = new System.Drawing.Point(384, 22);
            this.lblStopLossPrice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStopLossPrice.Name = "lblStopLossPrice";
            this.lblStopLossPrice.Size = new System.Drawing.Size(78, 23);
            this.lblStopLossPrice.TabIndex = 15;
            this.lblStopLossPrice.Text = "止损价：";
            // 
            // panelBandParam
            // 
            this.panelBandParam.BackColor = System.Drawing.Color.Transparent;
            this.panelBandParam.Controls.Add(this.tbStopLossPrice);
            this.panelBandParam.Controls.Add(this.tbStopWinPrice);
            this.panelBandParam.Controls.Add(this.lblStopWinPrice);
            this.panelBandParam.Controls.Add(this.lblStopLossPrice);
            this.panelBandParam.Location = new System.Drawing.Point(85, 346);
            this.panelBandParam.Name = "panelBandParam";
            this.panelBandParam.Size = new System.Drawing.Size(627, 67);
            this.panelBandParam.TabIndex = 16;
            // 
            // tbStopLossPrice
            // 
            this.tbStopLossPrice.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbStopLossPrice.Location = new System.Drawing.Point(480, 22);
            this.tbStopLossPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbStopLossPrice.MaxLength = 8;
            this.tbStopLossPrice.Name = "tbStopLossPrice";
            this.tbStopLossPrice.Size = new System.Drawing.Size(147, 29);
            this.tbStopLossPrice.TabIndex = 18;
            // 
            // tbStopWinPrice
            // 
            this.tbStopWinPrice.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbStopWinPrice.Location = new System.Drawing.Point(87, 20);
            this.tbStopWinPrice.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbStopWinPrice.MaxLength = 8;
            this.tbStopWinPrice.Name = "tbStopWinPrice";
            this.tbStopWinPrice.Size = new System.Drawing.Size(147, 29);
            this.tbStopWinPrice.TabIndex = 17;
            // 
            // panelContBoards
            // 
            this.panelContBoards.BackColor = System.Drawing.Color.Transparent;
            this.panelContBoards.Controls.Add(this.tbAvgCost);
            this.panelContBoards.Controls.Add(this.label8);
            this.panelContBoards.Controls.Add(this.tbContBoards);
            this.panelContBoards.Controls.Add(this.label7);
            this.panelContBoards.Location = new System.Drawing.Point(85, 262);
            this.panelContBoards.Name = "panelContBoards";
            this.panelContBoards.Size = new System.Drawing.Size(627, 67);
            this.panelContBoards.TabIndex = 19;
            // 
            // tbAvgCost
            // 
            this.tbAvgCost.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbAvgCost.Location = new System.Drawing.Point(480, 19);
            this.tbAvgCost.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbAvgCost.MaxLength = 8;
            this.tbAvgCost.Name = "tbAvgCost";
            this.tbAvgCost.Size = new System.Drawing.Size(147, 29);
            this.tbAvgCost.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label8.Location = new System.Drawing.Point(384, 22);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(78, 23);
            this.label8.TabIndex = 18;
            this.label8.Text = "成本价：";
            // 
            // tbContBoards
            // 
            this.tbContBoards.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbContBoards.Location = new System.Drawing.Point(87, 19);
            this.tbContBoards.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbContBoards.MaxLength = 4;
            this.tbContBoards.Name = "tbContBoards";
            this.tbContBoards.Size = new System.Drawing.Size(147, 29);
            this.tbContBoards.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label7.Location = new System.Drawing.Point(6, 22);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(78, 23);
            this.label7.TabIndex = 14;
            this.label7.Text = "连板数：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(90, 39);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 23);
            this.label6.TabIndex = 17;
            this.label6.Text = "操作：";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.rbtnSell);
            this.groupBox2.Controls.Add(this.rbtnBuy);
            this.groupBox2.Location = new System.Drawing.Point(172, 23);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox2.Size = new System.Drawing.Size(147, 57);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            // 
            // rbtnSell
            // 
            this.rbtnSell.AutoSize = true;
            this.rbtnSell.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnSell.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnSell.Location = new System.Drawing.Point(77, 22);
            this.rbtnSell.Name = "rbtnSell";
            this.rbtnSell.Size = new System.Drawing.Size(62, 27);
            this.rbtnSell.TabIndex = 5;
            this.rbtnSell.Text = "卖出";
            this.rbtnSell.UseVisualStyleBackColor = true;
            this.rbtnSell.CheckedChanged += new System.EventHandler(this.RbtnSell_CheckedChanged);
            // 
            // rbtnBuy
            // 
            this.rbtnBuy.AutoSize = true;
            this.rbtnBuy.Checked = true;
            this.rbtnBuy.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnBuy.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnBuy.Location = new System.Drawing.Point(8, 20);
            this.rbtnBuy.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnBuy.Name = "rbtnBuy";
            this.rbtnBuy.Size = new System.Drawing.Size(62, 27);
            this.rbtnBuy.TabIndex = 5;
            this.rbtnBuy.TabStop = true;
            this.rbtnBuy.Text = "买入";
            this.rbtnBuy.UseVisualStyleBackColor = true;
            this.rbtnBuy.CheckedChanged += new System.EventHandler(this.RbtnBuy_CheckedChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BackColor = System.Drawing.Color.Transparent;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label9.Location = new System.Drawing.Point(452, 39);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(95, 23);
            this.label9.TabIndex = 20;
            this.label9.Text = "回封买回：";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.rbtnNo);
            this.groupBox3.Controls.Add(this.rbtnYes);
            this.groupBox3.Location = new System.Drawing.Point(565, 23);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox3.Size = new System.Drawing.Size(147, 57);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            // 
            // rbtnNo
            // 
            this.rbtnNo.AutoSize = true;
            this.rbtnNo.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnNo.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnNo.Location = new System.Drawing.Point(95, 20);
            this.rbtnNo.Name = "rbtnNo";
            this.rbtnNo.Size = new System.Drawing.Size(45, 27);
            this.rbtnNo.TabIndex = 5;
            this.rbtnNo.Text = "否";
            this.rbtnNo.UseVisualStyleBackColor = true;
            this.rbtnNo.CheckedChanged += new System.EventHandler(this.RbtnNo_CheckedChanged);
            // 
            // rbtnYes
            // 
            this.rbtnYes.AutoSize = true;
            this.rbtnYes.Checked = true;
            this.rbtnYes.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnYes.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnYes.Location = new System.Drawing.Point(22, 20);
            this.rbtnYes.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnYes.Name = "rbtnYes";
            this.rbtnYes.Size = new System.Drawing.Size(45, 27);
            this.rbtnYes.TabIndex = 5;
            this.rbtnYes.TabStop = true;
            this.rbtnYes.Text = "是";
            this.rbtnYes.UseVisualStyleBackColor = true;
            this.rbtnYes.CheckedChanged += new System.EventHandler(this.RbtnYes_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.Transparent;
            this.label10.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label10.Location = new System.Drawing.Point(318, 162);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 23);
            this.label10.TabIndex = 21;
            this.label10.Text = "成";
            // 
            // AddStockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(793, 613);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.panelContBoards);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panelBandParam);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.tbPosition);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.tbMoney);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAddStcok);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "AddStockForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "添加股票";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelBandParam.ResumeLayout(false);
            this.panelBandParam.PerformLayout();
            this.panelContBoards.ResumeLayout(false);
            this.panelContBoards.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAddStcok;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbMoney;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.TextBox tbPosition;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnDragonLeader;
        private System.Windows.Forms.RadioButton rbtnTomorrow;
        private System.Windows.Forms.RadioButton rbtnLongTerm;
        private System.Windows.Forms.RadioButton rbWeakTurnStrong;
        private System.Windows.Forms.RadioButton rbtnBand;
        private System.Windows.Forms.Label lblStopWinPrice;
        private System.Windows.Forms.Label lblStopLossPrice;
        private System.Windows.Forms.Panel panelBandParam;
        private System.Windows.Forms.TextBox tbStopLossPrice;
        private System.Windows.Forms.TextBox tbStopWinPrice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbtnSell;
        private System.Windows.Forms.RadioButton rbtnBuy;
        private System.Windows.Forms.Panel panelContBoards;
        private System.Windows.Forms.TextBox tbContBoards;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbAvgCost;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton rbtnNo;
        private System.Windows.Forms.RadioButton rbtnYes;
        private System.Windows.Forms.Label label10;
    }
}