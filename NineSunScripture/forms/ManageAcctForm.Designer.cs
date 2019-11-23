namespace NineSunScripture.forms
{
    partial class ManageAcctForm
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
            this.lvAccounts = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tbCommPwd = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbBrokerName = new System.Windows.Forms.TextBox();
            this.lblSecurityName = new System.Windows.Forms.Label();
            this.btnDel = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbAccout = new System.Windows.Forms.TextBox();
            this.tbTHSVersion = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbIP = new System.Windows.Forms.TextBox();
            this.tbBrokerId = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvAccounts
            // 
            this.lvAccounts.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lvAccounts.Location = new System.Drawing.Point(46, 64);
            this.lvAccounts.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lvAccounts.Name = "lvAccounts";
            this.lvAccounts.Size = new System.Drawing.Size(510, 886);
            this.lvAccounts.TabIndex = 0;
            this.lvAccounts.UseCompatibleStateImageBehavior = false;
            this.lvAccounts.SelectedIndexChanged += new System.EventHandler(this.lvAccounts_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnReset);
            this.panel1.Controls.Add(this.tbCommPwd);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.tbBrokerName);
            this.panel1.Controls.Add(this.lblSecurityName);
            this.panel1.Controls.Add(this.btnDel);
            this.panel1.Controls.Add(this.btnEdit);
            this.panel1.Controls.Add(this.tbPassword);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.tbAccout);
            this.panel1.Controls.Add(this.tbTHSVersion);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.tbPort);
            this.panel1.Controls.Add(this.tbIP);
            this.panel1.Controls.Add(this.tbBrokerId);
            this.panel1.Location = new System.Drawing.Point(609, 64);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(416, 886);
            this.panel1.TabIndex = 1;
            // 
            // tbCommPwd
            // 
            this.tbCommPwd.Location = new System.Drawing.Point(123, 451);
            this.tbCommPwd.Name = "tbCommPwd";
            this.tbCommPwd.Size = new System.Drawing.Size(191, 25);
            this.tbCommPwd.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(21, 451);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(74, 21);
            this.label9.TabIndex = 19;
            this.label9.Text = "通讯密码";
            // 
            // tbBrokerName
            // 
            this.tbBrokerName.Location = new System.Drawing.Point(123, 82);
            this.tbBrokerName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbBrokerName.Name = "tbBrokerName";
            this.tbBrokerName.Size = new System.Drawing.Size(191, 25);
            this.tbBrokerName.TabIndex = 1;
            // 
            // lblSecurityName
            // 
            this.lblSecurityName.AutoSize = true;
            this.lblSecurityName.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblSecurityName.Location = new System.Drawing.Point(21, 82);
            this.lblSecurityName.Name = "lblSecurityName";
            this.lblSecurityName.Size = new System.Drawing.Size(74, 21);
            this.lblSecurityName.TabIndex = 17;
            this.lblSecurityName.Text = "券商名称";
            this.lblSecurityName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnDel
            // 
            this.btnDel.Location = new System.Drawing.Point(256, 770);
            this.btnDel.Name = "btnDel";
            this.btnDel.Size = new System.Drawing.Size(111, 42);
            this.btnDel.TabIndex = 15;
            this.btnDel.Text = "删除";
            this.btnDel.UseVisualStyleBackColor = true;
            this.btnDel.Click += new System.EventHandler(this.btnDel_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(46, 770);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(114, 42);
            this.btnEdit.TabIndex = 14;
            this.btnEdit.Text = "修改";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(123, 387);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(191, 25);
            this.tbPassword.TabIndex = 6;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(21, 387);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 21);
            this.label8.TabIndex = 13;
            this.label8.Text = "密码";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(21, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 21);
            this.label1.TabIndex = 3;
            this.label1.Text = "券商Id";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(21, 140);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 21);
            this.label2.TabIndex = 4;
            this.label2.Text = "IP";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(21, 195);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 21);
            this.label3.TabIndex = 5;
            this.label3.Text = "端口";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(21, 259);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 21);
            this.label5.TabIndex = 6;
            this.label5.Text = "同花顺版本";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(21, 319);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(74, 21);
            this.label7.TabIndex = 8;
            this.label7.Text = "资金账号";
            // 
            // tbAccout
            // 
            this.tbAccout.Location = new System.Drawing.Point(123, 319);
            this.tbAccout.Name = "tbAccout";
            this.tbAccout.Size = new System.Drawing.Size(191, 25);
            this.tbAccout.TabIndex = 5;
            // 
            // tbTHSVersion
            // 
            this.tbTHSVersion.Location = new System.Drawing.Point(123, 257);
            this.tbTHSVersion.Name = "tbTHSVersion";
            this.tbTHSVersion.Size = new System.Drawing.Size(191, 25);
            this.tbTHSVersion.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(46, 670);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(114, 42);
            this.btnAdd.TabIndex = 10;
            this.btnAdd.Text = "增加";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(123, 195);
            this.tbPort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(191, 25);
            this.tbPort.TabIndex = 3;
            // 
            // tbIP
            // 
            this.tbIP.Location = new System.Drawing.Point(123, 140);
            this.tbIP.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbIP.Name = "tbIP";
            this.tbIP.Size = new System.Drawing.Size(191, 25);
            this.tbIP.TabIndex = 2;
            // 
            // tbBrokerId
            // 
            this.tbBrokerId.Location = new System.Drawing.Point(123, 23);
            this.tbBrokerId.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbBrokerId.Name = "tbBrokerId";
            this.tbBrokerId.Size = new System.Drawing.Size(191, 25);
            this.tbBrokerId.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(50, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 26);
            this.label4.TabIndex = 2;
            this.label4.Text = "账号列表";
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(256, 670);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(114, 42);
            this.btnReset.TabIndex = 20;
            this.btnReset.Text = "重置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // ManageAcctForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 1012);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lvAccounts);
            this.Font = new System.Drawing.Font("微软雅黑", 8.830189F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ManageAcctForm";
            this.Text = "账号管理";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView lvAccounts;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbIP;
        private System.Windows.Forms.TextBox tbBrokerId;
        private System.Windows.Forms.TextBox tbAccout;
        private System.Windows.Forms.TextBox tbTHSVersion;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnDel;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.TextBox tbBrokerName;
        private System.Windows.Forms.Label lblSecurityName;
        private System.Windows.Forms.TextBox tbCommPwd;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btnReset;
    }
}