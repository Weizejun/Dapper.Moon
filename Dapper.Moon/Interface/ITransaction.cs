using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Dapper.Moon
{
    public interface ITransaction
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void RollbackTransaction();
        RunTransactionResult RunTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
    }
}
