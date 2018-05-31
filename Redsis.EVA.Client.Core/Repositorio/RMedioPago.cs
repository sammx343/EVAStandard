using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RMedioPago
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataTable GetAllMediosPago()
        {
            DataTable dt = null;

            //Consulta de listado de Medios de pago.
            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("Select a.* from dbo.medio_pago a", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RMedioPago.GetAllMediosPago] la consulta no produjo resultados");
                }
            }

            return dt;
        }
    }
}
