using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Models;

namespace QuantumConcepts.Common.Controllers
{
    public abstract class BaseController<M>
        where M : BaseModel, new()
    {
        protected M Model { get; private set; }

        public BaseController()
        {
            this.Model = new M();
        }

        public abstract void InitializeView();
    }
}
