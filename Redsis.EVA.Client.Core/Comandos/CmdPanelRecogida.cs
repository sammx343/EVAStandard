using EvaPOS;
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
    public class CmdPanelRecogida : ComandoAbstract
    {
        Solicitudes.SolicitudPanelRecogidas Solicitud;
        string codigoRecogida = string.Empty;

        public CmdPanelRecogida(ISolicitud solicitud) : base(solicitud)
        {
            this.Solicitud = solicitud as Solicitudes.SolicitudPanelRecogidas;
            codigoRecogida = Solicitud.CodigoRecogida;
        }

        public override void Ejecutar()
        {
            if (Config.ViewMode == InternalSettings.ModoConsola)
                IniciarRecogida();

            Telemetria.Instancia.AgregaMetrica(new Evento("PanelRecogidas"));

            log.Info("[CmdPanelRecogida] Mostrando panel recogidas.");
            if (Config.ViewMode == InternalSettings.ModoConsola)
            {
                iu.MostrarPanelRecogida(Solicitud.CodigoRecogida);
                iu.PanelRecogidas.VisorEntrada = string.Empty;
                iu.PanelRecogidas.VisorMensaje = string.Empty;
                //iu.PanelOperador.MensajeOperador = string.Empty;
            }
            else
            {
                iu.MostrarModalRecogida();
                iu.PanelOperador.MensajeOperador = Solicitud.CodigoRecogida;
            }

            try
            {
                Entorno.Instancia.Impresora?.AbrirCajon();
            }
            catch (Exception ex)
            {
                Entorno.Instancia.Vista.PanelOperador.MensajeOperador = "No se pudo abrir el cajón monedero.";
                log.Info("Error al abrir cajón monedero: " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }

        private void AbrirCajon()
        {

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

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
