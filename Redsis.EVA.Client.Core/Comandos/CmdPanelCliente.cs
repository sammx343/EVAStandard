using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelCliente : ComandoAbstract
    {
        Solicitudes.SolicitudPanelListarClientes Solicitud;

        public CmdPanelCliente(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudPanelListarClientes;
        }

        public override void Ejecutar()
        {
            log.Info("[Ejecutar] Mostrar panel clientes");
        }
    }
}
