using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using EvaPOS.Enums;
using EvaPOS.FSM;
using PRUEBA1.FSM;
using Redsis.EVA.Client.Core.Comandos;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Repositorio;
using Redsis.EVA.Client.Core.Solicitudes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Redsis.EVA.Client.Core
{
    /// <summary>
    /// Procesa solicitudes, las convierte a comandos y somete a cola de
    /// procesamiento.
    /// La cola de comandos es procesada secuencialmente por un solo Thread.
    /// 
    /// ES CRITICO MANTENER UN SOLO THREAD PARA GARANTIZAR QUE NO HAY ACCESO
    /// CONCURRENTE A ESTRUCTURAS DE DATOS O DB.
    /// 
    /// </summary>
    public class Reactor : IReactor
    {
        #region propiedades

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private CreadorComandos creadorComandos;
        FSM stateMachine = null;
        List<EstadosFSM> estados;
        List<TransicionesFSM> transiciones;
        EstadosFSM estadoInicial;
        long ultimaTransaccion;

        public EstadosFSM EstadoFSMAnterior { get; private set; }

        public EstadosFSM EstadoFSMActual { get; private set; }

        private static Reactor instancia;

        public static Reactor Instancia
        {
            get
            {
                try
                {
                    if (instancia == null)
                        instancia = new Reactor();

                    return instancia;
                }
                catch (EvaApplicationException ex)
                {
                    throw ex;
                }
            }
        }

        #endregion

        #region Constructor

        private Reactor()
        {
            try
            {
                this.creadorComandos = new CreadorComandos();
                InicializarFSM();
            }
            catch (EvaApplicationException evaEx)
            {
                throw new EvaApplicationException(evaEx.Message);
            }
        }

        #endregion

        #region Funciones y metodos

        public string ExportGraphic()
        {
            string ans = "";

            ans = stateMachine.GraficoDoT();

            return ans;
        }

        private ActionBlock<IComando> procesador = new ActionBlock<IComando>(comando =>
        {
            try
            {
                comando.Ejecutar();
            }
            catch (EvaApplicationException evaEx)
            {
                log.ErrorFormat("[Reactor.procesador] {0}", evaEx.Message);
                Entorno.Instancia.Vista.PanelOperador.MensajeOperador = evaEx.Message;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[EXCEPCIÓN NO CONTROLADA EN LA APLICACIÓN: [Reactor.procesador] {0}", ex.ToString());
                //Entorno.Instancia.Vista.PanelOperador.MensajeOperador = ex.Message;
            }
        });

        public void Procesar(ISolicitud s)
        {
            // En este punto la FSM debe validar que el mensaje es valido.
            // Si pasa tal validación, se crea el comando que lo procesa.
            // TODO CONTROLAR posible problema de concurrencia modificando la FSM
            // entre mensajes y comandos.

            try
            {
                string nombreSolicitud = Enum.GetName(typeof(Solicitud), s.TipoSolicitud);
                stateMachine.LanzaTransicion(nombreSolicitud);

                //
                if (s.TipoSolicitud != Solicitud.CancelarOperacion)
                    ProcesarSolicitud(s);
            }
            catch (TransicionNoDefinidaException defEx)
            {
                log.WarnFormat("[Procesar.TransicionNoDefinidaException] {0}", defEx.Message);
                Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
            }
            catch (TransicionInvalidaException tranEx)
            {
                log.WarnFormat("[Procesar.TransicionInvalidaException] {0}", tranEx.Message);
                Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                //Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
            }
            catch (CondicionFallidaException condEx)
            {
                log.WarnFormat("[Procesar.CondicionFallidaException] {0}", condEx.Message);
                Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Procesar.Exception] {0}", ex.Message);
                Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
            }
        }

        private void ProcesarSolicitud(ISolicitud s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("El parametro solicitud es nulo o vacío");
            }
            else
            {
                //
                log.Debug("Solicitud: " + s);
                IComando c = creadorComandos.Crear(s);
                log.Debug("Comando ingresado en cola: " + c.ToString());
                bool post = procesador.Post(c);
                if (!post)
                {
                    log.ErrorFormat("No se pudo encolar la solicitud [{0}]", s.ToString());
                }
            }
        }

        private void InicializarFSM()
        {
            try
            {
                //
                estados = new List<EstadosFSM>()
                {
                    EstadosFSM.Inicio,
                    EstadosFSM.IniciarSesion,
                    EstadosFSM.InicioSesion,
                    EstadosFSM.Vender,
                    EstadosFSM.Pago,
                    EstadosFSM.TirillaVenta,
                    EstadosFSM.TirillaDevolucion,
                    EstadosFSM.Fin,
                    EstadosFSM.NoVenta,
                    EstadosFSM.RegistrarDispositivos,
                    EstadosFSM.EstadosDispositivos,
                    EstadosFSM.TerminalAsegurada,
                    EstadosFSM.Prestamos,
                    EstadosFSM.Recogida,
                    EstadosFSM.CancelarItem,
                    EstadosFSM.CancelarItemDevolucion,
                    EstadosFSM.CancelarItemVentaEspecialSinMedioPago,
                    EstadosFSM.Devolucion,
                    EstadosFSM.VentaEspecialSinMedioPago,
                    EstadosFSM.Degustacion,
                    EstadosFSM.TerminarDevolucion,
                    EstadosFSM.PagoDatafono,
                    EstadosFSM.ConsultarPrecio,
                    EstadosFSM.RegistrarRecogida,
                    EstadosFSM.ReintentoPago,
                    EstadosFSM.CancelarTransaccion,
                    EstadosFSM.CancelarDevolucion,
                    EstadosFSM.PantallaCliente,
                    EstadosFSM.ImprimirUltima,
                    EstadosFSM.VentaEspecial,
                    EstadosFSM.TerminarVentaEspecial,
                    EstadosFSM.CancelarVentaEspecial,
                    EstadosFSM.PagoDatafonoManual,
                    EstadosFSM.CierreDatafono,
                    EstadosFSM.PanelCierreDatafono,
                    EstadosFSM.Ajustes,
                    EstadosFSM.AgregarAjuste,
                    EstadosFSM.TerminarAjuste,
                    EstadosFSM.CancelarItemAjuste,
                    EstadosFSM.CancelarTransaccionAjuste,
                    EstadosFSM.AgregarItemAjuste,
                    EstadosFSM.Arqueo,
                    EstadosFSM.AgregarValorArqueo,
                    EstadosFSM.GuardarArqueo,
                    EstadosFSM.CancelarPago,
                    EstadosFSM.CancelarOperacionDevolucion,
                    EstadosFSM.CancelarVenta,
                    EstadosFSM.CancelarConsultaPrecio,
                    EstadosFSM.CancelarCancelarItem,
                    EstadosFSM.CancelarRecogida,
                    EstadosFSM.PagoEfectivo,
                    EstadosFSM.AgregarPrestamo,
                    EstadosFSM.SolicitarIntervencionRecogida,
                    EstadosFSM.ValidarIntervencionRecogida,
                    EstadosFSM.TerminarRecogida,
                    EstadosFSM.SolicitarIntervencionPrestamo,
                    EstadosFSM.ValidarIntervencionPrestamo,
                    EstadosFSM.TerminarPrestamo
                };

                //
                transiciones = new List<TransicionesFSM>()
                {
                    TransicionesFSM.IniciarSesion,
                    TransicionesFSM.InicioSesion,
                    TransicionesFSM.Vender,
                    TransicionesFSM.AgregarItem,
                    TransicionesFSM.Pagar,
                    TransicionesFSM.PagoEfectivo,
                    TransicionesFSM.VerTirilla,
                    TransicionesFSM.VerTirillaDevolucion,
                    TransicionesFSM.Volver,
                    TransicionesFSM.TerminarDevolucion,
                    TransicionesFSM.RegistrarDispositivos,
                    TransicionesFSM.ComprobarEstadoDispositivos,
                    TransicionesFSM.TerminalAsegurada,
                    TransicionesFSM.Prestamos,
                    TransicionesFSM.Recogida,
                    TransicionesFSM.EstadoDevolucion,
                    TransicionesFSM.CancelarItem,
                    TransicionesFSM.CancelarItemDevolucion,
                    TransicionesFSM.AgregarPrestamo,
                    TransicionesFSM.PagoDatafono,
                    TransicionesFSM.EstadoConsultarPrecio,
                    TransicionesFSM.ConsultarPrecio,
                    TransicionesFSM.AgregarItemDevolucion,
                    TransicionesFSM.PagarVentaDatafono,
                    TransicionesFSM.RegistrarRecogida,
                    TransicionesFSM.ReintentarPago,
                    TransicionesFSM.CancelarTransaccion,
                    TransicionesFSM.CancelarDevolucion,
                    TransicionesFSM.PantallaCliente,
                    TransicionesFSM.LimpiarVisor,
                    TransicionesFSM.AgregarCliente,
                    TransicionesFSM.ImprimirUltima,
                    TransicionesFSM.VentaEspecial,
                    TransicionesFSM.RegistrarVentaEspecial,
                    TransicionesFSM.RegistrarVentaEspecialSinMedioPago,
                    TransicionesFSM.AgregarItemVentaEspecialSinMedioPago,
                    TransicionesFSM.CancelarItemVentaEspecialSinMedioPago,
                    TransicionesFSM.CancelarVentaEspecialSinMedioPago,
                    TransicionesFSM.TerminarVentaEspecialSinMedioPago,
                    TransicionesFSM.AgregarClienteVentaEspecialSinMedioPago,
                    TransicionesFSM.CancelarVentaEspecial,
                    TransicionesFSM.PagoDatafonoManual,
                    TransicionesFSM.CierreDatafono,
                    TransicionesFSM.PanelCierreDatafono,
                    TransicionesFSM.Ajustes,
                    TransicionesFSM.AgregarAjuste,
                    TransicionesFSM.TerminarAjuste,
                    TransicionesFSM.CancelarItemAjuste,
                    TransicionesFSM.CancelarTransaccionAjuste,
                    TransicionesFSM.AgregarItemAjuste,
                    TransicionesFSM.Arqueo,
                    TransicionesFSM.AgregarValorArqueo,
                    TransicionesFSM.GuardarArqueo,
                    TransicionesFSM.CancelarPago,
                    TransicionesFSM.CancelarTransaccionRecogida,
                    TransicionesFSM.NoCancelar,
                    TransicionesFSM.CancelarOperacion,
                    TransicionesFSM.SolicitarIntervencionRecogida,
                    TransicionesFSM.ValidarIntervencionRecogida,
                    TransicionesFSM.TerminarRecogida,
                     TransicionesFSM.SolicitarIntervencionPrestamo,
                    TransicionesFSM.ValidarIntervencionPrestamo,
                    TransicionesFSM.TerminarPrestamo
                };

                //
                estadoInicial = EstadosFSM.Inicio;
                EstadoFSMActual = estadoInicial;
                stateMachine = new FSM(estados, transiciones, estadoInicial);

                #region Configuración de estados

                stateMachine.Definir(EstadosFSM.CancelarConsultaPrecio)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.InicioSesion)
                    .Transicion(TransicionesFSM.IniciarSesion, EstadosFSM.IniciarSesion);
                //
                stateMachine.Definir(EstadosFSM.Inicio)
                    .Transicion(TransicionesFSM.InicioSesion, EstadosFSM.InicioSesion)
                    .Transicion(TransicionesFSM.IniciarSesion, EstadosFSM.IniciarSesion)
                    .Transicion(TransicionesFSM.ComprobarEstadoDispositivos, EstadosFSM.EstadosDispositivos);

                stateMachine.Definir(EstadosFSM.RegistrarDispositivos)
                    .Transicion(TransicionesFSM.RegistrarDispositivos, EstadosFSM.RegistrarDispositivos)
                    .Transicion(TransicionesFSM.ComprobarEstadoDispositivos, EstadosFSM.EstadosDispositivos);

                stateMachine.Definir(EstadosFSM.EstadosDispositivos)
                    .Transicion(TransicionesFSM.ComprobarEstadoDispositivos, EstadosFSM.EstadosDispositivos)
                    .Transicion(TransicionesFSM.PantallaCliente, EstadosFSM.PantallaCliente)
                    .Transicion(TransicionesFSM.TerminalAsegurada, EstadosFSM.TerminalAsegurada);


                stateMachine.Definir(EstadosFSM.PantallaCliente)
                    .Transicion(TransicionesFSM.TerminalAsegurada, EstadosFSM.TerminalAsegurada);

                stateMachine.Definir(EstadosFSM.TerminalAsegurada)
                    .Transicion(TransicionesFSM.InicioSesion, EstadosFSM.InicioSesion);

                //
                ConfigInicioSesion();

                stateMachine.Definir(EstadosFSM.IniciarSesion)
                    .Transicion(TransicionesFSM.IniciarSesion, EstadosFSM.IniciarSesion)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender);

                //
                ConfigVender();


                stateMachine.Definir(EstadosFSM.CancelarTransaccion)
                    .Transicion(TransicionesFSM.CancelarTransaccion, EstadosFSM.CancelarTransaccion)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.CancelarItem)
                    .Transicion(TransicionesFSM.AgregarItem, EstadosFSM.CancelarItem)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarCancelarItem)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.CancelarCancelarItem)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .AlEntrar(() =>
                    {
                        ProcesarSolicitud(new SolicitudCancelar_CancelarItem(Solicitud.CancelarCancelarItem));
                    }, "CancelarCancelarItem");

                stateMachine.Definir(EstadosFSM.CancelarItemDevolucion)
                    .Transicion(TransicionesFSM.AgregarItemDevolucion, EstadosFSM.CancelarItemDevolucion)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Devolucion);

                stateMachine.Definir(EstadosFSM.CancelarItemVentaEspecialSinMedioPago)
                    .Transicion(TransicionesFSM.AgregarItemVentaEspecialSinMedioPago, EstadosFSM.CancelarItemVentaEspecialSinMedioPago)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.VentaEspecialSinMedioPago);

                //
                ConfigAgregarAjuste();

                //
                ConfigDevolucion();

                stateMachine.Definir(EstadosFSM.Recogida)
                    .Transicion(TransicionesFSM.Recogida, EstadosFSM.Recogida)
                    //.Transicion(TransicionesFSM.SolicitarIntervencionRecogida, EstadosFSM.SolicitarIntervencionRecogida)
                    .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.Recogida)
                    .Transicion(TransicionesFSM.RegistrarRecogida, EstadosFSM.RegistrarRecogida)
                    .Transicion(TransicionesFSM.RegistrarRecogida, EstadosFSM.Recogida)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarRecogida)
                    .Transicion(TransicionesFSM.TerminarRecogida, EstadosFSM.TerminarRecogida)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .TransicionCondicionada(TransicionesFSM.TerminarRecogida.GetEnumName(), EstadosFSM.TerminarRecogida.GetEnumName(), (() =>
                    {
                        bool tieneRecogidas = true;

                        if (Entorno.Instancia.Recogida.listRecogidas.IsNullOrEmptyList())
                        {
                            tieneRecogidas = false;
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        }

                        return tieneRecogidas;
                    }), "TrTerminarRecogida")
                    .AlSalir(() =>
                    {
                        int intervencionRegogida = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.recogidas");
                        switch (intervencionRegogida)
                        {
                            case 0:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual no permite realizar la operación.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "No se permite la operación";
                                break;
                            case 1:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual solicita intervención.");

                                if (instancia.EstadoFSMActual == EstadosFSM.Recogida)
                                    Helpers.Utilidades.GenerarTransaccionApertura();

                                break;
                            case 2:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual no necesita intervención.");
                                break;
                            default:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual es inválido.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Operación inválida"; break;
                        }


                    }, "Salir");

                stateMachine.Definir(EstadosFSM.TerminarRecogida)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Recogida)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender);

                //
                ConfigIntervencionRecogida();

                stateMachine.Definir(EstadosFSM.Prestamos)
                    //.Transicion(TransicionesFSM.Prestamos, EstadosFSM.Prestamos)
                    .Transicion(TransicionesFSM.AgregarPrestamo, EstadosFSM.AgregarPrestamo)
                    .Transicion(TransicionesFSM.AgregarPrestamo, EstadosFSM.Prestamos)
                    .Transicion(TransicionesFSM.TerminarPrestamo, EstadosFSM.TerminarPrestamo)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                 .TransicionCondicionada(TransicionesFSM.TerminarPrestamo.GetEnumName(), EstadosFSM.TerminarPrestamo.GetEnumName(), (() =>
                 {
                     bool impideTransicion = true;
                     int intervencion = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.prestamos");
                     switch (intervencion)
                     {
                         case 0:
                             impideTransicion = true;
                             log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual no permite realizar la operación.");
                             Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "No se permite la operación";
                             break;
                         case 1:
                             impideTransicion = true;
                             log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual solicita intervención.");
                             Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Se solicita intervención para la operación";

                             bool tieneRecogidas = (Entorno.Instancia.Prestamo.Valor > 0);
                             if (!tieneRecogidas)
                             {
                                 Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                             }

                             break;
                         case 2:
                             impideTransicion = false;
                             break;
                         default:
                             impideTransicion = true;
                             log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual es inválido.");
                             Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Operación inválida"; break;
                     }

                     

                     return !impideTransicion;
                 }), "TerminarPrestamo")
                    .AlSalir(() =>
                    {

                        int intervencion = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.prestamos");
                        switch (intervencion)
                        {
                            case 0:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual no permite realizar la operación.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "No se permite la operación";
                                break;
                            case 1:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual solicita intervención.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Se solicita intervención para la operación";

                                if (instancia.EstadoFSMActual == EstadosFSM.Prestamos)
                                    Helpers.Utilidades.GenerarTransaccionApertura();

                                break;
                            case 2:
                                break;
                            default:
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual es inválido.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Operación inválida"; break;
                        }

                        

                    }, "Salir");

                //
                ConfigIntervencionPrestamo();

                stateMachine.Definir(EstadosFSM.TerminarPrestamo)
                  .Transicion(TransicionesFSM.Volver, EstadosFSM.Prestamos)
                  .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.CancelarRecogida)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Recogida)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .AlEntrar(() =>
                    {
                        ProcesarSolicitud(new SolicitudCancelarRecogida(Solicitud.CancelarTransaccionRecogida));
                    }, "CancelarRecogida");

                stateMachine.Definir(EstadosFSM.Ajustes)
                   .Transicion(TransicionesFSM.AgregarAjuste, EstadosFSM.AgregarAjuste)
                   .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.VentaEspecial)
                    .Transicion(TransicionesFSM.RegistrarVentaEspecialSinMedioPago, EstadosFSM.VentaEspecialSinMedioPago)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                //
                ConfigVentaEspecialSinMedioPago();

                stateMachine.Definir(EstadosFSM.TirillaVenta)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.ImprimirUltima)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.TirillaDevolucion)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Devolucion);

                stateMachine.Definir(EstadosFSM.TerminarDevolucion)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.TerminarAjuste)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.TerminarVentaEspecial)
                   .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.ConsultarPrecio)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.ConsultarPrecio, EstadosFSM.ConsultarPrecio)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarConsultaPrecio)
                    .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.ConsultarPrecio)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.CancelarConsultaPrecio)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .AlEntrar(() =>
                    {
                        ProcesarSolicitud(new SolicitudCancelarConsultaPrecio(Solicitud.CancelarConsultaPrecio));
                    }, "CancelarConsultaPrecio");

                stateMachine.Definir(EstadosFSM.Pago)
                    //.Transicion(TransicionesFSM.Pagar, EstadosFSM.Pago)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.PagoEfectivo, EstadosFSM.PagoEfectivo)
                    .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.Pago)
                    .Transicion(TransicionesFSM.PagoDatafono, EstadosFSM.PagoDatafono)
                    .Transicion(TransicionesFSM.CancelarPago, EstadosFSM.CancelarPago)
                    .AlSalir(() =>
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = string.Empty;
                    }, "SalirPago");


                stateMachine.Definir(EstadosFSM.PagoEfectivo)
                    .Transicion(TransicionesFSM.Pagar, EstadosFSM.Pago)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                stateMachine.Definir(EstadosFSM.CancelarPago)
                    .Transicion(TransicionesFSM.CancelarPago, EstadosFSM.CancelarPago)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Pago);

                stateMachine.Definir(EstadosFSM.PagoDatafono)
                    .Transicion(TransicionesFSM.PagoDatafonoManual, EstadosFSM.PagoDatafonoManual)
                    .Transicion(TransicionesFSM.PagarVentaDatafono, EstadosFSM.PagoDatafono)
                    .Transicion(TransicionesFSM.ReintentarPago, EstadosFSM.ReintentoPago)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Pagar, EstadosFSM.Pago)
                     .AlSalir(() =>
                     {
                         Entorno.Instancia.Vista.PanelOperador.MensajeOperador = string.Empty;
                     }, "SalirPagoDatafono");

                stateMachine.Definir(EstadosFSM.ReintentoPago)
                    .Transicion(TransicionesFSM.PagoDatafonoManual, EstadosFSM.PagoDatafonoManual)
                    .Transicion(TransicionesFSM.ReintentarPago, EstadosFSM.ReintentoPago)
                    .Transicion(TransicionesFSM.PagarVentaDatafono, EstadosFSM.PagoDatafono)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Pagar, EstadosFSM.Pago)
                    .AlSalir(() =>
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = string.Empty;
                    }, "SalirReintentoPago");

                stateMachine.Definir(EstadosFSM.PagoDatafonoManual)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Pagar, EstadosFSM.Pago)
                    .Transicion(TransicionesFSM.PagarVentaDatafono, EstadosFSM.PagoDatafono);
                //.Transicion(TransicionesFSM.PagoDatafono, EstadosFSM.PagoDatafono)
                //.Transicion(TransicionesFSM.PagoEfectivo, EstadosFSM.Pago)

                stateMachine.Definir(EstadosFSM.PanelCierreDatafono)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.CierreDatafono, EstadosFSM.CierreDatafono);

                stateMachine.Definir(EstadosFSM.CierreDatafono)
                    .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.CierreDatafono, EstadosFSM.CierreDatafono)
                    .AlSalir(() => { Entorno.Instancia.Vista.PanelOperador.MensajeOperador = string.Empty; }, "SalirCierreDatafono");

                stateMachine.Definir(EstadosFSM.Arqueo)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Arqueo, EstadosFSM.Arqueo)
                    .Transicion(TransicionesFSM.AgregarValorArqueo, EstadosFSM.Arqueo)
                    .Transicion(TransicionesFSM.GuardarArqueo, EstadosFSM.GuardarArqueo);

                stateMachine.Definir(EstadosFSM.GuardarArqueo)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

                #endregion


                //
                stateMachine.OnTransitionedEvent += StateMachine_OnTransitionedEvent;
            }
            catch (TransicionNoDefinidaException tranEx)
            {
                log.ErrorFormat("[InicializarFSM] {0}", tranEx.Message);
                throw new EvaApplicationException(tranEx.Message);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[InicializarFSM] {0}", ex.Message);
                throw new EvaApplicationException(ex);
            }
        }

        private void ConfigIntervencionRecogida()
        {
            stateMachine.Definir(EstadosFSM.SolicitarIntervencionRecogida)
                .Transicion(TransicionesFSM.SolicitarIntervencionRecogida, EstadosFSM.SolicitarIntervencionRecogida)
                .Transicion(TransicionesFSM.ValidarIntervencionRecogida, EstadosFSM.ValidarIntervencionRecogida)
                .Transicion(TransicionesFSM.Recogida, EstadosFSM.Recogida)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

            stateMachine.Definir(EstadosFSM.ValidarIntervencionRecogida)
                .Transicion(TransicionesFSM.Recogida, EstadosFSM.Recogida)
                .Transicion(TransicionesFSM.SolicitarIntervencionRecogida, EstadosFSM.SolicitarIntervencionRecogida)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);
        }

        private void ConfigIntervencionPrestamo()
        {
            stateMachine.Definir(EstadosFSM.SolicitarIntervencionPrestamo)
                .Transicion(TransicionesFSM.SolicitarIntervencionPrestamo, EstadosFSM.SolicitarIntervencionPrestamo)
                .Transicion(TransicionesFSM.ValidarIntervencionPrestamo, EstadosFSM.ValidarIntervencionPrestamo)
                .Transicion(TransicionesFSM.Prestamos, EstadosFSM.Prestamos)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);

            stateMachine.Definir(EstadosFSM.ValidarIntervencionPrestamo)
                .Transicion(TransicionesFSM.Prestamos, EstadosFSM.Prestamos)
                .Transicion(TransicionesFSM.SolicitarIntervencionPrestamo, EstadosFSM.SolicitarIntervencionPrestamo)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender);
        }

        private void ConfigInicioSesion()
        {
            stateMachine.Definir(EstadosFSM.InicioSesion)
                .AlEntrar(() =>
                {
                    //
                    if (Entorno.Instancia.Vista.PanelActivo != null)
                    {
                        if (!(Entorno.Instancia.Vista.PanelActivo is IPanelLogin))
                        {
                            ProcesarSolicitud(new Solicitudes.SolicitudPanelInicioSesion(Solicitud.InicioSesion));
                        }
                    }
                    //
                }, "Iniciar Sesión")
                .Transicion(TransicionesFSM.IniciarSesion, EstadosFSM.IniciarSesion)
                .Transicion(TransicionesFSM.TerminalAsegurada, EstadosFSM.TerminalAsegurada);
        }

        private void ConfigAgregarAjuste()
        {
            stateMachine.Definir(EstadosFSM.AgregarAjuste)
                .Transicion(TransicionesFSM.Ajustes, EstadosFSM.AgregarAjuste)
                .Transicion(TransicionesFSM.AgregarAjuste, EstadosFSM.AgregarAjuste)
                //.Transicion(TransicionesFSM.TerminarAjuste, EstadosFSM.AgregarAjuste)
                .TransicionCondicionada(TransicionesFSM.TerminarAjuste.GetEnumName(),
                EstadosFSM.TerminarAjuste.GetEnumName(),
                (() =>
                {
                    return ((Entorno.Instancia.Ajuste.NumeroDeItemsVenta > 0) ? true : false);
                }),
                "Valida items en venta")
                .Transicion(TransicionesFSM.AgregarItemAjuste, EstadosFSM.AgregarAjuste)
                .Transicion(TransicionesFSM.CancelarItemAjuste, EstadosFSM.AgregarAjuste)
                .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.AgregarAjuste)
                .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarTransaccionAjuste)
                .TransicionCondicionada(TransicionesFSM.Volver.GetEnumName(),
                EstadosFSM.Vender.GetEnumName(),
                (() =>
                {
                    //return ((Entorno.Instancia.Devolucion.NumeroDeItemsVenta < 0) ? false : true);
                    bool transaccionAbierta = (Entorno.Instancia.Ajuste.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        string visor = Entorno.Instancia.Vista.PanelVentas.VisorCliente.Descripcion;
                        string empty = "";
                        if (string.Compare(visor, empty) == 0)
                        {
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                            Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                            Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                        }
                        else
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorCliente.LimpiarArticulo();
                        }

                    }
                    return !transaccionAbierta;
                }),
                "Valida items en venta");

            stateMachine.Definir(EstadosFSM.CancelarTransaccionAjuste)
                    .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                    .Transicion(TransicionesFSM.Volver, EstadosFSM.AgregarAjuste)
                    .AlEntrar(() =>
                    {
                        ProcesarSolicitud(new SolicitudCancelarAjuste(Solicitud.CancelarTransaccionAjuste));
                    }, "CancelarTransaccionAjuste");
        }

        private void ConfigVentaEspecialSinMedioPago()
        {
            stateMachine.Definir(EstadosFSM.VentaEspecialSinMedioPago)
                .Transicion(TransicionesFSM.AgregarItemVentaEspecialSinMedioPago, EstadosFSM.VentaEspecialSinMedioPago)
                .Transicion(TransicionesFSM.AgregarClienteVentaEspecialSinMedioPago, EstadosFSM.VentaEspecialSinMedioPago)
                .Transicion(TransicionesFSM.CancelarItemVentaEspecialSinMedioPago, EstadosFSM.CancelarItemVentaEspecialSinMedioPago)
                .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarVentaEspecial)
                .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.VentaEspecialSinMedioPago)
                .TransicionCondicionada(TransicionesFSM.TerminarVentaEspecialSinMedioPago.GetEnumName(),
                EstadosFSM.TerminarVentaEspecial.GetEnumName(),
                (() =>
                {
                    return ((Entorno.Instancia.VentaEspecialSinMedioPago.NumeroDeItemsVenta > 0) ? true : false);
                }),
                "Valida items en venta")
                .TransicionCondicionada(TransicionesFSM.Volver.GetEnumName(),
                EstadosFSM.Vender.GetEnumName(),
                (() =>
                {
                    //return ((Entorno.Instancia.Devolucion.NumeroDeItemsVenta < 0) ? false : true);
                    bool transaccionAbierta = (Entorno.Instancia.VentaEspecialSinMedioPago.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        string visor = Entorno.Instancia.Vista.PanelVentas.VisorCliente.Descripcion;
                        string empty = "";
                        if (string.Compare(visor, empty) == 0)
                        {
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                            Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                            Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                        }
                        else
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorCliente.LimpiarArticulo();
                        }

                    }
                    return !transaccionAbierta;
                }),
                "Valida items en venta");

            //Cancelar operación
            stateMachine.Definir(EstadosFSM.CancelarVentaEspecial)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.VentaEspecialSinMedioPago)
                .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                .AlEntrar(() =>
                {
                    ProcesarSolicitud(new SolicitudCancelarVentaEspecial(Solicitud.CancelarVentaEspecial));
                }, "CancelarVentaEspecial");

        }

        private void ConfigDevolucion()
        {
            //Devolucion
            stateMachine.Definir(EstadosFSM.Devolucion)
                .Transicion(TransicionesFSM.AgregarItemDevolucion, EstadosFSM.Devolucion)
                .Transicion(TransicionesFSM.CancelarItemDevolucion, EstadosFSM.CancelarItemDevolucion)
                //.Transicion(TransicionesFSM.EstadoConsultarPrecio, EstadosFSM.ConsultarPrecio)
                .Transicion(TransicionesFSM.AgregarCliente, EstadosFSM.Devolucion)
                .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.Devolucion)
                .Transicion(TransicionesFSM.VerTirilla, EstadosFSM.TirillaDevolucion)
                .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarDevolucion)
                .TransicionCondicionada(TransicionesFSM.TerminarDevolucion.GetEnumName(),
                EstadosFSM.TerminarDevolucion.GetEnumName(),
                (() =>
                {
                    return ((Entorno.Instancia.Devolucion.NumeroDeItemsVenta < 0) ? true : false);
                }), "Valida items en venta")
                .TransicionCondicionada(TransicionesFSM.Volver.GetEnumName(),
                EstadosFSM.Vender.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Devolucion.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        string visor = Entorno.Instancia.Vista.PanelVentas.VisorCliente.Descripcion;
                        string empty = "";
                        if (string.Compare(visor, empty) == 0)
                        {
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                            Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                            Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                        }
                        else
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorCliente.LimpiarArticulo();
                        }

                    }
                    return !transaccionAbierta;
                }),
                "Valida items en venta")
                .AlEntrar(() =>
                {
                    //
                    if (Entorno.Instancia.Vista.PanelActivo != null)
                    {
                        if (!(Entorno.Instancia.Vista.PanelActivo is IPanelVentas))
                        {
                            ProcesarSolicitud(new Solicitudes.SolicitudPanelVenta(Solicitud.Vender));
                        }
                    }
                    //
                }, "Vender");


            //CancelarOperacionDevolucion
            stateMachine.Definir(EstadosFSM.CancelarDevolucion)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Devolucion)
                .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                .AlEntrar(() =>
                {
                    ProcesarSolicitud(new SolicitudCancelarDevolucion(Solicitud.CancelarDevolucion));
                }, "CancelarDevolucion");
        }

        private void ConfigVender()
        {
            //CancelarVender
            stateMachine.Definir(EstadosFSM.CancelarVenta)
                .Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.Vender)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                .AlEntrar(() =>
                {
                    ProcesarSolicitud(new SolicitudCancelarVenta(Solicitud.CancelarVenta));
                }, "CancelarVenta")
                .AlSalir(() =>
                {
                    if (!string.IsNullOrEmpty(Entorno.Instancia.Vista.PanelVentas.VisorEntrada))
                        Entorno.Instancia.Vista.PanelVentas.VisorEntrada = "";

                    Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "";
                    Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "";

                }, "CancelarVentaOut");

            //Vender
            stateMachine.Definir(EstadosFSM.Vender)
                .Transicion(TransicionesFSM.Vender, EstadosFSM.Vender)
                .Transicion(TransicionesFSM.AgregarItem, EstadosFSM.Vender)
                .Transicion(TransicionesFSM.CancelarItem, EstadosFSM.CancelarItem)
                .Transicion(TransicionesFSM.AgregarCliente, EstadosFSM.Vender)
                .Transicion(TransicionesFSM.LimpiarVisor, EstadosFSM.Vender)
                .Transicion(TransicionesFSM.Volver, EstadosFSM.Vender)
                .Transicion(TransicionesFSM.VerTirilla, EstadosFSM.TirillaVenta)
                //.Transicion(TransicionesFSM.RegistrarRecogida, EstadosFSM.RegistrarRecogida)
                //.Transicion(TransicionesFSM.CancelarOperacion, EstadosFSM.CancelarVenta)
                .TransicionCondicionada(TransicionesFSM.CancelarOperacion.GetEnumName(), EstadosFSM.CancelarVenta.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    int itemsVenta = Entorno.Instancia.Venta.NumeroDeItemsVenta;

                    if (!transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No hay una transacción abierta";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    else
                    {
                        if (itemsVenta <= 0)
                        {
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No hay artículos para cancelar";
                            Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                        }
                    }

                    return transaccionAbierta;

                }), "CancelarOperacion")
                .TransicionCondicionada(TransicionesFSM.VentaEspecial.GetEnumName(), EstadosFSM.VentaEspecial.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);

                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    else
                    {
                        Respuesta respuestaEstadoImpresora = Entorno.Instancia.Impresora.ValidarEstado();
                        if (!respuestaEstadoImpresora.Valida)
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Revise el estado de la impresora";

                            if (Entorno.Instancia.Impresora.ImpideOperacion)
                            {
                                transaccionAbierta = true;
                                log.Error("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                            }
                            else
                            {
                                log.Warn("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                            }

                        }
                    }


                    return !transaccionAbierta;
                }), "Venta Especial")
                .TransicionCondicionada(TransicionesFSM.SolicitarIntervencionPrestamo.GetEnumName(), EstadosFSM.SolicitarIntervencionPrestamo.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    else
                    {
                        Respuesta respuestaEstadoImpresora = Entorno.Instancia.Impresora.ValidarEstado();
                        if (!respuestaEstadoImpresora.Valida)
                        {
                            if (Entorno.Instancia.Impresora.ImpideOperacion)
                            {
                                transaccionAbierta = true;
                                log.Error("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Revise el estado de la impresora";
                            }
                            else
                            {
                                log.Warn("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                            }

                            //todo: mostrar mensaje de estado de impresora.


                        }
                    }
                    return !transaccionAbierta;
                }), "Terminal Asegurada")
                .TransicionCondicionada(TransicionesFSM.Prestamos.GetEnumName(), EstadosFSM.Prestamos.GetEnumName(), 
                (() => 
                {
                    bool impideTransicion = (Entorno.Instancia.Venta.EstaAbierta);
                    if (impideTransicion)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    else
                    {
                        int intervencion = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.prestamos");
                        switch (intervencion)
                        {
                            case 0:
                                impideTransicion = true;
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual no permite realizar la operación.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "No se permite la operación";
                                break;
                            case 1:
                                impideTransicion = true;
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual solicita intervención.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Se solicita intervención para la operación";
                                break;
                            case 2:
                                impideTransicion = false;
                                break;
                            default:
                                impideTransicion = true;
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.prestamos] tiene un valor de {intervencion} el cual es inválido.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Operación inválida"; break;
                        }
                    }
                    return !impideTransicion;
                }), "Prestamos")
                .TransicionCondicionada(TransicionesFSM.SolicitarIntervencionRecogida.GetEnumName(), EstadosFSM.SolicitarIntervencionRecogida.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    else
                    {
                        Respuesta respuestaEstadoImpresora = Entorno.Instancia.Impresora.ValidarEstado();
                        if (!respuestaEstadoImpresora.Valida)
                        {
                            if (Entorno.Instancia.Impresora.ImpideOperacion)
                            {
                                transaccionAbierta = true;
                                log.Error("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Revise el estado de la impresora";
                            }
                            else
                            {
                                log.Warn("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                            }
                        }
                    }
                    return !transaccionAbierta;
                }), "SolicitarIntervencionRecogida")
                .TransicionCondicionada(TransicionesFSM.Recogida.GetEnumName(), EstadosFSM.Recogida.GetEnumName(),
                (() =>
                {
                    bool impideTransicion = (Entorno.Instancia.Venta.EstaAbierta);
                    if (impideTransicion)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    else
                    {
                        int intervencionRegogida = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.recogidas");
                        switch (intervencionRegogida)
                        {
                            case 0:
                                impideTransicion = true;
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual no permite realizar la operación.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "No se permite la operación";
                                break;
                            case 1:
                                impideTransicion = true;
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual solicita intervención.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Se solicita intervención para la operación";
                                break;
                            case 2:
                                impideTransicion = false;
                                break;
                            default:
                                impideTransicion = true;
                                log.Error($"[Reactor.Ejecutar] El parámetro [pdv.intervencion.recogidas] tiene un valor de {intervencionRegogida} el cual es inválido.");
                                Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Operación inválida"; break;
                        }
                    }
                    return !impideTransicion;
                }), "Recogida")
                .TransicionCondicionada(TransicionesFSM.Ajustes.GetEnumName(), EstadosFSM.Ajustes.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    return !transaccionAbierta;
                }), "Terminal Asegurada")
                .Transicion(TransicionesFSM.EstadoConsultarPrecio, EstadosFSM.ConsultarPrecio)
                .TransicionCondicionada(TransicionesFSM.TerminalAsegurada.GetEnumName(), EstadosFSM.TerminalAsegurada.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    return !transaccionAbierta;
                }), "Terminal Asegurada")
                .TransicionCondicionada(TransicionesFSM.EstadoDevolucion.GetEnumName(), EstadosFSM.Devolucion.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    return !transaccionAbierta;
                }), "Devolucion")
                .TransicionCondicionada(TransicionesFSM.Pagar.GetEnumName(), EstadosFSM.Pago.GetEnumName(),
                (() =>
                {

                    int itemsVenta = Entorno.Instancia.Venta.NumeroDeItemsVenta;
                    bool nroItemsValido = false;
                    bool permitirTransicion = Entorno.Instancia.Venta.EstaAbierta;
                    nroItemsValido = ((itemsVenta > 0) ? true : false);
                    if (permitirTransicion && nroItemsValido)
                    {
                        Respuesta respuestaEstadoImpresora = Entorno.Instancia.Impresora.ValidarEstado();
                        if (!respuestaEstadoImpresora.Valida)
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Revise el estado de la impresora";

                            if (Entorno.Instancia.Impresora.ImpideOperacion)
                            {
                                permitirTransicion = true;
                                log.Error("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                            }
                            else
                            {
                                log.Warn("[Reactor.Ejecutar] La operación no puede continuar debido al estado de la impresora.");
                            }

                        }
                    }
                    else
                    {
                        log.Error("[Reactor.Ejecutar] La operación no puede continuar debido al numero de items en tirilla.");
                        if (!permitirTransicion)
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "No existe transaccion abierta.";
                        }
                        else
                        {
                            Entorno.Instancia.Vista.PanelVentas.VisorMensaje = "Numero de items en tirilla no es valido.";
                        }

                    }

                    return (permitirTransicion && nroItemsValido);
                }), "Valida items en venta")
                .TransicionCondicionada(TransicionesFSM.ImprimirUltima.GetEnumName(), EstadosFSM.Vender.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    return !transaccionAbierta;
                }), "Imprimir Ultima")
                .TransicionCondicionada(TransicionesFSM.PanelCierreDatafono.GetEnumName(), EstadosFSM.PanelCierreDatafono.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    return !transaccionAbierta;
                }), "Cierre Datafono")
                .TransicionCondicionada(TransicionesFSM.Arqueo.GetEnumName(), EstadosFSM.Arqueo.GetEnumName(),
                (() =>
                {
                    bool transaccionAbierta = (Entorno.Instancia.Venta.EstaAbierta);
                    if (transaccionAbierta)
                    {
                        Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "Operación Inválida";
                        Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                        Entorno.Instancia.Vista.EstadoApp.EstadoValido = "[X]";
                    }
                    return !transaccionAbierta;
                }), "Arqueo")
                .AlEntrar(() =>
                {
                    //
                    if (Entorno.Instancia.Vista.PanelActivo != null)
                    {
                        if (!(Entorno.Instancia.Vista.PanelActivo is IPanelVentas))
                        {
                            ProcesarSolicitud(new Solicitudes.SolicitudPanelVenta(Solicitud.Vender));
                        }
                    }
                    //
                }, "Vender");
            //
        }

        private Solicitud ObtenerSolicitudPorEstado(EstadosFSM estadoAnterior)
        {
            Solicitud solicitud = Solicitud.Vender;
            try
            {
                switch (estadoAnterior)
                {
                    case EstadosFSM.Inicio:
                        break;
                    case EstadosFSM.IniciarSesion:
                        break;
                    case EstadosFSM.InicioSesion:
                        break;
                    case EstadosFSM.Vender:
                        break;
                    case EstadosFSM.Pago:
                        break;
                    case EstadosFSM.TirillaVenta:
                        break;
                    case EstadosFSM.Fin:
                        break;
                    case EstadosFSM.NoVenta:
                        break;
                    case EstadosFSM.RegistrarDispositivos:
                        break;
                    case EstadosFSM.EstadosDispositivos:
                        break;
                    case EstadosFSM.TerminalAsegurada:
                        break;
                    case EstadosFSM.Prestamos:
                        break;
                    case EstadosFSM.Recogida:
                        break;
                    case EstadosFSM.RegistrarRecogida:
                        break;
                    case EstadosFSM.CancelarItem:
                        break;
                    case EstadosFSM.Devolucion:
                        solicitud = Solicitud.EstadoDevolucion;
                        break;
                    case EstadosFSM.PagoDatafono:
                        break;
                    case EstadosFSM.ConsultarPrecio:
                        break;
                    case EstadosFSM.TirillaDevolucion:
                        break;
                    case EstadosFSM.TerminarDevolucion:
                        break;
                    case EstadosFSM.ReintentoPago:
                        break;
                    case EstadosFSM.CancelarItemDevolucion:
                        break;
                    case EstadosFSM.CancelarTransaccion:
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[StateMachine_OnTransitionedEvent] {0}", ex.Message);
            }
            return solicitud;
        }

        private void StateMachine_OnTransitionedEvent(string estado)
        {
            try
            {
                EstadoFSMAnterior = EstadoFSMActual;
                EstadoFSMActual = (EstadosFSM)Enum.Parse(typeof(EstadosFSM), estado);
                log.InfoFormat("[StateMachine_OnTransitionedEvent] Estado anterior: {0}", EstadoFSMAnterior.GetEnumName());
                log.InfoFormat("[StateMachine_OnTransitionedEvent] Estado actual: {0}", estado);

                //Estados que no deben se visualizados.
                List<EstadosFSM> estadosNoVisibles = new List<EstadosFSM>()
                {
                    EstadosFSM.IniciarSesion,
                    EstadosFSM.InicioSesion,
                    EstadosFSM.TerminalAsegurada,
                    EstadosFSM.RegistrarDispositivos,
                    EstadosFSM.EstadosDispositivos,
                    EstadosFSM.PantallaCliente
                };


                //Interface
                if (!estadosNoVisibles.Contains(EstadoFSMActual))
                {
                    Entorno.Instancia.Vista.EstadoApp.EstadoActual = EstadoFSMActual.GetEnumName();
                    Entorno.Instancia.Vista.EstadoApp.EstadoFSM = EstadoFSMActual;
                    Entorno.Instancia.Vista.EstadoApp.EstadoValido = ""; //Se modifica al lanzar una transición.

                    if (Entorno.Instancia.Vista.PanelVentas != null)
                        Entorno.Instancia.Vista.PanelVentas.VisorEntrada = "";
                }
                else
                {
                    Entorno.Instancia.Vista.EstadoApp.EstadoActual = "";
                    Entorno.Instancia.Vista.EstadoApp.EstadoFSM = EstadoFSMActual;
                    Entorno.Instancia.Vista.EstadoApp.EstadoValido = "";

                    if (Entorno.Instancia.Vista.PanelVentas != null)
                        Entorno.Instancia.Vista.PanelVentas.VisorEntrada = "";
                }
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[StateMachine_OnTransitionedEvent] {0}", ex.Message);
            }
        }

        #endregion

    }
}