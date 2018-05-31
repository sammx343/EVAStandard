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
    public class SolicitudPanelRecogidas : ISolicitud
    {
        public Solicitud TipoSolicitud
        {
            get;
            set;
        }

        public string CodigoRecogida { get; set; }

        /// <summary>
        /// Inicializa una solicitud de panel de recogida.
        /// </summary>
        /// <param name="solicitud">Tipo de Solicitud</param>
        /// <param name="codigo">Código de la recogida</param>
        public SolicitudPanelRecogidas(Solicitud solicitud, string codigo)
        {
            TipoSolicitud = solicitud;
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
