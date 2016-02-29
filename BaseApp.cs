using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QuantumConcepts.Common.Utils;
using System.Security.Cryptography;

namespace QuantumConcepts.Common
{
    public abstract class BaseApp
    {
        public static string ApplicationTitle { get { return Application.ProductName; } }
        public static Version Version { get { return new Version(Application.ProductVersion); } }
        public static string ApplicationKey { get; private set; }
        public static string ApiKey { get { return "8mwUZMowWlPaGQC07kKZbrHRKxZ7RJ0kSOGjGx6eGo0="; } }
        public static string ApiSecret { get { return "6TzPoMNhQnUj7GUsnkBNOQ=="; } }
        public static string Url { get; private set; }
        public static string TrialLimitations { get; set; }
        public static string DeviceUniqueID
        {
            get
            {
                StringBuilder id = new StringBuilder();
                KeyValuePair<string, object>? key = null;

                if ((key = WMIUtil.Search(WMIUtil.Processor, "ProcessorId")).HasValue)
                    id.Append(key.Value.Value);

                if ((key = WMIUtil.Search(WMIUtil.BIOS, "Manufacturer")).HasValue)
                    id.Append(key.Value.Value);


                return id.ToString();
            }
        }
        public static bool IsTrial { get; private set; }
        public static DateTime ValidFrom { get; private set; }
        public static DateTime? ExpirationDate { get; private set; }

        public static void Initialize(string applicationKey, string url, string trialLimitations)
        {
            BaseApp.ApplicationKey = applicationKey;
            BaseApp.Url = url;
            BaseApp.TrialLimitations = trialLimitations;
        }

        public static void InitializeActivationData(bool isTrial, DateTime validFrom, DateTime? expirationDate)
        {
            BaseApp.IsTrial = isTrial;
            BaseApp.ValidFrom = validFrom;
            BaseApp.ExpirationDate = expirationDate;
        }
    }
}
