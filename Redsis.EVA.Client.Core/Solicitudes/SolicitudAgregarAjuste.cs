using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudAgregarAjuste : ISolicitud
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string CodigoAjuste { get; set; }
        public Solicitud TipoSolicitud
        {
            get;
            set;
        }

        public SolicitudAgregarAjuste(Solicitud solicitudId, string valorEntrada)
        {
            CodigoAjuste = valorEntrada;
            this.TipoSolicitud = solicitudId;
        }

        public override string ToString()
        {
            string ans = string.Empty;

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return base.ToString();
        }
    }
}
