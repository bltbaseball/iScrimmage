using System;
using System.Reflection;

namespace iScrimmage.Core.Data.Mapping
{
    public class PropertyMap
    {
        public PropertyMap(){}

        public PropertyMap(PropertyInfo prop)
        {
            Property = prop;
            Name = prop.Name;
            ColumnName = prop.Name;
        }

        public PropertyInfo Property { get; set; }

        /// <summary>
        /// The name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column name for this property
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Should this property be ignored when generating sql
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        /// True if the property can contain a null value
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// True if the changes to the property should be stored in the ChangeLog
        /// </summary>
        public bool IsAuditable { get { return AuditPart != null; } }

        /// <summary>
        /// Auditable definition for this property. Will be null if IsAuditable is false
        /// </summary>
        public AuditPropertyPart AuditPart { get; protected set; }

        public AuditPropertyPart Audit()
        {
            return AuditPart ?? (AuditPart = new AuditPropertyPart() {ReadableName = Name, IsReference = false});
        }

        public AuditPropertyPart Audit(string readableName)
        {
            Audit().ReadableName = readableName;
            return AuditPart;
        }
    }
}
