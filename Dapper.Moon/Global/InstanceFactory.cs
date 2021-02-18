using System;
using System.Collections.Generic;
using System.Reflection;

namespace Dapper.Moon
{
    internal class InstanceFactory
    {
        static Assembly assembly = Assembly.Load(GlobalConfig.AssemblyName);

        public static T CreateInstance<T>(string className)
        {
            Type type = assembly.GetType(GlobalConfig.AssemblyName + "." + className);
            var result = (T)Activator.CreateInstance(type, true);
            return result;
        }

        public static T CreateInstance<T>(string className, object[] args)
        {
            Type type = assembly.GetType(GlobalConfig.AssemblyName + "." + className);
            var result = (T)Activator.CreateInstance(type, args);
            return result;
        }

        #region
        public static IInsertable<T> CreateInsertable<T>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Insertable`1, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(typeof(T));
            IInsertable<T> instance = (IInsertable<T>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IUpdateable<T> CreateUpdateable<T>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Updateable`1, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(typeof(T));
            IUpdateable<T> instance = (IUpdateable<T>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IDeleteable<T> CreateDeleteable<T>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Deleteable`1, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(typeof(T));
            IDeleteable<T> instance = (IDeleteable<T>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IQueryable<T> CreateQueryable<T>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Queryable`1, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(typeof(T));
            IQueryable<T> instance = (IQueryable<T>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IQueryable<T, T2> CreateQueryable<T, T2>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Queryable`2, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(new Type[] 
            { typeof(T), typeof(T2) });
            IQueryable<T, T2> instance = (IQueryable<T, T2>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IQueryable<T, T2, T3> CreateQueryable<T, T2, T3>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Queryable`3, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(new Type[] 
            { typeof(T), typeof(T2), typeof(T3) });
            IQueryable<T, T2, T3> instance = (IQueryable<T, T2, T3>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IQueryable<T, T2, T3, T4> CreateQueryable<T, T2, T3, T4>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Queryable`4, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(new Type[] 
            { typeof(T), typeof(T2), typeof(T3), typeof(T4) });
            IQueryable<T, T2, T3, T4> instance = (IQueryable<T, T2, T3, T4>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IQueryable<T, T2, T3, T4, T5> CreateQueryable<T, T2, T3, T4, T5>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Queryable`5, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(new Type[] 
            { typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
            IQueryable<T, T2, T3, T4, T5> instance = (IQueryable<T, T2, T3, T4, T5>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }

        public static IQueryable<T, T2, T3, T4, T5, T6> CreateQueryable<T, T2, T3, T4, T5, T6>(DbType dbType, IRepository _Repository)
        {
            Type classType = Type.GetType($"{GlobalConfig.AssemblyName}.{dbType.ToString()}Queryable`6, {GlobalConfig.AssemblyName}");
            Type constructedType = classType.MakeGenericType(new Type[] 
            { typeof(T), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
            IQueryable<T, T2, T3, T4, T5, T6> instance = (IQueryable<T, T2, T3, T4, T5, T6>)Activator.CreateInstance(constructedType, new object[] { _Repository });
            return instance;
        }
        #endregion
    }
}
