using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarVentaEspecialSinMedioPago : ComandoAbstract
    {
        public Solicitudes.SolicitudAgregarVentaEspecial Solicitud { get; set; }
        public string idventa;

        public CmdAgregarVentaEspecialSinMedioPago(ISolicitud solicitud) : base(solicitud)
        {
            if (!Entorno.Instancia.Venta.EstaAbierta)
            {
                Solicitud = solicitud as Solicitudes.SolicitudAgregarVentaEspecial;
                idventa = Solicitud.ValorEntrada;
                EVentaEspecial ventaEspecial = Entorno.Instancia.TipoVentaEspecial.Venta(idventa);
                Entorno.Instancia.VentaEspecialSinMedioPago = new EFacturaVentaEspecialSinMedioPago(ventaEspecial);
                Entorno.Instancia.VentaEspecialSinMedioPago.EstaAbierta = false;
            }
            else
            {
                log.Debug("[CmdAgregarVentaEspecialSinMedioPago] Ya hay una venta en curso.");
            }

        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("EstadoVentaEspecial"));

            log.Info("[CmdAgregarVentaEspecialSinMedioPago] Cambio de estado para venta especial.");
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
