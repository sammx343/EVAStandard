using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudCancelar_CancelarItem : ISolicitud
    {
        public Solicitud TipoSolicitud { get; set; }

        public SolicitudCancelar_CancelarItem(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }
    }
}
