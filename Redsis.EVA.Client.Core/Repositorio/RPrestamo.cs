using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RPrestamo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public int CrearPrestamo(string idVenta, decimal valor, string codTerminal, string tipo, int diaTransac, long nroTransac, string prefijo, string usuario)
        {
            int records = rVenta.CrearVenta(idVenta, 0, valor, codTerminal, tipo, diaTransac, 0, nroTransac, prefijo, usuario, 0);

            if (records <= 0)
            {
                log.Info("[RPrestamo.CrearPrestamo] la consulta no produjo resultados");
            }

            return records;

            /*
            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[venta] \n");
            queryStringBuilder.Append("           ([id_venta] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[bruto_ngtv] \n");
            queryStringBuilder.Append("           ,[bruto_pstv] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_terminal] \n");
            queryStringBuilder.Append("           ,[cod_tipo_transac] \n");
            queryStringBuilder.Append("           ,[cod_vendedor] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[dia_transac] \n");
            queryStringBuilder.Append("           ,[domicilio] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[nro_fact] \n");
            queryStringBuilder.Append("           ,[nro_transac] \n");
            queryStringBuilder.Append("           ,[prefijo] \n");
            queryStringBuilder.Append("           ,[id_terminal] \n");
            queryStringBuilder.Append("           ,[tipo] \n");
            queryStringBuilder.Append("           ,[id_tipo_transac] \n");
            queryStringBuilder.Append("           ,[id_usuario] \n");
            queryStringBuilder.Append("           ,[valor_cambio] \n");
            queryStringBuilder.Append("           ,[id_vendedor]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_venta \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@bruto_ngtv \n");
            queryStringBuilder.Append("           ,@bruto_pstv \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@cod_terminal \n");
            queryStringBuilder.Append("           ,@cod_tipo_trans \n");
            queryStringBuilder.Append("           ,@cod_vendedor \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@dia_transac \n");
            queryStringBuilder.Append("           ,@domicilio \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@nro_fact \n");
            queryStringBuilder.Append("           ,@nro_transac \n");
            queryStringBuilder.Append("           ,@prefijo \n");
            queryStringBuilder.Append("           ,@id_terminal \n");
            queryStringBuilder.Append("           ,@tipo \n");
            queryStringBuilder.Append("           ,@id_tipo_transac \n");
            queryStringBuilder.Append("           ,@id_usuario \n");
            queryStringBuilder.Append("           ,@valor_cambio \n");
            queryStringBuilder.Append("           ,@id_vendedor)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.

                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;

                oCmd.Parameters.AddWithValue("@id_venta", idVenta);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@bruto_ngtv", 0);
                oCmd.Parameters.AddWithValue("@bruto_pstv", brutoPstv);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_tipo_trans", tipo);
                oCmd.Parameters.AddWithValue("@cod_vendedor", DBNull.Value);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@dia_transac", diaTransac);
                oCmd.Parameters.AddWithValue("@domicilio", 0);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nro_fact", 0);
                oCmd.Parameters.AddWithValue("@nro_transac", nroTransac);
                oCmd.Parameters.AddWithValue("@prefijo", prefijo);
                oCmd.Parameters.AddWithValue("@id_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@tipo", 0);
                oCmd.Parameters.AddWithValue("@id_tipo_transac", tipo);
                oCmd.Parameters.AddWithValue("@id_usuario", usuario);
                oCmd.Parameters.AddWithValue("@valor_cambio", 0);
                oCmd.Parameters.AddWithValue("@id_vendedor", DBNull.Value);


                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RPrestamo.CrearPrestamo] la consulta no produjo resultados");
                }
            }

            return records;
            */
        }

        public int ActualizarRegistroVenta(string idRegistroVenta, decimal valor)
        {
            int records = rVenta.ActualizarRegistroVenta(idRegistroVenta, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, valor, 0, 0);

            if (records <= 0)
            {
                log.Info("[RPrestamo.ActualizarRegistroVenta] la consulta no produjo resultados");
            }

            return records;

            /*
            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("UPDATE[dbo].[registro_venta] \n");
            queryStringBuilder.Append("   SET[version] = version + 1, \n");
            queryStringBuilder.Append("      [last_updated] = @last_updated, \n");
            queryStringBuilder.Append("      [nro_prestamos] = nro_prestamos + 1, \n");
            queryStringBuilder.Append("      [total_prestamos] = total_prestamos + @total_prestamos, \n");
            queryStringBuilder.Append(" WHERE [id_registro_venta] = @id_registro_venta");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_registro_venta", idRegistroVenta);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@total_prestamos", total_prestamos);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.ActualizarRegistroVenta] la consulta no produjo resultados");
                }
            }

            return records;
            */
        }

        public int CrearMedioPago(string codMedioPago, string codTerminal, long nroTransac, decimal valor, string idVenta)
        {

            int records = rVenta.CrearMedioPago(codMedioPago, codTerminal, 0, nroTransac, valor, idVenta, 0, "00", "00", 0);

            if (records <= 0)
            {
                log.Info("[RPrestamo.CrearMedioPago] la consulta no produjo resultados");
            }

            return records;
            /*
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_medios_pago] \n");
            queryStringBuilder.Append("           ([id_ventas_medios_pago] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[anulado] \n");
            queryStringBuilder.Append("           ,[cod_banco] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_medio_pago] \n");
            queryStringBuilder.Append("           ,[cod_terminal] \n");
            queryStringBuilder.Append("           ,[consecutivo] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[documento] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[id_medio_pago] \n");
            queryStringBuilder.Append("           ,[meses_plazo] \n");
            queryStringBuilder.Append("           ,[nro_cuenta] \n");
            queryStringBuilder.Append("           ,[nro_transac] \n");
            queryStringBuilder.Append("           ,[valor] \n");
            queryStringBuilder.Append("           ,[id_venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_ventas_medios_pago \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@anulado \n");
            queryStringBuilder.Append("           ,@cod_banco \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@cod_medio_pago \n");
            queryStringBuilder.Append("           ,@cod_terminal \n");
            queryStringBuilder.Append("           ,@consecutivo \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@documento \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@id_medio_pago \n");
            queryStringBuilder.Append("           ,@meses_plazo \n");
            queryStringBuilder.Append("           ,@nro_cuenta \n");
            queryStringBuilder.Append("           ,@nro_transac \n");
            queryStringBuilder.Append("           ,@valor \n");
            queryStringBuilder.Append("           ,@id_venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ventas_medios_pago", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@anulado", Convert.ToByte(0));
                oCmd.Parameters.AddWithValue("@cod_banco", "00");
                oCmd.Parameters.AddWithValue("@cod_empresa", "00");
                oCmd.Parameters.AddWithValue("@cod_medio_pago", codMedioPago);
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@consecutivo", consecutivo);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@documento", 0);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@id_medio_pago", codMedioPago);
                oCmd.Parameters.AddWithValue("@meses_plazo", 0);
                oCmd.Parameters.AddWithValue("@nro_cuenta", "00");
                oCmd.Parameters.AddWithValue("@nro_transac", nroTransac);
                oCmd.Parameters.AddWithValue("@valor", valor);
                oCmd.Parameters.AddWithValue("@id_venta", idVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RPrestamo.CrearMedioPago] la consulta no produjo resultados");
                }
            }

            return records;
            */
        }

        public int CrearRegistroVenta(string idRegistroVenta, string codTerminal, string usuario)
        {
            int records = rVenta.CrearRegistroVenta(idRegistroVenta, codTerminal, usuario);

            if (records <= 0)
            {
                log.Info("[RPrestamo.CrearRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearTotalesMedioPago(string idtotalesMedioPago, string idREgistroVenta, string idMedioPago)
        {
            int records = rVenta.CrearTotalesMedioPago(idtotalesMedioPago, idREgistroVenta, idMedioPago);

            if (records <= 0)
            {
                log.Info("[RPrestamo.CrearTotalesMedioPago] la consulta no produjo resultados");
            }

            return records;
        }

        public int ActualizarTotalesMedioPago(string id, decimal total)
        {
            int records = rVenta.ActualizarTotalesMedioPago(id, total);

            if (records <= 0)
            {
                log.Info("[RPrestamo.ActualizarTotalesMedioPago] la consulta no produjo resultados");
            }

            return records;
        }
    }
}
