using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Dapper.Moon
{
    public partial class DapperMoon : IDisposable
    {
        private IRepository _Repository = null;
        private string _connectionString = null;
        private DbType _dbType = DbType.SqlServer;

        private DapperMoon(string connectionString, DbType dbType = DbType.SqlServer)
        {
            this._connectionString = connectionString;
            this._dbType = dbType;
        }

        public static DapperMoon Create(string connectionString, DbType dbType = DbType.SqlServer)
        {
            //运行环境检测
            //EnvironmentManagement.DapperRuntime();
            //EnvironmentManagement.JsonRuntime();
            switch (dbType)
            {
                case DbType.MySql:
                    //EnvironmentManagement.MySqlRuntime();
                    break;
                case DbType.Oracle:
                    //EnvironmentManagement.OracleRuntime();
                    break;
            }
            return new DapperMoon(connectionString, dbType);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        public Action<SqlBuilderResult> OnExecuting { get; set; }

        /// <summary>
        /// 数据库通用操作方法，由Dapper实现
        /// </summary>
        public IRepository Repository
        {
            get
            {
                if (_Repository == null)
                {
                    _Repository = InstanceFactory.CreateInstance<IRepository>(_dbType.ToString() + "Repository", new object[] { _connectionString });
                    _Repository.Aop = new AopProvider();
                    _Repository.Aop.OnExecuting = OnExecuting;
                }
                return _Repository;
            }
        }

        #region Transaction
        /// <summary>
        /// 提供给跨库事务的方法 可直接用 RunTransaction
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            Repository.BeginTransaction(isolationLevel);
        }
        /// <summary>
        /// 提供给跨库事务的方法 可直接用 RunTransaction
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void CommitTransaction()
        {
            Repository.CommitTransaction();
        }
        /// <summary>
        /// 提供给跨库事务的方法 可直接用 RunTransaction
        /// </summary>
        /// <param name="isolationLevel"></param>
        public void RollbackTransaction()
        {
            Repository.RollbackTransaction();
        }
        /// <summary>
        /// 事务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public RunTransactionResult RunTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            return Repository.RunTransaction(action, isolationLevel);
        }
        #endregion Transaction

        private IInsertable<T> _Insertable<T>()
        {
            IInsertable<T> insertable = InstanceFactory.CreateInsertable<T>(_dbType, Repository);
            return insertable;
        }

        public IInsertable<T> Insertable<T>(T t)
        {
            IInsertable<T> insertable = _Insertable<T>();
            return insertable.Insertable(t);
        }

        public IInsertable<T> Insertable<T>(List<T> list)
        {
            IInsertable<T> insertable = _Insertable<T>();
            return insertable.Insertable(list);
        }

        private IUpdateable<T> _Updateable<T>()
        {
            IUpdateable<T> updateable = InstanceFactory.CreateUpdateable<T>(_dbType, Repository);
            return updateable;
        }

        public IUpdateable<T> Updateable<T>(T t)
        {
            IUpdateable<T> updateable = _Updateable<T>();
            return updateable.Updateable(t);
        }

        public IUpdateable<T> Updateable<T>()
        {
            IUpdateable<T> updateable = _Updateable<T>();
            return updateable.Updateable();
        }

        private IDeleteable<T> _Deleteable<T>()
        {
            IDeleteable<T> deleteable = InstanceFactory.CreateDeleteable<T>(_dbType, Repository);
            return deleteable;
        }

        public IDeleteable<T> Deleteable<T>()
        {
            IDeleteable<T> deleteable = _Deleteable<T>();
            return deleteable.Deleteable();
        }

        public IDeleteable<T> Deleteable<T>(object[] pks)
        {
            IDeleteable<T> deleteable = _Deleteable<T>();
            return deleteable.Deleteable(pks);
        }

        public IQueryable<T> Queryable<T>()
        {
            IQueryable<T> queryable = InstanceFactory.CreateQueryable<T>(_dbType, Repository);
            return queryable;
        }

        /// <summary>
        /// 多表连接，表1 别名 t1,表2 别名 t2
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        public IQueryable<T, T2> Queryable<T, T2>()
        {
            IQueryable<T, T2> queryable = InstanceFactory.CreateQueryable<T, T2>(_dbType, Repository);
            return queryable;
        }

        public IQueryable<T, T2, T3> Queryable<T, T2, T3>()
        {
            IQueryable<T, T2, T3> queryable = InstanceFactory.CreateQueryable<T, T2, T3>(_dbType, Repository);
            return queryable;
        }

        public IQueryable<T, T2, T3, T4> Queryable<T, T2, T3, T4>()
        {
            IQueryable<T, T2, T3, T4> queryable = InstanceFactory.CreateQueryable<T, T2, T3, T4>(_dbType, Repository);
            return queryable;
        }

        public IQueryable<T, T2, T3, T4, T5> Queryable<T, T2, T3, T4, T5>()
        {
            IQueryable<T, T2, T3, T4, T5> queryable = InstanceFactory.CreateQueryable<T, T2, T3, T4, T5>(_dbType, Repository);
            return queryable;
        }

        public IQueryable<T, T2, T3, T4, T5, T6> Queryable<T, T2, T3, T4, T5, T6>()
        {
            IQueryable<T, T2, T3, T4, T5, T6> queryable = InstanceFactory.CreateQueryable<T, T2, T3, T4, T5, T6>(_dbType, Repository);
            return queryable;
        }

        public IDbFirst DbFirst()
        {
            var dbFirst = InstanceFactory.CreateInstance<IDbFirst>(_dbType.ToString() + "DbFirst", new object[] { Repository });
            return dbFirst;
        }

        public void Dispose()
        {
            _Repository?.Dispose();
        }
    }
}
