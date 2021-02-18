using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public interface IUpdateable<T>
    {
        IUpdateable<T> Updateable(T t);
        IUpdateable<T> Updateable();
        IUpdateable<T> Where(Expression<Func<T, bool>> where);
        IUpdateable<T> SetColumns(Expression<Func<T, object>> columns);
        /// <summary>
        /// 忽略列
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns);
        IUpdateable<T> SetColumns(string sql, object param = null);
        IUpdateable<T> TableName(string tableName);
        int Execute();
        SqlBuilderResult ToSql();
    }
}
