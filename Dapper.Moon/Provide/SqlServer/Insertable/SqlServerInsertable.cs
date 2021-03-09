using System;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public class SqlServerInsertable<T> : InsertableProvide<T>
    {
        public SqlServerInsertable(IRepository _Repository)
            : base(_Repository) { }

        public override long ExecuteIdentity()
        {
            SqlBuilderResult sqlBuilderResult = ToSql();
            sqlBuilderResult.Sql = string.Concat(sqlBuilderResult.Sql, ";select scope_identity()");
            return Repository.ExecuteScalar<long>(sqlBuilderResult.Sql, sqlBuilderResult.DynamicParameters);
        }

        public override async Task<long> ExecuteIdentityAsync()
        {
            SqlBuilderResult sqlBuilderResult = ToSql();
            sqlBuilderResult.Sql = string.Concat(sqlBuilderResult.Sql, ";select scope_identity()");
            return await Repository.ExecuteScalarAsync<long>(sqlBuilderResult.Sql, sqlBuilderResult.DynamicParameters);
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
                if (item.IsIdentity || item.Ignored)
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
                    var dynamicMethod = CommonUtils.EmitGetProperty(property, false);
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
