using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MeowChatClientLibrary;
using Timer = System.Windows.Forms.Timer;

namespace MeowChatClient {
    public partial class Statistic: Form {
        private readonly Timer _Timer = new Timer();

        public Statistic() {
            InitializeComponent();
            _Timer.Interval = 1000;
            _Timer.Enabled = true;
            _Timer.Tick += new EventHandler(Timer_tick);
            _Timer.Start();
        }

        public void Start() {
            LblMessagSent.Text = ClientStatistics.MessagesSent.ToString();
            LblMessagReceived.Text = ClientStatistics.MessagesReceived.ToString();
            LblPrivateMessagSent.Text = ClientStatistics.MessagesPrivateSent.ToString();
            LblPrivateMessagReceived.Text = ClientStatistics.MessagesPrivateReceived.ToString();
            LblTotalMessagSent.Text = (ClientStatistics.MessagesSent + ClientStatistics.MessagesPrivateSent).ToString();
            LblTotalMessagReceived.Text = (ClientStatistics.MessagesReceived + ClientStatistics.MessagesPrivateReceived).ToString();
            LblServerMessages.Text = ClientStatistics.ServerMessage.ToString();
        }


        private void Timer_tick(object sender, EventArgs e) {
            LblTime.Text = ClientStatistics.ConnectedTime;
            LblTime.Update();
        }

        private void FrmStatistics_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
        }

        public void UpdateStatics(StatisticsEntry staticsEntry) {
            switch (staticsEntry) {
                case StatisticsEntry.MessageSent:
                    if (LblMessagSent.InvokeRequired) {
                        Invoke(new MethodInvoker(delegate{
                            LblMessagSent.Text = ClientStatistics.MessagesSent.ToString();
                        }));
                    }
                    else {
                        LblMessagSent.Text = ClientStatistics.MessagesSent.ToString();
                    }
                    break;
                case StatisticsEntry.MessageReceied:
                    if (LblMessagSent.InvokeRequired) {
                        Invoke(new MethodInvoker(delegate{
                            LblMessagReceived.Text = ClientStatistics.MessagesReceived.ToString();
                        }));
                    }
                    else {
                        LblMessagReceived.Text = ClientStatistics.MessagesReceived.ToString();
                    }
                    break;
                case StatisticsEntry.MessagePrivateSent:
                    if (LblMessagSent.InvokeRequired) {
                        Invoke(new MethodInvoker(delegate{
                            LblPrivateMessagSent.Text = ClientStatistics.MessagesPrivateSent.ToString();
                        }));
                    }
                    else {
                        LblPrivateMessagSent.Text = ClientStatistics.MessagesPrivateSent.ToString();
                    }
                    break;
                case StatisticsEntry.MessagePrivateReceived:
                    if (LblMessagSent.InvokeRequired) {
                        Invoke(new MethodInvoker(delegate{
                            LblPrivateMessagReceived.Text = ClientStatistics.MessagesPrivateReceived.ToString();
                        }));
                    }
                    else {
                        LblPrivateMessagReceived.Text = ClientStatistics.MessagesPrivateReceived.ToString();
                    }
                    break;
                case StatisticsEntry.ServerMessage:
                    if (LblServerMessages.InvokeRequired) {
                        Invoke(new MethodInvoker(delegate{
                            LblServerMessages.Text = ClientStatistics.ServerMessage.ToString();
                        }));
                    }
                    else {
                        LblServerMessages.Text = ClientStatistics.ServerMessage.ToString();
                    }
                    break;
            }
            if (LblMessagSent.InvokeRequired) {
                Invoke(new MethodInvoker(delegate{
                    LblTotalMessagSent.Text = (ClientStatistics.MessagesSent + ClientStatistics.MessagesPrivateSent).ToString();
                    LblTotalMessagReceived.Text = (ClientStatistics.MessagesReceived + ClientStatistics.MessagesPrivateReceived).ToString();
                }));
            }
            else {
                LblTotalMessagSent.Text = (ClientStatistics.MessagesSent + ClientStatistics.MessagesPrivateSent).ToString();
                LblTotalMessagReceived.Text = (ClientStatistics.MessagesReceived + ClientStatistics.MessagesPrivateReceived).ToString();
            }
        }
    }
}