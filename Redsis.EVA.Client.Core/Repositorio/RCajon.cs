using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RCajon
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public int CrearTransAbrirCajon(string idVenta, decimal valor, string codTerminal, string tipo, int diaTransac, long nroTransac, string prefijo, string usuario)
        {
            int records = rVenta.CrearVenta(idVenta, valor, 0, codTerminal, tipo, diaTransac, 0, nroTransac, prefijo, usuario, 0);

            if (records <= 0)
            {
                log.Info("[RCajon.CrearTransAbrirCajon] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearRegistroVenta(string idRegistroVenta, string codTerminal, string usuario)
        {
            int records = rVenta.CrearRegistroVenta(idRegistroVenta, codTerminal, usuario);

            if (records <= 0)
            {
                log.Info("[RCajon.CrearRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int ActualizarTerminal(string codTerminal, long factura, long transaccion)
        {
            int records = rVenta.ActualizarTerminal(codTerminal, factura, transaccion);

            if (records <= 0)
            {
                log.Info("[RCajon.ActualizarTerminal] la consulta no produjo resultados");
            }

            return records;
        }
    }
}
