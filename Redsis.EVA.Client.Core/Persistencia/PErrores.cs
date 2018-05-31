using EvaPOS;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PErrores
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public EErrores ObetenerErrores()
        {
            EErrores result = new EErrores();
            var errores = new RError();
            DataTable dt = errores.ObetenerErrores();
            foreach (DataRow dr in dt.Rows)
            {
                result.ListaErrores.Add(ErrorUtil.InstanciarDesde(dr));
            }
            return result;
        }

        public Dictionary<int, EError> ObtenerListaErrores()
        {
            var repositorio = new RError();
            DataTable registros = repositorio.ObetenerErrores();
            if (registros.IsNullOrEmptyTable())
            {
                return null;
            }
            else
            {
                Dictionary<int, EError> errores = new Dictionary<int, EError>();
                foreach (DataRow registro in registros.Rows)
                {
                    try
                    {
                        errores.Add((int)registro["codigo_error"], ErrorUtil.InstanciarDesde(registro));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                return errores;
            }

        }
    }
    #region CreaInstanciaError
    public class ErrorUtil
    {
        public static EError InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ApplicationException("Registro nulo o contiene campos nulos.");
            }
            var a = new EError(
                (int)registro["codigo_error"],
                (string)registro["mensaje"],
                (string)registro["descripcion"],
                (string)registro["explicacion"]
            );
            return a;
        }
    }
    #endregion
}
