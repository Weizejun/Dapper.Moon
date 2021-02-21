using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Collections.Generic;

namespace Dapper.Moon
{
    /// <summary>
    /// 表达式解析
    /// </summary>
    public partial class ExpressionProvide
    {
        //数据库方言
        private ISqlDialect SqlDialect = null;
        //参数
        private DynamicParameters Parameters = null;
        //是否需要前缀
        private bool IsPrefix = false;
        //是否转义列名
        private bool IsQuote = true;
        //参数前缀
        private const string ParameterName = "param";
        //参数下标
        private int Index = 0;
        //表达式实体类
        private Dictionary<Type, MapperTable> dictMapperTable = null;

        private ExpressionProvide(ISqlDialect _SqlDialect, params MapperTable[] tables)
        {
            LoadMappingTable(tables);
            this.SqlDialect = _SqlDialect;
        }

        public static ExpressionProvide Create(ISqlDialect _SqlDialect, params MapperTable[] tables)
        {
            if (tables == null || tables.Length == 0) throw new Exception("the parameter is empty");
            return new ExpressionProvide(_SqlDialect, tables);
        }

        #region MappingTable
        public void AppendMappingTable(MapperTable item)
        {
            if (!dictMapperTable.ContainsKey(item.EntityType))
            {
                dictMapperTable.Add(item.EntityType, item);
            }
        }

        private void LoadMappingTable(MapperTable[] tables)
        {
            if (tables == null || tables.Length == 0) throw new Exception("the parameter is empty");
            dictMapperTable = new Dictionary<Type, MapperTable>();
            foreach (var item in tables)
            {
                if (!dictMapperTable.ContainsKey(item.EntityType))
                {
                    dictMapperTable.Add(item.EntityType, item);
                }
            }
        }

        /// <summary>
        /// 通过表达式参数 获取实体类
        /// </summary>
        /// <param name="exp"></param>
        private void LoadMappingTable(Expression exp)
        {
            LambdaExpression le = exp as LambdaExpression;
            if (le?.Parameters?.Count > 0)
            {
                if (le.Parameters[0].Type.FullName.Contains("Dapper.Moon.MoonFunc"))
                {
                    foreach (var item in le.Parameters[0].Type.GenericTypeArguments)
                    {
                        if (!dictMapperTable.ContainsKey(item))
                        {
                            dictMapperTable.Add(item, ClassMapper.Mapping(item));
                        }
                    }
                }
                else
                {
                    //自定义实体类型 不用判断是否是系统类型 例如：string
                    var type = le.Parameters[0].Type;
                    if (!dictMapperTable.ContainsKey(type))
                    {
                        dictMapperTable.Add(type, ClassMapper.Mapping(type));
                    }
                }
            }
        }
        #endregion MappingTable

        /// <summary>
        /// 表达式解析入口路由
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <param name="isPrefix">是否需要前缀</param>
        /// <param name="isQuote">是否转义列名</param>
        /// <returns></returns>
        public SqlBuilderResult ExpressionRouter(Expression exp, bool isPrefix = false, bool isQuote = true)
        {
            if (exp == null) return null;
            IsPrefix = isPrefix;
            IsQuote = isQuote;
            Parameters = new DynamicParameters();
            string sql = Resolve(exp);
            return new SqlBuilderResult()
            {
                Sql = sql,
                DynamicParameters = Parameters
            };
        }

        #region Resolve
        private string Resolve(Expression exp)
        {
            if (exp is BinaryExpression)
            {
                //表示具有二进制运算符的表达式
                return BinarExpressionProvider(exp);
            }
            else if (exp is ConstantExpression)
            {
                //表示具有常数值的表达式
                return ConstantExpressionProvider(exp);
            }
            else if (exp is LambdaExpression)
            {
                //介绍 lambda 表达式。 它捕获一个类似于 .NET 方法主体的代码块
                return LambdaExpressionProvider(exp);
            }
            else if (exp is MemberExpression)
            {
                //表示访问字段或属性
                return MemberExpressionProvider(exp);
            }
            else if (exp is MethodCallExpression)
            {
                //表示对静态方法或实例方法的调用
                return MethodCallExpressionProvider(exp);
            }
            else if (exp is NewArrayExpression)
            {
                //表示创建一个新数组，并可能初始化该新数组的元素
                return NewArrayExpressionProvider(exp);
            }
            else if (exp is ParameterExpression)
            {
                //表示一个命名的参数表达式。
                return ParameterExpressionProvider(exp);
            }
            else if (exp is UnaryExpression)
            {
                //表示具有一元运算符的表达式
                return UnaryExpressionProvider(exp);
            }
            else if (exp is NewExpression)
            {
                //表示构造函数调用
                return NewExpressionProvider(exp);
            }
            return "";
        }
        #endregion Resolve

