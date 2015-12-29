using LibraryMeowChat;
using MeowChatClientLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MeowChatClient
{
    public partial class FrmChat : Form
    {
        //List which stores all the colors of the Clients current connected
        private readonly List<ClientChatHistory> _ClientChatHistoryList = new List<ClientChatHistory>();

        private int _CursorPosition;
        private readonly Statistic _FrmStatistics = new Statistic();
        private readonly FrmClientImages _FrmClientImages = new FrmClientImages();

        //Fired when the client recieves a message with one of the following commands from the servers PrivateMessage/PrivateStarted and PrivateStopped
        public event TabPagePrivateChatReceiveClientHandler TabPagePrivateChatReceiveClientEvent;

        //Fired when statics window needs to be updated
        public event FrmStatisticsUpdateHandler FrmStatisticsUpdateEvent;

        public event FrmClientImagesChangeNameHandler FrmClientImagesChangeNameEvent;

        public FrmChat()
        {
            InitializeComponent();
            TextBoxPubMsg.Select();
            FrmStatisticsUpdateEvent += _FrmStatistics.UpdateStatics;
            FrmClientImagesChangeNameEvent += _FrmClientImages.ChangeTabName;
            ClientNetworkEngine.ClientNetworkEngineLoginEvent += Login;
            ClientNetworkEngine.ClientNetworkEngineClientsListEvent += ClientsList;
            ClientNetworkEngine.ClientNetworkEngineDisconnectEvent += Disconnect;
            ClientNetworkEngine.ClientNetworkEngineMessageEvent += Message;
            ClientNetworkEngine.ClientNetworkEngineLogoutEvent += Logout;
            ClientNetworkEngine.ClientNetworkEngineColorChangedEvent += ColorChanged;
            ClientNetworkEngine.ClientNetworkEnginePrivateChatStartEvent += PrivateStart;
            ClientNetworkEngine.ClientNetworkEnginePrivateMessageEvent += PrivateMessage;
            ClientNetworkEngine.ClientNetworkEnginePrivateStoppedEvent += PrivateStopped;
            ClientNetworkEngine.ClientNetworkEngineNameChangeEvent += NameChange;
            ClientNetworkEngine.ClientNetworkEngineServerMessageEvent += ServerMessage;
            ClientNetworkEngine.ClientNetworkEngineImageMessageEvent += ImageMessage;
        }

        private void FrmChat_Load(object sender, EventArgs e)
        {
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
            if (frmLogin.DialogResult == DialogResult.Cancel)
            {
                Close();
            }
            // Set window name
            Text = @"Chat: " + Client.Name;
            ClientStatistics.Start();
        }

        // Clients list
        private void ClientsList(string clientsList)
        {
            Invoke(new Action((delegate
            {
                //_ClientsColor.Add(new ClientChatProp(ClientConnection.ClientName)); //Add this Client to the ClientChatProp list
                ListBoxClientList.Items.AddRange(clientsList.Split(','));
                //remove the empty selection box in list view

                ListBoxClientList.Items.RemoveAt(ListBoxClientList.Items.Count - 1);
                // Add all the connected clients to ClientChatProp list
                foreach (object t in ListBoxClientList.Items)
                {
                    _ClientChatHistoryList.Add(new ClientChatHistory(t.ToString()));
                }
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectionColor = Color.Black;
                RichTextClientPub.SelectionBackColor = Color.GreenYellow;
                RichTextClientPub.SelectedText = @"<<< You have joined the chat successfully >>>" + Environment.NewLine;
                _CursorPosition = RichTextClientPub.SelectionStart;
            })));
        }

        // Login
        private void Login(string clientName, string message)
        {
            Invoke(new Action((delegate
            {
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectionColor = Color.Black;
                RichTextClientPub.SelectionBackColor = Color.LightGreen;
                ListBoxClientList.Items.Add(clientName);
                RichTextClientPub.SelectedText = Time.NowTime() + " " + message + Environment.NewLine;
                if (clientName != Client.Name)
                {
                    _ClientChatHistoryList.Add(new ClientChatHistory(clientName));
                }
                _CursorPosition = RichTextClientPub.SelectionStart;
            })));
        }

        // File => Statistics
        private void StaticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_FrmStatistics.Visible)
            {
                _FrmStatistics.BringToFront();
                return;
            }
            _FrmStatistics.Visible = true;
            //_FrmStatistics.Show();
        }

        // File => Reconnect
        private void ReconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClientNetworkEngine.Status)
            {
                return;
            }
            ClientNetworkEngine.Reconnect();
            Thread.Sleep(5);
            //FrmChat_Load(this, null);
            BtnPubSnd.Enabled = true;
            Text = @"Chat: " + Client.Name;
        }

        // File => Disconnect
        private void DisconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClientNetworkEngine.Status)
            {
                return;
            }
            foreach (TabPage tabPage in TabControlClient.TabPages)
            {
                TabPagePrivateChatReceiveClientEvent?.Invoke(tabPage.Name, null, null, TabPagePrivateChatClient.TabCommand.Disconnected);
            }
            ClientNetworkEngine.Disconnect();
            ClientStatistics.Stop();
            Thread.Sleep(50);
            BtnPubSnd.Enabled = false;
            Text = @"Chat: " + Client.Name + @"[Disconnected]";
            RichTextClientPub.SelectionStart = _CursorPosition;
            RichTextClientPub.SelectionColor = Color.Black;
            RichTextClientPub.SelectionBackColor = Color.Crimson;
            RichTextClientPub.SelectedText = @"You are disonnected now " + Environment.NewLine;
            _CursorPosition = RichTextClientPub.SelectionStart;
            ListBoxClientList.Items.Clear();
            _ClientChatHistoryList.Clear();
        }

        // File => Exit
        private void ClickExitToolStripMenuItem(object sender, EventArgs e)
        {
            Close();
        }

        // Form closing event
        private void FrmChat_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ClientNetworkEngine.Status)
            {
                return;
            }
            if (MessageBox.Show(@"Are you sure you want to exit?", @"Chat: " + Client.Name, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
            {
                return;
            }
            ClientNetworkEngine.Disconnect();
        }

        // Disconnect
        private void Disconnect()
        {
            ClientStatistics.Stop();
            Invoke(new Action((delegate
            {
                TabPagePrivateChatReceiveClientEvent?.Invoke(null, null, null, TabPagePrivateChatClient.TabCommand.Disconnect);
                BtnPubSnd.Enabled = false;
                ListBoxClientList.Items.Clear();
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectionColor = Color.Black;
                RichTextClientPub.SelectionBackColor = Color.Tomato;
                RichTextClientPub.SelectedText = Time.NowTime() + " Disconnected from the server" + Environment.NewLine;
                _CursorPosition = RichTextClientPub.SelectionStart;
            })));
        }

        // Chat => Change Name
        private void ChangeNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ClientNetworkEngine.Status)
            {
                return;
            }
            using (ChangeName changeName = new ChangeName(Client.Name))
            {
                if (changeName.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                if (ListBoxClientList.Items.Cast<object>().Any(item => changeName.NameNew == item.ToString()))
                {
                    MessageBox.Show(@"The name " + changeName.NameNew + @"already taken", @"Chat: 5" + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                ClientNetworkEngine.NameChange(changeName.NameNew);
            }
        }

        // Chat => Change color
        private void ChangeColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BtnColorPick_Click(this, null);
        }

        // Chat => Recieved Images
        private void receivedImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _FrmClientImages.Visible = true;
        }

        // Help => About
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmAbout about = new FrmAbout();
            about.Show();
        }

        // Public Chat Send button
        private void BtnSend_Click(object sender, EventArgs e)
        {
            if (TextBoxPubMsg.Text.Length <= 0)
            {
                return;
            }
            ClientNetworkEngine.SendMessage(TextBoxPubMsg.Text);
            // Reset the TextBoxPubMsg
            TextBoxPubMsg.Text = null;
            // Update statistics
            ++ClientStatistics.MessagesSent;
            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessageSent);
        }

        // Color Button
        private void BtnColorPick_Click(object sender, EventArgs e)
        {
            if (!ClientNetworkEngine.Status)
            {
                return;
            }
            DialogResult pickColor = ColorPicker.ShowDialog();
            if (pickColor != DialogResult.OK)
            {
                return;
            }
            ClientNetworkEngine.ChangeColor(ColorPicker.Color);
            Client.Color = ColorPicker.Color;
        }

        // Image Buttton
        private void BtnSendPhotoPublic_Click(object sender, EventArgs e)
        {
            if (!ClientNetworkEngine.Status)
            {
                return;
            }
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = @"Open ImageMessage";
                    openFileDialog.Filter = @"Images|*.png;*.bmp;*.jpg;*.gif*";
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    long fileSize = new FileInfo(openFileDialog.FileName).Length;
                    if (fileSize > 2097152)
                    {
                        MessageBox.Show(@"This file is too big, you can send files no larger than 3MB", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ClientNetworkEngine.SendImage(File.ReadAllBytes(openFileDialog.FileName));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> BtnColorPick_Click", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // List box Clients double click
        private void ListBoxClientList_DoubleClick(object sender, EventArgs e)
        {
            if (ListBoxClientList.SelectedItem.ToString() == Client.Name)
            {
                MessageBox.Show(@"You can't start a private chat with yourself", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (TabControlClient.TabPages.OfType<TabPagePrivateChatClient>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == ListBoxClientList.SelectedItem.ToString()))
            {
                MessageBox.Show(@"That private chat already opned", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ClientNetworkEngine.StartPrivateChat(ListBoxClientList.SelectedItem.ToString());
            NewTabPagePrivateChatClient(ListBoxClientList.SelectedItem.ToString());
            TabFormat.ItemEvenSize(TabControlClient);
        }

        // Method to which creates new class of TabPagePrivateChatClient and adds it to TabControlClient
        private void NewTabPagePrivateChatClient(string tabName)
        {
            //if (!ClientConnection.Status)
            //{
            //    return;
            //}
            TabPagePrivateChatClient newPrivateTab = new TabPagePrivateChatClient(tabName);
            TabPagePrivateChatReceiveClientEvent += newPrivateTab.TabPageTabPagePrivateReceiveMessageClient;
            newPrivateTab.TabPagePrivateChatSendClientEvent += TabPagePrivateChatSendClient;
            newPrivateTab.TabPagePrivateChatSendImageClietEvent += TabPagePrivateChatSendImageClient;
            TabControlClient.TabPages.Add(newPrivateTab);
        }

        // Send private message
        private void TabPagePrivateChatSendClient(string clientNamePrivate, string message)
        {
            ClientNetworkEngine.SendPrivateMessage(clientNamePrivate, message);
            ++ClientStatistics.MessagesPrivateSent;
            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateSent);
        }

        //Send private Photo method event
        private void TabPagePrivateChatSendImageClient(string namePrivate)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = @"Open ImageMessage";
                    openFileDialog.Filter = @"Images|*.png;*.bmp;*.jpg;*.gif*";
                    if (openFileDialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }
                    long fileSize = new FileInfo(openFileDialog.FileName).Length;
                    if (fileSize > 2097152)
                    {
                        MessageBox.Show(@"This file is too big, you can send files no larger than 3MB", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ClientNetworkEngine.SendImagePrivate(File.ReadAllBytes(openFileDialog.FileName), Client.Name, namePrivate);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @" -> BtnColorPick_Click", @"Chat: " + Client.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // TabControl DrawItem, used to the draw the X on each tab
        private void TabControlClient_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Draw the name of the tab
            e.Graphics.DrawString(TabControlClient.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 10, e.Bounds.Top + 7);
            for (int i = 1; i < TabControlClient.TabPages.Count; i++)
            {
                Rectangle tabRect = TabControlClient.GetTabRect(i);
                //Not active tab
                if (i != TabControlClient.SelectedIndex)
                {
                    //Rectangle r = TabControlClient.TabPages[i].Text;
                    using (Brush brush = new SolidBrush(Color.OrangeRed))
                    {
                        e.Graphics.FillRectangle(brush, tabRect.Right - 23, 6, 16, 16);
                    }
                    using (Pen pen = new Pen(Color.Black, 2))
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
                else {
                    //Rectangle r = TabControlClient.TabPages[i].Text;
                    //RectangleF tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                    using (Brush brush = new SolidBrush(Color.Silver))
                    {
                        e.Graphics.FillRectangle(brush, tabRect.Right - 23, 6, 16, 16);
                    }
                    using (Pen pen = new Pen(Color.Black, 2))
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

        // Click event on TabPage, checks whenever the click was in the X rectangle area
        private void TabControlClient_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 1; i < TabControlClient.TabPages.Count; i++)
            {
                Rectangle tabRect = TabControlClient.GetTabRect(i);
                //Getting the position of the "x" mark.

                //Rectangle tabXarea = new Rectangle(tabRect.Right - TabControlClient.TabPages[i].Text.Length, tabRect.Top, 9, 7);
                Rectangle closeXButtonArea = new Rectangle(tabRect.Right - 23, 6, 16, 16);
                //Rectangle closeButton = new Rectangle(tabRect.Right - 13, tabRect.Top + 6, 9, 7);
                if (!closeXButtonArea.Contains(e.Location))
                {
                    continue;
                }
                if (MessageBox.Show(@"Would you like to Close this Tab?", @"Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    continue;
                }
                ClientNetworkEngine.PrivateClose(TabControlClient.TabPages[i].Name);
                TabControlClient.TabPages.RemoveAt(i);
                break;
            }
        }

        // Logout
        private void Logout(string clientName, string message)
        {
            Invoke(new Action((delegate
            {
                ListBoxClientList.Items.Remove(clientName);
                for (int i = 0; i < _ClientChatHistoryList.Count; i++)
                {
                    if (_ClientChatHistoryList[i].Name == clientName)
                    {
                        _ClientChatHistoryList.Remove(_ClientChatHistoryList[i]);
                        TabPagePrivateChatReceiveClientEvent?.Invoke(clientName, null, null, TabPagePrivateChatClient.TabCommand.Disconnected);
                    }
                }
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectionColor = Color.Black;
                RichTextClientPub.SelectionBackColor = Color.Tomato;
                RichTextClientPub.SelectedText = Time.NowTime() + " " + message + Environment.NewLine;
                _CursorPosition = RichTextClientPub.SelectionStart;
            })));
        }

        // Message
        private void Message(string clientName, string message, Color color)
        {
            Invoke(new Action((delegate
            {
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectedText = Time.NowTime() + " ";
                int selectionStart = RichTextClientPub.SelectionStart;
                RichTextClientPub.SelectionColor = color;
                RichTextClientPub.SelectedText = clientName + ": " + message;
                RichTextClientPub.SelectedText = Environment.NewLine;

                _CursorPosition = RichTextClientPub.SelectionStart;
                foreach (ClientChatHistory clientChatHistory in _ClientChatHistoryList.Where(clientColor => clientColor.Name == clientName))
                {
                    int[] selectionArr = { selectionStart, RichTextClientPub.TextLength - selectionStart };
                    clientChatHistory.Messages.Add(selectionArr);
                }
            })));
            if (clientName != Client.Name)
            {
                ++ClientStatistics.MessagesReceived;
                FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessageReceied);
            }
        }

        // Name change
        private void NameChange(string clientName, string clientNameNew, Color color)
        {
            Invoke(new Action((delegate
            {
                int index = ListBoxClientList.FindString(clientName);
                ListBoxClientList.Items[index] = clientNameNew;

                foreach (ClientChatHistory clientColor in _ClientChatHistoryList.Where(ClientChatHistory => ClientChatHistory.Name == clientName))
                {
                    clientColor.Name = clientNameNew;
                }
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectionColor = Color.Black;
                RichTextClientPub.SelectionBackColor = Color.CornflowerBlue;
                RichTextClientPub.SelectedText = Time.NowTime() + " " + @"<<< " + clientName + @" have changed nickname to " + clientNameNew + @" >>>" + Environment.NewLine;
                _CursorPosition = RichTextClientPub.SelectionStart;
                if (Client.Name == clientNameNew)
                {
                    Text = @"Chat: " + clientNameNew;
                }
                TabPagePrivateChatReceiveClientEvent?.Invoke(clientName, null, clientNameNew, TabPagePrivateChatClient.TabCommand.NameChange);
                foreach (TabPage tabPage in TabControlClient.TabPages.Cast<TabPage>().Where(tabPage => tabPage.Name == clientName))
                {
                    tabPage.Name = clientNameNew;
                    tabPage.Text = clientNameNew;
                    TabControlClient.Invalidate();
                }
                TabFormat.ItemEvenSize(TabControlClient);
            })));
            Invoke(new Action((delegate
            {
                _FrmClientImages.Text = clientNameNew + @" Received Images";
            })));
            FrmClientImagesChangeNameEvent?.Invoke(clientName, clientNameNew);
            ColorChanged(clientName, color);
        }

        // Color changed
        private void ColorChanged(string clientName, Color color)
        {
            Invoke(new Action((delegate
            {
                foreach (int[] selectedText in _ClientChatHistoryList.Where(clientColor => clientColor.Name == clientName).SelectMany(clientColor => clientColor.Messages))
                {
                    RichTextClientPub.Select(selectedText[0], selectedText[1]);
                    RichTextClientPub.SelectionColor = color;
                }
            })));
        }

        // Private start
        private void PrivateStart(string clientNamePrivate)
        {
            if (TabControlClient.TabPages.Cast<TabPage>().Any(tabPagePrivateChat => tabPagePrivateChat.Name == clientNamePrivate))
            {
                TabPagePrivateChatReceiveClientEvent?.Invoke(clientNamePrivate, null, null, TabPagePrivateChatClient.TabCommand.Resumed);
                return;
            }
            Invoke(new Action((delegate
            {
                NewTabPagePrivateChatClient(clientNamePrivate);
                TabFormat.ItemEvenSize(TabControlClient);
            })));
        }

        // Private message
        private void PrivateMessage(string clientName, string clientNamePrivate, string message)
        {
            TabPagePrivateChatReceiveClientEvent?.Invoke(clientName, clientNamePrivate, message, TabPagePrivateChatClient.TabCommand.Message);
            if (Client.Name == clientNamePrivate)
            {
                ++ClientStatistics.MessagesPrivateReceived;
                FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateReceived);
                return;
            }
            ++ClientStatistics.MessagesPrivateSent;
            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.MessagePrivateSent);
        }

        // Private stopped
        private void PrivateStopped(string clientName)
        {
            TabPagePrivateChatReceiveClientEvent?.Invoke(clientName, null, null, TabPagePrivateChatClient.TabCommand.Closed);
        }

        // Server message
        private void ServerMessage(string message)
        {
            Invoke(new Action((delegate
            {
                RichTextClientPub.SelectionStart = _CursorPosition;
                RichTextClientPub.SelectionColor = Color.Black;
                RichTextClientPub.SelectionBackColor = Color.MediumPurple;
                RichTextClientPub.SelectedText = Time.NowTime() + " " + "Server Message: " + message + Environment.NewLine;
                _CursorPosition = RichTextClientPub.SelectionStart;
            })));
            ++ClientStatistics.ServerMessage;
            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ServerMessage);
        }

        // Image message
        private void ImageMessage(string clientName, string clientNamePrivate, Image img)
        {
            if (clientNamePrivate != null)
            {
                if (Client.Name == clientName)
                {
                    ++ClientStatistics.ImagesPrivateSent;
                    FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesPrivateSent);
                    // Let the user know that the private photo he sent went thro
                    TabPagePrivateChatReceiveClientEvent?.Invoke(clientName, null, null, TabPagePrivateChatClient.TabCommand.ImageMessageSent);
                    return;
                }
                // Show the photo to the receiving user
                ++ClientStatistics.ImagesPrivateReceived;
                FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesPrivateReceived);
                _FrmClientImages.NewImage(img, clientName + " Private");
                TabPagePrivateChatReceiveClientEvent?.Invoke(clientName, null, null, TabPagePrivateChatClient.TabCommand.ImageMessage);
                if (_FrmClientImages.Visible == false)
                {
                    if (InvokeRequired)
                    {
                        BeginInvoke(new MethodInvoker(delegate
                        {
                            _FrmClientImages.Visible = true;
                            _FrmClientImages.BringToFront();
                        }));
                    }
                    else {
                        _FrmClientImages.Visible = true;
                        _FrmClientImages.BringToFront();
                    }
                }
                return;
            }

            if (Client.Name == clientName)
            {
                ++ClientStatistics.ImagesSent;
                FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesSent);
                Invoke(new Action((delegate
                {
                    RichTextClientPub.SelectionStart = _CursorPosition;
                    RichTextClientPub.SelectionColor = Color.Black;
                    RichTextClientPub.SelectionBackColor = Color.Yellow;
                    RichTextClientPub.SelectedText = Time.NowTime() + " " + " ImageMessage sent successfully" + Environment.NewLine;
                    _CursorPosition = RichTextClientPub.SelectionStart;
                })));
                return;
            }
            ++ClientStatistics.ImagesReceived;
            FrmStatisticsUpdateEvent?.Invoke(StatisticsEntry.ImagesReceived);
            _FrmClientImages.NewImage(img, clientName);
            if (_FrmClientImages.Visible == false)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(delegate
                    {
                        _FrmClientImages.Visible = true;
                        _FrmClientImages.BringToFront();
                    }));
                }
                else {
                    _FrmClientImages.Visible = true;
                    _FrmClientImages.BringToFront();
                }
            }
        }

        // Automaticlaly scrolldown rchTxtPubChat
        private void RichTextChatBoxText_Changed(object sender, EventArgs e)
        {
            RichTextClientPub.SelectionStart = RichTextClientPub.Text.Length;
            RichTextClientPub.ScrollToCaret();
        }
    }
}