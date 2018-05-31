using EvaPOS;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarVentaEspecial : ComandoAbstract
    {
        Solicitudes.SolicitudAgregarVentaEspecial solicitud;
        public string codigoRecogida { get; private set; }

        public CmdAgregarVentaEspecial(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudAgregarVentaEspecial;

        }

        public override void Ejecutar()
        {

            try
            {
                if (Config.ViewMode == InternalSettings.ModoTouch)
                {
                    if (string.IsNullOrEmpty(Entorno.Instancia.Vista.ModalVentaEspecial.CodigoVenta))
                        return;
                }

                //
                //AgregarVentaEspecial();
            }
            catch (Exception ex)
            {
                iu.PanelOperador.MensajeOperador = ex.Message;
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

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