        /// <summary>
        /// 解析 i=>new {a = 1}
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string NewExpressionProvider(Expression exp)
        {
            StringBuilder sbSelectFields = new StringBuilder();
            NewExpression ne = exp as NewExpression;
            if (ne.Members == null) return "";
            for (var i = 0; i < ne.Members.Count; i++)
            {
                //别名或属性名
                string name = ne.Members[i].Name;
                //判断是否是属性访问、方法调用
                if (ne.Arguments[i].NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression mex = ne.Arguments[i] as MemberExpression;
                    //当前属性所属类型
                    Type entityType = mex.Member.ReflectedType;
                    if (entityType.ToString() == "System.String")
                    {
                        string methodName = mex.Member.Name;
                        switch (methodName)
                        {
                            case "Length":
                                sbSelectFields.Append(SqlDialect.Length(Resolve(mex.Expression))).Append(" ").Append(name).Append(",");
                                break;
                            default:
                                throw new Exception("unsupported expression");
                        }
                        //跳出本次循环
                        continue;
                    }
                    if (entityType.ToString() == "System.DateTime")
                    {
                        string methodName = mex.Member.Name;
                        switch (methodName)
                        {
                            case "Now":
                                sbSelectFields.Append(SqlDialect.GetDate).Append(" ").Append(name).Append(",");
                                break;
                            case "Year":
                                sbSelectFields.Append(SqlDialect.Year(Resolve(mex.Expression))).Append(" ").Append(name).Append(",");
                                break;
                            case "Month":
                                sbSelectFields.Append(SqlDialect.Month(Resolve(mex.Expression))).Append(" ").Append(name).Append(",");
                                break;
                            case "Day":
                                sbSelectFields.Append(SqlDialect.Day(Resolve(mex.Expression))).Append(" ").Append(name).Append(",");
                                break;
                            case "Hour":
                                sbSelectFields.Append(SqlDialect.Hour(Resolve(mex.Expression))).Append(" ").Append(name).Append(",");
                                break;
                            default:
                                throw new Exception("unsupported expression");
                        }
                        //跳出本次循环
                        continue;
                    }
                    //属性访问      
                    string argumentString = ne.Arguments[i].ToString();
                    string[] arguments = argumentString.Split('.');
                    string colName = arguments[arguments.Length - 1];
                    if (IsPrefix)
                    {
                        //i.t1.id = t1.id 、 i.t1 = t1.*
                        //前缀
                        string prefix = arguments[1];
                        switch (arguments.Length)
                        {
                            case 2:
                                sbSelectFields.Append(prefix + ".*,");
                                break;
                            case 3:
                                //定义别名的情况
                                if (name != colName)
                                {
                                    colName = GetColumnName(entityType, colName);
                                    sbSelectFields.Append(prefix).Append(".").Append(IsQuote ? SqlDialect.SetSqlName(colName) : colName).Append(" ").Append(name).Append(",");
                                }
                                else
                                {
                                    colName = GetColumnName(entityType, colName);
                                    sbSelectFields.Append(prefix).Append(".").Append(IsQuote ? SqlDialect.SetSqlName(colName) : colName).Append(",");
                                }
                                break;
                        }
                    }
                    else
                    {
                        //定义别名的情况
                        if (name != colName)
                        {
                            colName = GetColumnName(entityType, colName);
                            colName = IsQuote ? SqlDialect.SetSqlName(colName) : colName;
                            sbSelectFields.Append(colName).Append(" ").Append(name).Append(",");
                        }
                        else
                        {
                            colName = GetColumnName(entityType, colName);
                            sbSelectFields.Append(IsQuote ? SqlDialect.SetSqlName(colName) : colName).Append(",");
                        }
                    }
                }
                else if (ne.Arguments[i].NodeType == ExpressionType.Call)
                {
                    //方法调用
                    string result = Resolve(ne.Arguments[i]);
                    sbSelectFields.Append(result).Append(" ").Append(name).Append(",");
                }
            }
            if (sbSelectFields.Length > 1)
            {
                sbSelectFields.Remove(sbSelectFields.Length - 1, 1);
            }
            return sbSelectFields.ToString();
        }

