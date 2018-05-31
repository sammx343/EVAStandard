using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Enums;
using System.Globalization;
using System.Reflection;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdIniciarSesion : ComandoAbstract
    {
        private Solicitudes.SolicitudIniciarSesion solicitud;

        public CmdIniciarSesion(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudIniciarSesion;
        }

        public override void Ejecutar()
        {
            //Validar usuario
            PUsuario usuarioPer = new PUsuario();
            Entidades.EUsuario usuario = null;
            bool permiteAcceso = true;

            //
            if (string.IsNullOrEmpty(solicitud.IdUsuario) || string.IsNullOrEmpty(solicitud.Clave))
            {
                log.Warn("Credenciales de usuario incompletas.");
                Telemetria.Instancia.Id(Common.Config.Terminal).Usuario(solicitud.IdUsuario).AgregaMetrica(new Evento("IniciasSesionInvalido"));
                iu.PanelOperador.MensajeOperador = "Campos faltantes.";
                iu.PanelLogin.Clave = string.Empty;
            }
            else
            {
                Respuesta respuesta = new Respuesta();
                usuario = usuarioPer.Autenticar(solicitud.IdUsuario, solicitud.Clave, out respuesta);
                if (respuesta.Valida)
                {
                    ////Solicitud de actualización
                    //Task<MessageResult> r = iu.PanelDispositivo.VerificarActualizacion();
                    //r.Wait();

                    //if (r.Result == MessageResult.None)
                    //{
                    //Cargue de parametros.
                    PParametros parametrosPer = new PParametros();
                    PErrores erroresPer = new PErrores();
                    respuesta = new Respuesta();

                    Entidades.EParametros parametros = parametrosPer.ObetenerParametros(out respuesta);
                    Entidades.EImpuestos impuestos = new Entidades.EImpuestos();
                    impuestos.Poblar(parametros);

                    Dictionary<int, EError> errores = erroresPer.ObtenerListaErrores();
                    if (respuesta.Valida)
                    {
                        Entorno.Instancia.Parametros = parametros;
                        Entorno.Instancia.Impuestos = impuestos;
                        Entorno.Instancia.setMensajesError(errores);
                        CargarParametros();
                        log.Info("[CmdIniciarSesion]: Parametros establecidos correctamente.");


                        //Información del operador.
                        Entorno.Instancia.Usuario = usuario;

                        //Terminal
                        respuesta = new Respuesta(false);
                        ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo("100003", out respuesta);
                        if (respuesta.Valida)
                        {
                            try
                            {
                                bool defImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
                                if (defImpuestoCompuesto)
                                {
                                    if (!parametrosPer.VerificarExixteTablaImpuestos())
                                    {
                                        permiteAcceso = false;

                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.ErrorFormat("[CmdIniciarSesion] {0}", ex.Message);
                                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                            }

                            if (permiteAcceso)
                            {
                                //Version assemblyVersion = Assembly.GetEntryAssembly().GetName().Version;
                                //string version = String.Format("{0}.{1}.{2}.{3}", assemblyVersion.Major, assemblyVersion.Minor, assemblyVersion.Build, assemblyVersion.Revision);
                                //string logInicioApp = string.Format("--> INICIO DE SESION, VERSION, {0} <--", version);
                                //log.Info(logInicioApp);


                                Entorno.Instancia.Terminal = terminal;

                                iu.PanelOperador.CodigoTerminal = terminal.Codigo;
                                iu.PanelOperador.NombreUsuario = usuario.Nombre;

                                //
                                string msg = string.Format("[CmdIniciarSesion]: Sesión iniciada correctamente. {0}", usuario.ToString());
                                log.Info(msg);

                                // se debe preguntar por la interfaz que se encuentra en las configuraciones de la aplicación.

                                // modo caracter
                                //Solicitudes.SolicitudPanelVenta solicitudPanelventa = new Solicitudes.SolicitudPanelVenta(Enums.Solicitud.Vender);
                                //Reactor.Instancia.Procesar(solicitudPanelventa);

                                // modo touch
                                Solicitudes.SolicitudPanelVenta solicitudPanelventa = new Solicitudes.SolicitudPanelVenta(Enums.Solicitud.Vender);
                                Reactor.Instancia.Procesar(solicitudPanelventa);
                                //iu.PanelLogin.IdUsuario = "";
                                //iu.PanelLogin.Clave = "";
                                Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "";

                                //Ids de mis tablas de acumulados
                                //Entorno.Instancia.IdsAcumulados = new PVenta().IdsAcumulados(Entorno.Instancia.Usuario, Entorno.Instancia.Terminal);
                                ////Clientes
                                //Entorno.Instancia.Clientes = new PClientes().GetAllClientes();
                                ////Codigos de recogida
                                //Entorno.Instancia.CodigosRecogida = new PRecogida().GetCodigosRecogida(out respuesta);
                                ////Tipos de Venta Especial
                                //Entorno.Instancia.TipoVentaEspecial = new PVentaEspecial().GetAllVentaEspecial(Entorno.Instancia.Clientes);

                                //// Ajustes
                                //Entorno.Instancia.TiposAjustes = new PTiposAjuste().GetAllTiposAjuste();

                                //Telemetria.Instancia.Id(Entorno.Instancia.Terminal.Codigo).Usuario(Entorno.Instancia.Usuario.Usuario).AgregaMetrica(new Evento("IniciarSesion"));
                                if (iu.PanelVentas != null)
                                {
                                    iu.PanelVentas.VisorEntrada = "";
                                    iu.PanelVentas.VisorFechaActual = DateTime.Now.ToString("dd/MM/yyyy");
                                }

                            }
                            else
                            {
                                log.Warn("Informacion en base de Datos no Concuerda con Parametros.");
                                Telemetria.Instancia.Id(Common.Config.Terminal).Usuario(solicitud.IdUsuario).AgregaMetrica(new Evento("IniciasSesionInvalido"));
                                iu.PanelOperador.MensajeOperador = respuesta.Mensaje;
                                iu.PanelLogin.Clave = string.Empty;
                            }
                        }
                        else
                        {
                            Telemetria.Instancia.Id(Common.Config.Terminal).Usuario(solicitud.IdUsuario).AgregaMetrica(new Evento("TerminalInvalida"));
                            log.ErrorFormat("[CmdIniciarSesion] No se puede iniciar la aplicación ya que no se encontró terminal asociada al código \"{0}\"", Common.Config.Terminal);
                            Entorno.Vista.PanelOperador.MensajeOperador = respuesta.Mensaje;
                        }

                    }
                    else
                    {

                        Entorno.Vista.PanelOperador.MensajeOperador = respuesta.Mensaje;
                    }
                    //}
                    //else if (r.Result == MessageResult.Affirmative)
                    //{
                    //    log.Info("[CmdIniciarSesion] Proceso de actualización en proceso, se cancela el inicio de sesión.");
                    //}
                    //}
                    //else
                    //{
                    //    log.Warn("Credenciales de usuario invalidas.");
                    //    Telemetria.Instancia.Id(Common.Config.Terminal).Usuario(solicitud.IdUsuario).AgregaMetrica(new Evento("IniciasSesionInvalido"));
                    //    iu.PanelOperador.MensajeOperador = respuesta.Mensaje;
                    //    iu.PanelLogin.Clave = string.Empty;
                    //}
                }
            }
        }

        private Respuesta CargarParametros()
        {
            //Cargando parametros.
            Respuesta respuesta = new Respuesta();
            try
            {
                PParametros parametrosPer = new PParametros();
                EParametros listaParametros = parametrosPer.ObetenerParametros(out respuesta);
                if (respuesta.Valida)
                {
                    Entorno.Instancia.Parametros = listaParametros;

                    //Parametros de entradas de valores (formatos)
                    try
                    {
                        CultureInfo culture = CultureInfo.CurrentCulture;

                        InternalSettings.ThousandSeparator = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.dinero.caracter_miles");
                        InternalSettings.DecimalSeparator = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.eva.caracter_decimal");
                        InternalSettings.CurrencySymbol = Entorno.Instancia.Parametros.ObtenerValorParametro<string>("pdv.dinero.simbolo_moneda");
                        InternalSettings.WholeNumberLimit = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.dinero.limite_valor_miles");
                        InternalSettings.DecimalLimit = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.dinero.limite_decimales");


                        if (string.IsNullOrEmpty(InternalSettings.ThousandSeparator))
                            InternalSettings.ThousandSeparator = culture.NumberFormat.NumberGroupSeparator;

                        if (string.IsNullOrEmpty(InternalSettings.DecimalSeparator))
                            InternalSettings.DecimalSeparator = culture.NumberFormat.NumberDecimalSeparator;

                        if (string.IsNullOrEmpty(InternalSettings.CurrencySymbol))
                            InternalSettings.CurrencySymbol = culture.NumberFormat.CurrencySymbol;

                        if (InternalSettings.DecimalLimit <= 0)
                            InternalSettings.DecimalLimit = culture.NumberFormat.NumberDecimalDigits;
                        else
                        {
                            if (InternalSettings.DecimalLimit > 3)
                                InternalSettings.DecimalLimit = 3;
                        }

                        if (InternalSettings.WholeNumberLimit <= 0)
                            InternalSettings.WholeNumberLimit = 1000000;


                        InternalSettings.CurrencySymbolFlag = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.dinero.maneja_simbolo_moneda");
                        InternalSettings.DecimalFlag = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.maneja_decimales");
                    }
                    catch (Exception ex)
                    {
                        log.ErrorFormat("[CargarParametros] {0}", ex.Message);
                        Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));

                    }

                }
                else
                {
                    log.ErrorFormat("[ActualizarApp] {0}", respuesta.Mensaje);
                }
            }
            catch (Exception)
            {
                log.Error("[ActualizarApp] error al intentar cargar los parametros");

            }

            return respuesta;
        }
    }
}
