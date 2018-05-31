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
    public class CmdEstadoDevolucion : ComandoAbstract
    { 
        public Solicitudes.SolicitudDevolucion Solicitud { get; set; }

        public CmdEstadoDevolucion(ISolicitud solicitud) : base(solicitud)
        {
            if (!Entorno.Instancia.Venta.EstaAbierta)
            {
                Solicitud = solicitud as Solicitudes.SolicitudDevolucion;
                Entorno.Instancia.Devolucion = new EDevolucion();
                Entorno.Instancia.Devolucion.EstaAbierta = false;
            }
            else
            {
                log.Warn("[CmdEstadoDevolucion] Ya hay una venta en curso.");
            }

        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("EstadoDevolucion"));

            log.Info("[CmdEstadoDevolucion] Cambio de estado para devolucion.");
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
