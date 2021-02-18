using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Dapper.Moon
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters _dynamicParameters = new DynamicParameters();

        private readonly List<OracleParameter> _sqlParameters = new List<OracleParameter>();

        public void Add(string name, object value = null, OracleDbType dbType = OracleDbType.Varchar2, ParameterDirection direction = ParameterDirection.Input, int? size = null)
        {
            //_dynamicParameters.Add(name, value, dbType, direction, size);
            var oracleParameter = new OracleParameter(name, value)
            {
                OracleDbType = dbType,
                Direction = direction
            };
            if (size.HasValue)
            {
                oracleParameter.Size = Convert.ToInt32(size);
            }
            _sqlParameters.Add(oracleParameter);
        }

        public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction = ParameterDirection.Input)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType) { Direction = direction };
            _sqlParameters.Add(oracleParameter);
        }

        public void Add(string name, OracleDbType oracleDbType, int size, ParameterDirection direction = ParameterDirection.Input)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType, size) { Direction = direction };
            _sqlParameters.Add(oracleParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)_dynamicParameters).AddParameters(command, identity);

            var oracleCommand = command as OracleCommand;

            if (oracleCommand != null)
            {
                oracleCommand.Parameters.AddRange(_sqlParameters.ToArray());
            }
        }

        public object Get(string parameterName)
        {
            var parameter = _sqlParameters.SingleOrDefault(t => t.ParameterName == parameterName);
            if (parameter != null)
                return parameter.Value;
            return null;
        }

        public List<OracleParameter> Get()
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