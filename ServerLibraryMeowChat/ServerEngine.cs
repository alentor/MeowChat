﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryMeowChat;

namespace MeowChatServerLibrary {
    public class ServerEngine {
        public event ServerEngineServerStartedHandler ServerEngineServerStartedEvent;
        public event ServerEngineClientToAddHandler ServerEngineClientToAddEvent;
        public event ServerEngineClientToRemoveHandler ServerEngineClientToRemoveEvent;
        public event ServerEngineSendPublicMessageHandler ServerEngineSendPublicMessageEvent;
        public event ServerEngineServerStopBeganHandler ServerEngineServerStopBeganEvent;
        public event ServerEngineStopTickHandler ServerEngineStopTickEvent;
        public event ServerEngineStoppedHandler ServerEngineStoppedEvent;
        public event ServerEngineClientColorChangedHandler ServerEngineClientColorChangedEvent;
        public event ServerEngineClientNameChangedHandler ServerEngineClientNameChangedEvent;
        public event ServerEnginePrivateChatStartedHandler ServerEnginePrivateChatStartedEvent;
        public event ServerEnginePrivateChatMessageHandler ServerEnginePrivateChatMessageEvent;
        public event ServerEnginePrivateChatStoppedHandler ServerEnginePrivateChatStoppedEvent;
        // List which contains all the connected Clients
        private readonly List <Client> _ClientList = new List <Client>();
        // Max byte size to be recieved/sent
        private readonly byte[] _ByteMessage = new byte[2097152];
        // Server socket is the socket from which ServerEngine is Communicating
        private Socket _ServerSocket;
        // Significance the current state of the ServerEngine. True => Running Flase => not running.
        private bool _ServerEngineStatus;

