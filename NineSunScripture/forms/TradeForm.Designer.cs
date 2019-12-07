namespace NineSunScripture.forms
{
    partial class TradeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TradeForm));
            this.tbCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbPrice = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnOneFourth = new System.Windows.Forms.RadioButton();
            this.rbtnAllIn = new System.Windows.Forms.RadioButton();
            this.rbtnOneSecond = new System.Windows.Forms.RadioButton();
            this.rbtnOneThird = new System.Windows.Forms.RadioButton();
            this.btnBuy = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbCode
            // 
            this.tbCode.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbCode.Location = new System.Drawing.Point(188, 90);
            this.tbCode.Name = "tbCode";
            this.tbCode.Size = new System.Drawing.Size(167, 31);
            this.tbCode.TabIndex = 0;
            this.tbCode.TextChanged += new System.EventHandler(this.tbCode_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label1.Location = new System.Drawing.Point(95, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "代码：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label3.Location = new System.Drawing.Point(95, 195);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "价格：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label4.Location = new System.Drawing.Point(425, 195);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "仓位：";
            // 
            // tbPrice
            // 
            this.tbPrice.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbPrice.Location = new System.Drawing.Point(188, 189);
            this.tbPrice.Name = "tbPrice";
            this.tbPrice.Size = new System.Drawing.Size(167, 31);
            this.tbPrice.TabIndex = 6;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.rbtnOneFourth);
            this.groupBox1.Controls.Add(this.rbtnAllIn);
            this.groupBox1.Controls.Add(this.rbtnOneSecond);
            this.groupBox1.Controls.Add(this.rbtnOneThird);
            this.groupBox1.Location = new System.Drawing.Point(500, 177);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(295, 61);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // rbtnOneFourth
            // 
            this.rbtnOneFourth.AutoSize = true;
            this.rbtnOneFourth.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnOneFourth.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnOneFourth.Location = new System.Drawing.Point(6, 21);
            this.rbtnOneFourth.Name = "rbtnOneFourth";
            this.rbtnOneFourth.Size = new System.Drawing.Size(60, 29);
            this.rbtnOneFourth.TabIndex = 4;
            this.rbtnOneFourth.Text = "1/4";
            this.rbtnOneFourth.UseVisualStyleBackColor = true;
            this.rbtnOneFourth.CheckedChanged += new System.EventHandler(this.rbtnOneFourth_CheckedChanged);
            // 
            // rbtnAllIn
            // 
            this.rbtnAllIn.AutoSize = true;
            this.rbtnAllIn.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnAllIn.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnAllIn.Location = new System.Drawing.Point(214, 21);
            this.rbtnAllIn.Name = "rbtnAllIn";
            this.rbtnAllIn.Size = new System.Drawing.Size(68, 29);
            this.rbtnAllIn.TabIndex = 2;
            this.rbtnAllIn.Text = "满仓";
            this.rbtnAllIn.UseVisualStyleBackColor = true;
            this.rbtnAllIn.CheckedChanged += new System.EventHandler(this.rbtnAllIn_CheckedChanged);
            // 
            // rbtnOneSecond
            // 
            this.rbtnOneSecond.AutoSize = true;
            this.rbtnOneSecond.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnOneSecond.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnOneSecond.Location = new System.Drawing.Point(148, 21);
            this.rbtnOneSecond.Name = "rbtnOneSecond";
            this.rbtnOneSecond.Size = new System.Drawing.Size(60, 29);
            this.rbtnOneSecond.TabIndex = 1;
            this.rbtnOneSecond.Text = "1/2";
            this.rbtnOneSecond.UseVisualStyleBackColor = true;
            this.rbtnOneSecond.CheckedChanged += new System.EventHandler(this.rbtnOneSecond_CheckedChanged);
            // 
            // rbtnOneThird
            // 
            this.rbtnOneThird.AutoSize = true;
            this.rbtnOneThird.Checked = true;
            this.rbtnOneThird.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rbtnOneThird.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.rbtnOneThird.Location = new System.Drawing.Point(82, 21);
            this.rbtnOneThird.Name = "rbtnOneThird";
            this.rbtnOneThird.Size = new System.Drawing.Size(60, 29);
            this.rbtnOneThird.TabIndex = 0;
            this.rbtnOneThird.Text = "1/3";
            this.rbtnOneThird.UseVisualStyleBackColor = true;
            this.rbtnOneThird.CheckedChanged += new System.EventHandler(this.rbtnOneThird_CheckedChanged);
            // 
            // btnBuy
            // 
            this.btnBuy.BackColor = System.Drawing.Color.Transparent;
            this.btnBuy.BackgroundImage = global::NineSunScripture.Properties.Resources.btn_red2;
            this.btnBuy.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBuy.FlatAppearance.BorderSize = 0;
            this.btnBuy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuy.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBuy.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnBuy.Location = new System.Drawing.Point(263, 392);
            this.btnBuy.Name = "btnBuy";
            this.btnBuy.Size = new System.Drawing.Size(294, 72);
            this.btnBuy.TabIndex = 8;
            this.btnBuy.Text = "买  入";
            this.btnBuy.UseVisualStyleBackColor = false;
            this.btnBuy.Click += new System.EventHandler(this.btnBuy_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label2.Location = new System.Drawing.Point(425, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 25);
            this.label2.TabIndex = 9;
            this.label2.Text = "名称：";
            // 
            // tbName
            // 
            this.tbName.Font = new System.Drawing.Font("微软雅黑", 12.22642F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.Location = new System.Drawing.Point(506, 90);
            this.tbName.Name = "tbName";
            this.tbName.ReadOnly = true;
            this.tbName.Size = new System.Drawing.Size(167, 31);
            this.tbName.TabIndex = 10;
            // 
            // TradeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(831, 569);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnBuy);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tbPrice);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbCode);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TradeForm";
            this.Text = "买入";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbPrice;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnAllIn;
        private System.Windows.Forms.RadioButton rbtnOneSecond;
        private System.Windows.Forms.RadioButton rbtnOneThird;
        private System.Windows.Forms.Button btnBuy;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.RadioButton rbtnOneFourth;
    }
}