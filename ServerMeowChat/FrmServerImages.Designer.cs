namespace MeowChatServer
{
    partial class FrmServerImages
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
            this.TabControlServerImages = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // TabControlServerImages
            // 
            this.TabControlServerImages.Location = new System.Drawing.Point(3, 3);
            this.TabControlServerImages.Name = "TabControlServerImages";
            this.TabControlServerImages.SelectedIndex = 0;
            this.TabControlServerImages.Size = new System.Drawing.Size(410, 399);
            this.TabControlServerImages.TabIndex = 1;
            // 
            // FrmServerImages
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(414, 404);
            this.Controls.Add(this.TabControlServerImages);
            this.MaximizeBox = false;
            this.Name = "FrmServerImages";
            this.Text = "Server Images";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmImage_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TabControlServerImages;
    }
}