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
    public class SolicitudCierreDatafono : ISolicitud
    {
        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public SolicitudCierreDatafono(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }
    }
}
