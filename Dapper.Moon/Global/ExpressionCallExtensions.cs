using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    /// <summary>
    /// 自定义表达式插件标记
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ExpressionCallPlugAttribute : Attribute
    {
        public ExpressionCallPlugAttribute() { }
    }

    public class ExpressionCallPlugContext
    {
        public DbType DbType { get; set; }

        public ISqlDialect SqlDialect { get; set; }

        public Dictionary<string, object> Value { get; set; }

        public string Result { get; set; }
    }

    [ExpressionCallPlug]
    public static class ExpressionCallPlug
    {
        private static ExpressionCallPlugContext context = new ExpressionCallPlugContext();
        public static int XXX(this DateTime dateTime, string dateType, DateTime field)
        {
            switch (context.DbType)
            {
                case DbType.SqlServer:
                    break;
                case DbType.MySql:
                    break;
                case DbType.Oracle:
                    break;
            }
            context.Result = "24";
            return 0;
        }
    }
}
