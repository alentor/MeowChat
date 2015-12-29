using LibraryMeowChat;
using MeowChatClientLibrary;
using System;
using System.Windows.Forms;

namespace MeowChatClient {
    public partial class FrmLogin: Form {
        public FrmLogin() {
            InitializeComponent();
            ClientNetworkEngine.ClientNetworkEngineLoggedinEvent += Loggedin;
            ClientNetworkEngine.ClientNetworkEngineAttemptLoginErrorEvent += AttemptLoginErrorError;
            ClientNetworkEngine.ClientNetworkEngineRegisterMessageEvent += RegisterMessage;
        }

        // Checks if Name/IP/Port fileds are filed with at least one charter
        private void txtBoxName_TextChanged(object sender, EventArgs e) {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxName.Text.Length > 0 && txtBoxPort.Text.Length > 0) {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        // Checks if Name/IP/Port fileds are filed with at least one charter
        private void txtBxServerIp_TextChanged(object sender, EventArgs e) {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxName.Text.Length > 0 && txtBoxPort.Text.Length > 0) {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        // Checks if Name/IP/Port fileds are filed with at least one charter
        private void txtBoxPort_TextChanged(object sender, EventArgs e) {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxName.Text.Length > 0 && txtBoxPort.Text.Length > 0) {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        // Button connect
        private void btnConnect_Click(object sender, EventArgs e) {
            btnConnect.Enabled = false;
            btnColorPick.Enabled = false;
            txtBoxPort.Enabled = false;
            txtBoxName.Enabled = false;
            txtBoxServerIp.Enabled = false;
            ClientNetworkEngine.AttemptConnect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxName.Text, txtBoxUserName.Text, Client.Color);
            //ClientConnection.Connect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxName.Text);
        }

        // Register button
        private void btnRegister_Click(object sender, EventArgs e) {
            ClientNetworkEngine.Register(txtBoxRegUserName.Text, txtBoxServerIp.Text, int.Parse(txtBoxPort.Text));
            btnConnect.Enabled = false;
            btnColorPick.Enabled = false;
            txtBoxPort.Enabled = false;
            txtBoxName.Enabled = false;
            txtBoxServerIp.Enabled = false;
        }

        // Button Cancel
        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
            DialogResult = DialogResult.Cancel;
            ClientNetworkEngine.Disconnect();
        }

        // Logged will be invoked from ClientNetworkServer on a successful Login
        private void Loggedin() {
            DialogResult = DialogResult.OK;
            //ClientNetworkEngine.ClientNetworkEngineRegisterMessageEvent -= RegisterMessage;
            //if (Visible) {
            //    Invoke(new Action(Close));
            //}
        }

        // Login error
        private void AttemptLoginErrorError(string errorMessage) {
            Invoke(new Action(delegate{
                btnConnect.Enabled = true;
                btnColorPick.Enabled = true;
                txtBoxPort.Enabled = true;
                txtBoxName.Enabled = true;
                txtBoxServerIp.Enabled = true;
            }));
            MessageBox.Show(errorMessage, @"Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Register message
        private void RegisterMessage(string errorMessage) {
            if (IsDisposed) {
                return;
            }
            Invoke(new Action(delegate{
                btnConnect.Enabled = true;
                btnColorPick.Enabled = true;
                txtBoxPort.Enabled = true;
                txtBoxName.Enabled = true;
                txtBoxServerIp.Enabled = true;
            }));
            MessageBox.Show(errorMessage, @"Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // Color pick Button
        private void btnColorPick_Click(object sender, EventArgs e) {
            DialogResult pickColor = colorPicker.ShowDialog();
            if (pickColor == DialogResult.OK) {
                //string color = GenericStatic.HexConverter(colorPicker.Color);
                //ClientConnection.Color = color;
                Client.Color = colorPicker.Color;
                //MessageBox.Show(str, @"Chat: " + ClientConnection.FrmPrivateName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}