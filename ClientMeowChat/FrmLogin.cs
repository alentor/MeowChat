using LibraryMeowChat;
using MeowChatClientLibrary;
using System;
using System.Windows.Forms;

namespace MeowChatClient {
    public partial class FrmLogin: Form {

        public FrmLogin() {
            InitializeComponent();
            ClientNetworkEngine.ClientNetworkEngineLoggedinEvent += Loggedin;
            //ClientConnection.LoginFrmCloseEvent += Connected;
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
            ClientNetworkEngine.AttemptConnect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxName.Text, Client.Color);
            //ClientConnection.Connect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxName.Text);
        }

        //Button Cancel
        private void btnCancel_Click(object sender, EventArgs e) {
            Close();
            DialogResult = DialogResult.Cancel;

        }

        // Logged will be invoked from ClientNetworkServer on a successful Login
        private void Loggedin() {
            if (Visible) {
            Invoke(new Action(Close));
            }
        }

       

        //Color pick Button
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