using LibraryMeowChat;
using MeowChatClientLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MeowChatClient
{
    public partial class FrmChat : Form
    {
        //List which stores all the colors of the Clients current connected
        private readonly List<ClientChatHistory> _ListClientsColor = new List<ClientChatHistory>();
        //Max byte size to be recieved and sent
        private byte[] _ByteMessage = new byte[2097152];
        private int _CursorPosition;
        private readonly Statistic _FrmStatistics = new Statistic();
        private readonly FrmClientImages _FrmClientImages = new FrmClientImages();
        //Fired when the client recieves a message with one of the following commands from the servers PrivateMessage/PrivateStarted and PrivateStopped
        public event TabPagePrivateChatReceiveClientHandler TabPagePrivateChatReceiveClientEvent;
        //Fired when statics window needs to be updated
        public event FrmStatisticsUpdateHandler FrmStatisticsUpdateEvent;
        public event FrmClientImagesChangeNameHandler FrmClientImagesChangeNameEvent;

        public FrmChat()
        {
            InitializeComponent();
            TextBoxPubMsg.Select();
        }

        //On FrmChat Load we are sending a reuqest to get the list of all the connected clients form the server
        private void FrmChat_Load(object sender, EventArgs e)
        {
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
            //ClientStatistics.StartStatistics();
            //_FrmStatistics.Start();
            //FrmStatisticsUpdateEvent += _FrmStatistics.UpdateStatics;
            //FrmClientImagesChangeNameEvent += _FrmClientImages.ChangeTabName;
            //try
            //{
            //    MessageStructure msgToSend = new MessageStructure
            //    {
            //        Command = Command.List,
            //        ClientName = ClientConnection.ClientName
            //    };
            //    _ByteMessage = msgToSend.ToByte();
            //    ClientConnection.Socket.BeginSend(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnSend, null);
            //    _ByteMessage = new byte[2097152];
            //    ClientConnection.Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + @" -> FrmChat_Load", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        //This method handles all the received data from the server
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                //Let the server know the message was recieved
                ClientConnection.Socket.EndReceive(ar);
                if (!ClientConnection.Status)
                {
                    return;
                }
                //Convert message from bytes to messageStracure class and store it in msgReceieved
                MessageStructure msgReceived = new MessageStructure(_ByteMessage);
                //Set new bytes and start recieving again
                _ByteMessage = new byte[2097152];
                if (msgReceived.Command == Command.Disconnect)
                {
                    Invoke(new Action((delegate
                    {
                        ClientConnection.ServerDisconnectCall();
                        ClientStatistics.StopStatistics();
                        BtnPubSnd.Enabled = false;
                        ListBoxClientList.Items.Clear();
                        RichTextClientPub.SelectionStart = _CursorPosition;
                        RichTextClientPub.SelectionColor = Color.Black;
                        RichTextClientPub.SelectionBackColor = Color.Tomato;
                        RichTextClientPub.SelectedText = GenericStatic.Time() + " Disconnected from the server" + Environment.NewLine;
                        _CursorPosition = RichTextClientPub.SelectionStart;
                    })));
                    return;
                }
                ClientConnection.Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
                //Case switch statment message stracture
                switch (msgReceived.Command)
                {
                    case Command.Login:
                        Invoke(new Action((delegate
                        {
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.LightGreen;
                            ListBoxClientList.Items.Add(msgReceived.ClientName);
                            RichTextClientPub.SelectedText = GenericStatic.Time() + " " + msgReceived.Message + Environment.NewLine;
                            if (msgReceived.ClientName != ClientConnection.ClientName)
                            {
                                _ListClientsColor.Add(new ClientChatHistory(msgReceived.ClientName));
                            }
                            _CursorPosition = RichTextClientPub.SelectionStart;
                        })));
                        break;

                    case Command.List:
                        ClientConnection.ClientName = msgReceived.ClientName; //Set ClientConnection name
                        Invoke(new Action((delegate
                        {
                            Text = @"Chat: " + ClientConnection.ClientName; //Set window name
                            //_ClientsColor.Add(new ClientChatProp(ClientConnection.ClientName)); //Add this Client to the ClientChatProp list
                            ListBoxClientList.Items.AddRange(msgReceived.Message.Split(','));
                            //remove the empty selection box in list view
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectedText = @"<<< " + ClientConnection.ClientName + @" has joined the room >>>" + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                            ListBoxClientList.Items.RemoveAt(ListBoxClientList.Items.Count - 1);
                            //Add all the connected clients to ClientChatProp list
                            foreach (object t in ListBoxClientList.Items)
                            {
                                _ListClientsColor.Add(new ClientChatHistory(t.ToString()));
                            }
                        })));
                        break;

                    case Command.Logout:
                        Invoke(new Action((delegate
                        {
                            ListBoxClientList.Items.Remove(msgReceived.ClientName);
                            for (int i = 0; i < _ListClientsColor.Count; i++)
                            {
                                if (_ListClientsColor[i].Name == msgReceived.ClientName)
                                {
                                    _ListClientsColor.Remove(_ListClientsColor[i]);
                                    TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2);
                                }
                            }
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.Tomato;
                            RichTextClientPub.SelectedText = GenericStatic.Time() + " " + msgReceived.Message + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                        })));
                        break;

                    case Command.NameChange:
                        Invoke(new Action((delegate
                        {
                            int index = ListBoxClientList.FindString(msgReceived.ClientName);
                            ListBoxClientList.Items[index] = msgReceived.Message;

                            foreach (ClientChatHistory clientColor in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName))
                            {
                                clientColor.Name = msgReceived.Message;
                            }
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.CornflowerBlue;
                            RichTextClientPub.SelectedText = GenericStatic.Time() + " " + @"<<< " + msgReceived.ClientName + @" have changed nickname to " + msgReceived.Message + @" >>>" + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                            if (ClientConnection.ClientName == msgReceived.ClientName)
                            {
                                Text = @"Chat: " + msgReceived.Message;
                                ClientConnection.ClientName = msgReceived.Message;
                            }
                            foreach (TabPage tabPage in TabControlClient.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Name == msgReceived.ClientName))
                            {
                                tabPage.Name = msgReceived.Message;
                                tabPage.Text = msgReceived.Message;
                                TabControlClient.Invalidate();
                            }
                            GenericStatic.FormatItemSize(TabControlClient);
                        })));
                        Invoke(new Action((delegate
                        {
                            _FrmClientImages.Text = msgReceived.Message + @" Received Images";
                        })));
                        FrmClientImagesChangeNameEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                        goto case Command.ColorChanged;

                    case Command.Message:
                        Invoke(new Action((delegate
                        {
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            Color color = ColorTranslator.FromHtml(msgReceived.Color);
                            RichTextClientPub.SelectedText = GenericStatic.Time() + " ";
                            int selectionStart = RichTextClientPub.SelectionStart;
                            RichTextClientPub.SelectionColor = color;
                            RichTextClientPub.SelectedText = msgReceived.ClientName + ": " + msgReceived.Message;
                            RichTextClientPub.SelectedText = Environment.NewLine;

                            _CursorPosition = RichTextClientPub.SelectionStart;
                            foreach (ClientChatHistory clientColor in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName))
                            {
                                int[] selectionArr = { selectionStart, RichTextClientPub.TextLength - selectionStart };
                                clientColor.Messages.Add(selectionArr);
                            }
                        })));
                        if (ClientConnection.ClientName == msgReceived.ClientName)
                        {
                            ++ClientStatistics.MessagesSent;
                            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessageSent);
                            break;
                        }
                        ++ClientStatistics.MessagesReceived;
                        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessageReceied);
                        break;

                    case Command.ColorChanged:
                        Invoke(new Action((delegate
                        {
                            Color newColor = ColorTranslator.FromHtml(msgReceived.Color);
                            foreach (int[] selectedText in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName).SelectMany(clientColor => clientColor.Messages))
                            {
                                RichTextClientPub.Select(selectedText[0], selectedText[1]);
                                RichTextClientPub.SelectionColor = newColor;
                            }
                        })));
                        break;

                    case Command.PrivateStarted:
                        if (TabControlClient.TabPages.Cast<TabPage>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == msgReceived.ClientName))
                        {
                            TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 3);
                            break;
                        }
                        Invoke(new Action((delegate
                        {
                            NewTabPagePrivateChatClient(msgReceived.ClientName);
                            GenericStatic.FormatItemSize(TabControlClient);
                        })));
                        break;

                    case Command.PrivateMessage:
                        TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 0);
                        if (ClientConnection.ClientName == msgReceived.Private)
                        {
                            ++ClientStatistics.MessagesPrivateReceived;
                            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateReceived);
                            break;
                        }
                        ++ClientStatistics.MessagesPrivateSent;
                        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateSent);
                        break;

                    case Command.PrivateStopped:
                        TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1);
                        break;

                    case Command.ServerMessage:
                        Invoke(new Action((delegate
                        {
                            RichTextClientPub.SelectionStart = _CursorPosition;
                            RichTextClientPub.SelectionColor = Color.Black;
                            RichTextClientPub.SelectionBackColor = Color.MediumPurple;
                            RichTextClientPub.SelectedText = GenericStatic.Time() + " " + "Server Message: " + msgReceived.Message + Environment.NewLine;
                            _CursorPosition = RichTextClientPub.SelectionStart;
                        })));
                        ++ClientStatistics.ServerMessage;
                        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ServerMessage);
                        break;

                    case Command.ImageMessage:
                        if (msgReceived.Private != null)
                        {
                            if (ClientConnection.ClientName == msgReceived.ClientName)
                            {
                                ++ClientStatistics.ImagesPrivateSent;
                                FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesPrivateSent);
                                // Let the user know that the private photo he sent went thro
                                TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 4);
                                break;
                            }
                            // Show the photo to the receiving user
                            ++ClientStatistics.ImagesPrivateReceived;
                            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesPrivateReceived);
                            _FrmClientImages.NewImage(msgReceived.ImgByte, msgReceived.ClientName + " Private");
                            TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 5);
                            if (_FrmClientImages.Visible == false)
                            {
                                if (InvokeRequired)
                                {
                                    BeginInvoke(new MethodInvoker(delegate
                                    {
                                        _FrmClientImages.Visible = true;
                                        _FrmClientImages.BringToFront();
                                    }));
                                }
                                else
                                {
                                    _FrmClientImages.Visible = true;
                                    _FrmClientImages.BringToFront();
                                }
                            }
                            break;
                        }
                        if (ClientConnection.ClientName == msgReceived.ClientName)
                        {
                            ++ClientStatistics.ImagesSent;
                            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesSent);
                            Invoke(new Action((delegate
                            {
                                RichTextClientPub.SelectionStart = _CursorPosition;
                                RichTextClientPub.SelectionColor = Color.Black;
                                RichTextClientPub.SelectionBackColor = Color.Yellow;
                                RichTextClientPub.SelectedText = GenericStatic.Time() + " " + " ImageMessage sent successfully" + Environment.NewLine;
                                _CursorPosition = RichTextClientPub.SelectionStart;
                            })));
                            break;
                        }
                        ++ClientStatistics.ImagesReceived;
                        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesReceived);
                        _FrmClientImages.NewImage(msgReceived.ImgByte, msgReceived.ClientName);
                        if (_FrmClientImages.Visible == false)
                        {
                            if (InvokeRequired)
                            {
                                BeginInvoke(new MethodInvoker(delegate
                                {
                                    _FrmClientImages.Visible = true;
                                    _FrmClientImages.BringToFront();
                                }));
                            }
                            else
                            {
                                _FrmClientImages.Visible = true;
                                _FrmClientImages.BringToFront();
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //After the message is sent/recieved end the async operation
        private static void OnSend(IAsyncResult ar)
        {
            try
            {
                ClientConnection.Socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnSend", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Public Chat Send button
        private void BtnSend_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBoxPubMsg.Text.Length <= 0)
                {
                    return;
                }

                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.Message,
                    ClientName = ClientConnection.ClientName,
                    Color = ClientConnection.Color,
                    Message = TextBoxPubMsg.Text
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                //reset the TextBoxPubMsg
                TextBoxPubMsg.Text = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> BtnSend_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //File => Reconnect
        private void ReconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClientConnection.Status)
            {
                return;
            }
            ClientConnection.Connect(ClientConnection.Address, ClientConnection.Port, ClientConnection.ClientName);
            Thread.Sleep(5);
            //FrmChat_Load(this, null);
            BtnPubSnd.Enabled = true;
        }

        //File => Disconnect
        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClientConnection.Status)
            {
                return;
            }
            foreach (TabPage tabPage in TabControlClient.TabPages)
            {
                TabPagePrivateChatReceiveClientEvent?.Invoke(tabPage.Name, "0", "0", 2);
            }
            ClientConnection.Disconnect();
            ClientStatistics.StopStatistics();
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
        private void ClickExitToolStripMenuItem(object sender, EventArgs e)
        {
            Close();
        }

        //Chat => Change Name
        private void ChangeNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ClientConnection.Status)
                {
                    return;
                }
                //call to frmChangeName
                using (ChangeName changeName = new ChangeName(ClientConnection.ClientName))
                {
                    if (changeName.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    if (ListBoxClientList.Items.Cast<object>().Any(item => changeName.NameNew == item.ToString()))
                    {
                        MessageBox.Show(@"The name " + changeName.NameNew + @"already taken", @"Chat: 5" + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    MessageStructure msgToSend = new MessageStructure
                    {
                        Command = Command.NameChange,
                        ClientName = ClientConnection.ClientName,
                        Message = changeName.NameNew
                    };
                    byte[] msgToSendByte = msgToSend.ToByte();
                    ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> ChangeNameToolStripMenuItem_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Chat => Change color
        private void ChangeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnColorPick_Click(this, null);
        }

        //Closing FrmChat
        private void FrmChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(@"Are you sure you want to exit?", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
            if (!ClientConnection.Status)
            {
                return;
            }
            ClientConnection.Status = false;
            ClientConnection.Disconnect();
            //Give some time overhead for the client finish sending the disconnect call
            //before terminating all the the related threads
            Thread.Sleep(10);
        }

        //Color Picker
        private void BtnColorPick_Click(object sender, EventArgs e)
        {
            if (!ClientConnection.Status)
            {
                return;
            }
            DialogResult pickColor = ColorPicker.ShowDialog();
            try
            {
                if (pickColor != DialogResult.OK)
                {
                    return;
                }
                string colorHex = GenericStatic.HexConverter(ColorPicker.Color);
                ClientConnection.Color = colorHex;
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.ColorChanged,
                    ClientName = ClientConnection.ClientName,
                    Color = colorHex
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> BtnColorPick_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Pick Photo
        private void BtnSendPhotoPublic_Click(object sender, EventArgs e)
        {
            if (!ClientConnection.Status)
            {
                return;
            }
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    ClientName = ClientConnection.ClientName,
                    Command = Command.ImageMessage
                };
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = @"Open ImageMessage";
                    openFileDialog.Filter = @"Images|*.png;*.bmp;*.jpg;*.gif*";
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    long fileSize = new FileInfo(openFileDialog.FileName).Length;
                    if (fileSize > 2097152)
                    {
                        MessageBox.Show(@"This file is too big, you can send files no larger than 3MB", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    msgToSend.ImgByte = File.ReadAllBytes(openFileDialog.FileName);
                    byte[] msgToSendByte = msgToSend.ToByte();
                    ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> BtnColorPick_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Automaticlaly scrolldown rchTxtPubChat
        private void RichTextChatBoxText_Changed(object sender, EventArgs e)
        {
            RichTextClientPub.SelectionStart = RichTextClientPub.Text.Length;
            RichTextClientPub.ScrollToCaret();
        }

        //List double click to start a new private chat
        private void ListBoxClientList_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (ListBoxClientList.SelectedItem.ToString() == ClientConnection.ClientName)
                {
                    MessageBox.Show(@"You can't start a private chat with yourself", @"Chat: 5" + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                if (TabControlClient.TabPages.OfType<TabPagePrivateChatClient>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == ListBoxClientList.SelectedItem.ToString()))
                {
                    MessageBox.Show(@"That private chat already opned", @"Chat: 5" + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                NewTabPagePrivateChatClient(ListBoxClientList.SelectedItem.ToString());
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.PrivateStarted,
                    ClientName = ClientConnection.ClientName,
                    Private = ListBoxClientList.SelectedItem.ToString()
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);

                Invoke(new Action((delegate
                {
                    GenericStatic.FormatItemSize(TabControlClient);
                })));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> ListBoxClientList_DoubleClick", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Method to which createsa new class of TabPagePrivateChatClient and adds it to TabControlClient
        private void NewTabPagePrivateChatClient(string tabName)
        {
            if (!ClientConnection.Status)
            {
                return;
            }
            TabPagePrivateChatClient newPrivateTab = new TabPagePrivateChatClient(tabName);
            TabPagePrivateChatReceiveClientEvent += newPrivateTab.TabPageTabPagePrivateReceiveMessageClient;
            newPrivateTab.TabPagePrivateChatSendClientEvent += TabPagePrivateChatSendClient;
            newPrivateTab.TabPagePrivateChatSendImageClientEvent += TabPagePrivateChatSendImageClient;

            TabControlClient.TabPages.Add(newPrivateTab);
        }

        //Send private message method event
        private void TabPagePrivateChatSendClient(string namePrivate, string message)
        {
            MessageStructure msgToSend = new MessageStructure
            {
                Command = Command.PrivateMessage,
                ClientName = ClientConnection.ClientName,
                Private = namePrivate,
                Message = message
            };
            byte[] msgToSendByte = msgToSend.ToByte();
            ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
        }

        //Send private Photo method event
        private void TabPagePrivateChatSendImageClient(string namePrivate, string message)
        {
            if (!ClientConnection.Status)
            {
                return;
            }
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.ImageMessage,
                    Private = namePrivate,
                    ClientName = ClientConnection.ClientName
                };
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = @"Open ImageMessage";
                    openFileDialog.Filter = @"Images|*.png;*.bmp;*.jpg;*.gif*";
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    long fileSize = new FileInfo(openFileDialog.FileName).Length;
                    if (fileSize > 2097152)
                    {
                        MessageBox.Show(@"This file is too big, you can send files no larger than 3MB", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    msgToSend.ImgByte = File.ReadAllBytes(openFileDialog.FileName);
                    byte[] msgToSendByte = msgToSend.ToByte();
                    ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> BtnColorPick_Click", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //MessageStructure msgToSend = new MessageStructure {
            //    Command = Command.ImageMessage,
            //    ClientName = ClientConnection.ClientName,
            //    Private = namePrivate,
            //};
            //byte[] msgToSendByte = msgToSend.ToByte();
            //ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
        }

        //TabControl DrawItem, used to the draw the X on each tab
        private void TabControlClient_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Draw the name of the tab
            e.Graphics.DrawString(TabControlClient.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 10, e.Bounds.Top + 7);
            for (int i = 1; i < TabControlClient.TabPages.Count; i++)
            {
                Rectangle tabRect = TabControlClient.GetTabRect(i);
                //Not active tab
                if (i != TabControlClient.SelectedIndex)
                {
                    //Rectangle r = TabControlClient.TabPages[i].Text;
                    using (Brush brush = new SolidBrush(Color.OrangeRed))
                    {
                        e.Graphics.FillRectangle(brush, tabRect.Right - 23, 6, 16, 16);
                    }
                    using (Pen pen = new Pen(Color.Black, 2))
                    {
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
                else
                {
                    //Rectangle r = TabControlClient.TabPages[i].Text;
                    //RectangleF tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                    using (Brush brush = new SolidBrush(Color.Silver))
                    {
                        e.Graphics.FillRectangle(brush, tabRect.Right - 23, 6, 16, 16);
                    }
                    using (Pen pen = new Pen(Color.Black, 2))
                    {
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
        private void TabControlClient_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 1; i < TabControlClient.TabPages.Count; i++)
            {
                Rectangle tabRect = TabControlClient.GetTabRect(i);
                //Getting the position of the "x" mark.

                //Rectangle tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                Rectangle closeXButtonArea = new Rectangle(tabRect.Right - 23, 6, 16, 16);
                //Rectangle closeButton = new Rectangle(tabRect.Right - 13, tabRect.Top + 6, 9, 7);
                if (!closeXButtonArea.Contains(e.Location))
                {
                    continue;
                }
                if (MessageBox.Show(@"Would you like to Close this Tab?", @"Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    continue;
                }
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.PrivateStopped,
                    ClientName = ClientConnection.ClientName,
                    Private = TabControlClient.TabPages[i].Name
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                TabControlClient.TabPages.RemoveAt(i);
                break;
            }
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout about = new FrmAbout();
            about.Show();
        }

        private void StaticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_FrmStatistics.Visible)
            {
                _FrmStatistics.BringToFront();
                return;
            }
            _FrmStatistics.Visible = true;
            //_FrmStatistics.Show();
        }

        private void receivedImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FrmClientImages.Visible = true;
        }
    }
}