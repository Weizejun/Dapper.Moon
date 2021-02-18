using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Dapper.Moon
{
    public class MySqlDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters _dynamicParameters = new DynamicParameters();

        private readonly List<MySqlParameter> _sqlParameters = new List<MySqlParameter>();

        public void Add(string name, object value = null, MySqlDbType dbType = MySqlDbType.VarChar, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            //_dynamicParameters.Add(name, value, dbType, direction, size);
            var sqlParameter = new MySqlParameter(name, value)
            {
                MySqlDbType = dbType,
                Direction = direction
            };
            if (size.HasValue)
            {
                sqlParameter.Size = Convert.ToInt32(size);
            }
            _sqlParameters.Add(sqlParameter);
        }

        public void Add(string name, MySqlDbType dbType, ParameterDirection direction = ParameterDirection.Input)
        {
            var sqlParameter = new MySqlParameter(name, dbType) { Direction = direction };
            _sqlParameters.Add(sqlParameter);
        }

        public void Add(string name, MySqlDbType dbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            var sqlParameter = new MySqlParameter(name, dbType, size) { Direction = direction };
            _sqlParameters.Add(sqlParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)_dynamicParameters).AddParameters(command, identity);

            var sqlCommand = command as MySqlCommand;

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

        public List<MySqlParameter> Get()
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