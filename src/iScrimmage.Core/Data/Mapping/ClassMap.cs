using iScrimmage.Core.Data.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iScrimmage.Core.Data.Exceptions;
using iScrimmage.Core.Extensions;
using Dapper;

namespace iScrimmage.Core.Data.Mapping
{
    public class ClassMap
    {
        public ClassMap(Type t)
        {
            EntityType = t;
            References = new List<ReferenceMap>();
        }

        public String Table { get; protected set; }

        public KeyMap Id { get; set; }

        public IDictionary<String,PropertyMap> Properties { get; set; }

        public Type EntityType { get; protected set; }

        public List<ReferenceMap> References { get; set; }

        public IList<PropertyMap> AuditableProperties
        {
            get
            {
                return Properties.Select(p => p.Value).Where(p => p.IsAuditable).ToList();
            }
        }

        public ClassMap SetTable(string name)
        {
            Table = name;
            return this;
        }

        public PropertyMap Ignore(string property)
        {
            if (!Properties.ContainsKey(property))
            {
                throw new MappingException(property, "Property can not be ignored because it does not exist in property collection");
            }

            var prop = Properties[property];

            if (prop != null)
            {
                prop.IsIgnored = true;
            }

            return prop;
        }

        
        /// <summary>
        /// Returns an existing property map or creates one if it does not exist.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <remarks>Will throw an exception if the property name does not exists on this entity</remarks>
        public PropertyMap Map(string name)
        {
            if(Properties.ContainsKey(name))
            {
                return Properties[name];
            }

            return Map(name, name);
        }

        /// <summary>
        /// Updates or creates all properties of a PropertyMap
        /// </summary>
        /// <param name="name"></param>
        /// <param name="column"></param>
        /// <param name="ignore"></param>
        /// <param name="nullable"></param>
        /// <returns></returns>
        /// <remarks>Will throw an exception if the property name does not exists on this entity</remarks>
        public PropertyMap Map(string name, string column, bool ignore = false, bool? nullable = null)
        {
            var propInfo = EntityType.GetProperty(name);

            if (propInfo == null)
            {
                throw new MappingException(name, "Property does not exist on class being mapped");
            }

            var map = Properties.ContainsKey(name) ? Properties[name] : new PropertyMap();

            map.ColumnName = column ?? name;
            map.Name = name;
            map.Nullable = nullable != null ? nullable.Value : map.Nullable;
            map.IsIgnored = ignore;
            map.Property = propInfo;

            return map;
        }

        /// <summary>
        /// Generates and assigns values for the Key property 
        /// </summary>
        /// <param name="data"></param>
        public ClassMap AssignIdentifierValues(dynamic data)
        {
            Id.AssignIdentifier(data);

            return this;
        }
    }
}
