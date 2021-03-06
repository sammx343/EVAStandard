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
    public class SolicitudValidarIntervencion : ISolicitud
    {
        public Solicitud TipoSolicitud { get; set; }

        public string CodigoRecogida { get; set; }
        public string ValorEntrada { get; set; }

        public SolicitudValidarIntervencion(Solicitud solicitud, string valorEntrada, string codigo = "")
        {
            TipoSolicitud = solicitud;
            ValorEntrada = valorEntrada;
            CodigoRecogida = codigo;
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
