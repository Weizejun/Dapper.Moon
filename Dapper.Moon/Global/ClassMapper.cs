using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    /// <summary>
    /// 实体类映射
    /// </summary>
    internal class ClassMapper
    {
        /// <summary>
        /// 实体类映射
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static MapperTable Mapping<T>()
        {
            MapperTable mapperTable = new MapperTable()
            {
                EntityType = typeof(T),
                Properties = new List<PropertyMap>(),
                PropertiesDict = new Dictionary<string, PropertyMap>()
            };
            SetTableName(mapperTable);
            SetProperties(mapperTable);
            return mapperTable;
        }

        /// <summary>
        /// 实体类映射
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MapperTable Mapping(Type type)
        {
            MapperTable mapperTable = new MapperTable()
            {
                EntityType = type,
                Properties = new List<PropertyMap>(),
                PropertiesDict = new Dictionary<string, PropertyMap>()
            };
            SetTableName(mapperTable);
            SetProperties(mapperTable);
            return mapperTable;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="mapperTable"></param>
        private static void SetTableName(MapperTable mapperTable)
        {
            mapperTable.TableName = mapperTable.EntityType.Name;
            var tableAttribute = mapperTable.EntityType.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault();
            if (tableAttribute != null)
            {
                mapperTable.TableName = ((TableAttribute)tableAttribute).Name;
            }
        }

        /// <summary>
        /// 获取实体类中所有属性
        /// </summary>
        /// <param name="mapperTable"></param>
        private static void SetProperties(MapperTable mapperTable)
        {
            foreach (var propertyInfo in mapperTable.EntityType.GetProperties())
            {
                PropertyMap propertyMap = new PropertyMap()
                {
                    ColumnName = propertyInfo.Name,
                    PropertyInfo = propertyInfo,
                    IsIdentity = false,
                    Name = propertyInfo.Name,
                    IsPrimaryKey = false
                };

                var dga = propertyInfo.GetCustomAttributes(typeof(DatabaseGeneratedAttribute), true).FirstOrDefault();
                if (dga != null)
                {
                    if (((DatabaseGeneratedAttribute)dga).DatabaseGeneratedOption == DatabaseGeneratedOption.Identity)
                    {
                        //自增长
                        propertyMap.IsIdentity = true;

                        var seq = propertyInfo.GetCustomAttributes(typeof(SequenceAttribute), true).FirstOrDefault();
                        if (seq != null)
                        {
                            propertyMap.SequenceName = (seq as SequenceAttribute)?.Name;
                        }
                    }
                }

                var columnAttribute = propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
                if (columnAttribute != null)
                {
                    //数据库表中列名
                    propertyMap.ColumnName = ((ColumnAttribute)columnAttribute).Name;
                }

                var keyAttribute = propertyInfo.GetCustomAttributes(typeof(KeyAttribute), true).FirstOrDefault();
                if (keyAttribute != null)
                {
                    //主键
                    propertyMap.IsPrimaryKey = true;
                }

                var ignoredAttribute = propertyInfo.GetCustomAttributes(typeof(IgnoredAttribute), true).FirstOrDefault();
                if (ignoredAttribute != null)
                {
                    //忽略
                    propertyMap.Ignored = true;
                }

                mapperTable.Properties.Add(propertyMap);
                mapperTable.PropertiesDict.Add(propertyMap.Name, propertyMap);
            }
        }
    }

    /// <summary>
    /// 实体类忽略属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoredAttribute : Attribute
    {
        public IgnoredAttribute() { }
    }

    /// <summary>
    /// oracle 序列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class SequenceAttribute : Attribute
    {
        /// <summary>
        /// 序列名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 序列名称
        /// </summary>
        /// <param name="name"></param>
        public SequenceAttribute(string name) { this.Name = name; }
    }
}
