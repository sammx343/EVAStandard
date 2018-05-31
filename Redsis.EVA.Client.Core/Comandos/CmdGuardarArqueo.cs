using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdGuardarArqueo : ComandoAbstract
    {
        public CmdGuardarArqueo(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            PArqueo pArqueo = new PArqueo();
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            Respuesta respuesta = new Respuesta();

            //
            string arqueo = ProcesarPlantilla.Arqueo(Entorno.Instancia.Vista.PanelArqueo.Caja);
            string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

            var tiempoGuardarArqueo = new MetricaTemporizador("GuardarArqueo");
            pArqueo.GuardarArqueo(Entorno.Instancia.Vista.PanelArqueo.Caja, ref idsAcumulados, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, ((int)TipoTransaccion.Arqueo).ToString(), arqueo, modeloImpresora, out respuesta);

            if (!respuesta.Valida)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoGuardarArqueo.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Error", respuesta.Mensaje));
            }
            else
            {
                Dictionary<EMedioPago, List<decimal>> arqueos = Entorno.Instancia.Vista.PanelArqueo.Caja.Arqueo;
                tiempoGuardarArqueo.Para();
                foreach (var ar in arqueos)
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarArqueo.AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Caja", ar.Value[0]).AgregarPropiedad("Conteo", ar.Value[1]).AgregarPropiedad("Diferencia", ar.Value[2]));
                    tiempoGuardarArqueo.Props.Clear();
                }
                
                //Telemetria.Instancia.AgregaMetrica(tiempoGuardarArqueo.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Resultados",arqueos));
                
                //
                string resultadosArqueo = "";
                foreach (var item in Entorno.Instancia.Vista.PanelArqueo.Caja.Arqueo)
                {
                    resultadosArqueo += Environment.NewLine;
                    resultadosArqueo += String.Format("Medio Pago: {0}, Valor en Caja: {1}, Valor ingresado: {2}, Diferencia: {3} ", item.Key, item.Value[0], item.Value[1], item.Value[2]);
                }
                log.Info("[CmdGuardarArqueo] Arqueo registrado correctamente. Resultados:" + resultadosArqueo);

                //
                ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);

                //
                Solicitudes.SolicitudPanelVenta volver = new Solicitudes.SolicitudPanelVenta(Solicitud.Vender);
                Reactor.Instancia.Procesar(volver);

                // Imprimir
                log.Info("[CmdGuardarArqueo] Copia Impresión: " + Environment.NewLine + arqueo);
                Entorno.Instancia.Impresora.Imprimir(arqueo, cortarPapel: true, abrirCajon: false);

                Entorno.Instancia.Terminal = terminal;
                Entorno.Instancia.Vista.PanelArqueo.Caja = null;
            }
        }
    }
}
