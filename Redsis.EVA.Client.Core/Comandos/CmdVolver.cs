using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS.Enums;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdVolver : ComandoAbstract
    {
        Solicitudes.SolicitudVolver solicitudVolver;

        public CmdVolver(ISolicitud solicitud) : base(solicitud)
        {
            solicitudVolver = solicitud as Solicitudes.SolicitudVolver;
        }

        public override void Ejecutar()
        {
            //Telemetria.Instancia.AgregaMetrica(new Evento("Volver"));

            log.Info("[CmdVolver] Volver.");
            if (Reactor.Instancia.EstadoFSMAnterior == EstadosFSM.Devolucion)
            {
                Entorno.Instancia.Devolucion = null;
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
