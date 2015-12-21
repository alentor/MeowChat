using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryMeowChat;


namespace MeowChatServerLibrary {
    /// <summary>
    /// Server Networ Engine is responsible for all the clients network communication
    /// </summary>
    public static class ServerNetworkEngine {
        public static event ServerNetworkEngineEngineServerStartedHandler ServerNetworkEngineEngineServerStartedEvent;
        public static event ServerNetworkEngineEngineClientToAddHandler ServerNetworkEngineEngineClientToAddEvent;
        public static event ServerNetworkEngineClientToRemoveHandler ServerNetworkEngineClientToRemoveEvent;
        public static event ServerNetworkEngineSendPublicMessageHandler ServerNetworkEngineSendPublicMessageEvent;
        public static event ServerNetworkEngineServerStopBeganHandler ServerNetworkEngineServerStopBeganEvent;
        public static event ServerNetworkEngineServerStopTickHandler ServerNetworkEngineServerStopTickEvent;
        public static event ServerNetworkEngineServerStoppedHandler ServerNetworkEngineServerStoppedEvent;
        public static event ServerNetworkEngineClientColorChangedHandler ServerNetworkEngineClientColorChangedEvent;
        public static event ServerNetworkEngineClientNameChangedHandler ServerNetworkEngineClientNameChangedEvent;
        public static event ServerNetworkEnginePrivateChatStartedHandler ServerNetworkEnginePrivateChatStartedEvent;
        public static event ServerNetworkEnginePrivateChatMessageHandler ServerNetworkEnginePrivateChatMessageEvent;
        public static event ServerNetworkEnginePrivateChatStoppedHandler ServerNetworkEnginePrivateChatStoppedEvent;
        public static event ServerNetworkEngineImageMessageHandler ServerNetworkEngineImageMessageEvent;
        // List which contains all the connected Clients
        private static readonly List <Client> _ClientList = new List <Client>();
        // Max byte size to be recieved/sent
        private static readonly byte[] _ByteMessage = new byte[2097152];
        // Server socket is the socket from which ServerNetworkEngine is Communicating
        private static Socket _ServerSocket;
        // Significance the current state of the ServerNetworkEngine. True => Running Flase => not running.
        private static bool _ServerEngineStatus;
        // an int to count the 
        private static int _DisconnectCout;

        //public ServerNetworkEngine(FrmServer frmServer) {

