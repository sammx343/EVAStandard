using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelInicioSesion : ComandoAbstract
    {
        Solicitudes.SolicitudPanelInicioSesion solicitudInicioSesion;

        public CmdPanelInicioSesion(ISolicitud solicitud) : base(solicitud)
        {
            solicitudInicioSesion = solicitud as Solicitudes.SolicitudPanelInicioSesion;
        }

        public override void Ejecutar()
        {
            Telemetria.Instancia.AgregaMetrica(new Evento("PanelInicioSesion"));
            log.Info("[CmdPanelInicioSesion] Mostrar panel Inicio de sesión");
            iu.MostrarPanelInicioSesion();
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
