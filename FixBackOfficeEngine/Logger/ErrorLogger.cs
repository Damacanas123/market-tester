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
        internal static void LogError(Exception ex)
        {
            Util.AppendStringToFile(CommonFolders.ErrorLogPath, ex.ToString());
        }

        internal static void DBError(Exception ex)
        {
            Util.AppendStringToFile(CommonFolders.DBErrorLogPath, ex.ToString());
        }
    }
}
