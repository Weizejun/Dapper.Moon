using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dapper.Moon
{
    public abstract class DeleteableProvide<T> : IDeleteable<T>
    {
        protected IRepository Repository;
        protected ExpressionProvide ExpressionProvideObj;
        protected SqlBuilderResult _Where;
        protected T SaveObject;
        protected MapperTable MasterTable;
        protected string MasterTableName;
        protected object[] InParams;

        public DeleteableProvide(IRepository _Repository)
        {
            Repository = _Repository;

            MasterTable = ClassMapper.Mapping<T>();
            MasterTableName = MasterTable.TableName;

            ExpressionProvideObj = ExpressionProvide.Create(Repository.SqlDialect, MasterTable);
        }

        public IDeleteable<T> Deleteable()
        {
            return this;
        }

        public IDeleteable<T> Deleteable(object[] pks)
        {
            InParams = pks;
            return this;
        }

        public IDeleteable<T> TableName(string tableName)
        {
            if (tableName == null || tableName == "") throw new Exception("wrong table name");
            MasterTableName = tableName;
            return this;
        }

        public IDeleteable<T> Where(Expression<Func<T, bool>> where)
        {
            _Where = ExpressionProvideObj.ExpressionRouter(where);
            return this;
        }

        public int Execute()
        {
            SqlBuilderResult result = ToSql();
            return Repository.Execute(result.Sql, result.DynamicParameters);
        }

        public async Task<int> ExecuteAsync()
        {
            SqlBuilderResult result = ToSql();
            return await Repository.ExecuteAsync(result.Sql, result.DynamicParameters);
        }

        public virtual SqlBuilderResult ToSql()
        {
            if (_Where == null && InParams == null)
            {
                throw new Exception("condition error");
            }
            DynamicParameters dynamicParameters = new DynamicParameters();
            string whereSql = "";
            if (_Where != null)
            {
                whereSql = _Where.Sql;
            }
            else
            {
                if (InParams != null && InParams.Length > 0)
                {
                    //多个主键无法删除
                    var array = MasterTable.Properties.Where(i => i.IsPrimaryKey);
                    if (array != null && array.Count() == 1)
                    {
                        PropertyMap propertyMap = array.FirstOrDefault();
                        whereSql = Repository.SqlDialect.SetSqlName(propertyMap.ColumnName) + " in ({0})";
                        string parameterSql = string.Join(",", InParams.Select(i => $"'{i}'"));
                        whereSql = string.Format(whereSql, parameterSql);
                    }
                    else
                    {
                        throw new Exception("primary key is empty or has more than one primary key");
                    }
                }
            }
            if (_Where != null)
            {
                dynamicParameters.AddDynamicParams(_Where.DynamicParameters);
            }
            return new SqlBuilderResult()
            {
                Sql = string.Format("delete from {0} where {1}", Repository.SqlDialect.SetSqlName(MasterTableName), whereSql),
                DynamicParameters = dynamicParameters
            };
        }
    }
}
