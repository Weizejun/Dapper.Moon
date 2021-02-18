using System;
using System.IO;

namespace Dapper.Moon
{
    /// <summary>
    /// 运行环境检测
    /// </summary>
    public class EnvironmentManagement
    {
        private static string GetPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static void JsonRuntime()
        {
            string filePath = Path.Combine(GetPath(), "Newtonsoft.Json.dll");
            if (!File.Exists(filePath))
            {
                throw new Exception("Newtonsoft.Json.dll reference not found");
            }
        }

        public static void DapperRuntime()
        {
            string filePath = Path.Combine(GetPath(), "Dapper.dll");
            if (!File.Exists(filePath))
            {
                throw new Exception("Dapper.dll reference not found");
            }
        }

        public static void MySqlRuntime()
        {
            string filePath = Path.Combine(GetPath(), "MySql.Data.dll");
            if (!File.Exists(filePath))
            {
                throw new Exception("MySql.Data.dll reference not found");
            }
        }

        public static void OracleRuntime()
        {
            string filePath = Path.Combine(GetPath(), "Oracle.ManagedDataAccess.dll");
            if (!File.Exists(filePath))
            {
                throw new Exception("Oracle.ManagedDataAccess.dll reference not found");
            }
        }
    }
}
