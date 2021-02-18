using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;

namespace Dapper.Moon
{
    public class OracleDbFirst : IDbFirst
    {
        private IRepository Repository { get; set; }

        public OracleDbFirst(IRepository _Repository)
        {
            Repository = _Repository;
        }

        public void Builder(DbFirstOption option)
        {
            if (string.IsNullOrWhiteSpace(option.SaveFolder) || string.IsNullOrWhiteSpace(option.Namespace))
            {
                throw new Exception("the parameter is empty");
            }
            if (!Directory.Exists(option.SaveFolder))
            {
                Directory.CreateDirectory(option.SaveFolder);
            }
            string where = "1=1";
            if (option.Tables?.Length > 0)
            {
                where = "table_name in(" + string.Join(",", option.Tables.Select(i => $"'{i.ToUpper()}'")) + ")";
            }

            string username = Repository.ExecuteScalar<string>("select username from user_users");

            string sql = $"select table_name from user_tables where TABLESPACE_NAME is not null and user='{username}' and " + where;

            string tableInfoSql = $@"SELECT column_name ColumnName,data_type ColumnType,nullable IsNull,
            (select position from user_cons_columns where table_name=cc.table_name and column_name=cc.column_name and position='1') IsPrimaryKey
            FROM all_tab_cols cc WHERE table_name = :tableName";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine("using System;");
            builder.AppendLine("using System.ComponentModel.DataAnnotations;");
            builder.AppendLine("using System.ComponentModel.DataAnnotations.Schema;");
            builder.AppendLine();
            builder.AppendLine("namespace " + option.Namespace);

            var list = Repository.Query<string>(sql);
            foreach (var item in list)
            {
                var cols = Repository.Query<DbTableColumnInfo>(tableInfoSql, new { tableName = item });
                StringBuilder tempBuilder = new StringBuilder();
                tempBuilder.AppendLine("{");
                tempBuilder.AppendLine($"    public class {item}");
                tempBuilder.AppendLine("    {");
                foreach (var itemB in cols)
                {
                    if (itemB.IsPrimaryKey == "1")
                    {
                        tempBuilder.AppendLine("        [Key]");
                    }
                    tempBuilder.AppendLine("        public " + GetSqlDbType(itemB.ColumnType) + " " + itemB.ColumnName + " { get; set; }");
                }
                tempBuilder.AppendLine("    }");
                tempBuilder.AppendLine("}");
                File.WriteAllText(option.SaveFolder + $"\\{item}.cs", builder.ToString() + tempBuilder.ToString());
            }
        }

        private string GetSqlDbType(string columnType)
        {
            switch (columnType.ToLower())
            {
                case "number":
                    return "decimal";
                case "float":
                    return "double";
                case "date":
                case "timestamp":
                    return "DateTime";
                case "nvarchar2":
                case "varchar2":
                case "char":
                case "nchar":
                    return "string";
                default: return "string";
            }
        }
    }
}