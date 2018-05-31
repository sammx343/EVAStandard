using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdEstadoArqueo : ComandoAbstract
    {
        public Solicitudes.SolicitudEstadoArqueo Solicitud { get; set; }

        public CmdEstadoArqueo(ISolicitud solicitud) : base(solicitud)
        {
            if (!Entorno.Instancia.Venta.EstaAbierta)
            {
                Solicitud = solicitud as Solicitudes.SolicitudEstadoArqueo;
            }
            else
            {
                log.Warn("[CmdEstadoArqueo] Ya hay una venta en curso.");
            }

        }

        public override void Ejecutar()
        {
            Respuesta res = new Respuesta();
            PArqueo parqueo = new PArqueo();
            PMediosPago pmediospago = new PMediosPago();
            EMediosPago mediosPago = pmediospago.GetAllMediosPago();
            Entorno.Instancia.Vista.PanelArqueo.Caja = parqueo.obtenerEcaja(Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, mediosPago, out res);
            Entorno.Instancia.Vista.PanelArqueo.CargarCaja();

            //Telemetria.Instancia.AgregaMetrica(new Evento("EstadoArqueo"));

            log.Info("[CmdEstadoArqueo] Inicio de Arqueo.");
            Telemetria.Instancia.AgregaMetrica(new Evento("InicioArqueo").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));

            try
            {
                if (Entorno.Instancia.Impresora != null)
                {
                    Entorno.Instancia.Impresora.AbrirCajon();
                }
            }
            catch (Exception ex)
            {
                Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No se pudo abrir el cajón monedero.";
                log.Info("Error al abrir cajón monedero: " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
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
