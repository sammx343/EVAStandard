using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Linq;
using System.Data;

namespace EvaPOS
{
    public static class Extensions
    {
        
        public static bool IsNullOrEmptyList<T>(this List<T> list)
        {
            bool isNullOrEmpty = true;

            //
            if (list != null)
            {
                if (list.Count > 0)
                {
                    isNullOrEmpty = false;
                }
            }

            //
            return isNullOrEmpty;
        }

        public static bool IsNullOrEmptyTable(this DataTable dt)
        {
            bool isNullOrEmpty = true;

            //
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    isNullOrEmpty = false;
                }
            }

            //
            return isNullOrEmpty;
        }

        public static bool IsNullOrEmptyList<T>(this ObservableCollection<T> list)
        {
            bool isNullOrEmpty = true;

            //
            if (list != null)
            {
                if (list.Count > 0)
                {
                    isNullOrEmpty = false;
                }
            }

            //
            return isNullOrEmpty;
        }

        public static bool IsNullOrEmptyList<T>(this IEnumerable<T> list)
        {
            bool isNullOrEmpty = true;

            //
            if (list != null)
            {
                if (list.OfType<T>().Count() > 0)
                {
                    isNullOrEmpty = false;
                }
            }

            //
            return isNullOrEmpty;
        }

        //public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) where TAttribute : Attribute
        //{
        //    var att = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;
        //    if (att != null)
        //    {
        //        return valueSelector(att);
        //    }

        //    return default(TValue);
        //}

        public static string ClearXmlValueString(this string value)
        {
            string ans = value;

            ans = ans.Replace("\t", "");
            ans = ans.Replace("\n", "");

            return ans;
        }


        public static string GetEnumName(this Enums.EstadosFSM value)
        {
            string enumName = Enum.GetName(typeof(Enums.EstadosFSM), value);
            return enumName;
        }

        public static string GetEnumName(this Enums.TransicionesFSM value)
        {
            string enumName = Enum.GetName(typeof(Enums.TransicionesFSM), value);
            return enumName;
        }

        public static string ToStringDefinition(this Array array)
        {
            string def = "";

            if (array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < array.Length)
                    {
                        def += array.GetValue(i).ToString() + ",";
                    }
                    else
                    {
                        def += array.GetValue(i).ToString();
                    }
                }
            }

            return def;
        }

        //public static string GetDescriptionValue<T>(this T val)
        //{
        //    DescriptionAttribute[] attributes = (DescriptionAttribute[])val.GetType().GetField(val.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
        //    return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        //}

        public static string ToDescriptionString(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

           if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
           else
              return value.ToString();
        }

        public static string Left(this string str, int length)
        {
            str = (str ?? string.Empty);
            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string Right(this string str, int length)
        {
            str = (str ?? string.Empty);
            return (str.Length >= length)
                ? str.Substring(str.Length - length, length)
                : str;
        }

        public static double ToSeconds(this long ms)
        {
            return TimeSpan.FromMilliseconds(ms).TotalSeconds;
        }

        /// <summary>
        /// Convierte un valor decimal a formato moneda según parametrización de servidor.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToCustomCurrencyFormat<T>(this T value, bool useThousandsLimit = true)
        {
            string Amount = value.ToString();
            string ans = StringHelper.ConvertStringToFormatNumber(Amount, true, useThousandsLimit: useThousandsLimit);
            return ans;

        }

        public static string ToCustomNumberFormat<T>(this T value, bool useThousandsLimit = true)
        {
            string Amount = value.ToString();
            string ans = StringHelper.ConvertStringToFormatNumber(Amount, false, useThousandsLimit: useThousandsLimit);
            return ans;

        }

        //public static DateTime ToDateTimeFromDouble(this string value)
        //{
        //    double d = double.Parse(value);
        //    DateTime conv = DateTime.
        //        DateTime.FromOADate(d);
        //    return conv;
        //}

        public static double UnformatCurrencyValue(this string value)
        {
            double valorPago = 0;

            string convertValue = value;
            convertValue = convertValue.Replace(InternalSettings.CurrencySymbol, "");
            convertValue = convertValue.Replace(InternalSettings.ThousandSeparator, "");

            if (InternalSettings.DecimalFlag)
            {
                if (convertValue.Contains(InternalSettings.DecimalSeparator))
                {
                    string[] amount = new string[2];
                    amount = convertValue.Split(InternalSettings.DecimalSeparator.ToCharArray());
                    convertValue = amount[0];
                }
            }

            double.TryParse(convertValue, out valorPago);

            return valorPago;
        }

        public static string ListToString<T>(this IList<T> list)
        {
            return string.Join(Environment.NewLine, list.ToArray());
        }

        public static string ListToString<T>(this IList<T> list, string separator)
        {
            return string.Join(separator, list.ToArray());
        }
    }
}
