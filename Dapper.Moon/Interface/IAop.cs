using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public interface IAop
    {
        /// <summary>
        /// 执行前
        /// </summary>
        Action<SqlBuilderResult> OnExecuting { get; set; }
    }
}
