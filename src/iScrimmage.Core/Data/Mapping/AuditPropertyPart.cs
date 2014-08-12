using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Mapping
{
    /// <summary>
    /// Specifies the details and rules for recording changes to a mapped class property
    /// </summary>
    public class AuditPropertyPart
    {
        public AuditPropertyPart()
        {
            AuditOnInsert = true;
            AuditOnUpdate = true;
            AuditOnDelete = true;
        }

        /// <summary>
        /// Log new values on insert
        /// </summary>
        public bool AuditOnInsert { get; protected set; }

        /// <summary>
        /// Log changed values on update
        /// </summary>
        public bool AuditOnUpdate { get; protected set; }

        /// <summary>
        /// Log final values on Delete
        /// </summary>
        public bool AuditOnDelete { get; protected set; }

        /// <summary>
        /// True if this property has cross walk values
        /// </summary>
        public bool HasCrossWalk { get; protected set; }

        /// <summary>
        /// Map of entity values to their readable counter parts
        /// </summary>
        public CrossWalk CrossWalk { get; set; }

        /// <summary>
        /// Name this property should appear as in the change log. Defaults to the name of the property
        /// </summary>
        public string ReadableName { get; set; }

        /// <summary>
        /// True if this property is a refernce to another domain object(table) and we need to query for the value to log
        /// </summary>
        public bool IsReference { get; set; }

        /// <summary>
        /// Type of the object being referenced. The object's class map will be used to query the table
        /// </summary>
        public Type ReferenceType { get; set; }

        /// <summary>
        /// The column to in the referenced table whose value we want to place in the Change Log
        /// </summary>
        public string ReferenceColumn { get; set; }

        /// <summary>
        /// Column whose value is stored in this property. Should usually be the primary key column of the ReferenceType
        /// </summary>
        public string ReferenceKeyColumn { get; set; }

        /// <summary>
        /// Set the references property rules for this auditable entity
        /// </summary>
        /// <param name="referenceType">Type of object that if being referenced</param>
        /// <param name="column">Column in the referenced table to place in the audit log table</param>
        /// <param name="keyColumn">Key column in the referenced table to use when joinin for value column</param>
        /// <returns></returns>
        public AuditPropertyPart References(Type referenceType, string column, string keyColumn )
        {
            IsReference = true;
            ReferenceType = referenceType;
            ReferenceColumn = column;
            ReferenceKeyColumn = keyColumn;

            return this;
        }

        /// <summary>
        /// Set up a mapping of values (keys) to readable entity values. Useful for flags and enumerations
        /// </summary>
        /// <returns></returns>
        public AuditPropertyPart SetCrossWalk(IDictionary<object, string> values)
        {
            HasCrossWalk = true;
            CrossWalk = new CrossWalk();
            
            foreach (var v in values)
            {
                CrossWalk[v.Key] = v.Value;
            }

            return this;
        }

        public AuditPropertyPart SetCrossWalk(CrossWalk values)
        {
            HasCrossWalk = true;
            CrossWalk = values;

            return this;
        }

        /// <summary>
        /// Set the actions on which this property will be audited. Defaults to All
        /// </summary>
        public AuditPropertyPart AuditOn(bool insert, bool update, bool delete)
        {
            AuditOnInsert = insert;
            AuditOnUpdate = update;
            AuditOnDelete = delete;

            return this;
        }
    }
}
