using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;

namespace Dapper.Moon
{
    public class MySqlDbFirst : IDbFirst
    {
        private IRepository Repository { get; set; }

        public MySqlDbFirst(IRepository _Repository)
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
                where = "table_name in(" + string.Join(",", option.Tables.Select(i => $"'{i}'")) + ")";
            }
            string sql = $"select table_name from information_schema.TABLES where table_schema='{Repository.Connection.Database}' and " + where;
            string tableInfoSql = $@"select column_name ColumnName,data_type ColumnType,is_nullable IsNull,
            column_key IsPrimaryKey,extra IsIdentity
            from information_schema.columns where table_name=@tableName and table_schema='{Repository.Connection.Database}'";

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
                    if (itemB.IsIdentity == "auto_increment")
                    {
                        tempBuilder.AppendLine("        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                    }
                    if (itemB.IsPrimaryKey == "PRI")
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
                case "bit": return "bool";
                case "tinyint": return "byte";
                case "smallint": 
                case "mediumint":
                case "int": return "int";
                case "bigint": return "long";
                case "real":
                case "double": 
                case "float": return "double";
                case "numeric":
                case "decimal": return "decimal";
                case "year": 
                case "time": 
                case "date": 
                case "timestamp": 
                case "datetime": return "DateTime";
                //case "tinyblob": return MySqlDbType.TinyBlob;
                //case "blob": return MySqlDbType.Blob;
                //case "mediumblob": return MySqlDbType.MediumBlob;
                //case "longblob": return MySqlDbType.LongBlob;
                //case "binary": return MySqlDbType.Binary;
                //case "varbinary": return MySqlDbType.VarBinary;
                case "tinytext":
                case "text":
                case "mediumtext":
                case "char":
                case "varchar":
                case "longtext": return "string";
                //case "set": return MySqlDbType.Set;
                //case "enum": return MySqlDbType.Enum;
                //case "point": return MySqlDbType.Geometry;
                //case "linestring": return MySqlDbType.Geometry;
                //case "polygon": return MySqlDbType.Geometry;
                //case "geometry": return MySqlDbType.Geometry;
                //case "multipoint": return MySqlDbType.Geometry;
                //case "multilinestring": return MySqlDbType.Geometry;
                //case "multipolygon": return MySqlDbType.Geometry;
                //case "geometrycollection": return MySqlDbType.Geometry;
                default: return "string";
            }
        }
    }
}