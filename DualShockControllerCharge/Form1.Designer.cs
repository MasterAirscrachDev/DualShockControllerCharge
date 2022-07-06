namespace DualShockControllerCharge
{
    partial class DSChargeView
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DSChargeView));
            this.TextBox = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.ChargeIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.ChargeIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // TextBox
            // 
            this.TextBox.AutoSize = true;
            this.TextBox.Font = new System.Drawing.Font("Impact", 19.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.TextBox.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.TextBox.Location = new System.Drawing.Point(12, 9);
            this.TextBox.Name = "TextBox";
            this.TextBox.Size = new System.Drawing.Size(168, 41);
            this.TextBox.TabIndex = 0;
            this.TextBox.Text = "***************";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ChargeIcon
            // 
            this.ChargeIcon.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ChargeIcon.BackgroundImage")));
            this.ChargeIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ChargeIcon.Location = new System.Drawing.Point(421, 12);
            this.ChargeIcon.Name = "ChargeIcon";
            this.ChargeIcon.Size = new System.Drawing.Size(44, 50);
            this.ChargeIcon.TabIndex = 1;
            this.ChargeIcon.TabStop = false;
            // 
            // DSChargeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(477, 73);
            this.Controls.Add(this.ChargeIcon);
            this.Controls.Add(this.TextBox);
            this.Name = "DSChargeView";
            this.Text = "DSChargeView";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Enter += new System.EventHandler(this.DSChargeView_Enter);
            this.Leave += new System.EventHandler(this.DSChargeView_Leave);
            ((System.ComponentModel.ISupportInitialize)(this.ChargeIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label TextBox;
        private System.Windows.Forms.Timer timer1;
        private PictureBox ChargeIcon;
    }
}