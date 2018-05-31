using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RArqueo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public DataTable ObtenerArqueo(string codTerminal, string codUsuario)
        {
            DataTable dt = null;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("with t as \n");
            queryStringBuilder.Append("            (select tm.id_medio, case when tm.id_medio = 1 then tm.total - rv.total_recog_efect else tm.total end total from registro_venta rv \n");
            queryStringBuilder.Append("            inner join totales_medio_pago tm on tm.id_registro_venta = rv.id_registro_venta \n");
            queryStringBuilder.Append("            where rv.date_created between convert(date, @date_created) and DATEADD(day,1, convert(date, @date_created)) \n");
            queryStringBuilder.Append("            and rv.cod_terminal = @cod_terminal\n");
            queryStringBuilder.Append("            and rv.cod_usuario = @cod_usuario\n");
            queryStringBuilder.Append("            ) \n");
            queryStringBuilder.Append("            select mp.id_medio_pago, case when t.total is null then 0 else t.total end total from medio_pago mp \n");
            queryStringBuilder.Append("            left join t \n");
            queryStringBuilder.Append("            on t.id_medio = mp.id_medio_pago");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.

                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_usuario", codUsuario);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RArqueo.ObtenerArqueo] la consulta no produjo resultados");
                }
            }

            return dt;
        }

        public int CrearArquero(string idVenta, string codTerminal, string tipo, int diaTransac, long nroTransac, string prefijo, string usuario)
        {
            int records = rVenta.CrearVenta(idVenta, 0, 0, codTerminal, tipo, diaTransac, 0, nroTransac, prefijo, usuario, 0);

            if (records <= 0)
            {
                log.Info("[RPrestamo.CrearArquero] la consulta no produjo resultados");
            }

            return records;
        }

        public int ActualizarRegistroArqueo(string idRegistroVenta)
        {
            int records = rVenta.ActualizarRegistroVenta(idRegistroVenta, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

            if (records <= 0)
            {
                log.Info("[RPrestamo.ActualizarRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearRegistroVenta(string idRegistroVenta, string codTerminal, string usuario)
        {
            int records = rVenta.CrearRegistroVenta(idRegistroVenta, codTerminal, usuario);

            if (records <= 0)
            {
                log.Info("[RArqueo.CrearRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int GuardarArqueo(string codMedioPago, int nroTransac, decimal valorConteo, decimal valorDiferencia, decimal valorEnCaja, string idVenta)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[arqueo_caja] \n");
            queryStringBuilder.Append("           ([id_arqueo_caja] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_medio_pago] \n");
            queryStringBuilder.Append("           ,[codigo_motivo_diferencia_id] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[des_motivo_diferencia] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[id_medio_pago] \n");
            queryStringBuilder.Append("           ,[nro_transac] \n");
            queryStringBuilder.Append("           ,[valor_conteo] \n");
            queryStringBuilder.Append("           ,[valor_diferencia] \n");
            queryStringBuilder.Append("           ,[valor_en_caja] \n");
            queryStringBuilder.Append("           ,[id_venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@idArqueoCaja \n");
            queryStringBuilder.Append("           ,0 \n");
            queryStringBuilder.Append("           ,'00' \n");
            queryStringBuilder.Append("           ,@codMedioPago \n");
            queryStringBuilder.Append("           ,@codigoMotivoDiferenciaId \n");
            queryStringBuilder.Append("           ,@fecha \n");
            queryStringBuilder.Append("           ,@desMotivoDiferencia \n");
            queryStringBuilder.Append("           ,@fecha \n");
            queryStringBuilder.Append("           ,@codMedioPago \n");
            queryStringBuilder.Append("           ,@nroTransac \n");
            queryStringBuilder.Append("           ,@valorConteo \n");
            queryStringBuilder.Append("           ,@valorDiferencia \n");
            queryStringBuilder.Append("           ,@valorEnCaja \n");
            queryStringBuilder.Append("           ,@idVenta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@idArqueoCaja", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@codMedioPago", codMedioPago);
                oCmd.Parameters.AddWithValue("@codigoMotivoDiferenciaId", DBNull.Value);
                oCmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@desMotivoDiferencia", DBNull.Value);
                oCmd.Parameters.AddWithValue("@nroTransac", nroTransac);
                oCmd.Parameters.AddWithValue("@valorConteo", valorConteo);
                oCmd.Parameters.AddWithValue("@valorDiferencia", valorDiferencia);
                oCmd.Parameters.AddWithValue("@valorEnCaja", valorEnCaja);
                oCmd.Parameters.AddWithValue("@idVenta", idVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearMedioPago] la consulta no produjo resultados");
                }
            }

            return records;
        }
    }
}
