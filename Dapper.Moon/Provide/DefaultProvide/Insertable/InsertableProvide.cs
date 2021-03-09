using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public abstract class InsertableProvide<T> : IInsertable<T>
    {
        protected IRepository Repository { get; set; }
        protected T SaveObject { get; set; }
        protected List<T> SaveList { get; set; }
        protected MapperTable MasterTable { get; set; }
        protected string MasterTableName { get; set; }

        public InsertableProvide(IRepository _Repository)
        {
            Repository = _Repository;

            MasterTable = ClassMapper.Mapping<T>();
            MasterTableName = MasterTable.TableName;
        }

        public IInsertable<T> Insertable(T t)
        {
            SaveObject = t;
            return this;
        }

        public IInsertable<T> Insertable(List<T> list)
        {
            SaveList = list;
            return this;
        }

        public IInsertable<T> TableName(string tableName)
        {
            if (tableName == null || tableName == "") throw new Exception("wrong table name");
            MasterTableName = tableName;
            return this;
        }

        public int BulkInsert()
        {
            if (SaveList == null)
            {
                throw new Exception("save object is empty");
            }
            return Repository.BulkInsert(ToDataTable());
        }

        public async Task<int> BulkInsertAsync()
        {
            if (SaveList == null)
            {
                throw new Exception("save object is empty");
            }
            return await Repository.BulkInsertAsync(ToDataTable());
        }

        public virtual int Execute()
        {
            SqlBuilderResult result = ToSql();
            return Repository.Execute(result.Sql, result.DynamicParameters);
        }

        public virtual async Task<int> ExecuteAsync()
        {
            SqlBuilderResult result = ToSql();
            return await Repository.ExecuteAsync(result.Sql, result.DynamicParameters);
        }

        protected virtual SqlBuilderResult ToSqlBatch()
        {
            var columns = MasterTable.Properties.Where(i => !i.IsIdentity && !i.Ignored);
            if (!columns.Any())
            {
                throw new ArgumentException("no column");
            }
            var columnNames = columns.Select(i => Repository.SqlDialect.SetSqlName(i.ColumnName));
            string columnSql = string.Join(",", columnNames);
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("insert into ").Append(Repository.SqlDialect.SetSqlName(MasterTableName)).Append(" (");
            sbSql.Append(columnSql).Append(")");
            sbSql.AppendLine("values");

            DynamicParameters parameters = new DynamicParameters();
            int colIndex = 0;
            foreach (var item in SaveList)
            {
                sbSql.AppendLine("(");
                foreach (var itemCol in columns.ToList())
                {
                    var property = itemCol.PropertyInfo;
                    object value = property.GetValue(item);
                    string paramName = Repository.SqlDialect.ParameterPrefix + property.Name + colIndex;
                    sbSql.Append($"{paramName},");
                    parameters.Add(paramName, value);
                    ++colIndex;
                }
                sbSql.Remove(sbSql.Length - 1, 1);
                sbSql.AppendLine("),");
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            return new SqlBuilderResult()
            {
                Sql = sbSql.ToString(),
                DynamicParameters = parameters
            };
        }

        public abstract long ExecuteIdentity();
        public abstract Task<long> ExecuteIdentityAsync();
        public abstract DataTable ToDataTable();

        public virtual SqlBuilderResult ToSql()
        {
            if (SaveObject == null && SaveList == null)
            {
                throw new Exception("save object is empty");
            }
            if (SaveList == null) SaveList = new List<T>();
            if (SaveObject != null) SaveList.Add(SaveObject);
            return ToSqlBatch();
        }
    }
}
