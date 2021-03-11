using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public class QueryableSqlBuilder
    {
        public ISqlDialect SqlDialect { get; set; }
        public SqlBuilderResult Where { get; set; }
        public string MasterTableName { get; set; }
        public List<string> TableNames { get; set; }
        public string SelectColumns { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        /// <summary>
        /// 排序字段，包含 asc和desc
        /// </summary>
        public string OrderBy { get; set; }
        public string GroupByField { get; set; }
        public string FunctionSql { get; set; }
        public SqlBuilderResult Function { get; set; }
        public List<SqlBuilderResult> Joins { get; set; }
        public MapperTable SelectIntoTable { get; set; }
        public string IntoTableColumn { get; set; }
        public bool IsDistinct { get; set; }
    }
}
