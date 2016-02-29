using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace QuantumConcepts.Common.Utils
{
    public class CookieUtil
    {
        public static CookieUtil Instance { get; private set; }

        static CookieUtil()
        {
            CookieUtil.Instance = new CookieUtil();
        }

        protected CookieUtil() { }

        protected HttpCookie Get(string name)
        {
            return HttpContext.Current.Request.Cookies[name];
        }

        protected int GetInt(string name, int defaultValue)
        {
            int? value = GetInt(name);

            return (value.HasValue ? value.Value : defaultValue);
        }

        protected int? GetInt(string name)
        {
            HttpCookie cookie = Get(name);
            int value;

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value) && int.TryParse(cookie.Value, out value))
                return value;

            return null;
        }

        protected string GetString(string name)
        {
            HttpCookie cookie = Get(name);

            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
                return cookie.Value;

            return null;
        }

        protected void Set(string name, string value)
        {
            Set(name, value, DateTime.Now.AddMonths(1));
        }

        protected void Set(string name, string value, int expirationDays)
        {
            Set(name, value, DateTime.Now.AddDays(expirationDays));
        }

        protected void Set(string name, string value, DateTime expirationDate)
        {
            HttpCookie cookie = Get(name);
            bool expire = (value == null);

            if (cookie != null)
            {
                if (expire)
                    cookie.Expires = DateTime.Now.AddDays(-1);
                else
                {
                    cookie.Value = value;
                    cookie.Expires = expirationDate;
                }
            }
            else if (!expire)
            {
                cookie = new HttpCookie(name, value);
                cookie.Expires = expirationDate;
                cookie.Secure = false;
            }
            else
                return; //No need to expire a cookie that doesn't exist.

            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}