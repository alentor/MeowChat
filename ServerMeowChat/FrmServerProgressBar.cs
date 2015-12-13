using MeowChatServerLibrary;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MeowChatServer
{
    public partial class FrmServerProgressBar : Form
    {
        private readonly List<Client> _InternalClientList;

        public FrmServerProgressBar(List<Client> internalClientList)
        {
            _InternalClientList = internalClientList;
            InitializeComponent();
            if (_InternalClientList.Count == 0)
            {
                return;
            }
            ProgressBar1.Maximum = _InternalClientList.Count - 1;
        }

        public void UpdateProgressBar(int sections)
        {
            if (sections < _InternalClientList.Count - 1)
            {
                ++sections;
            }
            if (InvokeRequired)
            {
                Invoke(new Action((delegate
                {
                    ProgressBar1.Value = sections;
                })));
                Invoke(new Action((delegate
                {
                    LblDisconnecting.Text = @"Disconnecting " + _InternalClientList[sections].ClientName + @" " + sections + @" of " + ProgressBar1.Maximum;
                })));
                Invoke(new Action((delegate
                {
                    ProgressBar1.Update();
                })));
            }
            else
            {
                ProgressBar1.Value = sections;
            }
        }
    }
}