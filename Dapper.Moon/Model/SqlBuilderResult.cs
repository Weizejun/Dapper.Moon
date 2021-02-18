using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public class SqlBuilderResult
    {
        /// <summary>
        /// sql语句
        /// </summary>
        public string Sql { get; set; }
        /// <summary>
        /// sql参数 可使用RawString方法查看参数名称和值
        /// </summary>
        public DynamicParameters DynamicParameters { get; set; }
    }
}
