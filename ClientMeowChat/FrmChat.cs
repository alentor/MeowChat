using LibraryMeowChat;
using MeowChatClientLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MeowChatClient {
    public partial class FrmChat: Form {
        //List which stores all the colors of the Clients current connected
        private readonly List <ChatLines> _ListClientsColor = new List <ChatLines>();

        //Max byte size to be recieved and sent
        private byte[] _ByteMessage = new byte[1024];

        private int _CursorPosition;

        public FrmChat() {
            InitializeComponent();
            TextBoxPubMsg.Select();
        }

        //Event to connect FrmChat with every TabPagePrivateChatClient in TabControlClient
        public event TabPagePrivateChatReceiveClientHandler PrivateReceivedMessageClientEvent;

        //On FrmChat Load we are sending a reuqest to get the list of all the connected clients form the server
        private void FrmChat_Load(object sender, EventArgs e) {
            try {
                var msgToSend = new MessageStracture {
                    Command = Command.List,
                    ClientName = ClientConnection.ClientName
                };
                _ByteMessage = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnSend, null);
                _ByteMessage = new byte[1024];
                ClientConnection.Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> FrmChat_Load", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //This method handles all the received data from the server
        private void OnReceive(IAsyncResult ar) {
            try {
                //Let the server know the message was recieved
                ClientConnection.Socket.EndReceive(ar);
                if (!ClientConnection.Status) {
                    return;
                }
                //Convert message from bytes to messageStracure class and store it in msgReceieved
                var msgReceived = new MessageStracture(_ByteMessage);
                //Set new bytes and start recieving again
                _ByteMessage = new byte[1024];
                if (msgReceived.Command == Command.Disconnect) {
                    Invoke(new Action((delegate{
                        ClientConnection.ServerDisconnectCall();
                        BtnPubSnd.Enabled = false;
                        ListBoxClientList.Items.Clear();
                        RichTextClientPub.SelectionStart = _CursorPosition;
                        RichTextClientPub.SelectionColor = Color.Black;
                        RichTextClientPub.SelectionBackColor = Color.Tomato;
                        RichTextClientPub.SelectedText = ChatMethodsStatic.Time() + " Disconnected from the server" + Environment.NewLine;
                        _CursorPosition = RichTextClientPub.SelectionStart;
                    })));
                    return;
                }
                ClientConnection.Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
                //Case switch statment message stracture
                switch (msgReceived.Command) {
                    case Command.Login:
                        Invoke(new Action((delegate{
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.LightGreen;
                            ListBoxClientList.Items.Add(msgReceived.ClientName);
                            RichTextClientPub.SelectedText = ChatMethodsStatic.Time() + " " + msgReceived.Message + Environment.NewLine;
                            if (msgReceived.ClientName != ClientConnection.ClientName) {
                                _ListClientsColor.Add(new ChatLines(msgReceived.ClientName));
                            }
                            _CursorPosition = RichTextClientPub.SelectionStart;
                        })));
                        break;

                    case Command.List:
                        ClientConnection.ClientName = msgReceived.ClientName; //Set ClientConnection name
                        Invoke(new Action((delegate{
                            Text = @"Chat: " + ClientConnection.ClientName; //Set window name
                            //_ClientsColor.Add(new ClientChatProp(ClientConnection.ClientName)); //Add this Client to the ClientChatProp list
                            ListBoxClientList.Items.AddRange(msgReceived.Message.Split(','));
                            //remove the empty selection box in list view
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectedText = @"<<< " + ClientConnection.ClientName + @" has joined the room >>>" + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                            ListBoxClientList.Items.RemoveAt(ListBoxClientList.Items.Count - 1);
                            //Add all the connected clients to ClientChatProp list
                            foreach (var t in ListBoxClientList.Items) {
                                _ListClientsColor.Add(new ChatLines(t.ToString()));
                            }
                        })));
                        break;

                    case Command.Logout:
                        Invoke(new Action((delegate{
                            ListBoxClientList.Items.Remove(msgReceived.ClientName);
                            for (var i = 0; i < _ListClientsColor.Count; i++) {
                                if (_ListClientsColor[i].Name == msgReceived.ClientName) {
                                    _ListClientsColor.Remove(_ListClientsColor[i]);
                                    PrivateReceivedMessageClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2);
                                }
                            }
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.Tomato;
                            RichTextClientPub.SelectedText = ChatMethodsStatic.Time() + " " + msgReceived.Message + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                        })));
                        break;

                    case Command.NameChange:
                        Invoke(new Action((delegate{
                            var index = ListBoxClientList.FindString(msgReceived.ClientName);
                            ListBoxClientList.Items[index] = msgReceived.Message;

                            foreach (var clientColor in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName)) {
                                clientColor.Name = msgReceived.Message;
                            }
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.CornflowerBlue;
                            RichTextClientPub.SelectedText = ChatMethodsStatic.Time() + " " + @"<<< " + msgReceived.ClientName + @" have changed nickname to " + msgReceived.Message + @" >>>" + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                            if (ClientConnection.ClientName == msgReceived.ClientName) {
                                Text = @"Chat: " + msgReceived.Message;
                                ClientConnection.ClientName = msgReceived.Message;
                            }
                            foreach (var tabPage in TabControlClient.TabPages.Cast <TabPage>().Where(tabPage => tabPage.Name == msgReceived.ClientName)) {
                                tabPage.Name = msgReceived.Message;
                                tabPage.Text = msgReceived.Message;
                                TabControlClient.Invalidate();
                            }
                            ChatMethodsStatic.FormatItemSize(TabControlClient);
                        })));
                        goto case Command.ColorChange;

                    case Command.Message:
                        Invoke(new Action((delegate{
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            var color = ColorTranslator.FromHtml(msgReceived.Color);
                            RichTextClientPub.SelectedText = ChatMethodsStatic.Time() + " ";
                            var selectionStart = RichTextClientPub.SelectionStart;
                            RichTextClientPub.SelectionColor = color;
                            RichTextClientPub.SelectedText = msgReceived.ClientName + ": " + msgReceived.Message /*+ Environment.NewLine*/;
                            RichTextClientPub.SelectedText = Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                            foreach (var clientColor in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName)) {
                                int[] selectionArr = {selectionStart, RichTextClientPub.TextLength - selectionStart};
                                clientColor.Messages.Add(selectionArr);
                            }
                        })));
                        break;

                    case Command.ColorChange:
                        Invoke(new Action((delegate{
                            var newColor = ColorTranslator.FromHtml(msgReceived.Color);
                            foreach (var selectedText in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName).SelectMany(clientColor => clientColor.Messages)) {
                                RichTextClientPub.Select(selectedText[0], selectedText[1]);
                                RichTextClientPub.SelectionColor = newColor;
                            }
                        })));
                        break;

                    case Command.PrivateStart:
                        if (TabControlClient.TabPages.Cast <TabPage>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == msgReceived.ClientName)) {
                            PrivateReceivedMessageClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 3);
                            return;
                        }
                        Invoke(new Action((delegate{
                            NewTabPagePrivateChatClient(msgReceived.ClientName);
                            ChatMethodsStatic.FormatItemSize(TabControlClient);
                        })));
                        break;

                    case Command.PrivateMessage:
                        PrivateReceivedMessageClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 0);
                        break;

                    case Command.PrivateStop:
                        PrivateReceivedMessageClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1);
                        break;

                    case Command.ServerMessage:
                        Invoke(new Action((delegate{
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.MediumPurple;
                            RichTextClientPub.SelectedText = ChatMethodsStatic.Time() + " " + "Server Message: " + msgReceived.Message + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                        })));
                        break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //After the message is sent/recieved end the async operation
        private static void OnSend(IAsyncResult ar) {
            try {
                ClientConnection.Socket.EndSend(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnSend", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Public Chat Send button
        private void BtnSend_Click(object sender, EventArgs e) {
            try {
                if (TextBoxPubMsg.Text.Length <= 0) {
                    return;
                }

                var msgToSend = new MessageStracture {
                    Command = Command.Message,
                    ClientName = ClientConnection.ClientName,
                    Color = ClientConnection.Color,
                    Message = TextBoxPubMsg.Text
                };
                var msgToSendByte = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                //reset the TextBoxPubMsg
                TextBoxPubMsg.Text = null;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> BtnSend_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //File => Reconnect
        private void ReconnectToolStripMenuItem_Click(object sender, EventArgs e) {
            if (ClientConnection.Status) {
                return;
            }
            ClientConnection.Connect(ClientConnection.Address, ClientConnection.Port, ClientConnection.ClientName);
            Thread.Sleep(5);
            FrmChat_Load(this, null);
            BtnPubSnd.Enabled = true;
        }

        //File => Disconnect
        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!ClientConnection.Status) {
                return;
            }
            foreach (TabPage tabPage in TabControlClient.TabPages) {
                PrivateReceivedMessageClientEvent?.Invoke(tabPage.Name, "0", "0", 2);
            }
            ClientConnection.Disconnect();
            Thread.Sleep(50);
            BtnPubSnd.Enabled = false;
            Text = @"Chat: " + ClientConnection.ClientName + @"[Disconnected]";
            RichTextClientPub.SelectionColor = Color.Black;
            RichTextClientPub.SelectionBackColor = Color.Crimson;
            RichTextClientPub.SelectedText = @"You are disonnected now " + Environment.NewLine;
            ListBoxClientList.Items.Clear();
            _ListClientsColor.Clear();
        }

        //File => Exit
        private void ClickExitToolStripMenuItem(object sender, EventArgs e) {
            Close();
        }

        //Chat => Change Name
        private void ChangeNameToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                if (!ClientConnection.Status) {
                    return;
                }
                //call to frmChangeName
                using (var changeName = new ChangeName(ClientConnection.ClientName)) {
                    if (changeName.ShowDialog() != DialogResult.OK) {
                        return;
                    }
                    if (ListBoxClientList.Items.Cast <object>().Any(item => changeName.NameNew == item.ToString())) {
                        MessageBox.Show(@"The name " + changeName.NameNew + @"already taken", @"Chat: 5" + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    var msgToSend = new MessageStracture {
                        Command = Command.NameChange,
                        ClientName = ClientConnection.ClientName,
                        Message = changeName.NameNew
                    };
                    var msgToSendByte = msgToSend.ToByte();
                    ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ChangeNameToolStripMenuItem_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chat => Change color
        private void ChangeColorToolStripMenuItem_Click(object sender, EventArgs e) {
            BtnColorPick_Click(this, null);
        }

        //Closing FrmChat
        private void FrmChat_FormClosing(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show(@"Are you sure you want to exit?", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No) {
                e.Cancel = true;
                return;
            }
            if (!ClientConnection.Status) {
                return;
            }
            ClientConnection.Status = false;
            ClientConnection.Disconnect();
            //Give some time overhead for the client finish sending the disconnect call
            //before terminating all the the related threads
            Thread.Sleep(10);
        }

        //Color Picker
        private void BtnColorPick_Click(object sender, EventArgs e) {
            if (!ClientConnection.Status) {
                return;
            }
            var pickColor = ColorPicker.ShowDialog();
            try {
                if (pickColor == DialogResult.OK) {
                    var colorHex = ChatMethodsStatic.HexConverter(ColorPicker.Color);
                    ClientConnection.Color = colorHex;
                    var msgToSend = new MessageStracture {
                        Command = Command.ColorChange,
                        ClientName = ClientConnection.ClientName,
                        Color = colorHex
                    };
                    var msgToSendByte = msgToSend.ToByte();
                    ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> BtnColorPick_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Automaticlaly scrolldown rchTxtPubChat
        private void RichTextChatBoxText_Changed(object sender, EventArgs e) {
            RichTextClientPub.SelectionStart = RichTextClientPub.Text.Length;
            RichTextClientPub.ScrollToCaret();
        }

        //List double click to start a new private chat
        private void ListBoxClientList_DoubleClick(object sender, EventArgs e) {
            try {
                if (ListBoxClientList.SelectedItem.ToString() == ClientConnection.ClientName) {
                    MessageBox.Show(@"You can't start a private chat with yourself", @"Chat: 5" + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (TabControlClient.TabPages.OfType <TabPagePrivateChatClient>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == ListBoxClientList.SelectedItem.ToString())) {
                    MessageBox.Show(@"That private chat already opned", @"Chat: 5" + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                NewTabPagePrivateChatClient(ListBoxClientList.SelectedItem.ToString());
                var msgToSend = new MessageStracture {
                    Command = Command.PrivateStart,
                    ClientName = ClientConnection.ClientName,
                    Private = ListBoxClientList.SelectedItem.ToString()
                };
                var msgToSendByte = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);

                Invoke(new Action((delegate{
                    ChatMethodsStatic.FormatItemSize(TabControlClient);
                })));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message+ @" -> ListBoxClientList_DoubleClick", @"Chat: "  + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Method to which createsa new class of TabPagePrivateChatClient and adds it to TabControlClient
        private void NewTabPagePrivateChatClient(string tabName) {
            var newPrivateTab = new TabPagePrivateChatClient(tabName);
            PrivateReceivedMessageClientEvent += newPrivateTab.TabPagePrivateReceiveMessageClient;
            newPrivateTab.TabPagePrivateChatSendClientEvent += TabPagePrivateChatSendClient;
            TabControlClient.TabPages.Add(newPrivateTab);
        }

        //Send private message method event
        private void TabPagePrivateChatSendClient(string namePrivate, string message) {
            var msgToSend = new MessageStracture {
                Command = Command.PrivateMessage,
                ClientName = ClientConnection.ClientName,
                Private = namePrivate,
                Message = message
            };
            var msgToSendByte = msgToSend.ToByte();
            ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
        }

        //TabControl DrawItem, used to the draw the X on each tab
        private void TabControlClient_DrawItem(object sender, DrawItemEventArgs e) {
            //Draw the name of the tab
            e.Graphics.DrawString(TabControlClient.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 10, e.Bounds.Top + 7);
            for (var i = 1; i < TabControlClient.TabPages.Count; i++) {
                var tabRect = TabControlClient.GetTabRect(i);
                //Not active tab
                if (i != TabControlClient.SelectedIndex) {
                    //Rectangle r = TabControlClient.TabPages[i].Text;
                    //using (Brush brush = new SolidBrush(Color.Yellow)) {
                    //    e.Graphics.FillRectangle(brush, tabXarea.X + tabXarea.Width - 18, 6, 16, 16);
                    //}
                    using (var pen = new Pen(Color.Black, 2)) {
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        e.Graphics.DrawLine(pen, tabRect.Right - 9, 8, tabRect.Right - 21, 20);
                        e.Graphics.DrawLine(pen, tabRect.Right - 9, 20, tabRect.Right - 21, 8);
                        e.Graphics.SmoothingMode = SmoothingMode.Default;
                        pen.Color = Color.Red;
                        pen.Width = 1;
                        e.Graphics.DrawRectangle(pen, tabRect.Right - 23, 6, 16, 16);
                        pen.Dispose();
                    }
                }
                //Active tab
                else {
                    //Rectangle r = TabControlClient.TabPages[i].Text;
                    //RectangleF tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                    //using (Brush brush = new SolidBrush(Color.Purple)) {
                    //    e.Graphics.FillRectangle(brush, tabXarea.X + tabXarea.Width - 18, 6, 16, 16);
                    //}
                    using (var pen = new Pen(Color.Silver, 2)) {
                        e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                        e.Graphics.DrawLine(pen, tabRect.Right - 9, 8, tabRect.Right - 21, 20);
                        e.Graphics.DrawLine(pen, tabRect.Right - 9, 20, tabRect.Right - 21, 8);
                        e.Graphics.SmoothingMode = SmoothingMode.Default;
                        pen.Color = Color.Red;
                        pen.Width = 1;
                        //e.Graphics.DrawRectangle(pen, tabXarea.X + tabXarea.Width - 18, 6, 16, 16);
                        e.Graphics.DrawRectangle(pen, tabRect.Right - 23, 6, 16, 16);
                        pen.Dispose();
                    }
                }
            }
        }

        //Click event on TabPage, checks whenever the click was in the X rectangle area
        private void TabControlClient_MouseClick(object sender, MouseEventArgs e) {
            for (var i = 1; i < TabControlClient.TabPages.Count; i++) {
                var tabRect = TabControlClient.GetTabRect(i);
                //Getting the position of the "x" mark.

                //Rectangle tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                var closeXButtonArea = new Rectangle(tabRect.Right - 23, 6, 16, 16);
                //Rectangle closeButton = new Rectangle(tabRect.Right - 13, tabRect.Top + 6, 9, 7);
                if (closeXButtonArea.Contains(e.Location)) {
                    if (MessageBox.Show(@"Would you like to Close this Tab?", @"Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        var msgToSend = new MessageStracture {
                            Command = Command.PrivateStop,
                            ClientName = ClientConnection.ClientName,
                            Private = TabControlClient.TabPages[i].Name
                        };
                        var msgToSendByte = msgToSend.ToByte();
                        ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                        TabControlClient.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
            var about = new FrmAbout();
            about.ShowDialog();
        }
    }
}