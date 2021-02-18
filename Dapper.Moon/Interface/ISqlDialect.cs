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
    public interface ISqlDialect
    {
        string ParameterPrefix { get; }
        string ToDateTime(string field);
        string Between(object field, object a, object b);
        string Concat(string column, string val);
        string SetSqlName(string name);
        string GetDate { get; }
        string Datediff(string dateType, string column, string dt);
        string IsNull(object column, object val);
        string Guid { get; }
    }
}
