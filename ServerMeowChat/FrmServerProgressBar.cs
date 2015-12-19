using MeowChatServerLibrary;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MeowChatServer {
    public partial class FrmServerProgressBar: Form {
        //private readonly List <Client> _InternalClientList;
        private readonly int TotalConnectedClients;

        //Default Constractor
        //public FrmServerProgressBar(List <Client> internalClientList) {
        //    _InternalClientList = internalClientList;
        //    InitializeComponent();
        //    if (_InternalClientList.Count == 0) {
        //        return;
        //    }
        //    ProgressBar1.Maximum = _InternalClientList.Count - 1;
        //}

        public FrmServerProgressBar(int totalConnectedClients) {
            TotalConnectedClients = totalConnectedClients;
            InitializeComponent();
            if (TotalConnectedClients == 0) {
                return;
            }
            ProgressBar1.Maximum = TotalConnectedClients;
        }


        public void UpdateProgressBar(string currentDisconnectintClientName) {
            //if (sections < _InternalClientList.Count - 1) {
            //    ++sections;
            //}
            //if (sections < TotalConnectedClients) {
            //    ++sections;
            //}
            if (InvokeRequired) {
                Invoke(new Action((delegate{
                    ++ProgressBar1.Value;
                })));
                Invoke(new Action((delegate
                {
                    LblDisconnecting.Text = @"Disconnecting " + currentDisconnectintClientName+ @" " + ProgressBar1.Value + @" of " + ProgressBar1.Maximum;
                })));
                Invoke(new Action((delegate{
                    ProgressBar1.Update();
                })));
            }
            else {
                ++ProgressBar1.Value;
            }
        }
    }
}