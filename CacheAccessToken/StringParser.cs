using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CacheAccessToken
{
    internal class StringParser
    {
        private Dictionary<string, string> ParseCookies(HttpResponseMessage response)
        {
            var collection = new Dictionary<string, string>();
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookie))
            {
                foreach (var cookies in setCookie)
                {
                    string[] values = cookies.Split(';');
                    foreach (var cookie in values)
                    {
                        string[] a = cookie.Split('=');
                        if (a.Length != 2)
                        {
                            continue;
                        }

                        if (collection.ContainsKey(a[0]))
                        {
                            collection.Add(a[0], a[1]);
                        }
                    }
                }
            }
            return collection;
        }

        private Dictionary<string, string> ParseCookiesLinq(HttpResponseMessage response)
        {
            var collection = new Dictionary<string, string>();

            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> setCookies))
            {
                foreach (string[] keyValue in from setCookie in setCookies
                                              let cookies = setCookie.Split(';')
                                              from a in
                                                  from cookie in cookies
                                                  let kv = cookie.Split('=')
                                                  select kv
                                              select a)
                {
                    if (keyValue.Length != 2)
                    {
                        continue;
                    }

                    if (collection.ContainsKey(keyValue[0]))
                    {
                        collection.Add(keyValue[0], keyValue[1]);
                    }
                }
            }
            return collection;
        }
    }
}