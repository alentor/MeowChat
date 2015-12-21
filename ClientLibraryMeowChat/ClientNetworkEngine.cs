using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryMeowChat;

namespace MeowChatClientLibrary {
    public class ClientNetworkEngine {
        public event ClientNetworkEngineLoggedinHandler ClientNetworkEngineLoggedinEvent;
        public event ClientEngineLoginErrorHandler ClientEngineLoginErrorEvent;
        private byte[] _ByteMessage;
        public bool Status;
        public Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public IPEndPoint _IpdEndPoint;
        public int Port;


        public void AttemptConnect(string ipAdress, int port, string name, Color color) {
            try {
                Client.Name = name;
                Client.Color = color;
                Port = port;
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _IpdEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress), Port);
                Socket.BeginConnect(_IpdEndPoint, OnAttemptConnect, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> AttemptConnect", @"Chat: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnAttemptConnect(IAsyncResult ar) {
            try {
                Socket.EndConnect(ar); //notify the server the connection was established succefully
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.AttemptLogin,
                    ClientName = Client.Name,
                    Message = null
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                // Ssend the login credinails of the established connection to the server
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                _ByteMessage = new byte[2097152];
                Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnAttemptConnect", @"Chat: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar) {
            try {
                //Let the server know the message was recieved
                Socket.EndReceive(ar);
                //Convert message from bytes to messageStracure class and store it in msgReceieved
                MessageStructure msgReceived = new MessageStructure(_ByteMessage);
                //Set new bytes and start recieving again
                _ByteMessage = new byte[2097152];
                //if (msgReceived.Command == Command.Disconnect)
                //{
                //    Invoke(new Action((delegate
                //    {
                //        ClientConnection.ServerDisconnectCall();
                //        ClientStatistics.StopStatistics();
                //        BtnPubSnd.Enabled = false;
                //        ListBoxClientList.Items.Clear();
                //        RichTextClientPub.SelectionStart = _CursorPosition;
                //        RichTextClientPub.SelectionColor = Color.Black;
                //        RichTextClientPub.SelectionBackColor = Color.Tomato;
                //        RichTextClientPub.SelectedText = GenericStatic.Time() + " Disconnected from the server" + Environment.NewLine;
                //        _CursorPosition = RichTextClientPub.SelectionStart;
                //    })));
                //    return;
                //}
                Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
                //Case switch statment message stracture
                switch (msgReceived.Command) {
                    case Command.AttemptLogin:

                        break;

                    case Command.Login:
                        if (msgReceived.ClientName == Client.Name) {
                            ClientNetworkEngineLoggedinEvent?.Invoke();
                        }
                        //goto case Command.List;

                        //Invoke(new Action((delegate
                        //{
                        //    RichTextClientPub.SelectionStart = _CursorPosition;
                        //    RichTextClientPub.SelectionColor = Color.Black;
                        //    RichTextClientPub.SelectionBackColor = Color.LightGreen;
                        //    ListBoxClientList.Items.Add(msgReceived.ClientName);
                        //    RichTextClientPub.SelectedText = GenericStatic.Time() + " " + msgReceived.Message + Environment.NewLine;
                        //    if (msgReceived.ClientName != ClientConnection.ClientName)
                        //    {
                        //        _ListClientsColor.Add(new ClientChatHistory(msgReceived.ClientName));
                        //    }
                        //    _CursorPosition = RichTextClientPub.SelectionStart;
                        //})));
                        break;

                    case Command.List:
                        ClientConnection.ClientName = msgReceived.ClientName; //Set ClientConnection name
                        //Invoke(new Action((delegate
                        //{
                        //    Text = @"Chat: " + ClientConnection.ClientName; //Set window name
                        //    //_ClientsColor.Add(new ClientChatProp(ClientConnection.ClientName)); //Add this Client to the ClientChatProp list
                        //    ListBoxClientList.Items.AddRange(msgReceived.Message.Split(','));
                        //    //remove the empty selection box in list view
                        //    RichTextClientPub.SelectionColor = Color.Black;
                        //    RichTextClientPub.SelectedText = @"<<< " + ClientConnection.ClientName + @" has joined the room >>>" + Environment.NewLine;
                        //    _CursorPosition = RichTextClientPub.SelectionStart;
                        //    ListBoxClientList.Items.RemoveAt(ListBoxClientList.Items.Count - 1);
                        //    //Add all the connected clients to ClientChatProp list
                        //    foreach (object t in ListBoxClientList.Items)
                        //    {
                        //        _ListClientsColor.Add(new ClientChatHistory(t.ToString()));
                        //    }
                        //})));
                        break;

                    //case Command.Logout:
                    //    Invoke(new Action((delegate
                    //    {
                    //        ListBoxClientList.Items.Remove(msgReceived.ClientName);
                    //        for (int i = 0; i < _ListClientsColor.Count; i++)
                    //        {
                    //            if (_ListClientsColor[i].Name == msgReceived.ClientName)
                    //            {
                    //                _ListClientsColor.Remove(_ListClientsColor[i]);
                    //                TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2);
                    //            }
                    //        }
                    //        RichTextClientPub.SelectionStart = _CursorPosition;
                    //        RichTextClientPub.SelectionColor = Color.Black;
                    //        RichTextClientPub.SelectionBackColor = Color.Tomato;
                    //        RichTextClientPub.SelectedText = GenericStatic.Time() + " " + msgReceived.Message + Environment.NewLine;
                    //        _CursorPosition = RichTextClientPub.SelectionStart;
                    //    })));
                    //    break;

                    //case Command.NameChange:
                    //    Invoke(new Action((delegate
                    //    {
                    //        int index = ListBoxClientList.FindString(msgReceived.ClientName);
                    //        ListBoxClientList.Items[index] = msgReceived.Message;

                    //        foreach (ClientChatHistory clientColor in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName))
                    //        {
                    //            clientColor.Name = msgReceived.Message;
                    //        }
                    //        RichTextClientPub.SelectionStart = _CursorPosition;
                    //        RichTextClientPub.SelectionColor = Color.Black;
                    //        RichTextClientPub.SelectionBackColor = Color.CornflowerBlue;
                    //        RichTextClientPub.SelectedText = GenericStatic.Time() + " " + @"<<< " + msgReceived.ClientName + @" have changed nickname to " + msgReceived.Message + @" >>>" + Environment.NewLine;
                    //        _CursorPosition = RichTextClientPub.SelectionStart;
                    //        if (ClientConnection.ClientName == msgReceived.ClientName)
                    //        {
                    //            Text = @"Chat: " + msgReceived.Message;
                    //            ClientConnection.ClientName = msgReceived.Message;
                    //        }
                    //        foreach (TabPage tabPage in TabControlClient.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Name == msgReceived.ClientName))
                    //        {
                    //            tabPage.Name = msgReceived.Message;
                    //            tabPage.Text = msgReceived.Message;
                    //            TabControlClient.Invalidate();
                    //        }
                    //        GenericStatic.FormatItemSize(TabControlClient);
                    //    })));
                    //    Invoke(new Action((delegate
                    //    {
                    //        _FrmClientImages.Text = msgReceived.Message + @" Received Images";
                    //    })));
                    //    FrmClientImagesChangeNameEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                    //    goto case Command.ColorChanged;

                    //case Command.Message:
                    //    Invoke(new Action((delegate
                    //    {
                    //        RichTextClientPub.SelectionStart = _CursorPosition;
                    //        Color color = ColorTranslator.FromHtml(msgReceived.Color);
                    //        RichTextClientPub.SelectedText = GenericStatic.Time() + " ";
                    //        int selectionStart = RichTextClientPub.SelectionStart;
                    //        RichTextClientPub.SelectionColor = color;
                    //        RichTextClientPub.SelectedText = msgReceived.ClientName + ": " + msgReceived.Message;
                    //        RichTextClientPub.SelectedText = Environment.NewLine;

                    //        _CursorPosition = RichTextClientPub.SelectionStart;
                    //        foreach (ClientChatHistory clientColor in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName))
                    //        {
                    //            int[] selectionArr = { selectionStart, RichTextClientPub.TextLength - selectionStart };
                    //            clientColor.Messages.Add(selectionArr);
                    //        }
                    //    })));
                    //    if (ClientConnection.ClientName == msgReceived.ClientName)
                    //    {
                    //        ++ClientStatistics.MessagesSent;
                    //        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessageSent);
                    //        break;
                    //    }
                    //    ++ClientStatistics.MessagesReceived;
                    //    FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessageReceied);
                    //    break;

                    //case Command.ColorChanged:
                    //    Invoke(new Action((delegate
                    //    {
                    //        Color newColor = ColorTranslator.FromHtml(msgReceived.Color);
                    //        foreach (int[] selectedText in _ListClientsColor.Where(clientColor => clientColor.Name == msgReceived.ClientName).SelectMany(clientColor => clientColor.Messages))
                    //        {
                    //            RichTextClientPub.Select(selectedText[0], selectedText[1]);
                    //            RichTextClientPub.SelectionColor = newColor;
                    //        }
                    //    })));
                    //    break;

                    //case Command.PrivateStarted:
                    //    if (TabControlClient.TabPages.Cast<TabPage>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == msgReceived.ClientName))
                    //    {
                    //        TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 3);
                    //        break;
                    //    }
                    //    Invoke(new Action((delegate
                    //    {
                    //        NewTabPagePrivateChatClient(msgReceived.ClientName);
                    //        GenericStatic.FormatItemSize(TabControlClient);
                    //    })));
                    //    break;

                    //case Command.PrivateMessage:
                    //    TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 0);
                    //    if (ClientConnection.ClientName == msgReceived.Private)
                    //    {
                    //        ++ClientStatistics.MessagesPrivateReceived;
                    //        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateReceived);
                    //        break;
                    //    }
                    //    ++ClientStatistics.MessagesPrivateSent;
                    //    FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateSent);
                    //    break;

                    //case Command.PrivateStopped:
                    //    TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1);
                    //    break;

                    //case Command.ServerMessage:
                    //    Invoke(new Action((delegate
                    //    {
                    //        RichTextClientPub.SelectionStart = _CursorPosition;
                    //        RichTextClientPub.SelectionColor = Color.Black;
                    //        RichTextClientPub.SelectionBackColor = Color.MediumPurple;
                    //        RichTextClientPub.SelectedText = GenericStatic.Time() + " " + "Server Message: " + msgReceived.Message + Environment.NewLine;
                    //        _CursorPosition = RichTextClientPub.SelectionStart;
                    //    })));
                    //    ++ClientStatistics.ServerMessage;
                    //    FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ServerMessage);
                    //    break;

                    //case Command.ImageMessage:
                    //    if (msgReceived.Private != null)
                    //    {
                    //        if (ClientConnection.ClientName == msgReceived.ClientName)
                    //        {
                    //            ++ClientStatistics.ImagesPrivateSent;
                    //            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesPrivateSent);
                    //            // Let the user know that the private photo he sent went thro
                    //            TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 4);
                    //            break;
                    //        }
                    //        // Show the photo to the receiving user
                    //        ++ClientStatistics.ImagesPrivateReceived;
                    //        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesPrivateReceived);
                    //        _FrmClientImages.NewImage(msgReceived.ImgByte, msgReceived.ClientName + " Private");
                    //        TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 5);
                    //        if (_FrmClientImages.Visible == false)
                    //        {
                    //            if (InvokeRequired)
                    //            {
                    //                BeginInvoke(new MethodInvoker(delegate
                    //                {
                    //                    _FrmClientImages.Visible = true;
                    //                    _FrmClientImages.BringToFront();
                    //                }));
                    //            }
                    //            else
                    //            {
                    //                _FrmClientImages.Visible = true;
                    //                _FrmClientImages.BringToFront();
                    //            }
                    //        }
                    //        break;
                    //    }
                    //    if (ClientConnection.ClientName == msgReceived.ClientName)
                    //    {
                    //        ++ClientStatistics.ImagesSent;
                    //        FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesSent);
                    //        Invoke(new Action((delegate
                    //        {
                    //            RichTextClientPub.SelectionStart = _CursorPosition;
                    //            RichTextClientPub.SelectionColor = Color.Black;
                    //            RichTextClientPub.SelectionBackColor = Color.Yellow;
                    //            RichTextClientPub.SelectedText = GenericStatic.Time() + " " + " ImageMessage sent successfully" + Environment.NewLine;
                    //            _CursorPosition = RichTextClientPub.SelectionStart;
                    //        })));
                    //        break;
                    //    }
                    //    ++ClientStatistics.ImagesReceived;
                    //    FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesReceived);
                    //    _FrmClientImages.NewImage(msgReceived.ImgByte, msgReceived.ClientName);
                    //    if (_FrmClientImages.Visible == false)
                    //    {
                    //        if (InvokeRequired)
                    //        {
                    //            BeginInvoke(new MethodInvoker(delegate
                    //            {
                    //                _FrmClientImages.Visible = true;
                    //                _FrmClientImages.BringToFront();
                    //            }));
                    //        }
                    //        else
                    //        {
                    //            _FrmClientImages.Visible = true;
                    //            _FrmClientImages.BringToFront();
                    //        }
                    //    }
                        break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // After the message is sent/recieved end the async operation
        private void OnSend(IAsyncResult ar) {
            try {
                Socket.EndSend(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnSend", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void Reconnect() {
            try {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket.BeginConnect(_IpdEndPoint, OnAttemptConnect, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ListBoxClientList_DoubleClick", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}