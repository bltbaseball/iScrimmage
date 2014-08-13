using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;
using System.ComponentModel;


namespace iScrimmage.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToBase64(this string source)
        {
            var bytes = Encoding.ASCII.GetBytes(source);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string encodedString)
        {
            var bytes = Convert.FromBase64String(encodedString);
            return Encoding.ASCII.GetString(bytes);
        }
        
        public static string SubstringBefore(this string s, string match)
        {
            var idx = s.IndexOf(match, System.StringComparison.OrdinalIgnoreCase);
            if (idx == -1)
            {
                return "";
            }
            return s.Substring(0, idx);
        }

        public static string SubstringAfter(this string s, string match, bool excludeMatch = true)
        {
            var idx = s.LastIndexOf(match, System.StringComparison.OrdinalIgnoreCase);
            if (idx == -1)
            {
                return String.Empty;
            }

            if (excludeMatch)
            {
                idx = idx + match.Length;
            }
            return idx >= s.Length ? "" : s.Substring(idx);
        }

        public static string InsertBefore(this string s, string match, string insert)
        {
            var idx = s.LastIndexOf(match, System.StringComparison.OrdinalIgnoreCase);
            if (idx == -1)
            {
                return s;
            }

            return s.SubstringBefore(match) + insert + s.SubstringAfter(match, false);
        }

        public static string AppendMessage(this string replyText, string originalMessage, string linePrefix = ">")
        {
            var sb = new StringBuilder(replyText);

            if (!String.IsNullOrEmpty(originalMessage))
            {
                sb.Append("\n\n------ Original Message ------\n\n");
                sb.Append(linePrefix + originalMessage.Replace("\n", "  \n" + linePrefix));
            }
            return sb.ToString();
        }

        public static string Wordify(this string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            var regex = new Regex("(?<=[a-z])(?<x>[A-Z0-9])|(?<=.)(?<x>[A-Z0-9])(?=[a-z])");
            return regex.Replace(s, " ${x}");
        }

        public static string FormatWith(this string s, params object[] args)
        {
            return String.Format(s, args);
        }

        public static String RemoveWhiteSpace(this string s)
        {
            return Regex.Replace(s, @"\s", "");
        }

        public static String RemoveDoubleSpaces(this string s)
        {
            if (String.IsNullOrEmpty(s)) return s;

            return s.Replace("  ", " ");
        }
        public static string Pluralize(this string s)
        {
            return s + (s.Last() == 's' || s.Substring(s.Length-2, 2) == "es"? "" :"s");
        }

        public static string Shorten(this string s, int length)
        {
            if (s.Length > length)
            {
                return s.Substring(0, length) + "...";
            }
            return s;
        }

        public static List<string> QuickSplit(this string s, string separator)
        {
            if(String.IsNullOrEmpty(s))
            {
                return new List<string>();
            }

            return s.Split(new[] {separator}, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }

    public static class StringInjectExtension
    {
        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching object properties.
        /// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="injectionObject">The object whose properties should be injected in the string</param>
        /// <returns>A version of the formatString string with keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, object injectionObject)
        {
            return formatString.Inject(GetPropertyHash(injectionObject));
        }

        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching dictionary entries.
        /// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="dictionary">An <see cref="IDictionary"/> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with dictionary keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, IDictionary dictionary)
        {
            return formatString.Inject(new Hashtable(dictionary));
        }

        /// <summary>
        /// Extension method that replaces keys in a string with the values of matching hashtable entries.
        /// <remarks>Uses <see cref="String.Format()"/> internally; custom formats should match those used for that method.</remarks>
        /// </summary>
        /// <param name="formatString">The format string, containing keys like {foo} and {foo:SomeFormat}.</param>
        /// <param name="attributes">A <see cref="Hashtable"/> with keys and values to inject into the string</param>
        /// <returns>A version of the formatString string with hastable keys replaced by (formatted) key values.</returns>
        public static string Inject(this string formatString, Hashtable attributes)
        {
            string result = formatString;
            if (attributes == null || formatString == null)
                return result;

            foreach (string attributeKey in attributes.Keys)
            {
                result = result.InjectSingleValue(attributeKey, attributes[attributeKey]);
            }
            return result;
        }

        /// <summary>
        /// Replaces all instances of a 'key' (e.g. {foo} or {foo:SomeFormat}) in a string with an optionally formatted value, and returns the result.
        /// </summary>
        /// <param name="formatString">The string containing the key; unformatted ({foo}), or formatted ({foo:SomeFormat})</param>
        /// <param name="key">The key name (foo)</param>
        /// <param name="replacementValue">The replacement value; if null is replaced with an empty string</param>
        /// <returns>The input string with any instances of the key replaced with the replacement value</returns>
        public static string InjectSingleValue(this string formatString, string key, object replacementValue)
        {
            string result = formatString;
            //regex replacement of key with value, where the generic key format is:
            //Regex foo = new Regex("{(foo)(?:}|(?::(.[^}]*)}))");
            Regex attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");  //for key = foo, matches {foo} and {foo:SomeFormat}

            //loop through matches, since each key may be used more than once (and with a different format string)
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0) //matched {foo:SomeFormat}
                {
                    //do a double string.Format - first to build the proper format string, and then to format the replacement value
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                }
                else //matched {foo}
                {
                    replacement = (replacementValue ?? string.Empty).ToString();
                }
                //perform replacements, one match at a time
                result = result.Replace(m.ToString(), replacement);  //attributeRegex.Replace(result, replacement, 1);
            }
            return result;

        }


        /// <summary>
        /// Creates a HashTable based on current object state.
        /// <remarks>Copied from the MVCToolkit HtmlExtensionUtility class</remarks>
        /// </summary>
        /// <param name="properties">The object from which to get the properties</param>
        /// <returns>A <see cref="Hashtable"/> containing the object instance's property names and their values</returns>
        private static Hashtable GetPropertyHash(object properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }
            return values;
        }

    }


}
