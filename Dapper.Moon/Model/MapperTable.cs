using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    /// <summary>
    /// 映射表信息
    /// </summary>
    public class MapperTable
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public Type EntityType { get; set; }
        /// <summary>
        /// 属性集合
        /// </summary>
        public IList<PropertyMap> Properties { get; set; }
        /// <summary>
        /// 属性字典集合 key = 属性名称
        /// </summary>
        public Dictionary<string, PropertyMap> PropertiesDict { get; set; }
    }
}
