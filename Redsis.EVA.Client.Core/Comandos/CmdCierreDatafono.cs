using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdCierreDatafono : ComandoAbstract
    {
        Solicitudes.SolicitudCierreDatafono Solicitud;

        public CmdCierreDatafono(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudCierreDatafono;
        }

        public override void Ejecutar()
        {
            //iu.PanelCierreDatafono.CierreIntegrado();     
        }
    }
}
