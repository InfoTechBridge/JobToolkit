using JobToolkit.DBRepository;
using ORMToolkit.Core;
using System;
using System.Runtime.Serialization;

namespace JobToolkit.SQLite
{
    public class SQLiteJobRepository : DBJobRepository
    {
        public SQLiteJobRepository(string connectionString) 
            : base(new DataManagerNoConfilict(connectionString, new ORMToolkit.SQLite.SQLiteDataProviderFactory()))
        {
            OrmToolkitSettings.ObjectFactory = new ORMToolkit.Core.Factories.ObjectFactory2();
        }

        public SQLiteJobRepository(string connectionString, IFormatter formatter) 
            : base(new DataManagerNoConfilict(connectionString, new ORMToolkit.SQLite.SQLiteDataProviderFactory()), formatter)
        {
            OrmToolkitSettings.ObjectFactory = new ORMToolkit.Core.Factories.ObjectFactory2();
        }
    }
}
