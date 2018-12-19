using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace QuantumConcepts.Common.Utils
{
    public class ResourceUrl
    {
        protected ResourceUrl _parentUrl = null;
        protected string _name = null;
        protected bool _requireSecurity = false;
        protected string _relativeUrl = null;

        public ResourceUrl ParentUrl { get { return _parentUrl; } }
        public bool RequireSecurity { get { return _requireSecurity; } }

        public string Name { get { return _name; } }

        public string RelativeUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_relativeUrl))
                {
                    SetRelativeUrl();
                }

                return _relativeUrl;
            }
        }

        public ResourceUrl() { }

        public ResourceUrl(ResourceUrl parentUrl, string name)
            : this(parentUrl, name, false)
        { }

        public ResourceUrl(ResourceUrl parentUrl, string name, bool requireSecurity)
        {
            _parentUrl = parentUrl;
            _name = name;
            _requireSecurity = (requireSecurity || (parentUrl != null && parentUrl.RequireSecurity));
        }

        private void SetRelativeUrl()
        {
            _relativeUrl = (_parentUrl == null ? "" : _parentUrl.RelativeUrl + "/") + _name;
        }

        public string FormatUrl(params UrlParameter[] parameters)
        {
            return FormatUrl(this.RelativeUrl, parameters);
        }

        public string FormatUrl(string baseUrl, IEnumerable<UrlParameter> parameters)
        {
            if (parameters != null && parameters.Count() > 0)
            {
                StringBuilder url = new StringBuilder(baseUrl);

                url.Append("?");

                foreach (UrlParameter parameter in parameters)
                {
                    url.Append(parameter.ToString());

                    if (parameter != parameters.Last())
                        url.Append("&");
                }

                return url.ToString();
            }

            return baseUrl;
        }

        public string FormatRelativeUrl(UrlParameterList parameters)
        {
            return FormatUrl(this.RelativeUrl, parameters);
        }
        
        public override string ToString()
        {
            return this.RelativeUrl;
        }

        protected virtual bool SSLEnabled { get { return false; } }
    }

    public class UrlParameterList : List<UrlParameter>
    {
        public UrlParameter Add(string name, string value)
        {
            UrlParameter parameter = new UrlParameter(name, value);

            this.Add(parameter);

            return parameter;
        }
    }

    public class UrlParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public UrlParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            StringBuilder parameterString = new StringBuilder();

            parameterString.Append(this.Name);
            parameterString.Append("=");

            if (!string.IsNullOrEmpty(this.Value))
            {
                parameterString.Append(WebUtility.UrlEncode(this.Value));
            }

            return parameterString.ToString();
        }
    }
}
