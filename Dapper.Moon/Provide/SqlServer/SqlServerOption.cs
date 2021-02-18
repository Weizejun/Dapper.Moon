using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    /// <summary>
    /// SqlServer配置
    /// </summary>
    public class SqlServerOption
    {
        public SqlServerOption() { }

        /// <summary>
        /// SqlServer配置
        /// </summary>
        /// <param name="useRowNumberForPaging">是否启用RowNumber分页</param>
        public SqlServerOption(bool useRowNumberForPaging)
        {
            this.UseRowNumberForPaging = useRowNumberForPaging;
        }

        /// <summary>
        /// 是否启用RowNumber分页
        /// </summary>
        public bool UseRowNumberForPaging { get; set; } = false;
    }
}