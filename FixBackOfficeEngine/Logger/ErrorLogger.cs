using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.Helper;
using BackOfficeEngine.AppConstants;

namespace BackOfficeEngine.Logger
{
    internal class ErrorLogger
    {
        private const string LOG_DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.ffffff";
        internal static void LogError(Exception ex)
        {
            Util.AppendStringToFile(CommonFolders.ErrorLogPath, DateTime.Now.ToString(LOG_DATE_FORMAT) + "\n" +  ex.ToString());
        }

        internal static void DBError(Exception ex)
        {
            Util.AppendStringToFile(CommonFolders.DBErrorLogPath,DateTime.Now.ToString(LOG_DATE_FORMAT) + "\n" + ex.ToString());
        }
    }
}
