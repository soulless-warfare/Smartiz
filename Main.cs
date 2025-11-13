using Guna.UI2.WinForms;
using Microsoft.VisualBasic.ApplicationServices;
using Samrtiz;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace Smartiz
{
    public partial class Main : Form
    {
        private Panel CurrentPanel;
        private List<byte[]> NUImages;
        private FaceRegistration.Accuracy AIAccuracy;
        private FaceRecognition.SimilarityMode SimilarityMode;
        private bool MultiProgramming = false;
        private string ID;
        private bool FI = true;
        private bool RecOK = false;
        private bool IsWorking = false;
        private string ResID = string.Empty;

        /////////////////////////////////////////////////////////////////////////////

        public Main()
        {
            InitializeComponent();

            this.Visible = false;
            this.WindowState = FormWindowState.Maximized;

            BUFFER(Dashboard_pnl);
            BUFFER(Search_pnl);
            BUFFER(Managment_pnl);
            BUFFER(Backup_pnl);
            BUFFER(UISetting_pnl);
            BUFFER(AISetting_pnl);
            BUFFER(About_pnl);
            BUFFER(RemoveUser_pnl);
            BUFFER(AddUser_pnl);
            BUFFER(Profile_pnl);
        }

        protected override void OnActivated(EventArgs e)
        {
            this.Visible = true;
            base.OnActivated(e);
        }

        internal void SetMain(string id)
        {
            ID = id;

            Users_dgv.AutoGenerateColumns = false;

            MAIN_SetVarible();
            PROFILE_SetVarible();
            DASHBOARD_SetVarible();
            SEARCH_SetVarible();
            MANAGMENT_SetVarible();
            UISETTING_SetVarible();
            AISETTING_SetVarible();
            ABOUT_SetVarible();

            ShowTab(Dashboard_pnl);
            TopName_lbl.Text = "داشبورد";
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>Centralized message display with error code logging</summary>
        private void ShowMessage(string message, int errorCode)
        {
            string displayText = errorCode != 0 ? $"MS{errorCode} Error || {message}" : message;
            MessageBox.Show(displayText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>Displays specified panel and bring menu button to front</summary>
        private void ShowTab(System.Windows.Forms.Panel panel)
        {
            try
            {
                if (panel == CurrentPanel) return;

                Dashboard_pnl.Visible = Search_pnl.Visible = Managment_pnl.Visible = Backup_pnl.Visible = UISetting_pnl.Visible = AISetting_pnl.Visible = About_pnl.Visible = false;

                panel.Visible = true;
                panel.BringToFront();

                CurrentPanel = panel;
            }
            catch
            {
                ShowMessage($"{panel.Name} has proble to show", 8181);
            }
        }
        /// <summary>Active double buffer for panel</summary>
        public void BUFFER(System.Windows.Forms.Panel panel)
        {
            try
            {
                typeof(System.Windows.Forms.Panel).InvokeMember("DoubleBuffered", BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic, null, panel, new object[] { true });
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.OptimizedDoubleBuffer |
                              ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.UserPaint, true);
                this.UpdateStyles();
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8006);
            }
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
        /// <summary>Update row numbers</summary>
        private void UpdateRowNumbers(DataGridView dataGridView, string ColumnsName)
        {
            try
            {
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    dataGridView.Rows[i].Cells[ColumnsName].Value = i + 1;
                }
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8009);
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
        /// <summary>Found user by id and fill the atributes</summary>
        private void FillUser((string UserID, float SimilarityPercent) user)
        {
            try
            {
                DataRow USER = Program.Users.ExtractionRow("ID", user.UserID);

                ResultUsername_lbl.Text = USER["Username"].ToString();

                try
                {
                    byte[] imageData = (byte[])USER["Image"];

                    if (imageData != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            ResultImage_pic.Image = Image.FromStream(ms);
                        }
                    }

                    else ResultImage_pic.Image = ImageFromName("ProfileImage");
                }
                catch
                {
                    ResultImage_pic.Image = ImageFromName("ProfileImage");
                }

                ResID = user.UserID;

                ResultPercent_rdg.Value = (int)user.SimilarityPercent;
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8009);
            }
        }
        /// <summary>Found user with his image</summary>
        private async Task<(string UserID, float SimilarityPercent)> FoundUser(byte[] image, FaceRegistration.Accuracy accuracy)
        {
            try
            {
                RecProgress_pgb.Value = 0;
                RecProgress_pgb.ProgressColor = Color.Navy;
                RecProgress_pgb.ProgressColor2 = Color.RoyalBlue;

                RecProgress_pgb.Visible = true;

                FaceRegistration registration = new FaceRegistration(RecProgress_pgb);
                float[] embededds;

                embededds = await registration.RegistrationAsync(image, accuracy);

                RecProgress_pgb.Value = 0;
                RecProgress_pgb.ProgressColor = Color.Green;
                RecProgress_pgb.ProgressColor2 = Color.Lime;

                FaceRecognition recognition = new FaceRecognition(SimilarityMode, 0.6f, RecProgress_pgb);
                var user = await recognition.IdentifyUserAsync(Program.Specifics.DataTable, Program.Embeddeds.DataTable, embededds);

                return user;
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8009);
                return (null, 0);
            }
        }
        private class User
        {
            public string Username { get; set; }
            public string FullName { get; set; }
            public string Description { get; set; }
            public string ImageProfilePath { get; set; }
            public string ImageFolderPath { get; set; }
        }
        /// <summary>Found User with JSON file</summary>
        public List<(string Username, string FullName, string Description, string ImageProfilePath, string ImageFolderPath)> ReadUsersFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("JSON file not founded", filePath);

            string jsonContent = File.ReadAllText(filePath);

            List<User> users = JsonConvert.DeserializeObject<List<User>>(jsonContent);

            var result = new List<(string Username, string FullName, string Description, string ImageProfilePath, string ImageFolderPath)>();

            foreach (var u in users)
            {
                result.Add((u.Username, u.FullName, u.Description, u.ImageProfilePath, u.ImageFolderPath));
            }

            return result;
        }
        /// <summary>Register and save user in to data base</summary>
        private async Task<bool> RegisterUser(DataRow user, List<byte[]> imagesbyte, FaceRegistration.Accuracy accuracy)
        {
            try
            {
                if (Program.Users.Find("Username", user["Username"]) != null)
                {
                    MessageBox.Show("this user has been registerd");
                    return false;
                }

                FaceRegistration registration = new FaceRegistration(RegisterProgress_pgb);

                RegisterProgress_pgb.Value = 0;
                RegisterProgress_pgb.Visible = true;

                var result = await registration.RegistrationAsync(imagesbyte, user["ID"].ToString(), accuracy);
                if (result) Program.Users.Add(user);

                return result;
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8009);
                return false;
            }
        }
        /// <summary>Extract Image by folder path</summary>
        private List<byte[]> ExtractImageByFolder(string folderPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                    return new List<byte[]>();

                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif", ".tiff", ".webp" };
                List<byte[]> result = new();

                foreach (var file in Directory.EnumerateFiles(folderPath))
                {
                    string ext = Path.GetExtension(file).ToLowerInvariant();

                    if (!validExtensions.Contains(ext))
                        continue;

                    try
                    {
                        using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                        using (var ms = new MemoryStream())
                        {
                            fs.CopyTo(ms);
                            byte[] bytes = ms.ToArray();
                            ms.Position = 0;

                            Image img = Image.FromStream(ms);
                            result.Add(bytes);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
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
                    SizeMode = PictureBoxSizeMode.Normal,
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
                        SizeMode = PictureBoxSizeMode.Normal
                    };
                    preview.Controls.Add(large);
                    preview.ShowDialog();
                };

                panel.Controls.Add(pic);
            }
        }
        /// <summary>Export database to sql file</summary>
        public string ExportInserts(DataTable table, string filePath, string tableName)
        {
            try
            {
                StringBuilder sql = new StringBuilder();

                foreach (DataRow row in table.Rows)
                {
                    StringBuilder values = new StringBuilder();

                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        object val = row[i];

                        if (val == DBNull.Value)
                        {
                            values.Append("NULL");
                        }
                        else if (val is string || val is DateTime)
                        {
                            string safe = val.ToString().Replace("'", "''");
                            values.Append($"'{safe}'");
                        }
                        else if (val is byte[] bytes)
                        {
                            string hex = BitConverter.ToString(bytes).Replace("-", "");
                            values.Append($"X'{hex}'");
                        }
                        else if (val is bool b)
                        {
                            values.Append(b ? "1" : "0");
                        }
                        else
                        {
                            values.Append(Convert.ToString(val, System.Globalization.CultureInfo.InvariantCulture));
                        }

                        if (i < table.Columns.Count - 1)
                            values.Append(", ");
                    }

                    string columns = string.Join(", ", table.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\""));

                    sql.AppendLine($"INSERT INTO \"{tableName}\" ({columns}) VALUES ({values});");
                }

                File.WriteAllText(filePath, sql.ToString(), Encoding.UTF8);

                return "Successfully";
            }
            catch (Exception ex)
            {
                return $"break: {ex.Message}";
            }
        }

        /// <summary>import database from file</summary>
        private string ImportInserts(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    throw new FileNotFoundException("SQL file not found.", filePath);

                string sqlScript = File.ReadAllText(filePath, Encoding.UTF8);

                string[] commands = Regex.Split(sqlScript, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                using (var conn = new SqliteConnection(Program.connectionString))
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (string command in commands)
                        {
                            string trimmed = command.Trim();
                            if (string.IsNullOrWhiteSpace(trimmed)) continue;

                            using (var cmd = new SqliteCommand(trimmed, conn, transaction))
                            {
                                try
                                {
                                    cmd.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    return $"break: {ex.Message}";
                                }
                            }
                        }

                        transaction.Commit();
                    }

                    conn.Close();
                }

                return "Successfully";
            }
            catch (Exception ex)
            {
                return $"break: {ex.Message}";
            }
        }
        /// <summary>Remove datatable rows</summary>
        private string ClearDataBase(string tableName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tableName))
                    return "break: Table name is empty";

                using (var conn = new SqliteConnection(Program.connectionString))
                {
                    conn.Open();

                    string command = $"DELETE FROM \"{tableName}\";";

                    using (var cmd = new SqliteCommand(command, conn))
                    {
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            return $"break: {ex.Message}";
                        }
                    }

                    using (var vacuum = new SqliteCommand("VACUUM;", conn))
                    {
                        vacuum.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                return "Successfully";
            }
            catch (Exception ex)
            {
                return $"break: {ex.Message}";
            }
        }


        /////////////////////////////////////////////////////////////////////////////

        private void MAIN_SetVarible()
        {
            try
            {
                DataRow ADMIN = Program.Admins.ExtractionRow("ID", ID);

                MainUsername_lbl.Text = ADMIN["Username"].ToString();
                MainUserID_lbl.Text = ADMIN["ID"].ToString();

                try
                {
                    byte[] imageData = (byte[])ADMIN["Image"];

                    if (imageData != null)
                    {
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            MainUser_pic.Image = Image.FromStream(ms);
                        }
                    }

                    else MainUser_pic.Image = ImageFromName("ProfileImage");
                }
                catch
                {
                    MainUser_pic.Image = ImageFromName("ProfileImage");
                }
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void PROFILE_SetVarible()
        {
            try
            {

            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void DASHBOARD_SetVarible()
        {
            try
            {
                if (Program.Users.DataTable != null)
                {
                    UserCounter2_lbl.Text = Program.Users.DataTable.Rows.Count.ToString();
                    Users_dgv.DataSource = Program.Users.DataTable;
                }
                else UserCounter2_lbl.Text = "0";
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void SEARCH_SetVarible()
        {
            try
            {
                UsersCount2_lbl.Text = Program.Users.DataTable.Rows.Count.ToString();
                SpecificsCount2_lbl.Text = Program.Specifics.DataTable.Rows.Count.ToString();
                EmbeddedCount2_lbl.Text = Program.Embeddeds.DataTable.Rows.Count.ToString();

                WithFace_rbt.Checked = FI;
                WithID_rbt.Checked = !FI;
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void MANAGMENT_SetVarible()
        {
            try
            {
                if (Program.Users.DataTable == null) return;

                DataTable temp = Program.Users.DataTable;

                foreach (DataRow newUser in temp.Rows)
                {
                    string userid = newUser["ID"].ToString();
                    DataRow[] specifics = Program.Specifics.ExtractionRows("UserID", userid);
                    List<byte[]> images = new List<byte[]>();

                    foreach (var item in specifics)
                    {
                        images.Add((byte[])item["Image"]);
                    }

                    UserProfile userProfile = new UserProfile(newUser, images);
                    userProfile.Width = this.ManagmentUsers_flp.Width - 27;
                    ManagmentUsers_flp.Controls.Add(userProfile);
                }
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void BACKUP_SetVarible()
        {
            try
            {

            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void UISETTING_SetVarible()
        {
            try
            {

            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void AISETTING_SetVarible()
        {
            try
            {
                AccuracyLow_rbt.Checked = AccuracyHigh_rbt.Checked = AccuracyIntricate_rbt.Checked = false;
                AccuracyIntermediate_rbt.Checked = true;
                AIAccuracy = FaceRegistration.Accuracy.Intermediate;
                SimilarityModeEuclideanWeighted_rbt.Checked = true;
                SimilarityMode = FaceRecognition.SimilarityMode.EuclideanWeighted;
                Parallel_rbt.Checked = false;
                Sequential_rbt.Checked = true;
                MultiProgramming = false;
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        private void ABOUT_SetVarible()
        {
            try
            {

            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        /////////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////////////


        /////////////////////////////////////////////////////////////////////////////

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void Dashboard_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Dashboard_pnl);
            TopName_lbl.Text = "داشبورد";
        }

        private void Search_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Search_pnl);
            TopName_lbl.Text = "جستجو کاربر";
        }

        private void Managment_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Managment_pnl);
            TopName_lbl.Text = "مدیریت کاربران";
        }

        private void Backup_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Backup_pnl);
            TopName_lbl.Text = "داده ها و بکاپ";
        }

        private void UISetting_btn_Click(object sender, EventArgs e)
        {
            ShowTab(UISetting_pnl);
            TopName_lbl.Text = "تنظیمات بصری";
        }

        private void AISetting_btn_Click(object sender, EventArgs e)
        {
            ShowTab(AISetting_pnl);
            TopName_lbl.Text = "مدیریت هوش مصنوعی";
        }

        private void About_btn_Click(object sender, EventArgs e)
        {
            ShowTab(About_pnl);
            TopName_lbl.Text = "ارتباط با ما";
        }

        private void AddUser_btn_Click(object sender, EventArgs e)
        {
            if (AddUser_btn.Height + 10 <= 50)
            {
                RemoveUser_btn.Location = new Point(RemoveUser_btn.Location.X, AddUser_btn.Location.Y);
                RemoveUser_btn.Size = new Size(RemoveUser_btn.Width, AddUser_btn.Height);

                AddUser_btn.Location = new Point(AddUser_btn.Location.X, AddUser_btn.Location.Y - 10);
                AddUser_btn.Size = new Size(AddUser_btn.Width, AddUser_btn.Height + 10);
            }

            RemoveUser_pnl.Visible = false;
            AddUser_pnl.Visible = true;
            AddUser_pnl.BringToFront();
        }

        private void RemoveUser_btn_Click(object sender, EventArgs e)
        {
            if (RemoveUser_btn.Height + 10 <= 50)
            {
                AddUser_btn.Location = new Point(AddUser_btn.Location.X, RemoveUser_btn.Location.Y);
                AddUser_btn.Size = new Size(AddUser_btn.Width, RemoveUser_btn.Height);

                RemoveUser_btn.Location = new Point(RemoveUser_btn.Location.X, RemoveUser_btn.Location.Y - 10);
                RemoveUser_btn.Size = new Size(RemoveUser_btn.Width, RemoveUser_btn.Height + 10);
            }

            AddUser_pnl.Visible = false;
            RemoveUser_pnl.Visible = true;
            RemoveUser_pnl.BringToFront();
        }

        private void MainUser_pic_Click(object sender, EventArgs e)
        {
            ShowTab(Profile_pnl);
            TopName_lbl.Text = "پروفایل";
        }

        private void TopSetting_btn_Click(object sender, EventArgs e)
        {
            ShowTab(AISetting_pnl);
            TopName_lbl.Text = "مدیریت هوش مصنوعی";
        }

        private void TopSearch_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Search_pnl);
            TopName_lbl.Text = "جستجو کاربر";
        }

        private void DashboardSetting_btn_Click(object sender, EventArgs e)
        {
            ShowTab(UISetting_pnl);
            TopName_lbl.Text = "تنظیمات بصری";
        }

        private void DashboardSearch_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Search_pnl);
            TopName_lbl.Text = "جستجو کاربر";
        }

        private void DashboardProfile_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Profile_pnl);
            TopName_lbl.Text = "پروفایل";
        }

        private void DashboardManagment_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Managment_pnl);
            TopName_lbl.Text = "مدیریت کاربران";
        }

        private void DashboardBackup_btn_Click(object sender, EventArgs e)
        {
            ShowTab(Backup_pnl);
            TopName_lbl.Text = "داده ها و بکاپ";
        }

        private void Users_dgv_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            UpdateRowNumbers(Users_dgv, "Counter_dgvc");
        }

        private void Users_dgv_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateRowNumbers(Users_dgv, "Counter_dgvc");
        }

        private void WithFace_rbt_CheckedChanged(object sender, EventArgs e)
        {
            if (!FI)
            {
                FI = true;

                RecID_txt.Visible = false;

                RecImage_pic.BringToFront();
                RecImage_pic.Visible = true;
            }
        }

        private void WithID_rbt_CheckedChanged(object sender, EventArgs e)
        {
            if (FI)
            {
                FI = false;

                RecImage_pic.Visible = false;

                RecID_txt.BringToFront();
                RecID_txt.Visible = true;
            }
        }

        private void RecImage_pic_Click(object sender, EventArgs e)
        {
            try
            {
                if (FI)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "jpg Files|*.jpg";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        RecImage_pic.Image = Image.FromFile(openFileDialog.FileName);

                        RecOK = true;
                    }
                }
            }
            catch
            {
                ShowMessage("Problems in the central part of the file", 8061);
            }
        }

        private void RecReset_btn_Click(object sender, EventArgs e)
        {
            RecImage_pic.Image = null;
            RecID_txt.Text = null;
            RecOK = false;
        }

        private async void RecSearch_btn_Click(object sender, EventArgs e)
        {
            if (FI && RecOK)
            {
                if (IsWorking)
                {
                    MessageBox.Show("We are working on previous Proccess");
                    return;
                }

                IsWorking = true;

                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    RecImage_pic.Image?.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    imageBytes = ms.ToArray();
                }

                var user = await FoundUser(imageBytes, AIAccuracy);

                if (user.UserID != null) FillUser(user);
                else MessageBox.Show("break");

                IsWorking = false;
            }
            else if (!FI)
            {
                if (IsWorking)
                {
                    MessageBox.Show("We are working on previous Proccess");
                    return;
                }

                IsWorking = true;

                DataRow user = Program.Users.ExtractionRow("ID", RecID_txt.Text);

                if (user != null) FillUser((user["ID"].ToString(), 100));
                else MessageBox.Show("break");

                IsWorking = false;
            }
        }

        private void ResultViewUser_btn_Click(object sender, EventArgs e)
        {
            DataRow opuser = Program.Users.ExtractionRow("ID", ResID);
            DataRow[] specifics = Program.Specifics.ExtractionRows("UserID", ResID);

            List<byte[]> images = new List<byte[]>();

            foreach (var item in specifics)
            {
                images.Add((byte[])item["Image"]);
            }


            UserView userView = new UserView(opuser,images);
            userView.ShowDialog();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }

        private void AddNUImages_btn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                NUImages = ExtractImageByFolder(dialog.SelectedPath);
                ShowImages(UserImagesNU_flp, NUImages);
            }
            else NUImages = null;
        }

        private void UserImageNU_pic_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "jpg Files|*.jpg";

            if (ofd.ShowDialog() == DialogResult.OK) UserImageNU_pic.Image = Image.FromFile(ofd.FileName);
            else ImageFromName("");
        }

        private async void SaveNU_btn_Click(object sender, EventArgs e)
        {
            if (IsWorking)
            {
                MessageBox.Show("We are working on previous Proccess");
                return;
            }

            IsWorking = true;

            DataRow newUser = Program.Users.DataTable.NewRow();

            string userid = Program.Users.CreateID("ID", 10);
            string username = UsernameNU_txt.Text;
            string fullname = FullNameNU_txt.Text;
            string workinghours = DateTime.Now.ToString();
            string description = DescriptionNU_txt.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullname))
            {
                MessageBox.Show("Please write username and fullname");
                return;
            }

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                UserImageNU_pic.Image?.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                imageBytes = ms.ToArray();
            }


            newUser.SetField("ID", userid);
            newUser.SetField("Username", username);
            newUser.SetField("FullName", fullname);
            newUser.SetField("PhoneNumber", "");
            newUser.SetField("WorkingHours", workinghours);
            newUser.SetField("Description", description);
            newUser.SetField("Image", imageBytes);

            if (NUImages != null)
            {
                bool result = await RegisterUser(newUser, NUImages, AIAccuracy);
                if (result)
                {
                    MessageBox.Show("Successfuly");

                    UsersCount2_lbl.Text = Program.Users.DataTable.Rows.Count.ToString();
                    SpecificsCount2_lbl.Text = Program.Specifics.DataTable.Rows.Count.ToString();
                    EmbeddedCount2_lbl.Text = Program.Embeddeds.DataTable.Rows.Count.ToString();
                    UserCounter2_lbl.Text = Program.Users.DataTable.Rows.Count.ToString();

                    DataRow[] specifics = Program.Specifics.ExtractionRows("UserID", userid);
                    List<byte[]> images = new List<byte[]>();

                    foreach (var item in specifics)
                    {
                        images.Add((byte[])item["Image"]);
                    }

                    UserProfile userProfile = new UserProfile(newUser,images);
                    userProfile.Width = this.ManagmentUsers_flp.Width - 27;
                    ManagmentUsers_flp.Controls.Add(userProfile);
                }
                else MessageBox.Show("break");
            }
            else
            {
                MessageBox.Show("You dont select images");
            }

            IsWorking = false;
        }

        private async void AddNUWithJSON_btn_Click(object sender, EventArgs e)
        {
            if (IsWorking)
            {
                MessageBox.Show("We are working on previous Proccess");
                return;
            }

            IsWorking = true;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON Files|*.json";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var users = ReadUsersFromJson(openFileDialog.FileName);

                foreach (var user in users)
                {
                    DataRow newUser = Program.Users.DataTable.NewRow();

                    string userid = Program.Users.CreateID("ID", 10);
                    string username = user.Username;
                    string fullname = user.FullName;
                    string workinghours = DateTime.Now.ToString();
                    string description = user.Description;

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullname))
                    {
                        MessageBox.Show("Please write username and fullname");
                        return;
                    }

                    byte[] imageBytes;
                    using (var fs = new FileStream(user.ImageProfilePath, FileMode.Open, FileAccess.Read))
                    using (var ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        byte[] bytes = ms.ToArray();
                        ms.Position = 0;

                        Image img = Image.FromStream(ms);
                        imageBytes = bytes;
                    }


                    newUser.SetField("ID", userid);
                    newUser.SetField("Username", username);
                    newUser.SetField("FullName", fullname);
                    newUser.SetField("PhoneNumber", "");
                    newUser.SetField("WorkingHours", workinghours);
                    newUser.SetField("Description", description);
                    newUser.SetField("Image", imageBytes);

                    var images = ExtractImageByFolder(user.ImageFolderPath);

                    if (images != null)
                    {
                        UsernameNU_txt.Text = username;
                        FullNameNU_txt.Text = fullname;
                        DescriptionNU_txt.Text = description;
                        UserImageNU_pic.Image = ByteToImage(imageBytes);
                        ShowImages(UserImagesNU_flp, images);


                        bool result = await RegisterUser(newUser, images, AIAccuracy);
                        if (result)
                        {
                            UsersCount2_lbl.Text = Program.Users.DataTable.Rows.Count.ToString();
                            SpecificsCount2_lbl.Text = Program.Specifics.DataTable.Rows.Count.ToString();
                            EmbeddedCount2_lbl.Text = Program.Embeddeds.DataTable.Rows.Count.ToString();
                            UserCounter2_lbl.Text = Program.Users.DataTable.Rows.Count.ToString();

                            DataRow[] specifics = Program.Specifics.ExtractionRows("UserID", userid);
                            List<byte[]> imagesb = new List<byte[]>();

                            foreach (var item in specifics)
                            {
                                imagesb.Add((byte[])item["Image"]);
                            }

                            UserProfile userProfile = new UserProfile(newUser, imagesb);
                            userProfile.Width = this.ManagmentUsers_flp.Width - 27;
                            ManagmentUsers_flp.Controls.Add(userProfile);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You dont select images");
                    }
                }

                MessageBox.Show("Successfuly");
            }


            IsWorking = false;
        }

        private void AccuracyLow_rbt_CheckedChanged(object sender, EventArgs e)
        {
            AIAccuracy = FaceRegistration.Accuracy.Low;
        }

        private void AccuracyIntermediate_rbt_CheckedChanged(object sender, EventArgs e)
        {
            AIAccuracy = FaceRegistration.Accuracy.Intermediate;
        }

        private void AccuracyHigh_rbt_CheckedChanged(object sender, EventArgs e)
        {
            AIAccuracy = FaceRegistration.Accuracy.High;
        }

        private void AccuracyIntricate_rbt_CheckedChanged(object sender, EventArgs e)
        {
            AIAccuracy = FaceRegistration.Accuracy.Intricate;
        }

        private void Sequential_rbt_CheckedChanged(object sender, EventArgs e)
        {
            MultiProgramming = false;
        }

        private void Parallel_rbt_CheckedChanged(object sender, EventArgs e)
        {
            MultiProgramming = true;
        }

        private void SimilarityModeEuclideanWeighted_rbt_CheckedChanged(object sender, EventArgs e)
        {
            SimilarityMode = FaceRecognition.SimilarityMode.EuclideanWeighted;
        }

        private void SimilarityModeCosineWeighted_rbt_CheckedChanged(object sender, EventArgs e)
        {
            SimilarityMode = FaceRecognition.SimilarityMode.CosineWeighted;
        }

        private void UserBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL Files|*.sql";

            if (saveFileDialog.ShowDialog() == DialogResult.OK) MessageBox.Show(ExportInserts(Program.Users.DataTable, saveFileDialog.FileName, "Users"));
        }

        private void SpecificsBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL Files|*.sql";

            if (saveFileDialog.ShowDialog() == DialogResult.OK) MessageBox.Show(ExportInserts(Program.Specifics.DataTable, saveFileDialog.FileName, "Specifics"));
        }

        private void EmbeddesBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "SQL Files|*.sql";

            if (saveFileDialog.ShowDialog() == DialogResult.OK) MessageBox.Show(ExportInserts(Program.Embeddeds.DataTable, saveFileDialog.FileName, "Embeddeds"));
        }

        private void LoadUserBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQL Files|*.sql";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(ImportInserts(openFileDialog.FileName));
                MessageBox.Show("For set change restart the app");
            }
        }

        private void LoadSpecificsBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQL Files|*.sql";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(ImportInserts(openFileDialog.FileName));
                MessageBox.Show("For set change restart the app");
            }
        }

        private void LoadEmbeddesBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SQL Files|*.sql";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(ImportInserts(openFileDialog.FileName));
                MessageBox.Show("For set change restart the app");
            }
        }

        private void DeleteUsersBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(ClearDataBase("Users"));
            SetMain(ID);
        }

        private void DeleteSpecificsBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(ClearDataBase("Specifics"));
            SetMain(ID);
        }

        private void DeleteEmbeddesBackup_llbl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show(ClearDataBase("Embeddeds"));
            SetMain(ID);
        }
    }
}
