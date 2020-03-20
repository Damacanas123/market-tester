using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.AppConstants
{
    internal class CommonFolders
    {
        
        private const string FILE_DELIMITER = "\\";
        internal static string SeqNumsDir { get { return Engine.resourcePath + "Seqnums" + FILE_DELIMITER; } }
        internal static string LogDir { get { return Engine.resourcePath + "Log" + FILE_DELIMITER; } }
        internal static string OrderMessagesBaseDir { get { return Engine.resourcePath + "OrderMessages" + FILE_DELIMITER; } }


        internal static string ClOrdIdFilePath { get { return SeqNumsDir + "ClOrdId.seqnum"; } }
        internal static string NonProtocolOrderIDPath {get { return SeqNumsDir + "NonProtocolOrderID.seqnum"; } }
        internal static string OrderDBFilePath { get { return Engine.resourcePath + "orders.db"; } }
        internal static string ErrorLogPath { get { return LogDir + "ErrorLog.log"; } }
        internal static string DBErrorLogPath { get { return LogDir + "DBErrorLog.log"; } }

        internal static List<string> Dirs = new List<string>()
        {
            SeqNumsDir,LogDir,OrderMessagesBaseDir
        };

        internal static List<string> SeqNumFilePaths = new List<string>()
        {
            ClOrdIdFilePath,NonProtocolOrderIDPath
        };

        internal static List<string> FilePaths = new List<string>()
        {
            ErrorLogPath,DBErrorLogPath
        };

        //these paths are prepended with main resource path at initialization time of Engine. See EngineBootstrapper class
    }
}
