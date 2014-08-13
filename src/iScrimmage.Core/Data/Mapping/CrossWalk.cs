using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Mapping
{
    public class CrossWalk: Dictionary<object, string>
    {
            public static CrossWalk Genders = new CrossWalk()
                                              {
                                                  {0, "Female"},
                                                  {1, "Male"}
                                              };
    }
}
