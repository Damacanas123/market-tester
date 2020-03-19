using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using BackOfficeEngine.Helper;
using BackOfficeEngine.AppConstants;

namespace BackOfficeEngine.Helper.IdGenerator
{
    internal class ClOrdIdGenerator : BaseIDGenerator<ClOrdIdGenerator>
    {
        

        protected override object fileLock { get; set ; }
        protected override string filePath { get; set; }

        public ClOrdIdGenerator()
        {
            fileLock = new object();
            filePath = CommonFolders.ClOrdIdFilePath;
        }

    }
}
