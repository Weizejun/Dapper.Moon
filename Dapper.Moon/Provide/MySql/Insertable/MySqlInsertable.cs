using System;
using System.Data;

namespace Dapper.Moon
{
    public class MySqlInsertable<T> : InsertableProvide<T>
    {
        public MySqlInsertable(IRepository _Repository)
         : base(_Repository) { }

        public override long ExecuteIdentity()
        {
            SqlBuilderResult sqlBuilderResult = ToSql();
            sqlBuilderResult.Sql = string.Concat(sqlBuilderResult.Sql, ";select last_insert_id()");
            return Repository.ExecuteScalar<long>(sqlBuilderResult.Sql, sqlBuilderResult.DynamicParameters);
        }

        public override DataTable ToDataTable()
        {
            if (SaveList == null || SaveList.Count == 0)
            {
                throw new Exception("object is empty");
            }
            DataTable table = new DataTable();
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
                    var dynamicMethod = CommonUtils.EmitGetProperty(property, true);
                    //mysql批量添加是通过文件导入的方式 不支持表中类型是数字 值是null
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
            table.TableName = MasterTable.TableName;
            return table;
        }
    }
}
