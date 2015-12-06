using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryMeowChat;
using MeowChatClientLibrary;

namespace MeowChatClient {
    public partial class FrmStatistics: Form {
        private readonly Timer _Timer = new Timer();

        public FrmStatistics() {
            InitializeComponent();
            _Timer.Interval = 1000;
            _Timer.Enabled = true;
            _Timer.Tick += new EventHandler(Timer_tick);
            _Timer.Start();
            LblTime.Text = ClientStatistics.ConnectedTime;
        }

        private void FrmStatistics_Load(object sender, EventArgs e) {
        }


        private void Timer_tick(object sender, EventArgs e) {
            LblTime.Text = ClientStatistics.ConnectedTime;
            LblTime.Update();
        }

        private void FrmStatistics_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Hide();
        }
    }
}