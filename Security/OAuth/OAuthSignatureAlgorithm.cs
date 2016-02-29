using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Utils.DescriptiveEnum;

namespace QuantumConcepts.Common.Security.OAuth
{
    [DescriptiveEnumEnforcement(DescriptiveEnumEnforcementAttribute.EnforcementTypeEnum.ThrowException)]
    public enum OAuthSignatureAlgorithm
    {
        [Description("HMAC-SHA1")]
        HMACSHA1,

        [Description("RSA-SHA1")]
        RSASHA1,

        [Description("PLAINTEXT")]
        PlainText
    }
}