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
    public class SolicitudIniciarSesion : ISolicitud
    {
        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public string IdUsuario { get; set; }
        public string Clave { get; set; }

        public SolicitudIniciarSesion(Solicitud solicitud, string usuario, string clave)
        {
            this.TipoSolicitud = solicitud;

            this.IdUsuario = usuario;
            this.Clave = clave;
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
