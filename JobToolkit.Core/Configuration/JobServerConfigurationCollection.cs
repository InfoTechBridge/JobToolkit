using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JobToolkit.Core.Configuration
{
    [ConfigurationCollection(typeof(JobServerConfiguration))]
    public class JobServerConfigurationCollection : ConfigurationElementCollection
    {
        //internal const string PropertyName = "connection";

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }
        //protected override string ElementName
        //{
        //    get
        //    {
        //        return PropertyName;
        //    }
        //}

        //protected override bool IsElementName(string elementName)
        //{
        //    return elementName.Equals(PropertyName, StringComparison.InvariantCultureIgnoreCase);
        //}

        public override bool IsReadOnly()
        {
            return false;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new JobServerConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((JobServerConfiguration)(element)).Name;
        }

        public JobServerConfiguration this[int index]
        {
            get
            {
                return BaseGet(index) as JobServerConfiguration;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new JobServerConfiguration this[string name]
        {
            get { return (JobServerConfiguration)BaseGet(name); }
            set
            {
                if (BaseGet(name) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(name)));
                }
                BaseAdd(value);
            }
        }

        public int IndexOf(JobServerConfiguration element)
        {
            return BaseIndexOf(element);
        }

        public void Add(JobServerConfiguration element)
        {
            BaseAdd(element);

            // Your custom code goes here.

        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);

            // Your custom code goes here.

        }

        public void Remove(JobServerConfiguration element)
        {
            if (BaseIndexOf(element) >= 0)
            {
                BaseRemove(element.Name);
                // Your custom code goes here.
                Console.WriteLine("JobServerConfigurationCollection: {0}", "Removed collection element!");
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);

            // Your custom code goes here.

        }

        public void Remove(string name)
        {
            BaseRemove(name);

            // Your custom code goes here.

        }
        public void Clear()
        {
            BaseClear();

            // Your custom code goes here.
            Console.WriteLine("JobServerConfigurationCollection: {0}", "Removed entire collection!");
        }

    }
}
