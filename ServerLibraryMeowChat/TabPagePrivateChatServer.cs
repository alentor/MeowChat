using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeowChatServerLibrary
{
    public class TabPagePrivateChatServer : TabPage
    {
        private readonly int _CursorPosition = 0;
        private readonly RichTextBox _RchTxtPrivChtServer = new RichTextBox();
        public string TabName0, TabName1;

        //Constactor
        public TabPagePrivateChatServer(string tabName0, string tabName1)
        {
            TabName0 = tabName0;
            TabName1 = tabName1;
            // RchTxtPrivChat
            _RchTxtPrivChtServer.BackColor = Color.White;
            _RchTxtPrivChtServer.ForeColor = Color.Black;
            _RchTxtPrivChtServer.Location = new Point(0, 3);
            _RchTxtPrivChtServer.Name = tabName0 + " - " + tabName1 + "TabPagePrivateChatServer";
            _RchTxtPrivChtServer.ReadOnly = true;
            _RchTxtPrivChtServer.ScrollBars = RichTextBoxScrollBars.Vertical;
            _RchTxtPrivChtServer.Size = new Size(406, 312);
            _RchTxtPrivChtServer.TabIndex = 12;
            _RchTxtPrivChtServer.Text = "";
            _RchTxtPrivChtServer.TextChanged += _RchTxtPrivChtServer_TextChanged;
            // TabPagePrivateChat
            Controls.Add(_RchTxtPrivChtServer);
            Location = new Point(4, 28);
            Name = TabName0 + " - " + TabName1;
            Padding = new Padding(3);
            Size = new Size(610, 402);
            //this.TabIndex = 1;//mke it more accurate
            Text = TabName0 + " - " + TabName1;
            UseVisualStyleBackColor = true;
            //PrivateReceivedMessageEvent += TabPagePrivateReceivedReceivedMessage;
        }

        //Method which handles the event TabPagePrivateReceiveMessageServerEvent, which being fired in FrmServer
        public void TabPagePrivateReceiveMessageServer(string tabName0, string tabName1, string message, int caseId)
        {
            Invoke(new Action((delegate
            {
                _RchTxtPrivChtServer.SelectionStart = _CursorPosition;

                if (tabName0 == TabName0 || tabName0 == TabName1 && tabName1 == TabName0 || tabName1 == TabName1)
                {
                    switch (caseId)
                    {
                        case 0:
                            if (tabName0 == TabName0)
                            {
                                _RchTxtPrivChtServer.SelectionColor = Color.Blue;
                                _RchTxtPrivChtServer.SelectedText = TabName0 + @": " + message + Environment.NewLine;
                                _RchTxtPrivChtServer.SelectionStart = _CursorPosition;
                            }
                            else
                            {
                                _RchTxtPrivChtServer.SelectionColor = Color.Red;
                                _RchTxtPrivChtServer.SelectedText = TabName1 + @": " + message + Environment.NewLine;
                                _RchTxtPrivChtServer.SelectionStart = _CursorPosition;
                            }
                            break;

                        case 1:
                            _RchTxtPrivChtServer.SelectionBackColor = Color.Red;
                            _RchTxtPrivChtServer.SelectedText = TabName0 + " have closed the chat" + Environment.NewLine;
                            _RchTxtPrivChtServer.SelectionStart = _CursorPosition;
                            break;

                        case 2:
                            _RchTxtPrivChtServer.SelectionBackColor = Color.Red;
                            _RchTxtPrivChtServer.SelectedText = TabName0 + " have quit" + Environment.NewLine;
                            _RchTxtPrivChtServer.SelectionStart = _CursorPosition;
                            break;
                    }
                }
                if (caseId == 3)
                {
                    _RchTxtPrivChtServer.SelectionBackColor = Color.LightGreen;
                    _RchTxtPrivChtServer.SelectedText = TabName0 + " have resumed the chat" + Environment.NewLine;
                    _RchTxtPrivChtServer.SelectionStart = _CursorPosition;
                }
            })));
        }

        private void _RchTxtPrivChtServer_TextChanged(object sender, EventArgs e)
        {
            _RchTxtPrivChtServer.SelectionStart = _RchTxtPrivChtServer.Text.Length;
            _RchTxtPrivChtServer.ScrollToCaret();
        }
    }
}