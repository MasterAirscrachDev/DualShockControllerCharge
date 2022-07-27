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
            this.statusIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // statusIcon
            // 
            this.statusIcon.ContextMenuStrip = this.statusContext;
            this.statusIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("statusIcon.Icon")));
            this.statusIcon.Text = "DSChargeView";
            this.statusIcon.Visible = true;
            // 
            // statusContext
            // 
            this.statusContext.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusContext.Name = "statusContext";
            this.statusContext.Size = new System.Drawing.Size(61, 4);
            // 
            // DSChargeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ClientSize = new System.Drawing.Size(277, 48);
            this.Name = "DSChargeView";
            this.ShowInTaskbar = false;
            this.Text = "DSChargeView";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.OnLoad);
            this.Enter += new System.EventHandler(this.DSChargeView_Enter);
            this.Leave += new System.EventHandler(this.DSChargeView_Leave);
            this.ResumeLayout(false);

        }

        #endregion
        private NotifyIcon statusIcon;
        private ContextMenuStrip statusContext;
    }
}