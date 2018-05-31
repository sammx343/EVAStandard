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
    public class SolicitudVista : ISolicitud
    {
        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public Paneles Panel { get; set; }

        public SolicitudVista(Solicitud sol, Paneles panel)
        {
            TipoSolicitud = sol;
            Panel = panel;
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
