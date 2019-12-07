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
            this.rbtnTomorrow = new System.Windows.Forms.RadioButton();
            this.rbtnLongTerm = new System.Windows.Forms.RadioButton();
            this.rbtnDragonLeader = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
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
            this.btnAddStcok.Location = new System.Drawing.Point(245, 456);
            this.btnAddStcok.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAddStcok.Name = "btnAddStcok";
            this.btnAddStcok.Size = new System.Drawing.Size(305, 72);
            this.btnAddStcok.TabIndex = 5;
            this.btnAddStcok.Text = "添  加";
            this.btnAddStcok.UseVisualStyleBackColor = false;
            this.btnAddStcok.Click += new System.EventHandler(this.btnAddStcok_Click);
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
            this.label1.Size = new System.Drawing.Size(69, 25);
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
            this.tbCode.Size = new System.Drawing.Size(147, 31);
            this.tbCode.TabIndex = 1;
            this.tbCode.TextChanged += new System.EventHandler(this.tbCode_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(475, 221);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 25);
            this.label2.TabIndex = 9;
            this.label2.Text = "成交额：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(91, 221);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 25);
            this.label3.TabIndex = 10;
            this.label3.Text = "仓位：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(475, 107);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 25);
            this.label4.TabIndex = 11;
            this.label4.Text = "名称：";
            // 
            // tbMoney
            // 
            this.tbMoney.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbMoney.Location = new System.Drawing.Point(565, 215);
            this.tbMoney.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbMoney.MaxLength = 5;
            this.tbMoney.Name = "tbMoney";
            this.tbMoney.Size = new System.Drawing.Size(147, 31);
            this.tbMoney.TabIndex = 4;
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.Location = new System.Drawing.Point(565, 104);
            this.tbName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbName.MaxLength = 4;
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(147, 31);
            this.tbName.TabIndex = 11;
            // 
            // tbPosition
            // 
            this.tbPosition.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPosition.Location = new System.Drawing.Point(172, 215);
            this.tbPosition.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbPosition.MaxLength = 4;
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(147, 31);
            this.tbPosition.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.Color.Transparent;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label5.Location = new System.Drawing.Point(91, 342);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 25);
            this.label5.TabIndex = 12;
            this.label5.Text = "分类：";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.rbtnTomorrow);
            this.groupBox1.Controls.Add(this.rbtnLongTerm);
            this.groupBox1.Controls.Add(this.rbtnDragonLeader);
            this.groupBox1.Location = new System.Drawing.Point(172, 306);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(541, 86);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // rbtnTomorrow
            // 
            this.rbtnTomorrow.AutoSize = true;
            this.rbtnTomorrow.Checked = true;
            this.rbtnTomorrow.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnTomorrow.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnTomorrow.Location = new System.Drawing.Point(24, 32);
            this.rbtnTomorrow.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnTomorrow.Name = "rbtnTomorrow";
            this.rbtnTomorrow.Size = new System.Drawing.Size(68, 29);
            this.rbtnTomorrow.TabIndex = 5;
            this.rbtnTomorrow.TabStop = true;
            this.rbtnTomorrow.Text = "最新";
            this.rbtnTomorrow.UseVisualStyleBackColor = true;
            this.rbtnTomorrow.CheckedChanged += new System.EventHandler(this.rbtnTomorrow_CheckedChanged);
            // 
            // rbtnLongTerm
            // 
            this.rbtnLongTerm.AutoSize = true;
            this.rbtnLongTerm.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnLongTerm.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnLongTerm.Location = new System.Drawing.Point(136, 34);
            this.rbtnLongTerm.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnLongTerm.Name = "rbtnLongTerm";
            this.rbtnLongTerm.Size = new System.Drawing.Size(68, 29);
            this.rbtnLongTerm.TabIndex = 1;
            this.rbtnLongTerm.Text = "常驻";
            this.rbtnLongTerm.UseVisualStyleBackColor = true;
            this.rbtnLongTerm.CheckedChanged += new System.EventHandler(this.rbtnLongTerm_CheckedChanged);
            // 
            // rbtnDragonLeader
            // 
            this.rbtnDragonLeader.AutoSize = true;
            this.rbtnDragonLeader.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnDragonLeader.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnDragonLeader.Location = new System.Drawing.Point(264, 36);
            this.rbtnDragonLeader.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.rbtnDragonLeader.Name = "rbtnDragonLeader";
            this.rbtnDragonLeader.Size = new System.Drawing.Size(68, 29);
            this.rbtnDragonLeader.TabIndex = 0;
            this.rbtnDragonLeader.Text = "龙头";
            this.rbtnDragonLeader.UseVisualStyleBackColor = true;
            this.rbtnDragonLeader.CheckedChanged += new System.EventHandler(this.rbtnDragonLeader_CheckedChanged);
            // 
            // AddStockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(793, 613);
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
            this.Text = "AddStockForm";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
    }
}