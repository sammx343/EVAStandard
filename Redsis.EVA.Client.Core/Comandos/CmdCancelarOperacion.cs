using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdCancelarOperacion : ComandoAbstract
    {
        Solicitudes.SolicitudCancelarOperacion Solicitud;

        public CmdCancelarOperacion(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudCancelarOperacion;
        }

        public override void Ejecutar()
        {
            log.Info("--> Cancelar operación");
        }
    }
}
