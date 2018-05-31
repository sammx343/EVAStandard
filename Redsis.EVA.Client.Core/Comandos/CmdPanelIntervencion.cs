using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelIntervencion : ComandoAbstract
    {
        Solicitudes.SolicitudSolicitarIntervencion Solicitud;
        public string CodigoRecogida { get; set; }

        public CmdPanelIntervencion(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudSolicitarIntervencion;

            if (Solicitud != null)
                CodigoRecogida = Solicitud.CodigoRecogida;
        }

        public override void Ejecutar()
        {
            try
            {
                Entorno.Instancia.Recogida = null;
                Entorno.Instancia.Prestamo = null;

                if (Solicitud.TipoSolicitud == Enums.Solicitud.SolicitarIntervencionRecogida)
                {
                    int solicitaIntervencionRecogida = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.recogidas");

                    switch (solicitaIntervencionRecogida)
                    {
                        case 0:
                            Entorno.Instancia.Recogida = null;

                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No se pueden realizar recogidas.";
                            Solicitudes.SolicitudVolver volver = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                            Reactor.Instancia.Procesar(volver);
                            break;
                        case 1:
                            if (Config.ViewMode == InternalSettings.ModoConsola)
                            {
                                IniciarRecogida();

                                log.Info("[CmdPanelIntervencion] Mostrando panel intervención.");
                                iu.MostrarPanelIntervencion(Solicitud.CodigoRecogida);
                                iu.PanelIntervencion.VisorEntrada = string.Empty;
                                iu.PanelIntervencion.VisorMensaje = string.Empty;
                            }
                            break;
                        case 2:
                            Solicitudes.SolicitudPanelRecogidas panelRecogida = new Solicitudes.SolicitudPanelRecogidas(Enums.Solicitud.Recogida, CodigoRecogida);
                            Reactor.Instancia.Procesar(panelRecogida);
                            break;
                        default:
                            log.Error("[CmdPanelIntervencion] Intervención Recogida: Opción inválida.");
                            break;
                    }
                }
                else if (Solicitud.TipoSolicitud == Enums.Solicitud.SolicitarIntervencionPrestamo)
                {
                    int solicitaIntervencionPrestamos = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.intervencion.prestamos");

                    switch (solicitaIntervencionPrestamos)
                    {
                        case 0:
                            Entorno.Instancia.Prestamo = null;

                            Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No se pueden realizar prestamos.";
                            Solicitudes.SolicitudVolver volver = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
                            Reactor.Instancia.Procesar(volver);
                            break;
                        case 1:
                            if (Config.ViewMode == InternalSettings.ModoConsola)
                            {
                                log.Info("[CmdPanelIntervencion] Mostrando panel intervención.");
                                iu.MostrarPanelIntervencion(Solicitud.CodigoRecogida);
                                iu.PanelIntervencion.VisorEntrada = string.Empty;
                                iu.PanelIntervencion.VisorMensaje = string.Empty;
                            }
                            break;
                        case 2:
                            Solicitudes.SolicitudPanelPrestamos panelRecogida = new Solicitudes.SolicitudPanelPrestamos(Enums.Solicitud.Prestamos, CodigoRecogida);
                            Reactor.Instancia.Procesar(panelRecogida);
                            break;
                        default:
                            log.Error("[CmdPanelIntervencion] Intervención Prestamo: Opción inválida.");
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error($"[CmdPanelIntervencion] Error { ex.Message }");
            }
        }

        private void IniciarRecogida()
        {
            // código de la recogida
            string codigo = string.Empty;
            if (Entorno.Instancia.Vista.ModalRecogidas != null)
                codigo = Entorno.Instancia.Vista.ModalRecogidas.CodigoRecogida;
            else
                codigo = Solicitud.CodigoRecogida;

            ECodigoRecogida eCodigo = Entorno.Instancia.CodigosRecogida.CodigoRecogida(codigo);

            //
            List<decimal> listRecogidas = new List<decimal>();
            Entorno.Instancia.Recogida = new Entidades.ERecogida(eCodigo, listRecogidas);
            Entorno.Instancia.Recogida.listRecogidas = new List<decimal>();
            Entorno.Instancia.Recogida.EstaAbierta = false;
        }
    }
}
