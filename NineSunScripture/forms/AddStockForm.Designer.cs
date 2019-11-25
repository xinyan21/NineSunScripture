namespace NineSunScripture.forms
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
            this.rbtnDragonLeader = new System.Windows.Forms.RadioButton();
            this.rbtnLongTerm = new System.Windows.Forms.RadioButton();
            this.rbtnTomorrow = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddStcok
            // 
            this.btnAddStcok.BackColor = System.Drawing.Color.Red;
            this.btnAddStcok.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAddStcok.ForeColor = System.Drawing.Color.Snow;
            this.btnAddStcok.Location = new System.Drawing.Point(215, 302);
            this.btnAddStcok.Name = "btnAddStcok";
            this.btnAddStcok.Size = new System.Drawing.Size(259, 59);
            this.btnAddStcok.TabIndex = 5;
            this.btnAddStcok.Text = "添  加";
            this.btnAddStcok.UseVisualStyleBackColor = false;
            this.btnAddStcok.Click += new System.EventHandler(this.btnAddStcok_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(79, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "代码：";
            // 
            // tbCode
            // 
            this.tbCode.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCode.Location = new System.Drawing.Point(150, 69);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(129, 27);
            this.tbCode.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(416, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 21);
            this.label2.TabIndex = 9;
            this.label2.Text = "成交额：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(416, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 21);
            this.label3.TabIndex = 10;
            this.label3.Text = "仓位：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(79, 153);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 21);
            this.label4.TabIndex = 11;
            this.label4.Text = "名称：";
            // 
            // tbMoney
            // 
            this.tbMoney.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbMoney.Location = new System.Drawing.Point(495, 153);
            this.tbMoney.Name = "tbMoney";
            this.tbMoney.Size = new System.Drawing.Size(129, 27);
            this.tbMoney.TabIndex = 4;
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.Location = new System.Drawing.Point(150, 147);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(129, 27);
            this.tbName.TabIndex = 3;
            // 
            // tbPosition
            // 
            this.tbPosition.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPosition.Location = new System.Drawing.Point(495, 70);
            this.tbPosition.Name = "tbPosition";
            this.tbPosition.Size = new System.Drawing.Size(129, 27);
            this.tbPosition.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(79, 234);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 21);
            this.label5.TabIndex = 12;
            this.label5.Text = "分类：";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.rbtnTomorrow);
            this.groupBox1.Controls.Add(this.rbtnLongTerm);
            this.groupBox1.Controls.Add(this.rbtnDragonLeader);
            this.groupBox1.Location = new System.Drawing.Point(150, 209);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(474, 59);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // rbtnDragonLeader
            // 
            this.rbtnDragonLeader.AutoSize = true;
            this.rbtnDragonLeader.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnDragonLeader.Location = new System.Drawing.Point(6, 21);
            this.rbtnDragonLeader.Name = "rbtnDragonLeader";
            this.rbtnDragonLeader.Size = new System.Drawing.Size(60, 25);
            this.rbtnDragonLeader.TabIndex = 0;
            this.rbtnDragonLeader.TabStop = true;
            this.rbtnDragonLeader.Text = "龙头";
            this.rbtnDragonLeader.UseVisualStyleBackColor = true;
            this.rbtnDragonLeader.CheckedChanged += new System.EventHandler(this.rbtnDragonLeader_CheckedChanged);
            // 
            // rbtnLongTerm
            // 
            this.rbtnLongTerm.AutoSize = true;
            this.rbtnLongTerm.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnLongTerm.Location = new System.Drawing.Point(91, 21);
            this.rbtnLongTerm.Name = "rbtnLongTerm";
            this.rbtnLongTerm.Size = new System.Drawing.Size(60, 25);
            this.rbtnLongTerm.TabIndex = 1;
            this.rbtnLongTerm.TabStop = true;
            this.rbtnLongTerm.Text = "常驻";
            this.rbtnLongTerm.UseVisualStyleBackColor = true;
            this.rbtnLongTerm.CheckedChanged += new System.EventHandler(this.rbtnLongTerm_CheckedChanged);
            // 
            // rbtnTomorrow
            // 
            this.rbtnTomorrow.AutoSize = true;
            this.rbtnTomorrow.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnTomorrow.Location = new System.Drawing.Point(183, 21);
            this.rbtnTomorrow.Name = "rbtnTomorrow";
            this.rbtnTomorrow.Size = new System.Drawing.Size(60, 25);
            this.rbtnTomorrow.TabIndex = 2;
            this.rbtnTomorrow.TabStop = true;
            this.rbtnTomorrow.Text = "明日";
            this.rbtnTomorrow.UseVisualStyleBackColor = true;
            this.rbtnTomorrow.CheckedChanged += new System.EventHandler(this.rbtnTomorrow_CheckedChanged);
            // 
            // AddStockForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(718, 427);
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