using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Security
{
    public class CredentialException : Exception
    {
        public CredentialException(string message)
            : base(message, null)
        {
        }
    }
}
