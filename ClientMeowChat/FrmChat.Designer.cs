using System.Windows.Forms;

namespace MeowChatClient
{
    partial class FrmChat
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmChat));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chatToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeColorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.tabPagePublicChatClient = new System.Windows.Forms.TabPage();
            this.rchTxtClientPub = new System.Windows.Forms.RichTextBox();
            this.txtBxPubMsg = new System.Windows.Forms.TextBox();
            this.btnPubSnd = new System.Windows.Forms.Button();
            this.lstClientList = new System.Windows.Forms.ListBox();
            this.btnColorPick = new System.Windows.Forms.Button();
            this.TabControlClient = new System.Windows.Forms.TabControl();
            this.menuStrip1.SuspendLayout();
            this.tabPagePublicChatClient.SuspendLayout();
            this.TabControlClient.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.chatToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(625, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reconnectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // reconnectToolStripMenuItem
            // 
            this.reconnectToolStripMenuItem.Name = "reconnectToolStripMenuItem";
            this.reconnectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.reconnectToolStripMenuItem.Text = "&Reconnect";
            this.reconnectToolStripMenuItem.Click += new System.EventHandler(this.ReconnectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.disconnectToolStripMenuItem.Text = "&Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.DisconnectToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ClickexitToolStripMenuItem);
            // 
            // chatToolStripMenuItem
            // 
            this.chatToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeNameToolStripMenuItem,
            this.changeColorToolStripMenuItem});
            this.chatToolStripMenuItem.Name = "chatToolStripMenuItem";
            this.chatToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.chatToolStripMenuItem.Text = "&Chat";
            // 
            // changeNameToolStripMenuItem
            // 
            this.changeNameToolStripMenuItem.Name = "changeNameToolStripMenuItem";
            this.changeNameToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.changeNameToolStripMenuItem.Text = "Cha&nge name";
            this.changeNameToolStripMenuItem.Click += new System.EventHandler(this.changeNameToolStripMenuItem_Click);
            // 
            // changeColorToolStripMenuItem
            // 
            this.changeColorToolStripMenuItem.Name = "changeColorToolStripMenuItem";
            this.changeColorToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.changeColorToolStripMenuItem.Text = "Change &Color";
            this.changeColorToolStripMenuItem.Click += new System.EventHandler(this.changeColorToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tabPagePublicChatClient
            // 
            this.tabPagePublicChatClient.Controls.Add(this.rchTxtClientPub);
            this.tabPagePublicChatClient.Controls.Add(this.txtBxPubMsg);
            this.tabPagePublicChatClient.Controls.Add(this.btnPubSnd);
            this.tabPagePublicChatClient.Controls.Add(this.lstClientList);
            this.tabPagePublicChatClient.Controls.Add(this.btnColorPick);
            this.tabPagePublicChatClient.Location = new System.Drawing.Point(4, 28);
            this.tabPagePublicChatClient.Name = "tabPagePublicChatClient";
            this.tabPagePublicChatClient.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePublicChatClient.Size = new System.Drawing.Size(610, 402);
            this.tabPagePublicChatClient.TabIndex = 10;
            this.tabPagePublicChatClient.Text = "Public Chat";
            this.tabPagePublicChatClient.UseVisualStyleBackColor = true;
            // 
            // rchTxtClientPub
            // 
            this.rchTxtClientPub.BackColor = System.Drawing.Color.White;
            this.rchTxtClientPub.ForeColor = System.Drawing.Color.Black;
            this.rchTxtClientPub.Location = new System.Drawing.Point(3, 3);
            this.rchTxtClientPub.Name = "rchTxtClientPub";
            this.rchTxtClientPub.ReadOnly = true;
            this.rchTxtClientPub.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rchTxtClientPub.Size = new System.Drawing.Size(484, 370);
            this.rchTxtClientPub.TabIndex = 8;
            this.rchTxtClientPub.Text = "";
            this.rchTxtClientPub.TextChanged += new System.EventHandler(this.RchTxtChatBoxText_Changed);
            // 
            // txtBxPubMsg
            // 
            this.txtBxPubMsg.Location = new System.Drawing.Point(3, 379);
            this.txtBxPubMsg.Name = "txtBxPubMsg";
            this.txtBxPubMsg.Size = new System.Drawing.Size(482, 20);
            this.txtBxPubMsg.TabIndex = 0;
            // 
            // btnPubSnd
            // 
            this.btnPubSnd.Location = new System.Drawing.Point(520, 377);
            this.btnPubSnd.Name = "btnPubSnd";
            this.btnPubSnd.Size = new System.Drawing.Size(89, 23);
            this.btnPubSnd.TabIndex = 1;
            this.btnPubSnd.Text = "&Send";
            this.btnPubSnd.UseVisualStyleBackColor = true;
            this.btnPubSnd.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lstClientList
            // 
            this.lstClientList.FormattingEnabled = true;
            this.lstClientList.Location = new System.Drawing.Point(493, 3);
            this.lstClientList.Name = "lstClientList";
            this.lstClientList.Size = new System.Drawing.Size(114, 368);
            this.lstClientList.TabIndex = 5;
            this.lstClientList.DoubleClick += new System.EventHandler(this.lstClientList_DoubleClick);
            // 
            // btnColorPick
            // 
            this.btnColorPick.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnColorPick.BackgroundImage")));
            this.btnColorPick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnColorPick.Location = new System.Drawing.Point(491, 377);
            this.btnColorPick.Name = "btnColorPick";
            this.btnColorPick.Size = new System.Drawing.Size(23, 23);
            this.btnColorPick.TabIndex = 7;
            this.btnColorPick.UseVisualStyleBackColor = true;
            this.btnColorPick.Click += new System.EventHandler(this.btnColorPick_Click);
            // 
            // TabControlClient
            // 
            this.TabControlClient.Controls.Add(this.tabPagePublicChatClient);
            this.TabControlClient.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.TabControlClient.ItemSize = new System.Drawing.Size(80, 24);
            this.TabControlClient.Location = new System.Drawing.Point(3, 21);
            this.TabControlClient.Name = "TabControlClient";
            this.TabControlClient.SelectedIndex = 0;
            this.TabControlClient.Size = new System.Drawing.Size(618, 434);
            this.TabControlClient.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControlClient.TabIndex = 9;
            this.TabControlClient.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.TabControlClient_DrawItem);
            this.TabControlClient.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TabControlClient_MouseClick);
            // 
            // FrmChat
            // 
            this.AcceptButton = this.btnPubSnd;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(625, 455);
            this.Controls.Add(this.TabControlClient);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FrmChat";
            this.Text = "frmChat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmChat_FormClosing);
            this.Load += new System.EventHandler(this.FrmChat_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPagePublicChatClient.ResumeLayout(false);
            this.tabPagePublicChatClient.PerformLayout();
            this.TabControlClient.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reconnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chatToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem changeNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private ColorDialog colorPicker;
        private ToolStripMenuItem changeColorToolStripMenuItem;
        private TabPage tabPagePublicChatClient;
        private RichTextBox rchTxtClientPub;
        private TextBox txtBxPubMsg;
        private Button btnPubSnd;
        private ListBox lstClientList;
        private Button btnColorPick;
        private TabControl TabControlClient;
    }
}