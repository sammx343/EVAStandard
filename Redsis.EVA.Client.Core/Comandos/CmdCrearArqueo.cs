using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdCrearArqueo : ComandoAbstract
    {
        public CmdCrearArqueo(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            EMediosPago mediosPago = new PMediosPago().GetAllMediosPago();
            Respuesta respuesta = new Respuesta();
            if (Entorno.Instancia.Vista.PanelArqueo.Caja == null)
            {
                respuesta = new Respuesta(false);
                Entorno.Instancia.Vista.PanelArqueo.Caja = new PArqueo().obtenerEcaja(Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, mediosPago, out respuesta);
            }
        }
    }
}
