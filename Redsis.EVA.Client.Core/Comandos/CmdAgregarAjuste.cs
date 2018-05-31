using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarAjuste : ComandoAbstract
    {
        Solicitudes.SolicitudAgregarAjuste solicitud;
        public string codigoAjuste { get; set; }

        public CmdAgregarAjuste(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudAgregarAjuste;
            codigoAjuste = this.solicitud.CodigoAjuste;
        }

        public override void Ejecutar()
        {
            try
            {
                if (!Entorno.Instancia.Venta.EstaAbierta)
                {
                    Entorno.Instancia.Ajuste = new Entidades.EAjuste();
                    Entorno.Instancia.Ajuste.EstaAbierta = false;
                    Entorno.Instancia.Vista.ModalAjustes.CodigoAjuste = codigoAjuste;
                }

                log.Debug("[Ejecutar] Iniciar modo ajuste");
            }
            catch (Exception ex)
            {
                iu.PanelOperador.MensajeOperador = ex.Message;
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }
    }
}
