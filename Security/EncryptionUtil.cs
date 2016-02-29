using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace QuantumConcepts.Common.Security {
    /// <summary>Provides common functionlity related to encryption.</summary>
    public abstract class EncryptionUtil {
        /// <summary>The RijndaelManaged instance that will be used to encrypt/decrypt text via Rijndael.</summary>
        protected readonly AesCryptoServiceProvider AesServiceProvider = new AesCryptoServiceProvider();

        /// <summary>The 32-bit encryption key to use for AES/Rijndael encryption.</summary>
        protected abstract byte[] EncryptionKey { get; }

        /// <summary>Returns the text encrypted via SHA1.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <returns>The encrypted text.</returns>
        public string EncryptTextViaSHA1(string text) {
            return EncryptText(text, new SHA1CryptoServiceProvider());
        }

        /// <summary>Returns the text encrypted via MD5.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <returns>The encrypted text.</returns>
        public string EncryptTextViaMD5(string text) {
            return EncryptText(text, new MD5CryptoServiceProvider());
        }

        /// <summary>Encyrpts the specified text using the specified hash algorithm.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="algorithm">The algorithm to use to encrypt the text.</param>
        /// <returns>The encrypted text.</returns>
        public string EncryptText(string text, HashAlgorithm algorithm) {
            UTF8Encoding encoding = new UTF8Encoding();
            byte[] bytes = algorithm.ComputeHash(encoding.GetBytes(text));

            return Convert.ToBase64String(bytes);
        }

        /// <summary>Generates an encryption vector to use for the AES/Rijndael encryption algorithm.</summary>
        /// <returns>An encryption vector.</returns>
        public static byte[] GenerateEncryptionVector() {
            RijndaelManaged rm = new RijndaelManaged();

            rm.GenerateIV();

            return rm.IV;
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector used for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        /// <remarks>This overload does not expect an encryption vector. It will be generated, used, and then returned as an out parameter.</remarks>
        public string EncryptTextViaRijndael(string text, out string iv) {
            byte[] vector = GenerateEncryptionVector();
            byte[] encrypted = EncryptTextViaRijndael(text, vector);

            iv = Convert.ToBase64String(vector);

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector used for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public byte[] EncryptTextViaRijndael(string text, out byte[] iv) {
            iv = GenerateEncryptionVector();

            return EncryptTextViaRijndael(text, iv);
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector used for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public string EncryptTextViaRijndaelAsString(string text, out byte[] iv) {
            iv = GenerateEncryptionVector();

            return Convert.ToBase64String(EncryptTextViaRijndael(text, iv));
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector to use for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public string EncryptTextViaRijndael(string text, string iv, bool shouldPadIV = true) {
            byte[] ivToUse;

            if (shouldPadIV)
                ivToUse = PadIV(iv);
            else
                ivToUse = Convert.FromBase64String(iv);

            return Convert.ToBase64String(EncryptTextViaRijndael(text, ivToUse));
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector to use for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public string EncryptTextViaRijndaelAsString(string text, byte[] iv) {
            return Convert.ToBase64String(EncryptTextViaRijndael(text, iv, this.EncryptionKey));
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector to use for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public byte[] EncryptTextViaRijndael(string text, byte[] iv) {
            return EncryptTextViaRijndael(text, iv, this.EncryptionKey);
        }

        /// <summary>Returns the text encrypted via AES/Rijndael.</summary>
        /// <param name="text">The text to encrypt.</param>
        /// <param name="iv">The encryption vector to use for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public byte[] EncryptTextViaRijndael(string text, byte[] iv, byte[] key) {
            byte[] bytes = (new UTF8Encoding()).GetBytes(text);
            byte[] encryptedBytes = null;

            using (MemoryStream inStream = new MemoryStream(bytes))
            using (MemoryStream outStream = new MemoryStream()) {
                EncryptViaRijndael(inStream, outStream, iv, key);
                encryptedBytes = outStream.ToArray();
            }

            return encryptedBytes;
        }

        /// <summary>Returns the input stream encrypted via AES/Rijndael.</summary>
        /// <param name="inStream">The stream to encrypt.</param>
        /// <param name="outStream">The stream containing the result of the encryption.</param>
        /// <param name="iv">The encryption vector to use for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public void EncryptViaRijndael(Stream inStream, Stream outStream, byte[] iv) {
            EncryptViaRijndael(inStream, outStream, iv, this.EncryptionKey);
        }

        /// <summary>Returns the input stream encrypted via AES/Rijndael.</summary>
        /// <param name="inStream">The stream to encrypt.</param>
        /// <param name="outStream">The stream containing the result of the encryption.</param>
        /// <param name="iv">The encryption vector to use for the encryption.</param>
        /// <returns>The encrypted text.</returns>
        public void EncryptViaRijndael(Stream inStream, Stream outStream, byte[] iv, byte[] key) {
            ICryptoTransform transform = this.AesServiceProvider.CreateEncryptor(key, iv);
            CryptoStream encryptStream = new CryptoStream(outStream, transform, CryptoStreamMode.Write);

            inStream.CopyTo(encryptStream);
            encryptStream.FlushFinalBlock();
        }

        /// <summary>Decrypts the provided text using the provided encryption vector.</summary>
        /// <param name="text">The encrypted text to decrypt.</param>
        /// <param name="iv">The encryption vector which was used to encrypt the text.</param>
        /// <returns>The decrypted text.</returns>
        public string DecryptTextViaRijndael(string text, string iv, bool shouldPadIV = true) {
            byte[] ivToUse;

            if (shouldPadIV)
                ivToUse = PadIV(iv);
            else
                ivToUse = Convert.FromBase64String(iv);

            return DecryptTextViaRijndael(Convert.FromBase64String(text), ivToUse);
        }

        /// <summary>Decrypts the provided text using the provided encryption vector.</summary>
        /// <param name="text">The encrypted text to decrypt.</param>
        /// <param name="iv">The encryption vector which was used to encrypt the text.</param>
        /// <returns>The decrypted text.</returns>
        public string DecryptTextViaRijndael(string text, byte[] iv) {
            return DecryptTextViaRijndael(Convert.FromBase64String(text), iv);
        }

        /// <summary>Decrypts the provided text using the provided encryption vector.</summary>
        /// <param name="text">The encrypted text to decrypt.</param>
        /// <param name="iv">The encryption vector which was used to encrypt the text.</param>
        /// <returns>The decrypted text.</returns>
        public string DecryptTextViaRijndael(byte[] text, byte[] iv) {
            return DecryptTextViaRijndael(text, iv, this.EncryptionKey);
        }

        /// <summary>Decrypts the provided text using the provided encryption vector.</summary>
        /// <param name="bytes">The encrypted text to decrypt.</param>
        /// <param name="iv">The encryption vector which was used to encrypt the text.</param>
        /// <returns>The decrypted text.</returns>
        public string DecryptTextViaRijndael(byte[] bytes, byte[] iv, byte[] key) {
            byte[] decryptedBytes = null;

            using (MemoryStream inStream = new MemoryStream(bytes))
            using (MemoryStream outStream = new MemoryStream()) {
                DecryptViaRijndael(inStream, outStream, iv, key);
                decryptedBytes = outStream.ToArray();
            }

            return (new UTF8Encoding()).GetString(decryptedBytes, 0, decryptedBytes.Length);
        }

        /// <summary>Decrypts the provided input stream using the provided encryption vector.</summary>
        /// <param name="inStream">The encrypted stream to decrypt.</param>
        /// <param name="outStream">The stream containing the result of the encryption.</param>
        /// <param name="iv">The encryption vector which was used to encrypt the stream.</param>
        /// <returns>The decrypted text.</returns>
        public void DecryptViaRijndael(Stream inStream, Stream outStream, byte[] iv) {
            DecryptViaRijndael(inStream, outStream, iv, this.EncryptionKey);
        }

        /// <summary>Decrypts the provided input stream using the provided encryption vector.</summary>
        /// <param name="inStream">The encrypted stream to decrypt.</param>
        /// <param name="outStream">The stream containing the result of the encryption.</param>
        /// <param name="iv">The encryption vector which was used to encrypt the stream.</param>
        /// <returns>The decrypted text.</returns>
        public void DecryptViaRijndael(Stream inStream, Stream outStream, byte[] iv, byte[] key) {
            ICryptoTransform transform = this.AesServiceProvider.CreateDecryptor(key, iv);
            CryptoStream decryptStream = new CryptoStream(outStream, transform, CryptoStreamMode.Write);

            inStream.CopyTo(decryptStream);
            decryptStream.FlushFinalBlock();
        }

        protected byte[] PadIV(string iv) {
            const string safeChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+";

            int length = (this.AesServiceProvider.BlockSize / 8);
            string fixedIV = iv;
            StringBuilder paddedIV = null;
            Random random = new Random(length);

            //Only use up to the first 8 chars of the IV.
            if (fixedIV.Length > length)
                fixedIV = fixedIV.Substring(0, length);

            //Remove any characters which are not valid for a Base64 string.
            fixedIV = Regex.Replace(fixedIV, "[^a-z0-9+/]", "", RegexOptions.IgnoreCase);

            //Pad the string to it is the correct length, and chop the remainder.
            paddedIV = new StringBuilder(fixedIV);

            for (int i = paddedIV.Length; i < length; i++)
                paddedIV.Append(safeChars[random.Next(0, safeChars.Length)]);

            return Encoding.UTF8.GetBytes(paddedIV.ToString());
        }
    }
}