namespace NineSunScripture.util.test
{
    partial class TestForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestForm));
            this.btnTestBuy = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lblTenthGearPrice = new System.Windows.Forms.Label();
            this.btnTenthGearPrice = new System.Windows.Forms.Button();
            this.btnTestRRB = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestBuy
            // 
            this.btnTestBuy.Location = new System.Drawing.Point(40, 40);
            this.btnTestBuy.Name = "btnTestBuy";
            this.btnTestBuy.Size = new System.Drawing.Size(155, 46);
            this.btnTestBuy.TabIndex = 0;
            this.btnTestBuy.Text = "测试策略买入";
            this.btnTestBuy.UseVisualStyleBackColor = true;
            this.btnTestBuy.Click += new System.EventHandler(this.btnTestBuy_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(40, 135);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 46);
            this.button1.TabIndex = 1;
            this.button1.Text = "测试策略卖出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(401, 40);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(155, 46);
            this.button2.TabIndex = 2;
            this.button2.Text = "查询当日成交";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblTenthGearPrice
            // 
            this.lblTenthGearPrice.Location = new System.Drawing.Point(397, 251);
            this.lblTenthGearPrice.Name = "lblTenthGearPrice";
            this.lblTenthGearPrice.Size = new System.Drawing.Size(200, 300);
            this.lblTenthGearPrice.TabIndex = 3;
            this.lblTenthGearPrice.Text = "label1";
            // 
            // btnTenthGearPrice
            // 
            this.btnTenthGearPrice.Location = new System.Drawing.Point(401, 150);
            this.btnTenthGearPrice.Name = "btnTenthGearPrice";
            this.btnTenthGearPrice.Size = new System.Drawing.Size(155, 46);
            this.btnTenthGearPrice.TabIndex = 4;
            this.btnTenthGearPrice.Text = "测试十档行情";
            this.btnTenthGearPrice.UseVisualStyleBackColor = true;
            this.btnTenthGearPrice.Click += new System.EventHandler(this.btnTenthGearPrice_Click);
            // 
            // btnTestRRB
            // 
            this.btnTestRRB.Location = new System.Drawing.Point(40, 224);
            this.btnTestRRB.Name = "btnTestRRB";
            this.btnTestRRB.Size = new System.Drawing.Size(155, 46);
            this.btnTestRRB.TabIndex = 5;
            this.btnTestRRB.Text = "测试逆回购";
            this.btnTestRRB.UseVisualStyleBackColor = true;
            this.btnTestRRB.Click += new System.EventHandler(this.btnTestRRB_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(40, 319);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(155, 46);
            this.button3.TabIndex = 6;
            this.button3.Text = "测试重启";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(40, 414);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(155, 46);
            this.button4.TabIndex = 7;
            this.button4.Text = "测试Json";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(40, 495);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(155, 46);
            this.button5.TabIndex = 8;
            this.button5.Text = "查询资金";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(778, 669);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnTestRRB);
            this.Controls.Add(this.btnTenthGearPrice);
            this.Controls.Add(this.lblTenthGearPrice);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTestBuy);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTestBuy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblTenthGearPrice;
        private System.Windows.Forms.Button btnTenthGearPrice;
        private System.Windows.Forms.Button btnTestRRB;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
    }
}