using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;


namespace MeowChatClientLibrary {
    public enum StatisticsEntry {
        MessageSent,
        MessageReceied,
        MessagePrivateSent,
        MessagePrivateReceived,
        ServerMessage
    }

    public static class ClientStatistics {
        private static readonly Stopwatch sr_stopWatchConnectedTime = new Stopwatch();
        private static readonly Timer sr_timerConnectedTime = new Timer();
        public static int MessagesSent;
        public static int MessagesReceived;
        public static int MessagesPrivateSent;
        public static int MessagesPrivateReceived;
        public static int ServerMessage;
        public static string ConnectedTime;

        //public 
        public static void StartStatistics() {
            sr_stopWatchConnectedTime.Reset();
            sr_timerConnectedTime.Enabled = true;
            sr_timerConnectedTime.Tick += new EventHandler(sr_timerConnectedTime_tick);
            sr_stopWatchConnectedTime.Start();
            MessagesSent = 0;
            MessagesReceived = 0;
            MessagesPrivateSent = 0;
            MessagesPrivateReceived = 0;
            ServerMessage = 0;
        }

        private static void sr_timerConnectedTime_tick(object sender, EventArgs e) {
            ConnectedTime = sr_stopWatchConnectedTime.Elapsed.Hours.ToString("00") + @":" + sr_stopWatchConnectedTime.Elapsed.Minutes.ToString("00") + @":" + sr_stopWatchConnectedTime.Elapsed.Seconds.ToString("00");
        }

        public static void StopStatistics() {
            sr_stopWatchConnectedTime.Stop();
        }
    }
}