namespace Smartiz
{
    partial class UserProfile
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Main_pnl = new Guna.UI2.WinForms.Guna2Panel();
            ViewUser_btn = new Guna.UI2.WinForms.Guna2Button();
            Username_lbl = new Label();
            UserImage_pic = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            Main_pnl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)UserImage_pic).BeginInit();
            SuspendLayout();
            // 
            // Main_pnl
            // 
            Main_pnl.BorderRadius = 30;
            Main_pnl.Controls.Add(ViewUser_btn);
            Main_pnl.Controls.Add(Username_lbl);
            Main_pnl.Controls.Add(UserImage_pic);
            Main_pnl.CustomizableEdges = customizableEdges4;
            Main_pnl.FillColor = Color.White;
            Main_pnl.Location = new Point(15, 15);
            Main_pnl.Name = "Main_pnl";
            Main_pnl.ShadowDecoration.CustomizableEdges = customizableEdges5;
            Main_pnl.Size = new Size(220, 180);
            Main_pnl.TabIndex = 0;
            // 
            // ViewUser_btn
            // 
            ViewUser_btn.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ViewUser_btn.BackColor = Color.White;
            ViewUser_btn.BorderColor = Color.FromArgb(210, 0, 65);
            ViewUser_btn.BorderRadius = 20;
            ViewUser_btn.BorderThickness = 2;
            ViewUser_btn.CustomizableEdges = customizableEdges1;
            ViewUser_btn.DisabledState.BorderColor = Color.DarkGray;
            ViewUser_btn.DisabledState.CustomBorderColor = Color.DarkGray;
            ViewUser_btn.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            ViewUser_btn.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            ViewUser_btn.FillColor = Color.Empty;
            ViewUser_btn.Font = new Font("MRT_Two Medium", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 178);
            ViewUser_btn.ForeColor = Color.FromArgb(210, 0, 65);
            ViewUser_btn.Location = new Point(50, 130);
            ViewUser_btn.Name = "ViewUser_btn";
            ViewUser_btn.ShadowDecoration.CustomizableEdges = customizableEdges2;
            ViewUser_btn.Size = new Size(120, 45);
            ViewUser_btn.TabIndex = 10;
            ViewUser_btn.Text = "مشاهده";
            ViewUser_btn.Click += ViewUser_btn_Click;
            // 
            // Username_lbl
            // 
            Username_lbl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Username_lbl.BackColor = Color.White;
            Username_lbl.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Username_lbl.ForeColor = Color.Black;
            Username_lbl.Location = new Point(10, 93);
            Username_lbl.Name = "Username_lbl";
            Username_lbl.Size = new Size(200, 30);
            Username_lbl.TabIndex = 9;
            Username_lbl.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // UserImage_pic
            // 
            UserImage_pic.Anchor = AnchorStyles.Top;
            UserImage_pic.BackColor = Color.White;
            UserImage_pic.ErrorImage = Properties.Resources.ProfileImage;
            UserImage_pic.Image = Properties.Resources.ProfileImage;
            UserImage_pic.ImageRotate = 0F;
            UserImage_pic.InitialImage = Properties.Resources.ProfileImage;
            UserImage_pic.Location = new Point(70, 10);
            UserImage_pic.Name = "UserImage_pic";
            UserImage_pic.ShadowDecoration.CustomizableEdges = customizableEdges3;
            UserImage_pic.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            UserImage_pic.Size = new Size(80, 80);
            UserImage_pic.SizeMode = PictureBoxSizeMode.StretchImage;
            UserImage_pic.TabIndex = 8;
            UserImage_pic.TabStop = false;
            // 
            // UserProfile
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(230, 230, 255);
            Controls.Add(Main_pnl);
            Name = "UserProfile";
            Size = new Size(250, 210);
            Main_pnl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)UserImage_pic).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel Main_pnl;
        private Guna.UI2.WinForms.Guna2CirclePictureBox UserImage_pic;
        private Label Username_lbl;
        private Guna.UI2.WinForms.Guna2Button ViewUser_btn;
    }
}
