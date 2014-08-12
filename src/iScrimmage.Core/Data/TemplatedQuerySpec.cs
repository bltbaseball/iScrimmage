using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iScrimmage.Core.Common;

namespace iScrimmage.Core.Data
{
    public abstract class TemplatedQuerySpec<TQ, T> : IQuerySpec<T>
        where T : class
        where TQ : IQuerySpec<T>
    {
        // ReSharper disable StaticFieldInGenericType
        private static String _template;
        // ReSharper restore StaticFieldInGenericType

        public abstract T Execute(IDataContext context);

        protected String Template
        {
            get
            {
                if (String.IsNullOrEmpty(_template))
                {
                    var type = typeof(TQ);

                    _template = ResourceHelper.ReadEmbeddedResource(type);
                }

                return _template;
            }
        }
    }
}
