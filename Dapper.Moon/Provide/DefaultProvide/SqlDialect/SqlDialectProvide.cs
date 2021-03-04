using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    /// <summary>
    /// 数据库方言
    /// </summary>
    public abstract class SqlDialectProvide : ISqlDialect
    {
        public virtual string ParameterPrefix
        {
            get { return "@"; }
        }

        public virtual string ToDateTime(string field)
        {
            return $"cast({field} as datetime)";
        }

        public virtual string Between(object field, object a, object b)
        {
            return $"{field} between {a} and {b}";
        }

        public virtual string Concat(string column, string val)
        {
            return $"concat({column},{val})";
        }

        public abstract string SetSqlName(string name);
        public abstract string GetDate { get; }
        public abstract string Datediff(string dateType, string column, string dt);
        public abstract string IsNull(object column, object val);
        public abstract string Guid { get; }
        public abstract string Length(string field);

        public virtual string PartitionBy(object partitionByField, object orderByField, Moon.OrderBy orderBy = OrderBy.Asc)
        {
            return $"row_number() over(partition by {partitionByField} order by {orderByField} {orderBy.ToString()})";
        }

        public abstract string IndexOf(string field, object val);

        public virtual string PadLeft(string field, object b, object c)
        {
            return $"lpad({field}, {b}, {c})";
        }

        public virtual string PadRight(string field, object b, object c)
        {
            return $"lpad({field}, {b}, {c})";
        }

        public virtual string Replace(string field, object b, object c)
        {
            return $"replace({field}, {b}, {c})";
        }

        public virtual string Substring(string field, object b, object c)
        {
            return $"substr({field}, {b}, {c} + 1)";
        }

        public virtual string ToLower(string field)
        {
            return $"lower({field})";
        }

        public virtual string ToUpper(string field)
        {
            return $"upper({field})";
        }

        public virtual string Trim(string field)
        {
            return $"trim({field})";
        }

        public virtual string TrimEnd(string field)
        {
            return $"rtrim({field})";
        }

        public virtual string TrimStart(string field)
        {
            return $"ltrim({field})";
        }

        public virtual string FirstOrDefault(string field)
        {
            return $"substr({field},1,1)";
        }

        public abstract string Year(string field);
        public abstract string Month(string field);
        public abstract string Day(string field);
        public abstract string Hour(string field);

        public string Ceiling(string field)
        {
            return $"ceiling({field})";
        }

        public string Abs(string field)
        {
            return $"abs({field})";
        }
    }
}
