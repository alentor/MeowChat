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
            this.lblName = new System.Windows.Forms.Label();
            this.lblServerIp = new System.Windows.Forms.Label();
            this.txtBoxName = new System.Windows.Forms.TextBox();
            this.txtBoxServerIp = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnColorPick = new System.Windows.Forms.Button();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.lvlPort = new System.Windows.Forms.Label();
            this.txtBoxUserName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBoxRegUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRegister = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(48, 49);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(38, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "&Name:";
            // 
            // lblServerIp
            // 
            this.lblServerIp.AutoSize = true;
            this.lblServerIp.Location = new System.Drawing.Point(41, 23);
            this.lblServerIp.Name = "lblServerIp";
            this.lblServerIp.Size = new System.Drawing.Size(54, 13);
            this.lblServerIp.TabIndex = 1;
            this.lblServerIp.Text = "&Server IP:";
            // 
            // txtBoxName
            // 
            this.txtBoxName.Location = new System.Drawing.Point(86, 45);
            this.txtBoxName.Name = "txtBoxName";
            this.txtBoxName.Size = new System.Drawing.Size(99, 20);
            this.txtBoxName.TabIndex = 0;
            this.txtBoxName.Text = "alen";
            this.txtBoxName.TextChanged += new System.EventHandler(this.txtBoxName_TextChanged);
            // 
            // txtBoxServerIp
            // 
            this.txtBoxServerIp.Location = new System.Drawing.Point(95, 19);
            this.txtBoxServerIp.Name = "txtBoxServerIp";
            this.txtBoxServerIp.Size = new System.Drawing.Size(99, 20);
            this.txtBoxServerIp.TabIndex = 1;
            this.txtBoxServerIp.Text = "127.0.0.1";
            this.txtBoxServerIp.TextChanged += new System.EventHandler(this.txtBxServerIp_TextChanged);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(126, 71);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(68, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(37, 71);
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
            this.btnColorPick.Location = new System.Drawing.Point(102, 71);
            this.btnColorPick.Name = "btnColorPick";
            this.btnColorPick.Size = new System.Drawing.Size(23, 23);
            this.btnColorPick.TabIndex = 8;
            this.btnColorPick.UseVisualStyleBackColor = true;
            this.btnColorPick.Click += new System.EventHandler(this.btnColorPick_Click);
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(95, 45);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(30, 20);
            this.txtBoxPort.TabIndex = 9;
            this.txtBoxPort.Text = "8888";
            this.txtBoxPort.TextChanged += new System.EventHandler(this.txtBoxPort_TextChanged);
            // 
            // lvlPort
            // 
            this.lvlPort.AutoSize = true;
            this.lvlPort.Location = new System.Drawing.Point(66, 49);
            this.lvlPort.Name = "lvlPort";
            this.lvlPort.Size = new System.Drawing.Size(29, 13);
            this.lvlPort.TabIndex = 10;
            this.lvlPort.Text = "&Port:";
            // 
            // txtBoxUserName
            // 
            this.txtBoxUserName.Location = new System.Drawing.Point(86, 19);
            this.txtBoxUserName.Name = "txtBoxUserName";
            this.txtBoxUserName.Size = new System.Drawing.Size(99, 20);
            this.txtBoxUserName.TabIndex = 11;
            this.txtBoxUserName.Text = "alen";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "&User Name:";
            // 
            // txtBoxRegUserName
            // 
            this.txtBoxRegUserName.Location = new System.Drawing.Point(94, 18);
            this.txtBoxRegUserName.Name = "txtBoxRegUserName";
            this.txtBoxRegUserName.Size = new System.Drawing.Size(99, 20);
            this.txtBoxRegUserName.TabIndex = 15;
            this.txtBoxRegUserName.Text = "alen";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "&User Name:";
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(125, 44);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(68, 23);
            this.btnRegister.TabIndex = 19;
            this.btnRegister.Text = "&Register";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtBoxUserName);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.txtBoxName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnConnect);
            this.groupBox1.Controls.Add(this.btnCancel);
            this.groupBox1.Controls.Add(this.btnColorPick);
            this.groupBox1.Location = new System.Drawing.Point(6, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 100);
            this.groupBox1.TabIndex = 20;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Login";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtBoxRegUserName);
            this.groupBox2.Controls.Add(this.btnRegister);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(6, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 73);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Register";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtBoxServerIp);
            this.groupBox3.Controls.Add(this.lblServerIp);
            this.groupBox3.Controls.Add(this.lvlPort);
            this.groupBox3.Controls.Add(this.txtBoxPort);
            this.groupBox3.Location = new System.Drawing.Point(6, 189);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 73);
            this.groupBox3.TabIndex = 22;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Server Information";
            // 
            // FrmLogin
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(211, 267);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblServerIp;
        private System.Windows.Forms.TextBox txtBoxName;
        private System.Windows.Forms.TextBox txtBoxServerIp;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnCancel;
        private Button btnColorPick;
        private ColorDialog colorPicker;
        private TextBox txtBoxPort;
        private Label lvlPort;
        private TextBox txtBoxUserName;
        private Label label1;
        private TextBox txtBoxRegUserName;
        private Label label2;
        private Button btnRegister;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
    }
}