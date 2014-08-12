using iScrimmage.Core.Extensions;
using System;
using System.Linq;

namespace iScrimmage.Core.Data.Conventions
{
    public interface ITableNamingConvention
    {
        string GetTableName(Type type);
    }

    public class DefaultTableNamingConvention : ITableNamingConvention
    {
        public string GetTableName(Type type)
        {
            return type.Name.MakePlural();
        }
    }

    public class TableAttributeNamingConvention: ITableNamingConvention
    {
        public string GetTableName(Type type)
        {   
            var name = type.Name.MakePlural();
            if (type.IsInterface && name.StartsWith("I"))
            {
                name = name.Substring(1);
            }

            //NOTE: This as dynamic trick should be able to handle both our own Table-attribute as well as the one in EntityFramework 
            var tableattr = type.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as dynamic;
            if (tableattr != null)
            {
                name = tableattr.Name;
            }

            return name;
        }
    }
}
