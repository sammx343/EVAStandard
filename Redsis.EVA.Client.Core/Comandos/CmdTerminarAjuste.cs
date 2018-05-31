using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdTerminarAjuste : ComandoAbstract
    {
        Solicitudes.SolicitudTerminarAjuste Solicitud;
        public string CodigoAjuste { get; private set; }

        public CmdTerminarAjuste(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as Solicitudes.SolicitudTerminarAjuste;
            CodigoAjuste = Solicitud.ValorEntrada;
        }

        public override void Ejecutar()
        {
            Respuesta respuesta = new Respuesta();
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            PAjuste Ajuste = new PAjuste();
            ETipoAjuste tipoAjuste = Entorno.Instancia.TiposAjustes.TiposAjuste.FirstOrDefault(x => x.Codigo.Equals(CodigoAjuste));

            if (!(tipoAjuste == null))
            {

                // Imprimir factura
                string factura = ProcesarPlantilla.Ajuste(Entorno.Instancia.Ajuste, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, tipoAjuste.Descripcion);
                string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

                //
                log.InfoFormat("[CmdPagarVenta] Ajuste Finalizado, Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                string tirillaActual = "";
                iu.PanelVentas.Tirilla.ForEach(x =>
                {
                    tirillaActual += String.Format("Codigo: {0}, Descripción: {1} ({4}), Cantidad: {2}, Precio: {3} ", x.Codigo, x.Descripcion, x.Cantidad, x.Subtotal, x.PrecioVentaUnidad);
                    tirillaActual += Environment.NewLine;
                });
                log.Info("Items tirilla:" + tirillaActual + Environment.NewLine + "Total: " + Entorno.Instancia.Ajuste.TotalVenta + Environment.NewLine + "Totales Impuestos: " + Entorno.Instancia.Ajuste.ImpuestosIncluidos.Sum(x => x.Value[2]) + Environment.NewLine + "Cant Artículos Vendidos: " + Entorno.Instancia.Ajuste.NumeroDeItemsVenta + Environment.NewLine);

                var tiempoGuardarAjuste = new MetricaTemporizador("TerminarAjusteFinalizado");
                Ajuste.GuardarAjuste(Entorno.Instancia.Ajuste, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, Entorno.Instancia.Terminal.Localidad, tipoAjuste, factura, modeloImpresora, out respuesta);
                if (respuesta.Valida == false)
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarAjuste.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Ajuste.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Ajuste.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Ajuste.NumeroDeItemsVenta).AgregarPropiedad("Error", respuesta.Mensaje));
                }
                else
                {
                    //Log a azure
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarAjuste.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)).AgregarPropiedad("TotalVenta", Entorno.Instancia.Ajuste.TotalVenta).AgregarPropiedad("TotalImpuestoVenta", Entorno.Instancia.Ajuste.ImpuestosIncluidos.Sum(x => x.Value[2])).AgregarPropiedad("NroArticulosVenta", Entorno.Instancia.Ajuste.NumeroDeItemsVenta));

                    // Imprimir
                    Entorno.Instancia.Impresora.Imprimir(factura, cortarPapel: true, abrirCajon: false);

                    //
                    Entorno.Instancia.IdsAcumulados = idsAcumulados;

                    //
                    respuesta = new Respuesta(false);
                    ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
                    Entorno.Instancia.Terminal = terminal;

                    LimpiarVentaFinalizada();
                    log.Info("[CmdTerminarAjuste] Ajuste finalizado");

                    //
                    decimal valorCambio = Entorno.Instancia.Ajuste.TotalVenta * -1;
                    iu.PanelVentas.VisorCliente.Total = 0;
                    //iu.PanelVentas.VisorMensaje = string.Format("Cambio: {0}", valorCambio.ToString("C"));

                    //
                    log.InfoFormat("[CmdTerminarAjuste] --> Transaccion finalizada");

                    iu.PanelOperador.CodigoCliente = "";
                    //
                    iu.PanelVentas.LimpiarOperacion();

                    Entorno.Instancia.Ajuste.EstaAbierta = false;
                    Entorno.Instancia.Ajuste = null;
                }
            }
        }

        private void LimpiarVentaFinalizada()
        {
            //
            iu.PanelVentas.LimpiarVentaFinalizada();

            //
            Solicitudes.SolicitudVolver solicitudPanelVenta = new Solicitudes.SolicitudVolver(Enums.Solicitud.Volver);
            Reactor.Instancia.Procesar(solicitudPanelVenta);
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
