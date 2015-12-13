using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

//using MeowChatClientLibrary;

namespace MeowChatServer {
    public partial class FrmServerImages: Form {
        private byte[] Imgbyte;
        private Image _Img;
        private int _ImageCouter;

        public FrmServerImages() {
            InitializeComponent();
        }

        public void NewImage(byte[] imgByte, string tabName) {
            ++_ImageCouter;
            Imgbyte = imgByte;
            PictureBox newPictureBox = new PictureBox {
                Location = new System.Drawing.Point(6, 6),
                Name = "newPictureBox",
                Size = new System.Drawing.Size(390, 336),
                SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom,
                TabIndex = 0,
                TabStop = false
            };
            newPictureBox.Click += new System.EventHandler(this.pictureBox1_Click);
            MemoryStream ms = new MemoryStream(imgByte);
            Image newImage = Image.FromStream(ms);
            newPictureBox.Image = newImage;
            Button btnSave = new Button {
                Location = new System.Drawing.Point(147, 345),
                Name = "saveButton",
                Size = new System.Drawing.Size(75, 23),
                TabIndex = 2,
                Text = @"&Save",
                UseVisualStyleBackColor = true
            };
            btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            TabPage newTabPage = new TabPage();
            newTabPage.Controls.Add(newPictureBox);
            newTabPage.Controls.Add(btnSave);
            newTabPage.Location = new System.Drawing.Point(4, 22);
            newTabPage.Text = tabName;
            newTabPage.Name = tabName;
            newTabPage.Padding = new System.Windows.Forms.Padding(3);
            newTabPage.Size = new System.Drawing.Size(402, 349);
            newTabPage.TabIndex = 0;
            newTabPage.UseVisualStyleBackColor = true;
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate{
                    TabControlServerImages.TabPages.Add(newTabPage);
                }));
            }
            else {
                TabControlServerImages.TabPages.Add(newTabPage);
            }
        }

        private void FrmImage_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
        }

        private void BtnSave_Click(object sender, EventArgs e) {
            if (Imgbyte != null) {
                _Img = ByteArrayToImage(Imgbyte);
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

        private static Image ByteArrayToImage(byte[] byteArrayIn) {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public void ChangeTabName(string tabName, string newTabName) {
            foreach (TabPage tabPage in TabControlServerImages.TabPages) {
                if (tabPage.Name == tabName) {
                    if (tabPage.InvokeRequired) {
                        Invoke(new MethodInvoker(delegate{
                            tabPage.Name = newTabName;
                            tabPage.Text = tabPage.Name;
                        }));
                    }
                    else {
                        tabPage.Name = newTabName;
                        tabPage.Text = tabPage.Name;
                    }
                }
                if (tabPage.Name != tabName + " Private") {
                    continue;
                }

                if (tabPage.InvokeRequired) {
                    Invoke(new MethodInvoker(delegate{
                        tabPage.Name = newTabName + " Private";
                        tabPage.Text = tabPage.Name;
                    }));
                }
                else {
                    tabPage.Name = newTabName + " Private";
                    tabPage.Text = tabPage.Name;
                }
            }
        }
    }
}