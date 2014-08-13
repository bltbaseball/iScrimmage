using System.Reflection;
using iScrimmage.Core.Data.Criterion;
using iScrimmage.Core.Data.Extensions;
using iScrimmage.Core.Data.Mapping;
using iScrimmage.Core.Extensions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iScrimmage.Core.Data
{
    public class TableAlias
    {
        public Type EntityType { get; set; }
        public String Alias { get; set; }
        public String Table { get; set; }
    }

    public class TableAliasMap: Dictionary<Type, TableAlias>
    {
        public TableAliasMap AddAlias(Type type, string table, string alias)
        {
            var a = new TableAlias() {Alias = alias, EntityType = type, Table = table};
            this.Add(type, a);
            return this;
        }
    }

    public class SqlBuilderDomain
    {
        private SqlBuilder _internal;
        private IDictionary<Type, String> _aliasMap;

        private SqlBuilder.Template _countTemplate;
        private SqlBuilder.Template _deleteTemplate;
        private SqlBuilder.Template _insertTemplate;
        private SqlBuilder.Template _selectTemplate;
        private SqlBuilder.Template _updateTemplate;

        public Type EntityType { get; set; }

        public ClassMap Map { get; set; }

        public int AliasLevel { get; set; }

        public Dictionary<int, TableAliasMap> AliasMap { get; set; }

        public SqlBuilderDomain(ClassMap map, int aliasLevel = 0)
        {
            Map = map;
            AliasLevel = aliasLevel;
            AliasMap = new Dictionary<int, TableAliasMap>();
            IniitializeTemplates();
        }
       
        public SqlBuilderDomain(Type entityType, int aliasLevel = 0)
        {
            AliasLevel = aliasLevel;
            AliasMap = new Dictionary<int, TableAliasMap>();
            Map = ClassMapper.GetMap(entityType);
            IniitializeTemplates();
        }

        public SqlBuilder.Template Count
        {
            get
            {
                if(_countTemplate == null)BuildCount();

                return _countTemplate;
            }
        }

        public SqlBuilder.Template Delete
        {
            get
            {
                if (_deleteTemplate == null) BuildDelete();

                return _deleteTemplate;
            }
        }
       
        public SqlBuilder.Template Insert
        {
            get
            {
                if (_insertTemplate == null) BuildInsert();

                return _insertTemplate;
            }
        }

        public SqlBuilder.Template Select
        {
            get
            {
                if (_selectTemplate == null) BuildSelect();

                return _selectTemplate;
            }
        }

        public SqlBuilder.Template Update
        {
            get
            {
                if (_updateTemplate == null) BuildUpdate();
                
                return _updateTemplate;
            }
        }

        public SqlBuilderDomain OrderBy(string sql, dynamic parameters = null)
        {
            if (String.IsNullOrEmpty(sql)) return this;

            _internal.OrderBy(sql, parameters);
            return this;
        }

        public SqlBuilderDomain Where(dynamic parameters)
        {
            var paramNames = GetParamNames((object)parameters);
            paramNames.ForEach(p => _internal.Where("[" + p + "]" + "= @" + p));
            _internal.AddParameters(parameters);
            return this;
        }

        public SqlBuilderDomain Where(ICriteria criteria)
        {
            _internal.Where(criteria.ToSql(), criteria.Parameters);
            return this;
        } 

        public SqlBuilderDomain Where(string sql, dynamic parameters = null)
        {
            _internal.Where(sql, parameters);
            return this;
        } 

        private void IniitializeTemplates()
        {
            EntityType = Map.EntityType;

            _internal = new SqlBuilder();

            InitializeAliasMap();
        }

        private void InitializeAliasMap()
        {
            var map = new TableAliasMap();

            if (Map.IsSubClassMap())
            {
                var subClass = Map as SubClassMap;
                map.AddAlias(subClass.EntityType, subClass.Join.TableName, CreateAlias(subClass.Join.TableName));
                map.AddAlias(subClass.ParentEntityType, Map.Table, CreateAlias(Map.Table));
            }
            else
            {
                map.AddAlias(Map.EntityType, Map.Table, CreateAlias(Map.Table));
            }

            AliasMap.Add(AliasLevel, map);
        }

        private String CreateAlias(string table)
        {  
            return "_" + table + AliasLevel;
        }

        private String GetAlias(Type t, int? level = null)
        {
            level = level ?? AliasLevel;

            return AliasMap[level.Value][t].Alias;
        }

        private String GetAlias(PropertyInfo property)
        {
            return GetAlias(!property.DeclaringType.IsAbstract ? property.DeclaringType : property.ReflectedType);
        }

        private void BuildCount()
        {
            throw new NotImplementedException();
        }

        private void BuildDelete()
        {
            throw new NotImplementedException();
        }

        private void BuildInsert()
        {
            var properties = PropertiesForInsert(Map);
            var cols = string.Join(",", properties.Select(p => "[" + p + "]"));
            var colParams = string.Join(",", properties.Select(p => "@" + p));

            var sql = "set nocount on\n insert " + Map.Table + " (" + cols + ") values (" + colParams + ")";

            _insertTemplate = _internal.AddTemplate(sql);
        }

        private void BuildSelect()
        {
            var tableName = Map.Table;
            var tableAlias = GetAlias(Map.EntityType);

            if (Map.IsSubClassMap())
            {
                var subClassMap = Map as SubClassMap;
                var joinPart = subClassMap.Join;
                var joinAlias = tableAlias;

                tableAlias = GetAlias(subClassMap.ParentEntityType);

                _internal.Join(String.Format("[{0}] as {1} on {2}.[{3}] = {4}.[{5}]",
                                            joinPart.TableName,
                                            joinAlias,
                                            joinAlias,
                                            joinPart.KeyColumn,
                                            tableAlias,
                                            subClassMap.Id.ColumnName));
            }

            var sql = String.Format("select /**select**/ from {0} as {1} /**join**/ /**leftjoin**/ /**where**/ /**orderby**/", tableName, tableAlias);
            var properties = GetParamNames(Map);
            _selectTemplate = _internal.AddTemplate(sql);

            _internal.Select(properties);
        }

        private void BuildUpdate()
        {
            var tableName = Map.Table;
            
            var updateProperties = String.Join(", \n", Map.Properties.Values.Select(v => "[" + v.ColumnName + "] = @" + v.Name));
            var whereClause = "[" + Map.Id.ColumnName + "] = @" + Map.Id.Name;
            var sql = String.Format("update {0} set {1} where {2}", tableName, updateProperties, whereClause);
            
            _updateTemplate = _internal.AddTemplate(sql);

        }

        public List<string> GetParamNames(object o)
        {
            if (o is DynamicParameters)
            {
                return (o as DynamicParameters).ParameterNames.ToList();
            }

            var t = o.GetType();

            if (t.IsAnonymousType())
            {
                return t.GetProperties().Select(p => p.Name).ToList();
            }

            return GetParamNames(Map);
        }

        public List<String> GetParamNames(ClassMap map, string alias = null, bool includeKey = true)
        {
            var props =  map.Properties.Values.Select(p => String.Format("{0}.[{1}]", alias ?? GetAlias(p.Property) , p.ColumnName)).ToList();

            if (includeKey)
            {
                props.Add(String.Format("{0}.[{1}]", alias ?? GetAlias(map.Id.Property), map.Id.ColumnName));
            }

            return props;
        }

        public List<String> PropertiesForInsert(ClassMap map)
        {
            var properties = map.Properties.Values.Select(p => p.ColumnName).ToList();
            properties.Add(map.Id.ColumnName);

            //Remove Identity properties from Insert
            if (Map.Id.Property.PropertyType.IsAssignableFrom(typeof(int)))
            {
                properties.Remove(Map.Id.Name);
            }

            return properties;
        }
    }
}
