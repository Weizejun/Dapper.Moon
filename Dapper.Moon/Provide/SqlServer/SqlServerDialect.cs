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

        public override string Length(string field) { return $"len({field})"; }

        public override string IndexOf(string field, object val)
        {
            return $"charindex({field},{val}) - 1";
        }

        public override string Substring(string field, object b, object c)
        {
            return $"substring({field}, {b}, {c} + 1)";
        }

        public override string FirstOrDefault(string field)
        {
            return $"substring({field},1,1)";
        }

        public override string PadLeft(string field, object b, object c)
        {
            throw new Exception("function not supported sqlserver");
        }

        public override string PadRight(string field, object b, object c)
        {
            throw new Exception("function not supported sqlserver");
        }

        public override string Trim(string field)
        {
            throw new Exception("function not supported sqlserver");
        }

        public override string Year(string field)
        {
            return $"datepart(year, {field})";
        }

        public override string Month(string field)
        {
            return $"datepart(month, {field})";
        }

        public override string Day(string field)
        {
            return $"datepart(day, {field})";
        }

        public override string Hour(string field)
        {
            return $"datepart(hour, {field})";
        }
    }
}
