﻿using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudPanelPrestamos : ISolicitud
    {

        public SolicitudPanelPrestamos(Solicitud solicitud)
        {
            TipoSolicitud = solicitud;
        }

        public SolicitudPanelPrestamos(Solicitud solicitud, string tipoPrestamo)
        {
            TipoSolicitud = solicitud;
            TipoPrestamo = tipoPrestamo;
        }

        public Solicitud TipoSolicitud
        {
            get;
            set;
        }

        public string TipoPrestamo {
            get;
            set;
        }

        public override string ToString()
        {
            string ans = "";
            if (this != null)
            {
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(TipoSolicitud.ToDescriptionString(), Newtonsoft.Json.Formatting.Indented);
            }
            return ans;
        }
    }
}
