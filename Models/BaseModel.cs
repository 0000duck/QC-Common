using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Models
{
    public abstract class BaseModel
    {
        public abstract void InitializeModel();
        public virtual void InitializeModelData() { }
    }
}
