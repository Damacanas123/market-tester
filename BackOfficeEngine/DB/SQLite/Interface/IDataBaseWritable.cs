using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BackOfficeEngine.DB.SQLite;

namespace BackOfficeEngine.DB.SQLite
{
    internal interface IDataBaseWritable
    {
        string TableName { get; }
        
        Dictionary<string,TableField> Fields { get; }
        Dictionary<string,object>  Values { get; }
        string DatabaseID { get; }
        
    }
}
