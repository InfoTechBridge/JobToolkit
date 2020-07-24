using ORMToolkit.Core.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core.Standard.Repositories.Database
{
    /// <summary>
    /// SQLIte saves DateTimeOffset as string in database but it couldn't convert it to DateTimeOffset when reading
    /// </summary>
    public class SqlLiteDateTimeOffsetMapper : IMapper
    {
        public object FromDb(object dataValue)
        {
            if (dataValue != null)
            {
                if (dataValue.GetType() == typeof(string))
                {
                    string str = dataValue as string;
                    DateTimeOffset value;
                    if (DateTimeOffset.TryParse(str, out value))
                        return value;
                    //value = (DateTimeOffset)DateTime.SpecifyKind(Convert.ToDateTime(value), DateTimeKind.Utc);
                }

                return dataValue;
            }
            else
                return DBNull.Value;
        }

        public object ToDb(object objectValue)
        {
            return objectValue;
        }
    }
}
