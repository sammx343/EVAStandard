using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RVentaEspecial
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataTable GetAllVentaEspecial()
        {
            DataTable dt = null;

            //Consulta de listado de Medios de pago.
            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("SELECT * FROM tipo_venta tp LEFT JOIN tipo_venta_cliente tvc ON tvc.id_tipo_venta = tp.id_tipo_venta where tp.activo = 1", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RVentaEspecial.GetAllVentaEspecial] la consulta no produjo resultados");
                }
            }

            return dt;
        }

        public int AsociarTipoVentaConEncabezado(string idTipoVenta, string idVenta)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_tipo] \n");
            queryStringBuilder.Append("            ([id_ventas_tipo], \n");
            queryStringBuilder.Append("             [version], \n");
            queryStringBuilder.Append("             [cod_empresa], \n");
            queryStringBuilder.Append("             [date_created], \n");
            queryStringBuilder.Append("             [last_updated], \n");
            queryStringBuilder.Append("             [id_tipo_venta], \n");
            queryStringBuilder.Append("             [id_venta]) \n");
            queryStringBuilder.Append("VALUES      ( @id_ventas_tipo, \n");
            queryStringBuilder.Append("              @version, \n");
            queryStringBuilder.Append("              @cod_empresa, \n");
            queryStringBuilder.Append("              @date_created, \n");
            queryStringBuilder.Append("              @last_updated, \n");
            queryStringBuilder.Append("              @id_tipo_venta, \n");
            queryStringBuilder.Append("              @id_venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ventas_tipo", Guid.NewGuid().ToString());
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@id_tipo_venta", idTipoVenta);
                oCmd.Parameters.AddWithValue("@id_venta", idVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVentaEspecial.AsociarTipoVentaConEncabezado] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int ActualizarRegistroVentaEspecial(decimal brutoNgtvVentaEspecial, decimal brutoPstvVentaEspecial, int nroArtVentaEspecial, int nroCancelacionVentaEspecial, int nroTransacVentaEspecial, decimal totalCancelacionVentaEspecial, decimal totalVentaEspecial, int nroDcto, decimal totalDcto, string idRegistroVenta)
        {
            int records = 0;

            StringBuilder QueryStringBuilder = new StringBuilder();
            QueryStringBuilder.Append("UPDATE [dbo].[registro_venta] \n");
            QueryStringBuilder.Append("SET    [bruto_ngtv_venta_especial] = @brutoNgtvVentaEspecial, \n");
            QueryStringBuilder.Append("       [bruto_pstv_venta_especial] = [bruto_pstv_venta_especial] + @brutoPstvVentaEspecial, \n");
            QueryStringBuilder.Append("       [nro_art_venta_especial] = [nro_art_venta_especial] + @nroArtVentaEspecial, \n");
            QueryStringBuilder.Append("       [version] = version + 1, \n");
            QueryStringBuilder.Append("       [nro_cancelacion_venta_especial] = [nro_cancelacion_venta_especial] + @nroCancelacionVentaEspecial, \n");
            QueryStringBuilder.Append("       [nro_transac_venta_especial] = [nro_transac_venta_especial] + @nroTransacVentaEspecial, \n");
            QueryStringBuilder.Append("       [total_cancelacion_venta_especial] = [total_cancelacion_venta_especial] + @totalCancelacionVentaEspecial, \n");
            QueryStringBuilder.Append("       [total_venta_especial] = [total_venta_especial] + @totalVentaEspecial, \n");
            QueryStringBuilder.Append("       [nro_dcto] = [nro_dcto] + @nroDcto, \n");
            QueryStringBuilder.Append("       [total_dcto] = [total_dcto] + @totalDcto, \n");
            QueryStringBuilder.Append("       [last_updated] = @lastUpdated \n");
            QueryStringBuilder.Append("WHERE  [id_registro_venta] = @idRegistroVenta");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(QueryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@brutoNgtvVentaEspecial", brutoNgtvVentaEspecial);
                oCmd.Parameters.AddWithValue("@brutoPstvVentaEspecial", brutoPstvVentaEspecial);
                oCmd.Parameters.AddWithValue("@nroArtVentaEspecial", nroArtVentaEspecial);
                oCmd.Parameters.AddWithValue("@nroCancelacionVentaEspecial", nroCancelacionVentaEspecial);
                oCmd.Parameters.AddWithValue("@nroTransacVentaEspecial", nroTransacVentaEspecial);
                oCmd.Parameters.AddWithValue("@totalCancelacionVentaEspecial", totalCancelacionVentaEspecial);
                oCmd.Parameters.AddWithValue("@totalVentaEspecial", totalVentaEspecial);
                oCmd.Parameters.AddWithValue("@nroDcto", nroDcto);
                oCmd.Parameters.AddWithValue("@totalDcto", totalDcto);
                oCmd.Parameters.AddWithValue("@lastUpdated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@idRegistroVenta", idRegistroVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVentaEspecial.ActualizarRegistroVenta] la consulta no produjo resultados");
                }
            }

            return records;
        }
    }
}
