﻿using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudCancelarConsultaPrecio : ISolicitud
    {
        public Solicitud TipoSolicitud { get; set; }

        public SolicitudCancelarConsultaPrecio(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }
    }
}
