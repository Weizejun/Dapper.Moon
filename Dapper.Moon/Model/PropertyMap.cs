using System.Reflection;

namespace Dapper.Moon
{
    /// <summary>
    /// 属性映射
    /// </summary>
    public class PropertyMap
    {
        /// <summary>
        /// 数据库中的列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否是自增长
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// oracle 序列名称
        /// </summary>
        public string SequenceName { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否忽略该列
        /// </summary>
        public bool Ignored { get; set; }
        /// <summary>
        /// PropertyInfo
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }
    }
}
