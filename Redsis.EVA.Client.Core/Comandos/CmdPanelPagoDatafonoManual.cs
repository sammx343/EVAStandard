using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelPagoDatafonoManual : ComandoAbstract
    {
        public Solicitudes.SolicitudPagoDatafonoManual Solicitud { get; set; }

        public CmdPanelPagoDatafonoManual(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudPagoDatafonoManual;
        }

        public override void Ejecutar()
        {
            throw new NotImplementedException();
        }
    }
}
