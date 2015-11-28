using System;
using System.Windows.Forms;

namespace ChatApplcationV1Client
{
    public partial class ChangeName : Form
    {
        public string NameNew { get; set; }
        private readonly string _NameOld;

        private void ChangeName_Load(object sender, EventArgs e)
        {
            Text = @"Change FrmPrivateName";
        }

        public ChangeName(string name)
        {
            InitializeComponent();
            _NameOld = name;
            txtNewName.Select();
        }

        //Button OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtNewName.Text.Length > 0)
            {
                if (txtNewName.Text != _NameOld)
                {
                    NameNew = txtNewName.Text;
                    DialogResult = DialogResult.OK;
                    Hide();
                }
            }
        }

        //Button Cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}