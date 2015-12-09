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
            this.BtnStartSrv = new System.Windows.Forms.Button();
            this.ListViewClients = new System.Windows.Forms.ListView();
            this.ColClientName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColClientHash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ColConctDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.RichTextServerConn = new System.Windows.Forms.RichTextBox();
            this.LblLocalIp = new System.Windows.Forms.Label();
            this.TxtBoxIpAddress = new System.Windows.Forms.TextBox();
            this.TxtBoxPort = new System.Windows.Forms.TextBox();
            this.LblIp = new System.Windows.Forms.Label();
            this.LblPort = new System.Windows.Forms.Label();
            this.BtnServerSnd = new System.Windows.Forms.Button();
            this.TxtBxServer = new System.Windows.Forms.TextBox();
            this.TabControlServer = new System.Windows.Forms.TabControl();
            this.TabConnTrack = new System.Windows.Forms.TabPage();
            this.TabPagePublicChatServer = new System.Windows.Forms.TabPage();
            this.RichTextServerPub = new System.Windows.Forms.RichTextBox();
            this.BtnStopSrv = new System.Windows.Forms.Button();
            this.TabControlServer.SuspendLayout();
            this.TabConnTrack.SuspendLayout();
            this.TabPagePublicChatServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // BtnStartSrv
            // 
            this.BtnStartSrv.Location = new System.Drawing.Point(175, 361);
            this.BtnStartSrv.Name = "BtnStartSrv";
            this.BtnStartSrv.Size = new System.Drawing.Size(73, 23);
            this.BtnStartSrv.TabIndex = 0;
            this.BtnStartSrv.Text = "&Start Server";
            this.BtnStartSrv.UseVisualStyleBackColor = true;
            this.BtnStartSrv.Click += new System.EventHandler(this.BtnStartSrv_Click);
            // 
            // ListViewClients
            // 
            this.ListViewClients.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColClientName,
            this.ColClientHash,
            this.ColConctDate});
            this.ListViewClients.FullRowSelect = true;
            this.ListViewClients.GridLines = true;
            this.ListViewClients.Location = new System.Drawing.Point(13, 12);
            this.ListViewClients.Name = "ListViewClients";
            this.ListViewClients.Size = new System.Drawing.Size(317, 341);
            this.ListViewClients.TabIndex = 2;
            this.ListViewClients.UseCompatibleStateImageBehavior = false;
            this.ListViewClients.View = System.Windows.Forms.View.Details;
            // 
            // ColClientName
            // 
            this.ColClientName.Text = "Client Name";
            this.ColClientName.Width = 117;
            // 
            // ColClientHash
            // 
            this.ColClientHash.Text = "Client IP";
            this.ColClientHash.Width = 80;
            // 
            // ColConctDate
            // 
            this.ColConctDate.Text = "Connect Date";
            this.ColConctDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ColConctDate.Width = 115;
            // 
            // RichTextServerConn
            // 
            this.RichTextServerConn.BackColor = System.Drawing.Color.White;
            this.RichTextServerConn.ForeColor = System.Drawing.Color.Black;
            this.RichTextServerConn.Location = new System.Drawing.Point(0, 3);
            this.RichTextServerConn.Name = "RichTextServerConn";
            this.RichTextServerConn.ReadOnly = true;
            this.RichTextServerConn.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.RichTextServerConn.Size = new System.Drawing.Size(406, 312);
            this.RichTextServerConn.TabIndex = 9;
            this.RichTextServerConn.Text = "";
            this.RichTextServerConn.TextChanged += new System.EventHandler(this.RichTextChatBox_TextChanged);
            // 
            // LblLocalIp
            // 
            this.LblLocalIp.AutoSize = true;
            this.LblLocalIp.Location = new System.Drawing.Point(333, 366);
            this.LblLocalIp.Name = "LblLocalIp";
            this.LblLocalIp.Size = new System.Drawing.Size(56, 13);
            this.LblLocalIp.TabIndex = 10;
            this.LblLocalIp.Text = "LblLocalIp";
            // 
            // TxtBoxIpAddress
            // 
            this.TxtBoxIpAddress.Location = new System.Drawing.Point(30, 362);
            this.TxtBoxIpAddress.Name = "TxtBoxIpAddress";
            this.TxtBoxIpAddress.Size = new System.Drawing.Size(60, 20);
            this.TxtBoxIpAddress.TabIndex = 11;
            this.TxtBoxIpAddress.Text = "127.0.0.1";
            // 
            // TxtBoxPort
            // 
            this.TxtBoxPort.Location = new System.Drawing.Point(121, 362);
            this.TxtBoxPort.Name = "TxtBoxPort";
            this.TxtBoxPort.Size = new System.Drawing.Size(36, 20);
            this.TxtBoxPort.TabIndex = 12;
            this.TxtBoxPort.Text = "8888";
            // 
            // LblIp
            // 
            this.LblIp.AutoSize = true;
            this.LblIp.Location = new System.Drawing.Point(10, 366);
            this.LblIp.Name = "LblIp";
            this.LblIp.Size = new System.Drawing.Size(20, 13);
            this.LblIp.TabIndex = 13;
            this.LblIp.Text = "IP:";
            // 
            // LblPort
            // 
            this.LblPort.AutoSize = true;
            this.LblPort.Location = new System.Drawing.Point(92, 366);
            this.LblPort.Name = "LblPort";
            this.LblPort.Size = new System.Drawing.Size(29, 13);
            this.LblPort.TabIndex = 14;
            this.LblPort.Text = "Port:";
            // 
            // BtnServerSnd
            // 
            this.BtnServerSnd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnServerSnd.Location = new System.Drawing.Point(708, 360);
            this.BtnServerSnd.Name = "BtnServerSnd";
            this.BtnServerSnd.Size = new System.Drawing.Size(43, 23);
            this.BtnServerSnd.TabIndex = 15;
            this.BtnServerSnd.Text = "Send";
            this.BtnServerSnd.UseVisualStyleBackColor = true;
            this.BtnServerSnd.Click += new System.EventHandler(this.BtnServerSnd_Click);
            // 
            // TxtBxServer
            // 
            this.TxtBxServer.Location = new System.Drawing.Point(421, 362);
            this.TxtBxServer.Name = "TxtBxServer";
            this.TxtBxServer.Size = new System.Drawing.Size(281, 20);
            this.TxtBxServer.TabIndex = 16;
            this.TxtBxServer.Text = "Global message";
            // 
            // TabControlServer
            // 
            this.TabControlServer.Controls.Add(this.TabConnTrack);
            this.TabControlServer.Controls.Add(this.TabPagePublicChatServer);
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
            // TabConnTrack
            // 
            this.TabConnTrack.Controls.Add(this.RichTextServerConn);
            this.TabConnTrack.Location = new System.Drawing.Point(4, 28);
            this.TabConnTrack.Name = "TabConnTrack";
            this.TabConnTrack.Padding = new System.Windows.Forms.Padding(3);
            this.TabConnTrack.Size = new System.Drawing.Size(407, 309);
            this.TabConnTrack.TabIndex = 0;
            this.TabConnTrack.Text = "Connection Track";
            this.TabConnTrack.UseVisualStyleBackColor = true;
            // 
            // TabPagePublicChatServer
            // 
            this.TabPagePublicChatServer.Controls.Add(this.RichTextServerPub);
            this.TabPagePublicChatServer.Location = new System.Drawing.Point(4, 28);
            this.TabPagePublicChatServer.Name = "TabPagePublicChatServer";
            this.TabPagePublicChatServer.Padding = new System.Windows.Forms.Padding(3);
            this.TabPagePublicChatServer.Size = new System.Drawing.Size(407, 309);
            this.TabPagePublicChatServer.TabIndex = 1;
            this.TabPagePublicChatServer.Text = "Pubic chat";
            this.TabPagePublicChatServer.UseVisualStyleBackColor = true;
            // 
            // RichTextServerPub
            // 
            this.RichTextServerPub.BackColor = System.Drawing.Color.White;
            this.RichTextServerPub.ForeColor = System.Drawing.Color.Black;
            this.RichTextServerPub.Location = new System.Drawing.Point(0, 3);
            this.RichTextServerPub.Name = "RichTextServerPub";
            this.RichTextServerPub.ReadOnly = true;
            this.RichTextServerPub.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.RichTextServerPub.Size = new System.Drawing.Size(406, 312);
            this.RichTextServerPub.TabIndex = 10;
            this.RichTextServerPub.Text = "";
            // 
            // BtnStopSrv
            // 
            this.BtnStopSrv.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BtnStopSrv.Enabled = false;
            this.BtnStopSrv.Location = new System.Drawing.Point(254, 362);
            this.BtnStopSrv.Name = "BtnStopSrv";
            this.BtnStopSrv.Size = new System.Drawing.Size(73, 23);
            this.BtnStopSrv.TabIndex = 18;
            this.BtnStopSrv.Text = "S&top";
            this.BtnStopSrv.UseVisualStyleBackColor = true;
            this.BtnStopSrv.Click += new System.EventHandler(this.btnStopSrv_Click);
            // 
            // FrmServer
            // 
            this.AcceptButton = this.BtnStartSrv;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BtnStopSrv;
            this.ClientSize = new System.Drawing.Size(754, 391);
            this.Controls.Add(this.BtnStopSrv);
            this.Controls.Add(this.TabControlServer);
            this.Controls.Add(this.TxtBxServer);
            this.Controls.Add(this.BtnServerSnd);
            this.Controls.Add(this.LblPort);
            this.Controls.Add(this.LblIp);
            this.Controls.Add(this.TxtBoxPort);
            this.Controls.Add(this.TxtBoxIpAddress);
            this.Controls.Add(this.LblLocalIp);
            this.Controls.Add(this.ListViewClients);
            this.Controls.Add(this.BtnStartSrv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmServer";
            this.Text = "Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmServer_FormClosing);
            this.Load += new System.EventHandler(this.ServerForm_Load);
            this.TabControlServer.ResumeLayout(false);
            this.TabConnTrack.ResumeLayout(false);
            this.TabPagePublicChatServer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BtnStartSrv;
        private System.Windows.Forms.ListView ListViewClients;
        private System.Windows.Forms.ColumnHeader ColClientName;
        private System.Windows.Forms.ColumnHeader ColClientHash;
        private System.Windows.Forms.ColumnHeader ColConctDate;
        private System.Windows.Forms.RichTextBox RichTextServerConn;
        private System.Windows.Forms.Label LblLocalIp;
        private System.Windows.Forms.TextBox TxtBoxIpAddress;
        private System.Windows.Forms.TextBox TxtBoxPort;
        private System.Windows.Forms.Label LblIp;
        private System.Windows.Forms.Label LblPort;
        private System.Windows.Forms.Button BtnServerSnd;
        private System.Windows.Forms.TextBox TxtBxServer;
        private System.Windows.Forms.TabControl TabControlServer;
        private System.Windows.Forms.TabPage TabConnTrack;
        private System.Windows.Forms.TabPage TabPagePublicChatServer;
        private RichTextBox RichTextServerPub;
        private Button BtnStopSrv;
    }
}

