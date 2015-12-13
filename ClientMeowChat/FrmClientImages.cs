using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MeowChatClientLibrary;

namespace MeowChatClient {
    public partial class FrmClientImages: Form {
        private Image _Img;
        private byte[] _Imgbyte;

        public FrmClientImages() {
            InitializeComponent();
            Text = ClientConnection.ClientName + @" Received Images";
        }

        public void NewImage(byte[] imgByte, string tabName) {
            _Imgbyte = imgByte;
            PictureBox newPictureBox = new PictureBox {
                Location = new Point(6, 6),
                Name = "newPictureBox",
                Size = new Size(390, 336),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabIndex = 0,
                TabStop = false
            };
            newPictureBox.Click += pictureBox1_Click;
            MemoryStream ms = new MemoryStream(imgByte);
            Image newImage = Image.FromStream(ms);
            newPictureBox.Image = newImage;
            Button btnSave = new Button {
                Location = new Point(147, 345),
                Name = "saveButton",
                Size = new Size(75, 23),
                TabIndex = 2,
                Text = @"&Save",
                UseVisualStyleBackColor = true
            };
            btnSave.Click += BtnSave_Click;
            TabPage newTabPage = new TabPage();
            newTabPage.Controls.Add(newPictureBox);
            newTabPage.Controls.Add(btnSave);
            newTabPage.Location = new Point(4, 22);
            newTabPage.Name = tabName;
            newTabPage.Text = tabName;
            newTabPage.Padding = new Padding(3);
            newTabPage.Size = new Size(402, 349);
            newTabPage.TabIndex = 0;
            newTabPage.UseVisualStyleBackColor = true;
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate{
                    TabControlClientImages.TabPages.Add(newTabPage);
                }));
            }
            else {
                TabControlClientImages.TabPages.Add(newTabPage);
            }
        }

        private void FrmImage_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            Visible = false;
        }

        private void BtnSave_Click(object sender, EventArgs e) {
            if (_Imgbyte == null) {
                return;
            }
            _Img = ByteArrayToImage(_Imgbyte);
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Filter = @"Images|*.png;"
            };
            //saveFileDialog.Filter = "Images|*.png;*.bmp;*.jpg*.gif*";

            //ImageFormat format = ImageFormat.Png;
            if (saveFileDialog.ShowDialog() == DialogResult.OK) {
                _Img.Save(saveFileDialog.FileName, ImageFormat.Png);
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
            foreach (TabPage tabPage in TabControlClientImages.TabPages) {
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