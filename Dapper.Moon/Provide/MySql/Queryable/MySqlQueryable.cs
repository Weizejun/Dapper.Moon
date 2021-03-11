using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public class MySqlQueryable<T> : QueryableProvide<T>
    {
        public MySqlQueryable(IRepository _Repository)
            : base(_Repository) { }

        internal static SqlBuilderResult ToSqlStatic(QueryableSqlBuilder query)
        {
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
            if (!string.IsNullOrWhiteSpace(query.OrderBy))
            {
                orderBySql = $" order by {query.OrderBy}";
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
                //select {0} from {1} {2} {3} limit {4},{5}
                sb.Append("select ").Append(selectColumns).Append(" ").Append(" from ").Append(fromSql)
                .Append(whereSql).Append(groupBy).Append(orderBySql).Append(" limit ").Append(query.Offset).Append(",").Append(query.Limit);

                result.Sql = sb.ToString();
            }
            else
            {
                if (query.Limit.HasValue)
                {
                    //select {0} from {1} {2} {3} limit {4}
                    query.Offset = 0;
                    sb.Append("select ").Append(selectColumns).Append(" ").Append(" from ").Append(fromSql)
                        .Append(whereSql).Append(groupBy).Append(orderBySql).Append(" limit ").Append(query.Limit);
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
        public override SqlBuilderResult ToSql() => ToSqlStatic(new QueryableSqlBuilder
        {
            SqlDialect = Repository.SqlDialect,
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
            SelectColumns = SelectColumns,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
        #endregion ToSql
    }

    public class MySqlQueryable<T, T2> : QueryableProvide<T, T2>
    {
        public MySqlQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => MySqlQueryable<T>.ToSqlStatic(new QueryableSqlBuilder
        {
            SqlDialect = Repository.SqlDialect,
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
            SelectColumns = SelectColumns,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class MySqlQueryable<T, T2, T3> : QueryableProvide<T, T2, T3>
    {
        public MySqlQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => MySqlQueryable<T>.ToSqlStatic(new QueryableSqlBuilder
        {
            SqlDialect = Repository.SqlDialect,
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
            SelectColumns = SelectColumns,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class MySqlQueryable<T, T2, T3, T4> : QueryableProvide<T, T2, T3, T4>
    {
        public MySqlQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => MySqlQueryable<T>.ToSqlStatic(new QueryableSqlBuilder
        {
            SqlDialect = Repository.SqlDialect,
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
            SelectColumns = SelectColumns,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class MySqlQueryable<T, T2, T3, T4, T5> : QueryableProvide<T, T2, T3, T4, T5>
    {
        public MySqlQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => MySqlQueryable<T>.ToSqlStatic(new QueryableSqlBuilder
        {
            SqlDialect = Repository.SqlDialect,
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
            SelectColumns = SelectColumns,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }

    public class MySqlQueryable<T, T2, T3, T4, T5, T6> : QueryableProvide<T, T2, T3, T4, T5, T6>
    {
        public MySqlQueryable(IRepository _Repository)
            : base(_Repository) { }

        public override SqlBuilderResult ToSql() => MySqlQueryable<T>.ToSqlStatic(new QueryableSqlBuilder
        {
            SqlDialect = Repository.SqlDialect,
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
            SelectColumns = SelectColumns,
            TableNames = TableNames,
            IsDistinct = IsDistinct
        });
    }
}
