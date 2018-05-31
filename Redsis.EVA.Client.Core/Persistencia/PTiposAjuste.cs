using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PTiposAjuste
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ETiposAjuste GetAllTiposAjuste()
        {
            var repositorio = new RAjuste();
            var tiposAjuste = new ETiposAjuste();
            var registros = repositorio.GetTiposAjuste();
            foreach (DataRow registro in registros.Rows)
            {
                var tipoAjuste = TipoAjusteUtil.InstanciarDesde(registro);
                tiposAjuste.TiposAjuste.Add(tipoAjuste);
            }
            return tiposAjuste;
        }
    }

    public class TipoAjusteUtil
    {
        public static ETipoAjuste InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ApplicationException("Registro nulo o contiene campos nulos.");
            }
            var a = new ETipoAjuste(
                (string)registro["id_ajuste_tipo"],
                (byte)registro["calcular_costo_venta"],
                (string)registro["cod_ajuste_tipo"],
                (string)registro["descripcion"],
                (byte)registro["editar_costo_venta"],
                (byte)registro["mostrar_costo_venta"],
                (string)registro["signo"],
                registro["activo"] != DBNull.Value ? (byte)registro["activo"] : Convert.ToByte(1)
                );
            return a;
        }
    }
}
