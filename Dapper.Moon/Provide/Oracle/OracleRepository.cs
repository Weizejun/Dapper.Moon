using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public class OracleRepository : BaseRepository, IRepository
    {
        private string _ConnectionString = null;
        private ISqlDialect _SqlDialect = null;

        public OracleRepository(string connectionString)
        {
            _ConnectionString = connectionString;
            _Connection = new OracleConnection(connectionString);
            _SqlDialect = new OracleDialect();
        }

        public IDbConnection Connection { get { return _Connection; } }
        public IAop Aop { get { return _Aop; } set { _Aop = value; } }
        public ISqlDialect SqlDialect { get { return _SqlDialect; } }

        /// <summary>
        /// 重写分页方法
        /// 带分页的sql查询 sql语句中包含查询条目数
        /// 例如：SELECT count(*) from t_adminuser;SELECT * from t_adminuser
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override QueryPageResult<T> QueryPage<T>(string sql, object param = null)
        {
            OracleDynamicParameters dyParams = QueryPage(sql, out string newsql, param);
            using (var query = _Connection.QueryMultiple(newsql, dyParams, commandTimeout: _CommandTimeout))
            {
                int total = query.ReadFirstOrDefault<int>();
                var rows = query.Read<T>().ToList();
                return new QueryPageResult<T>() { rows = rows, total = total };
            }
        }

        private OracleDynamicParameters QueryPage(string sql, out string newsql, object param = null)
        {
            newsql = "";
            if (string.IsNullOrWhiteSpace(sql)) return null;
            OracleDynamicParameters dyParams = new OracleDynamicParameters();
            if (param != null)
            {
                DynamicParameters oldParams = (param as DynamicParameters);
                foreach (var item in oldParams?.ParameterNames)
                {
                    object val = oldParams.Get<object>(item);
                    dyParams.Add(item, val);
                }
            }
            string[] array = sql.Split(';');
            if (array.Length < 2) return null;
            StringBuilder builder = new StringBuilder();
            builder.Append("BEGIN")
            .AppendLine(" OPEN :refCursor1 FOR ").Append(array[0]).Append(";")
            .AppendLine(" OPEN :refCursor2 FOR ").Append(array[1]).Append(";")
            .AppendLine("END;");

            dyParams.Add(":refCursor1", OracleDbType.RefCursor, ParameterDirection.Output);
            dyParams.Add(":refCursor2", OracleDbType.RefCursor, ParameterDirection.Output);

            OnExecuting(builder.ToString(), dyParams);
            newsql = builder.ToString();
            return dyParams;
        }

        public override async Task<QueryPageResult<T>> QueryPageAsync<T>(string sql, object param = null)
        {
            OracleDynamicParameters dyParams = QueryPage(sql, out string newsql, param);
            using (var query = await _Connection.QueryMultipleAsync(newsql, dyParams, commandTimeout: _CommandTimeout))
            {
                int total = await query.ReadFirstOrDefaultAsync<int>();
                var result = await query.ReadAsync<T>();
                var rows = result.ToList();
                return new QueryPageResult<T>() { rows = rows, total = total };
            }
        }

        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public override int BulkInsert(DataTable table)
        {
            throw new Exception("please use sqlload.exe tool");
            /*
            using (OracleBulkCopy bulkcopy = new OracleBulkCopy(_Connection as OracleConnection, OracleBulkCopyOptions.Default))
            {
                bulkcopy.DestinationTableName = table.TableName;
                bulkcopy.BulkCopyTimeout = 3600;
                bulkcopy.BatchSize = table.Rows.Count;
                foreach (DataColumn item in table.Columns)
                {
                    bulkcopy.ColumnMappings.Add(item.ColumnName, item.ColumnName);
                }
                bulkcopy.WriteToServer(table);
            }
            if (wasClosed) _Connection.Close();
            return rowCount;*/
        }

        public override Task<int> BulkInsertAsync(DataTable table)
        {
            throw new Exception("please use sqlload.exe tool");
        }

        protected override IDbCommand GetCommand(string sql, SqlMapper.IDynamicParameters param = null, CommandType? commandType = null)
        {
            OracleCommand cmd = new OracleCommand(sql, _Connection as OracleConnection);
            cmd.CommandType = commandType == null ? CommandType.Text : (CommandType)commandType;
            cmd.CommandTimeout = _CommandTimeout;
            if (param != null)
            {
                var sqlParams = (param as OracleDynamicParameters).Get() as List<OracleParameter>;
                cmd.Parameters.AddRange(sqlParams.ToArray());
            }
            return cmd;
        }

        protected override IDataAdapter GetAdapter(IDbCommand command)
        {
            return new OracleDataAdapter(command as OracleCommand);
        }

        public void Dispose()
        {
            _Connection?.Close();
        }
    }
}
