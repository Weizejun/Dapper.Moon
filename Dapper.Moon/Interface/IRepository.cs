using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    /// <summary>
    /// 数据库通用操作方法，由Dapper实现
    /// </summary>
    public interface IRepository : ITransaction, IDisposable
    {
        /// <summary>
        /// 获取当前连接
        /// </summary>
        IDbConnection Connection { get; }
        /// <summary>
        /// 数据库方言
        /// </summary>
        ISqlDialect SqlDialect { get; }
        /// <summary>
        /// 查询结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        List<T> Query<T>(string sql, object param = null, CommandType? commandType = null);
        Task<List<T>> QueryAsync<T>(string sql, object param = null, CommandType? commandType = null);
        /// <summary>
        /// 获取单条结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        T QueryFirst<T>(string sql, object param = null, CommandType? commandType = null);
        Task<T> QueryFirstAsync<T>(string sql, object param = null, CommandType? commandType = null);
        /// <summary>
        /// 单行单列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        T ExecuteScalar<T>(string sql, object param = null, CommandType? commandType = null);
        Task<T> ExecuteScalarAsync<T>(string sql, object param = null, CommandType? commandType = null);
        /// <summary>
        /// 增、删、改sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        int Execute(string sql, object param = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, object param = null, CommandType? commandType = null);
        /// <summary>
        /// 带分页的sql查询 sql语句中包含查询条目数
        /// 例如：SELECT count(*) from t_adminuser;SELECT * from t_adminuser
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        QueryPageResult<T> QueryPage<T>(string sql, object param = null);
        Task<QueryPageResult<T>> QueryPageAsync<T>(string sql, object param = null);
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        DataTable Query(string sql, object param = null, CommandType? commandType = null);
        Task<DataTable> QueryAsync(string sql, object param = null, CommandType? commandType = null);
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        int BulkInsert(DataTable table);
        Task<int> BulkInsertAsync(DataTable table);
        /// <summary>
        /// 返回结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param">SqlServerParameters、MySqlParameters、OracleDynamicParameters</param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string sql, SqlMapper.IDynamicParameters param = null, CommandType? commandType = null);
        /// <summary>
        /// 多条SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        Dapper.SqlMapper.GridReader QueryMultiple(string sql, object param = null, CommandType? commandType = null);
        Task<Dapper.SqlMapper.GridReader> QueryMultipleAsync(string sql, object param = null, CommandType? commandType = null);
        /// <summary>
        /// sql语句执行拦截
        /// </summary>
        IAop Aop { get; set; }
    }
}