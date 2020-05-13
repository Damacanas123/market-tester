using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackOfficeEngine.DB.SQLite
{
    public class TableField
    {
        internal string Name { get; }
        internal Type Type { get; }
        internal string Constraints { get; }
        internal int Length { get; }
        internal string DefaultValue { get; }

        internal TableField(string name,Type type,string constraints,int length)
        {
            Name = name;
            Type = type;
            Constraints = constraints;
            Length = length;
        }

        internal TableField(string name, Type type, string constraints, int length,string defaultValue)
        {
            Name = name;
            Type = type;
            Constraints = constraints;
            Length = length;
            DefaultValue = defaultValue;
        }
    }

    
}
