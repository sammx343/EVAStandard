using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Solicitudes;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdCancelar_CancelarItem : ComandoAbstract
    {
        public CmdCancelar_CancelarItem(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            log.InfoFormat("CmdCancelar_CancelarItem --> Cancelar cancelar item");

            //
            SolicitudVolver volver = new SolicitudVolver(Enums.Solicitud.Vender);
            System.Threading.Thread.Sleep(100);
            Reactor.Instancia.Procesar(volver);
        }
    }
}
