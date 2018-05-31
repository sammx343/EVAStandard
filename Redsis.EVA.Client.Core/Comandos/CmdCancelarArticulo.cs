using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdCancelarArticulo : ComandoAbstract
    {
        private SolicitudCancelarArticulo solicitud;
        public string CodigoArticulo { get; private set; }
        public int CantidadArticulo { get; private set; }


        public CmdCancelarArticulo(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as SolicitudCancelarArticulo;
        }

        public override void Ejecutar()
        {
            //
            log.Debug("[CmdCancelarArticulo.Ejecutar] Cancelar Item");            
        }
        
    }
}
