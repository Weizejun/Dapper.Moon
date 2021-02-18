using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public class MySqlDialect : SqlDialectProvide
    {
        public override string SetSqlName(string name)
        {
            return $"`{name}`";
        }

        public override string GetDate { get { return "now()"; } }

        public override string Datediff(string dateType, string column, string dt)
        {
            return $"timestampdiff({dateType},{column},{dt})";
        }

        public override string IsNull(object column, object val)
        {
            if (val == null || val.ToString() == "") val = "''";
            return $"ifnull({column},{val})";
        }

        public override string Guid { get { return "md5(uuid())"; } }
    }
}
