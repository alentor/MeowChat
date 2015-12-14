using System.Windows.Forms;

namespace MeowChatClient
{
    partial class FrmLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            this.lblServerIp = new System.Windows.Forms.Label();
            this.txtBoxServerIp = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnColorPick = new System.Windows.Forms.Button();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.lvlPort = new System.Windows.Forms.Label();
            this.BtnRegister = new System.Windows.Forms.Button();
            this.lblName = new System.Windows.Forms.Label();
            this.txtBoxUserName = new System.Windows.Forms.TextBox();
            this.txtBoxNewUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxNewName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblServerIp
            // 
            this.lblServerIp.AutoSize = true;
            this.lblServerIp.Location = new System.Drawing.Point(16, 42);
            this.lblServerIp.Name = "lblServerIp";
            this.lblServerIp.Size = new System.Drawing.Size(54, 13);
            this.lblServerIp.TabIndex = 1;
            this.lblServerIp.Text = "&Server IP:";
            // 
            // txtBoxServerIp
            // 
            this.txtBoxServerIp.Location = new System.Drawing.Point(71, 38);
            this.txtBoxServerIp.Name = "txtBoxServerIp";
            this.txtBoxServerIp.Size = new System.Drawing.Size(91, 20);
            this.txtBoxServerIp.TabIndex = 1;
            this.txtBoxServerIp.Text = "127.0.0.1";
            this.txtBoxServerIp.TextChanged += new System.EventHandler(this.txtBxServerIp_TextChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(95, 90);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(68, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Co&nnect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(6, 90);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnColorPick
            // 
            this.btnColorPick.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnColorPick.BackgroundImage")));
            this.btnColorPick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnColorPick.Location = new System.Drawing.Point(71, 90);
            this.btnColorPick.Name = "btnColorPick";
            this.btnColorPick.Size = new System.Drawing.Size(23, 23);
            this.btnColorPick.TabIndex = 8;
            this.btnColorPick.UseVisualStyleBackColor = true;
            this.btnColorPick.Click += new System.EventHandler(this.btnColorPick_Click);
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(71, 64);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(30, 20);
            this.txtBoxPort.TabIndex = 9;
            this.txtBoxPort.Text = "8888";
            this.txtBoxPort.TextChanged += new System.EventHandler(this.txtBoxPort_TextChanged);
            // 
            // lvlPort
            // 
            this.lvlPort.AutoSize = true;
            this.lvlPort.Location = new System.Drawing.Point(41, 68);
            this.lvlPort.Name = "lvlPort";
            this.lvlPort.Size = new System.Drawing.Size(29, 13);
            this.lvlPort.TabIndex = 10;
            this.lvlPort.Text = "Po&rt:";
            // 
            // BtnRegister
            // 
            this.BtnRegister.Location = new System.Drawing.Point(55, 171);
            this.BtnRegister.Name = "BtnRegister";
            this.BtnRegister.Size = new System.Drawing.Size(64, 23);
            this.BtnRegister.TabIndex = 11;
            this.BtnRegister.Text = "&Register";
            this.BtnRegister.UseVisualStyleBackColor = true;
            this.BtnRegister.Click += new System.EventHandler(this.BtnRegister_Click);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(9, 13);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(63, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "&UserName :";
            // 
            // txtBoxUserName
            // 
            this.txtBoxUserName.Location = new System.Drawing.Point(71, 9);
            this.txtBoxUserName.Name = "txtBoxUserName";
            this.txtBoxUserName.Size = new System.Drawing.Size(91, 20);
            this.txtBoxUserName.TabIndex = 0;
            this.txtBoxUserName.Text = "alen";
            this.txtBoxUserName.TextChanged += new System.EventHandler(this.txtBoxName_TextChanged);
            // 
            // txtBoxNewUserName
            // 
            this.txtBoxNewUserName.Location = new System.Drawing.Point(71, 119);
            this.txtBoxNewUserName.Name = "txtBoxNewUserName";
            this.txtBoxNewUserName.Size = new System.Drawing.Size(91, 20);
            this.txtBoxNewUserName.TabIndex = 12;
            this.txtBoxNewUserName.Text = "New UserName";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "&UserName :";
            // 
            // txtBoxNewName
            // 
            this.txtBoxNewName.Location = new System.Drawing.Point(71, 145);
            this.txtBoxNewName.Name = "txtBoxNewName";
            this.txtBoxNewName.Size = new System.Drawing.Size(91, 20);
            this.txtBoxNewName.TabIndex = 14;
            this.txtBoxNewName.Text = "First Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "&Name :";
            // 
            // FrmLogin
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(170, 200);
            this.Controls.Add(this.txtBoxNewName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtBoxNewUserName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BtnRegister);
            this.Controls.Add(this.txtBoxPort);
            this.Controls.Add(this.lvlPort);
            this.Controls.Add(this.btnColorPick);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtBoxServerIp);
            this.Controls.Add(this.txtBoxUserName);
            this.Controls.Add(this.lblServerIp);
            this.Controls.Add(this.lblName);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblServerIp;
        private System.Windows.Forms.TextBox txtBoxServerIp;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCancel;
        private Button btnColorPick;
        private ColorDialog colorPicker;
        private TextBox txtBoxPort;
        private Label lvlPort;
        private Button BtnRegister;
        private Label lblName;
        private TextBox txtBoxUserName;
        private TextBox txtBoxNewUserName;
        private Label label1;
        private TextBox txtBoxNewName;
        private Label label2;
    }
}