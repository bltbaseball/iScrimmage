using iScrimmage.Core.Common;
using iScrimmage.Core.Data.Conventions;
using iScrimmage.Core.Data.Generators;
using System;
using System.Collections.Concurrent;
using System.Linq;
using iScrimmage.Core.Data.Mapping.Attributes;
using iScrimmage.Core.Models;
using Microsoft.Practices.ServiceLocation;
using log4net;

namespace iScrimmage.Core.Data.Mapping
{
    public class ClassMapper
    {
       

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, ClassMap> ClassMaps = new ConcurrentDictionary<RuntimeTypeHandle, ClassMap>();

        public static ClassMap GetMap(Type type)
        {
            if (ClassMaps.ContainsKey(type.TypeHandle))
            {
                return ClassMaps[type.TypeHandle];
            }

            var builder = new ClassMapBuilder(type);
            var mapping = builder.BuildMap();

            ClassMaps[type.TypeHandle] = mapping;
            return mapping;
        }

        public static void SetMap(Type type, ClassMap map)
        {
            ClassMaps[type.TypeHandle] = map;
        }

        class ClassMapBuilder
        {
            private readonly ILog _logger = LogManager.GetLogger(typeof(ClassMapper));

            private readonly ClassMap _mapping;
            private readonly bool _isSubclass;
            private readonly Type _baseType;

            private readonly IPropertyConvention _propertyConvention;
            private readonly IPrimaryKeyConvention _primaryKeyConvention;
            private readonly ITableNamingConvention _tableNamingConvention;

            public ClassMapBuilder(Type type)
            {
                _baseType = type.BaseType;

                //Note: these can be eventually read from some sort configuration
                _propertyConvention = new DefaultPropertyConvention();
                _primaryKeyConvention = new DefaultPrimaryKeyConvention("Guid");
                _tableNamingConvention = new TableAttributeNamingConvention();
                
                if (_baseType != null && _baseType != typeof(object) && !_baseType.IsAbstract)
                {
                    _isSubclass = true;
                    _mapping = new SubClassMap(type, _baseType);
                }
                else
                {
                    _baseType = type;
                    _mapping = new ClassMap(type);    
                }
            }

            public ClassMap BuildMap()
            {
                TableName();
                Id();
                Properties();
                SubClass();
                ApplyOverrides();

                return _mapping;
            }

            private void ApplyOverrides()
            {
                var overrideType = typeof(IMappingOverride<>).MakeGenericType(_baseType);

                try
                {
                    dynamic mappingOverride = ServiceLocator.Current.GetInstance(overrideType);

                    if (mappingOverride != null)
                    {
                        mappingOverride.Override(_mapping);
                    }
                }
                catch (Exception e)
                {
                    _logger.Warn("No mapping override for " + _baseType, e);
                }
            }

            private void TableName()
            {
                var tableName = _tableNamingConvention.GetTableName(_baseType);
                _mapping.SetTable (tableName);
            } 

            private void Id()
            {
                var key = _primaryKeyConvention.GetKeyField(_baseType);

                _mapping.Id = new KeyMap()
                                  {
                                      ColumnName = key.ColumnName,
                                      Generator = new GuidCombGenerator(),
                                      Name = key.Name,
                                      Property = key.Property,
                                      UnsavedValue = Guid.Empty
                                  };

            }

            private void Properties()
            {
                var allProperties = _propertyConvention.GetFields(_mapping.EntityType).ToList();
                _mapping.Properties = allProperties.Where(p => p.Property != _mapping.Id.Property).ToDictionary(k => k.Name, v => v);
            }

            private void SubClass()
            {
                if (!_isSubclass) return;

                JoinPart join;
                var discriminator = _baseType.Name;
                var subClassMap = _mapping as SubClassMap;
                var subClassAttribute = _mapping.EntityType.GetCustomAttributes(false).FirstOrDefault(a => a is SubClassAttribute) as SubClassAttribute;

                if (subClassAttribute != null)
                {
                    discriminator = subClassAttribute.Discriminator ?? discriminator;
                    join = new JoinPart()
                                           {
                                               KeyColumn = subClassAttribute.KeyColumn,
                                               TableName = subClassAttribute.Table
                                           };
                }
                else
                {
                    join = new JoinPart()
                               {
                                   KeyColumn = _mapping.Id.ColumnName, //Assume join column is child table has same name as PK in parent table if not join attribute is specified
                                   TableName = subClassMap.Table
                               };
                }

                subClassMap.Discriminator = discriminator;
                subClassMap.Join = join;
            }
        }

       
    }
}
