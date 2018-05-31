using EvaPOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Redsis.EVA.Client.Common
{
    public class EncryptionUtil
    {
        //protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Cifra una cadena de texto. 
        /// Valida si el descfriado es posible.
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string Encrypt(string encryptString)
        {
            string EncryptionKey = InternalSettings.KeyValueForSecurity;
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        //cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }


        /// <summary>
        /// Cifra una cadna de texto.
        /// </summary>
        /// <param name="encryptString"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        public static bool TryEncrypt<T>(string encryptString, out T resultValue)
        {
            bool isValid = true;
            resultValue = default(T);

            try
            {
                //
                string encryptedValue = Encrypt(encryptString);

                //
                resultValue = (T)Convert.ChangeType(encryptedValue, typeof(T));
            }
            catch (Exception ex)
            {
                isValid = false;
                //log.ErrorFormat("[TryEncrypt]   {0}", ex.Message);
            }
            return isValid;
        }

        /// <summary>
        /// Descifra una cadena de texto.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = InternalSettings.KeyValueForSecurity;
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        //cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        /// <summary>
        /// Descifra una cadena de texto. 
        /// Valida si el descfriado es posible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cipherText"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        public static bool TryDecrypt<T>(string cipherText, out T resultValue)
        {
            bool converted = false;
            resultValue = default(T);

            try
            {
                //
                string decryptValue = Decrypt(cipherText);

                //
                resultValue = (T)Convert.ChangeType(decryptValue, typeof(T));
                converted = true;
            }
            catch (Exception ex)
            {
                //log.ErrorFormat("[TryDecrypt]   {0}", ex.Message);
            }

            return converted;
        }
    }
}
