using CommonLibrary;
using MeowChatClientLibrary;
using System;
using System.Windows.Forms;

namespace MeowChatClient
{
    public partial class FrmLogin : Form
    {
        //Stores clients established connection, this will happen when the FrmLogin will be closed
        public FrmLogin()
        {
            InitializeComponent();
            ClientConnection.LoginFrmCloseEvent += Connected;
        }

        //Check if Name/IP/Port fileds are filed with at least one charter
        private void txtBoxName_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxName.Text.Length > 0 && txtBoxPort.Text.Length > 0)
            {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        //Check if Name/IP/Port fileds are filed with at least one charter
        private void txtBxServerIp_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxName.Text.Length > 0 && txtBoxPort.Text.Length > 0)
            {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        //Check if Name/IP/Port fileds are filed with at least one charter
        private void txtBoxPort_TextChanged(object sender, EventArgs e)
        {
            if (txtBoxServerIp.Text.Length > 0 && txtBoxName.Text.Length > 0 && txtBoxPort.Text.Length > 0)
            {
                btnConnect.Enabled = true;
                return;
            }
            btnConnect.Enabled = false;
        }

        //Button connect
        private void btnConnect_Click(object sender, EventArgs e)
        {
            ClientConnection.Connect(txtBoxServerIp.Text, int.Parse(txtBoxPort.Text), txtBoxName.Text);
        }

        //Button Cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Connected, will run on a successful Connect
        private void Connected()
        {
            if (Visible)
            {
                Invoke((MethodInvoker)Close);
            }
        }

        //Color pick Button
        private void btnColorPick_Click(object sender, EventArgs e)
        {
            DialogResult pickColor = colorPicker.ShowDialog();
            if (pickColor == DialogResult.OK)
            {
                string color = ChatMethodsStatic.HexConverter(colorPicker.Color);
                ClientConnection.Color = color;
                //MessageBox.Show(str, @"Chat: " + ClientConnection.FrmPrivateName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}