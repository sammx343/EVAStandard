using Redsis.EVA.Client.Common;
using EvaPOS;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PParametros
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //TODO comprobar que los valores configurados para los parametros sean del tipo correspondiente al enum definido para los tipos de datos establecidos.
        public EParametros ObetenerParametros(out Respuesta respuesta)
        {
            respuesta = new Respuesta(false);
            EParametros result = null;


            var parametros = new RParametro();
            DataTable dt = parametros.ObtenerParametros();
            if (!dt.IsNullOrEmptyTable())
            {
                result = new EParametros();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        result.Agregar(ParametroUtil.InstanciarDesde(dr));
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message);
                        continue;
                    }
                }

                respuesta.Valida = true;
            }
            else
            {
                log.Info("[EParametros]: Parametros no encontrados");

                respuesta.Valida = false;
                respuesta.Mensaje = "Parametros no encontrados";
            }

            return result;
        }

        public bool VerificarExixteTablaImpuestos()
        {
            var parametros = new RParametro();
            DataRow dr = parametros.ValidarExisteTablaImpuestos();

            var res = (int)dr["Respuesta"];
            if (res == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #region CreaInstanciaEParametro
    public class ParametroUtil
    {
        public static EParametro InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ApplicationException("Datos de parametro nulos o vacíos");
            }

            var resultado = new EParametro(
                (string)registro["nombre"],
                (string)registro["tipo"],
                (string)registro["valor"],
                (string)registro["ambito"],
                (string)registro["id_ambito"]
                );

            return resultado;
        }
    }
    #endregion
}
