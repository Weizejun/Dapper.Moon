using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Dapper.Moon
{
    /// <summary>
    /// 通用工具类
    /// </summary>
    public static class CommonUtils
    {
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="isNullable">是否支持空类型</param>
        /// <returns></returns>
        public static DynamicMethod EmitGetProperty(PropertyInfo property, bool isNullable = false)
        {
            if (property == null || !property.CanRead) return null;
            var dynamicMethod = new DynamicMethod("get_" + property.Name, typeof(object),
                new[] { property.DeclaringType }, property.DeclaringType);
            var il = dynamicMethod.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, property.GetMethod);

            if (isNullable && Nullable.GetUnderlyingType(property.PropertyType) != null)
            {
                //Null类型
                il.Emit(OpCodes.Box, Nullable.GetUnderlyingType(property.PropertyType));
            }
            else
            {
                if (property.PropertyType.IsValueType)
                {
                    //如果是值类型，装箱
                    il.Emit(OpCodes.Box, property.PropertyType);
                }
                else
                {
                    //如果是引用类型，转换
                    il.Emit(OpCodes.Castclass, property.PropertyType);
                }
            }
            il.Emit(OpCodes.Ret);
            return dynamicMethod;
        }

        /// <summary>
        /// 将DataTable转换为标准的CSV字符串
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns>返回标准的CSV</returns>
        public static string ToCsvStr(DataTable dt)
        {
            //以半角逗号（即,）作分隔符，列为空也要表达其存在。
            //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
            //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataColumn item in dt.Columns)
            {
                sb.Append(item.ColumnName).Append(",");
            }

            sb = sb.Remove(sb.Length - 1, 1);
            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    colum = dt.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// 检查是否是单个字段表达式
        /// </summary>
        /// <param name="exp">new {a = "abc"} , i=>i.id</param>
        /// <param name="isSingle"></param>
        /// <returns></returns>
        public static bool ExpressionCheck(Expression exp, bool isSingle)
        {
            if (isSingle)
            {
                LambdaExpression le = exp as LambdaExpression;
                if (le.Body is NewExpression)
                {
                    throw new Exception("expression error, only one is supported");
                }
            }
            return true;
        }

        /// <summary>
        /// 解析匿名对象
        /// </summary>
        /// <param name="dynamic">new {a = "abc"}</param>
        /// <returns></returns>
        public static DynamicParameters GetDynamicParameters(object dynamic)
        {
            if (dynamic == null) return null;
            DynamicParameters dynamicParameters = new DynamicParameters();
            Type type = dynamic.GetType();
            //判断是否是匿名对象
            if (Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
              && type.IsGenericType && type.Name.Contains("AnonymousType")
              && (type.Name.StartsWith("<>"))
              && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic)
            {
                PropertyInfo[] properties = type.GetProperties();
                foreach (var item in properties)
                {
                    dynamicParameters.Add(item.Name, item.GetValue(dynamic));
                }
            }
            return dynamicParameters;
        }

        /// <summary>
        /// 是否是日期+时间格式
        /// </summary>
        /// <param name="source">2021-01-04 23:59:59 2021/01/04 23:59:59</param>
        /// <returns></returns>
        public static bool IsDateTime(string source)
        {
            string pattern = @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$";
            string pattern2 = @"^(((((1[6-9]|[2-9]\d)\d{2})/(0?[13578]|1[02])/(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})/(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$";
            return Regex.IsMatch(source, pattern) || Regex.IsMatch(source, pattern2);
        }
    }
}
