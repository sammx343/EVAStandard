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
    public class SolicitudComprobarEstadoDispositivos : ISolicitud
    {
       public List<IDetalleDispositivo> ListaDispositivos { get; private set; }

        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public SolicitudComprobarEstadoDispositivos(Solicitud solicitud, List<IDetalleDispositivo> listaDispositivos)
        {
            ListaDispositivos = listaDispositivos;
            TipoSolicitud = solicitud;
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
