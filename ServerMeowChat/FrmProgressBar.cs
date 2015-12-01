using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeowChatServerLibrary;

namespace MeowChatServer {
    public partial class FrmProgressBar: Form {
        public FrmProgressBar() {
            InitializeComponent();
        }


        public void UpdateProgressBar(int sections, List <Client> clientList) {
            //if (sections == 0) {
            //    return;
            //}

            if (InvokeRequired) {
                BeginInvoke(new Action(() => ProgressBar1.Maximum = clientList.Count - 1));
            }
            else {
                ProgressBar1.Maximum = clientList.Count;
            }
            if (InvokeRequired) {
                BeginInvoke(new Action(() => ProgressBar1.Value = sections));
            }
            else {
                ProgressBar1.Value = sections;
            }
            //Invoke(new Action((delegate{
            //    ProgressBar1.Maximum = clientList.Count;
            //    ProgressBar1.Value = sections;
            //})));
        }
    }
}