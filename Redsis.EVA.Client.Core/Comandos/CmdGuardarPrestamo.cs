using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdGuardarPrestamo : ComandoAbstract
    {
        public CmdGuardarPrestamo(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            log.Info("[CmdGuardarPrestamo.Ejecutar] Guardar prestamo");
            //Pantalla
            EMedioPago medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            PPrestamo pPrestamo = new PPrestamo();
            Respuesta respuesta = new Respuesta();


            pPrestamo.GuardarPrestamo(Entorno.Instancia.Prestamo, ref idsAcumulados, TipoTransaccion.Prestamo.ToString(), Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, medioPago, "contenido", "impresora", out respuesta);
            respuesta = new Respuesta(false);
            ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
            Entorno.Instancia.Terminal = terminal;
            Entorno.Instancia.Prestamo = null;

            //throw new NotImplementedException();
        }
    }
}
