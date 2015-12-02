using System;
using System.Drawing;
using System.Windows.Forms;

namespace MeowChatServerLibrary
{
    public class TabPagePrivateChatServer : TabPage
    {
        private int _CursorPosition = 0;
        private readonly RichTextBox _RichTextPrivChtServer = new RichTextBox();
        public string TabName0, TabName1;

        //Constactor
        public TabPagePrivateChatServer(string tabName0, string tabName1)
        {
            TabName0 = tabName0;
            TabName1 = tabName1;
            // RchTxtPrivChat
            _RichTextPrivChtServer.BackColor = Color.White;
            _RichTextPrivChtServer.ForeColor = Color.Black;
            _RichTextPrivChtServer.Location = new Point(0, 3);
            _RichTextPrivChtServer.Name = tabName0 + " - " + tabName1 + "TabPagePrivateChatServer";
            _RichTextPrivChtServer.ReadOnly = true;
            _RichTextPrivChtServer.ScrollBars = RichTextBoxScrollBars.Vertical;
            _RichTextPrivChtServer.Size = new Size(406, 312);
            _RichTextPrivChtServer.TabIndex = 12;
            _RichTextPrivChtServer.Text = "";
            _RichTextPrivChtServer.TextChanged += RichTextPrivChtServerTextChanged;
            // TabPagePrivateChat
            Controls.Add(_RichTextPrivChtServer);
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
                _RichTextPrivChtServer.SelectionStart = _CursorPosition;

                if (tabName0 == TabName0 || tabName0 == TabName1 && tabName1 == TabName0 || tabName1 == TabName1)
                {
                    switch (caseId)
                    {
                        case 0:
                            if (tabName0 == TabName0)
                            {
                                _RichTextPrivChtServer.SelectionColor = Color.Blue;
                                _RichTextPrivChtServer.SelectedText = TabName0 + @": " + message + Environment.NewLine;
                                _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                            }
                            else
                            {
                                _RichTextPrivChtServer.SelectionColor = Color.Red;
                                _RichTextPrivChtServer.SelectedText = TabName1 + @": " + message + Environment.NewLine;
                                _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                            }
                            break;

                        case 1:
                            _RichTextPrivChtServer.SelectionBackColor = Color.Red;
                            _RichTextPrivChtServer.SelectedText = TabName0 + " have closed the chat" + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                            break;

                        case 2:
                            _RichTextPrivChtServer.SelectionBackColor = Color.Red;
                            _RichTextPrivChtServer.SelectedText = TabName0 + " have quit" + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtServer.SelectionStart;
                            break;
                    }
                }
                if (caseId == 3)
                {
                    _RichTextPrivChtServer.SelectionBackColor = Color.LightGreen;
                    _RichTextPrivChtServer.SelectedText = TabName0 + " have resumed the chat" + Environment.NewLine;
                    _CursorPosition = _RichTextPrivChtServer.SelectionStart;
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