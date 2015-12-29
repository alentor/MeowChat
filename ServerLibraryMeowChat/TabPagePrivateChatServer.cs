using LibraryMeowChat;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeowChatServerLibrary
{
    public class TabPagePrivateChatServer : TabPage
    {
        public enum TabCommand
        {
            Resumed,
            Closed,
            Disconnected,
            Message,
            Null //No command, only used in MessageStructure constarctor
        }

        private int _CursorPosition;
        private readonly RichTextBox _RichTextPrivChtServer = new RichTextBox();
        public string ClientName, ClientNamePrivate;

        //Constactor
        public TabPagePrivateChatServer(string clientName, string clientNamePrivate)
        {
            ClientName = clientName;
            ClientNamePrivate = clientNamePrivate;
            // RchTxtPrivChat
            _RichTextPrivChtServer.BackColor = Color.White;
            _RichTextPrivChtServer.ForeColor = Color.Black;
            _RichTextPrivChtServer.Location = new Point(0, 3);
            _RichTextPrivChtServer.Name = clientName + " - " + clientNamePrivate + "TabPagePrivateChatServer";
            _RichTextPrivChtServer.ReadOnly = true;
            _RichTextPrivChtServer.ScrollBars = RichTextBoxScrollBars.Vertical;
            _RichTextPrivChtServer.Size = new Size(541, 312);
            _RichTextPrivChtServer.TabIndex = 12;
            _RichTextPrivChtServer.Text = "";
            _RichTextPrivChtServer.TextChanged += RichTextPrivChtServerTextChanged;
            // TabPagePrivateChat
            Controls.Add(_RichTextPrivChtServer);
            Location = new Point(4, 28);
            Name = ClientName + " - " + ClientNamePrivate;
            Padding = new Padding(3);
            Size = new Size(541, 402);
            //this.TabIndex = 1;//mke it more accurate
            Text = ClientName + " - " + ClientNamePrivate;
            UseVisualStyleBackColor = true;
            //PrivateReceivedMessageEvent += TabPagePrivateReceivedReceivedMessage;
        }

        //Method which handles the event TabPagePrivateReceiveMessageServerEvent, which being fired in FrmServer
        public void TabPagePrivateReceiveMessageServerDoAction(string clientName, string clientNamePrivate, string message, TabCommand command)
        {
            Invoke(new Action((delegate
            {
                _RichTextPrivChtServer.SelectionStart = _CursorPosition;
                switch (command)
                {
                    case TabCommand.Resumed:
                        _RichTextPrivChtServer.SelectionBackColor = Color.LightGreen;
                        _RichTextPrivChtServer.SelectedText = Time.NowTime() + " " + clientName + " have resumed the chat" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                        break;

                    case TabCommand.Message:
                        if (clientNamePrivate == ClientName)
                        {
                            _RichTextPrivChtServer.SelectionColor = Color.Blue;
                            _RichTextPrivChtServer.SelectedText = Time.NowTime() + " " + ClientName + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                        }
                        else {
                            _RichTextPrivChtServer.SelectionColor = Color.Red;
                            _RichTextPrivChtServer.SelectedText = Time.NowTime() + " " + ClientNamePrivate + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                        }
                        break;

                    case TabCommand.Closed:
                        _RichTextPrivChtServer.SelectionBackColor = Color.OrangeRed;
                        _RichTextPrivChtServer.SelectedText = Time.NowTime() + " " + clientName + " have closed the chat" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                        break;
                }
            })));
        }

        private void RichTextPrivChtServerTextChanged(object sender, EventArgs e)
        {
            _RichTextPrivChtServer.SelectionStart = _RichTextPrivChtServer.Text.Length;
            _RichTextPrivChtServer.ScrollToCaret();
        }
    }
}