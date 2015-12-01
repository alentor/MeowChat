using MeowChatClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeowChatClientLibrary
{
    public class TabPagePrivateChatClient : TabPage
    {
        private readonly RichTextBox _RchTxtPrivChtClient = new RichTextBox();
        private readonly TextBox _TxtBxPrvMsg = new TextBox();
        private readonly Button _BtnPrvSnd = new Button();

        public event TabPagePrivateChatSendClietHandler TabPagePrivateChatSendClientEvent;

        private int _CursorPosition;

        //Constactor
        public TabPagePrivateChatClient(string name)
        {
            Name = name;
            // RchTxtPrivChat
            _RchTxtPrivChtClient.BackColor = Color.White;
            _RchTxtPrivChtClient.ForeColor = Color.Black;
            _RchTxtPrivChtClient.Location = new Point(3, 3);
            _RchTxtPrivChtClient.Name = name + "RchTxtPrivChat";
            _RchTxtPrivChtClient.ReadOnly = true;
            _RchTxtPrivChtClient.ScrollBars = RichTextBoxScrollBars.Vertical;
            _RchTxtPrivChtClient.Size = new Size(604, 370);
            _RchTxtPrivChtClient.TabIndex = 12;
            _RchTxtPrivChtClient.Text = "";
            _RchTxtPrivChtClient.TextChanged += _RchTxtPrivChtClient_TextChanged;

            // txtBxPrvMsg
            _TxtBxPrvMsg.Location = new Point(3, 379);
            _TxtBxPrvMsg.Name = name + "TxtBxPrvMsg";
            _TxtBxPrvMsg.Size = new Size(511, 20);
            _TxtBxPrvMsg.TabIndex = 0;
            _TxtBxPrvMsg.Select();
            // btnPrvSnd
            _BtnPrvSnd.Location = new Point(520, 377);
            _BtnPrvSnd.Name = name + "BtnPrvSnd";
            _BtnPrvSnd.Size = new Size(89, 23);
            _BtnPrvSnd.TabIndex = 1;
            _BtnPrvSnd.Text = "&Send";
            _BtnPrvSnd.Click += BtnPrvSnd_Click;
            _BtnPrvSnd.UseVisualStyleBackColor = true;
            // TabPagePrivateChat
            Controls.Add(_RchTxtPrivChtClient);
            Controls.Add(_TxtBxPrvMsg);
            Controls.Add(_BtnPrvSnd);
            Location = new Point(4, 28);
            Name = name;
            Padding = new Padding(3);
            Size = new Size(610, 402);
            Text = name;
            UseVisualStyleBackColor = true;
        }

        //Button Send
        private void BtnPrvSnd_Click(object sender, EventArgs e)
        {
            if (_TxtBxPrvMsg.Text.Length > 0)
            {
                TabPagePrivateChatSendClientEvent?.Invoke(Name, _TxtBxPrvMsg.Text);
                _TxtBxPrvMsg.Text = "";
            }
        }

        //Method which handles the event TabPagePrivateReceiveMessageClientEvent, which being fired in FrmChat
        public void TabPagePrivateReceiveMessageClient(string tabName, string privateName, string message, int caseId)
        {
            Invoke(new Action((delegate
            {
                _RchTxtPrivChtClient.SelectionStart = _CursorPosition;
                switch (caseId)
                {
                    case 0:
                        _BtnPrvSnd.Enabled = true;
                        if (tabName == ClientConnection.ClientName && privateName == Name)
                        {
                            _RchTxtPrivChtClient.SelectionColor = Color.Blue;
                            _RchTxtPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + ClientConnection.ClientName + @": " + message + Environment.NewLine;
                            _CursorPosition = _RchTxtPrivChtClient.SelectionStart;
                        }
                        if (tabName == Name && privateName == ClientConnection.ClientName)
                        {
                            _RchTxtPrivChtClient.SelectionColor = Color.Red;
                            _RchTxtPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + Name + @": " + message + Environment.NewLine;
                            _CursorPosition = _RchTxtPrivChtClient.SelectionStart;
                        }
                        break;

                    case 1:
                        if (tabName == Name)
                        {
                            _RchTxtPrivChtClient.SelectionBackColor = Color.Red;
                            _RchTxtPrivChtClient.SelectedText = tabName + ChatMethodsStatic.Time() + " " + " has closed the chat" + Environment.NewLine;
                            _BtnPrvSnd.Enabled = false;
                            _CursorPosition = _RchTxtPrivChtClient.SelectionStart;
                        }
                        break;

                    case 2:
                        if (tabName == Name)
                        {
                            _RchTxtPrivChtClient.SelectionBackColor = Color.Red;
                            _RchTxtPrivChtClient.SelectedText = ChatMethodsStatic.Time() + " " + "Chat have been disconnected" + Environment.NewLine;
                            _BtnPrvSnd.Enabled = false;
                            _CursorPosition = _RchTxtPrivChtClient.SelectionStart;
                        }
                        break;

                    case 3:
                        _BtnPrvSnd.Enabled = true;
                        _RchTxtPrivChtClient.SelectionColor = Color.Black;
                        _RchTxtPrivChtClient.SelectionBackColor = Color.LightGreen;
                        _RchTxtPrivChtClient.SelectedText = tabName + " " + " has resumed the chat" + Environment.NewLine;
                        _CursorPosition = _RchTxtPrivChtClient.SelectionStart;
                        break;
                }
            })));
        }

        private void _RchTxtPrivChtClient_TextChanged(object sender, EventArgs e)
        {
            _RchTxtPrivChtClient.SelectionStart = _RchTxtPrivChtClient.Text.Length;
            _RchTxtPrivChtClient.ScrollToCaret();
        }
    }
}