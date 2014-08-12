using System;
using System.IO;
using System.Reflection;

namespace iScrimmage.Core.Common
{
    public class ResourceHelper
    {
        public static string ReadEmbeddedResource(Type t)
        {
            return ReadEmbeddedResource(t.FullName, t.Assembly);
        }

        public static string ReadEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            return ReadEmbeddedResource(resourceName, assembly);
        }

        public static string ReadEmbeddedResource(string resourceName, Assembly assembly)
        {
            var result = "";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }
    }
}
