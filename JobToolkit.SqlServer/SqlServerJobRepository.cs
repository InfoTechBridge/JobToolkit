using JobToolkit.DBRepository;
using ORMToolkit.Core;
using System;
using System.Runtime.Serialization;

namespace JobToolkit.SqlServer
{
    public class SqlServerJobRepository : DBJobRepository
    {
        public SqlServerJobRepository(string connectionString)
            : base(new DataManagerNoConfilict(connectionString, new ORMToolkit.Core.Factories.SqlServer.SqlServerFactory()))
        {
        }

        public SqlServerJobRepository(string connectionString, IFormatter formatter)
            : base(new DataManagerNoConfilict(connectionString, new ORMToolkit.Core.Factories.SqlServer.SqlServerFactory()), formatter)
        {
        }
    }
}