        public void StartServer(string ipAddressString, string portString) {
            try {
                _ClientList.Clear();
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, int.Parse(portString));
                // Bind the socket to local endPoint
                _ServerSocket.Bind(ipEndPoint);
                // Start listening for incoming connection, que up to 100 connections
                _ServerSocket.Listen(100);
                // Start accppting incoming connection, on a succefull accept call to OnAccept method
                _ServerSocket.BeginAccept((OnAccept), null);
                _ServerEngineStatus = true;
                ServerEngineServerStartedEvent?.Invoke();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> StartServer", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void ServerStop() {
            try {
                _ServerEngineStatus = false;
                ServerEngineServerStopBeganEvent?.Invoke(_ClientList.Count);
                Thread.Sleep(250);
                // Initialize the dialog that will contain the progress bar
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.Disconnect
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                for (int i = 0; i < _ClientList.Count; i++) {
                    //frmProgressBarDisconnect.UpdateProgressBar(i);
                    _ClientList[i].Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, _ClientList[i].Socket);
                    // Not necessary but in place to only make it "feel" like the clients are actually being disconencted instead split second disconnect
                    //Thread.Sleep(250);
                }
                // Sets the flag to indicates the process has stoppedf
                _ServerSocket?.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> btnStopSrv_Click", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnAccept(IAsyncResult ar) {
            if (!_ServerEngineStatus) {
                return;
            }
            try {
                // Create a connection(client) based on the accepted connection
                Socket clienSocket = _ServerSocket.EndAccept(ar);
                // Start accepting again
                _ServerSocket.BeginAccept((OnAccept), null);
                // Start receiving(listening for) information on the accepted socket, once information is receive go to OnReceive client
                clienSocket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, (OnReceive), clienSocket);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnAccept", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar) {
            //if (!_ServerEngineStatus) {
            //    return;
            //}
            try {
                // Casting the AsyncState to a socket class
                Socket receivedClientSocket = (Socket) ar.AsyncState;
                // Translating the array of received bytes to  an intelligent class MessageStructure
                MessageStructure msgReceived = new MessageStructure(_ByteMessage);
                // Constract the initial details of new object MessageStructure which will be sent out
                MessageStructure msgToSend = new MessageStructure {
                    Command = msgReceived.Command,
                    ClientName = msgReceived.ClientName,
                    Color = msgReceived.Color
                };
                // Create a new byte[] class which will filled in the following case switch statment
                byte[] messageBytes;
                switch (msgReceived.Command) {
                    case Command.Regiter:
                        msgToSend.Message = "Register";
                        messageBytes = msgToSend.ToByte();
                        receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        break;

                    case Command.AttemptLogin:
                        foreach (Client client in _ClientList) {
                            if (client.Name == msgReceived.ClientName) {
                                msgToSend.Message = "This user name already logged in";
                                messageBytes = msgToSend.ToByte();
                                receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                                return;
                            }
                        }
                        msgToSend.Command = Command.Login;
                        messageBytes = msgToSend.ToByte();
                        receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        break;

                    case Command.Login:
                        // remove this code when you'll start working on database
                        foreach (Client client in _ClientList.Where(clientLinq => msgReceived.ClientName == clientLinq.Name)) {
                            Random rnd = new Random();
                            msgReceived.ClientName = client.Name + rnd.Next(1, 999999);
                            msgToSend.ClientName = msgReceived.ClientName;
                            //break;
                        }


                        // When the Login Command is received the ServerEngine will 
                        // add that established connection (Socket) along
                        // with the provoided information to distinguish it (Name) to _ClientList
                        // as a Client and sent the command Login to ohter clients to handle
                        // it on their end excluding the sending client
                        Client newClient = new Client {
                            Socket = receivedClientSocket,
                            Name = msgReceived.ClientName,
                            Color = ColorTranslator.FromHtml(msgReceived.Color),
                            IpEndPoint = receivedClientSocket.RemoteEndPoint as IPEndPoint
                        };
                        // Adding the current handled established connection(client) to the connected _clientList
                        _ClientList.Add(newClient);
                        // Setting the message to broadcast to all other clients
                        msgToSend.Message = "<<< " + newClient.Name + " has joined the room >>>";
                        ServerEngineClientToAddEvent?.Invoke(newClient.Name, newClient.IpEndPoint);
                        break;

                    case Command.Logout:
                        // When the Logout Command is received the ServerEngine will
                        // remove the the client from _clientList a long with all of
                        // it's information, socket/clientName etc..
                        // server engine will also stop listening to the removed socket
                        // and broadcast the message to all clients excluding the removed client
                        receivedClientSocket.Shutdown(SocketShutdown.Both);
                        receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
                        // Setting the message to broadcast to all other clients
                        msgToSend.Message = "<<< " + msgReceived.ClientName + " has just left the chat >>>";
                        // Removing client (established connection) _clientList
                        foreach (Client client in _ClientList.Where(client => client.Socket == receivedClientSocket)) {
                            _ClientList.Remove(client);
                            break;
                        }
                        ServerEngineClientToRemoveEvent?.Invoke(msgReceived.ClientName);
                        break;

                    case Command.Disconnect:
                        receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
                        int clientIndex = _ClientList.FindIndex(client => client.Name == msgReceived.ClientName);
                        if (clientIndex + 1 == _ClientList.Count) {
                            ServerEngineStoppedEvent?.Invoke();
                        }
                        ServerEngineStopTickEvent?.Invoke(msgReceived.ClientName);
                        break;

                    case Command.List:
                        // when the List command received serverEngine will send the names of all the
                        // clients(established coneections) back to the requesting (client) (established connection)
                        msgToSend.Command = Command.List;
                        Client lastItem = _ClientList[_ClientList.Count - 1];
                        msgToSend.ClientName = lastItem.Name;
                        foreach (Client client in _ClientList) {
                            //To keep things simple we use a marker to separate the user names
                            msgToSend.Message += client.Name + ",";
                        }
                        // Convert msgToSend to a bytearray representative, this is needed in order to send(broadcat) the message over the TCP protocol
                        messageBytes = msgToSend.ToByte();
                        // Send(broadcast) the name of the estalished connections(cleints) in the chat
                        receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        break;

                    case Command.Message:
                        // Set the message which will be broadcasted to all the connected clients
                        msgToSend.Message = msgReceived.Message;
                        ServerEngineSendPublicMessageEvent?.Invoke(msgToSend.ClientName, ColorTranslator.FromHtml(msgReceived.Color), msgToSend.Message);
                        break;

                    case Command.NameChange:
                        foreach (Client client in _ClientList.Where(client => client.Name == msgReceived.ClientName)) {
                            client.Name = msgReceived.Message;
                            break;
                        }
                        msgToSend.Message = msgReceived.Message;
                        ServerEngineClientNameChangedEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                        //Invoke(new Action((delegate
                        //{
                        //    foreach (TabPagePrivateChatServer tabPage in TabControlServer.TabPages.OfType<TabPagePrivateChatServer>())
                        //    {
                        //        if (tabPage.TabName0 == msgReceived.Name)
                        //        {
                        //            tabPage.TabName0 = msgReceived.Message;
                        //            tabPage.Text = msgReceived.Message + @" - " + tabPage.TabName1;
                        //            TabControlServer.Invalidate();
                        //        }
                        //        if (tabPage.TabName1 == msgReceived.Name)
                        //        {
                        //            tabPage.TabName1 = msgReceived.Message;
                        //            tabPage.Text = tabPage.TabName0 + @" - " + msgReceived.Message;
                        //            TabControlServer.Invalidate();
                        //        }
                        //    }
                        //    GenericStatic.FormatItemSize(TabControlServer);
                        //})));
                        //Invoke(new Action((delegate
                        //{
                        //    _FrmServerImages.Text = msgReceived.Message + @" Received Images";
                        //})));
                        //FrmServerImagesChangeNameEvent?.Invoke(msgReceived.Name, msgReceived.Message);
                        goto case Command.ColorChanged;

                    case Command.ColorChanged:
                        Color newColor = ColorTranslator.FromHtml(msgToSend.Color);
                        foreach (Client client in _ClientList.Where(client => client.Name == msgReceived.ClientName)) {
                            client.Color = newColor;
                            break;
                        }
                        msgToSend.Message = msgReceived.Message;
                        ServerEngineClientColorChangedEvent?.Invoke(msgReceived.ClientName, newColor);
                        break;

                    case Command.PrivateStarted:
                        foreach (Client client in _ClientList) {
                            if (client.Name == msgReceived.Private) {
                                msgToSend.Private = msgReceived.Private;
                                messageBytes = msgToSend.ToByte();
                                client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                                ServerEnginePrivateChatStartedEvent?.Invoke(msgReceived.ClientName, msgReceived.Private);
                                break;
                            }
                        }
                        //if (TabControlServer.TabPages.OfType <TabPagePrivateChatServer>().Any(tabPagePrivateChatServer => tabPagePrivateChatServer.TabName0 == msgReceived.Name && tabPagePrivateChatServer.TabName1 == msgReceived.Private || tabPagePrivateChatServer.TabName0 == msgReceived.Private && tabPagePrivateChatServer.TabName1 == msgReceived.Name)) {
                        //    foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
                        //        msgToSend.Private = msgReceived.Private;
                        //        messageBytes = msgToSend.ToByte();
                        //        client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                        //    }
                        //    TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.Name, msgReceived.Private, msgReceived.Message, 3);
                        //    break;
                        //}
                        //Invoke(new Action((delegate{
                        //    NewTabPagePrivateChatServer(msgReceived.Name, msgReceived.Private);
                        //})));
                        //foreach (Client client in _ClientList) {
                        //    if (client.Name == msgReceived.Private) {
                        //        msgToSend.Private = msgReceived.Private;
                        //        messageBytes = msgToSend.ToByte();
                        //        client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                        //    }
                        //}
                        //Invoke(new Action((delegate{
                        //    GenericStatic.FormatItemSize(TabControlServer);
                        //})));
                        break;

                    case Command.PrivateMessage:
                        foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
                            msgToSend.Private = msgReceived.Private;
                            msgToSend.Message = msgReceived.Message;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                            ServerEnginePrivateChatMessageEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message);
                            break;
                        }
                        break;

                    case Command.PrivateStopped:
                        foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
                            msgToSend.Private = msgReceived.Private;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        }
                        ServerEnginePrivateChatStoppedEvent?.Invoke(msgReceived.ClientName, msgReceived.Private);
                        //TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.Name, msgReceived.Private, msgReceived.Message, 1);
                        break;

