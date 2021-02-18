using System;
using System.Collections.Generic;
using System.Data;

namespace Dapper.Moon
{
    /// <summary>
    /// 跨库事务
    /// </summary>
    public class TransactionFactory : ITransaction, IDisposable
    {
        private DapperMoon[] _dbContexts = null;

        private TransactionFactory(params DapperMoon[] dbContexts)
        {
            _dbContexts = dbContexts;
        }

        public static TransactionFactory Create(params DapperMoon[] dbContexts)
        {
            if (dbContexts == null || dbContexts.Length == 0) throw new Exception("the parameter is empty");
            return new TransactionFactory(dbContexts);
        }

        /// <summary>
        /// 通过下标访问当前数据库
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DapperMoon ChangeDatabase(int index)
        {
            if (index + 1 > _dbContexts.Length)
            {
                throw new Exception("array out of bounds");
            }
            return _dbContexts[index];
        }

        private void ForEach<T>(IEnumerable<T> iEnumberable, Action<T> func)
        {
            foreach (var item in iEnumberable)
            {
                func(item);
            }
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            ForEach<DapperMoon>(_dbContexts, i =>
            {
                i.BeginTransaction(isolationLevel);
            });
        }

        public void CommitTransaction()
        {
            ForEach<DapperMoon>(_dbContexts, i =>
            {
                i.CommitTransaction();
            });
        }

        public void RollbackTransaction()
        {
            ForEach<DapperMoon>(_dbContexts, i =>
            {
                i.RollbackTransaction();
            });
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

        public void Dispose()
        {
            ForEach<DapperMoon>(_dbContexts, i =>
            {
                i.Dispose();
            });
            _dbContexts = null;
        }
    }
}
