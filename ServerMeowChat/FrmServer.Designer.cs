using System.Windows.Forms;

namespace MeowChatServer
{
    partial class FrmServer
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
        /// 
        private void InitializeComponent()
        {
            this.btnStartSrv = new System.Windows.Forms.Button();
            this.listViewClients = new System.Windows.Forms.ListView();
            this.colClientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colClientHash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colConctDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.rchTxtServerConn = new System.Windows.Forms.RichTextBox();
            this.lblLocalIp = new System.Windows.Forms.Label();
            this.txtBoxIpAddress = new System.Windows.Forms.TextBox();
            this.txtBoxPort = new System.Windows.Forms.TextBox();
            this.lblIp = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.btnServerSnd = new System.Windows.Forms.Button();
            this.txtBxServer = new System.Windows.Forms.TextBox();
            this.TabControlServer = new System.Windows.Forms.TabControl();
            this.tabConnTrack = new System.Windows.Forms.TabPage();
            this.tabPagePublicChatServer = new System.Windows.Forms.TabPage();
            this.rchTxtServerPub = new System.Windows.Forms.RichTextBox();
            this.TabControlServer.SuspendLayout();
            this.tabConnTrack.SuspendLayout();
            this.tabPagePublicChatServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartSrv
            // 
            this.btnStartSrv.Location = new System.Drawing.Point(175, 361);
            this.btnStartSrv.Name = "btnStartSrv";
            this.btnStartSrv.Size = new System.Drawing.Size(73, 23);
            this.btnStartSrv.TabIndex = 0;
            this.btnStartSrv.Text = "&Start Server";
            this.btnStartSrv.UseVisualStyleBackColor = true;
            this.btnStartSrv.Click += new System.EventHandler(this.btnStartSrv_Click);
            // 
            // listViewClients
            // 
            this.listViewClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colClientName,
            this.colClientHash,
            this.colConctDate});
            this.listViewClients.FullRowSelect = true;
            this.listViewClients.GridLines = true;
            this.listViewClients.Location = new System.Drawing.Point(13, 12);
            this.listViewClients.Name = "listViewClients";
            this.listViewClients.Size = new System.Drawing.Size(317, 341);
            this.listViewClients.TabIndex = 2;
            this.listViewClients.UseCompatibleStateImageBehavior = false;
            this.listViewClients.View = System.Windows.Forms.View.Details;
            // 
            // colClientName
            // 
            this.colClientName.Text = "Client Name";
            this.colClientName.Width = 117;
            // 
            // colClientHash
            // 
            this.colClientHash.Text = "Client IP";
            this.colClientHash.Width = 80;
            // 
            // colConctDate
            // 
            this.colConctDate.Text = "Connect Date";
            this.colConctDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.colConctDate.Width = 115;
            // 
            // rchTxtServerConn
            // 
            this.rchTxtServerConn.BackColor = System.Drawing.Color.White;
            this.rchTxtServerConn.ForeColor = System.Drawing.Color.Black;
            this.rchTxtServerConn.Location = new System.Drawing.Point(0, 3);
            this.rchTxtServerConn.Name = "rchTxtServerConn";
            this.rchTxtServerConn.ReadOnly = true;
            this.rchTxtServerConn.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rchTxtServerConn.Size = new System.Drawing.Size(406, 312);
            this.rchTxtServerConn.TabIndex = 9;
            this.rchTxtServerConn.Text = "";
            this.rchTxtServerConn.TextChanged += new System.EventHandler(this.richTxtChatBox_TextChanged);
            // 
            // lblLocalIp
            // 
            this.lblLocalIp.AutoSize = true;
            this.lblLocalIp.Location = new System.Drawing.Point(333, 366);
            this.lblLocalIp.Name = "lblLocalIp";
            this.lblLocalIp.Size = new System.Drawing.Size(52, 13);
            this.lblLocalIp.TabIndex = 10;
            this.lblLocalIp.Text = "lblLocalIp";
            // 
            // txtBoxIpAddress
            // 
            this.txtBoxIpAddress.Location = new System.Drawing.Point(30, 362);
            this.txtBoxIpAddress.Name = "txtBoxIpAddress";
            this.txtBoxIpAddress.Size = new System.Drawing.Size(60, 20);
            this.txtBoxIpAddress.TabIndex = 11;
            this.txtBoxIpAddress.Text = "127.0.0.1";
            // 
            // txtBoxPort
            // 
            this.txtBoxPort.Location = new System.Drawing.Point(121, 362);
            this.txtBoxPort.Name = "txtBoxPort";
            this.txtBoxPort.Size = new System.Drawing.Size(36, 20);
            this.txtBoxPort.TabIndex = 12;
            this.txtBoxPort.Text = "8888";
            // 
            // lblIp
            // 
            this.lblIp.AutoSize = true;
            this.lblIp.Location = new System.Drawing.Point(10, 366);
            this.lblIp.Name = "lblIp";
            this.lblIp.Size = new System.Drawing.Size(20, 13);
            this.lblIp.TabIndex = 13;
            this.lblIp.Text = "IP:";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(92, 366);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 13);
            this.lblPort.TabIndex = 14;
            this.lblPort.Text = "Port:";
            // 
            // btnServerSnd
            // 
            this.btnServerSnd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnServerSnd.Location = new System.Drawing.Point(708, 360);
            this.btnServerSnd.Name = "btnServerSnd";
            this.btnServerSnd.Size = new System.Drawing.Size(43, 23);
            this.btnServerSnd.TabIndex = 15;
            this.btnServerSnd.Text = "Send";
            this.btnServerSnd.UseVisualStyleBackColor = true;
            this.btnServerSnd.Click += new System.EventHandler(this.btnServerSnd_Click);
            // 
            // txtBxServer
            // 
            this.txtBxServer.Location = new System.Drawing.Point(421, 362);
            this.txtBxServer.Name = "txtBxServer";
            this.txtBxServer.Size = new System.Drawing.Size(281, 20);
            this.txtBxServer.TabIndex = 16;
            this.txtBxServer.Text = "Global message";
            // 
            // TabControlServer
            // 
            this.TabControlServer.Controls.Add(this.tabConnTrack);
            this.TabControlServer.Controls.Add(this.tabPagePublicChatServer);
            this.TabControlServer.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabControlServer.ItemSize = new System.Drawing.Size(110, 24);
            this.TabControlServer.Location = new System.Drawing.Point(336, 12);
            this.TabControlServer.Name = "TabControlServer";
            this.TabControlServer.SelectedIndex = 0;
            this.TabControlServer.Size = new System.Drawing.Size(415, 341);
            this.TabControlServer.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControlServer.TabIndex = 17;
            this.TabControlServer.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControlServer_DrawItem);
            this.TabControlServer.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TabControlServert_MouseClick);
            // 
            // tabConnTrack
            // 
            this.tabConnTrack.Controls.Add(this.rchTxtServerConn);
            this.tabConnTrack.Location = new System.Drawing.Point(4, 28);
            this.tabConnTrack.Name = "tabConnTrack";
            this.tabConnTrack.Padding = new System.Windows.Forms.Padding(3);
            this.tabConnTrack.Size = new System.Drawing.Size(407, 309);
            this.tabConnTrack.TabIndex = 0;
            this.tabConnTrack.Text = "Connection Track";
            this.tabConnTrack.UseVisualStyleBackColor = true;
            // 
            // tabPagePublicChatServer
            // 
            this.tabPagePublicChatServer.Controls.Add(this.rchTxtServerPub);
            this.tabPagePublicChatServer.Location = new System.Drawing.Point(4, 28);
            this.tabPagePublicChatServer.Name = "tabPagePublicChatServer";
            this.tabPagePublicChatServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePublicChatServer.Size = new System.Drawing.Size(407, 309);
            this.tabPagePublicChatServer.TabIndex = 1;
            this.tabPagePublicChatServer.Text = "Pubic chat";
            this.tabPagePublicChatServer.UseVisualStyleBackColor = true;
            // 
            // rchTxtServerPub
            // 
            this.rchTxtServerPub.BackColor = System.Drawing.Color.White;
            this.rchTxtServerPub.ForeColor = System.Drawing.Color.Black;
            this.rchTxtServerPub.Location = new System.Drawing.Point(0, 3);
            this.rchTxtServerPub.Name = "rchTxtServerPub";
            this.rchTxtServerPub.ReadOnly = true;
            this.rchTxtServerPub.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rchTxtServerPub.Size = new System.Drawing.Size(406, 312);
            this.rchTxtServerPub.TabIndex = 10;
            this.rchTxtServerPub.Text = "";
            // 
            // FrmServer
            // 
            this.AcceptButton = this.btnStartSrv;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(754, 391);
            this.Controls.Add(this.TabControlServer);
            this.Controls.Add(this.txtBxServer);
            this.Controls.Add(this.btnServerSnd);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.lblIp);
            this.Controls.Add(this.txtBoxPort);
            this.Controls.Add(this.txtBoxIpAddress);
            this.Controls.Add(this.lblLocalIp);
            this.Controls.Add(this.listViewClients);
            this.Controls.Add(this.btnStartSrv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmServer";
            this.Text = "Server";
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.TabControlServer.ResumeLayout(false);
            this.tabConnTrack.ResumeLayout(false);
            this.tabPagePublicChatServer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartSrv;
        private System.Windows.Forms.ListView listViewClients;
        private System.Windows.Forms.ColumnHeader colClientName;
        private System.Windows.Forms.ColumnHeader colClientHash;
        private System.Windows.Forms.ColumnHeader colConctDate;
        private System.Windows.Forms.RichTextBox rchTxtServerConn;
        private System.Windows.Forms.Label lblLocalIp;
        private System.Windows.Forms.TextBox txtBoxIpAddress;
        private System.Windows.Forms.TextBox txtBoxPort;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.Button btnServerSnd;
        private System.Windows.Forms.TextBox txtBxServer;
        private System.Windows.Forms.TabControl TabControlServer;
        private System.Windows.Forms.TabPage tabConnTrack;
        private System.Windows.Forms.TabPage tabPagePublicChatServer;
        private RichTextBox rchTxtServerPub;
    }
}

