using MeowChatClientLibrary;
using System;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace MeowChatClient
{
    public partial class Statistic : Form
    {
        private readonly Timer _Timer = new Timer();

        public Statistic()
        {
            InitializeComponent();
            _Timer.Interval = 1000;
            _Timer.Enabled = true;
            _Timer.Tick += new EventHandler(Timer_tick);
            _Timer.Start();
        }

        public void Start()
        {
            LblMessagSent.Text = 0.ToString();
            LblImageSent.Text = 0.ToString();
            LblMessagReceived.Text = 0.ToString();
            LblImageReceived.Text = 0.ToString();
            LblPrivateMessagSent.Text = 0.ToString();
            LblPrivateImageSent.Text = 0.ToString();
            LblPrivateMessagReceived.Text = 0.ToString();
            LblPrivateImageReceived.Text = 0.ToString();
            LblTotalMessagSent.Text = 0.ToString();
            LblTotalMessagReceived.Text = 0.ToString();
            LblTotalImagesSent.Text = 0.ToString();
            LblTotalImagesReceived.Text = 0.ToString();
            LblServerMessages.Text = 0.ToString();
        }

        private void Timer_tick(object sender, EventArgs e)
        {
            LblTime.Text = ClientStatistics.ConnectedTime;
            LblTime.Update();
        }

        private void FrmStatistics_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Visible = false;
        }

        public void UpdateStatics(StatisticsEntry staticsEntry)
        {
            switch (staticsEntry)
            {
                case StatisticsEntry.MessageSent:
                    if (LblMessagSent.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblMessagSent.Text = ClientStatistics.MessagesSent.ToString();
                        }));
                    }
                    else
                    {
                        LblMessagSent.Text = ClientStatistics.MessagesSent.ToString();
                    }
                    break;

                case StatisticsEntry.MessageReceied:
                    if (LblMessagSent.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblMessagReceived.Text = ClientStatistics.MessagesReceived.ToString();
                        }));
                    }
                    else
                    {
                        LblMessagReceived.Text = ClientStatistics.MessagesReceived.ToString();
                    }
                    break;

                case StatisticsEntry.MessagePrivateSent:
                    if (LblMessagSent.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblPrivateMessagSent.Text = ClientStatistics.MessagesPrivateSent.ToString();
                        }));
                    }
                    else
                    {
                        LblPrivateMessagSent.Text = ClientStatistics.MessagesPrivateSent.ToString();
                    }
                    break;

                case StatisticsEntry.MessagePrivateReceived:
                    if (LblMessagSent.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblPrivateMessagReceived.Text = ClientStatistics.MessagesPrivateReceived.ToString();
                        }));
                    }
                    else
                    {
                        LblPrivateMessagReceived.Text = ClientStatistics.MessagesPrivateReceived.ToString();
                    }
                    break;

                case StatisticsEntry.ImagesSent:
                    if (LblImageSent.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblImageSent.Text = ClientStatistics.ImagesSent.ToString();
                        }));
                    }
                    else
                    {
                        LblImageSent.Text = ClientStatistics.ImagesSent.ToString();
                    }
                    break;

                case StatisticsEntry.ImagesReceived:
                    if (LblImageReceived.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblImageReceived.Text = ClientStatistics.ImagesReceived.ToString();
                        }));
                    }
                    else
                    {
                        LblImageReceived.Text = ClientStatistics.ImagesReceived.ToString();
                    }
                    break;

                case StatisticsEntry.ImagesPrivateSent:
                    if (LblPrivateImageSent.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblPrivateImageSent.Text = ClientStatistics.ImagesPrivateSent.ToString();
                        }));
                    }
                    else
                    {
                        LblPrivateImageSent.Text = ClientStatistics.ImagesPrivateSent.ToString();
                    }
                    break;

                case StatisticsEntry.ImagesPrivateReceived:
                    if (LblPrivateImageReceived.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblPrivateImageReceived.Text = ClientStatistics.ImagesPrivateReceived.ToString();
                        }));
                    }
                    else
                    {
                        LblPrivateImageReceived.Text = ClientStatistics.ImagesPrivateReceived.ToString();
                    }
                    break;

                case StatisticsEntry.ServerMessage:
                    if (LblServerMessages.InvokeRequired)
                    {
                        Invoke(new MethodInvoker(delegate
                        {
                            LblServerMessages.Text = ClientStatistics.ServerMessage.ToString();
                        }));
                    }
                    else
                    {
                        LblServerMessages.Text = ClientStatistics.ServerMessage.ToString();
                    }
                    break;
            }
            if (LblMessagSent.InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate
                {
                    LblTotalMessagSent.Text = (ClientStatistics.MessagesSent + ClientStatistics.MessagesPrivateSent).ToString();
                    LblTotalMessagReceived.Text = (ClientStatistics.MessagesReceived + ClientStatistics.MessagesPrivateReceived).ToString();
                    LblTotalImagesSent.Text = (ClientStatistics.ImagesPrivateSent + ClientStatistics.ImagesSent).ToString();
                    LblTotalImagesReceived.Text = (ClientStatistics.ImagesPrivateReceived + ClientStatistics.ImagesReceived).ToString();
                }));
            }
            else
            {
                LblTotalMessagSent.Text = (ClientStatistics.MessagesSent + ClientStatistics.MessagesPrivateSent).ToString();
                LblTotalMessagReceived.Text = (ClientStatistics.MessagesReceived + ClientStatistics.MessagesPrivateReceived).ToString();
                LblTotalImagesSent.Text = (ClientStatistics.ImagesPrivateSent + ClientStatistics.ImagesSent).ToString();
                LblTotalImagesReceived.Text = (ClientStatistics.ImagesPrivateReceived + ClientStatistics.ImagesReceived).ToString();
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Visible = false;
        }
    }
}