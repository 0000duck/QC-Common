using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    [XmlRoot("DI")]
    public class ODPCategory
    {
        private string _category;
        private string _summary;

        [XmlElement("DT")]
        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }

        [XmlElement("DS")]
        public string Summary
        {
            get { return _summary; }
            set { _summary = value; }
        }
    }
}
