using EvaPOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Redsis.EVA.Client.Common
{
    public class Config
    {
        #region Propiedades

        //protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Diccionario de todas las configuraciones usadas por la aplicación
        Dictionary<string, string[]> listConfig = new Dictionary<string, string[]>();


        public static string Terminal { get; private set; } = "";

        public static string ViewMode { get; set; }

        public static string MediosPago { get; set; }

        public static string CodigoPais { get; set; }

        public static bool UsaTecladoOPOS { get; set; } = false;

        public static bool SiempreActiva { get; set; } = true;

        #endregion

        /// <summary>
        /// Representa las configuraciones registradas en el archivo config.xml
        /// </summary>
        private Config()
        {
            try
            {
                string pathConfig = @"C:\Eva\Files";
                string configPath = System.IO.Path.Combine(pathConfig, "config.xml");

                //
                XDocument xmlFile = XDocument.Load(configPath);

                //Se cargan las configuraciones del archivo.
                var query = (from c in xmlFile.Elements("config").Elements()
                             select c).ToList();


                Terminal = "100003";
                //
                ViewMode = query.Where(c => c.Name == "ViewMode").FirstOrDefault().Value.ClearXmlValueString().ClearString();
                //
                CodigoPais = query.Where(c => c.Name == "codigopais").FirstOrDefault().Value.ClearXmlValueString().ClearString();
                //
                MediosPago = "MediosPago";
                //
                bool siempreActiva = true;
                string alwaysOn = query.Where(c => c.Name == "SiempreActiva").FirstOrDefault().Value.ClearXmlValueString().ClearString();
                if (bool.TryParse(alwaysOn, out siempreActiva))
                {
                    SiempreActiva = siempreActiva;
                }
                else
                {
                    //Log.Warn("[Config.Load] no se pudo cargar el valor para el parámetro \"SiempreActiva\"");
                }

                //log.Info("[Config.Load] Configuraciones inicializadas correctamente");
            }
            catch (Exception ex)
            {
                //log.ErrorFormat("[Config].Load {0}", ex.Message);
            }
        }

        private static Dictionary<string, string[]> LoadDictionary()
        {
            //Diccionario de todas las configuraciones usadas por la aplicación
            Dictionary<string, string[]> listConfig = new Dictionary<string, string[]>();
            //
            listConfig.Add("terminal", new string[] { "Eva", "Raiz de url de servicios web EVA" });
            //
            listConfig.Add("ViewMode", new string[] { "touch", "Establece el modo de diseño de la interfaz gráfica. (Modo consola [consola], Modo touch [touch]" });
            //
            listConfig.Add("MediosPago", new string[] { "", "Lista de medios de pago" });
            //
            listConfig.Add("codigopais", new string[] { "CO", "Codigo de país. [ISO Alpha-2]" });
            //
            listConfig.Add("SiempreActiva", new string[] { "true", "Indica si la aplicación se forzará a estar siempre delante en el escritorio de windows." });

            return listConfig;
        }

        public static void ValidConfig()
        {
            //Diccionario de datos con las configuraciones.
            Dictionary<string, string[]> listConfig = LoadDictionary();

            //validación del archivo en sistema de archivos.
            try
            {
                string pathConfig = Path.Combine(InternalSettings.PathPrincipal, InternalSettings.FilesFolder);
                string configPath = System.IO.Path.Combine(pathConfig, InternalSettings.ConfigFileName);

                //
                bool needSave = false;

                XDocument xmlFile = XDocument.Load(configPath);
                XDocument xmlFileBackup = XDocument.Load(configPath);

                //Configuraciones registradas en archivo físico
                var query = (from c in xmlFile.Elements("config").Elements()
                             select c).ToList();
                foreach (KeyValuePair<string, string[]> configKey in listConfig)
                {
                    try
                    {
                        //Se valida que la clave exista en el archivo de configuración.
                        bool existsKey = query.Exists(e => e.Name == configKey.Key);

                        //Si la clave no existe se crea con un valor por defecto.
                        if (!existsKey)
                        {
                            XComment comment = new XComment(configKey.Value[1]);
                            XElement element = new XElement(configKey.Key);
                            element.Value = configKey.Value[0].ClearXmlValueString().ClearString();

                            //
                            xmlFile.Element("config").Add(comment);
                            xmlFile.Element("config").Add(element);

                            //
                            needSave = true;

                            //
                            //log.DebugFormat("Se crea parametro de configuración: \"{0}\" / \"{1}\"", configKey.Key, configKey.Value[0].ClearXmlValueString().ClearString());

                        }
                    }
                    catch (Exception exQ)
                    {
                        //log.ErrorFormat("[ValidConfig.query] {0}", exQ.Message);
                    }
                }

                //guarda los cambios del archivo de configuración
                if (needSave)
                {
                    //
                    //xmlFile.Save(configPath);

                    //
                    //log.DebugFormat("[ValidConfig] Archivo de configuración [{0}] actualizado", configPath);
                }

                new Config();
            }
            catch (Exception ex)
            {
                //log.ErrorFormat("[ValidConfig] {0}", ex.Message);
            }
        }
    }
}
