using System;
using System.Collections.Generic;
using System.Text;

namespace QuantumConcepts.Common.Utils.DescriptiveEnum
{
    /// <summary>Empty interface meant to indicate an enum includes descriptions for each value.</summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class DescriptiveEnumEnforcementAttribute : System.Attribute
    {
        public enum EnforcementTypeEnum
        {
            ThrowException,
            DefaultToValue
        }

        private EnforcementTypeEnum _enforcementType = EnforcementTypeEnum.DefaultToValue;

        public EnforcementTypeEnum EnforcementType
        {
            get { return _enforcementType; }
            set { _enforcementType = value; }
        }

        public DescriptiveEnumEnforcementAttribute() { }

        public DescriptiveEnumEnforcementAttribute(EnforcementTypeEnum enforcementType)
        {
            _enforcementType = enforcementType;
        }
    }
}
