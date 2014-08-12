using System;

namespace iScrimmage.Core.Data.Mapping
{
    public class SubClassMap: ClassMap
    {
        public SubClassMap(Type t, Type parent) : base(t)
        {
            ParentEntityType = parent;
        }

        public Type ParentEntityType { get; set; }

        public JoinPart Join { get; set; } 

        public string Discriminator { get; set; }
    }
}
