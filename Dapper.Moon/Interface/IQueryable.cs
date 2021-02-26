using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Dapper.Moon
{
    public interface IQueryable<T>
    {
        IQueryable<T> Where(Expression<Func<T, bool>> where);
        IQueryable<T> Where(string where,object dynamic = null);
        IQueryable<T> TableName(string tableName);

        int Count(Expression<Func<T, object>> field);
        TResult Sum<TResult>(Expression<Func<T, TResult>> field);
        TResult Avg<TResult>(Expression<Func<T, TResult>> field);
        TResult Max<TResult>(Expression<Func<T, TResult>> field);
        TResult Min<TResult>(Expression<Func<T, TResult>> field);

        IQueryable<T> OrderBy(Expression<Func<T, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T> Select(Expression<Func<T, object>> field);
        IQueryable<T> Select(string field);
        IQueryable<T> GroupBy(Expression<Func<T, object>> field);

        List<T> ToList();
        T First();

        TResult First<TResult>();
        List<TResult> ToList<TResult>();

        IQueryable<T> Distinct();
        DataTable ToDataTable();
        IQueryable<T> Take(int limit);
        QueryPageResult<TResult> ToPageList<TResult>(int offset, int limit);
        int Into<TTarget>(Expression<Func<TTarget, object>> field = null);
        SqlBuilderResult ToSql();
        IQueryable<T> UseSqlServer(SqlServerOption option);
        IQueryable<T> UseMySql(MySqlOption option);
        IQueryable<T> UseOracle(OracleOption option);

        IQueryable<T> Union();
        IQueryable<T> UnionAll();
    }

    public interface IQueryable<T, T2> : IQueryable<T>
    {
        IQueryable<T, T2> TableName(string tableName,string tableName2);
        IQueryable<T, T2> Where(Expression<Func<MoonFunc<T, T2>, bool>> where);
        new IQueryable<T, T2> Where(string where, object dynamic = null);

        int Count(Expression<Func<MoonFunc<T, T2>, object>> field);
        TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field);
        TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field);
        TResult Max<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field);
        TResult Min<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field);

        IQueryable<T, T2> OrderBy(Expression<Func<MoonFunc<T, T2>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        new IQueryable<T, T2> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T, T2> Select(Expression<Func<MoonFunc<T, T2>, object>> field);
        new IQueryable<T, T2> Select(string field);

        IQueryable<T, T2> LeftJoin(Expression<Func<MoonFunc<T, T2>, bool>> join);
        IQueryable<T, T2> LeftJoin(string join, object dynamic = null);

        IQueryable<T, T2> InnerJoin(Expression<Func<MoonFunc<T, T2>, bool>> join);
        IQueryable<T, T2> InnerJoin(string join, object dynamic = null);
        new IQueryable<T, T2> Union();
        new IQueryable<T, T2> UnionAll();

        new IQueryable<T, T2> Take(int limit);

        IQueryable<T, T2> GroupBy(Expression<Func<MoonFunc<T, T2>, object>> field);
        new IQueryable<T, T2> Distinct();

        new IQueryable<T, T2> UseSqlServer(SqlServerOption option);
        new IQueryable<T, T2> UseMySql(MySqlOption option);
        new IQueryable<T, T2> UseOracle(OracleOption option);
    }

    public interface IQueryable<T, T2, T3> : IQueryable<T>
    {
        IQueryable<T, T2, T3> TableName(string tableName, string tableName2, string tableName3);
        IQueryable<T, T2, T3> Where(Expression<Func<MoonFunc<T, T2, T3>, bool>> where);
        new IQueryable<T, T2, T3> Where(string where, object dynamic = null);

        int Count(Expression<Func<MoonFunc<T, T2, T3>, object>> field);
        TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field);
        TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field);
        TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field);
        TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field);

        IQueryable<T, T2, T3> OrderBy(Expression<Func<MoonFunc<T, T2, T3>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        new IQueryable<T, T2, T3> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T, T2, T3> Select(Expression<Func<MoonFunc<T, T2, T3>, object>> field);
        new IQueryable<T, T2, T3> Select(string field);

        IQueryable<T, T2, T3> LeftJoin(Expression<Func<MoonFunc<T, T2, T3>, bool>> join);
        IQueryable<T, T2, T3> LeftJoin(string join, object dynamic = null);
        IQueryable<T, T2, T3> InnerJoin(Expression<Func<MoonFunc<T, T2, T3>, bool>> join);
        IQueryable<T, T2, T3> InnerJoin(string join, object dynamic = null);
        new IQueryable<T, T2, T3> Union();
        new IQueryable<T, T2, T3> UnionAll();

        new IQueryable<T, T2, T3> Take(int limit);

        IQueryable<T, T2, T3> GroupBy(Expression<Func<MoonFunc<T, T2, T3>, object>> field);
        new IQueryable<T, T2, T3> Distinct();

        new IQueryable<T, T2, T3> UseSqlServer(SqlServerOption option);
        new IQueryable<T, T2, T3> UseMySql(MySqlOption option);
        new IQueryable<T, T2, T3> UseOracle(OracleOption option);
    }

    public interface IQueryable<T, T2, T3, T4> : IQueryable<T>
    {
        IQueryable<T, T2, T3, T4> TableName(string tableName, string tableName2, string tableName3, string tableName4);
        IQueryable<T, T2, T3, T4> Where(Expression<Func<MoonFunc<T, T2, T3, T4>, bool>> where);
        new IQueryable<T, T2, T3, T4> Where(string where, object dynamic = null);

        int Count(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field);
        TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field);
        TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field);
        TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field);
        TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field);

        IQueryable<T, T2, T3, T4> OrderBy(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        new IQueryable<T, T2, T3, T4> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T, T2, T3, T4> Select(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field);
        new IQueryable<T, T2, T3, T4> Select(string field);

        IQueryable<T, T2, T3, T4> LeftJoin(Expression<Func<MoonFunc<T, T2, T3, T4>, bool>> join);
        IQueryable<T, T2, T3, T4> LeftJoin(string join, object dynamic = null);
        IQueryable<T, T2, T3, T4> InnerJoin(Expression<Func<MoonFunc<T, T2, T3, T4>, bool>> join);
        IQueryable<T, T2, T3, T4> InnerJoin(string join, object dynamic = null);
        new IQueryable<T, T2, T3, T4> Union();
        new IQueryable<T, T2, T3, T4> UnionAll();

        new IQueryable<T, T2, T3, T4> Take(int limit);

        IQueryable<T, T2, T3, T4> GroupBy(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field);
        new IQueryable<T, T2, T3, T4> Distinct();

        new IQueryable<T, T2, T3, T4> UseSqlServer(SqlServerOption option);
        new IQueryable<T, T2, T3, T4> UseMySql(MySqlOption option);
        new IQueryable<T, T2, T3, T4> UseOracle(OracleOption option);
    }

    public interface IQueryable<T, T2, T3, T4, T5> : IQueryable<T>
    {
        IQueryable<T, T2, T3, T4, T5> TableName(string tableName, string tableName2, string tableName3, string tableName4, string tableName5);
        IQueryable<T, T2, T3, T4, T5> Where(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, bool>> where);
        new IQueryable<T, T2, T3, T4, T5> Where(string where, object dynamic = null);

        int Count(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field);
        TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field);
        TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field);
        TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field);
        TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field);

        IQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        new IQueryable<T, T2, T3, T4, T5> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T, T2, T3, T4, T5> Select(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field);
        new IQueryable<T, T2, T3, T4, T5> Select(string field);

        IQueryable<T, T2, T3, T4, T5> LeftJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, bool>> join);
        IQueryable<T, T2, T3, T4, T5> LeftJoin(string join, object dynamic = null);
        IQueryable<T, T2, T3, T4, T5> InnerJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, bool>> join);
        IQueryable<T, T2, T3, T4, T5> InnerJoin(string join, object dynamic = null);
        new IQueryable<T, T2, T3, T4, T5> Union();
        new IQueryable<T, T2, T3, T4, T5> UnionAll();

        new IQueryable<T, T2, T3, T4, T5> Take(int limit);

        IQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field);
        new IQueryable<T, T2, T3, T4, T5> Distinct();

        new IQueryable<T, T2, T3, T4, T5> UseSqlServer(SqlServerOption option);
        new IQueryable<T, T2, T3, T4, T5> UseMySql(MySqlOption option);
        new IQueryable<T, T2, T3, T4, T5> UseOracle(OracleOption option);
    }

    public interface IQueryable<T, T2, T3, T4, T5, T6> : IQueryable<T>
    {
        IQueryable<T, T2, T3, T4, T5, T6> TableName(string tableName, string tableName2, string tableName3, string tableName4, string tableName5, string tableName6);
        IQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, bool>> where);
        new IQueryable<T, T2, T3, T4, T5, T6> Where(string where, object dynamic = null);

        int Count(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field);
        TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field);
        TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field);
        TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field);
        TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field);

        IQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        new IQueryable<T, T2, T3, T4, T5, T6> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc);
        IQueryable<T, T2, T3, T4, T5, T6> Select(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field);
        new IQueryable<T, T2, T3, T4, T5, T6> Select(string field);

        IQueryable<T, T2, T3, T4, T5, T6> LeftJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, bool>> join);
        IQueryable<T, T2, T3, T4, T5, T6> LeftJoin(string join, object dynamic = null);
        IQueryable<T, T2, T3, T4, T5, T6> InnerJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, bool>> join);
        IQueryable<T, T2, T3, T4, T5, T6> InnerJoin(string join, object dynamic = null);
        new IQueryable<T, T2, T3, T4, T5, T6> Union();
        new IQueryable<T, T2, T3, T4, T5, T6> UnionAll();

        new IQueryable<T, T2, T3, T4, T5, T6> Take(int limit);

        IQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field);
        new IQueryable<T, T2, T3, T4, T5, T6> Distinct();

        new IQueryable<T, T2, T3, T4, T5, T6> UseSqlServer(SqlServerOption option);
        new IQueryable<T, T2, T3, T4, T5, T6> UseMySql(MySqlOption option);
        new IQueryable<T, T2, T3, T4, T5, T6> UseOracle(OracleOption option);
    }
}