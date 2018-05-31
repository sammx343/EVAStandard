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
    public class SolicitudConsultarArticulo : ISolicitud
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string ValorEntrada { get; private set; }

        public Solicitud TipoSolicitud
        {
            get;
            set;
        }

        public SolicitudConsultarArticulo(Solicitud solicitudId, string valorEntrada)
        {
            ValorEntrada = valorEntrada;
            this.TipoSolicitud = solicitudId;
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(TipoSolicitud.ToDescriptionString(), Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
