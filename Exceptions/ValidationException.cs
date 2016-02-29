using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public string Tag { get; private set; }

        public ValidationException() : this(null, null, null) { }
        public ValidationException(string tag) : this(tag, null, null) { }
        public ValidationException(string tag, string message) : this(tag, message, null) { }
        public ValidationException(string tag, string message, Exception innerException)
            : base(message, innerException)
        {
            this.Tag = tag;
        }
    }
}