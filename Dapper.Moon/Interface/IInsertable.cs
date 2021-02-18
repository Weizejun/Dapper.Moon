using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public interface IInsertable<T>
    {
        IInsertable<T> Insertable(T t);
        IInsertable<T> Insertable(List<T> list);
        IInsertable<T> TableName(string tableName);
        int BulkInsert();
        int Execute();
        long ExecuteIdentity();
        DataTable ToDataTable();
        SqlBuilderResult ToSql();
    }
}
