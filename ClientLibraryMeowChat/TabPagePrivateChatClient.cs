using LibraryMeowChat;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace MeowChatClientLibrary {
    public class TabPagePrivateChatClient: TabPage {
        public enum TabCommand {
            Resumed,
            Closed,
            Disconnected,
            Disconnect,
            Message,
            ImageMessage,
            ImageMessageSent,
            NameChange,
            Null //No command, only used in MessageStructure constarctor
        }

        private readonly RichTextBox _RichTextPrivChtClient = new RichTextBox();
        private readonly TextBox _TextBoxPrvMsg = new TextBox();
        private readonly Button _BtnPrivateSendMessage = new Button();
        private readonly Button _BtnPrivateSendImage = new Button();
        private int _CursorPosition;
        public event TabPagePrivateChatSendClietHandler TabPagePrivateChatSendClientEvent;
        public event TabPagePrivateChatSendImageClietHandler TabPagePrivateChatSendImageClietEvent;

        //Constactor
        public TabPagePrivateChatClient(string name) {
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
            _BtnPrivateSendMessage.Location = new Point(520, 377);
            _BtnPrivateSendMessage.Name = name + "BtnPrvSnd";
            _BtnPrivateSendMessage.Size = new Size(89, 23);
            _BtnPrivateSendMessage.TabIndex = 1;
            _BtnPrivateSendMessage.Text = "&Send";
            _BtnPrivateSendMessage.Click += BtnPrivateSendMessageClick;
            _BtnPrivateSendMessage.UseVisualStyleBackColor = true;
            // BtnSendPhotoPrivate
            _BtnPrivateSendImage.BackgroundImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BtnSendPhotoPublic.BackgroundImage.png"));
            _BtnPrivateSendImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            _BtnPrivateSendImage.Location = new System.Drawing.Point(491, 377);
            _BtnPrivateSendImage.Name = "BtnSendPhotoPublic";
            _BtnPrivateSendImage.Size = new System.Drawing.Size(23, 23);
            //this.BtnSendPhotoPublic.TabIndex = 9;
            _BtnPrivateSendImage.UseVisualStyleBackColor = true;
            _BtnPrivateSendImage.Click += BtnPrivateSendImageClick;
            // TabPagePrivateChat
            Controls.Add(_RichTextPrivChtClient);
            Controls.Add(_TextBoxPrvMsg);
            Controls.Add(_BtnPrivateSendMessage);
            Controls.Add(_BtnPrivateSendImage);
            Location = new Point(4, 28);
            Name = name;
            Padding = new Padding(3);
            Size = new Size(610, 402);
            Text = name;
            UseVisualStyleBackColor = true;
        }

        //Button Send
        private void BtnPrivateSendMessageClick(object sender, EventArgs e) {
            if (_TextBoxPrvMsg.Text.Length > 0) {
                TabPagePrivateChatSendClientEvent?.Invoke(Name, _TextBoxPrvMsg.Text);
                _TextBoxPrvMsg.Text = "";
            }
        }

        //Button ImageMessage
        private void BtnPrivateSendImageClick(object sender, EventArgs e) {
            //TabPagePrivateChatSendImageClientEvent?.Invoke(Name, null);
            TabPagePrivateChatSendImageClietEvent?.Invoke(Name);
        }

        //Method which handles the event TabPagePrivateReceiveMessageClientEvent, which being fired in FrmChat
        public void TabPageTabPagePrivateReceiveMessageClient(string tabName, string privateName, string message, TabCommand command) {
            Invoke(new Action((delegate{
                _RichTextPrivChtClient.SelectionStart = _CursorPosition;
                switch (command) {
                    case TabCommand.Message:
                        if (tabName == Client.Name && privateName == Name) {
                            _RichTextPrivChtClient.SelectionColor = Color.Blue;
                            _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + Client.Name + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        if (tabName == Name && privateName == Client.Name) {
                            _RichTextPrivChtClient.SelectionColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + Name + @": " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }

                        break;
                    case TabCommand.Closed:
                        if (tabName == Name) {
                            _BtnPrivateSendMessage.Enabled = false;
                            _BtnPrivateSendImage.Enabled = false;
                            _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + tabName + " has closed the chat" + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }

                        break;
                    case TabCommand.Resumed:
                        _BtnPrivateSendMessage.Enabled = true;
                        _BtnPrivateSendImage.Enabled = true;
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.LightGreen;
                        _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + tabName + " has resumed the chat" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;

                    case TabCommand.Disconnected:
                        if (tabName == Name) {
                            _BtnPrivateSendMessage.Enabled = false;
                            _BtnPrivateSendImage.Enabled = false;
                            _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + tabName + " have been disconnected" + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case TabCommand.NameChange:
                        if (Name == tabName) {
                            _RichTextPrivChtClient.SelectionColor = Color.Red;
                            _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + Name + @" have changed his name to " + message + Environment.NewLine;
                            _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        }
                        break;

                    case TabCommand.ImageMessage:
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.Yellow;
                        _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + Name + " has sent an image" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;

                    case TabCommand.ImageMessageSent:
                        _RichTextPrivChtClient.SelectionColor = Color.Black;
                        _RichTextPrivChtClient.SelectionBackColor = Color.Yellow;
                        _RichTextPrivChtClient.SelectedText = Time.NowTime() + " " + "ImageMessage sent successfully" + Environment.NewLine;
                        _CursorPosition = _RichTextPrivChtClient.SelectionStart;
                        break;

                    case TabCommand.Disconnect:
                        _BtnPrivateSendMessage.Enabled = false;
                        _BtnPrivateSendImage.Enabled = false;
                        _RichTextPrivChtClient.SelectionBackColor = Color.Red;
                        _RichTextPrivChtClient.SelectedText = Time.NowTime() + " have been disconnected" + Environment.NewLine;
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