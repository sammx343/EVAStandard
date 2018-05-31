using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Solicitudes;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdCancelarConsultaPrecio : ComandoAbstract
    {
        SolicitudCancelarConsultaPrecio SolicitudCancelar;

        public CmdCancelarConsultaPrecio(ISolicitud solicitud) : base(solicitud)
        {
            SolicitudCancelar = solicitud as SolicitudCancelarConsultaPrecio;
        }

        public override void Ejecutar()
        {
            log.Info("CmdCancelarConsultaPrecio, cancela consulta de precio");

            //
            SolicitudPanelVenta solVolver = new SolicitudPanelVenta(Enums.Solicitud.Vender);
            System.Threading.Thread.Sleep(100);
            Reactor.Instancia.Procesar(solVolver);

        }
    }
}