        private string BinarExpressionProvider(Expression exp)
        {
            BinaryExpression be = exp as BinaryExpression;
            ExpressionType type = be.NodeType;
            StringBuilder leftSql = new StringBuilder("(");
            leftSql.Append(Resolve(be.Left));
            leftSql.Append(ExpressionTypeCast(type));
            string rightSql = Resolve(be.Right);
            if (rightSql == "null")
            {
                if (leftSql.ToString().EndsWith(" = "))
                {
                    string s = leftSql.ToString().Substring(0, leftSql.Length - 2);
                    leftSql.Clear().Append(s).Append("is null");
                }
                else if (leftSql.ToString().EndsWith(" <> "))
                {
                    string s = leftSql.ToString().Substring(0, leftSql.Length - 3);
                    leftSql.Clear().Append(s).Append("is not null");
                }
            }
            else
            {
                leftSql.Append(rightSql);
            }
            leftSql.Append(")");
            return leftSql.ToString();
        }

        /// <summary>
        /// 表示具有常数值的表达式
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string ConstantExpressionProvider(Expression exp)
        {
            ConstantExpression ce = exp as ConstantExpression;
            if (ce.Value == null)
            {
                return "null";
            }
            else if (ce?.Value?.ToString() == "True")
            {
                return "1=1";
            }
            else if (ce.Value is ValueType)
            {
                SetParameter(GetValueType(ce.Value));
                return SqlDialect.ParameterPrefix + ParameterName + (Index);
            }
            else if (ce.Value is string || ce.Value is DateTime || ce.Value is char)
            {
                SetParameter(GetValueType(ce.Value));
                return SqlDialect.ParameterPrefix + ParameterName + (Index);
            }
            return "";
        }

        private string LambdaExpressionProvider(Expression exp)
        {
            LambdaExpression le = exp as LambdaExpression;
            return Resolve(le.Body);
        }

