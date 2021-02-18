using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public class SqlServerQueryable<T> : QueryableProvide<T>
    {
        public SqlServerQueryable(IRepository _Repository)
            : base(_Repository) { }

        internal static SqlBuilderResult ToSqlStatic(SqlServerOption option, QueryableSqlBuilder query)
        {
            //SELECT SERVERPROPERTY('ProductVersion')
            SqlBuilderResult result = new SqlBuilderResult();
            result.DynamicParameters = new DynamicParameters();

            string whereSql = "";
            if (query.Where != null)
            {
                whereSql = query.Where.Sql;
                if (query.Where.DynamicParameters != null)
                    result.DynamicParameters.AddDynamicParams(query.Where.DynamicParameters);
            }
            StringBuilder fromSql = new StringBuilder(query.MasterTableName);
            if (query.Joins != null)
            {
                fromSql.Append(" t1");
                foreach (var item in query.Joins)
                {
                    fromSql.AppendLine(" ").Append(item.Sql);
                    if (item.DynamicParameters?.ParameterNames.Count() > 0)
                        result.DynamicParameters.AddDynamicParams(item.DynamicParameters);
                }
            }
            fromSql.Append(" ");
            if (!string.IsNullOrWhiteSpace(query.FunctionSql))
            {
                if (query.Function?.DynamicParameters != null)
                {
                    result.DynamicParameters.AddDynamicParams(query.Function.DynamicParameters);
                }
                result.Sql = $"select {query.FunctionSql} from {fromSql.ToString()} {whereSql}";
                return result;
            }
            string orderBySql = "";
            if (!string.IsNullOrWhiteSpace(query.OrderByField))
            {
                orderBySql = $" order by {query.OrderByField} {query.OrderBy.ToString()}";
            }

            string groupBy = "";
            if (!string.IsNullOrWhiteSpace(query.GroupByField))
            {
                groupBy = $" group by {query.GroupByField}";
            }

            string selectColumns = query.SelectColumns;
            if (string.IsNullOrWhiteSpace(selectColumns))
            {
                selectColumns = "*";
            }
            if (query.IsDistinct)
            {
                selectColumns = selectColumns.Insert(0, "distinct ");
            }
            StringBuilder sb = new StringBuilder();
            if (query.Offset.HasValue && query.Limit.HasValue)
            {
                //分页
                sb.Append("select count(1) from ").Append(fromSql).Append(whereSql).Append(groupBy);
                sb.AppendLine(";");
                if (option != null && option.UseRowNumberForPaging)
                {
                    //select * from (select {0},ROW_NUMBER() OVER({5}) _RowId from {1} {2}) a where _RowId BETWEEN {3} and {4}
                    sb.Append("select * from (").Append("select ").Append(selectColumns)
                        .Append(",ROW_NUMBER() OVER(").Append(orderBySql)
                        .Append(") _RowId from ").Append(fromSql).Append(whereSql).Append(groupBy)
                        .Append(") _T_ where _RowId BETWEEN ").Append(query.Offset).Append(" and ").Append(query.Limit);
                }
                else
                {
                    //select {0} from {1} {2} offset {3} rows fetch next {4} rows only
                    sb.Append("select ").Append(selectColumns).Append(" from ").Append(fromSql)
                        .Append(whereSql).Append(groupBy).Append(orderBySql).Append(" offset ").Append(query.Offset)
                        .Append(" rows fetch next ").Append(query.Limit).Append(" rows only");
                }
                result.Sql = sb.ToString();
            }
            else
            {
                if (query.Limit.HasValue)
                {
                    //select top {0} {1} from {2} {3} {4}
                    query.Offset = 0;
                    sb.Append("select top ").Append(query.Limit).Append(" ").Append(selectColumns).Append(" from ").Append(fromSql)
                        .Append(whereSql).Append(groupBy).Append(orderBySql);
                    result.Sql = sb.ToString();
                }
                else
                {
                    //没有分页
                    sb.Append("select ").Append(selectColumns).Append(" from ").Append(fromSql)
                        .Append(whereSql).Append(groupBy).Append(orderBySql);
                    result.Sql = sb.ToString();
                }
            }
            //select into
            if (query.SelectIntoTable != null)
            {
                MapperTable classMapper = query.SelectIntoTable;
                StringBuilder builder = new StringBuilder();
                builder.Append("insert into ").Append(query.SqlDialect.SetSqlName(classMapper.TableName));
                if (!string.IsNullOrWhiteSpace(query.IntoTableColumn))
                {
                    builder.Append("(").Append(query.IntoTableColumn).AppendLine(")");
                }
                else
                {
                    var columns = classMapper.Properties.Where(i => !i.IsIdentity && !i.Ignored);
                    if (!columns.Any())
                    {
                        throw new ArgumentException("no column");
                    }
                    var columnNames = columns.Select(i => query.SqlDialect.SetSqlName(i.ColumnName));
                    string columnSql = string.Join(",", columnNames);
                    builder.Append("(").Append(columnSql).AppendLine(")");
                }
                builder.AppendLine(result.Sql);
                result.Sql = builder.ToString();
            }
            return result;
        }

        #region ToSql
        public override SqlBuilderResult ToSql() => ToSqlStatic(SqlServerOption, new QueryableSqlBuilder
        {
            Function = Function,
            Where = __Where,
            FunctionSql = FunctionSql,
            GroupByField = GroupByField,
            IntoTableColumn = IntoTableColumn,
            SelectIntoTable = SelectIntoTable,
            Joins = Joins,
            Limit = Limit,
            MasterTableName = MasterTableName,
            Offset = Offset,
            OrderBy = __OrderBy,
            OrderByField = OrderByField,
            SelectColumns = SelectColumns,
            SqlDialect = Repository.SqlDialect,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
        #endregion ToSql
    }

    public class SqlServerQueryable<T, T2> : QueryableProvide<T, T2>
    {
        public SqlServerQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => SqlServerQueryable<T>.ToSqlStatic(SqlServerOption, new QueryableSqlBuilder
        {
            Function = Function,
            Where = __Where,
            FunctionSql = FunctionSql,
            GroupByField = GroupByField,
            IntoTableColumn = IntoTableColumn,
            SelectIntoTable = SelectIntoTable,
            Joins = Joins,
            Limit = Limit,
            MasterTableName = MasterTableName,
            Offset = Offset,
            OrderBy = __OrderBy,
            OrderByField = OrderByField,
            SelectColumns = SelectColumns,
            SqlDialect = Repository.SqlDialect,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class SqlServerQueryable<T, T2, T3> : QueryableProvide<T, T2, T3>
    {
        public SqlServerQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => SqlServerQueryable<T>.ToSqlStatic(SqlServerOption, new QueryableSqlBuilder
        {
            Function = Function,
            Where = __Where,
            FunctionSql = FunctionSql,
            GroupByField = GroupByField,
            IntoTableColumn = IntoTableColumn,
            SelectIntoTable = SelectIntoTable,
            Joins = Joins,
            Limit = Limit,
            MasterTableName = MasterTableName,
            Offset = Offset,
            OrderBy = __OrderBy,
            OrderByField = OrderByField,
            SelectColumns = SelectColumns,
            SqlDialect = Repository.SqlDialect,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class SqlServerQueryable<T, T2, T3, T4> : QueryableProvide<T, T2, T3, T4>
    {
        public SqlServerQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => SqlServerQueryable<T>.ToSqlStatic(SqlServerOption, new QueryableSqlBuilder
        {
            Function = Function,
            Where = __Where,
            FunctionSql = FunctionSql,
            GroupByField = GroupByField,
            IntoTableColumn = IntoTableColumn,
            SelectIntoTable = SelectIntoTable,
            Joins = Joins,
            Limit = Limit,
            MasterTableName = MasterTableName,
            Offset = Offset,
            OrderBy = __OrderBy,
            OrderByField = OrderByField,
            SelectColumns = SelectColumns,
            SqlDialect = Repository.SqlDialect,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class SqlServerQueryable<T, T2, T3, T4, T5> : QueryableProvide<T, T2, T3, T4, T5>
    {
        public SqlServerQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => SqlServerQueryable<T>.ToSqlStatic(SqlServerOption, new QueryableSqlBuilder
        {
            Function = Function,
            Where = __Where,
            FunctionSql = FunctionSql,
            GroupByField = GroupByField,
            IntoTableColumn = IntoTableColumn,
            SelectIntoTable = SelectIntoTable,
            Joins = Joins,
            Limit = Limit,
            MasterTableName = MasterTableName,
            Offset = Offset,
            OrderBy = __OrderBy,
            OrderByField = OrderByField,
            SelectColumns = SelectColumns,
            SqlDialect = Repository.SqlDialect,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class SqlServerQueryable<T, T2, T3, T4, T5, T6> : QueryableProvide<T, T2, T3, T4, T5, T6>
    {
        public SqlServerQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => SqlServerQueryable<T>.ToSqlStatic(SqlServerOption, new QueryableSqlBuilder
        {
            Function = Function,
            Where = __Where,
            FunctionSql = FunctionSql,
            GroupByField = GroupByField,
            IntoTableColumn = IntoTableColumn,
            SelectIntoTable = SelectIntoTable,
            Joins = Joins,
            Limit = Limit,
            MasterTableName = MasterTableName,
            Offset = Offset,
            OrderBy = __OrderBy,
            OrderByField = OrderByField,
            SelectColumns = SelectColumns,
            SqlDialect = Repository.SqlDialect,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }
}
