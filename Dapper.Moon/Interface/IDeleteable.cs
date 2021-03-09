using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public interface IDeleteable<T>
    {
        IDeleteable<T> Deleteable();
        IDeleteable<T> Deleteable(object[] pks);
        IDeleteable<T> Where(Expression<Func<T, bool>> where);
        IDeleteable<T> TableName(string tableName);
        int Execute();
        Task<int> ExecuteAsync();
        SqlBuilderResult ToSql();
    }
}
