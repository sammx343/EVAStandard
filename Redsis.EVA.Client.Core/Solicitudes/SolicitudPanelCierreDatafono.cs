using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudPanelCierreDatafono:ISolicitud
    {
        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public SolicitudPanelCierreDatafono(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }
    }
}