                    //case Command.Image:
                    //    if (msgReceived.Private != null)
                    //    {
                    //        _FrmServerImages.NewImage(msgReceived.ImgByte, msgReceived.Name + " Private " + msgReceived.Private);
                    //        Thread threadSendImagePrivate = new Thread(new ThreadStart(() =>
                    //        {
                    //            foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private))
                    //            {
                    //                msgToSend.Private = msgReceived.Private;
                    //                msgToSend.ImgByte = msgReceived.ImgByte;
                    //                messageBytes = msgToSend.ToByte();
                    //                client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                    //                receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                    //            }
                    //        }));
                    //        threadSendImagePrivate.Start();
                    //        Invoke(new Action((delegate
                    //        {
                    //            msgToSend.ImgByte = msgReceived.ImgByte;
                    //            RichTextServerConn.SelectionStart = _CursorPositionConn;
                    //            RichTextServerConn.SelectionBackColor = Color.DarkBlue;
                    //            RichTextServerConn.SelectionColor = Color.Yellow;
                    //            RichTextServerConn.SelectedText = GenericStatic.Time() + " ";
                    //            RichTextServerConn.SelectedText = msgToSend.Name + @" :" + "sent a photo to " + msgReceived.Private;
                    //            RichTextServerConn.SelectedText = Environment.NewLine;
                    //            _CursorPositionConn = RichTextServerConn.SelectionStart;
                    //        })));
                    //        break;
                    //    }
                    //    _FrmServerImages.NewImage(msgReceived.ImgByte, msgReceived.Name);
                    //    msgToSend.ImgByte = msgReceived.ImgByte;
                    //    messageBytes = msgToSend.ToByte();
                    //    Thread threadSendImageToAll = new Thread(new ThreadStart(() =>
                    //    {
                    //        foreach (Client client in _ClientList)
                    //        {
                    //            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                    //        }
                    //    }));
                    //    threadSendImageToAll.Start();
                    //    Invoke(new Action((delegate
                    //    {
                    //        msgToSend.ImgByte = msgReceived.ImgByte;
                    //        RichTextServerConn.SelectionStart = _CursorPositionConn;
                    //        RichTextServerConn.SelectionBackColor = Color.DarkBlue;
                    //        RichTextServerConn.SelectionColor = Color.Yellow;
                    //        RichTextServerConn.SelectedText = GenericStatic.Time() + " ";
                    //        RichTextServerConn.SelectedText = msgToSend.Name + @" :" + "sent a photo";
                    //        RichTextServerConn.SelectedText = Environment.NewLine;
                    //        _CursorPositionConn = RichTextServerConn.SelectionStart;
                    //    })));
                    //    break;
                }
                //Send message to clients
                if (msgToSend.Command != Command.List && msgToSend.Command != Command.PrivateStarted && msgToSend.Command != Command.PrivateMessage && msgToSend.Command != Command.PrivateStopped && msgToSend.Command != Command.Disconnect && msgToSend.Command != Command.Image) {
                    //Convert msgToSend to a bytearray representative, this needed to send(broadcat) the message over the TCP protocol
                    messageBytes = msgToSend.ToByte();
                    foreach (Client client in _ClientList) {
                        if (client.Socket != receivedClientSocket || msgToSend.Command != Command.Login) {
                            //Send(broadcast) the message to established connections(clients)
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                        }
                    }
                }
                //Continue listneing to receivedClientSocket established connection(client)
                if (msgReceived.Command != Command.Logout && msgReceived.Command != Command.Disconnect && msgReceived.Command != Command.AttemptLogin && msgReceived.Command != Command.Regiter) {
                    receivedClientSocket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, receivedClientSocket);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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
    }
}