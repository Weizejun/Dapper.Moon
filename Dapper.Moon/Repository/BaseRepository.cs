using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    /// <summary>
    /// 默认数据库通用操作,由Dapper实现
    /// </summary>
    public abstract class BaseRepository
    {
        protected IDbConnection _Connection { get; set; }
        protected IDbTransaction _Transaction { get; set; }
        protected int _CommandTimeout = 3600;
        protected IAop _Aop { get; set; }

        public BaseRepository()
        {
        }

        protected virtual void OnExecuting(string sql, object param)
        {
            if (_Aop != null && _Aop.OnExecuting != null)
            {
                var sqlBuilderResult = new SqlBuilderResult()
                {
                    Sql = sql
                };
                if (param != null && param is DynamicParameters)
                {
                    sqlBuilderResult.DynamicParameters = param as DynamicParameters;
                }
                _Aop.OnExecuting(sqlBuilderResult);
            }
        }

        public virtual List<T> Query<T>(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            return _Connection.Query<T>(sql, param, commandTimeout: _CommandTimeout, commandType: commandType).ToList();
        }

        public virtual async Task<List<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            var result = await _Connection.QueryAsync<T>(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
            return result.ToList();
        }

        public virtual T QueryFirst<T>(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            return _Connection.QueryFirstOrDefault<T>(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
        }

        public virtual async Task<T> QueryFirstAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            var result = await _Connection.QueryFirstOrDefaultAsync<T>(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
            return result;
        }

        public virtual QueryPageResult<T> QueryPage<T>(string sql, object param = null)
        {
            using (var query = QueryMultiple(sql, param))
            {
                int total = query.ReadFirstOrDefault<int>();
                var rows = query.Read<T>().ToList();
                return new QueryPageResult<T>() { rows = rows, total = total };
            }
        }

        public virtual async Task<QueryPageResult<T>> QueryPageAsync<T>(string sql, object param = null)
        {
            OnExecuting(sql, param);
            using (var query = await QueryMultipleAsync(sql, param))
            {
                int total = query.ReadFirstOrDefault<int>();
                var rows = query.Read<T>().ToList();
                return new QueryPageResult<T>() { rows = rows, total = total };
            }
        }

        public virtual Dapper.SqlMapper.GridReader QueryMultiple(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            return _Connection.QueryMultiple(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
        }

        public virtual Task<Dapper.SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            var result = _Connection.QueryMultipleAsync(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
            return result;
        }

        public virtual DataTable Query(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            DataTable table = new DataTable();
            using (var reader = _Connection.ExecuteReader(sql, param, commandTimeout: _CommandTimeout, commandType: commandType))
            {
                table.Load(reader);
                return table;
            }
        }

        public virtual async Task<DataTable> QueryAsync(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            DataTable table = new DataTable();
            using (var reader = await _Connection.ExecuteReaderAsync(sql, param, commandTimeout: _CommandTimeout, commandType: commandType))
            {
                table.Load(reader);
                return table;
            }
        }

        public virtual int Execute(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            return _Connection.Execute(sql, param, commandTimeout: _CommandTimeout, transaction: _Transaction, commandType: commandType);
        }

        public virtual async Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            var result = await _Connection.ExecuteAsync(sql, param, commandTimeout: _CommandTimeout, transaction: _Transaction, commandType: commandType);
            return result;
        }

        public virtual T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            return _Connection.ExecuteScalar<T>(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
        }

        public virtual async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            var result = await _Connection.ExecuteScalarAsync<T>(sql, param, commandTimeout: _CommandTimeout, commandType: commandType);
            return result;
        }

        public DataSet ExecuteDataSet(string sql, SqlMapper.IDynamicParameters param = null, CommandType? commandType = null)
        {
            OnExecuting(sql, param);
            bool wasClosed = _Connection?.State == ConnectionState.Closed;
            if (wasClosed) _Connection?.Open();
            DataSet ds = new DataSet();
            using (IDbCommand cmd = GetCommand(sql, param, commandType))
            {
                IDataAdapter adapter = GetAdapter(cmd);
                adapter.Fill(ds);
                cmd.Parameters?.Clear();
            }
            if (wasClosed) _Connection?.Close();
            return ds;
        }

        protected abstract IDbCommand GetCommand(string sql, SqlMapper.IDynamicParameters param = null, CommandType? commandType = null);
        protected abstract IDataAdapter GetAdapter(IDbCommand command);
        public abstract int BulkInsert(DataTable table);

        public abstract Task<int> BulkInsertAsync(DataTable table);

        #region 事务
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_Connection != null && _Connection.State != ConnectionState.Open)
            {
                _Connection.Open();
            }
            if (_Transaction == null)
            {
                _Transaction = _Connection.BeginTransaction(isolationLevel);
            }
        }

        public void CommitTransaction()
        {
            _Transaction?.Commit();
            _Transaction = null;
        }

        public void RollbackTransaction()
        {
            _Transaction?.Rollback();
            _Transaction = null;
        }

        public RunTransactionResult RunTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            RunTransactionResult runTransactionResult = new RunTransactionResult()
            {
                Success = true
            };
            try
            {
                BeginTransaction(isolationLevel);
                action();
                CommitTransaction();
            }
            catch (Exception ex)
            {
                runTransactionResult.Success = false;
                runTransactionResult.Exception = ex;
                RollbackTransaction();
            }
            return runTransactionResult;
        }
        #endregion 事务
    }
}
