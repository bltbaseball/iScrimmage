using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iScrimmage.Core.Data.Generators;

namespace iScrimmage.Core.Data.Mapping
{
    public class KeyMap: PropertyMap
    {
        public IIdentifyGenerator Generator{ get; set; }

        public object UnsavedValue { get; set; }

        public void AssignIdentifier(dynamic data)
        {
            if (Property.PropertyType.IsAssignableFrom(typeof(Guid)))
            {
                var currentKey = Property.GetValue(data);
                if (currentKey == Guid.Empty)
                {
                    var generator = new GuidCombGenerator();
                    Property.SetValue(data, generator.Generate(data));
                }
            }
        }
    }
}
