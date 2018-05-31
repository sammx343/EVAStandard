using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using EvaPOS;
using EvaPOS.Enums;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdValidarIntervencion : ComandoAbstract
    {
        Solicitudes.SolicitudValidarIntervencion Solicitud;
        private string codigoRecogida { get; set; }

        public CmdValidarIntervencion(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudValidarIntervencion;
            codigoRecogida = Solicitud.CodigoRecogida;
        }

        public override void Ejecutar()
        {
            try
            {
                Respuesta respuesta = new Respuesta();

                // validar contraseña
                string password = Entorno.Instancia.Vista.PanelIntervencion.VisorEntrada;
                string tokenSupervisor = Entorno.Instancia.Usuario.ClaveSupervisor;

                PUsuario usuarioPer = new PUsuario();

                if (!string.IsNullOrEmpty(password))
                {
                    Entidades.EUsuario supervisor = usuarioPer.ValidarClaveSupervisor(Entorno.Instancia.Usuario.Usuario, password, out respuesta);

                    if (respuesta.Valida)
                    {
                        Entorno.Instancia.Usuario.UsuarioSupervisor = supervisor;
                        if (Solicitud.TipoSolicitud == Enums.Solicitud.ValidarIntervencionPrestamo)
                        {
                            Telemetria.Instancia.AgregaMetrica(new Evento("PanelPrestamos"));

                            log.Info("[CmdPanelPrestamo] Mostrando panel prestamos.");
                            if (Config.ViewMode == InternalSettings.ModoConsola)
                            {
                                Solicitudes.SolicitudPanelPrestamos solPrestamos = new Solicitudes.SolicitudPanelPrestamos(Enums.Solicitud.Prestamos, codigoRecogida);
                                Reactor.Instancia.Procesar(solPrestamos);
                            }
                            else
                            {
                                iu.MostrarModalRecogida();
                                iu.PanelOperador.MensajeOperador = Entorno.Instancia.Vista.PanelIntervencion.CodigoRecogida;
                            }
                        }
                        else if (Solicitud.TipoSolicitud == Enums.Solicitud.ValidarIntervencionRecogida)
                        {
                            if (string.IsNullOrEmpty(codigoRecogida))
                            {
                                return;
                            }

                            Telemetria.Instancia.AgregaMetrica(new Evento("PanelRecogidas"));

                            log.Info("[CmdPanelRecogida] Mostrando panel recogidas.");
                            if (Config.ViewMode == InternalSettings.ModoConsola)
                            {
                                Solicitudes.SolicitudPanelRecogidas solRecogida = new Solicitudes.SolicitudPanelRecogidas(Enums.Solicitud.Recogida, codigoRecogida);
                                Reactor.Instancia.Procesar(solRecogida);
                            }
                            else
                            {
                                iu.MostrarModalRecogida();
                                iu.PanelOperador.MensajeOperador = Entorno.Instancia.Vista.PanelIntervencion.CodigoRecogida;
                            }
                        }

                        try
                        {
                            //Entorno.Instancia.Impresora?.AbrirCajon();
                        }
                        catch (Exception ex)
                        {
                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No se pudo abrir el cajón monedero.";
                            log.Info("Error al abrir cajón monedero: " + ex.Message);
                            Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                        }
                    }
                    else
                    {
                        //
                        MostrarPanelVenta();
                        iu.PanelVentas.VisorMensaje = "Contraseña incorrecta.";
                    }
                }
                else
                {
                    iu.PanelIntervencion.VisorMensaje = "Ingrese contraseña.";
                    log.WarnFormat("Se debe ingresar contraseña de intervención");

                    if (Reactor.Instancia.EstadoFSMActual == EstadosFSM.ValidarIntervencionPrestamo)
                    {
                        var solicitudOperacion = new SolicitudValidarIntervencion(Enums.Solicitud.SolicitarIntervencionPrestamo, Entorno.Instancia.Vista.PanelIntervencion.VisorEntrada, Entorno.Instancia.Vista.PanelIntervencion.CodigoRecogida);
                        Reactor.Instancia.Procesar(solicitudOperacion);
                    }
                    else if (Reactor.Instancia.EstadoFSMActual == EstadosFSM.ValidarIntervencionRecogida)
                    {
                        var solicitudOperacion = new SolicitudValidarIntervencion(Enums.Solicitud.SolicitarIntervencionRecogida, Entorno.Instancia.Vista.PanelIntervencion.VisorEntrada, Entorno.Instancia.Vista.PanelIntervencion.CodigoRecogida);
                        Reactor.Instancia.Procesar(solicitudOperacion);
                    }



                }


            }
            catch (Exception ex)
            {
                log.Error($"[Ejecutar] Error: {ex.Message}");
                throw;
            }
        }

        private void MostrarPanelVenta()
        {
            //
            Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
            Reactor.Instancia.Procesar(solicitudPanelVenta);
        }
    }
}
