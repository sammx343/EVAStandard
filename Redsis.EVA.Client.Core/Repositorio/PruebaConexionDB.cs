using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class PruebaConexionDB
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger("","");

        public static bool Probar()
        {
            bool ok = false;
            try
            {
                using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
                {
                    oConn.Open();
                    if (oConn.State == System.Data.ConnectionState.Open)
                    {
                        log.Debug("[PruebaConexionDB] Conexión con base de datos ok.");
                        ok = true;
                    }
                }
            }
            catch (Exception ex)
            {
                string err = string.Format("[PruebaConexionDB]: [{0}]", ex.Message);
                log.Error(err, ex);
            }
            return ok;
        }
    }
}
