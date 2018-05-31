using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace EvaPOS
{
    public static class StringHelper
    {
        static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        public static string ConvertHexToString(string hexString)
        {
            string result = "";
            string s2 = hexString.Replace(" ", "");
            for (int i = 0; i < s2.Length; i++)
            {
                result += Convert.ToChar(int.Parse(s2.Substring(i, 2), System.Globalization.NumberStyles.HexNumber));
            }
            return result;

            //var bytes = new byte[hexString.Length / 2];
            //for (var i = 0; i < bytes.Length; i++)
            //{
            //    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            //}

            //return Encoding.ASCII.GetString(bytes);
        }

        public static string ConvertStringToFormatNumber(string value, bool isCurrency, bool useThousandsLimit = true)
        {
            string ans = "";

            try
            {

                string thousandSeparator = InternalSettings.ThousandSeparator;
                string decimalSeparator = InternalSettings.DecimalSeparator;
                string currencySymbol = InternalSettings.CurrencySymbol;
                int decimalLimit = InternalSettings.DecimalLimit;
                int wholeNumberLimit = InternalSettings.WholeNumberLimit;
                bool currencySymbolFlag = InternalSettings.CurrencySymbolFlag;
                bool decimalFlag = InternalSettings.DecimalFlag;

                //Inicio Convert
                string Amount = value.ToString();

                if (currencySymbolFlag)
                {
                    if (Amount.CompareTo(currencySymbol) == 0)
                        return string.Empty;
                }

                if (!decimalFlag)
                {
                    int intPartValue = 0;
                    if (int.TryParse(Amount, out intPartValue))
                    {
                        decimal mathPartValue = Math.Truncate((decimal)intPartValue);
                        Amount = mathPartValue.ToString();
                    }
                }

                if (Amount.CompareTo(decimalSeparator) == 0)
                    return string.Empty;

                string[] amountSplit = new string[2];

                if (decimalSeparator.Length > 0)
                {
                    decimal intAmount = 0;
                    if (decimal.TryParse(Amount, out intAmount))
                    {
                        amountSplit[0] = Math.Truncate(intAmount).ToString();
                        amountSplit[1] = (intAmount - Math.Truncate(intAmount)).ToString();
                    }
                }
                else
                {
                    amountSplit = new string[1] { Amount };
                }

                string wholeNumber = amountSplit[0];
                wholeNumber = new Regex($@"[^(\-)?(\d)|{decimalSeparator}]").Replace(wholeNumber, string.Empty);

                if (useThousandsLimit)
                    wholeNumber = wholeNumber.Substring(0, Math.Min(wholeNumber.Length, wholeNumberLimit));


                if (new Regex(@"[0]").Matches(wholeNumber).Count == wholeNumber.Length)
                {
                    wholeNumber = "0";
                }

                string formattedWholeNumber = Regex.Replace(wholeNumber, @"\d{1,3}(?=(\d{3})+(?!\d))", $"$&{thousandSeparator}");

                //Eliminar separador decimal en caso que se digite más de uno
                string decimalPart = "";

                if (decimalFlag)
                {
                    if (amountSplit.Length >= 2)
                    {

                        //decimalPart = new Regex($@"[^\d|]").Replace(amountSplit[1], string.Empty);
                        Regex regex = new Regex("[1-9]+");
                        Match match = regex.Match(amountSplit[1]);
                        if (match.Success)
                        {
                            decimalPart = match.Value;
                        }

                        //
                        if (decimalPart.Length < decimalLimit)
                        {
                            int cantChars = (decimalLimit - decimalPart.Length);
                            cantChars = (decimalPart.Length + cantChars);
                            decimalPart = decimalPart.PadRight(cantChars, '0');
                        }

                        //
                        decimalPart = decimalSeparator + decimalPart.Substring(0, Math.Min(decimalPart.Length, decimalLimit));

                        //
                        if (amountSplit.Length > 2)
                            Amount = Amount.Remove(Amount.Length - 1);
                    }
                }

                string formattedAmount = formattedWholeNumber + decimalPart;


                if (isCurrency)
                {
                    if (currencySymbolFlag)
                    {
                        int indexMin = formattedAmount.IndexOf('-');
                        if (indexMin != -1)
                        {
                            if (indexMin == 0)
                            {
                                formattedAmount = formattedAmount.Insert(1, currencySymbol);
                            }
                        }
                        else
                        {
                            formattedAmount = currencySymbol + formattedAmount;
                        }
                    }
                }

                ans = formattedAmount;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ToCustomCurrencyFormat] {0}", ex.Message);
            }

            return ans;
        }

        public static string ConvertToCustomCurrencyFromEntry(this string value)
        {
            string ans = "";

            try
            {
                if (string.IsNullOrEmpty(value.ToString()))
                    return string.Empty;
                else
                {

                    string thousandSeparator = InternalSettings.ThousandSeparator;
                    string decimalSeparator = InternalSettings.DecimalSeparator;
                    string currencySymbol = InternalSettings.CurrencySymbol;
                    int decimalLimit = InternalSettings.DecimalLimit;
                    int wholeNumberLimit = InternalSettings.WholeNumberLimit;
                    bool currencySymbolFlag = InternalSettings.CurrencySymbolFlag;
                    bool decimalFlag = InternalSettings.DecimalFlag;


                    //Inicio Convert
                    string Amount = value.ToString();

                    if (currencySymbolFlag)
                    {
                        if (Amount.CompareTo(currencySymbol) == 0)
                            return string.Empty;
                    }

                    //
                    if (Amount.Contains(currencySymbol))
                    {
                        Amount = Amount.Replace(currencySymbol, "");
                    }

                    //
                    if (Amount.Contains(thousandSeparator))
                    {
                        Amount = Amount.Replace(thousandSeparator, "");

                        if (Amount.Contains(decimalSeparator))
                        {
                            string[] amountTemp = Amount.Split(decimalSeparator.ToCharArray());
                            Amount = amountTemp[0];
                        }
                    }

                    //
                    if (!decimalFlag)
                    {
                        //
                        decimal intPartValue = 0;
                        if (decimal.TryParse(Amount, out intPartValue))
                        {
                            decimal mathPartValue = Math.Truncate(intPartValue);
                            Amount = mathPartValue.ToString();
                        }
                    }
                    else
                    {
                        if (Amount.Contains(decimalSeparator))
                        {
                            Amount = Amount.Replace(decimalSeparator, "");
                            int lenDecimal = Amount.Length - decimalLimit;
                            Amount = Amount.Substring(0, lenDecimal);
                        }
                    }

                    //
                    if (Amount.CompareTo(decimalSeparator) == 0)
                        return string.Empty;

                    string[] amountSplit = new string[2];

                    if (decimalSeparator.Length > 0)
                    {
                        decimal intAmount = 0;
                        if (decimal.TryParse(Amount, out intAmount))
                        {
                            amountSplit[0] = Math.Truncate(intAmount).ToString();
                            amountSplit[1] = (intAmount - Math.Truncate(intAmount)).ToString();
                        }
                    }
                    else
                    {
                        amountSplit = new string[1] { Amount };
                    }

                    string wholeNumber = amountSplit[0];

                    // validar longitud del campo
                    if (wholeNumber.Length > wholeNumberLimit)
                    {
                        wholeNumber = wholeNumber.Substring(0, wholeNumber.Length - 1);
                    }

                    wholeNumber = new Regex($@"[^(\-)?(\d)|{decimalSeparator}]").Replace(wholeNumber, string.Empty);
                    wholeNumber = wholeNumber.Substring(0, Math.Min(wholeNumber.Length, wholeNumberLimit));

                    if (new Regex(@"[0]").Matches(wholeNumber).Count == wholeNumber.Length)
                    {
                        wholeNumber = "0";
                    }

                    string formattedWholeNumber = Regex.Replace(wholeNumber, @"\d{1,3}(?=(\d{3})+(?!\d))", $"$&{thousandSeparator}");

                    //Eliminar separador decimal en caso que se digite más de uno
                    string decimalPart = "";

                    if (decimalFlag)
                    {
                        if (amountSplit.Length >= 2)
                        {
                            decimalPart = new Regex($@"[^\d|]").Replace(amountSplit[1], string.Empty);
                            if (decimalPart.Length < decimalLimit)
                            {
                                int cantChars = (decimalLimit - decimalPart.Length);
                                cantChars = (decimalPart.Length + cantChars);
                                decimalPart = decimalPart.PadRight(cantChars, '0');
                            }

                            //
                            decimalPart = decimalSeparator + decimalPart.Substring(0, Math.Min(decimalPart.Length, decimalLimit));

                            //

                            if (amountSplit.Length > 2)
                                Amount = Amount.Remove(Amount.Length - 1);
                        }
                    }


                    //string formattedAmount = formattedWholeNumber + decimalPart;
                    string formattedAmount = formattedWholeNumber;

                    if (currencySymbolFlag)
                    {
                        int indexMin = formattedAmount.IndexOf('-');
                        if (indexMin != -1)
                        {
                            if (indexMin == 0)
                            {
                                formattedAmount = formattedAmount.Insert(1, currencySymbol);
                            }
                        }
                        else
                        {
                            formattedAmount = currencySymbol + formattedAmount;
                        }
                    }

                    ans = formattedAmount;
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ConvertToCustomCurrencyFromEntry] {0}", ex.Message);
            }

            return ans;
        }

        public static double ConvertToDoubleFromCustomCurrency(this string value)
        {
            double ans = 0;

            try
            {

                string thousandSeparator = InternalSettings.ThousandSeparator;
                string decimalSeparator = InternalSettings.DecimalSeparator;
                string currencySymbol = InternalSettings.CurrencySymbol;
                int decimalLimit = InternalSettings.DecimalLimit;
                int wholeNumberLimit = InternalSettings.WholeNumberLimit;
                bool currencySymbolFlag = InternalSettings.CurrencySymbolFlag;
                bool decimalFlag = InternalSettings.DecimalFlag;

                CultureInfo culture = new CultureInfo("es-CO");
                culture.NumberFormat.CurrencyDecimalSeparator = decimalSeparator;
                culture.NumberFormat.CurrencyDecimalDigits = decimalLimit;
                culture.NumberFormat.CurrencySymbol = currencySymbol;
                culture.NumberFormat.CurrencyGroupSeparator = thousandSeparator;
                culture.NumberFormat.PerMilleSymbol = thousandSeparator;

                if (!double.TryParse(value, NumberStyles.Currency, culture, out ans))
                {
                    log.Warn("El valor ingresado no tiene un formato correcto.");
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ToCustomCurrencyFormat] {0}", ex.Message);
            }

            return ans;
        }

        /// <summary>
        /// Convierte una cadena de texto a un formato de moneda según configuración de parametros de servidor.
        /// </summary>
        /// <param name="valueString">Debe ser un valor convertible a decimal</param>
        /// <returns></returns>
        public static string ConvertToCustomCurrencyFromCode(this string valueString)
        {
            string ans = "";
            ans = ConvertStringToFormatNumber(valueString, true);
            return ans;

            //try
            //{
            //    if (string.IsNullOrEmpty(valueString.ToString()))
            //        return string.Empty;
            //    else
            //    {

            //        string thousandSeparator = InternalSettings.ThousandSeparator;
            //        string decimalSeparator = InternalSettings.DecimalSeparator;
            //        string currencySymbol = InternalSettings.CurrencySymbol;
            //        int decimalLimit = InternalSettings.DecimalLimit;
            //        int wholeNumberLimit = InternalSettings.WholeNumberLimit;
            //        bool currencySymbolFlag = InternalSettings.CurrencySymbolFlag;
            //        bool decimalFlag = InternalSettings.DecimalFlag;

            //        //Inicio Convert
            //        string Amount = valueString.ToString();

            //        if (currencySymbolFlag)
            //        {
            //            if (Amount.CompareTo(currencySymbol) == 0)
            //                return string.Empty;
            //        }

            //        //
            //        if (!decimalFlag)
            //        {
            //            //
            //            decimal intPartValue = 0;
            //            if (decimal.TryParse(Amount, out intPartValue))
            //            {
            //                decimal mathPartValue = Math.Truncate(intPartValue);
            //                Amount = mathPartValue.ToString();
            //            }
            //        }

            //        //
            //        if (Amount.CompareTo(decimalSeparator) == 0)
            //            return string.Empty;

            //        string[] amountSplit = new string[2];

            //        if (decimalSeparator.Length > 0)
            //        {
            //            decimal intAmount = 0;
            //            if (decimal.TryParse(Amount, out intAmount))
            //            {
            //                amountSplit[0] = Math.Truncate(intAmount).ToString();
            //                amountSplit[1] = (intAmount - Math.Truncate(intAmount)).ToString();
            //            }
            //        }
            //        else
            //        {
            //            amountSplit = new string[1] { Amount };
            //        }

            //        string wholeNumber = amountSplit[0];
            //        wholeNumber = new Regex($@"[^(\-)?(\d)|{decimalSeparator}]").Replace(wholeNumber, string.Empty);
            //        wholeNumber = wholeNumber.Substring(0, Math.Min(wholeNumber.Length, wholeNumberLimit));

            //        if (new Regex(@"[0]").Matches(wholeNumber).Count == wholeNumber.Length)
            //        {
            //            wholeNumber = "0";
            //        }

            //        string formattedWholeNumber = Regex.Replace(wholeNumber, @"\d{1,3}(?=(\d{3})+(?!\d))", $"$&{thousandSeparator}");

            //        //Eliminar separador decimal en caso que se digite más de uno
            //        string decimalPart = "";

            //        if (decimalFlag)
            //        {
            //            if (amountSplit.Length >= 2)
            //            {
            //                decimalPart = new Regex($@"[^\d|]").Replace(amountSplit[1], string.Empty);
            //                if (decimalPart.Length < decimalLimit)
            //                {
            //                    int cantChars = (decimalLimit - decimalPart.Length);
            //                    cantChars = (decimalPart.Length + cantChars);
            //                    decimalPart = decimalPart.PadRight(cantChars, '0');
            //                }

            //                //
            //                decimalPart = decimalSeparator + decimalPart.Substring(0, Math.Min(decimalPart.Length, decimalLimit));

            //                //

            //                if (amountSplit.Length > 2)
            //                    Amount = Amount.Remove(Amount.Length - 1);
            //            }
            //        }


            //        string formattedAmount = formattedWholeNumber + decimalPart;

            //        if (currencySymbolFlag)
            //        {
            //            int indexMin = formattedAmount.IndexOf('-');
            //            if (indexMin != -1)
            //            {
            //                if (indexMin == 0)
            //                {
            //                    formattedAmount = formattedAmount.Insert(1, currencySymbol);
            //                }
            //            }
            //            else
            //            {
            //                formattedAmount = currencySymbol + formattedAmount;
            //            }
            //        }

            //        ans = formattedAmount;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    log.ErrorFormat("[ConvertToCustomCurrencyFromCode] {0}", ex.Message);
            //}

            //return ans;
        }

        public static IEnumerable<String> SplitInParts(String s, Int32 partLength)
        {
            if (s == null)
            {
                throw new ArgumentNullException("[s] es nulo o vació");
            }
            if (partLength <= 0)
            {
                throw new ArgumentException("Part length has to be positive.", "partLength");
            }

            //
            for (var i = 0; i < s.Length; i += partLength)
            {
                yield return s.Substring(i, Math.Min(partLength, s.Length - i)).Remove(0, 1);
            }
        }

        public static string ClearString(this string value)
        {
            string val = value.Replace("\n", "");
            val = val.Replace("\t", "");
            val = val.Trim();
            return val;
        }

    }
}
