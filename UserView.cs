using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Smartiz
{
    public partial class UserView : Form
    {
        private DataRow User;
        private DataRow Specific;

        /// <summary>Centralized message display with error code logging</summary>
        private void ShowMessage(string message, int errorCode)
        {
            string displayText = errorCode != 0 ? $"MS{errorCode} Error || {message}" : message;
            MessageBox.Show(displayText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>Extract Image from name from resource</summary>
        private Image ImageFromName(string ImageName)
        {
            try
            {
                return (Image)Properties.Resources.ResourceManager.GetObject(ImageName);
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8003);
                return null;
            }
        }
        /// <summary>Convert byte to image</summary>
        public Image ByteToImage(byte[] data)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException("Error: ", nameof(data));

            using (var ms = new MemoryStream(data))
            {
                return Image.FromStream(ms);
            }
        }
        /// <summary>Show images in flow layout panel</summary>
        public void ShowImages(FlowLayoutPanel panel, List<byte[]> imagesbyte)
        {
            panel.Controls.Clear();
            panel.AutoScroll = true;
            panel.WrapContents = true;
            panel.FlowDirection = FlowDirection.LeftToRight;

            foreach (var imgbyte in imagesbyte)
            {
                if (imgbyte == null) continue;

                Image img = ByteToImage(imgbyte);
                if (img == null) continue;

                Guna2PictureBox pic = new Guna2PictureBox
                {
                    Image = img,
                    SizeMode = PictureBoxSizeMode.StretchImage,
                    Width = img.Width,
                    Height = img.Height,
                    Margin = new Padding(5),
                    Cursor = Cursors.Hand,
                    BorderStyle = BorderStyle.None,
                    BorderRadius = 30,
                };

                pic.Click += (s, e) =>
                {
                    Form preview = new Form
                    {
                        Size = new System.Drawing.Size(img.Width, img.Height),
                        StartPosition = FormStartPosition.CenterScreen
                    };
                    PictureBox large = new PictureBox
                    {
                        Dock = DockStyle.Fill,
                        Image = img,
                        SizeMode = PictureBoxSizeMode.StretchImage
                    };
                    preview.Controls.Add(large);
                    preview.ShowDialog();
                };

                panel.Controls.Add(pic);
            }
        }
        /// <summary>Load user from database and set form atribute</summary>
        private void LoadVarible()
        {
            try
            {
                Username_txt.Text = User["Username"].ToString();
                FullName_txt.Text = User["FullName"].ToString();
                PhoneNumber_txt.Text = User["PhoneNumber"].ToString();
                Description_txt.Text = User["Description"].ToString();

                try
                {
                    byte[] imageData = (byte[])User["Image"];

                    if (imageData != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            UserImageUser_pic.Image = Image.FromStream(ms);
                        }
                    }

                    else UserImageUser_pic.Image = ImageFromName("ProfileImage");
                }
                catch
                {
                    UserImageUser_pic.Image = ImageFromName("ProfileImage");
                }

                //add image
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }
        /// <summary>Save user atribute to databse</summary>
        private void SetVarible()
        {
            try
            {
                string username = Username_txt.Text;
                string fullname = FullName_txt.Text;
                string description = Description_txt.Text;

                User["Username"] = username;
                User["Username"] = fullname;
                User["Username"] = description;

                //setimage
                //setimages
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        public UserView(DataRow user, List<byte[]>images)
        {
            InitializeComponent();

            User = user;
           // Specific = specific;

            LoadVarible();
            ShowImages(UserImages_flp,images);
        }

        private void SaveUser_btn_Click(object sender, EventArgs e)
        {
            SetVarible();
        }

        private void UserView_Load(object sender, EventArgs e)
        {

        }
    }
}
