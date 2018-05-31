using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Helpers
{
    public class Decimales
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static double ConvertirValorDecimal(string valor)
        {
            double precio = 0;
            try
            {
                var val = Entorno.Instancia.Parametros.Parametro("pdv.caracter_decimal");
                var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
                if (val != null)
                {
                    switch (val.ToString())
                    {
                        case ".":
                            ci.NumberFormat.NumberDecimalSeparator = ".";
                            ci.NumberFormat.NumberGroupSeparator = ",";
                            break;
                        case ",":
                            valor = valor.Replace(".", ",");
                            ci.NumberFormat.NumberDecimalSeparator = ",";
                            ci.NumberFormat.NumberGroupSeparator = ".";
                            break;
                    }
                    precio = double.Parse(valor, ci);
                }
                else
                {
                    ci.NumberFormat.NumberDecimalSeparator = ".";
                    ci.NumberFormat.NumberGroupSeparator = ",";
                    precio = double.Parse(valor, ci);
                }
            }
            catch (Exception)
            {

            }
            return precio;
        }

        public static double RedondearValores(string valor, bool isFiscalPrinter = false)
        {
            double val = 0;
            if (string.IsNullOrEmpty(valor))
                return 0;

            var permi = Entorno.Instancia.Parametros.Parametro("pdv.mostrar_decimales_precio_venta");
            if (permi != null)
            {
                if (Convert.ToBoolean(permi))
                {
                    if (isFiscalPrinter)
                        val = Math.Round(Convert.ToDouble(valor), 4);
                    else
                        val = Math.Round(Convert.ToDouble(valor), 2);
                }
                else if (isFiscalPrinter)
                    val = Math.Round(Convert.ToDouble(valor), 4);
                else
                {
                    val = Math.Round(Convert.ToDouble(valor), 0);
                }
            }
            else
            {
                val = Math.Round(Convert.ToDouble(valor), 0);
            }

            return val;
        }

        public static string RedondearValoresVisuales(string valor, bool isFiscalPrinter = false)
        {
            string val = "";
            try
            {

                if (string.IsNullOrEmpty(valor))
                    return "0";

                var permi = Entorno.Instancia.Parametros.Parametro("pdv.mostrar_decimales_precio_venta");
                if (permi != null)
                {
                    if (Convert.ToBoolean(permi.Valor))
                    {
                        if (isFiscalPrinter)
                        {
                            double valorTotal = Convert.ToDouble(valor);
                            double valorRedondeado = Math.Round(valorTotal, 2, MidpointRounding.AwayFromZero);
                            val = valorRedondeado.ToCustomCurrencyFormat();
                        }
                        else
                            val = Convert.ToDouble(valor).ToCustomCurrencyFormat();

                    }
                    else if (isFiscalPrinter)
                        val = Convert.ToDouble(valor).ToCustomCurrencyFormat();
                    else
                    {
                        val = Convert.ToDouble(valor).ToCustomCurrencyFormat();
                    }
                }
                else
                {
                    val = Convert.ToDouble(valor).ToCustomCurrencyFormat();
                }

            }
            catch (Exception ex)
            {
                log.ErrorFormat("[RedondearValoresVisuales] {0}", ex.Message);
            }
            return val;
        }

        internal static object ConvertirValorDecimalC(string valor, bool usarDecimales = false)
        {
            string precio = "0";
            try
            {
                var val = Entorno.Instancia.Parametros.Parametro("pdv.caracter_decimal");
                var ci = CultureInfo.InvariantCulture.Clone() as CultureInfo;
                if (val != null)
                {
                    switch (val.ToString())
                    {
                        case ".":
                            ci.NumberFormat.NumberDecimalSeparator = ".";
                            ci.NumberFormat.NumberGroupSeparator = ",";

                            break;
                        case ",":
                            valor = valor.Replace(".", ",");
                            ci.NumberFormat.NumberDecimalSeparator = ",";
                            ci.NumberFormat.NumberGroupSeparator = ".";
                            break;
                    }
                    var dec = Decimal.Parse(valor);
                    precio = dec.ToString("N", ci);
                }
                else
                {
                    ci.NumberFormat.NumberDecimalSeparator = ".";
                    ci.NumberFormat.NumberGroupSeparator = ",";
                    var dec = Decimal.Parse(valor);

                    if (usarDecimales)
                        precio = dec.ToString("N", ci);
                    else
                        precio = dec.ToString("N0");

                }
            }
            catch (Exception)
            {

            }
            return precio;
        }
    }
}
