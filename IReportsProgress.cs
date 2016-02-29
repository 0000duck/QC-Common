using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common
{
    public delegate void ReportProgressEventHandler(string message, long? current, long? total);

    public interface IReportsProgress
    {
        event ReportProgressEventHandler ReportProgress;
    }
}