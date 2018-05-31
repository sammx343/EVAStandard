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
    public class SolicitudPanelAjuste : ISolicitud
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public Solicitud TipoSolicitud
        {
            get;
            set;
        }

        public SolicitudPanelAjuste(Solicitud solicitudID)
        {
            this.TipoSolicitud = solicitudID;
        }

        public override string ToString()
        {
            string ans = string.Empty;

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(TipoSolicitud.ToDescriptionString(), Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
