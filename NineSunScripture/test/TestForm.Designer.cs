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
            this.btnTestBuy = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnTestBuy
            // 
            this.btnTestBuy.Location = new System.Drawing.Point(53, 176);
            this.btnTestBuy.Name = "btnTestBuy";
            this.btnTestBuy.Size = new System.Drawing.Size(155, 46);
            this.btnTestBuy.TabIndex = 0;
            this.btnTestBuy.Text = "测试策略买入";
            this.btnTestBuy.UseVisualStyleBackColor = true;
            this.btnTestBuy.Click += new System.EventHandler(this.btnTestBuy_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(312, 176);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 46);
            this.button1.TabIndex = 1;
            this.button1.Text = "测试策略卖出";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(53, 44);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(155, 46);
            this.button2.TabIndex = 2;
            this.button2.Text = "登录";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::NineSunScripture.Properties.Resources._3_1_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(778, 669);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnTestBuy);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("微软雅黑", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnTestBuy;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}