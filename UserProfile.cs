using Microsoft.VisualBasic.ApplicationServices;
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
    public partial class UserProfile : UserControl
    {
        DataRow User;
        List<byte[]> Images;


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
        /// <summary>Set varible user profile control</summary>
        private void Set_Varible()
        {
            if (User == null) return;

            Username_lbl.Text = User["Username"].ToString();

            try
            {
                byte[] imageData = (byte[])User["Image"];

                if (imageData != null)
                {
                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        UserImage_pic.Image = Image.FromStream(ms);
                    }
                }

                else UserImage_pic.Image = ImageFromName("ProfileImage");
            }
            catch
            {
                UserImage_pic.Image = ImageFromName("ProfileImage");
            }
        }

        public UserProfile(DataRow user, List<byte[]>images)
        {
            InitializeComponent();

            User = user;
            Images = images;

            Set_Varible();
        }

        private void ViewUser_btn_Click(object sender, EventArgs e)
        {
            UserView userView = new UserView(User, Images);
            userView.ShowDialog();
        }
    }
}
