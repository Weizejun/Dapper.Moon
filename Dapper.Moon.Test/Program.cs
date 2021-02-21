using System;

namespace Dapper.Moon.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlServerTest();
            MySqlTest();
            OracleTest();
            Console.WriteLine("== over ==");
            Console.ReadKey();
        }

        static void SqlServerTest()
        {
            SqlServerTest sql = new SqlServerTest();
            //sql.BulkInsertTest();
            //sql.InsertTest();
            //sql.UpdateTest();
            //sql.DeleteTest();
            //sql.ProcTest();
            //sql.TransactionTest();
            sql.QueryTest();
            //sql.LambdaQueryTest();
            //sql.DbFirstTest();
        }

        static void MySqlTest()
        {
            MySqlTest sql = new MySqlTest();
            //sql.BulkInsertTest();
            //sql.InsertTest();
            //sql.UpdateTest();
            //sql.DeleteTest();
            //sql.ProcTest();
            //sql.TransactionTest();
            sql.QueryTest();
            //sql.LambdaQueryTest();
            //sql.DbFirstTest();
        }

        static void OracleTest()
        {
            OracleTest sql = new OracleTest();
            //sql.InsertTest();
            //sql.UpdateTest();
            //sql.DeleteTest();
            //sql.ProcTest();
            //sql.TransactionTest();
            //sql.QueryTest();
            //sql.LambdaQueryTest();
            //sql.DbFirstTest();
        }
    }
}
