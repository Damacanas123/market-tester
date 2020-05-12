using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BackOfficeEngine.AppConstants;
using BackOfficeEngine.Model;
using BackOfficeEngine.DB.SQLite;
using BackOfficeEngine.Helper;

namespace BackOfficeEngine.Bootstrap
{
    internal class EngineBootstrapper
    {
        internal static List<Order> Bootstrap()
        {
            Fix.ITXRBootStrap();
            CreateDirectories();
            CreateFiles();
            CreateFilesSeqNum();
            CreateDatabaseTables();
            List<Order> orders = LoadOrders();
            LoadPositions();
            return orders;

        }

        

        private static void CreateDirectories()
        {
            if (Engine.resourcePath[Engine.resourcePath.Length - 1] != '\\')
            {
                Engine.resourcePath += "\\";
            }
            foreach(string directory in CommonFolders.Dirs)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            
        }

        private static void CreateFilesSeqNum()
        {
            foreach(string path in CommonFolders.SeqNumFilePaths)
            {
                if (!File.Exists(path))
                {
                    FileStream f = File.Create(path);
                    f.Write(new byte[] { (byte)'1' }, 0, 1);
                    f.Dispose();
                }
            }
            
        }

        private static void CreateFiles()
        {
            foreach(string path in CommonFolders.FilePaths)
            {
                if (!File.Exists(path))
                {
                    FileStream f = File.Create(path);
                    f.Dispose();
                }
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

        private static List<Order> LoadOrders()
        {
            using(SQLiteHandler handler = new SQLiteHandler())
            {
                return handler.GetAllOrders();
            }
        }

        private static void LoadPositions()
        {
            List<Position> positions = new List<Position>();
            using(SQLiteHandler handler = new SQLiteHandler())
            {
                positions = handler.GetPositions();
            }
            foreach(Position position in positions)
            {
                position.account.LoadPosition(position);
            }
        }
    }
}