        /// <summary>
        /// 表示访问字段或属性
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string MemberExpressionProvider(Expression exp)
        {
            if (!exp.ToString().StartsWith("value"))
            {
                MemberExpression me = exp as MemberExpression;
                if (me.Member.DeclaringType.ToString() == "System.DateTime")
                {
                    switch (me.Member.Name)
                    {
                        case "Now":
                            return SqlDialect.GetDate;
                        case "Year":
                            return SqlDialect.Year(Resolve(me.Expression));
                        case "Month":
                            return SqlDialect.Month(Resolve(me.Expression));
                        case "Day":
                            return SqlDialect.Day(Resolve(me.Expression));
                        case "Hour":
                            return SqlDialect.Hour(Resolve(me.Expression));
                        default:
                            throw new Exception("unsupported expression");
                    }
                }
                if (me.Member.DeclaringType.ToString() == "System.String")
                {
                    string methodName = me.Member.Name;
                    switch (methodName)
                    {
                        case "Length":
                            return SqlDialect.Length(Resolve(me.Expression));
                        default:
                            throw new Exception("unsupported expression");
                    }
                }

                if (IsPrefix)
                {
                    //i.t1.id = t1.id 、 i.t1 = t1.*
                    string expression = me.ToString();
                    string[] arguments = expression.Split('.');
                    string prefix = arguments[1];
                    switch (arguments.Length)
                    {
                        case 2:
                            return prefix + ".*";
                        case 3:
                            //当前属性所属类型
                            Type entityType = me.Member.ReflectedType;
                            string colName = GetColumnName(entityType, me.Member.Name);
                            return prefix + "." + (IsQuote ? SqlDialect.SetSqlName(colName) : colName);
                    }
                }
                else
                {
                    //当前属性所属类型
                    Type entityType = me.Member.ReflectedType;
                    string colName = GetColumnName(entityType, me.Member.Name);
                    return (IsQuote ? SqlDialect.SetSqlName(colName) : colName);
                }
            }
            else
            {
                var result = Expression.Lambda(exp).Compile().DynamicInvoke();
                if (result == null)
                {
                    return "null";
                }
                else if (result is ValueType)
                {
                    SetParameter(GetValueType(result));
                    return SqlDialect.ParameterPrefix + ParameterName + (Index);
                }
                else if (result is string || result is DateTime || result is char)
                {
                    SetParameter(GetValueType(result));
                    return SqlDialect.ParameterPrefix + ParameterName + (Index);
                }
                else if (result is int[])
                {
                    var array = result as int[];
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in array)
                    {
                        SetParameter(item);
                        sb.Append(SqlDialect.ParameterPrefix + ParameterName + (Index)).Append(",");
                    }
                    return sb.ToString(0, sb.Length - 1);
                }
                else if (result is string[])
                {
                    var array = result as string[];
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in array)
                    {
                        SetParameter(item);
                        sb.Append(SqlDialect.ParameterPrefix + ParameterName + (Index)).Append(",");
                    }
                    return sb.ToString(0, sb.Length - 1);
                }
            }
            return "";
        }

        /// <summary>
        /// 表示对静态方法或实例方法的调用
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        private string MethodCallExpressionProvider(Expression exp)
        {
            MethodCallExpression mce = exp as MethodCallExpression;
            string declaringType = mce.Method.DeclaringType.ToString();
            if (declaringType == "System.String")
            {
                //获取字段
                string left = Resolve(mce.Object ?? mce.Arguments[0]);
                object _value = "";
                if (mce.Object == null)
                {
                    switch (mce.Arguments[0].NodeType)
                    {
                        case ExpressionType.Call:
                        case ExpressionType.Constant:
                            ConstantExpression ce = mce.Arguments[1] as ConstantExpression;
                            //常量值访问
                            _value = ce?.Value;
                            break;
                        case ExpressionType.MemberAccess:
                            //属性访问
                            if (mce.Arguments.Count >= 2)
                            {
                                _value = Expression.Lambda(mce.Arguments[1]).Compile().DynamicInvoke();
                            }
                            break;
                        default:
                            throw new Exception("unsupported expression");
                    }
                }
                string result = "";
                switch (mce.Method.Name)
                {
                    case "Contains":
                        SetParameter("%" + GetValueType(_value) + "%");
                        result = string.Format("{0} like {1}", left, SqlDialect.ParameterPrefix + ParameterName + (Index));
                        break;
                    case "EndsWith":
                        SetParameter("%" + GetValueType(_value));
                        result = string.Format("{0} like {1}", left, SqlDialect.ParameterPrefix + ParameterName + (Index));
                        break;
                    case "StartsWith":
                        SetParameter(GetValueType(_value) + "%");
                        result = string.Format("{0} like {1}", left, SqlDialect.ParameterPrefix + ParameterName + (Index));
                        break;
                    case "IsNullOrEmpty":
                        result = string.Format("({0} is null or {0} = '')", left);
                        break;
                    case "Concat":
                        SetParameter(GetValueType(_value));
                        result = SqlDialect.Concat(left, SqlDialect.ParameterPrefix + ParameterName + (Index));
                        break;
                    case "IndexOf":
                        _value = (mce.Arguments[0] as ConstantExpression)?.Value;
                        SetParameter(GetValueType(_value));
                        result = SqlDialect.IndexOf(left, SqlDialect.ParameterPrefix + ParameterName + (Index));
                        break;
                    case "PadLeft":
                        _value = (mce.Arguments[0] as ConstantExpression)?.Value;
                        object val2 = (mce.Arguments[1] as ConstantExpression)?.Value;
                        SetParameter(GetValueType(_value));
                        _value = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        SetParameter(GetValueType(val2));
                        val2 = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        result = SqlDialect.PadLeft(left, _value, val2);
                        break;
                    case "PadRight":
                        _value = (mce.Arguments[0] as ConstantExpression)?.Value;
                        val2 = (mce.Arguments[1] as ConstantExpression)?.Value;
                        SetParameter(GetValueType(_value));
                        _value = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        SetParameter(GetValueType(val2));
                        val2 = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        result = SqlDialect.PadRight(left, _value, val2);
                        break;
                    case "Replace":
                        _value = (mce.Arguments[0] as ConstantExpression)?.Value;
                        val2 = (mce.Arguments[1] as ConstantExpression)?.Value;
                        SetParameter(GetValueType(_value));
                        _value = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        SetParameter(GetValueType(val2));
                        val2 = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        result = SqlDialect.Replace(left, _value, val2);
                        break;
                    case "Substring":
                        _value = (mce.Arguments[0] as ConstantExpression)?.Value;
                        val2 = (mce.Arguments[1] as ConstantExpression)?.Value;
                        SetParameter(GetValueType(_value));
                        _value = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        SetParameter(GetValueType(val2));
                        val2 = SqlDialect.ParameterPrefix + ParameterName + (Index);
                        result = SqlDialect.Substring(left, _value, val2);
                        break;
                    case "ToLower":
                        result = SqlDialect.ToLower(left);
                        break;
                    case "ToUpper":
                        result = SqlDialect.ToUpper(left);
                        break;
                    case "Trim":
                        result = SqlDialect.Trim(left);
                        break;
                    case "TrimEnd":
                        result = SqlDialect.TrimEnd(left);
                        break;
                    case "TrimStart":
                        result = SqlDialect.TrimStart(left);
                        break;
                    case "FirstOrDefault":
                        result = SqlDialect.FirstOrDefault(left);
                        break;
                    default:
                        throw new Exception("unsupported expression");
                }
                return result;
            }
            else if (declaringType == "System.Linq.Enumerable")
            {
                return string.Format("{0} in ({1})", Resolve(mce.Arguments[1]), Resolve(mce.Arguments[0]));
            }
            else if (declaringType == "System.Guid")
            {
                return SqlDialect.Guid;
            }
            else if (declaringType == "System.DateTime")
            {
                switch (mce.Method.Name)
                {
                    case "Parse":
                        return SqlDialect.ToDateTime(Resolve(mce.Arguments[0]));
                    default:
                        throw new Exception("unsupported expression");
                }
            }
            else if (declaringType == "Dapper.Moon.DbFunc")
            {
                #region DbFunc
                if (mce.Method.Name.Contains("Datediff"))
                {
                    if (mce.Method.Name.IndexOf('_') != -1)
                    {
                        string dateType = mce.Method.Name.Substring(mce.Method.Name.IndexOf('_') + 1);
                        string left = Resolve(mce.Arguments[0]);
                        string right = Resolve(mce.Arguments[1]);
                        return SqlDialect.Datediff(dateType, left, right);
                    }
                }
                string result = "";
                string column = "";
                if (mce.Arguments.Count > 0)
                {
                    column = Resolve(mce.Arguments[0]);
                }
                switch (mce.Method.Name)
                {
                    case "Count":
                        result = $"count({column})";
                        break;
                    case "Sum":
                        result = $"sum({column})";
                        break;
                    case "Max":
                        result = $"max({column})";
                        break;
                    case "Min":
                        result = $"min({column})";
                        break;
                    case "Avg":
                        result = $"avg({column})";
                        break;
                    case "Between":
                        string param1 = Resolve(mce.Arguments[1]);
                        string param2 = Resolve(mce.Arguments[2]);
                        object val1 = Parameters.Get<object>(param1);
                        object val2 = Parameters.Get<object>(param2);
                        if (CommonUtils.IsDateTime(val1.ToString()) && CommonUtils.IsDateTime(val2.ToString()))
                        {
                            //是否是日期格式 如果是日期格式 需要转型
                            param1 = SqlDialect.ToDateTime(param1);
                            param2 = SqlDialect.ToDateTime(param2);
                        }
                        result = SqlDialect.Between(column, param1, param2);
                        break;
                    case "IsNull":
                        result = SqlDialect.IsNull(column, Resolve(mce.Arguments[1]));
                        break;
                    case "DateTime":
                        result = SqlDialect.GetDate;
                        break;
                    case "Sequence":
                        result = (mce.Arguments[0] as ConstantExpression)?.Value.ToString();
                        break;
                }
                return result;
                #endregion DbFunc
            }
            return "";
        }

        private string NewArrayExpressionProvider(Expression exp)
        {
            NewArrayExpression ae = exp as NewArrayExpression;
            StringBuilder sb = new StringBuilder();
            foreach (Expression ex in ae.Expressions)
            {
                sb.Append(Resolve(ex)).Append(",");
            }
            return sb.ToString(0, sb.Length - 1);
        }

        private string ParameterExpressionProvider(Expression exp)
        {
            ParameterExpression pe = exp as ParameterExpression;
            return pe.Type.Name;
        }

        private string UnaryExpressionProvider(Expression exp)
        {
            UnaryExpression ue = exp as UnaryExpression;
            string result = Resolve(ue.Operand);
            ExpressionType type = exp.NodeType;
            if (type == ExpressionType.Not)
            {
                if (result.Contains(" in "))
                {
                    result = result.Replace(" in ", " not in ");
                }
                if (result.Contains(" like "))
                {
                    result = result.Replace(" like ", " not like ");
                }
                if (result.Contains(" is null"))
                {
                    result = result.Replace(" is null ", " is not null ");
                }
                if (result.Contains("= ''"))
                {
                    result = result.Replace("= ''", "<> ''");
                }
            }
            return result;
        }

        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="val"></param>
        private void SetParameter(object val)
        {
            Index++;
            string paramName = ParameterName + (Index);
            Parameters.Add(paramName, val);
        }

        /// <summary>
        /// 获取实体类数据库表中列名
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        private string GetColumnName(Type entityType, string colName)
        {
            if (dictMapperTable.ContainsKey(entityType))
            {
                if (dictMapperTable[entityType].PropertiesDict.ContainsKey(colName))
                {
                    colName = dictMapperTable[entityType].PropertiesDict[colName].ColumnName;
                }
            }
            return colName;
        }

        public SqlBuilderResult ExpressionFieldValue(Expression exp, bool isPrefix = false, bool isQuote = true)
        {
            UnaryExpression ue = exp as UnaryExpression;
            if (ue.Operand.NodeType != ExpressionType.Equal)
            {
                IsPrefix = isPrefix;
                IsQuote = isQuote;
                Parameters = new DynamicParameters();
                BinaryExpression be = ue.Operand as BinaryExpression;
                ExpressionType type = be.NodeType;
                StringBuilder leftSql = new StringBuilder();
                string leftResult = Resolve(be.Left);
                leftSql.Append(leftResult).Append("=").Append(leftResult);
                leftSql.Append(ExpressionTypeCast(type));
                leftSql.Append(Resolve(be.Right));
                return new SqlBuilderResult()
                {
                    Sql = leftSql.ToString(),
                    DynamicParameters = Parameters
                };
            }
            return ExpressionRouter(exp, isPrefix, isQuote);
        }

        #region 表达式类型转换
        /// <summary>
        /// 表达式类型转换
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ExpressionTypeCast(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return " and ";
                case ExpressionType.Equal:
                    return " = ";
                case ExpressionType.GreaterThan:
                    return " > ";
                case ExpressionType.GreaterThanOrEqual:
                    return " >= ";
                case ExpressionType.LessThan:
                    return " < ";
                case ExpressionType.LessThanOrEqual:
                    return " <= ";
                case ExpressionType.NotEqual:
                    return " <> ";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return " or ";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return " + ";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return " - ";
                case ExpressionType.Divide:
                    return " / ";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return " * ";
                default:
                    return null;
            }
        }

        /// <summary>
        /// 值类型转换
        /// </summary>
        /// <param name="_value"></param>
        /// <returns></returns>
        private object GetValueType(object _value)
        {
            var _type = _value.GetType().Name;
            switch (_type)
            {
                case "Decimal": return _value;
                case "Int32": return _value;
                case "DateTime": return _value;
                case "String": return _value;
                case "Char": return _value;
                case "Boolean": return _value;
                default: return _value;
            }
        }
        #endregion 表达式类型转换
    }
}