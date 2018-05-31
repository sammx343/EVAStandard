using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PTerminal
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ETerminal BuscarTerminalPorCodigo(string cod, out Respuesta respuesta)
        {
            var repositorio = new RTerminal();
            ETerminal terminal = null;
            respuesta = new Respuesta(false);
            var registro = repositorio.BuscarTerminalPorCodigo(cod);
            if (registro != null)
            {
                terminal = TerminalUtil.InstanciarDesde(registro);
                respuesta.Valida = true;
            }
            else
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "Terminal no encontrada.";
                log.Info("[ETerminal]: Terminal no encontrada");
                //throw new Exception("Terminal no encontrada");
            }
            return terminal;
        }
    }

    public class TerminalUtil
    {
        public static ETerminal InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ApplicationException("Registro nulo o contiene campos nulos.");
            }
            var a = new ETerminal(
                (string)registro["cod_terminal"],
                (string)registro["prefijo"],
                (long)registro["facturas_aviso"],
                (string)registro["nro_autorizacion"],
                (long)registro["ultima_factura"],
                (long)registro["nro_ultima_transaccion"],
                (DateTime)registro["fecha_autorizacion"],
                (int)registro["rango_alarma_nro_fac"],
                (long)registro["primera_factura"],
                (long)registro["factura_final"],
                new ELocalidad((string)registro["cod_localidad"], (string)registro["descrip"], (string)registro["ciudad"], (string)registro["dir1"]));
            return a;
        }
    }
}
