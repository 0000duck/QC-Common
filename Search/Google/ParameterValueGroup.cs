using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Search.Google
{
    public class ParameterValueGroup : List<IParameterValue>, IParameterValue
    {
        private SearchMode _mode = SearchMode.AND;

        public SearchMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public override string ToString()
        {
            StringBuilder value = new StringBuilder();

            foreach (IParameterValue thisValue in this)
            {
                value.Append(thisValue.ToString());

                if (thisValue != this[this.Count - 1])
                    value.Append(_mode == SearchMode.AND ? "." : "|");
            }

            return value.ToString();
        }
    }
}
