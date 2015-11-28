using ClientLibrary;
using System;
using System.Windows.Forms;

namespace ChatApplcationV1Client
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FrmLogin login = new FrmLogin();
            Application.Run(login);
            //if login dialog returns ok then it means we have estalished connection with the server
            //and can start to chat(recieve and send messages)
            //pass down the infromation of the established conneciton socket and name dow to the chatForm
            if (ClientConnection.Status)
            {
                FrmChat chat = new FrmChat();
                chat.ShowDialog();
            }
        }
    }
}