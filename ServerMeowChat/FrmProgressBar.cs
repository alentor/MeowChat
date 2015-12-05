using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeowChatServerLibrary;

namespace MeowChatServer {
    public partial class FrmProgressBar: Form {
        private readonly List <Client> _InternalClientList;

        public FrmProgressBar(List <Client> internalClientList) {
            _InternalClientList = internalClientList;
            InitializeComponent();
            if (_InternalClientList.Count == 0) {
                return;
            }
            ProgressBar1.Maximum = _InternalClientList.Count - 1;
        }

        public void UpdateProgressBar(int sections) {
            if (sections < _InternalClientList.Count - 1) {
                ++sections;
            }
            if (InvokeRequired) {
                BeginInvoke(new Action(() => ProgressBar1.Value = sections));
                BeginInvoke(new Action(() => LblDisconnecting.Text = @"Disconnecting " + _InternalClientList[sections].ClientName));
                BeginInvoke(new Action(() => LblDisconnecting.Update()));
            }
            else {
                ProgressBar1.Value = sections;
            }
        }
    }
}