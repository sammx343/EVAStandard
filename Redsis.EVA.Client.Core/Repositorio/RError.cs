using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RError
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataTable ObetenerErrores()
        {
            DataTable dt = null;

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                SqlCommand oCmd = new SqlCommand("Select * from dbo.tipo_error where codigo_error between 699 and 1000", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RError.ObtenerErrores] la consulta no produjo resultados");
                }
            }

            return dt;

        }
    }
}
