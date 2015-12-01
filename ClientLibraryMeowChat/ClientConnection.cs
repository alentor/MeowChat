using LibraryMeowChat;
using MeowChatClient;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MeowChatClientLibrary {
    //Stores the clients connection information, as well as handless clients connect and disconnect actions
    public static class ClientConnection {
        public static bool Status;
        public static string ClientName;
        public static string Color;
        public static Socket Socket;
        public static string Address;
        public static int Port;

        public static event FrmLoginCloseHandler LoginFrmCloseEvent;

        //Connect
        public static void Connect(string address, int port, string name) {
            try {
                Address = address;
                ClientName = name;
                Port = port;
                var ipAdressText = IPAddress.Parse(address);
                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipEndPoint = new IPEndPoint(ipAdressText, Port);
                Socket.BeginConnect(ipEndPoint, Connected, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ListBoxClientList_DoubleClick", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void Connected(IAsyncResult ar) {
            try {
                Socket.EndConnect(ar); //notify the server the connection was established succefully
                var msgToSend = new MessageStracture {
                    Command = Command.Login,
                    ClientName = ClientName,
                    Message = null
                };
                var msgToSendByte = msgToSend.ToByte();
                //send the login credinails of the established connection to the server and call to the methood OnSend
                Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> Connected", @"Chat: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnSend(IAsyncResult ar) {
            try {
                Socket.EndSend(ar);
                Status = true;
                LoginFrmCloseEvent?.Invoke(); //Fire event to close the FrmLogin
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " -> OnSend", @"Chat: " + ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Disconnect method
        public static void Disconnect() {
            try {
                Status = false;
                var msgToSend = new MessageStracture {
                    Command = Command.Logout,
                    ClientName = ClientName
                };
                var b = msgToSend.ToByte();
                Socket.Send(b, 0, b.Length, SocketFlags.None);
                Socket.Shutdown(SocketShutdown.Both);
                Socket.BeginDisconnect(true, (OnDisonnect), Socket);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " -> Disconnect", @"Chat: " + ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Shutdown Method
        public static void ServerDisconnectCall() {
            try {
                Status = false;
                var msgToSend = new MessageStracture {
                    Command = Command.Disconnect,
                    ClientName = ClientName
                };
                var b = msgToSend.ToByte();
                Socket.Send(b, 0, b.Length, SocketFlags.None);
                Socket.Shutdown(SocketShutdown.Both);
                Socket.BeginDisconnect(true, (OnDisonnect), Socket);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " -> ServerDisconnectCall", @"Chat: " + ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnDisonnect(IAsyncResult ar) {
            try {
                Socket = (Socket) ar.AsyncState;
                Socket.EndDisconnect(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + " -> OnDisonnect", @"Chat: " + ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            ;
        }
    }
}