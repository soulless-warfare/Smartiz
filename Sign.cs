using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Smartiz
{
    public partial class Sign : Form
    {
        private readonly Login loginForm;
        private bool passwordVisible = false;
     
        ///////////////////////////////////////////////////////////////////////////////////

        /// <summary>Centralized message display with error code logging</summary>
        private void ShowMessage(string message, int errorCode)
        {
            string displayText = errorCode != 0 ? $"MS{errorCode} Error || {message}" : message;
            MessageBox.Show(displayText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                const string renderError = "System resource optimization failed.";
                ShowMessage(renderError, 2001);
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
                const string imageError = "Failed to load image resource.";
                ShowMessage(imageError, 2002);
                return null;
            }
        }

        /// <summary>Performs user registration and stores new user in database</summary>
        private void PerformSignUp()
        {
            try
            {
                string fullName = Fullname_txt.Text;
                string username = Username_txt.Text;
                string phone = PhoneNumber_txt.Text;
                string email = Email_txt.Text;
                string password = Password_txt.Text;
                string confirmPassword = SecendPassword_txt.Text;

                if (ValidText.AllCorrect(new string[]{fullName}, new string[] { username,email,password,confirmPassword }, new string[] { phone }) == false) MessageBox.Show("Please fix the Problems and try again","Fix Problem",MessageBoxButtons.OK,MessageBoxIcon.Warning);   
                else
                {
                    if (new[] { fullName, username, phone, email, password, confirmPassword }
                        .Any(string.IsNullOrWhiteSpace))
                    {
                        Fullname_txt.BorderColor = string.IsNullOrWhiteSpace(fullName) ? Color.Red : Color.White;
                        Username_txt.BorderColor = string.IsNullOrWhiteSpace(username) ? Color.Red : Color.White;
                        PhoneNumber_txt.BorderColor = string.IsNullOrWhiteSpace(phone) ? Color.Red : Color.White;
                        Email_txt.BorderColor = string.IsNullOrWhiteSpace(email) ? Color.Red : Color.White;
                        Password_txt.BorderColor = string.IsNullOrWhiteSpace(password) ? Color.Red : Color.White;
                        SecendPassword_txt.BorderColor = string.IsNullOrWhiteSpace(confirmPassword) ? Color.Red : Color.White;

                        const string fieldError = "Please fill in all required fields.";
                        ShowMessage(fieldError, 0);
                        return;
                    }

                    if (password != confirmPassword)
                    {
                        const string matchError = "The password and its confirmation do not match.";
                        ShowMessage(matchError, 0);
                        return;
                    }

                    if (Program.Admins.Find("Username", username) != null)
                    {
                        const string duplicateError = "An account with this username already exists.";
                        ShowMessage(duplicateError, 0);
                        return;
                    }

                    DataRow newRow = Program.Admins.DataTable.NewRow();
                    string userId;
                    userId = Program.Admins.CreateID("ID", 10);

                    newRow.SetField("ID", userId);
                    newRow.SetField("Username", username);
                    newRow.SetField("Password", password);
                    newRow.SetField("FullName", fullName);
                    newRow.SetField("PhoneNumber", phone);
                    newRow.SetField("Email", email);
                    newRow.SetField("Image", new byte[1]);

                    Exception result = Program.Admins.Add(newRow);
                    if (result.Message == "Successfuly")
                    {
                        const string successMessage = "Your account was successfully created.";
                        MessageBox.Show(successMessage, "Sgin", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        this.Close();
                    }
                    else
                    {
                        const string createError = "Account creation failed.";
                        ShowMessage(createError, 2005);
                    }
                }
            }
            catch
            {
                const string generalError = "An error occurred while processing registration.";
                ShowMessage(generalError, 2006);
            }
        }   

        ///////////////////////////////////////////////////////////////////////////////////

        public Sign(Login login)
        {
            InitializeComponent();
            loginForm = login;
            this.DoubleBuffered = true;
            EnableDoubleBuffering(Sign_pnl);
        }

        ///////////////////////////////////////////////////////////////////////////////////

        private void Sign_Load(object sender, EventArgs e)
        {
            Fullname_txt.Focus();
            Password_txt.PasswordChar = '*';
            SecendPassword_txt.PasswordChar = '*';
        }
        private void Fullname_txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bool valid = ValidText.IsText(Fullname_txt.Text);
                FullnameCheck_lbl.Text = valid ? string.Empty : "Full name must not contain numbers.";
            }
            catch
            {
                const string validateError = "Full name validation failed.";
                ShowMessage(validateError, 2007);
            }
        }
        private void Fullname_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Username_txt.Focus();
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
                ShowMessage(validateError, 2008);
            }
        }
        private void Username_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                PhoneNumber_txt.Focus();
        }
        private void PhoneNumber_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bool valid = ValidText.IsNumber(PhoneNumber_txt.Text);
                PhoneNumberCheck_lbl.Text = valid ? string.Empty : "Phone number must contain only digits.";
            }
            catch
            {
                const string validateError = "Phone validation failed.";
                ShowMessage(validateError, 2009);
            }
        }
        private void PhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Email_txt.Focus();
        }
        private void Email_txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bool valid = ValidText.IsPatterned(Email_txt.Text);
                EmailCheck_lbl.Text = valid ? string.Empty : "Email must contain only English letters and digits.";
            }
            catch
            {
                const string validateError = "Email validation failed.";
                ShowMessage(validateError, 2010);
            }
        }
        private void Email_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Password_txt.Focus();
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
                ShowMessage(validateError, 2011);
            }
        }
        private void Password_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SecendPassword_txt.Focus();
        }
        private void SecendPassword_txt_TextChanged(object sender, EventArgs e)
        {
            try
            {
                bool valid = ValidText.IsPatterned(SecendPassword_txt.Text);
                SecendPasswordCheck_lbl.Text = valid ? string.Empty : "Password confirmation must contain only English letters and digits.";
            }
            catch
            {
                const string validateError = "Confirm password validation failed.";
                ShowMessage(validateError, 2012);
            }
        }
        private void SecendPassword_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                PerformSignUp();
        }
        private void ShowPassword_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (passwordVisible)
                {
                    ShowPassword_btn.BackgroundImage = GetResourceImage("ClosedEye");
                    Password_txt.PasswordChar = '*';
                    SecendPassword_txt.PasswordChar = '*';
                }
                else
                {
                    ShowPassword_btn.BackgroundImage = GetResourceImage("OpenEye");
                    Password_txt.PasswordChar = '\0';
                    SecendPassword_txt.PasswordChar = '\0';
                }

                passwordVisible = !passwordVisible;
            }
            catch
            {
                const string toggleError = "Unable to toggle password visibility.";
                ShowMessage(toggleError, 2013);
            }
        }
        private void SaveUser_btn_Click(object sender, EventArgs e)
        {
            PerformSignUp();
        }
        private void Sign_FormClosed(object sender, FormClosedEventArgs e)
        {
            loginForm.Show();
            this.Close();
        }

        ///////////////////////////////////////////////////////////////////////////////////
    }
}
