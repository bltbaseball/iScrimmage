using System;
using System.Collections.Generic;
using System.Linq;
using iScrimmage.Core.Data.Mapping;
using iScrimmage.Core.Data.Configuration;

namespace iScrimmage.Core.Data.Conventions
{
    public interface IPropertyConvention
    {
        IEnumerable<PropertyMap> GetFields(Type type);
    }

    public class DefaultPropertyConvention : IPropertyConvention
    {
        public IEnumerable<PropertyMap> GetFields(Type type)
        {
            var mappedProperties = new List<PropertyMap>();

            var allProperties = type.GetProperties().Where(t => DataConfiguration.IsBasicType(t.PropertyType));

            foreach (var prop in allProperties)
            {
                mappedProperties.Add(new PropertyMap(prop));
            }

            return mappedProperties;
        }
    }

}
