using JobToolkit.DBRepository;
using ORMToolkit.Core;
using System;
using System.Runtime.Serialization;

namespace JobToolkit.Oracle
{
    public class OracleJobRepository : DBJobRepository
    {
        public OracleJobRepository(string connectionString)
            : base(new DataManagerNoConfilict(connectionString, new ORMToolkit.OracleOdpManaged.OracleOdpManagedDataProviderFactory()))
        {
            OrmToolkitSettings.ObjectFactory = new ORMToolkit.Core.Factories.ObjectFactory2();
        }

        public OracleJobRepository(string connectionString, IFormatter formatter)
            : base(new DataManagerNoConfilict(connectionString, new ORMToolkit.OracleOdpManaged.OracleOdpManagedDataProviderFactory()), formatter)
        {
            OrmToolkitSettings.ObjectFactory = new ORMToolkit.Core.Factories.ObjectFactory2();
        }
    }
}
