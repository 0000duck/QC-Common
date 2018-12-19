using QuantumConcepts.Common.Extensions;
using System;
using System.Collections.Specialized;

namespace QuantumConcepts.Common.Utils
{
    /// <summary>Provides basic functionality for reading configuration settings from a source, such as an application or web configuration file.</summary>
    /// <typeparam name="T">The type of the ConfigUtil.</typeparam>
    public class ConfigUtil
    {
        public static ConfigUtil Instance { get; private set; }

        public NameValueCollection AppSettings { get; }

        public ConfigUtil(NameValueCollection appSettings) => this.AppSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        /// <summary>Indicates whether or not SSL is enabled.</summary>
        public bool SSLEnabled => GetAppSettingAsBool("SSLEnabled", false);

        /// <summary>Indicates whether or not cookie (remember me) authentication is enabled.</summary>
        public bool EnableAuthenticationCookie => GetAppSettingAsBool("EnableAuthenticationCookie", false);

        /// <summary>Indicates whether or not email is enabled.</summary>
        public bool EmailDisabled => GetAppSettingAsBool("EmailDisabled", false);

        /// <summary>Indicates a testing email to use for all emails sent from the system.</summary>
        public string OverrideEmailAddress => GetAppSetting("OverrideEmailAddress");

        /// <summary>The email recipient(s) to whom exceptions should be sent.</summary>
        public string[] InternalEmailRecipients => GetAppSettingAsStringArray("InternalEmailRecipients", ';');

        /// <summary>The Google Analytics Account Number to use.</summary>
        public string GoogleAnalyticsAccountNumber => GetAppSetting("GoogleAnalyticsAccountNumber");

        public string QCConnectAPIKey => GetAppSetting("QCConnectAPIKey");
        public string QCConnectAPISecret => GetAppSetting("QCConnectAPISecret");
        public int PageSize => GetAppSettingAsInt("PageSize", 25);

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
            var value = GetAppSetting(key);

            if (value.IsNullOrEmpty())
            {
                return null;
            }

            return value.Split(delimiter);
        }

        /// <summary>Gets the specified AppSetting as a nullable bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public bool? GetAppSettingAsNullableBool(string key)
        {

            if (bool.TryParse(GetAppSetting(key), out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>Gets the specified AppSetting as a bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public bool GetAppSettingAsBool(string key, bool defaultValue)
        {
            var value = GetAppSettingAsNullableBool(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable int.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public int? GetAppSettingAsNullableInt(string key)
        {

            if (int.TryParse(GetAppSetting(key), out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>Gets the specified AppSetting as an int.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public int GetAppSettingAsInt(string key, int defaultValue)
        {
            var value = GetAppSettingAsNullableInt(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable decimal.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public decimal? GetAppSettingAsNullableDecimal(string key)
        {

            if (decimal.TryParse(GetAppSetting(key), out var value))
            {
                return value;
            }

            return null;
        }

        /// <summary>Gets the specified AppSetting as a decimal.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <param name="defaultValue">The value to return if the key does not exist.</param>
        /// <returns>The value of the AppSetting or the default value.</returns>
        public decimal GetAppSettingAsDecimal(string key, decimal defaultValue)
        {
            var value = GetAppSettingAsNullableDecimal(key);

            return value.GetValueOrDefault(defaultValue);
        }

        /// <summary>Gets the specified AppSetting as a nullable bool.</summary>
        /// <param name="key">The key of the AppSetting to get.</param>
        /// <returns>The value of the AppSetting.</returns>
        public DateTime? GetAppSettingAsNullableDateTime(string key)
        {

            if (DateTime.TryParse(GetAppSetting(key), out DateTime value))
            {
                return value;
            }

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

            if (Enum.TryParse<T>(GetAppSetting(key), out T value))
            {
                return value;
            }

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
    }
}
