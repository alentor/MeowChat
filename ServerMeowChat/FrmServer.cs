using CommonLibrary;
using MeowChatServerLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MeowChatServer
{
    public partial class FrmServer : Form
    {
        private readonly byte[] _ByteMessage = new byte[1024]; //Max byte size to be recieved and sent
        private readonly List<Client> _ClientList = new List<Client>(); //List which contains all the connected clients
        private int _CursorPositionConn;
        private int _CursorPositionPub;
        private Socket _ServerSocket; //Server socket
        private TabPagePrivateChatReceiveServerHandler _TabPagePrivateChatReceiveServerEvent;

        public FrmServer()
        {
            InitializeComponent();
        }

        private void ServerForm_Load(object sender, EventArgs e)
        {
            try
            {
                //Controls contained in a TabPage are not created until the tab page is shown, and any data bindings in these controls are not activated until the tab page is shown.
                //https://msdn.microsoft.com/en-us/library/system.windows.forms.tabpage.aspx
                TabControlServer.TabPages[1].Show();
                TabControlServer.TabPages[0].Show();
                lblLocalIp.Text = GetLocalIpAddress(); //Get local IP address and display it on lblLocalIp
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //BTN Start
        private void btnStartSrv_Click(object sender, EventArgs e)
        {
            try
            {
                _ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var ipAddress = IPAddress.Parse(txtBoxIpAddress.Text);
                var ipEndPoint = new IPEndPoint(ipAddress, int.Parse(txtBoxPort.Text));
                _ServerSocket.Bind(ipEndPoint); //Bind the socket to local endPoint
                //10 specifies the number of incoming connections that can be queued for acceptance
                _ServerSocket.Listen(10); //Start listening for incoming connection
                //Start accppting incoming connection, on a succefull accept call to OnAccept method
                _ServerSocket.BeginAccept((OnAccept), null);
                //The inner server messages board
                rchTxtServerConn.SelectionStart = _CursorPositionConn;
                rchTxtServerConn.SelectionColor = Color.Black;
                rchTxtServerConn.SelectionBackColor = Color.Blue;
                rchTxtServerConn.SelectedText += @"Server have started " + DateTime.Now + Environment.NewLine;
                _CursorPositionConn = rchTxtServerConn.SelectionStart;
                //Disbale/Enable buttons as needed
                btnStartSrv.Enabled = false;
                btnStopSrv.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                //Create a connection(client) based on the async accepted connection
                var clienSocket = _ServerSocket.EndAccept(ar);
                //Start accepting again
                _ServerSocket.BeginAccept((OnAccept), null);
                //Start receiving(listening for) information on the accepted socket, once information is receive go to OnReceive client
                clienSocket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, (OnReceive), clienSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                //establised connection(client), and casting it to a socket class.
                //passing down the AsyncState information of the established connection
                var receivedClientSocket = (Socket)ar.AsyncState;
                //Transfering the array of received bytes from the established connection(client)
                //into an intelligent form of object MessageStracture
                var msgReceived = new MessageStracture(_ByteMessage);
                //Constractor for new object MessageStracture which will be sent to all establihed connections(clients)
                var msgToSend = new MessageStracture
                {
                    Command = msgReceived.Command,
                    ClientName = msgReceived.ClientName,
                    Color = msgReceived.Color
                };
                //Create a new byte[] to write into it msgToSend
                byte[] messageBytes;
                switch (msgReceived.Command)
                {
                    case Command.Login:
                        //checks if we don't have already a user with the same Name, if we do. we random a number to add to the name
                        foreach (var client in _ClientList.Where(clientLinq => msgReceived.ClientName == clientLinq.ClientName))
                        {
                            var rnd = new Random();
                            msgReceived.ClientName = client.ClientName + rnd.Next(1, 999999);
                            msgToSend.ClientName = msgReceived.ClientName;
                            //break;
                        }
                        //when the Login command received the server will add the established connection(client)
                        //to the ClientList and let every other established connection(clients)
                        //know of a new established connection(cleint)
                        var newClient = new Client
                        {
                            ClientSocket = receivedClientSocket,
                            ClientName = msgReceived.ClientName,
                            Color = msgReceived.Color
                        };
                        //adding the current handled established connection(client) to the clientlist
                        _ClientList.Add(newClient);
                        //adding the current handedled established connection(client) to UI clientlist
                        var remoteIpEndPoint = receivedClientSocket.RemoteEndPoint as IPEndPoint;
                        var row = new ListViewItem(new[] { msgReceived.ClientName, remoteIpEndPoint.Address.ToString(), DateTime.Now.ToString() });
                        Invoke(new Action((delegate
                        {
                            listViewClients.Items.Add(row);
                            //set the message which will be send (broadcasted) to all the established connections(clients)
                            msgToSend.Message = "<<< " + newClient.ClientName + " has joined the room at >>>";
                            //The inner server messages board
                            rchTxtServerConn.SelectionStart = _CursorPositionConn;
                            rchTxtServerConn.SelectionBackColor = Color.LightGreen;
                            rchTxtServerConn.SelectionColor = Color.Black;
                            rchTxtServerConn.SelectedText += msgToSend.Message + " " + DateTime.Now + Environment.NewLine;
                            ChatMethodsStatic.FormatItemSize(TabControlServer);
                            _CursorPositionConn = rchTxtServerConn.SelectionStart;
                        })));
                        break;

                    case Command.Logout:
                        //When the logout command received the server will remove the established connection(client) which
                        //have sent the command. The server will find the established connection(client) by the clientname
                        //close the connection, and remove the client form the UI clientList
                        var clientIndexLogout = 0;
                        foreach (var client in _ClientList)
                        {
                            if (client.ClientSocket == receivedClientSocket)
                            {
                                Invoke(new Action((delegate
                                {
                                    listViewClients.Items.RemoveAt(clientIndexLogout);
                                })));
                                _ClientList.RemoveAt(clientIndexLogout);
                                break;
                            }
                            ++clientIndexLogout;
                        }
                        receivedClientSocket.Shutdown(SocketShutdown.Both);
                        receivedClientSocket.BeginDisconnect(true, (OnDisonnect), receivedClientSocket);
                        msgToSend.Message = "<<< " + msgReceived.ClientName + " has just left the chat >>>";
                        Invoke(new Action((delegate
                        {
                            rchTxtServerConn.SelectionStart = _CursorPositionConn;
                            rchTxtServerConn.SelectionBackColor = Color.Tomato;
                            rchTxtServerConn.SelectionColor = Color.Black;
                            rchTxtServerConn.SelectedText += msgToSend.Message + " " + DateTime.Now + Environment.NewLine;
                            _CursorPositionConn = rchTxtServerConn.SelectionStart;
                            ChatMethodsStatic.FormatItemSize(TabControlServer);
                            _TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 2);
                        })));
                        break;

                    case Command.List:
                        //when the List command received the server will send the names of all the
                        //established coneections(clients) anmes back to the requesting established connection(client)
                        msgToSend.Command = Command.List;
                        var lastItem = _ClientList[_ClientList.Count - 1];
                        msgToSend.ClientName = lastItem.ClientName;
                        foreach (var client in _ClientList)
                        {
                            if (msgReceived.ClientName == client.ClientName)
                            {
                            }
                            //To keep things simple we use a marker to separate the user names
                            msgToSend.Message += client.ClientName + ",";
                        }
                        //Convert msgToSend to a bytearray representative, this is needed in order to send(broadcat) the message over the TCP protocol
                        messageBytes = msgToSend.ToByte();
                        //Send(broadcast) the name of the estalished connections(cleints) in the chat
                        receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        break;

                    case Command.Message:
                        //Set the message that we will send(broadcasted) to established connections(clients)
                        msgToSend.Message = msgReceived.Message;
                        var color = ColorTranslator.FromHtml(msgToSend.Color);
                        Invoke(new Action((delegate
                        {
                            rchTxtServerPub.SelectionStart = _CursorPositionPub;
                            rchTxtServerPub.SelectedText = ChatMethodsStatic.Time() + " ";
                            var selectionStart = rchTxtServerPub.SelectionStart;
                            rchTxtServerPub.SelectionColor = color;
                            rchTxtServerPub.SelectedText = msgToSend.ClientName + @" :" + msgToSend.Message;
                            rchTxtServerPub.SelectedText = Environment.NewLine;
                            _CursorPositionPub = rchTxtServerPub.SelectionStart;
                            foreach (var clientColor in _ClientList.Where(clientLinq => clientLinq.ClientName == msgToSend.ClientName))
                            {
                                int[] selectionArr = { selectionStart, rchTxtServerPub.TextLength - selectionStart };
                                clientColor.Messages.Add(selectionArr);
                            }
                        })));
                        break;

                    case Command.NameChange:
                        var clientIndexNameChange = 0;
                        foreach (var client in _ClientList)
                        {
                            if (client.ClientName == msgReceived.ClientName)
                            {
                                client.ClientName = msgReceived.Message;
                                Invoke(new Action((delegate
                                {
                                    listViewClients.Items[clientIndexNameChange].Text = msgReceived.Message;
                                })));
                                break;
                            }
                            ++clientIndexNameChange;
                        }
                        msgToSend.Message = msgReceived.Message;
                        Invoke(new Action((delegate
                        {
                            rchTxtServerConn.SelectionStart = _CursorPositionConn;
                            rchTxtServerConn.SelectionColor = Color.Black;
                            rchTxtServerConn.SelectionBackColor = Color.CornflowerBlue;
                            rchTxtServerConn.SelectedText += @"<<< " + msgToSend.ClientName + @" have changed nickname to " + msgToSend.Message + " " + DateTime.Now + @" >>>" + Environment.NewLine;
                            _CursorPositionConn = rchTxtServerConn.SelectionStart;
                            foreach (var tabPage in TabControlServer.TabPages.OfType<TabPagePrivateChatServer>())
                            {
                                if (tabPage.TabName0 == msgReceived.ClientName)
                                {
                                    tabPage.TabName0 = msgReceived.Message;
                                    tabPage.Text = msgReceived.Message + @" - " + tabPage.TabName1;
                                    TabControlServer.Invalidate();
                                }
                                if (tabPage.TabName1 == msgReceived.ClientName)
                                {
                                    tabPage.TabName1 = msgReceived.Message;
                                    tabPage.Text = tabPage.TabName0 + @" - " + msgReceived.Message;
                                    TabControlServer.Invalidate();
                                }
                            }
                            ChatMethodsStatic.FormatItemSize(TabControlServer);
                        })));
                        goto case Command.ColorChange;

                    case Command.ColorChange:
                        var newColor = ColorTranslator.FromHtml(msgToSend.Color);
                        foreach (var client in _ClientList.Where(client => client.ClientName == msgReceived.ClientName))
                        {
                            client.Color = msgReceived.Color;
                            foreach (var selectedText in client.Messages)
                            {
                                Invoke(new Action((delegate
                                {
                                    rchTxtServerPub.Select(selectedText[0], selectedText[1]);
                                    rchTxtServerPub.SelectionColor = newColor;
                                })));
                            }
                        }
                        msgToSend.Message = msgReceived.Message;
                        break;

                    case Command.PrivateStart:
                        if (TabControlServer.TabPages.OfType<TabPagePrivateChatServer>().Any(tabPagePrivateChatServer => tabPagePrivateChatServer.TabName0 == msgReceived.ClientName && tabPagePrivateChatServer.TabName1 == msgReceived.Private || tabPagePrivateChatServer.TabName0 == msgReceived.Private && tabPagePrivateChatServer.TabName1 == msgReceived.ClientName))
                        {
                            foreach (var client in _ClientList.Where(clientLinq => clientLinq.ClientName == msgReceived.Private))
                            {
                                msgToSend.Private = msgReceived.Private;
                                messageBytes = msgToSend.ToByte();
                                client.ClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.ClientSocket);
                            }
                            _TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 3);

                            break;
                        }
                        Invoke(new Action((delegate
                        {
                            NewTabPagePrivateChatServer(msgReceived.ClientName, msgReceived.Private);
                        })));
                        foreach (var client in _ClientList)
                        {
                            if (client.ClientName == msgReceived.Private)
                            {
                                msgToSend.Private = msgReceived.Private;
                                messageBytes = msgToSend.ToByte();
                                client.ClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.ClientSocket);
                            }
                        }
                        Invoke(new Action((delegate
                        {
                            ChatMethodsStatic.FormatItemSize(TabControlServer);
                        })));
                        break;

                    case Command.PrivateMessage:
                        foreach (var client in _ClientList.Where(clientLinq => clientLinq.ClientName == msgReceived.Private))
                        {
                            msgToSend.Private = msgReceived.Private;
                            msgToSend.Message = msgReceived.Message;
                            messageBytes = msgToSend.ToByte();
                            client.ClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.ClientSocket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        }
                        _TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 0);
                        break;

                    case Command.PrivateStop:
                        foreach (var client in _ClientList.Where(clientLinq => clientLinq.ClientName == msgReceived.Private))
                        {
                            msgToSend.Private = msgReceived.Private;
                            messageBytes = msgToSend.ToByte();
                            client.ClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.ClientSocket);
                            receivedClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, receivedClientSocket);
                        }
                        _TabPagePrivateChatReceiveServerEvent?.Invoke(msgReceived.ClientName, msgReceived.Private, msgReceived.Message, 1);
                        break;
                }
                //Send message to clients
                if (msgToSend.Command != Command.List && msgToSend.Command != Command.PrivateStart && msgToSend.Command != Command.PrivateMessage && msgToSend.Command != Command.PrivateStop)
                {
                    //Convert msgToSend to a bytearray representative, this needed to send(broadcat) the message over the TCP protocol
                    messageBytes = msgToSend.ToByte();
                    foreach (var client in _ClientList)
                    {
                        if (client.ClientSocket != receivedClientSocket || msgToSend.Command != Command.Login)
                        {
                            //Send(broadcast) the message to established connections(clients)
                            client.ClientSocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, OnSend, client.ClientSocket);
                        }
                    }
                }
                //Continue listneing to receivedClientSocket established connection(client)
                if (msgReceived.Command != Command.Logout)
                {
                    receivedClientSocket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, receivedClientSocket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //After the message is sent/recieved end the async operation
        private void OnSend(IAsyncResult ar)
        {
            try
            {
                //passing down AsyncState
                var client = (Socket)ar.AsyncState;
                //let the client know message send/recieve have ended
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //BTN Stop
        private void btnStopSrv_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var client in _ClientList)
                {
                    var msgToSend = new MessageStracture
                    {
                        Command = Command.Disconnect
                    };
                    var msgToSendByte = msgToSend.ToByte();
                    client.ClientSocket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.ClientSocket);
                }
                //_ServerSocket.Close();
                _ServerSocket.Shutdown(SocketShutdown.Both);
                _ServerSocket.BeginDisconnect(true, (OnDisonnect), _ServerSocket);
                MessageBox.Show(@"The server went down", @"Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnStopSrv.Enabled = false;
                btnStartSrv.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, @"Server 2", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void OnDisonnect(IAsyncResult ar)
        {
            var socket = (Socket)ar.AsyncState;
            socket.EndDisconnect(ar);
        }

        //Automaticlaly scrolldown richTxtChatBox
        private void richTxtChatBox_TextChanged(object sender, EventArgs e)
        {
            rchTxtServerConn.SelectionStart = rchTxtServerConn.Text.Length;
            rchTxtServerConn.ScrollToCaret();
        }

        private static string GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }

        //TabControl DrawItem, used to the draw the X on each tab
        private void TabControlServer_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Draw the name of the tab
            e.Graphics.DrawString(TabControlServer.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 10, e.Bounds.Top + 7);
            for (var i = 2; i < TabControlServer.TabPages.Count; i++)
            {
                var tabRect = TabControlServer.GetTabRect(i);
                //Not active tab
                if (i != TabControlServer.SelectedIndex)
                {
                    //Rectangle r = TabControlServer.TabPages[i].Text;
                    //using (Brush brush = new SolidBrush(Color.Yellow)) {
                    //    e.Graphics.FillRectangle(brush, tabXarea.X + tabXarea.Width - 18, 6, 16, 16);
                    //}
                    using (var pen = new Pen(Color.Black, 2))
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
                    //Rectangle r = TabControlServer.TabPages[i].Text;
                    //RectangleF tabXarea = new Rectangle(tabRect.Right - TabControlServer.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                    //using (Brush brush = new SolidBrush(Color.Purple)) {
                    //    e.Graphics.FillRectangle(brush, tabXarea.X + tabXarea.Width - 18, 6, 16, 16);
                    //}
                    using (var pen = new Pen(Color.Silver, 2))
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
        private void TabControlServert_MouseClick(object sender, MouseEventArgs e)
        {
            for (var i = 2; i < TabControlServer.TabPages.Count; i++)
            {
                var tabRect = TabControlServer.GetTabRect(i);
                //Getting the position of the "x" mark.
                //Rectangle tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                var closeXButtonArea = new Rectangle(tabRect.Right - 23, 6, 16, 16);
                //Rectangle closeButton = new Rectangle(tabRect.Right - 13, tabRect.Top + 6, 9, 7);
                if (closeXButtonArea.Contains(e.Location))
                {
                    if (MessageBox.Show(@"Would you like to Close this Tab?", @"Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        TabControlServer.TabPages.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        //Method to which createsa new class of TabPagePrivateChatServer and adds it to TabControlServer
        private void NewTabPagePrivateChatServer(string tabName0, string tabName1)
        {
            var newPrivateTab = new TabPagePrivateChatServer(tabName0, tabName1);
            TabControlServer.TabPages.Add(newPrivateTab);
            _TabPagePrivateChatReceiveServerEvent += newPrivateTab.TabPagePrivateReceiveMessageServer;
        }

        //Button send
        private void btnServerSnd_Click(object sender, EventArgs e)
        {
            var msgToSend = new MessageStracture
            {
                Command = Command.ServerMessage,
                Message = txtBxServer.Text
            };
            _CursorPositionPub = rchTxtServerPub.SelectionStart;
            rchTxtServerPub.SelectionColor = Color.Black;
            rchTxtServerPub.SelectionBackColor = Color.MediumPurple;
            rchTxtServerPub.SelectedText = txtBxServer.Text + Environment.NewLine;
            rchTxtServerPub.SelectionStart = _CursorPositionPub;
            var msgToSendByte = msgToSend.ToByte();
            foreach (var client in _ClientList)
            {
                client.ClientSocket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, client.ClientSocket);
            }
            txtBxServer.Text = "";
        }
    }
}