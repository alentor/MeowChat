using MeowChatClient;
using System;
using System.Drawing;
using System.Windows.Forms;
using LibraryMeowChat;

namespace MeowChatClientLibrary {
    public class TabPagePrivateChatClient: TabPage {
        private readonly RichTextBox _RichTextPrivChtClient = new RichTextBox();
        private readonly TextBox _TextBoxPrvMsg = new TextBox();
        private readonly Button _BtnPrvSnd = new Button();

        public event TabPagePrivateChatSendClietHandler TabPagePrivateChatSendClientEvent;

        private int _CursorPosition;

        //Constactor
        public TabPagePrivateChatClient(string name) {
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
            _TextBoxPrvMsg.Size = new Size(511, 20);
            _TextBoxPrvMsg.TabIndex = 0;
            _TextBoxPrvMsg.Select();
            // btnPrvSnd
            _BtnPrvSnd.Location = new Point(520, 377);
            _BtnPrvSnd.Name = name + "BtnPrvSnd";
            _BtnPrvSnd.Size = new Size(89, 23);
            _BtnPrvSnd.TabIndex = 1;
            _BtnPrvSnd.Text = "&Send";
            _BtnPrvSnd.Click += BtnPrvSnd_Click;
            _BtnPrvSnd.UseVisualStyleBackColor = true;
            // TabPagePrivateChat
            Controls.Add(_RichTextPrivChtClient);
            Controls.Add(_TextBoxPrvMsg);
            Controls.Add(_BtnPrvSnd);
            Location = new Point(4, 28);
            Name = name;
            Padding = new Padding(3);
            Size = new Size(610, 402);
            Text = name;
            UseVisualStyleBackColor = true;
        }

        //Button Send
        private void BtnPrvSnd_Click(object sender, EventArgs e) {
            if (_TextBoxPrvMsg.Text.Length > 0) {
                TabPagePrivateChatSendClientEvent?.Invoke(Name, _TextBoxPrvMsg.Text);
                _TextBoxPrvMsg.Text = "";
            }
        }

        //Method which handles the event TabPagePrivateReceiveMessageClientEvent, which being fired in FrmChat
        public void TabPagePrivateReceiveMessageClient(string tabName, string privateName, string message, int caseId) {
            Invoke(new Action((delegate{
                _RichTextPrivChtClient.SelectionStart = _CursorPosition;
                switch (caseId) {
                    case 0:
                        _BtnPrvSnd.Enabled = true;
                        if (tabName == ClientConnection.ClientName && privateName == Name) {
                            _RichTextPrivChtClient.SelectionColor = Color.Blue;
                            _RichTextPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + ClientConnection.ClientName + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        if (tabName == Name && privateName == ClientConnection.ClientName) {
                            _RichTextPrivChtClient.SelectionColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + Name + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case 1:
                        if (tabName == Name) {
                            _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + tabName + " has closed the chat" + Environment.NewLine;
                            _BtnPrvSnd.Enabled = false;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case 2:
                        if (tabName == Name) {
                            _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + tabName + "Chat have been disconnected" + Environment.NewLine;
                            _BtnPrvSnd.Enabled = false;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case 3:
                        _BtnPrvSnd.Enabled = true;
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.LightGreen;
                        _RichTextPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + tabName + " has resumed the chat" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;
                }
            })));
        }

        private void RichTextPrivChtClientTextChanged(object sender, EventArgs e) {
            _RichTextPrivChtClient.SelectionStart = _RichTextPrivChtClient.Text.Length;
            _RichTextPrivChtClient.ScrollToCaret();
        }
    }
}