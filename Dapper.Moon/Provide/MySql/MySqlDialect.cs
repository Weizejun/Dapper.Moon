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

        public override string Length(string field) { return $"char_length({field})"; }

        public override string IndexOf(string field, object val)
        {
            return $"locate({field}, {val}) - 1";
        }

        public override string Year(string field)
        {
            return $"year({field})";
        }

        public override string Month(string field)
        {
            return $"month({field})";
        }

        public override string Day(string field)
        {
            return $"dayofmonth({field})";
        }

        public override string Hour(string field)
        {
            return $"hour({field})";
        }
    }
}
