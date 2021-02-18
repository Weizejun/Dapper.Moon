using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public abstract class QueryableProvide<T> : IQueryable<T>
    {
        #region
        protected IRepository Repository { get; set; }
        protected ExpressionProvide ExpressionProvideObj { get; set; }
        protected SqlServerOption SqlServerOption { get; set; }
        protected SqlBuilderResult __Where { get; set; }
        protected string MasterTableName { get; set; }
        protected List<string> TableNames { get; set; }
        protected List<MapperTable> JoinTables { get; set; }
        protected string SelectColumns { get; set; }
        protected int? Offset { get; set; }
        protected int? Limit { get; set; }
        protected OrderBy __OrderBy { get; set; }
        protected string OrderByField { get; set; }
        protected string GroupByField { get; set; }
        protected string FunctionSql { get; set; }
        protected SqlBuilderResult Function { get; set; }
        protected List<SqlBuilderResult> Joins { get; set; }
        protected MapperTable SelectIntoTable { get; set; }
        protected string IntoTableColumn { get; set; }
        protected bool IsDistinct { get; set; }
        #endregion

        public QueryableProvide(IRepository _Repository)
        {
            Repository = _Repository;

            MapperTable classMapperT = ClassMapper.Mapping<T>();
            MasterTableName = Repository.SqlDialect.SetSqlName(classMapperT.TableName);
            TableNames = new List<string>();
            JoinTables = new List<MapperTable>();
            JoinTables.Add(classMapperT);

            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect , JoinTables.ToArray());
        }

        #region
        public IQueryable<T> TableName(string tableName)
        {
            MasterTableName = Repository.SqlDialect.SetSqlName(tableName);
            return this;
        }

        protected void _Where(Expression where, bool isPrefix = false)
        {
            __Where = ExpressionProvideObj.ExpressionRouter(where, isPrefix);
            if (__Where != null)
            {
                __Where.Sql = __Where.Sql.Insert(0, "where ");
            }
        }

        protected void _Where(string where, object dynamic = null)
        {
            if (!string.IsNullOrWhiteSpace(where))
            {
                __Where = new SqlBuilderResult()
                {
                    Sql = "where " + where
                };
                var dp = CommonUtils.GetDynamicParameters(dynamic);
                __Where.DynamicParameters = dp;
            }
        }

        public IQueryable<T> Where(string where, object dynamic = null)
        {
            _Where(where, dynamic);
            return this;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> where)
        {
            _Where(where);
            return this;
        }

        protected int _Count(Expression field, bool isPrefix = false)
        {
            CommonUtils.ExpressionCheck(field, true);
            Function = ExpressionProvideObj.ExpressionRouter(field, isPrefix);
            FunctionSql = $"count({Function.Sql})";
            SqlBuilderResult result = ToSql();
            return Repository.ExecuteScalar<int>(result.Sql, result.DynamicParameters);
        }

        public int Count(Expression<Func<T, object>> field)
        {
            return _Count(field);
        }

        protected TResult _Sum<TResult>(Expression field, bool isPrefix = false)
        {
            CommonUtils.ExpressionCheck(field, true);
            Function = ExpressionProvideObj.ExpressionRouter(field, isPrefix);
            FunctionSql = $"sum({Function.Sql})";
            SqlBuilderResult result = ToSql();
            return Repository.ExecuteScalar<TResult>(result.Sql, result.DynamicParameters);
        }

        public TResult Sum<TResult>(Expression<Func<T, TResult>> field)
        {
            return _Sum<TResult>(field);
        }

        protected TResult _Avg<TResult>(Expression field, bool isPrefix = false)
        {
            CommonUtils.ExpressionCheck(field, true);
            Function = ExpressionProvideObj.ExpressionRouter(field, isPrefix);
            FunctionSql = $"avg({Function.Sql})";
            SqlBuilderResult result = ToSql();
            return Repository.ExecuteScalar<TResult>(result.Sql, result.DynamicParameters);
        }

        public TResult Avg<TResult>(Expression<Func<T, TResult>> field)
        {
            return _Avg<TResult>(field);
        }

        protected TResult _Max<TResult>(Expression field, bool isPrefix = false)
        {
            CommonUtils.ExpressionCheck(field, true);
            Function = ExpressionProvideObj.ExpressionRouter(field, isPrefix);
            FunctionSql = $"max({Function.Sql})";
            SqlBuilderResult result = ToSql();
            return Repository.ExecuteScalar<TResult>(result.Sql, result.DynamicParameters);
        }

        public TResult Max<TResult>(Expression<Func<T, TResult>> field)
        {
            return _Max<TResult>(field);
        }

        protected TResult _Min<TResult>(Expression field, bool isPrefix = false)
        {
            CommonUtils.ExpressionCheck(field, true);
            Function = ExpressionProvideObj.ExpressionRouter(field, isPrefix);
            FunctionSql = $"min({Function.Sql})";
            SqlBuilderResult result = ToSql();
            return Repository.ExecuteScalar<TResult>(result.Sql, result.DynamicParameters);
        }

        public TResult Min<TResult>(Expression<Func<T, TResult>> field)
        {
            return _Min<TResult>(field);
        }

        protected void _Take(int limit)
        {
            Limit = limit;
        }

        public IQueryable<T> Take(int limit)
        {
            _Take(limit);
            return this;
        }

        protected QueryPageResult<TResult> _ToPageList<TResult>(int offset, int limit)
        {
            if (string.IsNullOrWhiteSpace(OrderByField))
            {
                throw new Exception("the sort condition is missing");
            }

            Offset = offset;
            Limit = limit;
            SqlBuilderResult result = ToSql();
            return Repository.QueryPage<TResult>(result.Sql, result.DynamicParameters);
        }

        public QueryPageResult<TResult> ToPageList<TResult>(int offset, int limit)
        {
            return _ToPageList<TResult>(offset, limit);
        }

        protected List<TResult> _ToList<TResult>()
        {
            SqlBuilderResult result = ToSql();
            return Repository.Query<TResult>(result.Sql, result.DynamicParameters);
        }

        protected TResult _First<TResult>()
        {
            Limit = 1;
            SqlBuilderResult result = ToSql();
            return Repository.QueryFirst<TResult>(result.Sql, result.DynamicParameters);
        }

        public T First()
        {
            return _First<T>();
        }

        public List<T> ToList()
        {
            return _ToList<T>();
        }

        public TResult First<TResult>()
        {
            return _First<TResult>();
        }

        public List<TResult> ToList<TResult>()
        {
            return _ToList<TResult>();
        }

        public DataTable ToDataTable()
        {
            SqlBuilderResult result = ToSql();
            return Repository.Query(result.Sql, result.DynamicParameters);
        }

        protected void _OrderBy(Expression field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc, bool isPrefix = false)
        {
            __OrderBy = orderBy;
            OrderByField = ExpressionProvideObj.ExpressionRouter(field, isPrefix).Sql;
        }

        public IQueryable<T> OrderBy(Expression<Func<T, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            _OrderBy(field, orderBy);
            return this;
        }

        protected void _OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            __OrderBy = orderBy;
            OrderByField = field;
        }

        public IQueryable<T> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            _OrderBy(field, orderBy);
            return this;
        }

        protected void _GroupBy(Expression field, bool isPrefix = false)
        {
            GroupByField = ExpressionProvideObj.ExpressionRouter(field, isPrefix).Sql;
            if (string.IsNullOrWhiteSpace(SelectColumns))
            {
                SelectColumns = GroupByField;
            }
        }

        public IQueryable<T> GroupBy(Expression<Func<T, object>> field)
        {
            _GroupBy(field);
            return this;
        }

        protected void _Select(string field)
        {
            SelectColumns = field;
        }

        protected void _Select(Expression field, bool isPrefix = false)
        {
            var ep = ExpressionProvideObj.ExpressionRouter(field, isPrefix);
            SelectColumns = ep.Sql;
            if (__Where == null)
            {
                __Where = new SqlBuilderResult();
                __Where.DynamicParameters = new DynamicParameters();
            }
            if (ep.DynamicParameters.ParameterNames.Count() > 0)
            {
                __Where.DynamicParameters.AddDynamicParams(ep.DynamicParameters);
            }
        }

        public IQueryable<T> Select(string field)
        {
            _Select(field);
            return this;
        }

        public IQueryable<T> Select(Expression<Func<T, object>> field)
        {
            _Select(field);
            return this;
        }

        protected void _Join(string join, object dynamic = null)
        {
            if (!string.IsNullOrWhiteSpace(join))
            {
                Joins = new List<SqlBuilderResult>();
                Joins.Add(new SqlBuilderResult()
                {
                    Sql = join,
                    DynamicParameters = CommonUtils.GetDynamicParameters(dynamic)
                });
            }
        }

        protected void _Join(string joinType, Expression join)
        {
            if (Joins == null)
            {
                Joins = new List<SqlBuilderResult>();
            }
            if (Joins.Count + 1 > TableNames.Count)
            {
                throw new Exception("table join error");
            }
            var eb = ExpressionProvideObj.ExpressionRouter(join, true);
            Joins.Add(eb);
            string asName = "t" + (Joins.Count + 1);
            string tableName = TableNames[Joins.Count - 1];
            eb.Sql = eb.Sql.Insert(0, $"{joinType} {tableName} {asName} on ");
        }

        protected void _Distinct()
        {
            IsDistinct = true;
        }

        public IQueryable<T> Distinct()
        {
            _Distinct();
            return this;
        }

        public int Into<TTarget>(Expression<Func<TTarget, object>> field = null)
        {
            SelectIntoTable = ClassMapper.Mapping<TTarget>();
            ExpressionProvideObj.AppendMappingTable(SelectIntoTable);
            if (field != null)
            {
                IntoTableColumn = ExpressionProvideObj.ExpressionRouter(field).Sql;
            }
            SqlBuilderResult result = ToSql();
            return Repository.Execute(result.Sql, result.DynamicParameters);
        }

        protected void _UseSqlServer(SqlServerOption option) { SqlServerOption = option; }
        protected void _UseMySql(MySqlOption option) { }
        protected void _UseOracle(OracleOption option) { }

        public IQueryable<T> UseSqlServer(SqlServerOption option)
        {
            _UseSqlServer(option);
            return this;
        }
        public IQueryable<T> UseMySql(MySqlOption option) { return this; }
        public IQueryable<T> UseOracle(OracleOption option) { return this; }
        #endregion

        public abstract SqlBuilderResult ToSql();
    }

    #region T2-T6
    public abstract class QueryableProvide<T, T2> : QueryableProvide<T>, IQueryable<T, T2>
    {
        public QueryableProvide(IRepository _Repository)
            : base(_Repository)
        {
            MapperTable classMapperT2 = ClassMapper.Mapping<T2>();
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT2.TableName));

            JoinTables.Add(classMapperT2);
            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, JoinTables.ToArray());
        }

        public IQueryable<T, T2> TableName(string tableName, string tableName2)
        {
            TableNames.Clear();
            MasterTableName = Repository.SqlDialect.SetSqlName(tableName);
            TableNames.Add(Repository.SqlDialect.SetSqlName(tableName2));
            return this;
        }

        public IQueryable<T, T2> Where(Expression<Func<MoonFunc<T, T2>, bool>> where)
        {
            base._Where(where, true);
            return this;
        }

        public new IQueryable<T, T2> Where(string where, object dynamic = null)
        {
            base._Where(where, dynamic);
            return this;
        }

        public int Count(Expression<Func<MoonFunc<T, T2>, object>> field)
        {
            return base._Count(field, true);
        }

        public TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field)
        {
            return base._Sum<TResult>(field, true);
        }

        public TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field)
        {
            return base._Avg<TResult>(field, true);
        }

        public TResult Max<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field)
        {
            return base._Max<TResult>(field, true);
        }

        public TResult Min<TResult>(Expression<Func<MoonFunc<T, T2>, TResult>> field)
        {
            return base._Min<TResult>(field, true);
        }

        public IQueryable<T, T2> OrderBy(Expression<Func<MoonFunc<T, T2>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            base._OrderBy(field, orderBy, true);
            return this;
        }

        public new IQueryable<T, T2> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            __OrderBy = orderBy;
            OrderByField = field;
            return this;
        }

        public IQueryable<T, T2> GroupBy(Expression<Func<MoonFunc<T, T2>, object>> field)
        {
            _GroupBy(field, true);
            return this;
        }

        public IQueryable<T, T2> Select(Expression<Func<MoonFunc<T, T2>, object>> field)
        {
            base._Select(field, true);
            return this;
        }

        public new IQueryable<T, T2> Select(string field)
        {
            base._Select(field);
            return this;
        }

        public IQueryable<T, T2> LeftJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public IQueryable<T, T2> LeftJoin(Expression<Func<MoonFunc<T, T2>, bool>> join)
        {
            base._Join("left join", join);
            return this;
        }

        public IQueryable<T, T2> InnerJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public IQueryable<T, T2> InnerJoin(Expression<Func<MoonFunc<T, T2>, bool>> join)
        {
            base._Join("inner join", join);
            return this;
        }

        public new IQueryable<T, T2> Take(int limit)
        {
            _Take(limit);
            return this;
        }

        public new IQueryable<T, T2> Distinct()
        {
            _Distinct();
            return this;
        }

        public new IQueryable<T, T2> UseSqlServer(SqlServerOption option)
        {
            _UseSqlServer(option);
            return this;
        }
        public new IQueryable<T, T2> UseMySql(MySqlOption option) { return this; }
        public new IQueryable<T, T2> UseOracle(OracleOption option) { return this; }
    }

    public abstract class QueryableProvide<T, T2, T3> : QueryableProvide<T>, IQueryable<T, T2, T3>
    {
        public QueryableProvide(IRepository _Repository)
            : base(_Repository)
        {
            MapperTable classMapperT2 = ClassMapper.Mapping<T2>();
            MapperTable classMapperT3 = ClassMapper.Mapping<T3>();
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT2.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT3.TableName));

            JoinTables.Add(classMapperT2);
            JoinTables.Add(classMapperT3);
            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, JoinTables.ToArray());
        }

        public IQueryable<T, T2, T3> TableName(string tableName, string tableName2, string tableName3)
        {
            TableNames.Clear();
            MasterTableName = tableName;
            TableNames.Add(tableName2);
            TableNames.Add(tableName3);
            return this;
        }

        public new IQueryable<T, T2, T3> Select(string field)
        {
            base._Select(field);
            return this;
        }

        public IQueryable<T, T2, T3> Where(Expression<Func<MoonFunc<T, T2, T3>, bool>> where)
        {
            base._Where(where, true);
            return this;
        }

        public new IQueryable<T, T2, T3> Where(string where, object dynamic = null)
        {
            base._Where(where, dynamic);
            return this;
        }

        public int Count(Expression<Func<MoonFunc<T, T2, T3>, object>> field)
        {
            return base._Count(field, true);
        }

        public TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field)
        {
            return base._Sum<TResult>(field, true);
        }

        public TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field)
        {
            return base._Avg<TResult>(field, true);
        }

        public TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field)
        {
            return base._Max<TResult>(field, true);
        }

        public TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3>, TResult>> field)
        {
            return base._Min<TResult>(field, true);
        }

        public IQueryable<T, T2, T3> OrderBy(Expression<Func<MoonFunc<T, T2, T3>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            base._OrderBy(field, orderBy, true);
            return this;
        }

        public new IQueryable<T, T2, T3> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            __OrderBy = orderBy;
            OrderByField = field;
            return this;
        }

        public IQueryable<T, T2, T3> GroupBy(Expression<Func<MoonFunc<T, T2, T3>, object>> field)
        {
            _GroupBy(field, true);
            return this;
        }

        public IQueryable<T, T2, T3> Select(Expression<Func<MoonFunc<T, T2, T3>, object>> field)
        {
            base._Select(field, true);
            return this;
        }

        public new IQueryable<T, T2, T3> Take(int limit)
        {
            _Take(limit);
            return this;
        }

        public new IQueryable<T, T2, T3> Distinct()
        {
            _Distinct();
            return this;
        }

        public IQueryable<T, T2, T3> LeftJoin(Expression<Func<MoonFunc<T, T2, T3>, bool>> join)
        {
            base._Join("left join", join);
            return this;
        }

        public IQueryable<T, T2, T3> LeftJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public IQueryable<T, T2, T3> InnerJoin(Expression<Func<MoonFunc<T, T2, T3>, bool>> join)
        {
            base._Join("inner join", join);
            return this;
        }

        public IQueryable<T, T2, T3> InnerJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public new IQueryable<T, T2, T3> UseSqlServer(SqlServerOption option)
        {
            _UseSqlServer(option);
            return this;
        }
        public new IQueryable<T, T2, T3> UseMySql(MySqlOption option) { return this; }
        public new IQueryable<T, T2, T3> UseOracle(OracleOption option) { return this; }
    }

    public abstract class QueryableProvide<T, T2, T3, T4> : QueryableProvide<T>, IQueryable<T, T2, T3, T4>
    {
        public QueryableProvide(IRepository _Repository)
            : base(_Repository)
        {
            MapperTable classMapperT2 = ClassMapper.Mapping<T2>();
            MapperTable classMapperT3 = ClassMapper.Mapping<T3>();
            MapperTable classMapperT4 = ClassMapper.Mapping<T4>();
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT2.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT3.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT4.TableName));

            JoinTables.Add(classMapperT2);
            JoinTables.Add(classMapperT3);
            JoinTables.Add(classMapperT4);
            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, JoinTables.ToArray());
        }

        public IQueryable<T, T2, T3, T4> TableName(string tableName, string tableName2, string tableName3, string tableName4)
        {
            TableNames.Clear();
            MasterTableName = tableName;
            TableNames.Add(tableName2);
            TableNames.Add(tableName3);
            TableNames.Add(tableName4);
            return this;
        }

        public new IQueryable<T, T2, T3, T4> Select(string field)
        {
            base._Select(field);
            return this;
        }

        public IQueryable<T, T2, T3, T4> Where(Expression<Func<MoonFunc<T, T2, T3, T4>, bool>> where)
        {
            base._Where(where, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4> Where(string where, object dynamic = null)
        {
            base._Where(where, dynamic);
            return this;
        }

        public int Count(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field)
        {
            return base._Count(field, true);
        }

        public TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field)
        {
            return base._Sum<TResult>(field, true);
        }

        public TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field)
        {
            return base._Avg<TResult>(field, true);
        }

        public TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field)
        {
            return base._Max<TResult>(field, true);
        }

        public TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4>, TResult>> field)
        {
            return base._Min<TResult>(field, true);
        }

        public IQueryable<T, T2, T3, T4> OrderBy(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            base._OrderBy(field, orderBy, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            __OrderBy = orderBy;
            OrderByField = field;
            return this;
        }

        public IQueryable<T, T2, T3, T4> GroupBy(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field)
        {
            _GroupBy(field, true);
            return this;
        }

        public IQueryable<T, T2, T3, T4> Select(Expression<Func<MoonFunc<T, T2, T3, T4>, object>> field)
        {
            base._Select(field, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4> Take(int limit)
        {
            _Take(limit);
            return this;
        }

        public new IQueryable<T, T2, T3, T4> Distinct()
        {
            _Distinct();
            return this;
        }

        public IQueryable<T, T2, T3, T4> LeftJoin(Expression<Func<MoonFunc<T, T2, T3, T4>, bool>> join)
        {
            base._Join("left join", join);
            return this;
        }

        public IQueryable<T, T2, T3, T4> LeftJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public IQueryable<T, T2, T3, T4> InnerJoin(Expression<Func<MoonFunc<T, T2, T3, T4>, bool>> join)
        {
            base._Join("inner join", join);
            return this;
        }

        public IQueryable<T, T2, T3, T4> InnerJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public new IQueryable<T, T2, T3, T4> UseSqlServer(SqlServerOption option)
        {
            _UseSqlServer(option);
            return this;
        }
        public new IQueryable<T, T2, T3, T4> UseMySql(MySqlOption option) { return this; }
        public new IQueryable<T, T2, T3, T4> UseOracle(OracleOption option) { return this; }
    }

    public abstract class QueryableProvide<T, T2, T3, T4, T5> : QueryableProvide<T>, IQueryable<T, T2, T3, T4, T5>
    {
        public QueryableProvide(IRepository _Repository)
            : base(_Repository)
        {
            MapperTable classMapperT2 = ClassMapper.Mapping<T2>();
            MapperTable classMapperT3 = ClassMapper.Mapping<T3>();
            MapperTable classMapperT4 = ClassMapper.Mapping<T4>();
            MapperTable classMapperT5 = ClassMapper.Mapping<T5>();
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT2.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT3.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT4.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT5.TableName));

            JoinTables.Add(classMapperT2);
            JoinTables.Add(classMapperT3);
            JoinTables.Add(classMapperT4);
            JoinTables.Add(classMapperT5);
            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, JoinTables.ToArray());
        }

        public IQueryable<T, T2, T3, T4, T5> TableName(string tableName, string tableName2, string tableName3, string tableName4, string tableName5)
        {
            TableNames.Clear();
            MasterTableName = tableName;
            TableNames.Add(tableName2);
            TableNames.Add(tableName3);
            TableNames.Add(tableName4);
            TableNames.Add(tableName5);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5> Select(string field)
        {
            base._Select(field);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> Where(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, bool>> where)
        {
            base._Where(where, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5> Where(string where, object dynamic = null)
        {
            base._Where(where, dynamic);
            return this;
        }

        public int Count(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field)
        {
            return base._Count(field, true);
        }

        public TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field)
        {
            return base._Sum<TResult>(field, true);
        }

        public TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field)
        {
            return base._Avg<TResult>(field, true);
        }

        public TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field)
        {
            return base._Max<TResult>(field, true);
        }

        public TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, TResult>> field)
        {
            return base._Min<TResult>(field, true);
        }

        public IQueryable<T, T2, T3, T4, T5> OrderBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            base._OrderBy(field, orderBy, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            __OrderBy = orderBy;
            OrderByField = field;
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> GroupBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field)
        {
            _GroupBy(field, true);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> Select(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, object>> field)
        {
            base._Select(field, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5> Take(int limit)
        {
            _Take(limit);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5> Distinct()
        {
            _Distinct();
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> LeftJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, bool>> join)
        {
            base._Join("left join", join);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> LeftJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> InnerJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5>, bool>> join)
        {
            base._Join("inner join", join);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5> InnerJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5> UseSqlServer(SqlServerOption option)
        {
            _UseSqlServer(option);
            return this;
        }
        public new IQueryable<T, T2, T3, T4, T5> UseMySql(MySqlOption option) { return this; }
        public new IQueryable<T, T2, T3, T4, T5> UseOracle(OracleOption option) { return this; }
    }

    public abstract class QueryableProvide<T, T2, T3, T4, T5, T6> : QueryableProvide<T>, IQueryable<T, T2, T3, T4, T5, T6>
    {
        public QueryableProvide(IRepository _Repository)
            : base(_Repository)
        {
            MapperTable classMapperT2 = ClassMapper.Mapping<T2>();
            MapperTable classMapperT3 = ClassMapper.Mapping<T3>();
            MapperTable classMapperT4 = ClassMapper.Mapping<T4>();
            MapperTable classMapperT5 = ClassMapper.Mapping<T5>();
            MapperTable classMapperT6 = ClassMapper.Mapping<T6>();
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT2.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT3.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT4.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT5.TableName));
            TableNames.Add(Repository.SqlDialect.SetSqlName(classMapperT6.TableName));

            JoinTables.Add(classMapperT2);
            JoinTables.Add(classMapperT3);
            JoinTables.Add(classMapperT4);
            JoinTables.Add(classMapperT5);
            JoinTables.Add(classMapperT6);
            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, JoinTables.ToArray());
        }

        public IQueryable<T, T2, T3, T4, T5, T6> TableName(string tableName, string tableName2, string tableName3, string tableName4, string tableName5, string tableName6)
        {
            TableNames.Clear();
            MasterTableName = tableName;
            TableNames.Add(tableName2);
            TableNames.Add(tableName3);
            TableNames.Add(tableName4);
            TableNames.Add(tableName5);
            TableNames.Add(tableName6);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5, T6> Select(string field)
        {
            base._Select(field);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> Where(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, bool>> where)
        {
            base._Where(where, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5, T6> Where(string where, object dynamic = null)
        {
            base._Where(where, dynamic);
            return this;
        }

        public int Count(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field)
        {
            return base._Count(field, true);
        }

        public TResult Sum<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field)
        {
            return base._Sum<TResult>(field, true);
        }

        public TResult Avg<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field)
        {
            return base._Avg<TResult>(field, true);
        }

        public TResult Max<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field)
        {
            return base._Max<TResult>(field, true);
        }

        public TResult Min<TResult>(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, TResult>> field)
        {
            return base._Min<TResult>(field, true);
        }

        public IQueryable<T, T2, T3, T4, T5, T6> OrderBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            base._OrderBy(field, orderBy, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5, T6> OrderBy(string field, OrderBy orderBy = Dapper.Moon.OrderBy.Asc)
        {
            __OrderBy = orderBy;
            OrderByField = field;
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> GroupBy(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field)
        {
            _GroupBy(field, true);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> Select(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, object>> field)
        {
            base._Select(field, true);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5, T6> Take(int limit)
        {
            _Take(limit);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5, T6> Distinct()
        {
            _Distinct();
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> LeftJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, bool>> join)
        {
            base._Join("left join", join);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> LeftJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> InnerJoin(Expression<Func<MoonFunc<T, T2, T3, T4, T5, T6>, bool>> join)
        {
            base._Join("inner join", join);
            return this;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> InnerJoin(string join, object dynamic = null)
        {
            base._Join(join, dynamic);
            return this;
        }

        public new IQueryable<T, T2, T3, T4, T5, T6> UseSqlServer(SqlServerOption option)
        {
            _UseSqlServer(option);
            return this;
        }
        public new IQueryable<T, T2, T3, T4, T5, T6> UseMySql(MySqlOption option) { return this; }
        public new IQueryable<T, T2, T3, T4, T5, T6> UseOracle(OracleOption option) { return this; }
    }
    #endregion T2-T6
}
