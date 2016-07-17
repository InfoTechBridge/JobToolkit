using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JobToolkit.Core.Configuration
{
    public class RepositoryConfiguration : ConfigurationElement
    {
        public RepositoryConfiguration(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public RepositoryConfiguration()
        {

        }

        [ConfigurationProperty("name", DefaultValue = "DefaultRepository", IsKey = true, IsRequired = true)]
        [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = "JobToolkit.Core.CacheJobRepository", IsKey = false, IsRequired = false)]
        public string Type
        {
            get { return (string)base["type"]; }
            set { base["type"] = value; }
        }

        [ConfigurationProperty("formatterType", DefaultValue = "System.Runtime.Serialization.Formatters.Binary.BinaryFormatter", IsKey = false, IsRequired = false)]
        public string FormatterType
        {
            get { return (string)base["formatterType"]; }
            set { base["formatterType"] = value; }
        }

        [ConfigurationProperty("connectionString", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ConnectionString
        {
            get { return (string)base["connectionString"]; }
            set { base["connectionString"] = value; }
        }
                
    }
}
