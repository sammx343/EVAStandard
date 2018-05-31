using Redsis.EVA.Client.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EvaPOS
{
    public class InternalSettings
    {

        //public static T GetInternalConfigValue<T>(string name)
        //{
        //    string value = System.Configuration.ConfigurationManager.AppSettings[name].ToString();
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        return (T)Convert.ChangeType(value, typeof(T));
        //    }
        //    else
        //    {
        //        return default(T);
        //    }
        //}

        /// <summary>
        ///  Obtiene el valor asociado a la clave de seguridad para encriptar / desencriptar. Valor: {1ECA74EB-96D1-4140-B7D3-03AFFA0B9C35}
        /// </summary>
        public static string KeyValueForSecurity
        {
            get;
            private set;
        } = (new Guid("1ECA74EB-96D1-4140-B7D3-03AFFA0B9C35")).ToString();


        #region Datafono 
        /// <summary>
        /// Obtiene la longitud establecida para la respuesta de la operación "Compra de Tarjeta"
        /// </summary>
        public static readonly int Length_CompraTarjeta = 23;

        /// <summary>
        /// Obtiene la longitud establecida para la respuesta de la opracion "Anulación de Compra"
        /// </summary>
        public static readonly int Length_AnulacionCompra = 23;

        /// <summary>
        /// Obtiene la longitud establecida para la cantidad de caracteres de un resultado de la lista de cierre integrado
        /// </summary>
        public static readonly int Length_CierreIntegradoItem = 180;

        /// <summary>
        /// 
        /// </summary>
        public static readonly int Fields_CierreIntegrado = 22;

        public static readonly string TipoCuentaTCredito = "CR";

        public static readonly string TipoCuentaPagoManual = "pagomanual";

        public static readonly string[] TipoCuentaTDebito = new string[] { "DB", "EL", "AH", "CO" };

        public static EntidadFinanciera EntidadDatafono = EntidadFinanciera.NONE;

        public static bool MultipleMonitor = false;

        public static string ConfigFileRedeban = "config.sip";

        public static int Length_NumeroRecibo = 6;

        #endregion

        #region Auto Update

        /// <summary>
        /// Obtiene o establece el nombre de la tabla Azure Storage que almacena la información de productos disponibles.
        /// </summary>
        public static string ATS_ProductoVersiones = "ProductoVersiones";

        /// <summary>
        /// Obtiene el nombre de la tabla de azure storage que almancena la información de autorizaciones de actualizaciones del aplicativo.
        /// </summary>
        public static string ATS_ActualizacionTerminales = "ActualizacionTerminales";

        /// <summary>
        /// Obtiene o establece el nombre del archivo que contiene la información de actualiación de la aplicación.
        /// </summary>
        public static string UpdateInfoFileName = "UpdateInfo.json";
        #endregion

        #region ViewMode

        public static readonly string ModoConsola = "consola";
        public static readonly string ModoTouch = "touch";

        #endregion

        #region Paths

        public static readonly string PathPrincipal = @"C:\EVA";
        public static readonly string ImagenesFolder = "Imagenes";
        public static readonly string FilesFolder = "Files";
        public static readonly string PixelImage = "pack://application:,,,/EVA Client 2;component/Resources/Images/FFFFFF-0.png";
        public static readonly string ConfigFileName = "config.xml";
        public static readonly string DispositivosFileName = "dispositivos.xml";
        public static readonly string TecladoTactilFileName = "TecladoTactil.xml";

        #endregion

        #region devices

        public static readonly string OPOSPrinter = "opos";
        public static readonly string FiscalPrinter = "fiscal";

        public static readonly string PAVOScanner = "pavo";
        public static readonly string OPOSScanner = "opos";

        public static double ActualWindowHeight = 0;
        public static double ActualWindowWidth = 0;

        #endregion

        #region FormatoValores

        public static string ThousandSeparator { get; set; }

        public static string DecimalSeparator { get; set; }

        public static string CurrencySymbol { get; set; }

        public static int DecimalLimit { get; set; }

        public static int WholeNumberLimit { get; set; }

        public static bool CurrencySymbolFlag { get; set; }

        public static bool DecimalFlag { get; set; }

        #endregion

    }
}
