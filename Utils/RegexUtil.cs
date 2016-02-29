using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Utils
{
    public class RegexUtil
    {
        public enum KnownRegex
        {
            PhoneNumber,
            PostalCode,
            AmericanExpressCreditCard,
            DiscoverCreditCard,
            MastercardCreditCard,
            VisaCreditCard,
            CreditCard,
            EmailAddress,
            Username,
            SSN
        }

        private static Dictionary<KnownRegex, string> KNOWN_REGEX = null;

        private const string Regex_JavaScriptReplace = "\\?<[^>]+>";
        
        private const string Regex_Group_AreaCode = "AreaCode";
        private const string Regex_Group_CityCode = "CityCode";
        private const string Regex_Group_Number = "Number";
        private const string Regex_Group_Extension = "Extension";
        public const string Regex_PhoneNumber = "(?:(?:\\((?<" + Regex_Group_AreaCode + ">\\d{3})\\)\\s(?<" + Regex_Group_CityCode + ">\\d{3})-(?<" + Regex_Group_Number + ">\\d{4})x?(?<" + Regex_Group_Extension + ">\\d{1,6})?)|(?:(?<" + Regex_Group_AreaCode + ">\\d{3})(?<" + Regex_Group_CityCode + ">\\d{3})(?<" + Regex_Group_Number + ">\\d{4})x?(?<" + Regex_Group_Extension + ">\\d{1,6})?))";
        public const string Regex_PhoneNumber_Replacement = "(${" + Regex_Group_AreaCode + "}) ${" + Regex_Group_CityCode + "}-${" + Regex_Group_Number + "}x${" + Regex_Group_Extension + "}";

        private const string Regex_Group_PostalCode = "PostalCode";
        private const string Regex_Group_PlusFour = "PlusFour";
        public const string Regex_PostalCode = "(?:(?:(?<" + Regex_Group_PostalCode + ">\\d{5})(?<" + Regex_Group_PlusFour + ">-\\d{4})?)|((?<" + Regex_Group_PostalCode + ">\\d{5})(?<" + Regex_Group_PlusFour + ">\\d{4})?))";
        public const string Regex_PostalCode_Replacement = "${PostalCode}-${PlusFour}";

        public const string Regex_CreditCard_AmericanExpress = "(34|37)\\d{13}";
        public const string Regex_CreditCard_Discover = "6011\\d{12}";
        public const string Regex_CreditCard_Mastercard = "5[1-5]\\d{14}";
        public const string Regex_CreditCard_Visa = "4\\d{15}";
        public const string Regex_CreditCard = "(" + Regex_CreditCard_AmericanExpress + ")|(" + Regex_CreditCard_Mastercard + ")|(" + Regex_CreditCard_Visa + ")";

        public const string Regex_EmailAddress = "[^@]+@.+\\..{2,}";

        public const string Regex_Username = "^[\\w_]{5,}$";

        public const string Regex_SSN = @"^\d{3}-\d{2}-\d{4}$";

        static RegexUtil()
        {
            KNOWN_REGEX = new Dictionary<KnownRegex, string>();
            KNOWN_REGEX.Add(KnownRegex.PhoneNumber, Regex_PhoneNumber);
            KNOWN_REGEX.Add(KnownRegex.PostalCode, Regex_PostalCode);
            KNOWN_REGEX.Add(KnownRegex.AmericanExpressCreditCard, Regex_CreditCard_AmericanExpress);
            KNOWN_REGEX.Add(KnownRegex.DiscoverCreditCard, Regex_CreditCard_Discover);
            KNOWN_REGEX.Add(KnownRegex.MastercardCreditCard, Regex_CreditCard_Mastercard);
            KNOWN_REGEX.Add(KnownRegex.VisaCreditCard, Regex_CreditCard_Visa);
            KNOWN_REGEX.Add(KnownRegex.CreditCard, Regex_CreditCard);
            KNOWN_REGEX.Add(KnownRegex.EmailAddress, Regex_EmailAddress);
            KNOWN_REGEX.Add(KnownRegex.Username, Regex_Username);
            KNOWN_REGEX.Add(KnownRegex.SSN, Regex_SSN);
        }

        public static Regex GetRegex(KnownRegex knownRegex)
        {
            return new Regex(KNOWN_REGEX[knownRegex]);
        }

        public static string GetRegexString(KnownRegex knownRegex)
        {
            return KNOWN_REGEX[knownRegex];
        }

        public static string GetRegexStringForJavaScript(KnownRegex knownRegex)
        {
            return MakeRegexSafeForJavaScript(KNOWN_REGEX[knownRegex]);
        }

        public static string MakeRegexSafeForJavaScript(string pattern)
        {
            return Regex.Replace(pattern, Regex_JavaScriptReplace, "");
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            return Regex.Replace(phoneNumber, Regex_PhoneNumber, Regex_PhoneNumber_Replacement).Trim();
        }

        public static string UnformattedPhoneNumber(string phoneNumber)
        {
            Match match = Regex.Match(phoneNumber, Regex_PhoneNumber);
            string unformattedPhoneNumber = null;

            if (match.Success)
            {
                string areaCode = match.Groups[Regex_Group_AreaCode].Value;
                string cityCode = match.Groups[Regex_Group_CityCode].Value;
                string number = match.Groups[Regex_Group_Number].Value;
                string extension = match.Groups[Regex_Group_Extension].Value;

                unformattedPhoneNumber = (areaCode + cityCode + number + (string.IsNullOrEmpty(extension) ? "" : "x" + extension));
            }

            return unformattedPhoneNumber;
        }

        public static string GetPhoneExtension(string phoneNumber)
        {
            Match match = Regex.Match(phoneNumber, Regex_PhoneNumber);

            if (match.Success && !string.IsNullOrEmpty(match.Groups[Regex_Group_Extension].Value))
                return match.Groups[Regex_Group_Extension].Value;

            return null;
        }

        public static string FormatPostalCode(string postalCode)
        {
            string formattedPostalCode = Regex.Replace(postalCode, Regex_PostalCode, Regex_PostalCode_Replacement);

            if (string.IsNullOrEmpty(formattedPostalCode))
                return null;

            if (formattedPostalCode.Length < 10)
                return formattedPostalCode.Substring(0, Math.Min(formattedPostalCode.Length, 5));

            return formattedPostalCode;
        }

        public static bool IsValid(string expression)
        {
            if (expression == null)
                return false;

            try
            {
                Regex.Match(string.Empty, expression);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}
