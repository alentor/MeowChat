using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MeowChatClient {
    public partial class FrmImage: Form {
        public byte[] Imgbyte;

        public FrmImage() {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
        }

        private void FrmImage_Load(object sender, EventArgs e) {
            //MemoryStream ms = new MemoryStream(Imgbyte);
            //Image img = Image.FromStream(ms);
            //pictureBox1.Image = img;
        }

        public void NewImage(byte[] imgByte) {
            PictureBox newPictureBox = new PictureBox();
            newPictureBox.Location = new System.Drawing.Point(6, 6);
            newPictureBox.Name = "pictureBox1";
            newPictureBox.Size = new System.Drawing.Size(390, 336);
            newPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            newPictureBox.TabIndex = 0;
            newPictureBox.TabStop = false;
            MemoryStream ms = new MemoryStream(imgByte);
            Image newImage = Image.FromStream(ms);
            newPictureBox.Image = newImage;
            TabPage newTabPage = new TabPage();
            newTabPage.Controls.Add(newPictureBox);
            newTabPage.Location = new System.Drawing.Point(4, 22);
            newTabPage.Name = tabControl1.TabCount.ToString();
            newTabPage.Padding = new System.Windows.Forms.Padding(3);
            newTabPage.Size = new System.Drawing.Size(402, 349);
            newTabPage.TabIndex = 0;
            newTabPage.Text = tabControl1.TabCount.ToString();
            newTabPage.UseVisualStyleBackColor = true;
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate{
                    tabControl1.TabPages.Add(newTabPage);
                }));
            }
            else {
                tabControl1.TabPages.Add(newTabPage);
            }


            //tabControl1.
        }

        private void FrmImage_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
        }
    }
}