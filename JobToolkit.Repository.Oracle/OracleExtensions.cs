using Oracle.DataAccess.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Repository.Oracle
{
    internal static class OracleExtensions
    {
        public static OracleTimeStampTZ ToOracleTimeStampTZ(this DateTimeOffset value)
        {
            return new OracleTimeStampTZ(value.DateTime);
        }

        public static OracleTimeStampTZ? ToOracleTimeStampTZ(this DateTimeOffset? value)
        {
            return value.HasValue ? new OracleTimeStampTZ(value.GetValueOrDefault().DateTime) : (OracleTimeStampTZ?)null;
        }

        
        //public static implicit operator OracleTimeStampTZ(this DateTimeOffset x)
        //{
        //    return x.ToOracleTimeStampTZ();
        //}
    }
}
