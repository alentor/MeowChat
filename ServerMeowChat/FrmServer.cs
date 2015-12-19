using LibraryMeowChat;
using MeowChatServerLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace MeowChatServer {
    public partial class FrmServer: Form {
        private ServerEngine serverEngine = new ServerEngine();
        private readonly List <ClientMessagesPosition> _ClientMessagesesList = new List <ClientMessagesPosition>();
        private FrmServerProgressBar _FrmProgressBarDisconnect;

        private readonly byte[] _ByteMessage = new byte[2097152]; //Max byte size to be recieved and sent
        private readonly List <Client> _ClientList = new List <Client>(); //List which contains all the connected clients
        private int _CursorPositionConn;
        private int _CursorPositionPub;
        private Socket _ServerSocket; //Server socket
        private bool _IsServerRunning = true;
        private bool _IsDisconnectRunning;
        private readonly FrmServerImages _FrmServerImages = new FrmServerImages();
        private event TabPagePrivateServerDoActionHandler TabPagePrivateServerDoActionEvent;
        private event FrmServerImagesChangeNameHandler FrmServerImagesChangeNameEvent;

        public FrmServer() {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e) {
            // Controls contained in a TabPage are not created until the tab page is shown, and any data bindings in these controls are not activated until the tab page is shown.
            // https://msdn.microsoft.com/en-us/library/system.windows.forms.tabpage.aspx
            TabControlServer.TabPages[1].Show();
            TabControlServer.TabPages[0].Show();
            FrmServerImagesChangeNameEvent += _FrmServerImages.ChangeTabName;
            serverEngine.ServerEngineServerStartedEvent += ServerStarted;
            serverEngine.ServerEngineClientToAddEvent += ClientToAdd;
            serverEngine.ServerEngineSendPublicMessageEvent += PublicMessage;
            serverEngine.ServerEngineServerStopBeganEvent += ServerStopBegan;
            serverEngine.ServerEngineStopTickEvent += ServerStopTick;
            serverEngine.ServerEngineStoppedEvent += ServerStopped;
            serverEngine.ServerEngineClientColorChangedEvent += ClientColorChanged;
            serverEngine.ServerEngineClientToRemoveEvent += ClientToRemove;
            serverEngine.ServerEngineClientNameChangedEvent += ClientNameChanged;
            serverEngine.ServerEnginePrivateChatStartedEvent += PrivateChatStarted;
            serverEngine.ServerEnginePrivateChatMessageEvent += PrivateChatMessage;
            serverEngine.ServerEnginePrivateChatStoppedEvent += PrivateChatStopped;
            //Load += serverEngine.StartServer()
        }


        // Button Start
        private void BtnStartSrv_Click(object sender, EventArgs e) {
            serverEngine.StartServer(TxtBoxIpAddress.Text, TxtBoxPort.Text);
        }

        // Button stop
        private void btnStopSrv_Click(object sender, EventArgs e) {
            serverEngine.ServerStop();
        }

        // Server Started
        private void ServerStarted() {
            //The inner server messages board
            RichTextServerConn.SelectionStart = _CursorPositionConn;
            RichTextServerConn.SelectionColor = Color.Black;
            RichTextServerConn.SelectionBackColor = Color.BlueViolet;
            RichTextServerConn.SelectedText += @"Server have started " + GenericStatic.TimeDate() + Environment.NewLine;
            _CursorPositionConn = RichTextServerConn.SelectionStart;
            BtnStartSrv.Enabled = false;
            BtnStopSrv.Enabled = true;
        }

        // Server stop began
        private void ServerStopBegan(int clientsCount) {
            Thread disconnectingThread = new Thread(new ThreadStart(() =>{
                _FrmProgressBarDisconnect = new FrmServerProgressBar(clientsCount);
                if (InvokeRequired) {
                    Invoke(new MethodInvoker(delegate{
                        _FrmProgressBarDisconnect.ShowDialog(this);
                    }));
                }
                else {
                    _FrmProgressBarDisconnect.ShowDialog(this);
                }
            })) {
                IsBackground = false
            };
            disconnectingThread.Start();
        }

        // Server stop tick
        private void ServerStopTick(string currentDisconnectintClientName) {
            Invoke(new Action((delegate{
                foreach (ListViewItem item in ListViewClients.Items.Cast <ListViewItem>().Where(item => item.Name == currentDisconnectintClientName)) {
                    ListViewClients.Items.RemoveByKey(item.Name);
                    break;
                }
            })));

            Invoke(new Action((delegate{
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.Tomato;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectedText += "<<< " + currentDisconnectintClientName + " Disconnection preformed successfully  >>>" + " " + GenericStatic.Time() + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
                //GenericStatic.FormatItemSize(TabControlServer);
                //TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2);
            })));
            _FrmProgressBarDisconnect.UpdateProgressBar(currentDisconnectintClientName);
        }

        // Server stopped
        private void ServerStopped() {
            Invoke(new Action((delegate{
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.DarkRed;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectedText += "<<< Server stopped successfully  >>>" + " " + GenericStatic.Time() + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
                //GenericStatic.FormatItemSize(TabControlServer);
                //TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2);
                MessageBox.Show(this, @"Server stopped successfully");
            })));
            Invoke(new Action((delegate{
                _FrmProgressBarDisconnect.Close();
            })));
        }

        // Client to add
        private void ClientToAdd(string clientNameToAdd, IPEndPoint clientNameToAddIpEndPoint) {
            ClientMessagesPosition newClientMessagesPosition = new ClientMessagesPosition(clientNameToAdd);
            _ClientMessagesesList.Add(newClientMessagesPosition);
            Invoke(new Action((delegate{
                ListViewItem newRow = new ListViewItem(new[] {clientNameToAdd, clientNameToAddIpEndPoint.ToString(), GenericStatic.TimeDate()}) {
                    Name = clientNameToAdd
                };
                ListViewClients.Items.Add(newRow);
                //The inner server messages board
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.LightGreen;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectedText += " <<< " + clientNameToAdd + " has joined the room >>>" + Environment.NewLine;
                GenericStatic.FormatItemSize(TabControlServer);
                _CursorPositionConn = RichTextServerConn.SelectionStart;
            })));
        }

        // Client to remove
        private void ClientToRemove(string clientNameToRemove) {
            Invoke(new Action((delegate{
                ListViewClients.Items.RemoveByKey(clientNameToRemove);
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.Tomato;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectedText += "<<< " + clientNameToRemove + " has just left the chat >>> " + GenericStatic.TimeDate() + " " + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
                //GenericStatic.FormatItemSize(TabControlServer);
                //TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.Name, msgReceived.Private, msgReceived.Message, 2);
            })));
        }

        // Public Message
        private void PublicMessage(string clientName, Color clientColor, string message) {
            Invoke(new Action((delegate{
                RichTextServerPub.SelectionStart = _CursorPositionPub;
                RichTextServerPub.SelectedText = GenericStatic.Time() + " ";
                int selectionStart = RichTextServerPub.SelectionStart;
                RichTextServerPub.SelectionColor = clientColor;
                RichTextServerPub.SelectedText = clientName + @" :" + message;
                RichTextServerPub.SelectedText = Environment.NewLine;
                _CursorPositionPub = RichTextServerPub.SelectionStart;
                foreach (ClientMessagesPosition clientMessagesPosition in _ClientMessagesesList.Where(clientMessagesPosition => clientMessagesPosition.ClientName == clientName)) {
                    int[] selectionArr = {selectionStart, RichTextServerPub.TextLength - selectionStart};
                    clientMessagesPosition.Messages.Add(selectionArr);
                }
            })));
        }

        // Client Color Changed
        private void ClientColorChanged(string clientName, Color newColor) {
            foreach (ClientMessagesPosition position in _ClientMessagesesList) {
                if (position.ClientName == clientName) {
                    foreach (int[] messages in position.Messages) {
                        Invoke(new Action((delegate{
                            RichTextServerPub.Select(messages[0], messages[1]);
                            RichTextServerPub.SelectionColor = newColor;
                        })));
                    }
                }
            }
        }

        // Client Name Changed
        private void ClientNameChanged(string clientName, string newClientName) {
            // Change the ListViewItem.name in ListViewClients
            Invoke(new Action((delegate{
                for (int i = 0; i < ListViewClients.Items.Count; i++) {
                    if (ListViewClients.Items[i].Name == clientName) {
                        ListViewClients.Items[i].Text = newClientName;
                        ListViewClients.Items[i].Name = newClientName;
                        break;
                    }
                }
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectionBackColor = Color.CornflowerBlue;
                RichTextServerConn.SelectedText += @"<<< " + clientName + @" have changed his name to " + newClientName + " " + GenericStatic.TimeDate() + @" >>>" + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
            })));
        }

        // Private Chat Started
        private void PrivateChatStarted(string clientName, string clientNamePrivate) {
            if (TabControlServer.TabPages.OfType <TabPagePrivateChatServer>().Any(tabPagePrivateChatServer => tabPagePrivateChatServer.ClientName == clientName && tabPagePrivateChatServer.ClientNamePrivate == clientNamePrivate || tabPagePrivateChatServer.ClientName == clientNamePrivate && tabPagePrivateChatServer.ClientNamePrivate == clientName)) {
                TabPagePrivateServerDoActionEvent?.Invoke(clientName, clientNamePrivate, null, TabPagePrivateChatServer.TabCommand.Resumed);
                return;
            }
            Invoke(new Action((delegate{
                NewTabPagePrivateChatServer(clientName, clientNamePrivate);
            })));
            Invoke(new Action((delegate{
                GenericStatic.FormatItemSize(TabControlServer);
            })));
        }

        // Private Chat Message
        private void PrivateChatMessage(string clientName, string clientNamePrivate, string message) {
            TabPagePrivateServerDoActionEvent?.Invoke(clientName, clientNamePrivate, message, TabPagePrivateChatServer.TabCommand.Message);
        }

        // Private Chat Stopped
        private void PrivateChatStopped(string clientName, string clientNamePrivate) {
            TabPagePrivateServerDoActionEvent?.Invoke(clientName, clientName,null, TabPagePrivateChatServer.TabCommand.Closed);

        }
        //private void OnReceive(IAsyncResult ar) {
        //    if (!_IsServerRunning) {
        //        return;
        //    }
        //    try {
        //        //establised connection(client), and casting it to a socket class.
        //        //passing down the AsyncState information of the established connection
        //        Socket receivedClientSocket = (Socket) ar.AsyncState;
        //        //Transfering the array of received bytes from the established connection(client)
        //        //into an intelligent form of object MessageStructure
        //        MessageStructure msgReceived = new MessageStructure(_ByteMessage);
        //        //Constractor for new object MessageStructure which will be sent to all establihed connections(clients)
        //        MessageStructure msgToSend = new MessageStructure {
        //            Command = msgReceived.Command,
        //            ClientName = msgReceived.ClientName,
        //            Color = msgReceived.Color
        //        };
        //        //Create a new byte[] to write into it msgToSend
        //        byte[] messageBytes;
        //        switch (msgReceived.Command) {
        //            case Command.Regiter:
        //                msgToSend.Message = "Register";
        //                messageBytes = msgToSend.ToByte();
        //                receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
        //                break;

        //            case Command.AttemptLogin:
        //                foreach (Client client in _ClientList) {
        //                    if (client.Name == msgReceived.ClientName) {
        //                        msgToSend.Message = "This user name already logged in";
        //                    }
        //                }
        //                messageBytes = msgToSend.ToByte();
        //                receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
        //                break;

        //            case Command.Logout:
        //                //When the logout command received the server will remove the established connection(client) which
        //                //have sent the command. The server will find the established connection(client) by the clientname
        //                //close the connection, and remove the client form the UI clientList
        //                foreach (Client client in _ClientList.Where(client => client.Socket == receivedClientSocket)) {
        //                    Invoke(new Action((delegate{
        //                        ListViewClients.Items.RemoveByKey(client.Name);
        //                    })));
        //                    _ClientList.Remove(client);
        //                    break;
        //                }

        //                receivedClientSocket.Shutdown(SocketShutdown.Both);
        //                receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
        //                msgToSend.Message = "<<< " + msgReceived.ClientName + " has just left the chat >>>";
        //                Invoke(new Action((delegate{
        //                    RichTextServerConn.SelectionStart = _CursorPositionConn;
        //                    RichTextServerConn.SelectionBackColor = Color.Tomato;
        //                    RichTextServerConn.SelectionColor = Color.Black;
        //                    RichTextServerConn.SelectedText += msgToSend.Message + " " + GenericStatic.Time() + Environment.NewLine;
        //                    _CursorPositionConn = RichTextServerConn.SelectionStart;
        //                    GenericStatic.FormatItemSize(TabControlServer);
        //                    TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2, TabPagePrivateChatServer.TabCommand.Null);
        //                })));
        //                break;

        //            case Command.Disconnect:
        //                //When the logout command received the server will remove the established connection(client) which
        //                //have sent the command. The server will find the established connection(client) by the clientname
        //                //close the connection, and remove the client form the UI clientList
        //                foreach (Client client in _ClientList.Where(client => client.Socket == receivedClientSocket)) {
        //                    Invoke(new Action((delegate{
        //                        ListViewClients.Items.RemoveByKey(client.Name);
        //                    })));
        //                    //client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                    //_ClientList.Remove(client);
        //                    break;
        //                }

        //                foreach (Client client in _ClientList) {
        //                    receivedClientSocket.Shutdown(SocketShutdown.Both);
        //                }
        //                receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
        //                msgToSend.Message = "<<< " + msgReceived.ClientName + " Disconnection preformed successfully  >>>";
        //                Invoke(new Action((delegate{
        //                    RichTextServerConn.SelectionStart = _CursorPositionConn;
        //                    RichTextServerConn.SelectionBackColor = Color.Tomato;
        //                    RichTextServerConn.SelectionColor = Color.Black;
        //                    RichTextServerConn.SelectedText += msgToSend.Message + " " + GenericStatic.Time() + Environment.NewLine;
        //                    _CursorPositionConn = RichTextServerConn.SelectionStart;
        //                    GenericStatic.FormatItemSize(TabControlServer);
        //                    TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2, TabPagePrivateChatServer.TabCommand.Null);
        //                })));
        //                break;

        //            case Command.List:
        //                //when the List command received the server will send the names of all the
        //                //established coneections(clients) anmes back to the requesting established connection(client)
        //                msgToSend.Command = Command.List;
        //                Client lastItem = _ClientList[_ClientList.Count - 1];
        //                msgToSend.ClientName = lastItem.Name;
        //                foreach (Client client in _ClientList) {
        //                    //To keep things simple we use a marker to separate the user names
        //                    msgToSend.Message += client.Name + ",";
        //                }
        //                //Convert msgToSend to a bytearray representative, this is needed in order to send(broadcat) the message over the TCP protocol
        //                messageBytes = msgToSend.ToByte();
        //                //Send(broadcast) the name of the estalished connections(cleints) in the chat
        //                receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
        //                break;

        //            case Command.Message:
        //                //Set the message that we will send(broadcasted) to established connections(clients)
        //                msgToSend.Message = msgReceived.Message;
        //                Color color = ColorTranslator.FromHtml(msgToSend.Color);
        //                Invoke(new Action((delegate{
        //                    RichTextServerPub.SelectionStart = _CursorPositionPub;
        //                    RichTextServerPub.SelectedText = GenericStatic.Time() + " ";
        //                    int selectionStart = RichTextServerPub.SelectionStart;
        //                    RichTextServerPub.SelectionColor = color;
        //                    RichTextServerPub.SelectedText = msgToSend.ClientName + @" :" + msgToSend.Message;
        //                    RichTextServerPub.SelectedText = Environment.NewLine;
        //                    _CursorPositionPub = RichTextServerPub.SelectionStart;
        //                    foreach (Client clientColor in _ClientList.Where(clientLinq => clientLinq.Name == msgToSend.ClientName)) {
        //                        int[] selectionArr = {selectionStart, RichTextServerPub.TextLength - selectionStart};
        //                        clientColor.Messages.Add(selectionArr);
        //                    }
        //                })));
        //                break;

        //            //case Command.NameChange:
        //            //    int clientIndexNameChange = 0;
        //            //    foreach (Client client in _ClientList) {
        //            //        if (client.Name == msgReceived.Name) {
        //            //            client.Name = msgReceived.Message;
        //            //            Invoke(new Action((delegate{
        //            //                ListViewClients.Items[clientIndexNameChange].Text = msgReceived.Message;
        //            //            })));
        //            //            break;
        //            //        }
        //            //        ++clientIndexNameChange;
        //            //    }
        //            //    msgToSend.Message = msgReceived.Message;
        //            //    Invoke(new Action((delegate{
        //            //        foreach (ListViewItem listViewItem in ListViewClients.Items.Cast <ListViewItem>().Where(listViewItem => listViewItem.Name == msgReceived.Name)) {
        //            //            listViewItem.Name = msgReceived.Message;
        //            //        }
        //            //    })));
        //            //    Invoke(new Action((delegate{
        //            //        RichTextServerConn.SelectionStart = _CursorPositionConn;
        //            //        RichTextServerConn.SelectionColor = Color.Black;
        //            //        RichTextServerConn.SelectionBackColor = Color.CornflowerBlue;
        //            //        RichTextServerConn.SelectedText += @"<<< " + msgToSend.Name + @" have changed nickname to " + msgToSend.Message + " " + GenericStatic.Time() + @" >>>" + Environment.NewLine;
        //            //        _CursorPositionConn = RichTextServerConn.SelectionStart;
        //            //        foreach (TabPagePrivateChatServer tabPage in TabControlServer.TabPages.OfType <TabPagePrivateChatServer>()) {
        //            //            if (tabPage.TabName0 == msgReceived.Name) {
        //            //                tabPage.TabName0 = msgReceived.Message;
        //            //                tabPage.Text = msgReceived.Message + @" - " + tabPage.TabName1;
        //            //                TabControlServer.Invalidate();
        //            //            }
        //            //            if (tabPage.TabName1 == msgReceived.Name) {
        //            //                tabPage.TabName1 = msgReceived.Message;
        //            //                tabPage.Text = tabPage.TabName0 + @" - " + msgReceived.Message;
        //            //                TabControlServer.Invalidate();
        //            //            }
        //            //        }
        //            //        GenericStatic.FormatItemSize(TabControlServer);
        //            //    })));
        //            //    Invoke(new Action((delegate{
        //            //        _FrmServerImages.Text = msgReceived.Message + @" Received Images";
        //            //    })));
        //            //    FrmServerImagesChangeNameEvent?.Invoke(msgReceived.Name, msgReceived.Message);
        //            //    goto case Command.ClientColorChanged;

        //            //case Command.ClientColorChanged:
        //            //    Color newColor = ColorTranslator.FromHtml(msgToSend.Color);
        //            //    foreach (Client client in _ClientList.Where(client => client.Name == msgReceived.Name)) {
        //            //        client.Color = msgReceived.Color;
        //            //        foreach (int[] selectedText in client.Messages) {
        //            //            Invoke(new Action((delegate{
        //            //                RichTextServerPub.Select(selectedText[0], selectedText[1]);
        //            //                RichTextServerPub.SelectionColor = newColor;
        //            //            })));
        //            //        }
        //            //    }
        //            //    msgToSend.Message = msgReceived.Message;
        //            //    break;

        //            case Command.PrivateStarted:
        //                if (TabControlServer.TabPages.OfType <TabPagePrivateChatServer>().Any(tabPagePrivateChatServer => tabPagePrivateChatServer.TabName0 == msgReceived.ClientName && tabPagePrivateChatServer.TabName1 == msgReceived.Private || tabPagePrivateChatServer.TabName0 == msgReceived.Private && tabPagePrivateChatServer.TabName1 == msgReceived.ClientName)) {
        //                    foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
        //                        msgToSend.Private = msgReceived.Private;
        //                        messageBytes = msgToSend.ToByte();
        //                        client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                    }
        //                    TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 3, TabPagePrivateChatServer.TabCommand.Null);
        //                    break;
        //                }
        //                Invoke(new Action((delegate{
        //                    NewTabPagePrivateChatServer(msgReceived.ClientName, msgReceived.Private);
        //                })));
        //                foreach (Client client in _ClientList) {
        //                    if (client.Name == msgReceived.Private) {
        //                        msgToSend.Private = msgReceived.Private;
        //                        messageBytes = msgToSend.ToByte();
        //                        client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                    }
        //                }
        //                Invoke(new Action((delegate{
        //                    GenericStatic.FormatItemSize(TabControlServer);
        //                })));
        //                break;

        //            case Command.PrivateMessage:
        //                foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
        //                    msgToSend.Private = msgReceived.Private;
        //                    msgToSend.Message = msgReceived.Message;
        //                    messageBytes = msgToSend.ToByte();
        //                    client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                    receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
        //                    break;
        //                }
        //                TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 0, TabPagePrivateChatServer.TabCommand.Null);
        //                break;

        //            case Command.PrivateStopped:
        //                foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
        //                    msgToSend.Private = msgReceived.Private;
        //                    messageBytes = msgToSend.ToByte();
        //                    client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                    receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
        //                }
        //                TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1, TabPagePrivateChatServer.TabCommand.Null);
        //                break;

        //            case Command.Image:
        //                if (msgReceived.Private != null) {
        //                    _FrmServerImages.NewImage(msgReceived.ImgByte, msgReceived.ClientName + " Private " + msgReceived.Private);
        //                    Thread threadSendImagePrivate = new Thread(new ThreadStart(() =>{
        //                        foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
        //                            msgToSend.Private = msgReceived.Private;
        //                            msgToSend.ImgByte = msgReceived.ImgByte;
        //                            messageBytes = msgToSend.ToByte();
        //                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
        //                        }
        //                    }));
        //                    threadSendImagePrivate.Start();
        //                    Invoke(new Action((delegate{
        //                        msgToSend.ImgByte = msgReceived.ImgByte;
        //                        RichTextServerConn.SelectionStart = _CursorPositionConn;
        //                        RichTextServerConn.SelectionBackColor = Color.DarkBlue;
        //                        RichTextServerConn.SelectionColor = Color.Yellow;
        //                        RichTextServerConn.SelectedText = GenericStatic.Time() + " ";
        //                        RichTextServerConn.SelectedText = msgToSend.ClientName + @" :" + "sent a photo to " + msgReceived.Private;
        //                        RichTextServerConn.SelectedText = Environment.NewLine;
        //                        _CursorPositionConn = RichTextServerConn.SelectionStart;
        //                    })));
        //                    break;
        //                }
        //                _FrmServerImages.NewImage(msgReceived.ImgByte, msgReceived.ClientName);
        //                msgToSend.ImgByte = msgReceived.ImgByte;
        //                messageBytes = msgToSend.ToByte();
        //                Thread threadSendImageToAll = new Thread(new ThreadStart(() =>{
        //                    foreach (Client client in _ClientList) {
        //                        client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                    }
        //                }));
        //                threadSendImageToAll.Start();
        //                Invoke(new Action((delegate{
        //                    msgToSend.ImgByte = msgReceived.ImgByte;
        //                    RichTextServerConn.SelectionStart = _CursorPositionConn;
        //                    RichTextServerConn.SelectionBackColor = Color.DarkBlue;
        //                    RichTextServerConn.SelectionColor = Color.Yellow;
        //                    RichTextServerConn.SelectedText = GenericStatic.Time() + " ";
        //                    RichTextServerConn.SelectedText = msgToSend.ClientName + @" :" + "sent a photo";
        //                    RichTextServerConn.SelectedText = Environment.NewLine;
        //                    _CursorPositionConn = RichTextServerConn.SelectionStart;
        //                })));
        //                break;
        //        }
        //        //Send message to clients
        //        if (msgToSend.Command != Command.List && msgToSend.Command != Command.PrivateStarted && msgToSend.Command != Command.PrivateMessage && msgToSend.Command != Command.PrivateStopped && msgToSend.Command != Command.Disconnect && msgToSend.Command != Command.Image) {
        //            //Convert msgToSend to a bytearray representative, this needed to send(broadcat) the message over the TCP protocol
        //            messageBytes = msgToSend.ToByte();
        //            foreach (Client client in _ClientList) {
        //                if (client.Socket != receivedClientSocket || msgToSend.Command != Command.Login) {
        //                    //Send(broadcast) the message to established connections(clients)
        //                    client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
        //                }
        //            }
        //        }
        //        //Continue listneing to receivedClientSocket established connection(client)
        //        if (msgReceived.Command != Command.Logout && msgReceived.Command != Command.Disconnect && msgReceived.Command != Command.AttemptLogin && msgReceived.Command != Command.Regiter) {
        //            receivedClientSocket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, receivedClientSocket);
        //        }
        //    }
        //    catch (Exception ex) {
        //        MessageBox.Show(ex.Message + @" -> OnReceive", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //After the message is sent/recieved end the async operation
        private void OnSend(IAsyncResult ar) {
            try {
                //passing down AsyncState
                Socket client = (Socket) ar.AsyncState;
                //let the client know message send/recieve have ended
                client.EndSend(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" => OnSend", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnDisonnect(IAsyncResult ar) {
            try {
                Socket socket = (Socket) ar.AsyncState;
                socket.EndDisconnect(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnDisonnect", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Automaticlaly scrolldown richTxtChatBox
        private void RichTextChatBox_TextChanged(object sender, EventArgs e) {
            RichTextServerConn.SelectionStart = RichTextServerConn.Text.Length;
            RichTextServerConn.ScrollToCaret();
        }

        private static string GetLocalIpAddress() {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    return ip.ToString();
                }
            }
            return "Local IP Address Not Found!";
        }

        //TabControl DrawItem, used to the draw the X on each tab
        private void TabControlServer_DrawItem(object sender, DrawItemEventArgs e) {
            //Draw the name of the tab
            e.Graphics.DrawString(TabControlServer.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 10, e.Bounds.Top + 7);
            for (int i = 2; i < TabControlServer.TabPages.Count; i++) {
                Rectangle tabRect = TabControlServer.GetTabRect(i);
                //Not active tab
                if (i != TabControlServer.SelectedIndex) {
                    //Rectangle r = TabControlServer.TabPages[i].Text;
                    using (Brush brush = new SolidBrush(Color.OrangeRed)) {
                        e.Graphics.FillRectangle(brush, tabRect.Right - 23, 6, 16, 16);
                    }
                    using (Pen pen = new Pen(Color.Black, 2)) {
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
                    //Rectangle r = TabControlServer.TabPages[i].Text;
                    //RectangleF tabXarea = new Rectangle(tabRect.Right - TabControlServer.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                    using (Brush brush = new SolidBrush(Color.Silver)) {
                        e.Graphics.FillRectangle(brush, tabRect.Right - 23, 6, 16, 16);
                    }
                    using (Pen pen = new Pen(Color.Black, 2)) {
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
        private void TabControlServert_MouseClick(object sender, MouseEventArgs e) {
            for (int i = 2; i < TabControlServer.TabPages.Count; i++) {
                Rectangle tabRect = TabControlServer.GetTabRect(i);
                //Getting the position of the "x" mark.
                //Rectangle tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                Rectangle closeXButtonArea = new Rectangle(tabRect.Right - 23, 6, 16, 16);
                //Rectangle closeButton = new Rectangle(tabRect.Right - 13, tabRect.Top + 6, 9, 7);
                if (closeXButtonArea.Contains(e.Location)) {
                    if (MessageBox.Show(@"Would you like to Close this Tab?", @"Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                        TabControlServer.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //Method to which createsa new class of TabPagePrivateChatServer and adds it to TabControlServer
        private void NewTabPagePrivateChatServer(string tabName0, string tabName1) {
            TabPagePrivateChatServer newPrivateTab = new TabPagePrivateChatServer(tabName0, tabName1);
            TabControlServer.TabPages.Add(newPrivateTab);
            TabPagePrivateServerDoActionEvent += newPrivateTab.TabPagePrivateReceiveMessageServerDoAction;
        }

        //Button send
        private void BtnServerSnd_Click(object sender, EventArgs e) {
            MessageStructure msgToSend = new MessageStructure {
                Command = Command.ServerMessage,
                Message = TxtBxServer.Text
            };
            _CursorPositionPub = RichTextServerPub.SelectionStart;
            RichTextServerPub.SelectionColor = Color.Black;
            RichTextServerPub.SelectionBackColor = Color.MediumPurple;
            RichTextServerPub.SelectedText = TxtBxServer.Text + Environment.NewLine;
            RichTextServerPub.SelectionStart = _CursorPositionPub;
            byte[] msgToSendByte = msgToSend.ToByte();
            foreach (Client client in _ClientList) {
                client.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.Socket);
            }
            TxtBxServer.Text = "";
        }

        private void button1_Click(object sender, EventArgs e) {
            _FrmServerImages.Visible = true;
        }

        private void FrmServer_FormClosing(object sender, FormClosingEventArgs e) {
            btnStopSrv_Click(this, null);
        }
    }
}