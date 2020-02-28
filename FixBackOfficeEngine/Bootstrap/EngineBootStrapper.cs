using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BackOfficeEngine.AppConstants;
using BackOfficeEngine.Model;
using BackOfficeEngine.DB.SQLite;

namespace BackOfficeEngine.Bootstrap
{
    internal class EngineBootstrapper
    {
        internal static void Bootstrap()
        {
            CreateDirectories();
        }
        
        private static void CreateDirectories()
        {
            if (Engine.resourcePath[Engine.resourcePath.Length - 1] != '\\')
            {
                Engine.resourcePath += "\\";
            }
            CreateDirectory(CommonFolders.SeqNumsDir);
            CreateDirectory(CommonFolders.LogDir);
            CreateFile(CommonFolders.ClOrdIdFilePath);
            CreateFile(CommonFolders.NonProtocolOrderIDPath);
            CreateFile(CommonFolders.ErrorLogPath);
            CreateFile(CommonFolders.DBErrorLogPath);
            CreateDatabaseTables();

        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void CreateFile(string path)
        {
            if (!File.Exists(path))
            {
                FileStream f = File.Create(path);
                f.Write(new byte[] { (byte)'1' }, 0, 1);
                f.Dispose();
            }
        }

        private static void CreateDatabaseTables()
        {
            using(SQLiteHandler sqliteHandler = new SQLiteHandler())
            {
                sqliteHandler.CreateTable(new Order());
                sqliteHandler.CreateTable(new Position());
            }
        }
    }
}
