using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    [XmlRoot("C")]
    public class CachedPage
    {
        private string _size;
        private string _id;

        [XmlAttribute("SZ")]
        public string Size
        {
            get { return _size; }
            set { _size = value; }
        }

        [XmlAttribute("CID")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
    }
}
