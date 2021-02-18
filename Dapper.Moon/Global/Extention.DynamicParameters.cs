using System.Text;

namespace Dapper.Moon
{
    /// <summary>
    /// dapper DynamicParameters 扩展
    /// </summary>
    public static partial class Extention
    {
        /// <summary>
        /// 查看参数名 + 参数值
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string RawString(this DynamicParameters s)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var item in s?.ParameterNames)
            {
                object val = s.Get<object>(item);
                builder.Append(item).Append("=").Append(val?.ToString()).Append(",");
            }
            if (builder.Length > 0)
            {
                return builder.ToString(0, builder.Length - 1);
            }
            return "";
        }
    }
}
