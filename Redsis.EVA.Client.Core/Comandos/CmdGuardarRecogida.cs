using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdGuardarRecogida : ComandoAbstract
    {
        public CmdGuardarRecogida(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            log.Info("[CmdGuardarRecogida.Ejecutar] Guardar prestamo");

            PRecogida pRecogida = new PRecogida();
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            EMedioPago medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");
            Respuesta respuesta = new Respuesta();
            

            pRecogida.GuardarRecogida(Entorno.Instancia.Recogida, ref idsAcumulados, TipoTransaccion.Recogida.ToString(), Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, medioPago, "contenido", "impresora", out respuesta);
            respuesta = new Respuesta(false);
            ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
            Entorno.Instancia.Recogida = null;

            throw new NotImplementedException();
        }
    }
}
