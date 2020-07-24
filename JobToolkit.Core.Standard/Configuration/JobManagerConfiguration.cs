using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JobToolkit.Core.Configuration
{
    public class JobManagerConfiguration : ConfigurationElement
    {
        public JobManagerConfiguration(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public JobManagerConfiguration()
        {

        }

        [ConfigurationProperty("name", DefaultValue = "DefaultManager", IsKey = true, IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = "JobToolkit.Core.JobManager", IsKey = false, IsRequired = false)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("repository", DefaultValue = "DefaultRepository", IsKey = false, IsRequired = false)]
        public string Repository
        {
            get { return (string)base["repository"]; }
            set { base["repository"] = value; }
        }
    }
}
