using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Utils.DescriptiveEnum;

namespace QuantumConcepts.Common.Search.Google
{
    [DescriptiveEnumEnforcement(DescriptiveEnumEnforcementAttribute.EnforcementTypeEnum.ThrowException)]
    public enum SafeSearchMode
    {
        [Description("off")]
        Off,
        [Description("medium")]
        Medium,
        [Description("high")]
        High
    }
}
