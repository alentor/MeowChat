using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeowChatServerLibrary;

namespace MeowChatServer {
    public partial class FrmData: Form {
        public FrmData() {
            InitializeComponent();
            ServerDataEngine.ServerDataEngineRefreshClientsDbEvent += RefreshClientsDB;
            ServerDataEngine.ServerDateEngineRefreshMessagesDbEvent += RefreshMessagesDB;
        }

        private void FrmHisotry_Load(object sender, EventArgs e) {
            ServerDataEngine.StartDataServer();
            DataGridClientsDB.DataSource = ServerDataEngine.GetDataSet().Tables["ClientsDB"];
            DataGridMessagesDB.DataSource = ServerDataEngine.GetDataSet().Tables["MessagesDB"];
        }

        private void FrmHisotry_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
        }

        private void RefreshClientsDB() {
            Invoke(new Action((delegate{
                DataGridClientsDB.DataSource = ServerDataEngine.GetClientsDbTable();
            })));
        }

        private void RefreshMessagesDB() {
            Invoke(new Action((delegate{
                DataGridMessagesDB.DataSource = ServerDataEngine.GetMessagesDbTable();
            })));
        }

        private void FrmData_Shown(object sender, EventArgs e) {
            DataGridClientsDB.DataSource = ServerDataEngine.GetDataSet().Tables["ClientsDB"];
            DataGridMessagesDB.DataSource = ServerDataEngine.GetDataSet().Tables["MessagesDB"];
        }

        private void BtnRefresh_Click(object sender, EventArgs e) {
            DataGridClientsDB.DataSource = ServerDataEngine.GetDataSet().Tables["ClientsDB"];
            DataGridMessagesDB.DataSource = ServerDataEngine.GetDataSet().Tables["MessagesDB"];
        }

        private void BtnSrhID_Click(object sender, EventArgs e) {
            int num;
            if (!int.TryParse(TxtBoxSrhID.Text, out num)) {
                MessageBox.Show(@"Enter a number please");
                return;
            }
            DataGridClientsDB.DataSource = ServerDataEngine.SearchID(int.Parse(TxtBoxSrhID.Text));
        }

        private void BtnSrhUserName_Click(object sender, EventArgs e) {
            DataGridClientsDB.DataSource = ServerDataEngine.SearchUserName(TxtBoxSrhUserName.Text);
        }

        private void BtnSrhName_Click(object sender, EventArgs e) {
            DataGridClientsDB.DataSource = ServerDataEngine.SearchLoggedLastLogged(TxtBoxSrhLoggedLastLogged.Text);
        }

        private void BtnSrhKeyWord_Click(object sender, EventArgs e) {
            DataGridMessagesDB.DataSource = ServerDataEngine.SearchMessagesByKeyword(TxtBoxSrhKeyWord.Text);
        }

        private void BtnSrhDate_Click(object sender, EventArgs e) {
            DataGridMessagesDB.DataSource = ServerDataEngine.SearchByDate(TxtBoxSrhDate.Text);
        }

        private void BtnSrhPrivate_Click(object sender, EventArgs e) {
            DataGridMessagesDB.DataSource = ServerDataEngine.SearchByPrivate(TxtBoxPrivate.Text);
        }
    }
}