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
    }
}
