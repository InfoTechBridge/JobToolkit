using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JobToolkit.Core.Configuration
{
    /// <summary>
    /// Use the following web.config file.
    ///<?xml version="1.0" encoding="utf-8" ?>
    ///<configuration>
    ///  <configSections>
    ///    <section name="jobToolkit" type="JobToolkit.Core.Configuration.JobToolkitConfiguration, JobToolkit.Core" />
    ///  </configSections>
    ///  <jobToolkit defaultManager="DefaultManager" defaultServer="DefaultServer" defaultRepository="DefaultRepository" >
    ///     <managers>
    ///         <add name = "DefaultManager" [String] 
    ///          type = "" [String] 
    ///          repository = "" [String] />
    ///     </managers>
    ///     <servers>
    ///         <add name = "DefaultServer" [String] 
    ///          type = "" [String] 
    ///          repository = "" [String] />
    ///     </servers>
    ///     <repositories>
    ///         <add name = "DefaultRepository" [String] 
    ///          type = "" [String] 
    ///          connectionString = "" [String] />
    ///     </repositories>
    ///  </jobToolkit>
    ///</configuration>
    /// </summary>
    public class JobToolkitConfiguration : ConfigurationSection
    {
        public JobToolkitConfiguration()
        {
            JobManagerConfigurations.Add(new JobManagerConfiguration());
            JobServerConfigurations.Add(new JobServerConfiguration());
            RepositoryConfigurations.Add(new RepositoryConfiguration());
        }

        public static JobToolkitConfiguration Config
        {
            get
            {
                return (JobToolkitConfiguration)ConfigurationManager.GetSection("jobToolkit") ?? CreateDefaultSection();
            }
        }
        
        [ConfigurationProperty("defaultManager", DefaultValue = "DefaultManager", IsRequired = true)]
        public string DefaultManager
        {
            get
            {
                return (string)this["defaultManager"];
            }
            set
            {
                this["defaultManager"] = value;
            }
        }

        [ConfigurationProperty("defaultServer", DefaultValue = "DefaultServer", IsRequired = true)]
        public string DefaultServer
        {
            get
            {
                return (string)this["defaultServer"];
            }
            set
            {
                this["defaultServer"] = value;
            }
        }

        [ConfigurationProperty("defaultRepository", DefaultValue = "DefaultRepository", IsRequired = true)]
        public string DefaultRepository
        {
            get
            {
                return (string)this["defaultRepository"];
            }
            set
            {
                this["defaultRepository"] = value;
            }
        }              

        [ConfigurationProperty("managers", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RepositoryConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public JobManagerConfigurationCollection JobManagerConfigurations
        {
            get { return ((JobManagerConfigurationCollection)(base["managers"])); }
            set { base["managers"] = value; }
        }

        [ConfigurationProperty("servers", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RepositoryConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public JobServerConfigurationCollection JobServerConfigurations
        {
            get { return ((JobServerConfigurationCollection)(base["servers"])); }
            set { base["servers"] = value; }
        }

        [ConfigurationProperty("repositories", IsRequired = true, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(RepositoryConfigurationCollection), AddItemName = "add", ClearItemsName = "clear", RemoveItemName = "remove")]
        public RepositoryConfigurationCollection RepositoryConfigurations
        {
            get { return ((RepositoryConfigurationCollection)(base["repositories"])); }
            set { base["repositories"] = value; }
        }

        private static JobToolkitConfiguration CreateDefaultSection()
        {
            JobToolkitConfiguration defaultSection = new JobToolkitConfiguration();

            defaultSection.JobManagerConfigurations = new JobManagerConfigurationCollection();
            defaultSection.JobManagerConfigurations.Add(new JobManagerConfiguration());

            defaultSection.JobServerConfigurations = new JobServerConfigurationCollection();
            defaultSection.JobServerConfigurations.Add(new JobServerConfiguration());

            defaultSection.RepositoryConfigurations = new RepositoryConfigurationCollection();
            defaultSection.RepositoryConfigurations.Add(new RepositoryConfiguration());

            return defaultSection;
        }
    }
}
