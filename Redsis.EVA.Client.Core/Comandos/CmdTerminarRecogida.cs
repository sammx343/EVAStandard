using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Entidades;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdTerminarRecogida : ComandoAbstract
    {
        public Solicitudes.SolicitudTerminarRecogida Solicitud { get; set; }

        public CmdTerminarRecogida(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudTerminarRecogida;
        }

        public override void Ejecutar()
        {
            if (Solicitud.TipoSolicitud == Enums.Solicitud.TerminarRecogida)
            {
                //
                PRecogida pRecogida = new PRecogida();
                Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
                EMedioPago medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");

                //
                decimal totalRecogida = Entorno.Instancia.Recogida.listRecogidas.Sum();
                //Entorno.Instancia.Recogida.AgregarValor(totalRecogida);
                if (totalRecogida <= 0)
                {
                    throw new Exception("El valor no puede ser vacío o igual a cero.");
                }

                // Terminar Recogida
                // Generar Factura
                Respuesta respuesta = new Respuesta();
                string factura = ProcesarPlantilla.Recogidas(Entorno.Instancia.Recogida);
                string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

                var tiempoGuardarRecogida = new MetricaTemporizador("RecogidaAgregada");
                pRecogida.GuardarRecogida(Entorno.Instancia.Recogida, ref idsAcumulados, ((int)TipoTransaccion.Recogida).ToString(), Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, medioPago, factura ?? "contenido", modeloImpresora, out respuesta);

                // obtener id_venta de la recogida realizada
                string idVentaRecogida = respuesta.Mensaje;

                if (respuesta.Valida == false)
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarRecogida.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Valor", (Entorno.Instancia.Prestamo.Valor)).AgregarPropiedad("Error", respuesta.Mensaje));
                }
                else
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarRecogida.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Valor", (Entorno.Instancia.Recogida.Valor)).AgregarPropiedad("CodigoRecogida", (Entorno.Instancia.Recogida.CodigoRecogida.Codigo)));

                    log.Info("[CmdTerminarRecogida] Agregar Recogida: " + totalRecogida);

                    respuesta = new Respuesta(false);
                    ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                    if (respuesta.Valida == true)
                    {
                        if (Entorno.Instancia.Usuario.UsuarioSupervisor != null)
                        {
                            PIntervencion pInterv = new PIntervencion();
                            EIntervencion eInterv = new EIntervencion();

                            eInterv.id_venta = idVentaRecogida;
                            eInterv.claveSupervisor = Entorno.Instancia.Usuario.UsuarioSupervisor.ClaveSupervisor;
                            eInterv.motivo = "Intervención recogida";
                            eInterv.nro_transac = Convert.ToInt32(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                            pInterv.GuardarIntervencion(eInterv, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, out respuesta);
                        }

                        Entorno.Instancia.Terminal = terminal;
                        Entorno.Instancia.IdsAcumulados = idsAcumulados;
                        Entorno.Instancia.Recogida = null;
                        Entorno.Instancia.Usuario.UsuarioSupervisor = null;

                        iu.PanelVentas.VisorMensaje = "Recogida registrada correctamente.";
                        iu.PanelVentas.VisorEntrada = string.Empty;

                        if (Config.ViewMode == InternalSettings.ModoTouch)
                        {
                            Entorno.Instancia.Vista.PanelVentas.LimpiarOperacion();
                            Entorno.Instancia.Vista.ModalRecogidas.CodigoRecogida = string.Empty;
                        }
                        else
                            Entorno.Instancia.Vista.PanelRecogidas.VisorEntrada = string.Empty;

                        log.Info("[CmdTerminarRecogida] Recogida registrada correctamente.");

                        // Imprimir
                        Entorno.Instancia.Impresora.Imprimir(factura, cortarPapel: true, abrirCajon: false);

                        log.Info("[CmdTerminarRecogida] Imprimir Operación: " + factura);
                        //ePrestamo = null;
                        Solicitudes.SolicitudPanelVenta volver = new Solicitudes.SolicitudPanelVenta(Enums.Solicitud.Vender);
                        Reactor.Instancia.Procesar(volver);

                    }
                }
            }
            else if (Solicitud.TipoSolicitud == Enums.Solicitud.TerminarPrestamo)
            {
                
            }

        }
    }
}
