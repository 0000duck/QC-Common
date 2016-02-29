using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using QuantumConcepts.Common.Extensions;
using System.Collections.Specialized;

namespace QuantumConcepts.Common.Utils
{
    /// <summary>Provides basic functionality for reading configuration settings from a source, such as an application or web configuration file.</summary>
    /// <typeparam name="T">The type of the ConfigUtil.</typeparam>
    public class ConfigUtil
    {
        public static ConfigUtil Instance { get; private set; }
        
        public NameValueCollection AppSettings { get; }
        public ConnectionStringSettingsCollection ConnectionStrings { get; }

        static ConfigUtil()
        {
            ConfigUtil.Instance = new ConfigUtil();
        }

        protected ConfigUtil() : this(ConfigurationManager.AppSettings, ConfigurationManager.ConnectionStrings) { }

        public ConfigUtil(NameValueCollection appSettings, ConnectionStringSettingsCollection connectionStrings) {
            if (appSettings == null)
                throw new ArgumentNullException(nameof(appSettings));

            if (connectionStrings == null)
                throw new ArgumentNullException(nameof(connectionStrings));

            this.AppSettings = appSettings;
            this.ConnectionStrings = connectionStrings;
        }

        /// <summary>Gets the database connection string to use.</summary>
        public string ConnectionString { get { return GetConnectionString("Default"); } }

        /// <summary>Indicates whether or not SSL is enabled.</summary>
        public bool SSLEnabled { get { return GetAppSettingAsBool("SSLEnabled", false); } }

        /// <summary>Indicates whether or not cookie (remember me) authentication is enabled.</summary>
        public bool EnableAuthenticationCookie { get { return GetAppSettingAsBool("EnableAuthenticationCookie", false); } }

        /// <summary>Indicates whether or not email is enabled.</summary>
        public bool EmailDisabled { get { return GetAppSettingAsBool("EmailDisabled", false); } }

        /// <summary>Indicates a testing email to use for all emails sent from the system.</summary>
        public string OverrideEmailAddress { get { return GetAppSetting("OverrideEmailAddress"); } }

        /// <summary>The email recipient(s) to whom exceptions should be sent.</summary>
        public string[] InternalEmailRecipients { get { return GetAppSettingAsStringArray("InternalEmailRecipients", ';'); } }

        /// <summary>The Google Analytics Account Number to use.</summary>
        public string GoogleAnalyticsAccountNumber { get { return GetAppSetting("GoogleAnalyticsAccountNumber"); } }

        public string QCConnectAPIKey { get { return GetAppSetting("QCConnectAPIKey"); } }
        public string QCConnectAPISecret { get { return GetAppSetting("QCConnectAPISecret"); } }
        public QCConnectWS.Authorization QCConnectAPIAuthorization { get { return new QCConnectWS.Authorization() { Key = QCConnectAPIKey, Secret = QCConnectAPISecret }; } }
        public int PageSize { get { return GetAppSettingAsInt("PageSize", 25); } }

        /// <summary>Gets the specified AppSetting as a string.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public string GetAppSetting(string key, string defaultValue = null)
        {
            return (this.AppSettings[key] ?? defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a string array.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="delimiter">The character by which to delimit the value.</param>
        /// <returns>The value of the AppSetting.</returns>
        public string[] GetAppSettingAsStringArray(string key, char delimiter)
        {
            string value = GetAppSetting(key);

            if (value.IsNullOrEmpty())
                return null;

            return value.Split(delimiter);
        }

        /// <summary>Gets the specified AppSetting as a nullable bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public bool? GetAppSettingAsNullableBool(string key)
        {
            bool value;

            if (bool.TryParse(GetAppSetting(key), out value))
                return value;

            return null;
        }

        /// <summary>Gets the specified AppSetting as a bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public bool GetAppSettingAsBool(string key, bool defaultValue)
        {
            bool? value = GetAppSettingAsNullableBool(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable int.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public int? GetAppSettingAsNullableInt(string key)
        {
            int value;

            if (int.TryParse(GetAppSetting(key), out value))
                return value;

            return null;
        }

        /// <summary>Gets the specified AppSetting as an int.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public int GetAppSettingAsInt(string key, int defaultValue)
        {
            int? value = GetAppSettingAsNullableInt(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable decimal.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public decimal? GetAppSettingAsNullableDecimal(string key)
        {
            decimal value;

            if (decimal.TryParse(GetAppSetting(key), out value))
                return value;

            return null;
        }

        /// <summary>Gets the specified AppSetting as a decimal.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public decimal GetAppSettingAsDecimal(string key, decimal defaultValue)
        {
            decimal? value = GetAppSettingAsNullableDecimal(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public DateTime? GetAppSettingAsNullableDateTime(string key)
        {
            DateTime value;

            if (DateTime.TryParse(GetAppSetting(key), out value))
                return value;

            return null;
        }

        /// <summary>Gets the specified AppSetting as a bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public DateTime GetAppSettingAsDateTime(string key, DateTime defaultValue)
        {
            DateTime? value = GetAppSettingAsNullableDateTime(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable enum.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public T? GetAppSettingAsNullableEnum<T>(string key)
            where T : struct
        {
            T value;

            if (Enum.TryParse<T>(GetAppSetting(key), out value))
                return value;

            return null;
        }

        /// <summary>Gets the specified AppSetting as an enum.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public T GetAppSettingAsEnum<T>(string key, T defaultValue)
            where T : struct
        {
            T? value = GetAppSettingAsNullableEnum<T>(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified ConnectionString.</summary>
        /// <param name="key">The key of the ConnectionString to get.</param>
        /// <returns>The value of the ConnectionString or null.</returns>
        public string GetConnectionString(string key)
        {
            ConnectionStringSettings connectionString = this.ConnectionStrings?[key];

            return (connectionString == null ? null : connectionString.ConnectionString);
        }
    }
}
