using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.IO;

namespace Dapper.Moon
{
    public class SqlServerDbFirst : IDbFirst
    {
        private IRepository Repository { get; set; }

        public SqlServerDbFirst(IRepository _Repository)
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
                where = "name in(" + string.Join(",", option.Tables.Select(i => $"'{i}'")) + ")";
            }
            string sql = "select name from sys.objects where type='U' and " + where;
            string tableInfoSql = @"SELECT a.name ColumnName,
                (case when COLUMNPROPERTY( a.id,a.name,'IsIdentity')=1 then '1' else '0' end) IsIdentity, 
                (case when (SELECT count(*) FROM sysobjects  
                WHERE (name in (SELECT name FROM sysindexes  
                WHERE (id = a.id) AND (indid in  
                (SELECT indid FROM sysindexkeys  
                WHERE (id = a.id) AND (colid in  
                (SELECT colid FROM syscolumns WHERE (id = a.id) AND (name = a.name)))))))  
                AND (xtype = 'PK'))>0 then '1' else '0' end) IsPrimaryKey,b.name ColumnType,
                (case when a.isnullable=1 then '1' else '0' end) IsNull
                FROM  syscolumns a 
                left join systypes b on a.xtype=b.xusertype  
                inner join sysobjects d on a.id=d.id and d.xtype='U' and d.name<>'dtproperties' 
                WHERE d.name=@tableName order by a.id,a.colorder";

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
                    if (itemB.IsIdentity == "1")
                    {
                        tempBuilder.AppendLine("        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]");
                    }
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
                case "bit": return "bool";
                case "tinyint": return "byte";
                case "smallint":
                case "int": return "int";
                case "bigint": return "long";
                case "numeric":
                case "decimal":
                case "smallmoney":
                case "money": return "decimal";
                case "float": return "double";
                //case "real": return SqlDbType.Real;
                case "date":
                case "datetime":
                case "datetime2": return "DateTime";
                //case "datetimeoffset": return SqlDbType.DateTimeOffset;
                //case "smalldatetime": return SqlDbType.SmallDateTime;
                //case "time": return SqlDbType.Time;
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext": return "string";
                //case "binary": return SqlDbType.Binary;
                //case "varbinary": return SqlDbType.VarBinary;
                //case "image": return SqlDbType.Image;
                //case "timestamp": return SqlDbType.Timestamp;
                //case "uniqueidentifier": return SqlDbType.UniqueIdentifier;
                default: return "string";
            }
        }
    }
}