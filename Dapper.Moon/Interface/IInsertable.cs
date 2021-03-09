using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public interface IInsertable<T>
    {
        IInsertable<T> Insertable(T t);
        IInsertable<T> Insertable(List<T> list);
        IInsertable<T> TableName(string tableName);
        int BulkInsert();
        Task<int> BulkInsertAsync();
        int Execute();
        Task<int> ExecuteAsync();
        long ExecuteIdentity();
        Task<long> ExecuteIdentityAsync();
        DataTable ToDataTable();
        SqlBuilderResult ToSql();
    }
}
