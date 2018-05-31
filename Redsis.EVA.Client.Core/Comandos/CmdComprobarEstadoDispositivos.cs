using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
//using Redsis.EVA.Client.Dispositivos;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdComprobarEstadoDispositivos : ComandoAbstract
    {
        Solicitudes.SolicitudComprobarEstadoDispositivos Solicitud;

        public CmdComprobarEstadoDispositivos(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudComprobarEstadoDispositivos;
        }

        public override void Ejecutar()
        {
            Respuesta respuesta = new Respuesta();
            iu.PanelDispositivo.ComprobarEstadoDispositivos(out respuesta);
            log.Info("Comprobación de dispositivos finalizada");

            //foreach (IDetalleDispositivo dispositivo in Solicitud.ListaDispositivos)
            //{
            //    iu.PanelDispositivo.MostrarTitulo(string.Format("Comprobando estado de [{0}]", dispositivo.NombreDispositivo));

            //    //
            //    dispositivo.ComprobarEstadoDispositivo(out respuesta);
            //    if (!respuesta.Valida)
            //    {
            //        iu.PanelDispositivo.ContinuarAplicacion = respuesta.Valida;
            //        return;
            //    }
            //}
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
