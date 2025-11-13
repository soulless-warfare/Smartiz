using Microsoft.VisualBasic.Logging;
using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Smartiz
{
    public partial class Login : Form
    {
        internal Main mainForm = new Main();
        private Sign signForm;
        private Password_Recovery recoveryForm;
        internal DataRow userRecord;
        private bool passwordVisible = false;
        private bool authenticated = false;
 
        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>Centralized message display with error code logging</summary>
        private void ShowMessage(string message, int errorCode)
        {
            string displayText = errorCode != 0 ? $"MS{errorCode} Error || {message}" : message;
            MessageBox.Show(displayText,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        /// <summary>Enables double buffering on a panel to reduce flicker</summary>
        private void EnableDoubleBuffering(System.Windows.Forms.Panel targetPanel)
        {
            try
            {
                typeof(System.Windows.Forms.Panel)
                    .InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                        null, targetPanel, new object[] { true });
            }
            catch
            {
                const string renderErrorMessage = "Rendering optimization failed.";
                ShowMessage(renderErrorMessage, 1001);
            }
        }

        /// <summary>Retrieves an image resource by name</summary>
        private Image GetResourceImage(string resourceName)
        {
            try
            {
                return (Image)Properties.Resources.ResourceManager.GetObject(resourceName);
            }
            catch
            {
                const string loadGraphicError = "Failed to load graphic resources.";
                ShowMessage(loadGraphicError, 1002);
                return null;
            }
        }

        /// <summary>Performs user login validation and authentication</summary>
        private void PerformLogin()
        {
            try
            {

                string username = Username_txt.Text;
                string password = Password_txt.Text;

                if (ValidText.AllCorrect(new string[] {}, new string[] { username,password}, new string[] {}) == false) MessageBox.Show("Please fix the Problems and try again", "Fix Problem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                else
                {

                    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                    {
                        const string inputError = "Please enter both username and password.";
                        ShowMessage(inputError, 0);
                        return;
                    }

                    DataRow userRecord = Program.Admins.Find("Username", username);
                    if (userRecord != null && userRecord["Password"].ToString() == password)
                    {
                        authenticated = true;
                        this.Close();
                    }
                    else
                    {
                        const string authFailMessage = "Incorrect username or password.";
                        ShowMessage(authFailMessage, 0);
                    }
                }
            }
            catch
            {
                const string loginProcessError = "Login process failed due to invalid input.";
                ShowMessage(loginProcessError, 1005);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////

        public Login()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            EnableDoubleBuffering(Login_pnl);
        }

        ///////////////////////////////////////////////////////////////////////////////////

        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                Password_txt.PasswordChar = '*';
            }
            catch
            {
                const string uiInitError = "Failed to initialize login form.";
                ShowMessage(uiInitError, 1004);
            }
        }
        private void Load_btn_Click(object sender, EventArgs e)
        {
            PerformLogin();
        }
        private void Username_txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bool valid = ValidText.IsPatterned(Username_txt.Text);
                UsernameCheck_lbl.Text = valid ? string.Empty : "Username must contain only English letters and digits.";
            }
            catch
            {
                const string validateError = "Username validation failed.";
                ShowMessage(validateError, 1008);
            }
        }
        private void Username_txt_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    Password_txt.Focus();
            }
            catch
            {
                const string navigationError = "Failed to handle key navigation.";
                ShowMessage(navigationError, 1006);
            }
        }
        private void Password_txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bool valid = ValidText.IsPatterned(Password_txt.Text);
                PasswordCheck_lbl.Text = valid ? string.Empty : "Password must contain only English letters and digits.";
            }
            catch
            {
                const string validateError = "Password validation failed.";
                ShowMessage(validateError, 1011);
            }
        }
        private void Password_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                PerformLogin();
        }
        private void Sign_btn_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                signForm = new Sign(this);
                signForm.ShowDialog();
            }
            catch
            {
                const string registerError = "Unable to open the registration form.";
                ShowMessage(registerError, 1009);
            }
        }
        private void ShowPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (passwordVisible)
                {
                    ShowPassword_btn.BackgroundImage = GetResourceImage("ClosedEye");
                    Password_txt.PasswordChar = '*';
                }
                else
                {
                    ShowPassword_btn.BackgroundImage = GetResourceImage("OpenEye");
                    Password_txt.PasswordChar = '\0';
                }

                passwordVisible = !passwordVisible;
            }
            catch
            {
                const string toggleError = "Unable to toggle password visibility.";
                ShowMessage(toggleError, 1010);
            }
        }
        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (authenticated)
                {
                    userRecord = Program.Admins.Find("Username", Username_txt.Text);
                    if (userRecord != null)
                    {
                        mainForm.SetMain(userRecord["ID"].ToString());
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        const string sessionError = "User session could not be created.";
                        ShowMessage(sessionError, 0);
                        Application.Exit();
                    }
                }
                else
                {
                    Application.Exit();
                }
            }
            catch
            {
                const string closingError = "Unexpected error while closing the login form.";
                ShowMessage(closingError, 1013);
                Application.Exit();
            }
        }
        private void PasswordRecovery_llb_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                this.Hide();
                recoveryForm = new Password_Recovery(this);
                recoveryForm.ShowDialog();
            }
            catch
            {
                const string recoveryError = "Unable to open password recovery.";
                ShowMessage(recoveryError, 1014);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////
    }
}
