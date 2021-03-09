using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public class OracleInsertable<T> : InsertableProvide<T>
    {
        public OracleInsertable(IRepository _Repository)
            : base(_Repository) { }

        public override long ExecuteIdentity()
        {
            var propertyMap = MasterTable.Properties.Where(i => i.IsIdentity && !string.IsNullOrWhiteSpace(i.SequenceName)).FirstOrDefault();
            if (propertyMap == null)
            {
                throw new ArgumentException("no column");
            }
            //可采用 returning id into :id 语法查询当前的序列 待完善
            SqlBuilderResult sqlBuilderResult = ToSql();
            int rowCount = Repository.Execute(sqlBuilderResult.Sql, sqlBuilderResult.DynamicParameters);
            return Repository.ExecuteScalar<long>($"select {propertyMap.SequenceName}.currval from dual");
        }

        public override async Task<long> ExecuteIdentityAsync()
        {
            var propertyMap = MasterTable.Properties.Where(i => i.IsIdentity && !string.IsNullOrWhiteSpace(i.SequenceName)).FirstOrDefault();
            if (propertyMap == null)
            {
                throw new ArgumentException("no column");
            }
            //可采用 returning id into :id 语法查询当前的序列 待完善
            SqlBuilderResult sqlBuilderResult = ToSql();
            int rowCount = await Repository.ExecuteAsync(sqlBuilderResult.Sql, sqlBuilderResult.DynamicParameters);
            return await Repository.ExecuteScalarAsync<long>($"select {propertyMap.SequenceName}.currval from dual");
        }

        protected override SqlBuilderResult ToSqlBatch()
        {
            if (MasterTable.Properties.Where(i => i.IsIdentity).Any())
            {
                throw new ArgumentException("sequence not supported");
            }
            var columns = MasterTable.Properties.Where(i => !i.Ignored);
            if (!columns.Any())
            {
                throw new ArgumentException("no column");
            }
            if ((columns.Count() * SaveList.Count) > 1000)
            {
                throw new Exception("ORA-24335: cannot support more than 1000 columns");
            }
            var columnNames = columns.Select(i => Repository.SqlDialect.SetSqlName(i.ColumnName));
            string columnSql = string.Join(",", columnNames);
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("insert all ");
            DynamicParameters parameters = new DynamicParameters();
            int colIndex = 0;
            foreach (var item in SaveList)
            {
                sbSql.AppendLine("into ").Append(Repository.SqlDialect.SetSqlName(MasterTable.TableName)).Append(" (");
                sbSql.Append(columnSql).Append(")");
                sbSql.AppendLine("values(");
                foreach (var itemCol in columns.ToList())
                {
                    var property = itemCol.PropertyInfo;
                    if (itemCol.IsIdentity && !string.IsNullOrWhiteSpace(itemCol.SequenceName))
                    {
                        sbSql.Append(itemCol.SequenceName).Append(".nextval").Append(",");
                    }
                    else
                    {
                        object value = property.GetValue(item);
                        string paramName = Repository.SqlDialect.ParameterPrefix + property.Name + colIndex;
                        sbSql.Append($"{paramName},");
                        parameters.Add(paramName, value);
                        ++colIndex;
                    }
                }
                sbSql.Remove(sbSql.Length - 1, 1);
                sbSql.Append(")");
            }
            sbSql.AppendLine("select 1 from dual");
            return new SqlBuilderResult()
            {
                Sql = sbSql.ToString(),
                DynamicParameters = parameters
            };
        }

        public override SqlBuilderResult ToSql()
        {
            if (SaveObject == null) return ToSqlBatch();
            var columns = MasterTable.Properties.Where(i => !i.Ignored);
            if (!columns.Any())
            {
                throw new ArgumentException("no column");
            }
            DynamicParameters parameters = new DynamicParameters();
            StringBuilder columnSql = new StringBuilder();
            StringBuilder parameterSql = new StringBuilder();
            foreach (var item in columns)
            {
                columnSql.Append(Repository.SqlDialect.SetSqlName(item.ColumnName)).Append(",");
                if (item.IsIdentity && !string.IsNullOrWhiteSpace(item.SequenceName))
                {
                    parameterSql.Append(item.SequenceName).Append(".nextval").Append(",");
                }
                else
                {
                    parameterSql.Append(Repository.SqlDialect.ParameterPrefix).Append(item.ColumnName).Append(",");
                    object value = item.PropertyInfo.GetValue(SaveObject);
                    parameters.Add(item.ColumnName, value);
                }
            }
            columnSql.Remove(columnSql.Length - 1, 1);
            parameterSql.Remove(parameterSql.Length - 1, 1);
            return new SqlBuilderResult()
            {
                Sql = string.Format("insert into {0}({1}) values({2})", Repository.SqlDialect.SetSqlName(MasterTable.TableName), columnSql, parameterSql),
                DynamicParameters = parameters
            };
        }

        public override DataTable ToDataTable()
        {
            if (SaveList == null || SaveList.Count == 0)
            {
                throw new Exception("object is empty");
            }
            DataTable table = new DataTable(MasterTable.TableName);
            foreach (var item in MasterTable.Properties)
            {
                if (item.Ignored)
                {
                    continue;
                }
                var nullType = Nullable.GetUnderlyingType(item.PropertyInfo.PropertyType);
                table.Columns.Add(item.ColumnName, nullType == null ? item.PropertyInfo.PropertyType : nullType);
            }
            int colCount = table.Columns.Count;
            foreach (var item in SaveList)
            {
                Type type = item.GetType();
                var entityValues = new object[colCount];
                for (int i = 0; i < colCount; i++)
                {
                    var property = type.GetProperty(table.Columns[i].ColumnName);
                    var dynamicMethod = CommonUtils.EmitGetProperty(property, true);
                    object val = DBNull.Value;
                    if (dynamicMethod != null)
                    {
                        var getterMethod = dynamicMethod.CreateDelegate(typeof(Func<T, object>)) as Func<T, object>;
                        val = getterMethod(item);
                        if (val == null)
                        {
                            val = DBNull.Value;
                        }
                    }
                    entityValues[i] = val;
                }
                table.Rows.Add(entityValues);
            }
            return table;
        }
    }
}
