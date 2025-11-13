namespace Smartiz
{
    partial class Password_Recovery
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Password_Recovery));
            timer1 = new System.Windows.Forms.Timer(components);
            timer2 = new System.Windows.Forms.Timer(components);
            ForgotPass_pnl = new System.Windows.Forms.Panel();
            guna2Button2 = new Guna.UI2.WinForms.Guna2Button();
            time_text_lbl = new Label();
            time_num_lbl = new Label();
            ping_lbl = new Label();
            label4 = new Label();
            code_txt = new Guna.UI2.WinForms.Guna2TextBox();
            label3 = new Label();
            phone_txt = new Guna.UI2.WinForms.Guna2TextBox();
            username_txt = new Guna.UI2.WinForms.Guna2TextBox();
            guna2Button1 = new Guna.UI2.WinForms.Guna2Button();
            label2 = new Label();
            label1 = new Label();
            ForgotPass_pnl.SuspendLayout();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Interval = 500;
            timer1.Tick += timer1_Tick;
            // 
            // timer2
            // 
            timer2.Interval = 1000;
            timer2.Tick += timer2_Tick;
            // 
            // ForgotPass_pnl
            // 
            ForgotPass_pnl.BackColor = Color.Transparent;
            ForgotPass_pnl.Controls.Add(guna2Button2);
            ForgotPass_pnl.Controls.Add(time_text_lbl);
            ForgotPass_pnl.Controls.Add(time_num_lbl);
            ForgotPass_pnl.Controls.Add(ping_lbl);
            ForgotPass_pnl.Controls.Add(label4);
            ForgotPass_pnl.Controls.Add(code_txt);
            ForgotPass_pnl.Controls.Add(label3);
            ForgotPass_pnl.Controls.Add(phone_txt);
            ForgotPass_pnl.Controls.Add(username_txt);
            ForgotPass_pnl.Controls.Add(guna2Button1);
            ForgotPass_pnl.Controls.Add(label2);
            ForgotPass_pnl.Controls.Add(label1);
            ForgotPass_pnl.Location = new Point(-1, -4);
            ForgotPass_pnl.Name = "ForgotPass_pnl";
            ForgotPass_pnl.Size = new Size(960, 535);
            ForgotPass_pnl.TabIndex = 49;
            // 
            // guna2Button2
            // 
            guna2Button2.Animated = true;
            guna2Button2.BackColor = Color.Transparent;
            guna2Button2.BorderColor = Color.FromArgb(0, 192, 0);
            guna2Button2.BorderRadius = 20;
            guna2Button2.CustomizableEdges = customizableEdges1;
            guna2Button2.DisabledState.BorderColor = Color.DarkGray;
            guna2Button2.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2Button2.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2Button2.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2Button2.FillColor = Color.Green;
            guna2Button2.FocusedColor = Color.FromArgb(0, 192, 0);
            guna2Button2.Font = new Font("MRT_Two Medium", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 178);
            guna2Button2.ForeColor = Color.White;
            guna2Button2.Location = new Point(403, 405);
            guna2Button2.Name = "guna2Button2";
            guna2Button2.PressedColor = Color.FromArgb(0, 192, 0);
            guna2Button2.ShadowDecoration.CustomizableEdges = customizableEdges2;
            guna2Button2.Size = new Size(150, 46);
            guna2Button2.TabIndex = 53;
            guna2Button2.Text = "بازیابی رمز عبور";
            guna2Button2.Click += guna2Button2_Click;
            // 
            // time_text_lbl
            // 
            time_text_lbl.AutoSize = true;
            time_text_lbl.BackColor = Color.Transparent;
            time_text_lbl.Font = new Font("MRT_Two Medium", 9F, FontStyle.Bold, GraphicsUnit.Point, 178);
            time_text_lbl.ForeColor = Color.Red;
            time_text_lbl.Location = new Point(435, 378);
            time_text_lbl.Name = "time_text_lbl";
            time_text_lbl.Size = new Size(0, 24);
            time_text_lbl.TabIndex = 60;
            // 
            // time_num_lbl
            // 
            time_num_lbl.AutoSize = true;
            time_num_lbl.BackColor = Color.Transparent;
            time_num_lbl.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            time_num_lbl.ForeColor = Color.Red;
            time_num_lbl.Location = new Point(401, 377);
            time_num_lbl.Name = "time_num_lbl";
            time_num_lbl.Size = new Size(0, 26);
            time_num_lbl.TabIndex = 59;
            // 
            // ping_lbl
            // 
            ping_lbl.AutoSize = true;
            ping_lbl.BackColor = Color.Transparent;
            ping_lbl.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ping_lbl.ForeColor = Color.Red;
            ping_lbl.Location = new Point(168, 451);
            ping_lbl.Name = "ping_lbl";
            ping_lbl.Size = new Size(0, 26);
            ping_lbl.TabIndex = 58;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Transparent;
            label4.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(90, 451);
            label4.Name = "label4";
            label4.Size = new Size(84, 26);
            label4.TabIndex = 57;
            label4.Text = "PING :";
            // 
            // code_txt
            // 
            code_txt.BackColor = Color.Transparent;
            code_txt.BorderColor = Color.White;
            code_txt.BorderRadius = 25;
            code_txt.BorderThickness = 3;
            code_txt.CustomizableEdges = customizableEdges3;
            code_txt.DefaultText = "";
            code_txt.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            code_txt.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            code_txt.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            code_txt.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            code_txt.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            code_txt.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            code_txt.ForeColor = Color.DimGray;
            code_txt.HoverState.BorderColor = Color.FromArgb(0, 192, 0);
            code_txt.Location = new Point(329, 322);
            code_txt.Margin = new Padding(5);
            code_txt.Name = "code_txt";
            code_txt.PasswordChar = '\0';
            code_txt.PlaceholderText = "";
            code_txt.SelectedText = "";
            code_txt.ShadowDecoration.CustomizableEdges = customizableEdges4;
            code_txt.Size = new Size(289, 54);
            code_txt.TabIndex = 55;
            code_txt.TextAlign = HorizontalAlignment.Center;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("MRT_Two Medium", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label3.Location = new Point(552, 289);
            label3.Name = "label3";
            label3.Size = new Size(42, 31);
            label3.TabIndex = 56;
            label3.Text = "کد";
            // 
            // phone_txt
            // 
            phone_txt.BackColor = Color.Transparent;
            phone_txt.BorderColor = Color.White;
            phone_txt.BorderRadius = 25;
            phone_txt.BorderThickness = 3;
            phone_txt.CustomizableEdges = customizableEdges5;
            phone_txt.DefaultText = "";
            phone_txt.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            phone_txt.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            phone_txt.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            phone_txt.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            phone_txt.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            phone_txt.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            phone_txt.ForeColor = Color.DimGray;
            phone_txt.HoverState.BorderColor = Color.FromArgb(0, 192, 0);
            phone_txt.Location = new Point(166, 125);
            phone_txt.Margin = new Padding(5);
            phone_txt.Name = "phone_txt";
            phone_txt.PasswordChar = '\0';
            phone_txt.PlaceholderText = "";
            phone_txt.SelectedText = "";
            phone_txt.ShadowDecoration.CustomizableEdges = customizableEdges6;
            phone_txt.Size = new Size(289, 54);
            phone_txt.TabIndex = 50;
            phone_txt.TextAlign = HorizontalAlignment.Center;
            // 
            // username_txt
            // 
            username_txt.BackColor = Color.Transparent;
            username_txt.BorderColor = Color.White;
            username_txt.BorderRadius = 25;
            username_txt.BorderThickness = 3;
            username_txt.CustomizableEdges = customizableEdges7;
            username_txt.DefaultText = "";
            username_txt.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            username_txt.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            username_txt.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            username_txt.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            username_txt.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            username_txt.Font = new Font("Bookman Old Style", 13.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            username_txt.ForeColor = Color.DimGray;
            username_txt.HoverState.BorderColor = Color.FromArgb(0, 192, 0);
            username_txt.Location = new Point(517, 125);
            username_txt.Margin = new Padding(5);
            username_txt.Name = "username_txt";
            username_txt.PasswordChar = '\0';
            username_txt.PlaceholderText = "";
            username_txt.SelectedText = "";
            username_txt.ShadowDecoration.CustomizableEdges = customizableEdges8;
            username_txt.Size = new Size(289, 54);
            username_txt.TabIndex = 49;
            username_txt.TextAlign = HorizontalAlignment.Center;
            // 
            // guna2Button1
            // 
            guna2Button1.Animated = true;
            guna2Button1.BackColor = Color.Transparent;
            guna2Button1.BorderColor = Color.FromArgb(0, 192, 0);
            guna2Button1.BorderRadius = 20;
            guna2Button1.CustomizableEdges = customizableEdges9;
            guna2Button1.DisabledState.BorderColor = Color.DarkGray;
            guna2Button1.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2Button1.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2Button1.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2Button1.FillColor = Color.Green;
            guna2Button1.FocusedColor = Color.FromArgb(0, 192, 0);
            guna2Button1.Font = new Font("MRT_Two Medium", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 178);
            guna2Button1.ForeColor = Color.White;
            guna2Button1.Location = new Point(390, 202);
            guna2Button1.Name = "guna2Button1";
            guna2Button1.PressedColor = Color.FromArgb(0, 192, 0);
            guna2Button1.ShadowDecoration.CustomizableEdges = customizableEdges10;
            guna2Button1.Size = new Size(188, 45);
            guna2Button1.TabIndex = 54;
            guna2Button1.Text = "درخواست کد";
            guna2Button1.Click += guna2Button1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("MRT_Two Medium", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label2.Location = new Point(319, 91);
            label2.Name = "label2";
            label2.Size = new Size(115, 31);
            label2.TabIndex = 52;
            label2.Text = "شماره تلفن";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("MRT_Two Medium", 12F, FontStyle.Bold, GraphicsUnit.Point, 178);
            label1.Location = new Point(679, 91);
            label1.Name = "label1";
            label1.Size = new Size(104, 31);
            label1.TabIndex = 51;
            label1.Text = "نام کاربری";
            // 
            // Password_Recovery
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImage = Properties.Resources.Stratup;
            ClientSize = new Size(957, 530);
            Controls.Add(ForgotPass_pnl);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Password_Recovery";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Password Recovery";
            FormClosed += Password_Recovery_FormClosed;
            Load += Password_Recovery_Load;
            ForgotPass_pnl.ResumeLayout(false);
            ForgotPass_pnl.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Panel ForgotPass_pnl;
        private Guna.UI2.WinForms.Guna2Button guna2Button2;
        private Label time_text_lbl;
        private Label time_num_lbl;
        private Label ping_lbl;
        private Label label4;
        private Guna.UI2.WinForms.Guna2TextBox code_txt;
        private Label label3;
        private Guna.UI2.WinForms.Guna2TextBox phone_txt;
        private Guna.UI2.WinForms.Guna2TextBox username_txt;
        private Guna.UI2.WinForms.Guna2Button guna2Button1;
        private Label label2;
        private Label label1;
    }
}