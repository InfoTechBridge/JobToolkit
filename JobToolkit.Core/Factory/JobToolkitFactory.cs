using JobToolkit.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace JobToolkit.Core.Factory
{
    static class JobToolkitFactory
    {
        public static JobManager CreateJobManager(JobManagerConfiguration config)
        {
            //Contract.Requires<ArgumentNullException>(config != null);

            var repositoryName = string.IsNullOrEmpty(config.Repository.Trim()) ? JobToolkitConfiguration.Config.DefaultRepository : config.Repository;
            var repository = CreateJobRepository(JobToolkitConfiguration.Config.RepositoryConfigurations[repositoryName]);

            var managerType = string.IsNullOrEmpty(config.Type.Trim()) ? typeof(JobManager) : GetType(config.Type);
            var ctor = managerType.GetConstructor(new Type[] { typeof(IJobRepository) });
            if (ctor != null)
                return (JobManager)ctor.Invoke(new object[] { repository });

            // Default Constructor
            return (JobManager)System.Activator.CreateInstance(managerType);

        }

        public static IJobRepository CreateJobRepository(RepositoryConfiguration config)
        {
            //Contract.Requires<ArgumentNullException>(config != null);
            
            var formatter = (string.IsNullOrEmpty(config.FormatterType.Trim()) 
                ? new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
                : (IFormatter)System.Activator.CreateInstance(GetType(config.FormatterType)));

            var repositoryType = string.IsNullOrEmpty(config.Type.Trim()) ? typeof(CacheJobRepository) : GetType(config.Type);
            if (repositoryType == null)
                throw new ApplicationException("Invalid repository type.");

            var ctor = repositoryType.GetConstructor(new Type[] { typeof(string), typeof(IFormatter) });
            if (ctor != null)
            {
                var connStr = ConfigurationManager.ConnectionStrings[config.ConnectionString].ConnectionString;
                return (IJobRepository)ctor.Invoke(new object[] { connStr, formatter });
            }

            ctor = repositoryType.GetConstructor(new Type[] { typeof(IFormatter) });
            if (ctor != null)
                return (IJobRepository)ctor.Invoke(new object[] { formatter });
            
            // Default Constructor
            return (IJobRepository)System.Activator.CreateInstance(repositoryType);
            //var baseCtor = baseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null);
        }

        private static Type GetType(string typeName)
        {
            // If typeName is just FullName of the class It dos not returns type of classes that exist in another dll library
            // but if typeName is AssemblyQualifiedName it will be ok.
            // sampel: "System.Runtime.Caching.MemoryCache, System.Runtime.Caching, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null)
                    break;
            }
            return type;
        }
    }
}
