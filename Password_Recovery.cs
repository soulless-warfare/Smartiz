using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
//using Twilio;
//using Twilio.Rest.Api.V2010.Account;
//using Twilio.Types;

namespace Smartiz
{
    public partial class Password_Recovery : Form
    {
        private string verificationCode;
        private bool internetAvailable;
        private int remainingSeconds = 60;
        private readonly Login loginForm;

        private static readonly Regex PhonePattern = new Regex("^\\+?[0-9]{10,15}$", RegexOptions.Compiled);
        private static readonly Regex CleanPattern = new Regex("^[a-zA-Z0-9@._\\-+]*$", RegexOptions.Compiled);
        private static readonly Dictionary<string, string> CountryCodeMap = new Dictionary<string, string>
        {
            { "09", "+98" },
            { "01", "+1" },
            { "07", "+44" }
        };

        /// <summary>
        /// Centralized message display with error code logging.
        /// </summary>
        private void ShowMessage(string message, int errorCode)
        {
            string displayText = errorCode != 0 ? $"MS{errorCode} Error || {message}" : message;
            MessageBox.Show(displayText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Enables double buffering on a panel to reduce flicker.
        /// </summary>
        private void EnableDoubleBuffering(System.Windows.Forms.Panel panel)
        {
            try
            {
                typeof(System.Windows.Forms.Panel).InvokeMember(
                    "DoubleBuffered",
                    BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                    null,
                    panel,
                    new object[] { true });
            }
            catch
            {
                const string err = "System resource optimization failed.";
                ShowMessage(err, 3001);
            }
        }

        /// <summary>
        /// Retrieves an image resource by name.
        /// </summary>
        private Image GetResourceImage(string name)
        {
            try
            {
                return (Image)Properties.Resources.ResourceManager.GetObject(name);
            }
            catch
            {
                const string err = "Failed to load image resource.";
                ShowMessage(err, 3002);
                return null;
            }
        }

        /// <summary>
        /// Normalizes a raw phone number to international format.
        /// </summary>
        private string NormalizePhone(string raw)
        {
            raw = raw.Trim().Replace(" ", string.Empty);
            if (raw.StartsWith("+")) return raw;
            foreach (var kv in CountryCodeMap)
            {
                if (raw.StartsWith(kv.Key))
                    return kv.Value + raw.Substring(kv.Key.Length);
            }
            return raw;
        }

        /// <summary>
        /// Checks internet connectivity by pinging Google.
        /// </summary>
        private void CheckInternet()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("google.com");
                    internetAvailable = reply.Status == IPStatus.Success;
                    ping_lbl.Text = internetAvailable ? $"{reply.RoundtripTime} ms" : "Internet connection is not available. Please check your network.";
                }
            }
            catch
            {
                internetAvailable = false;
                ping_lbl.Text = "Internet connection is not available. Please check your network.";
            }
        }

        /// <summary>
        /// Sends a verification code via SMS to the specified phone number.
        /// </summary>
        //private void SendVerificationCode(string toPhone)
        //{
        //    verificationCode = new Random().Next(10000, 99999).ToString();
        //    var smsText = $"Verification Code\n\nYour code is: {verificationCode}\n\nPanel Management Software";
        //    MessageResource.Create(
        //        body: smsText,
        //        from: new PhoneNumber("+14132695260"),
        //        to: new PhoneNumber(toPhone));
        //}

        /// <summary>
        /// Sends the recovered password via SMS to the user.
        /// </summary>
        //private void SendPassword(string toPhone, string username, string password)
        //{
        //    var smsText = $"Password Recovery\n\nUsername: {username}\nPassword: {password}\n\nPanel Management Software";
        //    MessageResource.Create(
        //        body: smsText,
        //        from: new PhoneNumber("+14132695260"),
        //        to: new PhoneNumber(toPhone));
        //}

        /// <summary>
        /// Initializes form and Twilio client, connects to Users table.
        /// </summary>
        public Password_Recovery(Login login)
        {
            InitializeComponent();
            loginForm = login;
            this.DoubleBuffered = true;
            EnableDoubleBuffering(ForgotPass_pnl);
        }

        private void Password_Recovery_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckInternet();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            CheckInternet();
            if (!internetAvailable)
            {
                ShowMessage("Internet connection is not available.", 0);
                return;
            }

            try
            {
                string userInput = username_txt.Text.Trim();
                string phoneInput = NormalizePhone(phone_txt.Text);

                if (!PhonePattern.IsMatch(phoneInput))
                {
                    ShowMessage("Invalid phone number format.", 0);
                    return;
                }

                var userRow = Program.Admins.Find("Username", userInput);
                if (userRow == null)
                {
                    ShowMessage("User with provided details not found.", 0);
                    return;
                }

                string storedPhone = NormalizePhone(userRow["PhoneNumber"].ToString());
                if (userInput == userRow["Username"].ToString() && storedPhone == phoneInput)
                {
                    //SendVerificationCode(storedPhone);
                    timer2.Start();
                    time_text_lbl.Text = "Remaining Time:";
                    ShowMessage("Verification code has been sent.", 0);
                }
                else
                {
                    ShowMessage("Entered details do not match our records.", 0);
                }
            }
            catch
            {
                ShowMessage("Failed to send verification code.", 3003);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            time_num_lbl.Text = remainingSeconds.ToString();
            remainingSeconds--;
            if (remainingSeconds <= 0) timer2.Stop();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            if (remainingSeconds < 1)
            {
                ShowMessage("Verification code has expired.", 0);
                return;
            }

            if (verificationCode != code_txt.Text)
            {
                ShowMessage("Incorrect verification code.", 0);
                return;
            }

            try
            {
                string userInput = username_txt.Text.Trim();
                var userRow = Program.Admins.Find("Username", userInput);
                if (userRow != null)
                {
                    //SendPassword(
                    //    NormalizePhone(userRow["PhoneNumber"].ToString()),
                    //    userRow["Username"].ToString(),
                    //    userRow["Password"].ToString());

                    ShowMessage("Your password has been sent via SMS.", 0);
                    this.Close();
                }
            }
            catch
            {
                ShowMessage("Failed to send password via SMS.", 3004);
            }
        }

        private void Password_Recovery_FormClosed(object sender, FormClosedEventArgs e)
        {
            loginForm.Show();
            this.Close();
        }
    }
}