        //}
        public static void StartServer(string ipAddressString, string portString) {
            try {
                _ClientList.Clear();
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, int.Parse(portString));
                // Bind the socket to local endPoint
                _ServerSocket.Bind(ipEndPoint);
                // Start listening for incoming connection, que up to 100 connections
                _ServerSocket.Listen(100);
                // Start acceppting incoming connection, on a succefull accept call to OnAccept method
                _ServerSocket.BeginAccept((OnAccept), null);
                _ServerEngineStatus = true;
                ServerNetworkEngineEngineServerStartedEvent?.Invoke();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> StartServer", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ServerStop() {
            try {
                if (_ClientList.Count == 0) {
                    ServerNetworkEngineServerStoppedEvent?.Invoke();
                    _ServerEngineStatus = false;
                    _ServerSocket.Close();
                    return;
                }
                _ServerEngineStatus = false;
                ServerNetworkEngineServerStopBeganEvent?.Invoke(_ClientList.Count);
                // Initialize the dialog that will contain the progress bar
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.Disconnect
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Task.Factory.StartNew(() =>{
                    foreach (Client client in _ClientList) {
                        // Added only to slow down the progress bar advance for demonstration purposes
                        Thread.Sleep(150);
                        client.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.Socket);
                    }
                });
                _ServerSocket.Close();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> btnStopSrv_Click", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnAccept(IAsyncResult ar) {
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

        private static void OnReceive(IAsyncResult ar) {
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
                        //messageBytes = msgToSend.ToByte();
                        //receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        goto case Command.Login;

                    case Command.Login:
                        // remove this code when you'll start working on database
                        foreach (Client client in _ClientList.Where(clientLinq => msgReceived.ClientName == clientLinq.Name)) {
                            Random rnd = new Random();
                            msgReceived.ClientName = client.Name + rnd.Next(1, 999999);
                            msgToSend.ClientName = msgReceived.ClientName;
                        }
                        // When the Login Command is received the ServerNetworkEngine will 
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
                        ServerNetworkEngineEngineClientToAddEvent?.Invoke(newClient.Name, newClient.IpEndPoint);
                        break;

                    case Command.Logout:
                        // When the Logout Command is received the ServerNetworkEngine will
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
                        ServerNetworkEngineClientToRemoveEvent?.Invoke(msgReceived.ClientName);
                        break;

                    case Command.Disconnect:
                        receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
                        ServerNetworkEngineServerStopTickEvent?.Invoke(msgReceived.ClientName);
                        ++_DisconnectCout;
                        if (_ClientList.Count == _DisconnectCout) {
                            ServerNetworkEngineServerStoppedEvent?.Invoke();
                        }
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
                        ServerNetworkEngineSendPublicMessageEvent?.Invoke(msgToSend.ClientName, ColorTranslator.FromHtml(msgReceived.Color), msgToSend.Message);
                        break;

                    case Command.NameChange:
                        foreach (Client client in _ClientList.Where(client => client.Name == msgReceived.ClientName)) {
                            client.Name = msgReceived.Message;
                            break;
                        }
                        msgToSend.Message = msgReceived.Message;
                        ServerNetworkEngineClientNameChangedEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                        goto case Command.ColorChanged;

                    case Command.ColorChanged:
                        Color newColor = ColorTranslator.FromHtml(msgToSend.Color);
                        foreach (Client client in _ClientList.Where(client => client.Name == msgReceived.ClientName)) {
                            client.Color = newColor;
                            break;
                        }
                        msgToSend.Message = msgReceived.Message;
                        ServerNetworkEngineClientColorChangedEvent?.Invoke(msgReceived.ClientName, newColor);
                        break;

                    case Command.PrivateStarted:
                        foreach (Client client in _ClientList.Where(client => client.Name == msgReceived.Private)) {
                            msgToSend.Private = msgReceived.Private;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            ServerNetworkEnginePrivateChatStartedEvent?.Invoke(msgReceived.ClientName, msgReceived.Private);
                            break;
                        }
                        break;

                    case Command.PrivateMessage:
                        foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
                            msgToSend.Private = msgReceived.Private;
                            msgToSend.Message = msgReceived.Message;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                            ServerNetworkEnginePrivateChatMessageEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message);
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
                        ServerNetworkEnginePrivateChatStoppedEvent?.Invoke(msgReceived.ClientName, msgReceived.Private);
                        break;

                    case Command.ImageMessage:
                        MemoryStream ms = new MemoryStream(msgReceived.ImgByte);
                        if (msgReceived.Private != null) {
                            ServerNetworkEngineImageMessageEvent?.Invoke(Image.FromStream(ms), msgReceived.ClientName, msgReceived.Private);
                            Task.Factory.StartNew(() =>{
                                foreach (Client client in _ClientList.Where(clientLinq => clientLinq.Name == msgReceived.Private)) {
                                    msgToSend.Private = msgReceived.Private;
                                    msgToSend.ImgByte = msgReceived.ImgByte;
                                    messageBytes = msgToSend.ToByte();
                                    client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                                    receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                                    break;
                                }
                            });
                            break;
                        }
                        ServerNetworkEngineImageMessageEvent?.Invoke(Image.FromStream(ms), msgReceived.ClientName, msgReceived.Private);
                        msgToSend.ImgByte = msgReceived.ImgByte;
                        messageBytes = msgToSend.ToByte();
                        Task.Factory.StartNew(() =>{
                            foreach (Client client in _ClientList) {
                                client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            }
                        });
                        break;
                }

                // Send(broadcast) the message to clients (established connections)
                Task.Factory.StartNew(() =>{
                    if (msgToSend.Command != Command.List && msgToSend.Command != Command.PrivateStarted && msgToSend.Command != Command.PrivateMessage && msgToSend.Command != Command.PrivateStopped && msgToSend.Command != Command.Disconnect && msgToSend.Command != Command.ImageMessage) {
                        messageBytes = msgToSend.ToByte();
                        foreach (Client client in _ClientList) {
                            //if (client.Socket != receivedClientSocket || msgToSend.Command != Command.Login) {
                                client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            //}
                        }
                    }
                    // Continue listneing to receivedClientSocket established connection(client)
                    if (msgReceived.Command != Command.Logout && msgReceived.Command != Command.Disconnect && msgReceived.Command != Command.AttemptLogin && msgReceived.Command != Command.Regiter) {
                        receivedClientSocket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, receivedClientSocket);
                    }
                });
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnSend(IAsyncResult ar) {
            try {
                Socket client = (Socket) ar.AsyncState;
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

        //Server Message
        public static void ServerMessage(string message) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.ServerMessage,
                    Message = message
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                foreach (Client client in _ClientList) {
                    client.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.Socket);
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ServerMessage", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}