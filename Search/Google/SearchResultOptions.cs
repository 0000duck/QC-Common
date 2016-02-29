using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    [XmlRoot("HAS")]
    public class SearchResultOptions
    {
        private ODPCategory _odpCategory;
        private bool _link;
        private CachedPage _cachedPage;
        private bool _hasRelated;

        [XmlElement("DI")]
        public ODPCategory OdpCategory
        {
            get { return _odpCategory; }
            set { _odpCategory = value; }
        }

        //[XmlElement("L")]
        public bool Link
        {
            get { return _link; }
            set { _link = value; }
        }

        [XmlElement("C")]
        public CachedPage CachedPage
        {
            get { return _cachedPage; }
            set { _cachedPage = value; }
        }

        //[XmlElement("RT")]
        public bool HasRelated
        {
            get { return _hasRelated; }
            set { _hasRelated = value; }
        }
    }
}
