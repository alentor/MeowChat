using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;


namespace MeowChatClientLibrary {
    public static class ClientStatistics {
        private static readonly Stopwatch sr_stopWatchConnectedTime = new Stopwatch();
        private static readonly Timer sr_timerConnectedTime = new Timer();
        public static string ConnectedTime;
        //public 
        public static void StartCountConnectedTime() {
            //Thread timerThread = new Thread(new ThreadStart(() => {
               
            //}));
            //timerThread.Start();
            sr_timerConnectedTime.Enabled = true;
            sr_timerConnectedTime.Tick += new EventHandler(sr_timerConnectedTime_tick);
            sr_stopWatchConnectedTime.Start();

        }

        private static void sr_timerConnectedTime_tick(object sender, EventArgs e) {
            ConnectedTime = sr_stopWatchConnectedTime.Elapsed.Hours.ToString("00") + @":" + sr_stopWatchConnectedTime.Elapsed.Minutes.ToString("00") + @":" + sr_stopWatchConnectedTime.Elapsed.Seconds.ToString("00");
        }

        public static void StopCountConnectedTime() {
            sr_stopWatchConnectedTime.Stop();
            sr_stopWatchConnectedTime.Reset();
        }
    }
}