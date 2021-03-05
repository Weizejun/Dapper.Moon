using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Dapper.Moon
{
    public abstract class UpdateableProvide<T> : IUpdateable<T>
    {
        protected IRepository Repository { get; set; }
        protected ExpressionProvide ExpressionProvideObj { get; set; }
        protected SqlBuilderResult _Where { get; set; }
        protected List<SqlBuilderResult> Columns { get; set; } = new List<SqlBuilderResult>();
        protected bool? IsSingleColumn { get; set; }
        protected string _IgnoreColumns { get; set; }
        protected T SaveObject { get; set; }
        protected MapperTable MasterTable { get; set; }
        protected string MasterTableName { get; set; }
        protected string ColumnSql { get; set; }
        protected object ColumnSqlParam { get; set; }

        public UpdateableProvide(IRepository _Repository)
        {
            Repository = _Repository;

            MasterTable = ClassMapper.Mapping<T>();
            MasterTableName = MasterTable.TableName;
            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, MasterTable);
        }

        public IUpdateable<T> Updateable(T t)
        {
            SaveObject = t;
            return this;
        }

        public IUpdateable<T> Updateable()
        {
            return this;
        }

        public IUpdateable<T> Where(Expression<Func<T, bool>> where)
        {
            _Where = ExpressionProvideObj.ExpressionRouter(where);
            return this;
        }

        public IUpdateable<T> SetColumns(Expression<Func<T, object>> columns)
        {
            if (columns != null)
            {
                switch (columns.Body.NodeType)
                {
                    case ExpressionType.New:
                        if (IsSingleColumn != null && !Convert.ToBoolean(IsSingleColumn))
                        {
                            throw new Exception("specified column error");
                        }
                        IsSingleColumn = true;
                        Columns.Add(ExpressionProvideObj.ExpressionRouter(columns.Body, isQuote: false));
                        break;
                    case ExpressionType.Convert:
                        if (IsSingleColumn != null && Convert.ToBoolean(IsSingleColumn))
                        {
                            throw new Exception("specified column error");
                        }
                        IsSingleColumn = false;
                        Columns.Add(ExpressionProvideObj.ExpressionFieldValue(columns.Body));
                        break;
                    default:
                        throw new Exception("unsupported expression");
                }
            }
            return this;
        }

        public IUpdateable<T> SetColumns(string sql, object param)
        {
            ColumnSql = sql;
            ColumnSqlParam = param;
            return this;
        }

        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            IsSingleColumn = true;
            _IgnoreColumns = ExpressionProvideObj.ExpressionRouter(columns).Sql;
            return this;
        }

        public IUpdateable<T> TableName(string tableName)
        {
            if (tableName == null || tableName == "") throw new Exception("wrong table name");
            MasterTableName = tableName;
            return this;
        }

        public int Execute()
        {
            if (_Where == null)
            {
                throw new Exception("condition error");
            }
            if (Columns.Count == 0
                && string.IsNullOrWhiteSpace(this.ColumnSql)
                && string.IsNullOrWhiteSpace(this._IgnoreColumns))
            {
                throw new Exception("there are no updated columns");
            }
            if (Convert.ToBoolean(IsSingleColumn) && SaveObject == null)
            {
                throw new Exception("save object is empty");
            }
            SqlBuilderResult result = ToSql();
            return Repository.Execute(result.Sql, result.DynamicParameters);
        }

        public virtual SqlBuilderResult ToSql()
        {
            DynamicParameters dynamicParameters = new DynamicParameters();
            StringBuilder builder = new StringBuilder();
            if (Columns != null && Columns.Count > 0)
            {
                foreach (var item in Columns)
                {
                    if (!Convert.ToBoolean(IsSingleColumn))
                    {
                        builder.Append(item.Sql).Append(",");
                        dynamicParameters.AddDynamicParams(item.DynamicParameters);
                    }
                    else
                    {
                        foreach (var itemB in item.Sql.Split(','))
                        {
                            builder.Append(Repository.SqlDialect.SetSqlName(itemB)).Append("=")
                                .Append(Repository.SqlDialect.ParameterPrefix)
                                .Append(itemB).Append(",");
                            if (!MasterTable.PropertiesDict.TryGetValue(itemB, out PropertyMap property))
                            {
                                throw new Exception("property not found");
                            }
                            object value = property.PropertyInfo.GetValue(SaveObject);
                            dynamicParameters.Add(itemB, value);
                        }
                    }
                }
                builder.Remove(builder.Length - 1, 1);
                builder.Replace("(", "").Replace(")", "");
            }
            else if (!string.IsNullOrWhiteSpace(ColumnSql))
            {
                builder.Append(ColumnSql);
                if (ColumnSqlParam != null)
                {
                    var dp = CommonUtils.GetDynamicParameters(ColumnSqlParam);
                    if (dp != null)
                    {
                        dynamicParameters.AddDynamicParams(dp);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(_IgnoreColumns))
            {
                var columns = MasterTable.Properties.Where(i => !i.IsIdentity && !i.Ignored && !i.IsPrimaryKey);
                if (!columns.Any())
                {
                    throw new ArgumentException("no column");
                }
                var ignoreColumns = _IgnoreColumns.Split(',').Distinct().ToDictionary(a => a);
                foreach (var item in columns)
                {
                    if (!ignoreColumns.ContainsKey(Repository.SqlDialect.SetSqlName(item.Name)))
                    {
                        builder.Append(Repository.SqlDialect.SetSqlName(item.ColumnName))
                          .Append("=").Append(Repository.SqlDialect.ParameterPrefix)
                          .Append(item.ColumnName).Append(",");

                        object value = item.PropertyInfo.GetValue(SaveObject);
                        dynamicParameters.Add(item.ColumnName, value);
                    }
                }
                builder.Remove(builder.Length - 1, 1);
            }

            dynamicParameters.AddDynamicParams(_Where.DynamicParameters);

            return new SqlBuilderResult()
            {
                Sql = string.Format("update {0} set {1} where {2}", Repository.SqlDialect.SetSqlName(MasterTableName), builder.ToString(), _Where.Sql),
                DynamicParameters = dynamicParameters
            };
        }
    }
}
