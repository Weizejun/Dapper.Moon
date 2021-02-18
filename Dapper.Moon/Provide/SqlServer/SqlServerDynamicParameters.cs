using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dapper.Moon
{
    public class SqlServerDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters _dynamicParameters = new DynamicParameters();

        private readonly List<SqlParameter> _sqlParameters = new List<SqlParameter>();

        public void Add(string name, object value = null, SqlDbType dbType = SqlDbType.VarChar, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            //_dynamicParameters.Add(name, value, dbType, direction, size);
            var sqlParameter = new SqlParameter(name, value)
            {
                SqlDbType = dbType,
                Direction = direction
            };
            if (size.HasValue)
            {
                sqlParameter.Size = Convert.ToInt32(size);
            }
            _sqlParameters.Add(sqlParameter);
        }

        public void Add(string name, SqlDbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            var sqlParameter = new SqlParameter(name, dbType) { Direction = direction };
            _sqlParameters.Add(sqlParameter);
        }

        public void Add(string name, SqlDbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            var sqlParameter = new SqlParameter(name, dbType, size) { Direction = direction };
            _sqlParameters.Add(sqlParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)_dynamicParameters).AddParameters(command, identity);

            var sqlCommand = command as SqlCommand;

            if (sqlCommand != null)
            {
                sqlCommand.Parameters.AddRange(_sqlParameters.ToArray());
            }
        }

        public object Get(string parameterName)
        {
            var parameter = _sqlParameters.SingleOrDefault(t => t.ParameterName == parameterName);
            if (parameter != null)
                return parameter.Value;
            return null;
        }

        public List<SqlParameter> Get()
        {
            return _sqlParameters;
        }

        public object Get(int index)
        {
            var parameter = _sqlParameters[index];
            if (parameter != null)
                return parameter.Value;
            return null;
        }

        public void Clear()
        {
            _sqlParameters.Clear();
        }
    }
}