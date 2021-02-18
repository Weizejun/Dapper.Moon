using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Dapper.Moon
{
    public class SqlServerRepository : BaseRepository, IRepository
    {
        private string _ConnectionString { get; set; }
        private ISqlDialect _SqlDialect = null;

        public SqlServerRepository(string connectionString)
        {
            _ConnectionString = connectionString;
            _Connection = new SqlConnection(connectionString);
            _SqlDialect = new SqlServerDialect();
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
            int rowCount = table.Rows.Count;
            bool wasClosed = _Connection.State == ConnectionState.Closed;
            if (wasClosed) _Connection.Open();
            using (SqlBulkCopy bulkcopy =
                    _Transaction == null ?
                    new SqlBulkCopy(_ConnectionString, SqlBulkCopyOptions.Default) :
                    new SqlBulkCopy(_Connection as SqlConnection, SqlBulkCopyOptions.Default, _Transaction as SqlTransaction))
            {
                bulkcopy.DestinationTableName = table.TableName;
                bulkcopy.BulkCopyTimeout = _CommandTimeout;
                bulkcopy.BatchSize = rowCount;
                foreach (DataColumn item in table.Columns)
                {
                    bulkcopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                }
                bulkcopy.WriteToServer(table);
                table = null;
            }
            if (wasClosed) _Connection.Close();
            return rowCount;
        }

        protected override IDbCommand GetCommand(string sql, SqlMapper.IDynamicParameters param = null, CommandType? commandType = null)
        {
            SqlCommand cmd = new SqlCommand(sql, _Connection as SqlConnection);
            cmd.CommandType = commandType == null ? CommandType.Text : (CommandType)commandType;
            cmd.CommandTimeout = _CommandTimeout;
            if (param != null)
            {
                var sqlParams = (param as SqlServerDynamicParameters).Get() as List<SqlParameter>;
                cmd.Parameters.AddRange(sqlParams.ToArray());
            }
            return cmd;

        }

        protected override IDataAdapter GetAdapter(IDbCommand command)
        {
            return new SqlDataAdapter(command as SqlCommand);
        }

        public void Dispose()
        {
            _Connection?.Close();
        }
    }
}
