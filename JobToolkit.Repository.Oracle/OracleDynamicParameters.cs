using Dapper;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Repository.Oracle
{
    public class OracleDynamicParameters : SqlMapper.IDynamicParameters
    {
        private readonly DynamicParameters dynamicParameters = new DynamicParameters();

        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

        public void Add(string name, object value = null, DbType? dbType = null, ParameterDirection? direction = null, int? size = null)
        {
            dynamicParameters.Add(name, value, dbType, direction, size);
        }

        //public void Add(string name, object value, OracleDbType oracleDbType, ParameterDirection direction = ParameterDirection.Input)
        //{
        //    var oracleParameter = new OracleParameter(name, oracleDbType, value, direction);
        //    oracleParameters.Add(oracleParameter);
        //}
        public void Add(string name, OracleDbType oracleDbType, object value, ParameterDirection direction = ParameterDirection.Input)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType, value, direction);
            oracleParameters.Add(oracleParameter);
        }

        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

            var oracleCommand = command as OracleCommand;

            if (oracleCommand != null)
            {
                oracleCommand.BindByName = true;
                oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
            }
        }
    }
}
