using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibraryMeowChat;
using MeowChatClientLibrary;

namespace MeowChatClient {
    public partial class FrmImage: Form {
        private byte[] Imgbyte;
        private Image _Img;
        private int _ImageCouter;

        public FrmImage() {
            InitializeComponent();
            Text = ClientConnection.ClientName + @" Received Images";
        }

        public void NewImage(byte[] imgByte, string tabName) {
            ++_ImageCouter;
            Imgbyte = imgByte;
            PictureBox newPictureBox = new PictureBox();
            newPictureBox.Location = new System.Drawing.Point(6, 6);
            newPictureBox.Name = "newPictureBox";
            newPictureBox.Size = new System.Drawing.Size(390, 336);
            newPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            newPictureBox.TabIndex = 0;
            newPictureBox.TabStop = false;
            newPictureBox.Click += new System.EventHandler(this.pictureBox1_Click);
            MemoryStream ms = new MemoryStream(imgByte);
            Image newImage = Image.FromStream(ms);
            newPictureBox.Image = newImage;
            Button BtnSave = new Button();
            BtnSave.Location = new System.Drawing.Point(147, 345);
            BtnSave.Name = "saveButton";
            BtnSave.Size = new System.Drawing.Size(75, 23);
            BtnSave.TabIndex = 2;
            BtnSave.Text = @"&Save";
            BtnSave.UseVisualStyleBackColor = true;
            BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            TabPage newTabPage = new TabPage();
            newTabPage.Controls.Add(newPictureBox);
            newTabPage.Controls.Add(BtnSave);
            newTabPage.Location = new System.Drawing.Point(4, 22);
            newTabPage.Name = tabControlmages.TabCount.ToString();
            newTabPage.Padding = new System.Windows.Forms.Padding(3);
            newTabPage.Size = new System.Drawing.Size(402, 349);
            newTabPage.TabIndex = 0;
            newTabPage.Text = tabName + @" " + _ImageCouter;
            newTabPage.UseVisualStyleBackColor = true;
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate{
                    tabControlmages.TabPages.Add(newTabPage);
                }));
            }
            else {
                tabControlmages.TabPages.Add(newTabPage);
            }
        }

        private void FrmImage_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
        }

        private void BtnSave_Click(object sender, EventArgs e) {
            if (Imgbyte != null) {
                _Img = byteArrayToImage(Imgbyte);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = @"Images|*.png;";
                //saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg*.gif*";

                //ImageFormat format = ImageFormat.Png;
                if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    _Img.Save(saveFileDialog.FileName, ImageFormat.Png);
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) {
            BtnSave_Click(this, null);
        }

        private static Image byteArrayToImage(byte[] byteArrayIn) {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }
}