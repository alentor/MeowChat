using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryMeowChat;

namespace MeowChatClientLibrary {
    public static class ClientNetworkEngine {
        public static event ClientNetworkEngineLoginErrorHandler ClientNetworkEngineLoginErrorEvent;
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
        private static byte[] _ByteMessage;
        public static bool Status;
        public static Socket Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static IPEndPoint s_ipdEndPoint;


        public static void AttemptConnect(string ipAdress, int port, string name, Color color) {
            try {
                //Should be equal to username once the form is ready
                Client.UserName = name;
                Client.Name = name;
                s_ipdEndPoint = new IPEndPoint(IPAddress.Parse(ipAdress), port);
                Socket.BeginConnect(s_ipdEndPoint, OnAttemptConnect, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> AttemptConnect", @"Chat: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnAttemptConnect(IAsyncResult ar) {
            try {
                Status = true;
                Socket.EndConnect(ar); //notify the server the connection was established succefully
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.AttemptLogin,
                    Color = HexConverter.Convert(Client.Color),
                    ClientName = Client.Name,
                    Message = Client.UserName
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

        private static void OnReceive(IAsyncResult ar) {
            if (!Status) {
                return;
            }
            try {
                //Let the server know the message was recieved
                Socket.EndReceive(ar);
                //Convert message from bytes to messageStracure class and store it in msgReceieved
                MessageStructure msgReceived = new MessageStructure(_ByteMessage);
                //Set new bytes and start recieving again
                _ByteMessage = new byte[2097152];
                if (msgReceived.Command == Command.Disconnect) {
                    Status = false;
                    MessageStructure msgToSend = new MessageStructure {
                        Command = Command.Disconnect,
                        ClientName = Client.Name
                    };
                    byte[] b = msgToSend.ToByte();
                    Socket.Send(b, 0, b.Length, SocketFlags.None);
                    Socket.Shutdown(SocketShutdown.Both);
                    Socket.BeginDisconnect(true, (OnDisonnect), Socket);
                    ClientNetworkEngineDisconnectEvent?.Invoke();
                    return;
                }
                Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, Socket);
                switch (msgReceived.Command) {
                    case Command.AttemptLogin:

                        break;

                    case Command.Login:
                        if (msgReceived.ClientName == Client.Name) {
                            ClientNetworkEngineLoggedinEvent?.Invoke();
                            // Send Request for online client list
                            MessageStructure msgToSend = new MessageStructure {
                                Command = Command.List,
                                ClientName = Client.Name
                            };
                            byte[] byteMessageToSend = msgToSend.ToByte();
                            Socket.BeginSend(byteMessageToSend, 0, byteMessageToSend.Length, SocketFlags.None, OnSend, Socket);
                            return;
                        }
                        ClientNetworkEngineLoginEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                        break;

                    case Command.List:
                        ClientNetworkEngineClientsListEvent?.Invoke(msgReceived.Message);
                        break;

                    case Command.Logout:
                        ClientNetworkEngineLogoutEvent?.Invoke(msgReceived.ClientName, msgReceived.Message);
                        break;

                    case Command.Message:
                        ClientNetworkEngineMessageEvent?.Invoke(msgReceived.ClientName, msgReceived.Message, ColorTranslator.FromHtml(msgReceived.Color));
                        break;

                    case Command.NameChange:
                        if (Client.Name == msgReceived.ClientName) {
                            Client.Name = msgReceived.Message;
                        }
                        ClientNetworkEngineNameChangeEvent?.Invoke(msgReceived.ClientName, msgReceived.Message, ColorTranslator.FromHtml(msgReceived.Color));
                        break;

                    case Command.ColorChanged:
                        ClientNetworkEngineColorChangedEvent?.Invoke(msgReceived.ClientName, ColorTranslator.FromHtml(msgReceived.Color));
                        break;

                    case Command.PrivateStart:
                        ClientNetworkEnginePrivateChatStartEvent?.Invoke(msgReceived.ClientName);
                        break;

                    case Command.PrivateMessage:
                        ClientNetworkEnginePrivateMessageEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message);
                        break;

                    case Command.PrivateStopped:
                        ClientNetworkEnginePrivateStoppedEvent?.Invoke(msgReceived.ClientName);
                        //TabPagePrivateChatReceiveClientEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1);
                        break;

                    case Command.ServerMessage:
                        ClientNetworkEngineServerMessageEvent?.Invoke(msgReceived.Message);
                        break;

                    case Command.ImageMessage:
                        MemoryStream ms = new MemoryStream(msgReceived.ImgByte);
                        Image img = Image.FromStream(ms);
                        ClientNetworkEngineImageMessageEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, img);
                        break;
                }
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnReceive", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private static void OnSend(IAsyncResult ar) {
            try {
                Socket.EndSend(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnSend", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Reconnect() {
            try {
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Socket.BeginConnect(s_ipdEndPoint, OnAttemptConnect, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ListBoxClientList_DoubleClick", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void Disconnect() {
            try {
                if (!Status) {
                    return;
                }
                Status = false;
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.Logout,
                    ClientName = Client.Name
                };
                byte[] b = msgToSend.ToByte();
                Socket.Send(b, 0, b.Length, SocketFlags.None);
                Socket.Shutdown(SocketShutdown.Both);
                Socket.BeginDisconnect(true, (OnDisonnect), Socket);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " -> Disconnect", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnDisonnect(IAsyncResult ar) {
            try {
                Socket = (Socket) ar.AsyncState;
                Socket.EndDisconnect(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " -> OnDisonnect", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendMessage(string message) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.Message,
                    ClientName = Client.Name,
                    Color = HexConverter.Convert(Client.Color),
                    Message = message
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> SendMessage", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void NameChange(string clientNameNew) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.NameChange,
                    ClientName = Client.Name,
                    Color = HexConverter.Convert(Client.Color),
                    Message = clientNameNew
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> NameChange", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void ChangeColor(Color color) {
            try {
                //string colorHex = GenericStatic.HexConverter(ColorPicker.Color);
                //ClientConnection.Color = colorHex;    
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.ColorChanged,
                    ClientName = Client.Name,
                    Color = HexConverter.Convert(color)
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ChangeColor", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void StartPrivateChat(string clientNamePrivate) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.PrivateStart,
                    ClientName = Client.Name,
                    Private = clientNamePrivate
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> StartPrivateChat", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendPrivateMessage(string clientNamePrivate, string message) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.PrivateMessage,
                    ClientName = Client.Name,
                    Private = clientNamePrivate,
                    Message = message
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> SendPrivateMessage", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void PrivateClose(string tabName) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.PrivateStopped,
                    ClientName = Client.Name,
                    Private = tabName
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> PrivateStop", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendImage(byte[] imgByte) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    ClientName = Client.Name,
                    Command = Command.ImageMessage,
                    ImgByte = imgByte
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> SendImage", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SendImagePrivate(byte[] imgByte, string clientName, string clientNamePrivate) {
            try {
                MessageStructure msgToSend = new MessageStructure {
                    Command = Command.ImageMessage,
                    ClientName = Client.Name,
                    Private = clientNamePrivate,
                    ImgByte = imgByte
                };
                byte[] msgToSendByte = msgToSend.ToByte();
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> SendImagePrivate", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}