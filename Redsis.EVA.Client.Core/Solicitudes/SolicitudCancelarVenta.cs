using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudCancelarVenta : ISolicitud
    {
        public Solicitud TipoSolicitud { get; set; }

        public SolicitudCancelarVenta(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }
    }
}
