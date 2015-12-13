using LibraryMeowChat;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MeowChatClientLibrary
{
    public class TabPagePrivateChatClient : TabPage
    {
        private readonly RichTextBox _RichTextPrivChtClient = new RichTextBox();
        private readonly TextBox _TextBoxPrvMsg = new TextBox();
        private readonly Button _BtnPrvSnd = new Button();
        private readonly Button _BtnSendPhotoPrivate = new Button();
        private int _CursorPosition;
        public event TabPagePrivateChatSendClietHandler TabPagePrivateChatSendClientEvent;
        public event TabPagePrivateChatSendClietHandler TabPagePrivateChatSendImageClientEvent;

        //Constactor
        public TabPagePrivateChatClient(string name)
        {
            Name = name;
            // RchTxtPrivChat
            _RichTextPrivChtClient.BackColor = Color.White;
            _RichTextPrivChtClient.ForeColor = Color.Black;
            _RichTextPrivChtClient.Location = new Point(3, 3);
            _RichTextPrivChtClient.Name = name + "_RichTextPrivChat";
            _RichTextPrivChtClient.ReadOnly = true;
            _RichTextPrivChtClient.ScrollBars = RichTextBoxScrollBars.Vertical;
            _RichTextPrivChtClient.Size = new Size(604, 370);
            _RichTextPrivChtClient.TabIndex = 12;
            _RichTextPrivChtClient.Text = "";
            _RichTextPrivChtClient.TextChanged += RichTextPrivChtClientTextChanged;
            // txtBxPrvMsg
            _TextBoxPrvMsg.Location = new Point(3, 379);
            _TextBoxPrvMsg.Name = name + "TxtBxPrvMsg";
            _TextBoxPrvMsg.Size = new Size(482, 20);
            _TextBoxPrvMsg.TabIndex = 0;
            _TextBoxPrvMsg.Select();
            // btnPrvSnd
            _BtnPrvSnd.Location = new Point(520, 377);
            _BtnPrvSnd.Name = name + "BtnPrvSnd";
            _BtnPrvSnd.Size = new Size(89, 23);
            _BtnPrvSnd.TabIndex = 1;
            _BtnPrvSnd.Text = "&Send";
            _BtnPrvSnd.Click += _BtnPrvSnd_Click;
            _BtnPrvSnd.UseVisualStyleBackColor = true;
            // BtnSendPhotoPrivate
            _BtnSendPhotoPrivate.BackgroundImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BtnSendPhotoPublic.BackgroundImage.png"));
            _BtnSendPhotoPrivate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            _BtnSendPhotoPrivate.Location = new System.Drawing.Point(491, 377);
            _BtnSendPhotoPrivate.Name = "BtnSendPhotoPublic";
            _BtnSendPhotoPrivate.Size = new System.Drawing.Size(23, 23);
            //this.BtnSendPhotoPublic.TabIndex = 9;
            _BtnSendPhotoPrivate.UseVisualStyleBackColor = true;
            _BtnSendPhotoPrivate.Click += _BtnSendPhotoPrivate_Click;
            // TabPagePrivateChat
            Controls.Add(_RichTextPrivChtClient);
            Controls.Add(_TextBoxPrvMsg);
            Controls.Add(_BtnPrvSnd);
            Controls.Add(_BtnSendPhotoPrivate);
            Location = new Point(4, 28);
            Name = name;
            Padding = new Padding(3);
            Size = new Size(610, 402);
            Text = name;
            UseVisualStyleBackColor = true;
        }

        //Button Send
        private void _BtnPrvSnd_Click(object sender, EventArgs e)
        {
            if (_TextBoxPrvMsg.Text.Length > 0)
            {
                TabPagePrivateChatSendClientEvent?.Invoke(Name, _TextBoxPrvMsg.Text);
                _TextBoxPrvMsg.Text = "";
                ++ClientStatistics.MessagesPrivateSent;
            }
        }

        //Button Image
        private void _BtnSendPhotoPrivate_Click(object sender, EventArgs e)
        {
            TabPagePrivateChatSendImageClientEvent?.Invoke(Name, null);
        }

        //Method which handles the event TabPagePrivateReceiveMessageClientEvent, which being fired in FrmChat
        public void TabPageTabPagePrivateReceiveMessageClient(string tabName, string privateName, string message, int caseId)
        {
            Invoke(new Action((delegate
            {
                _RichTextPrivChtClient.SelectionStart = _CursorPosition;
                switch (caseId)
                {
                    case 0:
                        _BtnPrvSnd.Enabled = true;
                        if (tabName == ClientConnection.ClientName && privateName == Name)
                        {
                            _RichTextPrivChtClient.SelectionColor = Color.Blue;
                            _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + ClientConnection.ClientName + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        if (tabName == Name && privateName == ClientConnection.ClientName)
                        {
                            _RichTextPrivChtClient.SelectionColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + Name + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case 1:
                        if (tabName == Name)
                        {
                            _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + tabName + " has closed the chat" + Environment.NewLine;
                            _BtnPrvSnd.Enabled = false;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case 2:
                        if (tabName == Name)
                        {
                            _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + tabName + "Chat have been disconnected" + Environment.NewLine;
                            _BtnPrvSnd.Enabled = false;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case 3:
                        _BtnPrvSnd.Enabled = true;
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.LightGreen;
                        _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + tabName + " has resumed the chat" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;

                    case 4:
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.Yellow;
                        _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + "Image sent successfully" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;

                    case 5:
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.Yellow;
                        _RichTextPrivChtClient.SelectedText = GenericStatic.Time() + " " + Name + " has sent an image" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;
                }
            })));
        }

        private void RichTextPrivChtClientTextChanged(object sender, EventArgs e)
        {
            _RichTextPrivChtClient.SelectionStart = _RichTextPrivChtClient.Text.Length;
            _RichTextPrivChtClient.ScrollToCaret();
        }
    }
}