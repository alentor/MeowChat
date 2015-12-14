using LibraryMeowChat;
using MeowChatClientLibrary;
using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace MeowChatClient {
    public partial class FrmLogin: Form {
        private byte[] _ByteMessage;
        private MessageStructure msgToSend;
        //Stores clients established connection, this will happen when the FrmLogin will be closed
        public FrmLogin() {
            InitializeComponent();
            ClientConnection.LoginFrmCloseEvent += LoggedIn;
        }

        //Check if Name/IP/Port fileds are filed with at least one charter
        private void txtBoxName_TextChanged(object sender, EventArgs e) {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxUserName.Text.Length > 0 && txtBoxPort.Text.Length > 0) {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        //Check if Name/IP/Port fileds are filed with at least one charter
        private void txtBxServerIp_TextChanged(object sender, EventArgs e) {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxUserName.Text.Length > 0 && txtBoxPort.Text.Length > 0) {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        //Check if Name/IP/Port fileds are filed with at least one charter
        private void txtBoxPort_TextChanged(object sender, EventArgs e) {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxUserName.Text.Length > 0 && txtBoxPort.Text.Length > 0) {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        //Button connect
        private void btnConnect_Click(object sender, EventArgs e) {
            msgToSend = new MessageStructure {
                MessageType = MessageType.AttempLogin,
                ClientName = txtBoxUserName.Text,
                Message = null
            };
            AttempConnect();
            //ClientConnection.Connect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxName.Text);
        }

        private void AttempConnect() {
            try {
                IPAddress ipAdressText = IPAddress.Parse(txtBoxServerIp.Text);
                ClientConnection.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEndPoint = new IPEndPoint(ipAdressText, int.Parse(txtBoxPort.Text));
                ClientConnection.Socket.BeginConnect(ipEndPoint, AttemptLogin, null);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> ListBoxClientList_DoubleClick", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Button Cancel
        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
        }

        //LoggedIn, will run on a successful Connect
        private void LoggedIn() {
            if (Visible) {
                Invoke((MethodInvoker) Close);
            }
        }

        //Color pick Button
        private void btnColorPick_Click(object sender, EventArgs e) {
            DialogResult pickColor = colorPicker.ShowDialog();
            if (pickColor == DialogResult.OK) {
                string color = GenericStatic.HexConverter(colorPicker.Color);
                ClientConnection.Color = color;
                //MessageBox.Show(str, @"Chat: " + ClientConnection.FrmPrivateName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AttemptLogin(IAsyncResult ar) {
            ClientConnection.Socket.EndConnect(ar);

            byte[] msgToSendByte = msgToSend.ToByte();
            //send the login credinails of the established connection to the server and call to the methood OnSend
            ClientConnection.Socket.BeginSend(msgToSendByte, 0, msgToSendByte.Length, SocketFlags.None, OnSend, null);
            _ByteMessage = new byte[1024];
            ClientConnection.Socket.BeginReceive(_ByteMessage, 0, _ByteMessage.Length, SocketFlags.None, OnReceive, null);
        }

        private static void OnSend(IAsyncResult ar) {
            try {
                ClientConnection.Socket.EndSend(ar);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message + @" -> OnSend", @"Chat: " + ClientConnection.ClientName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnReceive(IAsyncResult ar) {
            ClientConnection.Socket.EndReceive(ar);
            MessageStructure msgReceived = new MessageStructure(_ByteMessage);
            _ByteMessage = new byte[1024];
            switch (msgReceived.MessageType) {
                case MessageType.AttempLogin:
                    if (msgReceived.Message != "This user name already logged in") {
                        ClientConnection.Connect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxUserName.Text);
                        break;
                    }
                    MessageBox.Show(this, msgReceived.Message);
                    break;

                case MessageType.Regiter:
                    MessageBox.Show(this, msgReceived.Message);
                    break;
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e) {
            msgToSend = new MessageStructure {
                MessageType = MessageType.Regiter,
                ClientName = txtBoxNewUserName.Text,
                Message = txtBoxNewName.Text
            };
            AttempConnect();
        }
    }
}