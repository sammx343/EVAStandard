﻿using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    class SolicitudCancelarVentaEspecial : ISolicitud
    {
        public Solicitud TipoSolicitud { get; set; }

        public SolicitudCancelarVentaEspecial(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }
    }
}
