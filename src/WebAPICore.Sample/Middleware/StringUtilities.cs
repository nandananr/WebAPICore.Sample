using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace  WebAPICore.Sample
{
    public static class StringUtilities
    {
        static public bool CompareStrings(string pattern, string url)
        {
            if (pattern.Length > url.Length)
            {
                return false;
            }

            string surl = url.Substring(0, pattern.Length);
            if (pattern.Equals(surl, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
            return false;
        }

        static public string ConvertDictionaryToString(Dictionary<string, string> dictionary)
        {
            return string.Join(",", dictionary.Select(x => x.Key + ":" + x.Value));
        }
    }
}
