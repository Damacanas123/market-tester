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

        internal bool PutQInTheBeginning { get; set; }

        public ClOrdIdGenerator()
        {
            fileLock = new object();
            filePath = CommonFolders.ClOrdIdFilePath;
        }

        public new string GetNextId()
        {
            if (PutQInTheBeginning)
                return "Q-" + base.GetNextId();
            else
                return base.GetNextId();
        }

    }
}
