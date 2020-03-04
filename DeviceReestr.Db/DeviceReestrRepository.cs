using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace DeviceReestr.Db
{
    public class DeviceReestrRepository
    {
        public string DbFile => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\DeviceReestr.sqlite";

        public SQLiteConnection DbConnection()
        {
            return new SQLiteConnection("Data Source=" + DbFile);
        }
    }
}