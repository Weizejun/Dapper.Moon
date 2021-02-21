using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;

namespace Dapper.Moon
{
    public class MySqlRepository : BaseRepository, IRepository
    {
        private string _ConnectionString { get; set; }
        private ISqlDialect _SqlDialect = null;
        public MySqlRepository(string connectionString)
        {
            _ConnectionString = connectionString;
            _Connection = new MySqlConnection(connectionString);
            _SqlDialect = new MySqlDialect();
        }

        public IDbConnection Connection { get { return _Connection; } }
        public IAop Aop { get { return _Aop; } set { _Aop = value; } }
        public ISqlDialect SqlDialect { get { return _SqlDialect; } }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public override int BulkInsert(DataTable table)
        {
            if (table?.Rows?.Count == 0) return 0;
            int rowCount = 0;
            string tmpPath = "";
            bool wasClosed = _Connection.State == ConnectionState.Closed;
            if (wasClosed) _Connection.Open();
            try
            {
                tmpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DateTime.Now.ToFileTime() + "_" + Guid.NewGuid().ToString("N") + ".tmp");
                string csv = CommonUtils.ToCsvStr(table);
                File.WriteAllText(tmpPath, csv, Encoding.UTF8);
                MySqlBulkLoader bulk = new MySqlBulkLoader(_Connection as MySqlConnection)
                {
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                    LineTerminator = "\r\n",
                    FileName = tmpPath,
                    NumberOfLinesToSkip = 1,//跳过第1行
                    TableName = table.TableName,
                    Local = false,
                    CharacterSet = "UTF8"
                };
                bulk.Columns.AddRange(table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList());
                rowCount = bulk.Load();
                table = null;
            }
            finally
            {
                if (wasClosed) _Connection.Close();
                if (File.Exists(tmpPath))
                    File.Delete(tmpPath);
            }
            return rowCount;
        }

        protected override IDbCommand GetCommand(string sql, SqlMapper.IDynamicParameters param = null, CommandType? commandType = null)
        {
            MySqlCommand cmd = new MySqlCommand(sql, _Connection as MySqlConnection);
            cmd.CommandType = commandType == null ? CommandType.Text : (CommandType)commandType;
            cmd.CommandTimeout = _CommandTimeout;
            if (param != null)
            {
                var sqlParams = (param as MySqlDynamicParameters).Get() as List<MySqlParameter>;
                cmd.Parameters.AddRange(sqlParams.ToArray());
            }
            return cmd;
        }

        protected override IDataAdapter GetAdapter(IDbCommand command)
        {
            return new MySqlDataAdapter(command as MySqlCommand);
        }

        public void Dispose()
        {
            _Connection?.Close();
        }
    }
}
