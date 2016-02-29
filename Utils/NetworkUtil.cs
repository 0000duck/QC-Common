using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace QuantumConcepts.Common.Utils
{
    public static class NetworkUtil
    {
        public static bool CheckInternetConnection()
        {
            return CheckInternetConnection("http://www.google.com");
        }

        private static bool CheckInternetConnection(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.OpenRead(url);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
