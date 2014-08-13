using System;

namespace iScrimmage.Core.Data.Mapping.Attributes
{
    /// <summary>
    /// Describes a joined subclass mapping between a parent and child domain object
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SubClassAttribute: Attribute
    {
        public SubClassAttribute()
        {
            Discriminator = null;
        }

        public string Table { get; set; }
        public string KeyColumn { get; set; }
        public string Discriminator { get; set; }
    }
}
