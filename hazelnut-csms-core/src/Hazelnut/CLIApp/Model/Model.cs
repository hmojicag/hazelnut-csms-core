namespace Hazelnut.CLIApp.Model {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;

    public class HazelnutCLIContext: DbContext {
        public DbSet<HUser> HUsers { get; set; }
        public DbSet<HCloudStorageDrive> HCloudStorageDrives { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbFullPath = GetDBPath();
            //Find a better logging here
            Console.WriteLine("SQLite DB path: " + dbFullPath);
            optionsBuilder.UseSqlite("Data Source=" + dbFullPath);
        }


        private string GetDBPath() {
            // Check:https://weka.wikispaces.com/Where+is+my+home+directory+located%3F
            string homePath;
            if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ) {
                homePath = Environment.GetEnvironmentVariable("USERPROFILE");
            } else {//Linux or OSX
                homePath = Environment.GetEnvironmentVariable("HOME");
            }
            string s = Path.DirectorySeparatorChar.ToString();
            string appFolderName = "HazelnutCSMS";
            string appFolderPath = homePath + s + appFolderName; 
            string dbName = "hazelnut-csms.db";
            string dbFullPath = appFolderPath + s + dbName;
            //Create App Folder if it doesn't exists
            Directory.CreateDirectory(appFolderPath);

            return dbFullPath;
        }

    }

    public class HUser {
        public int HUserId { get; set; }
        public string HUserName { get; set; }
        public string SyncType { get; set; }

        public List<HCloudStorageDrive> HCloudStorageDrives { get; set; }
    }

    public class HCloudStorageDrive {
        public int HCloudStorageDriveId { get; set; }
        public string Type { get; set; }
        public string Config { get; set; }

        public int HUserId { get; set; }
        public HUser Huser { get; set; }
    }


}