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
        string Between(object field, object a, object b);
        string SetSqlName(string name);
        string IsNull(object column, object val);
        string Guid { get; }
        string PartitionBy(object partitionByField, object orderByField, Moon.OrderBy orderBy = OrderBy.Asc);

        string Concat(string column, string val);
        string Length(string field);
        string IndexOf(string field, object val);
        string PadLeft(string field, object b, object c);
        string PadRight(string field, object b, object c);
        string Replace(string field, object b, object c);
        string Substring(string field, object b, object c);
        string ToLower(string field);
        string ToUpper(string field);
        string Trim(string field);
        string TrimEnd(string field);
        string TrimStart(string field);
        string FirstOrDefault(string field);

        string Year(string field);
        string Month(string field);
        string Day(string field);
        string Hour(string field);
        string ToDateTime(string field);
        string Datediff(string dateType, string column, string dt);
        string GetDate { get; }

        string Ceiling(string field);
        string Abs(string field);
    }
}
