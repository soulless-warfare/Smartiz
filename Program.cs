using Microsoft.Data.Sqlite;

namespace Smartiz
{
    public static class DatabaseHelper
    {
        public static string GetOrCreateDatabasePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            string panelFolder = Path.Combine(appData, "Smartiz");
            string dbFolder = Path.Combine(panelFolder, "XLxDxB");
            Directory.CreateDirectory(dbFolder);

            string dbPath = Path.Combine(dbFolder, "Smartiz.db");
            bool newDb = !File.Exists(dbPath);

            if (newDb)
            {
                using (var stream = File.Create(dbPath)) { }
                CreateTables(dbPath);
            }

            return dbPath;
        }

        private static void CreateTables(string dbPath)
        {
            using (var connection = new SqliteConnection($"Data Source={dbPath};"))
            {
                connection.Open();

                string createAdmins = @"
                CREATE TABLE IF NOT EXISTS Admins (
                    ID TEXT PRIMARY KEY,
                    Username TEXT NOT NULL,
                    Password TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    PhoneNumber TEXT NOT NULL,
                    Email TEXT NOT NULL,
                    Image BLOB
                );";

                string createUsers = @"
                CREATE TABLE IF NOT EXISTS Users (
                    ID TEXT PRIMARY KEY,
                    Username TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    PhoneNumber TEXT,
                    WorkingHours TEXT,
                    Description TEXT,
                    Image BLOB
                );";

                string createEmbeddeds = @"
                CREATE TABLE IF NOT EXISTS Embeddeds (
                    EMID TEXT PRIMARY KEY,
                    EmbeddedID TEXT,
                    EmbeddedIndex INTEGER,
                    Embedded FLOAT
                );";

                string createSpecifics = @"
                CREATE TABLE IF NOT EXISTS Specifics (
                    SpecificID TEXT PRIMARY KEY,
                    UserID TEXT,
                    EmbeddedID TEXT,
                    Image BLOB
                );";

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = createUsers; cmd.ExecuteNonQuery();
                    cmd.CommandText = createAdmins; cmd.ExecuteNonQuery();
                    cmd.CommandText = createEmbeddeds; cmd.ExecuteNonQuery();
                    cmd.CommandText = createSpecifics; cmd.ExecuteNonQuery();
                }
            }
        }
    }

    internal static class Program
    {
        public static SqliteDataTable Admins { get; set; }
        public static SqliteDataTable Users { get; set; }
        public static SqliteDataTable Embeddeds { get; set; }
        public static SqliteDataTable Specifics { get; set; }
        public static string connectionString { get; set; }

        /// <summary>The main entry point for the application</summary>
        [STAThread]
        static void Main()
        {
            connectionString = $"Data Source={DatabaseHelper.GetOrCreateDatabasePath()}";

            Admins = new SqliteDataTable(connectionString, "Admins");
            Users = new SqliteDataTable(connectionString, "Users");
            Embeddeds = new SqliteDataTable(connectionString, "Embeddeds");
            Specifics = new SqliteDataTable(connectionString, "Specifics");

            var adminsEX = Admins.Connect();
            var usersEX = Users.Connect();
            var embeddedsEX = Embeddeds.Connect();
            var specificsEX = Specifics.Connect();

            if (adminsEX && usersEX && embeddedsEX && specificsEX)
            {
                ApplicationConfiguration.Initialize();

                using (var login = new Login())
                {
                    if (login.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(login.mainForm);
                    }
                }
            }
            else MessageBox.Show("We cant connect to database");
        }
    }
}