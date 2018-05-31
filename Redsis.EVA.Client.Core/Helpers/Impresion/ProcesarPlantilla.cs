using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;
using Mustache;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Helpers.Impresion;
//using Redsis.EVA.Client.Dispositivos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Redsis.EVA.Client.Core.Helpers
{
    [System.Runtime.InteropServices.Guid("09E9B525-5B56-4CCA-B0D9-AE815ACF461E")]
    public class ProcesarPlantilla
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const string nombreMedioPagoDevolucion = "Efectivo";

        #region Factura

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string Factura(EVenta Venta, ETerminal Terminal, EUsuario Usuario)
        {
            // get template file
            string ticketFinal = string.Empty;
            try
            {
                string plantilla = LeerPlantilla("plantilla.txt");

                // get template file
                XElement xelement = XElement.Load("C:\\Eva\\Files\\Ticket.xml");
                XDocument doc = XDocument.Parse(xelement.ToString());
                IEnumerable<XElement> tickets =
                    from t in doc.Element("tickets").Elements("ticket")
                    where (string)t.Attribute("tipo") == TipoTicket.Factura.ToString()
                    select t;

                log.Debug("[ProcesarPlantilla.Factura] Armando factura de venta ...");
                Tirilla tick = new Tirilla();

                tick.Encabezado = ArmarEncabezado(tickets, Terminal);

                // get printer brand                
                tick.Encabezado.impresoraNCR = Entorno.Instancia.Impresora.Marca != null ? Entorno.Instancia.Impresora.Marca.ToUpper().Contains("NCR") : false;

                // build detail
                List<string> listLineaDetalle = new List<string>();

                // Obtener valor de parámetro que indica si es una factura consolidada.
                bool esFacturaConsolidada = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.impresion_factura_consolidada");
                List<EItemVenta> listArticulos = esFacturaConsolidada ? Venta.tirillaLimpia() : Venta.CopiaTirilla;

                tick.Detalle = ArmarDetalle(listArticulos, ref listLineaDetalle);


                tick.listLineaDetalle = listLineaDetalle;
                bool implementaImpuestoCompuesto;
                if (!bool.TryParse(Entorno.Instancia.Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
                {
                    implementaImpuestoCompuesto = false;
                }

                if (implementaImpuestoCompuesto)
                {
                    tick.listImpuesto = ArmarImpuestos(Venta.ImpuestosIncluidosCompuestos);
                }
                else
                {
                    tick.listImpuesto = ArmarImpuestos(Venta.ImpuestosIncluidos);

                }

                if (tick.listImpuesto.Any())
                {
                    tick.TotImpuesto = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.Total_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    tick.TotBase = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.Base_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    //tick.TotBase = decimales.RedondearValoresVisuales((tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.BaseT))).ToString());
                    tick.TotIva = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.IVA_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    tick.TotImtoString = "TOTAL".PadRight(8, ' ');
                }

                // build footer
                tick.Pie = ArmarPie(tickets, Venta);

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(tick);
                ticketFinal = actual;

                //Terminal.

                log.Info("[ProcesarPlantilla.Factura] Factura: " + ticketFinal);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Factura] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }

            return ticketFinal ?? string.Empty;
        }

        public static string Factura(EDevolucion Devolucion, ETerminal Terminal, EUsuario Usuario)
        {
            // get template file
            string ticketFinal = string.Empty;
            try
            {
                string plantilla = LeerPlantilla("plantilla.txt");

                // get template file
                XElement xelement = XElement.Load("C:\\Eva\\Files\\Ticket.xml");
                XDocument doc = XDocument.Parse(xelement.ToString());
                IEnumerable<XElement> tickets =
                    from t in doc.Element("tickets").Elements("ticket")
                    where (string)t.Attribute("tipo") == TipoTicket.Factura.ToString()
                    select t;

                log.Debug("[ProcesarPlantilla.Factura] Armando factura de devolución ...");
                Tirilla tick = new Tirilla();

                tick.Encabezado = ArmarEncabezado(tickets, Terminal);

                // get printer brand                
                tick.Encabezado.impresoraNCR = Entorno.Instancia.Impresora.Marca != null ? Entorno.Instancia.Impresora.Marca.ToUpper().Contains("NCR") : false;

                // build detail
                List<string> listLineaDetalle = new List<string>();

                // Obtener valor de parámetro que indica si es una factura consolidada.
                bool esFacturaConsolidada = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.impresion_factura_consolidada");
                List<EItemVenta> listArticulos = esFacturaConsolidada ? Devolucion.tirillaLimpia() : Devolucion.CopiaTirilla;

                tick.Detalle = ArmarDetalle(listArticulos, ref listLineaDetalle);

                tick.listLineaDetalle = listLineaDetalle;

                bool implementaImpuestoCompuesto;
                if (!bool.TryParse(Entorno.Instancia.Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
                {
                    implementaImpuestoCompuesto = false;
                }
                if (implementaImpuestoCompuesto)
                {
                    tick.listImpuesto = ArmarImpuestos(Devolucion.ImpuestosIncluidosCompuestos);
                }
                else
                {
                    tick.listImpuesto = ArmarImpuestos(Devolucion.ImpuestosIncluidos);
                }
                if (tick.listImpuesto.Any())
                {
                    tick.TotImpuesto = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.Total_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    tick.TotBase = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.Base_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    //tick.TotBase = decimales.RedondearValoresVisuales((tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.BaseT))).ToString());
                    tick.TotIva = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.IVA_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    tick.TotImtoString = "TOTAL".PadRight(8, ' ');
                }

                // build footer
                tick.Pie = ArmarPie(tickets, Devolucion);

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(tick);
                ticketFinal = actual;

                //Terminal.

                log.Info("[ProcesarPlantilla.Factura] Factura: " + ticketFinal);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Factura] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }

            return ticketFinal ?? string.Empty;
        }

        public static string Factura(EFacturaVentaEspecialSinMedioPago EFVESMP, ETerminal Terminal, EUsuario Usuario)
        {
            // get template file
            string ticketFinal = string.Empty;
            try
            {
                string plantilla = LeerPlantilla("plantilla.txt");


                // get template file
                XElement xelement = XElement.Load("C:\\Eva\\Files\\Ticket.xml");
                XDocument doc = XDocument.Parse(xelement.ToString());
                IEnumerable<XElement> tickets =
                    from t in doc.Element("tickets").Elements("ticket")
                    where (string)t.Attribute("tipo") == TipoTicket.Factura.ToString()
                    select t;

                log.Debug("[ProcesarPlantilla.Venta] Armando factura");
                Tirilla tick = new Tirilla();

                tick.Encabezado = ArmarEncabezado(tickets, Terminal);

                // get printer brand                
                tick.Encabezado.impresoraNCR = Entorno.Instancia.Impresora.Marca != null ? Entorno.Instancia.Impresora.Marca.ToUpper().Contains("NCR") : false;

                tick.Encabezado.DescTipoVenta = EFVESMP.TipoVentaEspecial.Descripcion;

                // build detail
                List<string> listLineaDetalle = new List<string>();
                tick.Detalle = ArmarDetalle(EFVESMP.CopiaTirilla, ref listLineaDetalle);
                tick.listLineaDetalle = listLineaDetalle;

                bool implementaImpuestoCompuesto;
                if (!bool.TryParse(Entorno.Instancia.Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
                {
                    implementaImpuestoCompuesto = false;
                }
                if (implementaImpuestoCompuesto)
                {
                    tick.listImpuesto = ArmarImpuestos(EFVESMP.ImpuestosIncluidosCompuestos);
                }
                else
                {
                    tick.listImpuesto = ArmarImpuestos(EFVESMP.ImpuestosIncluidos);
                }
                if (tick.listImpuesto.Any())
                {
                    tick.TotImpuesto = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.Total_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    tick.TotBase = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.Base_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    //tick.TotBase = decimales.RedondearValoresVisuales((tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.BaseT))).ToString());
                    tick.TotIva = tick.listImpuesto.Sum((Impuesto si) => Convert.ToDouble(si.IVA_Dec)).ToCustomNumberFormat().PadLeft(12, ' ');
                    tick.TotImtoString = "TOTAL".PadRight(8, ' ');
                }

                // build footer
                tick.Pie = ArmarPie(tickets, EFVESMP);

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(tick);
                ticketFinal = actual;

                //Terminal.

                log.Info("[ProcesarPlantilla.Factura] Factura: " + ticketFinal);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Factura] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }

            return ticketFinal ?? string.Empty;
        }

        internal static string Ajuste(EAjuste Ajuste, ETerminal Terminal, EUsuario Usuario, string nombreAjuste = null)
        {
            // get template file
            string ticketFinal = string.Empty;
            try
            {
                string plantilla = LeerPlantilla("plantilla_ajuste.txt");
                bool usaImpresoraNCR = Entorno.Instancia.Impresora.Marca != null ? Entorno.Instancia.Impresora.Marca.ToUpper().Contains("NCR") : false;

                log.Debug("[ProcesarPlantilla.Factura] Armando factura de ajuste ...");
                Ajuste tirillaAjuste = new Ajuste();
                tirillaAjuste.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura");
                tirillaAjuste.Mensaje = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.pie_factura");

                tirillaAjuste.Cajero = Entorno.Instancia.Usuario.Nombre;
                tirillaAjuste.Local = Terminal.Localidad.Nombre;
                tirillaAjuste.TipoAjuste = nombreAjuste ?? Entorno.Instancia.Vista.ModalAjustes.DescripcionAjuste;

                // get printer brand                
                tirillaAjuste.impresoraNCR = usaImpresoraNCR;

                // build detail
                List<string> listLineaDetalle = new List<string>();
                tirillaAjuste.Detalle = ArmarDetalle(Ajuste.CopiaTirilla, ref listLineaDetalle);
                tirillaAjuste.LineaDetalle = listLineaDetalle;

                tirillaAjuste.MTotal = "Total";
                tirillaAjuste.Total = Ajuste.TotalVenta.ToCustomCurrencyFormat();
                tirillaAjuste.POS = Terminal.Codigo;
                tirillaAjuste.Fecha = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                // build footer
                //tick.Pie = ArmarPie(tickets, Ajuste);

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(tirillaAjuste);
                ticketFinal = actual;

                //Terminal.

                log.Info("[ProcesarPlantilla.Factura] Factura: " + ticketFinal);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Factura] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }

            return ticketFinal ?? string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tickets"></param>
        /// <param name="venta"></param>
        /// <returns></returns>
        private static Pie ArmarPie(IEnumerable<XElement> tickets, EVenta venta)
        {
            Pie pieFactura = new Pie();
            try
            {
                //
                IEnumerable<XElement> piex = tickets.Elements("pie");
                pieFactura.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura");
                pieFactura.Mensaje = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.pie_factura");

                //
                pieFactura.MensajeResolucion = (string)piex.Elements("ResDian").FirstOrDefault<XElement>();
                pieFactura.MRango = (string)piex.Elements("Rango").FirstOrDefault<XElement>();
                pieFactura.MTotal = ((string)piex.Elements("Total").FirstOrDefault<XElement>()).PadLeft(20, ' ') + new string(" ".ToCharArray()).PadRight(3, ' ');
                pieFactura.MNFactura = (string)piex.Elements("Tiquete").FirstOrDefault<XElement>();
                pieFactura.MCambio = ((string)piex.Elements("Cambio").FirstOrDefault<XElement>()).PadLeft(20, ' ') + new string(" ".ToCharArray()).PadRight(3, ' ');
                pieFactura.MFechaExp = (string)piex.Elements("FechaExp").FirstOrDefault<XElement>() + " " + DateTime.Now.ToShortDateString();
                pieFactura.Total = Decimales.RedondearValoresVisuales(venta.TotalVenta.ToString()).PadLeft(14, ' ');

                pieFactura.Resolucion = string.Format("{0} {1}",
                    Entorno.Instancia.Terminal.NumeroAutorizacionFacturacion
                    , Entorno.Instancia.Terminal.FechaAutorizacionFacturacion.ToShortDateString());

                pieFactura.Rango = string.Format("Prefijo {0} desde {1} hasta {2}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.PrimeraFactura, Entorno.Instancia.Terminal.FacturaFinal);
                pieFactura.Local = Entorno.Instancia.Terminal.Localidad.Nombre;
                pieFactura.DirLocal = Entorno.Instancia.Terminal.Localidad.Direccion;
                pieFactura.FechaHora = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                pieFactura.NFactura = string.Format("{0} - {1}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.NumeroUltimaFactura + 1);
                pieFactura.Cambio = Decimales.RedondearValoresVisuales(Convert.ToString(venta.PorPagar * -1)).PadLeft(14, ' ');
                pieFactura.Cajero = Entorno.Instancia.Usuario.Nombre;
                pieFactura.TRX = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);
                pieFactura.Terminal = Entorno.Instancia.Terminal.Codigo;

                //
                if (venta.Cliente != null)
                {
                    pieFactura.ClienteActivo = true;
                    pieFactura.ClienteId = venta.Cliente.Codigo;
                    pieFactura.ClienteNombre = venta.Cliente.PrimerNombre + " " + venta.Cliente.PrimerApellido;
                    pieFactura.ClienteDir = "";
                    pieFactura.ClienteTel = "";
                }

                // pagos            
                List<DetalleTipoPago> list = new List<DetalleTipoPago>();
                if (venta.Pagos.Any())
                {
                    foreach (var item in venta.Pagos)
                    {
                        DetalleTipoPago tipoPago = new DetalleTipoPago();
                        tipoPago.nombre = item.MedioPago.Tipo.PadLeft(20, ' ') + new string(" ".ToCharArray()).PadRight(3, ' ');
                        tipoPago.valor = Decimales.RedondearValoresVisuales(item.Valor.ToString()).PadLeft(14, ' ');
                        list.Add(tipoPago);
                    }
                }


                pieFactura.listTPago = list;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarPie] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

            return pieFactura;
        }

        private static Pie ArmarPie(IEnumerable<XElement> tickets, EDevolucion devolucion)
        {
            Pie pieFactura = new Pie();
            try
            {
                //
                IEnumerable<XElement> piex = tickets.Elements("pie");
                pieFactura.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura");
                pieFactura.Mensaje = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.pie_factura");

                //
                pieFactura.MensajeResolucion = (string)piex.Elements("ResDian").FirstOrDefault<XElement>();
                pieFactura.MRango = (string)piex.Elements("Rango").FirstOrDefault<XElement>();
                pieFactura.MTotal = ((string)piex.Elements("Total").FirstOrDefault<XElement>()).PadLeft(20, ' ');
                pieFactura.MNFactura = (string)piex.Elements("Tiquete").FirstOrDefault<XElement>();
                pieFactura.MCambio = ((string)piex.Elements("Cambio").FirstOrDefault<XElement>()).PadLeft(20, ' '); ;
                pieFactura.MFechaExp = (string)piex.Elements("FechaExp").FirstOrDefault<XElement>() + " " + DateTime.Now.ToShortDateString();
                pieFactura.Total = Decimales.RedondearValoresVisuales(devolucion.TotalVenta.ToString()).PadLeft(14, ' ');

                pieFactura.Resolucion = string.Format("{0} {1}",
                    Entorno.Instancia.Terminal.NumeroAutorizacionFacturacion
                    , Entorno.Instancia.Terminal.FechaAutorizacionFacturacion.ToShortDateString());

                pieFactura.Rango = string.Format("Prefijo {0} desde {1} hasta {2}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.PrimeraFactura, Entorno.Instancia.Terminal.FacturaFinal);
                pieFactura.Local = Entorno.Instancia.Terminal.Localidad.Nombre;
                pieFactura.FechaHora = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                pieFactura.NFactura = string.Format("{0} - {1}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.NumeroUltimaFactura + 1);
                pieFactura.Cambio = Decimales.RedondearValoresVisuales((-devolucion.TotalVenta).ToString()).PadLeft(14, ' ');
                pieFactura.Cajero = Entorno.Instancia.Usuario.Nombre;
                pieFactura.TRX = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);
                pieFactura.Terminal = Entorno.Instancia.Terminal.Codigo;

                //
                if (devolucion.Cliente != null)
                {
                    pieFactura.ClienteActivo = true;
                    pieFactura.ClienteId = devolucion.Cliente.Codigo;
                    pieFactura.ClienteNombre = devolucion.Cliente.PrimerNombre + " " + devolucion.Cliente.PrimerApellido;
                    pieFactura.ClienteDir = "";
                    pieFactura.ClienteTel = "";
                }

                // pagos            
                List<DetalleTipoPago> list = new List<DetalleTipoPago>();
                DetalleTipoPago tipoPago = new DetalleTipoPago();
                tipoPago.nombre = nombreMedioPagoDevolucion.PadLeft(20, ' ');
                tipoPago.valor = Decimales.RedondearValoresVisuales("0").PadLeft(14, ' ');
                list.Add(tipoPago);


                pieFactura.listTPago = list;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarPie] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

            return pieFactura;
        }

        private static Pie ArmarPie(IEnumerable<XElement> tickets, EAjuste ajuste)
        {
            Pie pieFactura = new Pie();
            try
            {
                //
                IEnumerable<XElement> piex = tickets.Elements("pie");
                pieFactura.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura");
                pieFactura.Mensaje = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.pie_factura");

                //
                pieFactura.MensajeResolucion = (string)piex.Elements("ResDian").FirstOrDefault<XElement>();
                pieFactura.MRango = (string)piex.Elements("Rango").FirstOrDefault<XElement>();
                pieFactura.MTotal = ((string)piex.Elements("Total").FirstOrDefault<XElement>()).PadLeft(20, ' ');
                pieFactura.MNFactura = (string)piex.Elements("Tiquete").FirstOrDefault<XElement>();
                pieFactura.MCambio = ((string)piex.Elements("Cambio").FirstOrDefault<XElement>()).PadLeft(20, ' '); ;
                pieFactura.MFechaExp = (string)piex.Elements("FechaExp").FirstOrDefault<XElement>() + " " + DateTime.Now.ToShortDateString();
                pieFactura.Total = Decimales.RedondearValoresVisuales(ajuste.TotalVenta.ToString()).PadLeft(14, ' ');

                pieFactura.Resolucion = string.Format("{0} {1}",
                    Entorno.Instancia.Terminal.NumeroAutorizacionFacturacion
                    , Entorno.Instancia.Terminal.FechaAutorizacionFacturacion.ToShortDateString());

                pieFactura.Rango = string.Format("Prefijo {0} desde {1} hasta {2}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.PrimeraFactura, Entorno.Instancia.Terminal.FacturaFinal);
                pieFactura.Local = Entorno.Instancia.Terminal.Localidad.Nombre;
                pieFactura.FechaHora = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                pieFactura.NFactura = string.Format("{0} - {1}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.NumeroUltimaFactura + 1);
                pieFactura.Cambio = Decimales.RedondearValoresVisuales(Convert.ToString(0)).PadLeft(14, ' ');
                pieFactura.Cajero = Entorno.Instancia.Usuario.Nombre;
                pieFactura.TRX = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);
                pieFactura.Terminal = Entorno.Instancia.Terminal.Codigo;


                // pagos            
                List<DetalleTipoPago> list = new List<DetalleTipoPago>();
                DetalleTipoPago tipoPago = new DetalleTipoPago();
                tipoPago.nombre = nombreMedioPagoDevolucion.PadLeft(20, ' ');
                tipoPago.valor = Decimales.RedondearValoresVisuales(ajuste.TotalVenta.ToString()).PadLeft(14, ' ');
                list.Add(tipoPago);


                pieFactura.listTPago = list;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarPie] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

            return pieFactura;
        }

        private static Pie ArmarPie(IEnumerable<XElement> tickets, EFacturaVentaEspecialSinMedioPago EFVESMP)
        {
            Pie pieFactura = new Pie();
            try
            {
                //
                IEnumerable<XElement> piex = tickets.Elements("pie");
                pieFactura.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura");
                pieFactura.Mensaje = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.pie_factura");

                //
                pieFactura.MensajeResolucion = (string)piex.Elements("ResDian").FirstOrDefault<XElement>();
                pieFactura.MRango = (string)piex.Elements("Rango").FirstOrDefault<XElement>();
                pieFactura.MTotal = ((string)piex.Elements("Total").FirstOrDefault<XElement>()).PadLeft(20, ' ');
                pieFactura.MNFactura = (string)piex.Elements("Tiquete").FirstOrDefault<XElement>();
                pieFactura.MCambio = ((string)piex.Elements("Cambio").FirstOrDefault<XElement>()).PadLeft(20, ' ');
                pieFactura.MFechaExp = (string)piex.Elements("FechaExp").FirstOrDefault<XElement>() + " " + DateTime.Now.ToShortDateString();
                pieFactura.Total = Decimales.RedondearValoresVisuales(EFVESMP.TotalVenta.ToString()).PadLeft(14, ' ');

                pieFactura.Resolucion = string.Format("{0} {1}",
                    Entorno.Instancia.Terminal.NumeroAutorizacionFacturacion
                    , Entorno.Instancia.Terminal.FechaAutorizacionFacturacion.ToShortDateString());

                pieFactura.Rango = string.Format("Prefijo {0} desde {1} hasta {2}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.PrimeraFactura, Entorno.Instancia.Terminal.FacturaFinal);
                pieFactura.Local = Entorno.Instancia.Terminal.Localidad.Nombre;
                pieFactura.FechaHora = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                pieFactura.NFactura = string.Format("{0} - {1}", Entorno.Instancia.Terminal.Prefijo, Entorno.Instancia.Terminal.NumeroUltimaFactura + 1);
                pieFactura.Cambio = Decimales.RedondearValoresVisuales(Convert.ToString(0)).PadLeft(14, ' ');
                pieFactura.Cajero = Entorno.Instancia.Usuario.Nombre;
                pieFactura.TRX = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);
                pieFactura.Terminal = Entorno.Instancia.Terminal.Codigo;
                pieFactura.MensajeTipoVenta = EFVESMP.TipoVentaEspecial.Mensaje;

                //
                if (EFVESMP.Cliente != null)
                {
                    pieFactura.ClienteActivo = true;
                    pieFactura.ClienteId = EFVESMP.Cliente.Codigo;
                    pieFactura.ClienteNombre = EFVESMP.Cliente.PrimerNombre + " " + EFVESMP.Cliente.PrimerApellido;
                    pieFactura.ClienteDir = "";
                    pieFactura.ClienteTel = "";
                }

                // pagos            
                List<DetalleTipoPago> list = new List<DetalleTipoPago>();
                DetalleTipoPago tipoPago = new DetalleTipoPago();
                tipoPago.nombre = nombreMedioPagoDevolucion.PadLeft(20, ' ');
                tipoPago.valor = Decimales.RedondearValoresVisuales(EFVESMP.TotalVenta.ToString()).PadLeft(14, ' ');
                list.Add(tipoPago);


                pieFactura.listTPago = list;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarPie] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

            return pieFactura;
        }

        private static List<Impuesto> ArmarImpuestos(Dictionary<EImpuestosArticulo, List<decimal>> impuestosVenta)
        {
            bool esFacturaConsolidada = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.impresion_factura_consolidada");


            List<Impuesto> listImpuestos = new List<Impuesto>();
            try
            {
                foreach (var item in impuestosVenta)
                {
                    bool imprimeImpuesto = true;

                    ////Valida si debe imprimir el detalle de los impuestos aún cuando éste esté en cero (0), generalmente se da cuando se realica una cancelación de productos.
                    //if ((item.Value.Sum() > 0) | (Reactor.Instancia.EstadoFSMActual == Common.Enums.EstadosFSM.TerminarDevolucion))
                    //{
                    //    imprimeImpuesto = true;
                    //}
                    //else if (!esFacturaConsolidada)
                    //{
                    //    imprimeImpuesto = true;
                    //}
                    //else
                    //{
                    //    var x = Reactor.Instancia.EstadoFSMActual;
                    //    log.Error($"[Factura]    la suma de impuestos es menor o igua a cero y la el parámetro pdv.impresion_factura_consolidada tiene un valor de \"true\"");
                    //}
                    ////

                    //
                    if (imprimeImpuesto)
                    {
                        Impuesto impto = new Impuesto();

                        string nombre = item.Key.Identificador;
                        var porcentaje = item.Key.Porcentaje;
                        var valor = item.Key.Valor;
                        decimal compra = item.Value[0];
                        decimal iva = item.Value[1];
                        decimal baseGenerada = item.Value[2];

                        //
                        impto.Base_Dec = baseGenerada;
                        impto.IVA_Dec = iva;
                        impto.Total_Dec = compra;

                        // detalle
                        if (nombre.Length > 2)
                            nombre = nombre.Substring(0, 2);

                        impto.Id = nombre;
                        impto.Porcentaje = item.Key.TipoImpuesto == 1 ? string.Format("{0}={1}%", impto.Id, porcentaje).PadRight(8, ' ') : "";
                        impto.Porcentaje = item.Key.TipoImpuesto == 2 ? string.Format("{0}=${1}", impto.Id, (int)valor).PadRight(8, ' ') : impto.Porcentaje;
                        impto.Total = compra.ToCustomNumberFormat().PadLeft(12, ' ');
                        impto.Base = baseGenerada.ToCustomNumberFormat().PadLeft(12, ' ');
                        impto.IVA = iva.ToCustomNumberFormat().PadLeft(12, ' ');

                        // totales
                        impto.BaseT = baseGenerada.ToCustomNumberFormat();
                        impto.IVAT = iva.ToCustomNumberFormat();
                        impto.TotalT = compra.ToCustomNumberFormat();


                        listImpuestos.Add(impto);
                    }
                }


            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarImpuestos] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }
            return listImpuestos;
        }

        private static List<Impuesto> ArmarImpuestos(Dictionary<EImpuesto, List<decimal>> impuestosVenta)
        {
            bool esFacturaConsolidada = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.impresion_factura_consolidada");
            List<Impuesto> listImpuestos = new List<Impuesto>();
            try
            {
                foreach (var item in impuestosVenta)
                {
                    bool imprimeImpuesto = true;

                    ////Valida si debe imprimir el detalle de los impuestos aún cuando éste esté en cero (0), generalmente se da cuando se realica una cancelación de productos.
                    //if ((item.Value.Sum() > 0) | (Reactor.Instancia.EstadoFSMActual == Common.Enums.EstadosFSM.TerminarDevolucion))
                    //{
                    //    imprimeImpuesto = true;
                    //}
                    //else if (!esFacturaConsolidada)
                    //{
                    //    imprimeImpuesto = true;
                    //}
                    //else
                    //{
                    //    var x = Reactor.Instancia.EstadoFSMActual;
                    //    log.Error($"[Factura]    la suma de impuestos es menor o igua a cero y la el parámetro pdv.impresion_factura_consolidada tiene un valor de \"true\"");
                    //}
                    ////

                    if (imprimeImpuesto)
                    {
                        Impuesto impto = new Impuesto();

                        string nombre = item.Key.Identificador;
                        var porcentaje = item.Key.Porcentaje;
                        decimal compra = item.Value[0];
                        decimal iva = item.Value[1];
                        decimal baseGenerada = item.Value[2];

                        //
                        impto.Base_Dec = baseGenerada;
                        impto.IVA_Dec = iva;
                        impto.Total_Dec = compra;

                        // detalle
                        impto.Id = nombre;
                        impto.Porcentaje = string.Format("{0}={1}%", impto.Id, porcentaje).PadRight(8, ' ');
                        impto.Total = compra.ToCustomNumberFormat().PadLeft(12, ' ');
                        impto.Base = baseGenerada.ToCustomNumberFormat().PadLeft(12, ' ');
                        impto.IVA = iva.ToCustomNumberFormat().PadLeft(12, ' ');

                        // totales
                        impto.BaseT = baseGenerada.ToCustomNumberFormat();
                        impto.IVAT = iva.ToCustomNumberFormat();
                        impto.TotalT = compra.ToCustomNumberFormat();

                        listImpuestos.Add(impto);
                    }
                }


            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarImpuestos] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
            return listImpuestos;
        }

        private static List<DetalleFactura> ArmarDetalle(List<EItemVenta> tirilla, ref List<string> listLineaDetalle)
        {
            //TODO: construir listado de impuestos a imprimir a partir de los impuestos asociados a los artículos vendidos.

            // build detail
            List<DetalleFactura> listDetalle = new List<DetalleFactura>();
            try
            {
                listLineaDetalle = new List<string>();
                if (tirilla.Any())
                {
                    bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");

                    //TODO: validar la iteración para que continue en caso de error.
                    foreach (EItemVenta d in tirilla)
                    {
                        try
                        {
                            string impuesto = "ND";
                            if (implementaImpuestoCompuesto)
                            {
                                log.Info($"El artículo {d.Articulo.CodigoImpresion} implementa impuestocompuesto.");

                                //
                                var impuestoArticulo = d.Articulo.Impuestos.FirstOrDefault(i => i.Descripcion.Contains("INC"));
                                if (impuestoArticulo != null)
                                {
                                    log.Warn($"El artículo {d.Articulo.CodigoImpresion} contiene más de un impuesto configurado, se elige el impuesto con descripción \"INC\".");
                                    impuesto = impuestoArticulo.Identificador;
                                }
                                else
                                {
                                    log.Warn($"No se encontró impuesto asociado con el nombre \"INC\", se elige el primer impuesto configurado que coincida con el porcentaje de impuesto asociado al artículo {d.Articulo.CodigoImpresion}.");

                                    //
                                    impuestoArticulo = d.Articulo.Impuestos.FirstOrDefault();
                                    if (impuestoArticulo != null)
                                    {
                                        //log.Warn($"El artículo {d.Articulo.CodigoImpresion} contiene más de un impuesto configurado, se elige el impuesto con descripción \"INC\".");
                                        impuesto = impuestoArticulo.Identificador;
                                    }
                                    else
                                    {
                                        log.Error($"Lista de impuestos vacía. art.: {d.Articulo.DescripcionLarga}");
                                    }
                                    
                                }
                            }
                            else
                            {
                                //
                                var impArticulo = Entorno.Instancia.Impuestos.Impuestos.FirstOrDefault(x => x.Porcentaje == (float)d.Articulo.Impuesto1);
                                if (impArticulo != null)
                                {
                                    impuesto = impArticulo.Identificador;
                                }
                                else
                                {
                                    log.Error($"No se encontró impuesto asociado al porcentaje {d.Articulo.Impuesto1} del artículo {d.Articulo.CodigoImpresion} en el listado de impuestos configurados.");
                                }
                            }

                            //
                            if (impuesto.Length > 2)
                                impuesto = impuesto.Substring(0, 2);

                            #region Crear detalle para copia de impresión
                            DetalleFactura detalle = new DetalleFactura();

                            string descArticulo = d.Articulo.DescripcionCorta;

                            if (descArticulo.Length < 16)
                            {
                                int spacesCount = Math.Abs(descArticulo.Length - 16);
                                descArticulo = descArticulo.PadRight(20, ' ');

                            }

                            detalle.Descripcion = descArticulo;
                            detalle.Cantidad = d.Cantidad.ToString();
                            detalle.Codigo = d.Articulo.CodigoImpresion;
                            detalle.Peso = d.Peso.ToString();
                            detalle.Valor = d.Valor.ToCustomCurrencyFormat().PadLeft(12, ' ') + " " + impuesto;
                            detalle.SubTotal = d.Articulo.PrecioVenta1.ToCustomCurrencyFormat();

                            if (Convert.ToDouble(detalle.Peso) > 0)
                            {
                                detalle.PesoReq = true;
                                detalle.Peso = detalle.Peso;
                            }

                            if (Convert.ToInt32(detalle.Cantidad) > 1 || Convert.ToInt32(detalle.Cantidad) < 1)
                            {
                                detalle.ValCantidad = true;
                            }

                            if (!string.IsNullOrEmpty(detalle.Dscto))
                            {
                                detalle.BValorDscto = true;
                                detalle.ValorDscto = "-" + Decimales.RedondearValoresVisuales((detalle.Dscto).ToString()).PadLeft(13, ' ') + " ";
                                var sub = Convert.ToDouble(detalle.SubTotal) * Convert.ToDouble(detalle.Cantidad);
                                //double valor = sub + double.Parse(detalle.Dscto);
                                double valor = sub;
                                detalle.Valor = Decimales.RedondearValoresVisuales((valor).ToString()).PadLeft(12, ' ') + " " + impuesto;

                            }

                            if (!string.IsNullOrEmpty(detalle.Tdscto))
                            {
                                //detalle.BtipoDscto = App.DescuentoGeneral ? false : true;
                                detalle.tipoDscto = detalle.Tdscto;
                            }

                            listDetalle.Add(detalle);
                            #endregion

                            #region Crear detalle para impresión

                            string formatPrint = "";
                            string formatPrintCant = "";
                            if (Config.ViewMode == InternalSettings.ModoConsola)
                            {
                                if (InternalSettings.DecimalFlag)
                                {
                                    formatPrint = "{0,-16}{1,-23}{2,17}";
                                    formatPrintCant = "{0,-16}{1,-23}{2,17}";
                                }
                                else
                                {
                                    formatPrint = "{0,-16}{1,-23}{2,17}";
                                    formatPrintCant = "{0,-16}{1,-23}{2,17}";
                                }

                                //
                                if (Entorno.Instancia.Devolucion != null)
                                {
                                    if (Math.Abs(d.Cantidad) > 1)
                                    {
                                        detalle.lineaDetalle56 = string.Format(formatPrint, d.Articulo.CodigoImpresion, d.Articulo.DescripcionCorta, string.Empty);
                                        detalle.lineaDetalleCant56 = string.Format(formatPrintCant, string.Empty, d.Cantidad + "(" + d.Articulo.PrecioVenta1.ToCustomCurrencyFormat() + ")", d.Valor.ToCustomCurrencyFormat() + " " + impuesto);
                                    }
                                    else
                                    {
                                        detalle.lineaDetalle56 = string.Format(formatPrint, d.Articulo.CodigoImpresion, d.Articulo.DescripcionCorta, d.Valor.ToCustomCurrencyFormat() + " " + impuesto);
                                    }
                                }
                                else //Venta
                                {
                                    if (d.Cantidad > 1)
                                    {
                                        detalle.lineaDetalle56 = string.Format(formatPrint, d.Articulo.CodigoImpresion, d.Articulo.DescripcionCorta, string.Empty);
                                        detalle.lineaDetalleCant56 = string.Format(formatPrintCant, string.Empty, d.Cantidad + "(" + d.Articulo.PrecioVenta1.ToCustomCurrencyFormat() + ")", d.Valor.ToCustomCurrencyFormat() + " " + impuesto);
                                    }
                                    else
                                    {
                                        detalle.lineaDetalle56 = string.Format(formatPrint, d.Articulo.CodigoImpresion, d.Articulo.DescripcionCorta, d.Valor.ToCustomCurrencyFormat() + " " + impuesto);
                                    }
                                }
                            }
                            else
                            {
                                if (InternalSettings.DecimalFlag)
                                {
                                    formatPrint = "{0,-25}{1,-14}{2,17}";
                                }
                                else
                                {
                                    formatPrint = "{0,-25}{1,-16}{2,15}";
                                }

                                detalle.lineaDetalle56 = string.Format(formatPrint, d.Articulo.DescripcionCorta, d.Cantidad, d.Valor.ToCustomCurrencyFormat() + " " + impuesto);
                            }

                            listLineaDetalle.Add(detalle.lineaDetalle56);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            log.Error($"[ProcesarPlantilla.AmarDetalle] no se pudo obtener la información de impresión para el artículo {d.Articulo.CodigoImpresion}");
                            log.Error($"[ProcesarPlantilla.AmarDetalle]  {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarDetalle] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }
            return listDetalle;
        }

        private static List<DetalleFactura> GenerarBoletaLimpia(List<EItemVenta> tirilla, ref List<string> listLineaDetalle)
        {
            List<DetalleFactura> listDetalle = new List<DetalleFactura>();
            try
            {
                if (!tirilla.IsNullOrEmptyList())
                {
                    var groupedList = tirilla.GroupBy(x => x.CodigoLeido).ToList();
                    if (!groupedList.IsNullOrEmptyList())
                    {
                        foreach (var item in groupedList)
                        {
                            EItemVenta art = (EItemVenta)item.FirstOrDefault();

                            // Obtener ID del impuesto para los artículos
                            Dictionary<EImpuesto, List<decimal>> listImpuestos = Entorno.Instancia.Venta.ImpuestosIncluidos;
                            var impArticulo = listImpuestos.FirstOrDefault(x => x.Key.Porcentaje == (float)art.Articulo.Impuesto1);
                            string impuesto = impArticulo.Key != null ? impArticulo.Key.Identificador : string.Empty;

                            if (impuesto.Length > 2)
                                impuesto = impuesto.Substring(0, 2);

                            #region Crear detalle para copia de impresión
                            DetalleFactura detalle = new DetalleFactura();

                            // Sumar cantidades de artículo
                            decimal cantidad = item.Sum(x => x.Cantidad);

                            detalle.Descripcion = art.Articulo.DescripcionCorta;
                            detalle.Cantidad = cantidad.ToString();
                            detalle.Codigo = art.Articulo.CodigoImpresion;
                            detalle.Peso = art.Peso.ToString();
                            detalle.Valor = (art.Articulo.PrecioVenta1 * cantidad).ToCustomCurrencyFormat().PadLeft(12, ' ') + " " + impuesto;
                            detalle.SubTotal = art.Articulo.PrecioVenta1.ToCustomCurrencyFormat();

                            if (Convert.ToDouble(detalle.Peso) > 0)
                            {
                                detalle.PesoReq = true;
                                detalle.Peso = detalle.Peso;
                            }

                            if (Convert.ToInt32(detalle.Cantidad) > 1 || Convert.ToInt32(detalle.Cantidad) < 1)
                            {
                                detalle.ValCantidad = true;
                            }

                            if (!string.IsNullOrEmpty(detalle.Dscto))
                            {
                                detalle.BValorDscto = true;
                                detalle.ValorDscto = "-" + Decimales.RedondearValoresVisuales((detalle.Dscto).ToString()).PadLeft(13, ' ') + " ";
                                var sub = Convert.ToDouble(detalle.SubTotal) * Convert.ToDouble(detalle.Cantidad);
                                //double valor = sub + double.Parse(detalle.Dscto);
                                double valor = sub;
                                detalle.Valor = Decimales.RedondearValoresVisuales((valor).ToString()).PadLeft(12, ' ') + " " + impuesto;

                            }

                            if (!string.IsNullOrEmpty(detalle.Tdscto))
                            {
                                //detalle.BtipoDscto = App.DescuentoGeneral ? false : true;
                                detalle.tipoDscto = detalle.Tdscto;
                            }

                            listDetalle.Add(detalle);


                            #region Crear detalle para impresión

                            if (Config.ViewMode == InternalSettings.ModoConsola)
                            {
                                if (cantidad > 1)
                                {
                                    detalle.lineaDetalle56 = string.Format("{0, -16}{1, -29}{2,11}", art.Articulo.CodigoImpresion, art.Articulo.DescripcionCorta, string.Empty);
                                    detalle.lineaDetalleCant56 = string.Format("{0, -16}{1, -29}{2,11}", string.Empty, cantidad + "(" + art.Articulo.PrecioVenta1.ToCustomCurrencyFormat() + ")", (art.Valor * cantidad).ToCustomCurrencyFormat() + " " + impuesto);
                                }
                                else
                                    detalle.lineaDetalle56 = string.Format("{0, -16}{1, -29}{2,11}", art.Articulo.CodigoImpresion, art.Articulo.DescripcionCorta, art.Valor.ToCustomCurrencyFormat() + " " + impuesto);

                            }
                            else
                            {
                                detalle.lineaDetalle56 = string.Format("{0, -30}{1, -15}{2,11}", art.Articulo.DescripcionCorta, cantidad, (art.Valor * cantidad).ToCustomCurrencyFormat() + " " + impuesto);
                            }

                            listLineaDetalle.Add(detalle.lineaDetalle56);

                            #endregion
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[GenerarBoletaLimpia] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                //throw;
            }
            return listDetalle;
        }

        private static Encabezado ArmarEncabezado(IEnumerable<XElement> tickets, ETerminal Terminal)
        {
            // build header
            Encabezado enc = new Encabezado();
            try
            {
                enc.Empresa = Terminal.Localidad.Nombre;
                enc.Direccion1 = Terminal.Localidad.Direccion;

                enc.Local = Terminal.Localidad.Nombre;
                enc.DirLocal = Terminal.Localidad.Direccion;

                if (Entorno.Instancia.Devolucion != null && Entorno.Instancia.Devolucion.EstaAbierta)
                    enc.DescTipoVenta = "Devolución";
                else if (Entorno.Instancia.VentaEspecialSinMedioPago != null && Entorno.Instancia.VentaEspecialSinMedioPago.EstaAbierta)
                {
                    //
                    string codigoVentaEspecial = Entorno.Instancia.Vista.ModalVentaEspecial.CodigoVenta;
                    if (!string.IsNullOrEmpty(codigoVentaEspecial))
                    {
                        EVentaEspecial ventaEspecial = Entorno.Instancia.TipoVentaEspecial.ListaVentas.FirstOrDefault(x => x.Id.Equals(codigoVentaEspecial));

                        //
                        if (ventaEspecial != null)
                            enc.DescTipoVenta = ventaEspecial.Descripcion;
                    }
                }

            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ArmarEncabezado] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

            }
            return enc;
        }

        #endregion

        #region Arqueo

        public static string Arqueo(ECaja Caja)
        {
            string ticket = string.Empty;
            try
            {
                Arqueo arqueo = new Arqueo();

                string plantilla = LeerPlantilla("plantilla_arqueo.txt");

                // get template file
                XElement xelement = XElement.Load("C:\\Eva\\Files\\Ticket.xml");
                XDocument doc = XDocument.Parse(xelement.ToString());
                IEnumerable<XElement> tickets =
                    from t in doc.Element("tickets").Elements("ticket")
                    where (string)t.Attribute("tipo") == TipoTicket.Factura.ToString()
                    select t;

                arqueo.impresoraNCR = Entorno.Instancia.Impresora.Marca != null ? Entorno.Instancia.Impresora.Marca.ToUpper().Contains("NCR") : false;

                if (arqueo.impresoraNCR)
                    log.Debug("[Arqueo] Usa Impresora NCR");

                // objeto Caja.Arqueo es diccionario
                // 0. Actual en caja
                // 1. Conteo
                // 2. Diferencia
                arqueo.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura");
                arqueo.Cajero = "";
                arqueo.Local = "";
                arqueo.Terminal = Entorno.Instancia.Terminal.Codigo;
                arqueo.Trx = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                if (Caja.Arqueo.Any())
                {
                    List<LArqueo> listLArqueo = new List<LArqueo>();
                    arqueo.Fecha = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                    foreach (var item in Caja.Arqueo)
                    {
                        LArqueo larqueo = new LArqueo();
                        string textoArqueo = string.Empty;

                        decimal faltante = item.Value[2];
                        if (faltante == 0)
                        {
                            textoArqueo = " - ";
                        }
                        else
                        {
                            textoArqueo = faltante > 0 ? "Faltante" : "Sobrante";
                        }

                        larqueo.Nombre = item.Key.Tipo;
                        larqueo.Encaja = item.Value[0].ToCustomNumberFormat().PadLeft(13, ' ');
                        larqueo.Conteo = item.Value[1].ToCustomNumberFormat().PadLeft(13, ' ');
                        larqueo.Faltante = faltante.ToCustomNumberFormat().PadLeft(13, ' ');
                        larqueo.Concepto = textoArqueo;

                        //
                        listLArqueo.Add(larqueo);
                    }
                    arqueo.ListArqueo = listLArqueo;
                }

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(arqueo);
                ticket = actual;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Arqueo] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

            return ticket;
        }

        #endregion

        #region Prestamos y Recogidas

        public static string Prestamos(EPrestamo Prestamo)
        {
            string ticket = string.Empty;

            try
            {
                Prestamo prestamoTemplate = new Prestamo();

                string plantilla = LeerPlantilla("plantillaPrestamo.txt");

                // get template file
                XElement xelement = XElement.Load("C:\\Eva\\Files\\Ticket.xml");
                XDocument doc = XDocument.Parse(xelement.ToString());
                IEnumerable<XElement> tickets =
                    from t in doc.Element("tickets").Elements("ticket")
                    where (string)t.Attribute("tipo") == TipoTicket.Factura.ToString()
                    select t;


                prestamoTemplate.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura"); ;
                prestamoTemplate.MensajeTitulo = "BASE DE CAJA";
                prestamoTemplate.Concepto = "";
                prestamoTemplate.MensajeValor = "DE LA BASE";
                prestamoTemplate.Valor = InternalSettings.CurrencySymbol + Convert.ToDouble(Prestamo.Valor).ToCustomNumberFormat(useThousandsLimit: false);
                prestamoTemplate.Fecha = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                prestamoTemplate.Cajero = Entorno.Instancia.Usuario.Nombre + " " + Entorno.Instancia.Usuario.Apellido;
                prestamoTemplate.Local = "";
                prestamoTemplate.Terminal = Entorno.Instancia.Terminal.Codigo;
                prestamoTemplate.Trx = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(prestamoTemplate);
                ticket = actual ?? "";

                if (prestamoTemplate.impresoraNCR)
                    log.Debug("[ProcesarPlantilla.Prestamos] Usa Impresora NCR");


            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Prestamos] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                //throw;
            }
            return ticket;
        }

        public static string Recogidas(ERecogida Recogida)
        {
            string ticket = string.Empty;

            try
            {
                Prestamo prestamoTemplate = new Prestamo();

                string plantilla = LeerPlantilla("plantillaPrestamo.txt");

                // get template file
                XElement xelement = XElement.Load("C:\\Eva\\Files\\Ticket.xml");
                XDocument doc = XDocument.Parse(xelement.ToString());
                IEnumerable<XElement> tickets =
                    from t in doc.Element("tickets").Elements("ticket")
                    where (string)t.Attribute("tipo") == TipoTicket.Factura.ToString()
                    select t;

                prestamoTemplate.impresoraNCR = Entorno.Instancia.Impresora.Marca != null ? Entorno.Instancia.Impresora.Marca.ToUpper().Contains("NCR") : false;

                prestamoTemplate.Encabezado = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura"); ;
                prestamoTemplate.MensajeTitulo = "RECOGIDA";
                prestamoTemplate.Concepto = Recogida.CodigoRecogida.Descripcion;
                prestamoTemplate.MensajeValor = "DE LA RECOGIDA";
                prestamoTemplate.Valor = InternalSettings.CurrencySymbol + Convert.ToDouble(Recogida.Valor).ToCustomNumberFormat(useThousandsLimit: false);
                prestamoTemplate.Fecha = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString();

                prestamoTemplate.Cajero = Entorno.Instancia.Usuario.Nombre + " " + Entorno.Instancia.Usuario.Apellido;
                prestamoTemplate.Local = "";
                prestamoTemplate.Terminal = Entorno.Instancia.Terminal.Codigo;
                prestamoTemplate.Trx = Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                FormatCompiler compiler = new FormatCompiler();
                Generator generator = compiler.Compile(plantilla);
                string actual = generator.Render(prestamoTemplate);
                ticket = actual ?? "";

                if (prestamoTemplate.impresoraNCR)
                    log.Debug("[ProcesarPlantilla.Prestamos] Usa Impresora NCR");


            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Recogidas] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
            return ticket;
        }

        public static string AperturaCajon()
        {
            string ticket = string.Empty;

            try
            {
                Prestamo template = new Prestamo();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(string.Format("{0}a1{0}{1}{2}", template.Esc, template.C45, template.ASCII1));
                sb.AppendLine(Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.impresion.encabezado_factura"));
                sb.AppendLine("");
                sb.AppendLine("APERTURA DE CAJÓN MONEDERO^");
                sb.AppendLine("");
                sb.AppendLine(string.Format("{0}a1{0}{1}{2}", template.Esc, template.C45, template.ASCII1));
                sb.AppendLine("Fecha: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine("Cajero: " + Entorno.Instancia.Usuario.Nombre + " " + Entorno.Instancia.Usuario.Apellido);
                sb.AppendLine("POS: " + Entorno.Instancia.Terminal.Codigo);
                sb.AppendLine("N. Transacción: " + Convert.ToString(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1));
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine(Environment.NewLine);
                sb.AppendLine(Environment.NewLine);

                //
                ticket = sb.ToString();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Recogidas] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
            return ticket;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Obtiene el archivo y obtiene 
        /// </summary>
        /// <param name="nombreArchivo"></param>
        /// <returns></returns>
        private static string LeerPlantilla(string nombreArchivo)
        {
            string plantilla = string.Empty;
            try
            {
                string fullFilePath = "C:\\Eva\\Files\\" + nombreArchivo;
                if (File.Exists(fullFilePath))
                {
                    using (StreamReader sr = new StreamReader(fullFilePath))
                    {
                        plantilla = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[LeerPlantilla] {0}", ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
            return plantilla;
        }

        /// <summary>
        /// Tipo de ticket a imprimir.
        /// Se usa para obtener el tipo del ticket del archivo Ticket.xml
        /// </summary>
        public enum TipoTicket
        {
            Factura,
            Arqueo,
            Inventario
        }
        #endregion
    }
}
