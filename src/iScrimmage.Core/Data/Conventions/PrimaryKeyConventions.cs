using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using iScrimmage.Core.Data.Mapping;

namespace iScrimmage.Core.Data.Conventions
{
    public interface IPrimaryKeyConvention
    {
        String DefaultKeyName { get; }
        PropertyMap GetKeyField(Type type);
    }

    public class DefaultPrimaryKeyConvention: IPrimaryKeyConvention
    {
        private IEnumerable<PropertyInfo> _allProperties;

        public DefaultPrimaryKeyConvention(string keyName = "id")
        {
            DefaultKeyName = keyName;
        }

        public string DefaultKeyName { get; set; }

        public PropertyMap GetKeyField(Type type)
        {
            _allProperties = type.GetProperties();
            var keyProperties = _allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            if (keyProperties.Count > 1)
            {
                throw new ApplicationException("Primary Key Convention does not support multi part keys");
            }

            var keyProperty = keyProperties.FirstOrDefault();
            return new PropertyMap(keyProperty ?? DefaultProperty());;
        }

        private PropertyInfo DefaultProperty()
        {
            return _allProperties.FirstOrDefault(p => p.Name.ToLower() == DefaultKeyName.ToLower());
        }
    }
}
