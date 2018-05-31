using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdImprimirUltima : ComandoAbstract
    {
        Solicitudes.SolicitudImprimirUltima solicitudImprimirUltima;

        public CmdImprimirUltima(ISolicitud solicitud) : base(solicitud)
        {
            solicitudImprimirUltima = solicitud as Solicitudes.SolicitudImprimirUltima;
        }

        public override void Ejecutar()
        {
            PVenta pventa = new PVenta();
            Respuesta respuesta = new Respuesta();
            EUsuario usuario = Entorno.Instancia.Usuario;
            ETerminal terminal = Entorno.Instancia.Terminal;
            string ultima = pventa.ImprimirUltima(terminal.Codigo, usuario.IdUsuario, out respuesta);

            if (string.IsNullOrEmpty(ultima))
            {
                Entorno.Vista.PanelVentas.VisorMensaje = "No se pudo imprimir la última factura";
            }
            else
            {
                // Imprimir
                Entorno.Instancia.Impresora.Imprimir(ultima, cortarPapel: true, abrirCajon: false);

                Telemetria.Instancia.AgregaMetrica(new Evento("ImprimirUltimaFactura").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (ultima)));

                log.InfoFormat("[ImprimirUltima] Ultima factura impresa. Factura: {0}", ultima);
            }

            //
            if (Config.ViewMode == InternalSettings.ModoTouch)
                Entorno.Instancia.Vista.PanelVentas.LimpiarOperacion();
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
