using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public class DbTableColumnInfo
    {
        public string ColumnName { get; set; }
        public string IsIdentity { get; set; }
        public string IsPrimaryKey { get; set; }
        public string ColumnType { get; set; }
        public string IsNull { get; set; }
    }
}
