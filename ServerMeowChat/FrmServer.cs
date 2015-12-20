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
        private readonly ServerNetworkEngineEngine _ServerNetworkEngineEngine = new ServerNetworkEngineEngine();
        private readonly List <ClientMessagesPosition> _ClientMessagesesList = new List <ClientMessagesPosition>();
        private FrmServerProgressBar _FrmProgressBarDisconnect;
        private readonly FrmServerImages _FrmServerImages = new FrmServerImages();
        private event TabPagePrivateServerDoActionHandler TabPagePrivateServerDoActionEvent;
        private event FrmServerImagesChangeNameHandler FrmServerImagesChangeNameEvent;
        private event FrmServerServerMessageHandler FrmServerServerMessageEvent;
        private int _CursorPositionConn;
        private int _CursorPositionPub;
        private bool _IsServerRunning;
        public FrmServer() {
            InitializeComponent();
            // Controls contained in a TabPage are not created until the tab page is shown, and any data bindings in these controls are not activated until the tab page is shown.
            // https://msdn.microsoft.com/en-us/library/system.windows.forms.tabpage.aspx
            TabControlServer.TabPages[1].Show();
            TabControlServer.TabPages[0].Show();
            // Setting the events for Server Network Engine to FrmServer
            _ServerNetworkEngineEngine.ServerNetworkEngineEngineServerStartedEvent += ServerNetworkEngineNetworkStarted;
            _ServerNetworkEngineEngine.ServerNetworkEngineEngineClientToAddEvent += ClientToAdd;
            _ServerNetworkEngineEngine.ServerNetworkEngineSendPublicMessageEvent += PublicMessage;
            _ServerNetworkEngineEngine.ServerNetworkEngineServerStopBeganEvent += ServerNetworkNetworkEngineStopBegan;
            _ServerNetworkEngineEngine.ServerNetworkEngineServerStopTickEvent += ServerNetworkNetworkEngineStopTick;
            _ServerNetworkEngineEngine.ServerNetworkEngineServerStoppedEvent += ServerNetworkNetworkEngineStopped;
            _ServerNetworkEngineEngine.ServerNetworkEngineClientColorChangedEvent += ClientColorChanged;
            _ServerNetworkEngineEngine.ServerNetworkEngineClientToRemoveEvent += ClientToRemove;
            _ServerNetworkEngineEngine.ServerNetworkEngineClientNameChangedEvent += ClientNameChanged;
            _ServerNetworkEngineEngine.ServerNetworkEnginePrivateChatStartedEvent += PrivateChatStarted;
            _ServerNetworkEngineEngine.ServerNetworkEnginePrivateChatMessageEvent += PrivateChatMessage;
            _ServerNetworkEngineEngine.ServerNetworkEnginePrivateChatStoppedEvent += PrivateChatStopped;
            _ServerNetworkEngineEngine.ServerNetworkEngineImageMessageEvent += ImageMessage;
            // Setting the event for server message from FrmServer to ServerNetworkEngine
            FrmServerServerMessageEvent += _ServerNetworkEngineEngine.ServerMessage;
            // Setting the event from FrmServer to FrmImages when name is changed
            FrmServerImagesChangeNameEvent += _FrmServerImages.ChangeTabName;
        }

        // Button Start
        private void BtnStartSrv_Click(object sender, EventArgs e) {
            _ServerNetworkEngineEngine.StartServer(TxtBoxIpAddress.Text, TxtBoxPort.Text);
        }

        // Button stop
        private void btnStopSrv_Click(object sender, EventArgs e) {
            _ServerNetworkEngineEngine.ServerStop();
        }

        // Server Started
        private void ServerNetworkEngineNetworkStarted() {
            // The inner server messages board
            RichTextServerConn.SelectionStart = _CursorPositionConn;
            RichTextServerConn.SelectionColor = Color.Black;
            RichTextServerConn.SelectionBackColor = Color.BlueViolet;
            RichTextServerConn.SelectedText += @"Server have started " + GenericStatic.TimeDate() + Environment.NewLine;
            _CursorPositionConn = RichTextServerConn.SelectionStart;
            BtnStartSrv.Enabled = false;
            BtnStopSrv.Enabled = true;
            _IsServerRunning = true;
        }

        // Server stop began
        private void ServerNetworkNetworkEngineStopBegan(int clientsCount) {
            new Thread(() =>{
                Thread.CurrentThread.IsBackground = true;
                _FrmProgressBarDisconnect = new FrmServerProgressBar(clientsCount);
                if (InvokeRequired) {
                    Invoke(new MethodInvoker(delegate{
                        _FrmProgressBarDisconnect.Show();
                    }));
                }
                else {
                    _FrmProgressBarDisconnect.Show();
                }
            }).Start();
        }

        // Server stop tick
        private void ServerNetworkNetworkEngineStopTick(string currentDisconnectintClientName) {
            Invoke(new Action((delegate{
                foreach (ListViewItem item in ListViewClients.Items.Cast <ListViewItem>().Where(item => item.Name == currentDisconnectintClientName)) {
                    ListViewClients.Items.RemoveByKey(item.Name);
                    break;
                }
            })));
            Invoke(new Action((delegate{
                // The inner server messages board
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.Tomato;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectedText += "<<< " + currentDisconnectintClientName + " Disconnection preformed successfully  >>>" + " " + GenericStatic.Time() + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
            })));
            _FrmProgressBarDisconnect.UpdateProgressBar(currentDisconnectintClientName);
        }

        // Server stopped
        private void ServerNetworkNetworkEngineStopped() {
            Invoke(new Action((delegate{
                // The inner server messages board
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.DarkRed;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectedText += "<<< Server stopped successfully  >>>" + " " + GenericStatic.Time() + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
                BtnStartSrv.Enabled = true;
                BtnStopSrv.Enabled = false;
                _IsServerRunning = false;
            })));
            Invoke(new Action((delegate{
                _FrmProgressBarDisconnect?.Close();
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
                // The inner server messages board
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
                // The inner server messages board
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
                // The inner server messages board
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
            foreach (int[] messages in _ClientMessagesesList.Where(position => position.ClientName == clientName).SelectMany(position => position.Messages)) {
                Invoke(new Action((delegate{
                    RichTextServerPub.Select(messages[0], messages[1]);
                    RichTextServerPub.SelectionColor = newColor;
                })));
            }
        }

        // Client Name Changed
        private void ClientNameChanged(string clientName, string newClientName) {
            // Change the ListViewItem.name in ListViewClients
            Invoke(new Action(delegate{
                for (int i = 0; i < ListViewClients.Items.Count; i++) {
                    if (ListViewClients.Items[i].Name != clientName) {
                        continue;
                    }
                    ListViewClients.Items[i].Text = newClientName;
                    ListViewClients.Items[i].Name = newClientName;
                    break;
                }
                foreach (TabPagePrivateChatServer tabPage in TabControlServer.TabPages.OfType <TabPagePrivateChatServer>()) {
                    if (tabPage.ClientName == clientName) {
                        tabPage.ClientName = newClientName;
                        tabPage.Text = newClientName + @" - " + tabPage.ClientNamePrivate;
                        TabControlServer.Invalidate();
                    }
                    if (tabPage.ClientNamePrivate == clientName) {
                        tabPage.ClientNamePrivate = newClientName;
                        tabPage.Text = tabPage.ClientName + @" - " + newClientName;
                        TabControlServer.Invalidate();
                    }
                }
                GenericStatic.FormatItemSize(TabControlServer);
                // The inner server messages board
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionColor = Color.Black;
                RichTextServerConn.SelectionBackColor = Color.CornflowerBlue;
                RichTextServerConn.SelectedText += @"<<< " + clientName + @" have changed his name to " + newClientName + " " + GenericStatic.TimeDate() + @" >>>" + Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
            }));
            FrmServerImagesChangeNameEvent?.Invoke(clientName, newClientName);
        }

        // Private Chat Started
        private void PrivateChatStarted(string clientName, string clientNamePrivate) {
            if (TabControlServer.TabPages.OfType <TabPagePrivateChatServer>().Any(tabPagePrivateChatServer => tabPagePrivateChatServer.ClientName == clientName && tabPagePrivateChatServer.ClientNamePrivate == clientNamePrivate || tabPagePrivateChatServer.ClientName == clientNamePrivate && tabPagePrivateChatServer.ClientNamePrivate == clientName)) {
                TabPagePrivateServerDoActionEvent?.Invoke(clientName, clientNamePrivate, null, TabPagePrivateChatServer.TabCommand.Resumed);
                return;
            }
            Invoke(new Action(delegate{
                NewTabPagePrivateChatServer(clientName, clientNamePrivate);
                GenericStatic.FormatItemSize(TabControlServer);

            }));
            //Invoke(new Action(delegate{
            //}));
        }

        // Private Chat Message
        private void PrivateChatMessage(string clientName, string clientNamePrivate, string message) {
            TabPagePrivateServerDoActionEvent?.Invoke(clientName, clientNamePrivate, message, TabPagePrivateChatServer.TabCommand.Message);
        }

        // Private Chat Stopped
        private void PrivateChatStopped(string clientName, string clientNamePrivate) {
            TabPagePrivateServerDoActionEvent?.Invoke(clientName, clientName, null, TabPagePrivateChatServer.TabCommand.Closed);
        }

        // Image Message
        private void ImageMessage(Image img, string clientName, string clientNamePrivate) {
            if (clientNamePrivate != null) {
                _FrmServerImages.NewImage(img, clientName + " Private " + clientNamePrivate);
                Invoke(new Action((delegate{
                    // The inner server messages board
                    RichTextServerConn.SelectionStart = _CursorPositionConn;
                    RichTextServerConn.SelectionBackColor = Color.DarkBlue;
                    RichTextServerConn.SelectionColor = Color.Yellow;
                    RichTextServerConn.SelectedText = GenericStatic.Time() + " ";
                    RichTextServerConn.SelectedText = clientName + @" :" + "sent a photo to " + clientNamePrivate;
                    RichTextServerConn.SelectedText = Environment.NewLine;
                    _CursorPositionConn = RichTextServerConn.SelectionStart;
                })));
                return;
            }
            _FrmServerImages.NewImage(img, clientName);
            Invoke(new Action((delegate{
                // The inner server messages board
                RichTextServerConn.SelectionStart = _CursorPositionConn;
                RichTextServerConn.SelectionBackColor = Color.DarkBlue;
                RichTextServerConn.SelectionColor = Color.Yellow;
                RichTextServerConn.SelectedText = GenericStatic.Time() + " ";
                RichTextServerConn.SelectedText = clientName + @" :" + "sent a photo";
                RichTextServerConn.SelectedText = Environment.NewLine;
                _CursorPositionConn = RichTextServerConn.SelectionStart;
            })));
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

        //Method to which creates a new class of TabPagePrivateChatServer and adds it to TabControlServer
        private void NewTabPagePrivateChatServer(string tabName0, string tabName1) {
            TabPagePrivateChatServer newPrivateTab = new TabPagePrivateChatServer(tabName0, tabName1);
            TabControlServer.TabPages.Add(newPrivateTab);
            TabPagePrivateServerDoActionEvent += newPrivateTab.TabPagePrivateReceiveMessageServerDoAction;
        }

        //Button send
        private void BtnServerSnd_Click(object sender, EventArgs e) {
            FrmServerServerMessageEvent?.Invoke(TxtBxServer.Text);
            //Automaticlaly scrolldown richTxtChatBox
            _CursorPositionPub = RichTextServerPub.SelectionStart;
            RichTextServerPub.SelectionColor = Color.Black;
            RichTextServerPub.SelectionBackColor = Color.MediumPurple;
            RichTextServerPub.SelectedText = TxtBxServer.Text + Environment.NewLine;
            RichTextServerPub.SelectionStart = _CursorPositionPub;
            TxtBxServer.Text = "";
        }

        private void button1_Click(object sender, EventArgs e) {
            _FrmServerImages.Visible = true;
        }

        private void FrmServer_FormClosing(object sender, FormClosingEventArgs e) {
            if (!_IsServerRunning) {
                return;
            }
            e.Cancel = true;
            MessageBox.Show(this, @"Stop the server first");
        }
    }
}