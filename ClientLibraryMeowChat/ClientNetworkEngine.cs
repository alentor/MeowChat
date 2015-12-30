using LibraryMeowChat;
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MeowChatClientLibrary
{/// <summary>
/// class which handles all network traffic and updates the UI/Notifys the UI accordingly
/// </summary>
    public static class ClientNetworkEngine
    {
        public static event ClientNetworkEngineRegisterHandler ClientNetworkEngineRegisterMessageEvent;

        public static event ClientNetworkEngineAttemptLoginErrorHandler ClientNetworkEngineAttemptLoginErrorEvent;

        public static event ClientNetworkEngineLoggedinHandler ClientNetworkEngineLoggedinEvent;

        public static event ClientNetworkEngineLoginHandler ClientNetworkEngineLoginEvent;

        public static event ClientNetworkEngineClientsListHandler ClientNetworkEngineClientsListEvent;

        public static event ClientNetworkEngineDisconnectHandler ClientNetworkEngineDisconnectEvent;

        public static event ClientNetworkEngineLogoutHandler ClientNetworkEngineLogoutEvent;

        public static event ClientNetworkEngineMessageHandler ClientNetworkEngineMessageEvent;

        public static event ClientNetworkEngineColorChangedHandler ClientNetworkEngineColorChangedEvent;

        public static event ClientNetworkEnginePrivateChatStartHandler ClientNetworkEnginePrivateChatStartEvent;

        public static event ClientNetworkEnginePrivateMessageHandler ClientNetworkEnginePrivateMessageEvent;

        public static event ClientNetworkEnginePrivateStoppedHandler ClientNetworkEnginePrivateStoppedEvent;

        public static event ClientNetworkEngineNameChangeHandler ClientNetworkEngineNameChangeEvent;

        public static event ClientNetworkEngineServerMessageHandler ClientNetworkEngineServerMessageEvent;

        public static event ClientNetworkEngineImageMessageHandler ClientNetworkEngineImageMessageEvent;

        private static byte[] s_byteMessage;
        public static bool Status;
        private static Socket s_socket /* = new s_socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)*/;
        private static IPEndPoint s_ipdEndPoint;

        public static void AttemptConnect(string ipAdress, int port, string name, string userName, Color color)
        {
            try
            {
                if (Status)
                {
                    s_socket.Close();
                    Status = false;
                }
                //Should be equal to username once the form is ready
                Client.UserName = userName;
                Client.Name = name;
                Client.Color = color;
                //if (Status) {
                //    Connect();
                //}
                s_ipdEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);
                s_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s_socket.BeginConnect(s_ipdEndPoint, OnAttemptConnect, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> AttemptConnect", @"Chat: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnAttemptConnect(IAsyncResult ar)
        {
            try
            {
                Status = true;
                s_socket.EndConnect(ar); //notify the server the connection was established succefully
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.AttemptLogin,
                    Color = HexConverter.Convert(Client.Color),
                    ClientName = Client.Name,
                    UserName = Client.UserName
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                // Ssend the login credinails of the established connection to the server
                //s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                s_socket.Send(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None);
                s_byteMessage = new byte[2097152];
                s_socket.BeginReceive(s_byteMessage, 0, s_byteMessage.Length, SocketFlags.None, OnReceive, null);
            }
            catch (Exception ex)
            {
                if (ClientNetworkEngineAttemptLoginErrorEvent != null)
                {
                    ClientNetworkEngineAttemptLoginErrorEvent.Invoke(ex.Message);
                }
                //MessageBox.Show(ex.Message + @" -> OnAttemptConnect", @"Chat: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnReceive(IAsyncResult ar)
        {
            if (!Status)
            {
                return;
            }
            try
            {
                // Let the server know the message was recieved
                s_socket.EndReceive(ar);
                // Convert message from bytes to messageStracure class and store it in msgReceieved
                MessageStructure msgReceived = new MessageStructure(s_byteMessage);
                // Set new bytes and start recieving again
                s_byteMessage = new byte[2097152];
                if (msgReceived.Command == Command.Disconnect)
                {
                    Status = false;
                    MessageStructure msgToSend = new MessageStructure
                    {
                        Command = Command.Disconnect,
                        ClientName = Client.Name,
                        UserName = Client.UserName
                    };
                    byte[] b = msgToSend.ToByte();
                    s_socket.Send(b, 0, b.Length, SocketFlags.None);
                    s_socket.Shutdown(SocketShutdown.Both);
                    s_socket.BeginDisconnect(true, (OnDisonnect), s_socket);
                    if (ClientNetworkEngineDisconnectEvent != null)
                    {
                        ClientNetworkEngineDisconnectEvent.Invoke();
                    }
                    return;
                }
                if (msgReceived.Command != Command.AttemptLogin && msgReceived.Command != Command.Register)
                {
                    s_socket.BeginReceive(s_byteMessage, 0, s_byteMessage.Length, SocketFlags.None, OnReceive, s_socket);
                }

                switch (msgReceived.Command)
                {
                    case Command.Register:
                        if (ClientNetworkEngineRegisterMessageEvent != null)
                        {
                            ClientNetworkEngineRegisterMessageEvent.Invoke(msgReceived.Message);
                        }
                        break;

                    case Command.AttemptLogin:
                        if (ClientNetworkEngineAttemptLoginErrorEvent != null)
                        {
                            ClientNetworkEngineAttemptLoginErrorEvent.Invoke(msgReceived.Message);
                        }
                        break;

                    case Command.Login:
                        if (msgReceived.ClientName == Client.Name)
                        {
                            if (ClientNetworkEngineLoggedinEvent != null)
                            {
                                ClientNetworkEngineLoggedinEvent.Invoke();
                            }
                            // Send Request for online client list
                            MessageStructure msgToSend = new MessageStructure
                            {
                                Command = Command.List,
                                ClientName = Client.Name
                            };
                            byte[] byteMessageToSend = msgToSend.ToByte();
                            s_socket.BeginSend(byteMessageToSend, 0, byteMessageToSend.Length, SocketFlags.None, OnSend, s_socket);
                            return;
                        }
                        if (ClientNetworkEngineLoginEvent != null)
                        {
                            ClientNetworkEngineLoginEvent.Invoke(msgReceived.ClientName, msgReceived.Message);
                        }
                        break;

                    case Command.List:
                        if (ClientNetworkEngineClientsListEvent != null)
                        {
                            ClientNetworkEngineClientsListEvent.Invoke(msgReceived.Message);
                        }
                        break;

                    case Command.Logout:
                        if (ClientNetworkEngineLogoutEvent != null)
                        {
                            ClientNetworkEngineLogoutEvent.Invoke(msgReceived.ClientName, msgReceived.Message);
                        }
                        break;

                    case Command.Message:
                        if (ClientNetworkEngineMessageEvent != null)
                        {
                            ClientNetworkEngineMessageEvent.Invoke(msgReceived.ClientName, msgReceived.Message, ColorTranslator.FromHtml(msgReceived.Color));
                        }
                        break;

                    case Command.NameChange:
                        if (Client.Name == msgReceived.ClientName)
                        {
                            Client.Name = msgReceived.Message;
                        }
                        if (ClientNetworkEngineNameChangeEvent != null)
                        {
                            ClientNetworkEngineNameChangeEvent.Invoke(msgReceived.ClientName, msgReceived.Message, ColorTranslator.FromHtml(msgReceived.Color));
                        }
                        break;

                    case Command.ColorChanged:
                        if (ClientNetworkEngineColorChangedEvent != null)
                        {
                            ClientNetworkEngineColorChangedEvent.Invoke(msgReceived.ClientName, ColorTranslator.FromHtml(msgReceived.Color));
                        }
                        break;

                    case Command.PrivateStart:
                        if (ClientNetworkEnginePrivateChatStartEvent != null)
                        {
                            ClientNetworkEnginePrivateChatStartEvent.Invoke(msgReceived.ClientName);
                        }
                        break;

                    case Command.PrivateMessage:
                        if (ClientNetworkEnginePrivateMessageEvent != null)
                        {
                            ClientNetworkEnginePrivateMessageEvent.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message);
                        }
                        break;

                    case Command.PrivateStopped:
                        if (ClientNetworkEnginePrivateStoppedEvent != null)
                        {
                            ClientNetworkEnginePrivateStoppedEvent.Invoke(msgReceived.ClientName);
                        }
                        //TabPagePrivateChatReceiveClientEvent.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1);
                        break;

                    case Command.ServerMessage:
                        if (ClientNetworkEngineServerMessageEvent != null)
                        {
                            ClientNetworkEngineServerMessageEvent.Invoke(msgReceived.Message);
                        }
                        break;

                    case Command.ImageMessage:
                        MemoryStream ms = new MemoryStream(msgReceived.ImgByte);
                        Image img = Image.FromStream(ms);
                        if (ClientNetworkEngineImageMessageEvent != null)
                        {
                            ClientNetworkEngineImageMessageEvent.Invoke(msgReceived.ClientName, msgReceived.Private, img);
                        }
                        break;
                }
            }
            //catch (ArgumentException)
            //{
            //    //MessageBox.Show(ex.Message + @" -> OnReceive", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnSend(IAsyncResult ar)
        {
            try
            {
                s_socket.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnSend", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Reconnect()
        {
            try
            {
                s_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s_socket.BeginConnect(s_ipdEndPoint, OnAttemptConnect, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> ListBoxClientList_DoubleClick", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Disconnect()
        {
            try
            {
                if (!Status)
                {
                    return;
                }
                Status = false;
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.Logout,
                    UserName = Client.UserName,
                    ClientName = Client.Name
                };
                byte[] b = msgToSend.ToByte();
                s_socket.Send(b, 0, b.Length, SocketFlags.None);
                s_socket.Shutdown(SocketShutdown.Both);
                s_socket.BeginDisconnect(true, (OnDisonnect), s_socket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " -> Disconnect", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnDisonnect(IAsyncResult ar)
        {
            try
            {
                s_socket = (Socket)ar.AsyncState;
                s_socket.EndDisconnect(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " -> OnDisonnect", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendMessage(string message)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.Message,
                    UserName = Client.UserName,
                    ClientName = Client.Name,
                    Color = HexConverter.Convert(Client.Color),
                    Message = message
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> SendMessage", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void NameChange(string clientNameNew)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.NameChange,
                    UserName = Client.UserName,
                    ClientName = Client.Name,
                    Color = HexConverter.Convert(Client.Color),
                    Message = clientNameNew
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> NameChange", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ChangeColor(Color color)
        {
            try
            {
                //string colorHex = GenericStatic.HexConverter(ColorPicker.Color);
                //ClientConnection.Color = colorHex;
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.ColorChanged,
                    UserName = Client.UserName,
                    ClientName = Client.Name,
                    Color = HexConverter.Convert(color)
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> ChangeColor", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void StartPrivateChat(string clientNamePrivate)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    UserName = Client.UserName,
                    Command = Command.PrivateStart,
                    ClientName = Client.Name,
                    Private = clientNamePrivate
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> StartPrivateChat", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendPrivateMessage(string clientNamePrivate, string message)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    UserName = Client.UserName,
                    Command = Command.PrivateMessage,
                    ClientName = Client.Name,
                    Private = clientNamePrivate,
                    Message = message
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> SendPrivateMessage", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PrivateClose(string tabName)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    UserName = Client.UserName,
                    Command = Command.PrivateStopped,
                    ClientName = Client.Name,
                    Private = tabName
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> PrivateStop", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendImage(byte[] imgByte)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    UserName = Client.UserName,
                    ClientName = Client.Name,
                    Command = Command.ImageMessage,
                    ImgByte = imgByte
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> SendImage", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendImagePrivate(byte[] imgByte, string clientName, string clientNamePrivate)
        {
            try
            {
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.ImageMessage,
                    UserName = Client.UserName,
                    ClientName = Client.Name,
                    Private = clientNamePrivate,
                    ImgByte = imgByte
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> SendImagePrivate", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Register(string userName, string ipAdress, int port)
        {
            try
            {
                if (Status)
                {
                    s_socket.Close();
                    Status = false;
                }
                s_ipdEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);
                s_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                s_socket.BeginConnect(s_ipdEndPoint, OnRegister, null);
                Client.UserName = userName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> Register", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnRegister(IAsyncResult ar)
        {
            try
            {
                Status = true;
                s_socket.EndConnect(ar);
                MessageStructure msgToSend = new MessageStructure
                {
                    Command = Command.Register,
                    UserName = Client.UserName,
                    ClientName = Client.Name
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
                s_byteMessage = new byte[2097152];
                s_socket.BeginReceive(s_byteMessage, 0, s_byteMessage.Length, SocketFlags.None, OnReceive, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> OnRegister", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //public static void AttemptConnect2(string ipAddress, int port) {
        //    s_ipdEndPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        //    s_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    s_socket.BeginConnect(s_ipdEndPoint, OnAttempConnect2, null);
        //}

        //private static void OnAttempConnect2(IAsyncResult ar) {
        //    s_socket.EndConnect(ar);
        //    //s_byteMessage = new byte[2097152];
        //    //s_socket.BeginReceive(s_byteMessage, 0, s_byteMessage.Length, SocketFlags.None, OnReceive, null);
        //    MessageBox.Show("attemptConnect2 => conncted");
        //    AttempRegister();
        //}

        //public static void AttempRegister() {
        //    try {
        //        s_byteMessage = new byte[2097152];
        //        s_socket.BeginReceive(s_byteMessage, 0, s_byteMessage.Length, SocketFlags.None, OnReceive, null);
        //        MessageStructure msgToSend = new MessageStructure {
        //            Command = Command.Regiter,
        //            UserName = "alen",
        //            ClientName = "alen"
        //        };
        //        byte[] msgToSendByte = msgToSend.ToByte();
        //        s_socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
        //    }
        //    catch (Exception ex) {
        //        MessageBox.Show(ex.Message + @" -> OnRegister", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
    }
}