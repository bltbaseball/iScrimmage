using iScrimmage.Core.Data.Mapping;

namespace iScrimmage.Core.Data.Extensions
{
    public static class ClassMapExtensions
    {
        public static bool IsSubClassMap(this ClassMap map)
        {
            return map is SubClassMap;
        }
    }
}
