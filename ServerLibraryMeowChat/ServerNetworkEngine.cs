using LibraryMeowChat;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeowChatServerLibrary
{
    /// <summary>
    /// Server Networ Engine is responsible for all the clients network communication
    /// </summary>
    public static class ServerNetworkEngine
    {
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
        private static readonly List<Client> sr_clientList = new List<Client>();

        // Max byte size to be recieved/sent
        private static readonly byte[] sr_byteMessage = new byte[2097152];

        // Server socket is the socket from which ServerNetworkEngine is Communicating
        private static Socket s_serverSocket;

        // Significance the current state of the ServerNetworkEngine. True => Running Flase => not running.
        public static bool Status;

        // an int to count the
        private static int s_disconnectCout;

        // img and mem stream are defined at class level because the stream that the image was
        // created on has to be open/alive at the time the image is saved.
        private static MemoryStream s_ms;

        private static Image s_img;

        //public ServerNetworkEngine(FrmServer frmServer) {
        //}
        public static void StartServer(string ipAddressString, string portString)
        {
            try
            {
                sr_clientList.Clear();
                s_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ipAddress = IPAddress.Parse(ipAddressString);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, int.Parse(portString));
                // Bind the socket to local endPoint
                s_serverSocket.Bind(ipEndPoint);
                // Start listening for incoming connection, que up to 100 connections
                s_serverSocket.Listen(100);
                // Start acceppting incoming connection, on a succefull accept call to OnAccept method
                s_serverSocket.BeginAccept((OnAccept), null);
                Status = true;
                ServerNetworkEngineEngineServerStartedEvent?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> StartServer", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ServerStop()
        {
            try
            {
                if (sr_clientList.Count == 0)
                {
                    ServerNetworkEngineServerStoppedEvent?.Invoke();
                    Status = false;
                    s_serverSocket.Close();
                    return;
                }
                Status = false;
                ServerNetworkEngineServerStopBeganEvent?.Invoke(sr_clientList.Count);
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.Disconnect
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Task.Factory.StartNew(() =>
                {
                    foreach (Client client in sr_clientList)
                    {
                        // Added only to slow down the progress bar advance for demonstration purposes
                        Thread.Sleep(150);
                        client.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.Socket);
                    }
                });
                s_serverSocket.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> btnStopSrv_Click", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnAccept(IAsyncResult ar)
        {
            if (!Status)
            {
                return;
            }
            try
            {
                // Create a connection(client) based on the accepted connection
                Socket clienSocket = s_serverSocket.EndAccept(ar);
                // Start accepting again
                s_serverSocket.BeginAccept((OnAccept), null);
                // Start receiving(listening for) information on the accepted socket, once information is receive go to OnReceive client
                clienSocket.BeginReceive(sr_byteMessage, 0, sr_byteMessage.Length, SocketFlags.None, (OnReceive), clienSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnAccept", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnReceive(IAsyncResult ar)
        {
            //if (!s_serverEngineStatus) {
            //    return;
            //}
            try
            {
                // Casting the AsyncState to a socket class
                Socket receivedClientSocket = (Socket)ar.AsyncState;
                receivedClientSocket.EndReceive(ar);
                // Translating the array of received bytes to  an intelligent class MessageStructure
                MessageStructure msgReceived = new MessageStructure(sr_byteMessage);
                // Constract the initial details of new object MessageStructure which will be sent out
                MessageStructure msgToSend = new MessageStructure
                {
                    UserName = msgReceived.UserName,
                    Command = msgReceived.Command,
                    ClientName = msgReceived.ClientName,
                    Color = msgReceived.Color
                };
                // Create a new byte[] class which will filled in the following case switch statment
                byte[] messageBytes;
                switch (msgReceived.Command)
                {
                    case Command.Register:
                        msgToSend.Message = ServerDataEngine.Register(msgReceived.UserName, "Haven't logged in yet", "null", "Offline");
                        messageBytes = msgToSend.ToByte();
                        receivedClientSocket.Send(messageBytes, 0, messageBytes.Length, SocketFlags.None);
                        return;

                    case Command.AttemptLogin:
                        if (sr_clientList.Any(client => client.Name == msgReceived.ClientName))
                        {
                            msgToSend.Message = "This Name already in use";
                            messageBytes = msgToSend.ToByte();
                            receivedClientSocket.Send(messageBytes, 0, messageBytes.Length, SocketFlags.None);
                            receivedClientSocket.Close();
                            return;
                        }
                        if (ServerDataEngine.CheckIfRegistered(msgReceived.UserName) == 0)
                        {
                            msgToSend.Message = "No user found which matches this User name";
                            messageBytes = msgToSend.ToByte();
                            receivedClientSocket.Send(messageBytes, 0, messageBytes.Length, SocketFlags.None);
                            receivedClientSocket.Close();
                            return;
                        }
                        if (ServerDataEngine.CheckStatus(msgReceived.UserName) == "Online")
                        {
                            msgToSend.Message = "This User Name already logged in";
                            messageBytes = msgToSend.ToByte();
                            receivedClientSocket.Send(messageBytes, 0, messageBytes.Length, SocketFlags.None);
                            receivedClientSocket.Close();
                            return;
                        }

                        msgToSend.Command = Command.Login;
                        msgReceived.Command = Command.Login;
                        //messageBytes = msgToSend.ToByte();
                        //receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        goto case Command.Login;

                    case Command.Login:
                        // When the Login Command is received the ServerNetworkEngine will
                        // add that established connection (Socket) along
                        // with the provoided information to distinguish it (Name) to sr_clientList
                        // as a Client and sent the command Login to ohter clients to handle
                        // it on their end excluding the sending client
                        // and set the status of the new logged in client to online
                        Client newClient = new Client
                        {
                            UserName = msgReceived.UserName,
                            Name = msgReceived.ClientName,
                            Socket = receivedClientSocket,
                            Color = ColorTranslator.FromHtml(msgReceived.Color),
                            IpEndPoint = receivedClientSocket.RemoteEndPoint as IPEndPoint
                        };
                        // Adding the current handled established connection(client) to the connected _clientList
                        sr_clientList.Add(newClient);
                        // Setting the message to broadcast to all other clients
                        msgToSend.Message = "<<< " + newClient.Name + " has joined the room >>>";
                        ServerDataEngine.UpdateStatus(msgReceived.UserName, msgReceived.ClientName, "Online");
                        ServerDataEngine.UpdateDate(msgReceived.UserName, Time.NowTimeDate());
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
                        foreach (Client client in sr_clientList.Where(client => client.Socket == receivedClientSocket))
                        {
                            sr_clientList.Remove(client);
                            ServerDataEngine.UpdateStatus(client.UserName, msgReceived.ClientName, "Offline");
                            break;
                        }
                        ServerNetworkEngineClientToRemoveEvent?.Invoke(msgReceived.ClientName);
                        break;

                    case Command.Disconnect:
                        receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
                        ServerNetworkEngineServerStopTickEvent?.Invoke(msgReceived.ClientName);
                        ServerDataEngine.UpdateStatus(msgReceived.UserName, msgReceived.ClientName, "Offline");
                        ++s_disconnectCout;
                        if (sr_clientList.Count == s_disconnectCout)
                        {
                            ServerNetworkEngineServerStoppedEvent?.Invoke();
                            ServerDataEngine.ResetStatus();
                        }
                        break;

                    case Command.List:
                        // when the List command received serverEngine will send the names of all the
                        // clients(established coneections) back to the requesting (client) (established connection)
                        msgToSend.Command = Command.List;
                        Client lastItem = sr_clientList[sr_clientList.Count - 1];
                        //msgToSend.ClientName = lastItem.Name;
                        foreach (Client client in sr_clientList)
                        {
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
                        ServerDataEngine.AddMessage(msgReceived.UserName, msgReceived.Message, Time.NowTimeDate(), "false", "null");
                        msgToSend.Message = msgReceived.Message;
                        ServerNetworkEngineSendPublicMessageEvent?.Invoke(msgToSend.ClientName, ColorTranslator.FromHtml(msgReceived.Color), msgToSend.Message);
                        break;

                    case Command.NameChange:
                        foreach (Client client in sr_clientList.Where(client => client.Name == msgReceived.ClientName))
                        {
                            client.Name = msgReceived.Message;
                            break;
                        }
                        msgToSend.Message = msgReceived.Message;
                        ServerNetworkEngineClientNameChangedEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                        ServerDataEngine.UpdateLoggedLastLogged(msgReceived.ClientName, msgReceived.Message);
                        goto case Command.ColorChanged;

                    case Command.ColorChanged:
                        Color newColor = ColorTranslator.FromHtml(msgToSend.Color);
                        foreach (Client client in sr_clientList.Where(client => client.Name == msgReceived.ClientName))
                        {
                            client.Color = newColor;
                            break;
                        }
                        msgToSend.Message = msgReceived.Message;
                        ServerNetworkEngineClientColorChangedEvent?.Invoke(msgReceived.ClientName, newColor);
                        break;

                    case Command.PrivateStart:
                        foreach (Client client in sr_clientList.Where(client => client.Name == msgReceived.Private))
                        {
                            msgToSend.Private = msgReceived.Private;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            ServerNetworkEnginePrivateChatStartedEvent?.Invoke(msgReceived.ClientName, msgReceived.Private);
                            break;
                        }
                        break;

                    case Command.PrivateMessage:
                        foreach (Client client in sr_clientList.Where(clientLinq => clientLinq.Name == msgReceived.Private))
                        {
                            msgToSend.Private = msgReceived.Private;
                            msgToSend.Message = msgReceived.Message;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                            ServerNetworkEnginePrivateChatMessageEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message);
                            ServerDataEngine.AddMessage(msgReceived.UserName, msgReceived.Message, Time.NowTimeDate(), client.UserName, "null");
                            break;
                        }
                        break;

                    case Command.PrivateStopped:
                        foreach (Client client in sr_clientList.Where(clientLinq => clientLinq.Name == msgReceived.Private))
                        {
                            msgToSend.Private = msgReceived.Private;
                            messageBytes = msgToSend.ToByte();
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        }
                        ServerNetworkEnginePrivateChatStoppedEvent?.Invoke(msgReceived.ClientName, msgReceived.Private);
                        break;

                    case Command.ImageMessage:
                        s_ms = new MemoryStream(msgReceived.ImgByte);
                        s_img = Image.FromStream(s_ms);
                        if (!Directory.Exists(@"Images\" + msgReceived.UserName))
                        {
                            Directory.CreateDirectory(@"Images\" + msgReceived.UserName);
                        }
                        if (msgReceived.Private != null)
                        {
                            ServerNetworkEngineImageMessageEvent?.Invoke(s_img, msgReceived.ClientName, msgReceived.Private);
                            Task.Factory.StartNew(() =>
                            {
                                foreach (Client client in sr_clientList.Where(clientLinq => clientLinq.Name == msgReceived.Private))
                                {
                                    msgToSend.Private = msgReceived.Private;
                                    msgToSend.ImgByte = msgReceived.ImgByte;
                                    messageBytes = msgToSend.ToByte();
                                    client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                                    receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                                    ServerDataEngine.AddMessage(msgReceived.UserName, "null", Time.NowTimeDate(), client.UserName, @"Images\" + msgReceived.UserName + @"\" + Time.SaveTimeDate() + @"-" + client.UserName);
                                    s_img.Save(@"Images\" + msgReceived.Private + @"\" + Time.SaveTimeDate() + "-" + client.UserName + @".png");
                                    //s_img.Save(@"Images\" + msgReceived.UserName + @"\" + Time.SaveTimeDate() + @".png");
                                    break;
                                }
                            });
                            break;
                        }
                        ServerNetworkEngineImageMessageEvent?.Invoke(s_img, msgReceived.ClientName, msgReceived.Private);
                        msgToSend.ImgByte = msgReceived.ImgByte;
                        messageBytes = msgToSend.ToByte();
                        Task.Factory.StartNew(() =>
                        {
                            foreach (Client client in sr_clientList)
                            {
                                client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                                ServerDataEngine.AddMessage(msgReceived.UserName, "null", Time.NowTimeDate(), "false", @"Images\" + msgReceived.UserName + @"\" + Time.SaveTimeDate());
                                s_img.Save(@"Images\" + msgReceived.UserName + @"\" + Time.SaveTimeDate() + @".png");
                            }
                        });
                        break;
                }

                // Send(broadcast) the message to clients (established connections)
                Task.Factory.StartNew(() =>
                {
                    if (msgToSend.Command != Command.List && msgToSend.Command != Command.PrivateStart && msgToSend.Command != Command.PrivateMessage && msgToSend.Command != Command.PrivateStopped && msgToSend.Command != Command.Disconnect && msgToSend.Command != Command.ImageMessage)
                    {
                        messageBytes = msgToSend.ToByte();
                        foreach (Client client in sr_clientList)
                        {
                            //if (client.Socket != receivedClientSocket || msgToSend.Command != Command.Login) {
                            client.Socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.Socket);
                            //}
                        }
                    }
                });
                // Continue listneing to receivedClientSocket established connection(client)
                if (msgReceived.Command != Command.Logout && msgReceived.Command != Command.Disconnect && msgReceived.Command != Command.AttemptLogin && msgReceived.Command != Command.Register)
                {
                    receivedClientSocket.BeginReceive(sr_byteMessage, 0, sr_byteMessage.Length, SocketFlags.None, OnReceive, receivedClientSocket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" => OnSend", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnDisonnect(IAsyncResult ar)
        {
            try
            {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndDisconnect(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnDisonnect", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Server Message
        public static void ServerMessage(string message)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.ServerMessage,
                    Message = message
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                foreach (Client client in sr_clientList)
                {
                    client.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.Socket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> ServerMessage", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}