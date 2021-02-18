using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    /// <summary>
    /// SqlServer方言
    /// </summary>
    public class SqlServerDialect : SqlDialectProvide
    {
        public override string SetSqlName(string name)
        {
            return $"[{name}]";
        }

        public override string GetDate { get { return "getdate()"; } }

        public override string Datediff(string dateType, string column, string dt)
        {
            return $"datediff({dateType},{column},{dt})";
        }

        public override string IsNull(object column, object val)
        {
            if (val == null || val.ToString() == "") val = "''";
            return $"isnull({column},{val})";
        }

        public override string Guid { get { return "REPLACE(NEWID(), '-', '')"; } }
    }
}
