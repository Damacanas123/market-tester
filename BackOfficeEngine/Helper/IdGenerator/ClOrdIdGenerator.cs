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
    
    public class ClOrdIdGenerator : BaseIDGenerator<ClOrdIdGenerator>
    {
        

        protected override object fileLock { get; set ; }
        protected override string filePath { get; set; }

        public bool PutQInTheBeginning { get; set; }

        /// <summary>
        /// do not call this constructor directly. Use Instance static field to get the instance of generator
        /// </summary>
        public ClOrdIdGenerator()
        {
            fileLock = new object();
            filePath = CommonFolders.ClOrdIdFilePath;
        }


        //an old requirement for mtxr. In future it may start to be used again. Checkbox on main page is commented out.
        //public new string GetNextId()
        //{

            
        //    if (PutQInTheBeginning)
        //        return "Q-" + base.GetNextId();
        //    else
        //        return base.GetNextId();
        //}

    }
}
