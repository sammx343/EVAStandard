using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RVenta
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int CrearVenta(string idVenta, decimal brutoNgtv, decimal brutoPstv, string codTerminal, string tipo, int diaTransac, long nroFact, long nroTransac, string prefijo, string usuario, decimal cambio)
        {
            //string idVenta = Guid.NewGuid().ToString();
            int records = 0;

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
                oCmd.Parameters.AddWithValue("@bruto_ngtv", brutoNgtv);
                oCmd.Parameters.AddWithValue("@bruto_pstv", brutoPstv);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_tipo_trans", tipo);
                oCmd.Parameters.AddWithValue("@cod_vendedor", DBNull.Value);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@dia_transac", diaTransac);
                oCmd.Parameters.AddWithValue("@domicilio", 0);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nro_fact", nroFact);
                oCmd.Parameters.AddWithValue("@nro_transac", nroTransac);
                oCmd.Parameters.AddWithValue("@prefijo", prefijo);
                oCmd.Parameters.AddWithValue("@id_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@tipo", 0);
                oCmd.Parameters.AddWithValue("@id_tipo_transac", tipo);
                oCmd.Parameters.AddWithValue("@id_usuario", usuario);
                oCmd.Parameters.AddWithValue("@valor_cambio", cambio);
                oCmd.Parameters.AddWithValue("@id_vendedor", DBNull.Value);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearVenta] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearRegistroVenta(string idRegistroVenta, string codTerminal, string usuario)
        {
            //string idVenta = Guid.NewGuid().ToString();
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[registro_venta] \n");
            queryStringBuilder.Append("           ([id_registro_venta] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[bruto_ngtv] \n");
            queryStringBuilder.Append("           ,[bruto_pstv] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_terminal] \n");
            queryStringBuilder.Append("           ,[cod_usuario] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[estado_terminal] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[nro_abono1] \n");
            queryStringBuilder.Append("           ,[nro_abono2] \n");
            queryStringBuilder.Append("           ,[nro_abono3] \n");
            queryStringBuilder.Append("           ,[nro_abono4] \n");
            queryStringBuilder.Append("           ,[nro_abono5] \n");
            queryStringBuilder.Append("           ,[nro_abono6] \n");
            queryStringBuilder.Append("           ,[nro_arqueos] \n");
            queryStringBuilder.Append("           ,[nro_art] \n");
            queryStringBuilder.Append("           ,[nro_cancelacion] \n");
            queryStringBuilder.Append("           ,[nro_dcto] \n");
            queryStringBuilder.Append("           ,[nro_prestamos] \n");
            queryStringBuilder.Append("           ,[nro_recog_cheque] \n");
            queryStringBuilder.Append("           ,[nro_recog_efect] \n");
            queryStringBuilder.Append("           ,[nro_transac_venta] \n");
            queryStringBuilder.Append("           ,[num_otros_ingresos] \n");
            queryStringBuilder.Append("           ,[num_transac_anuladas] \n");
            queryStringBuilder.Append("           ,[id_terminal] \n");
            queryStringBuilder.Append("           ,[total_abono1] \n");
            queryStringBuilder.Append("           ,[total_abono2] \n");
            queryStringBuilder.Append("           ,[total_abono3] \n");
            queryStringBuilder.Append("           ,[total_abono4] \n");
            queryStringBuilder.Append("           ,[total_abono5] \n");
            queryStringBuilder.Append("           ,[total_abono6] \n");
            queryStringBuilder.Append("           ,[total_cancelacion] \n");
            queryStringBuilder.Append("           ,[total_dcto] \n");
            queryStringBuilder.Append("           ,[total_otros_ingresos] \n");
            queryStringBuilder.Append("           ,[total_prestamos] \n");
            queryStringBuilder.Append("           ,[total_recog_cheque] \n");
            queryStringBuilder.Append("           ,[total_recog_efect] \n");
            queryStringBuilder.Append("           ,[total_transac_anuladas] \n");
            queryStringBuilder.Append("           ,[total_venta] \n");
            queryStringBuilder.Append("           ,[nro_devolucion] \n");
            queryStringBuilder.Append("           ,[total_devolucion] \n");

            queryStringBuilder.Append("           ,[bruto_ngtv_venta_especial] \n");
            queryStringBuilder.Append("           ,[bruto_pstv_venta_especial] \n");
            queryStringBuilder.Append("           ,[nro_art_venta_especial] \n");
            queryStringBuilder.Append("           ,[nro_cancelacion_venta_especial] \n");
            queryStringBuilder.Append("           ,[nro_transac_venta_especial] \n");
            queryStringBuilder.Append("           ,[total_cancelacion_venta_especial] \n");
            queryStringBuilder.Append("           ,[total_venta_especial] \n");

            queryStringBuilder.Append("           ,[id_usuario]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_registro_venta \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@bruto_ngtv \n");
            queryStringBuilder.Append("           ,@bruto_pstv \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@cod_terminal \n");
            queryStringBuilder.Append("           ,@cod_usuario \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@estado_terminal \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@nro_abono1 \n");
            queryStringBuilder.Append("           ,@nro_abono2 \n");
            queryStringBuilder.Append("           ,@nro_abono3 \n");
            queryStringBuilder.Append("           ,@nro_abono4 \n");
            queryStringBuilder.Append("           ,@nro_abono5 \n");
            queryStringBuilder.Append("           ,@nro_abono6 \n");
            queryStringBuilder.Append("           ,@nro_arqueos \n");
            queryStringBuilder.Append("           ,@nro_art \n");
            queryStringBuilder.Append("           ,@nro_cancelacion \n");
            queryStringBuilder.Append("           ,@nro_dcto \n");
            queryStringBuilder.Append("           ,@nro_prestamos \n");
            queryStringBuilder.Append("           ,@nro_recog_cheque \n");
            queryStringBuilder.Append("           ,@nro_recog_efect \n");
            queryStringBuilder.Append("           ,@nro_transac_venta \n");
            queryStringBuilder.Append("           ,@num_otros_ingresos \n");
            queryStringBuilder.Append("           ,@num_transac_anuladas \n");
            queryStringBuilder.Append("           ,@id_terminal \n");
            queryStringBuilder.Append("           ,@total_abono1 \n");
            queryStringBuilder.Append("           ,@total_abono2 \n");
            queryStringBuilder.Append("           ,@total_abono3 \n");
            queryStringBuilder.Append("           ,@total_abono4 \n");
            queryStringBuilder.Append("           ,@total_abono5 \n");
            queryStringBuilder.Append("           ,@total_abono6 \n");
            queryStringBuilder.Append("           ,@total_cancelacion \n");
            queryStringBuilder.Append("           ,@total_dcto \n");
            queryStringBuilder.Append("           ,@total_otros_ingresos \n");
            queryStringBuilder.Append("           ,@total_prestamos \n");
            queryStringBuilder.Append("           ,@total_recog_cheque \n");
            queryStringBuilder.Append("           ,@total_recog_efect \n");
            queryStringBuilder.Append("           ,@total_transac_anuladas \n");
            queryStringBuilder.Append("           ,@total_venta \n");
            queryStringBuilder.Append("           ,@nroDevolucion \n");
            queryStringBuilder.Append("           ,@totalDevolucion \n");

            queryStringBuilder.Append("           ,@bruto_ngtv_venta_especial \n");
            queryStringBuilder.Append("           ,@bruto_pstv_venta_especial \n");
            queryStringBuilder.Append("           ,@nro_art_venta_especial \n");
            queryStringBuilder.Append("           ,@nro_cancelacion_venta_especial \n");
            queryStringBuilder.Append("           ,@nro_transac_venta_especial \n");
            queryStringBuilder.Append("           ,@total_cancelacion_venta_especial \n");
            queryStringBuilder.Append("           ,@total_venta_especial \n");

            queryStringBuilder.Append("           ,@id_usuario)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.

                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_registro_venta", idRegistroVenta);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@bruto_ngtv", 0);
                oCmd.Parameters.AddWithValue("@bruto_pstv", 0);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_usuario", usuario);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@estado_terminal", "");
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nro_abono1", 0);
                oCmd.Parameters.AddWithValue("@nro_abono2", 0);
                oCmd.Parameters.AddWithValue("@nro_abono3", 0);
                oCmd.Parameters.AddWithValue("@nro_abono4", 0);
                oCmd.Parameters.AddWithValue("@nro_abono5", 0);
                oCmd.Parameters.AddWithValue("@nro_abono6", 0);
                oCmd.Parameters.AddWithValue("@nro_arqueos", 0);
                oCmd.Parameters.AddWithValue("@nro_art", 0);
                oCmd.Parameters.AddWithValue("@nro_cancelacion", 0);
                oCmd.Parameters.AddWithValue("@nro_dcto", 0);
                oCmd.Parameters.AddWithValue("@nro_prestamos", 0);
                oCmd.Parameters.AddWithValue("@nro_recog_cheque", 0);
                oCmd.Parameters.AddWithValue("@nro_recog_efect", 0);
                oCmd.Parameters.AddWithValue("@nro_transac_venta", 0);
                oCmd.Parameters.AddWithValue("@num_otros_ingresos", 0);
                oCmd.Parameters.AddWithValue("@num_transac_anuladas", 0);
                oCmd.Parameters.AddWithValue("@id_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@total_abono1", 0);
                oCmd.Parameters.AddWithValue("@total_abono2", 0);
                oCmd.Parameters.AddWithValue("@total_abono3", 0);
                oCmd.Parameters.AddWithValue("@total_abono4", 0);
                oCmd.Parameters.AddWithValue("@total_abono5", 0);
                oCmd.Parameters.AddWithValue("@total_abono6", 0);
                oCmd.Parameters.AddWithValue("@total_cancelacion", 0);
                oCmd.Parameters.AddWithValue("@total_dcto", 0);
                oCmd.Parameters.AddWithValue("@total_otros_ingresos", 0);
                oCmd.Parameters.AddWithValue("@total_prestamos", 0);
                oCmd.Parameters.AddWithValue("@total_recog_cheque", 0);
                oCmd.Parameters.AddWithValue("@total_recog_efect", 0);
                oCmd.Parameters.AddWithValue("@total_transac_anuladas", 0);
                oCmd.Parameters.AddWithValue("@total_venta", 0);
                oCmd.Parameters.AddWithValue("@id_usuario", usuario);
                oCmd.Parameters.AddWithValue("@nroDevolucion", 0);
                oCmd.Parameters.AddWithValue("@totalDevolucion", 0);

                oCmd.Parameters.AddWithValue("@bruto_ngtv_venta_especial", 0);
                oCmd.Parameters.AddWithValue("@bruto_pstv_venta_especial", 0);
                oCmd.Parameters.AddWithValue("@nro_art_venta_especial", 0);
                oCmd.Parameters.AddWithValue("@nro_cancelacion_venta_especial", 0);
                oCmd.Parameters.AddWithValue("@nro_transac_venta_especial", 0);
                oCmd.Parameters.AddWithValue("@total_cancelacion_venta_especial", 0);
                oCmd.Parameters.AddWithValue("@total_venta_especial", 0);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearRegistroVenta]: la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int ActualizarRegistroVenta(string idRegistroVenta, decimal brutoPositivo, decimal brutoNegativo, int nroArticulos, int nroCancelaciones, decimal total, int nroDevolucion, decimal totalDevolucion, int nroArqueos, int nroDcto, decimal cancelacion, int nroPrestamos, int nroRecogEfect, int nroTransacVenta, int numOtrosIngresos, int numTransacAnuladas, decimal totalDcto, decimal totalOtrosIngresos, decimal totalPrestamos, decimal totalRecogEfect, decimal totalTransacAnuladas)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("UPDATE [dbo].[registro_venta] \n");
            queryStringBuilder.Append("SET [version] = [version] + 1 \n");
            queryStringBuilder.Append(",[bruto_ngtv] = [bruto_ngtv] + @brutoNegativo \n");
            queryStringBuilder.Append(",[bruto_pstv] = [bruto_pstv] + @brutoPositivo \n");
            queryStringBuilder.Append(",[last_updated] = @fecha \n");
            queryStringBuilder.Append(",[nro_arqueos] = [nro_arqueos] + @nroArqueos \n");
            queryStringBuilder.Append(",[nro_art] = nro_art + @nroArticulos \n");
            queryStringBuilder.Append(",[nro_cancelacion] = [nro_cancelacion] + @nroCancelaciones \n");
            queryStringBuilder.Append(",[nro_dcto] = [nro_dcto] + @nroDcto \n");
            queryStringBuilder.Append(",[nro_prestamos] = [nro_prestamos] + @nroPrestamos \n");
            queryStringBuilder.Append(",[nro_recog_efect] = [nro_recog_efect] + @nroRecogEfect \n");
            queryStringBuilder.Append(",[nro_transac_venta] = [nro_transac_venta] + @nroTransacVenta \n");
            queryStringBuilder.Append(",[num_otros_ingresos] = [num_otros_ingresos] + @numOtrosIngresos \n");
            queryStringBuilder.Append(",[num_transac_anuladas] = [num_transac_anuladas] + @numTransacAnuladas \n");
            queryStringBuilder.Append(",[total_cancelacion] = [total_cancelacion] + @cancelacion \n");
            queryStringBuilder.Append(",[total_dcto] = [total_dcto] + @totalDcto \n");
            queryStringBuilder.Append(",[total_otros_ingresos] = [total_otros_ingresos] + @totalOtrosIngresos \n");
            queryStringBuilder.Append(",[total_prestamos] = [total_prestamos] + @totalPrestamos \n");
            queryStringBuilder.Append(",[total_recog_efect] = [total_recog_efect] + @totalRecogEfect \n");
            queryStringBuilder.Append(",[total_transac_anuladas] = [total_transac_anuladas] + @totalTransacAnuladas \n");
            queryStringBuilder.Append(",[total_venta] = [total_venta] + @total \n");
            queryStringBuilder.Append(",[nro_devolucion] = [nro_devolucion] + @nroDevolucion \n");
            queryStringBuilder.Append(",[total_devolucion] = [total_devolucion] + @totalDevolucion \n");
            queryStringBuilder.Append("WHERE [id_registro_venta] = @idRegistroVenta");

            //StringBuilder queryStringBuilder = new StringBuilder();
            //queryStringBuilder.Append("update rv \n");
            //queryStringBuilder.Append("set rv.version = rv.version + 1, \n");
            //queryStringBuilder.Append("bruto_pstv = bruto_pstv + @brutoPositivo, \n");
            //queryStringBuilder.Append("bruto_ngtv = bruto_ngtv + @brutoNegativo, \n");
            //queryStringBuilder.Append("last_updated = @fecha, \n");
            //queryStringBuilder.Append("nro_art = nro_art + @nroArticulos, \n");
            //queryStringBuilder.Append("nro_cancelacion = nro_cancelacion + @nroCancelaciones, \n");
            //queryStringBuilder.Append("nro_devolucion = nro_devolucion + @nroDevolucion, \n");
            //queryStringBuilder.Append("nro_transac_venta = nro_transac_venta + 1, \n");
            //queryStringBuilder.Append("total_cancelacion = total_cancelacion + @cancelacion, \n");
            //queryStringBuilder.Append("total_venta = total_venta + @total, \n");
            //queryStringBuilder.Append("total_devolucion = total_devolucion + @totalDevolucion \n");
            //queryStringBuilder.Append("from registro_venta rv \n");
            //queryStringBuilder.Append("where rv.id_registro_venta = @idRegistroVenta");


            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@brutoPositivo", brutoPositivo);
                oCmd.Parameters.AddWithValue("@brutoNegativo", brutoNegativo);
                oCmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nroArticulos", nroArticulos);
                oCmd.Parameters.AddWithValue("@nroDevolucion", nroDevolucion);
                oCmd.Parameters.AddWithValue("@nroCancelaciones", nroCancelaciones);

                oCmd.Parameters.AddWithValue("@total", total);
                oCmd.Parameters.AddWithValue("@totalDevolucion", totalDevolucion);
                oCmd.Parameters.AddWithValue("@idRegistroVenta", idRegistroVenta);

                oCmd.Parameters.AddWithValue("@nroArqueos", nroArqueos);
                oCmd.Parameters.AddWithValue("@nroDcto", nroDcto);
                oCmd.Parameters.AddWithValue("@cancelacion", cancelacion);

                oCmd.Parameters.AddWithValue("@nroPrestamos", nroPrestamos);
                oCmd.Parameters.AddWithValue("@nroRecogEfect", nroRecogEfect);
                oCmd.Parameters.AddWithValue("@nroTransacVenta", nroTransacVenta);
                oCmd.Parameters.AddWithValue("@numOtrosIngresos", numOtrosIngresos);
                oCmd.Parameters.AddWithValue("@numTransacAnuladas", numTransacAnuladas);
                oCmd.Parameters.AddWithValue("@totalDcto", totalDcto);
                oCmd.Parameters.AddWithValue("@totalOtrosIngresos", totalOtrosIngresos);
                oCmd.Parameters.AddWithValue("@totalPrestamos", totalPrestamos);
                oCmd.Parameters.AddWithValue("@totalRecogEfect", totalRecogEfect);
                oCmd.Parameters.AddWithValue("@totalTransacAnuladas", totalTransacAnuladas);

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
        }

        public int ActualizarTotalesMedioPago(string id, decimal total)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("UPDATE tmp \n");
            queryStringBuilder.Append("set tmp.version = tmp.version + 1, \n");
            queryStringBuilder.Append("tmp.last_updated = @fecha, \n");
            queryStringBuilder.Append("tmp.numero = tmp.numero + 1, \n");
            queryStringBuilder.Append("tmp.total = tmp.total + @total \n");
            queryStringBuilder.Append("from totales_medio_pago tmp \n");
            queryStringBuilder.Append("where tmp.id_tot_med_pago = @id");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@total", total);
                oCmd.Parameters.AddWithValue("@id", id);
                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.ActualizarTotalesMedioPago] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearTotalesMedioPago(string idtotalesMedioPago, string idREgistroVenta, string idMedioPago)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[totales_medio_pago] \n");
            queryStringBuilder.Append("           ([id_tot_med_pago] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[id_medio] \n");
            queryStringBuilder.Append("           ,[numero] \n");
            queryStringBuilder.Append("           ,[id_registro_venta] \n");
            queryStringBuilder.Append("           ,[total]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_tot_med_pago \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@id_medio \n");
            queryStringBuilder.Append("           ,@numero \n");
            queryStringBuilder.Append("           ,@id_registro_venta \n");
            queryStringBuilder.Append("           ,@total)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_tot_med_pago", idtotalesMedioPago);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@id_medio", idMedioPago);
                oCmd.Parameters.AddWithValue("@numero", 0);
                oCmd.Parameters.AddWithValue("@id_registro_venta", idREgistroVenta);
                oCmd.Parameters.AddWithValue("@total", 0);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearTotalesMedioPago] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearDetalleVenta(Guid idVentasArticulo, string idArticulo, string codImp, string codTerminal, int consecutivo, decimal impuesto, long nroTransac, decimal pctjImpuesto, decimal peso, string usuario, decimal valorVenta, string idVenta, int cantidad, string codigoLeido)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_articulo] \n");
            queryStringBuilder.Append("           ([id_ventas_articulo] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[id_articulo] \n");
            queryStringBuilder.Append("           ,[cantidad] \n");
            queryStringBuilder.Append("           ,[cod_barra] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_imp] \n");
            queryStringBuilder.Append("           ,[cod_terminal] \n");
            queryStringBuilder.Append("           ,[cod_vendedor] \n");
            queryStringBuilder.Append("           ,[consecutivo] \n");
            queryStringBuilder.Append("           ,[costo] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[factor_venta] \n");
            queryStringBuilder.Append("           ,[impuesto] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[nro_transac] \n");
            queryStringBuilder.Append("           ,[pctj_dcto] \n");
            queryStringBuilder.Append("           ,[pctj_impuesto] \n");
            queryStringBuilder.Append("           ,[peso] \n");
            queryStringBuilder.Append("           ,[usuario] \n");
            queryStringBuilder.Append("           ,[valor_dcto] \n");
            queryStringBuilder.Append("           ,[valor_venta] \n");
            queryStringBuilder.Append("           ,[vendedor_id] \n");
            queryStringBuilder.Append("           ,[id_venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_ventas_articulo \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@id_articulo \n");
            queryStringBuilder.Append("           ,@cantidad \n");
            queryStringBuilder.Append("           ,@cod_barra \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@cod_imp \n");
            queryStringBuilder.Append("           ,@cod_terminal \n");
            queryStringBuilder.Append("           ,@cod_vendedor \n");
            queryStringBuilder.Append("           ,@consecutivo \n");
            queryStringBuilder.Append("           ,@costo \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@factor_venta \n");
            queryStringBuilder.Append("           ,@impuesto \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@nro_transac \n");
            queryStringBuilder.Append("           ,@pctj_dcto \n");
            queryStringBuilder.Append("           ,@pctj_impuesto \n");
            queryStringBuilder.Append("           ,@peso \n");
            queryStringBuilder.Append("           ,@usuario \n");
            queryStringBuilder.Append("           ,@valor_dcto \n");
            queryStringBuilder.Append("           ,@valor_venta \n");
            queryStringBuilder.Append("           ,@vendedor_id \n");
            queryStringBuilder.Append("           ,@id_venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ventas_articulo", idVentasArticulo);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@id_articulo", idArticulo);
                oCmd.Parameters.AddWithValue("@cantidad", cantidad);
                oCmd.Parameters.AddWithValue("@cod_barra", codigoLeido);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@cod_imp", codImp);
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_vendedor", DBNull.Value);
                oCmd.Parameters.AddWithValue("@consecutivo", consecutivo);
                oCmd.Parameters.AddWithValue("@costo", 0);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@factor_venta", 1);
                oCmd.Parameters.AddWithValue("@impuesto", impuesto);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nro_transac", nroTransac);
                oCmd.Parameters.AddWithValue("@pctj_dcto", 0);
                oCmd.Parameters.AddWithValue("@pctj_impuesto", pctjImpuesto);
                oCmd.Parameters.AddWithValue("@peso", peso);
                oCmd.Parameters.AddWithValue("@usuario", usuario);
                oCmd.Parameters.AddWithValue("@valor_dcto", 0);
                oCmd.Parameters.AddWithValue("@valor_venta", valorVenta);
                oCmd.Parameters.AddWithValue("@vendedor_id", DBNull.Value);
                oCmd.Parameters.AddWithValue("@id_venta", idVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearDetalleVenta] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearVentasArticuloImpuesto(Guid idVentasArticulo, string nombre, string identificador, double porcentaje, decimal valor, int tipo, decimal venta, int idImpuesto)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_articulo_impuesto] \n");
            queryStringBuilder.Append("           ([id_ventas_articulo_impto] \n");
            queryStringBuilder.Append("           ,[id_ventas_articulo] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[nombre] \n");
            queryStringBuilder.Append("           ,[identificador] \n");
            queryStringBuilder.Append("           ,[porcentaje] \n");
            queryStringBuilder.Append("           ,[valor] \n");
            queryStringBuilder.Append("           ,[tipo_impuesto] \n");
            queryStringBuilder.Append("           ,[id_impuesto] \n");
            queryStringBuilder.Append("           ,[venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_ventas_articulo_impto \n");
            queryStringBuilder.Append("           ,@id_ventas_articulo \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@nombre \n");
            queryStringBuilder.Append("           ,@identificador \n");
            queryStringBuilder.Append("           ,@porcentaje \n");
            queryStringBuilder.Append("           ,@valor \n");
            queryStringBuilder.Append("           ,@tipo_impuesto \n");
            queryStringBuilder.Append("           ,@id_impuesto \n");
            queryStringBuilder.Append("           ,@venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ventas_articulo_impto", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@id_ventas_articulo", idVentasArticulo.ToString());
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@cod_empresa", "00");
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nombre", nombre);
                oCmd.Parameters.AddWithValue("@identificador", identificador);
                oCmd.Parameters.AddWithValue("@porcentaje", porcentaje);
                oCmd.Parameters.AddWithValue("@valor", valor);
                oCmd.Parameters.AddWithValue("@tipo_impuesto", tipo);
                oCmd.Parameters.AddWithValue("@venta", venta);
                oCmd.Parameters.AddWithValue("@id_impuesto", idImpuesto);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearDetalleVenta] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearMedioPago(string codMedioPago, string codTerminal, int consecutivo, long nroTransac, decimal valor, string idVenta, int documento, String codBanco, String numeroCuenta, int mesesPlazo)
        {
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
                oCmd.Parameters.AddWithValue("@cod_banco", codBanco);
                oCmd.Parameters.AddWithValue("@cod_empresa", "00");
                oCmd.Parameters.AddWithValue("@cod_medio_pago", codMedioPago);
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@consecutivo", consecutivo);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@documento", documento);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@id_medio_pago", codMedioPago);
                oCmd.Parameters.AddWithValue("@meses_plazo", mesesPlazo);
                oCmd.Parameters.AddWithValue("@nro_cuenta", numeroCuenta);
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
                    log.Info("[RVenta.CrearMedioPago] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearCopiaImpresion(string codEmpresa, string codLocalidad, string codTerminal, string codTipoTransac, string codUsuario, string contenido, string localidadId, string modeloImpresora, decimal nroFactura, int nroTransac, string prefijo, string terminalId, string tipoTransacId, string usuarioId)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[copia_impresion] \n");
            queryStringBuilder.Append("            ([id_copia_imp], \n");
            queryStringBuilder.Append("             [version], \n");
            queryStringBuilder.Append("             [cod_empresa], \n");
            queryStringBuilder.Append("             [cod_localidad], \n");
            queryStringBuilder.Append("             [cod_terminal], \n");
            queryStringBuilder.Append("             [cod_tipo_transac], \n");
            queryStringBuilder.Append("             [cod_usuario], \n");
            queryStringBuilder.Append("             [contenido], \n");
            queryStringBuilder.Append("             [date_created], \n");
            queryStringBuilder.Append("             [fecha_transac], \n");
            queryStringBuilder.Append("             [last_updated], \n");
            queryStringBuilder.Append("             [localidad_id], \n");
            queryStringBuilder.Append("             [modelo_impresora], \n");
            queryStringBuilder.Append("             [nro_factura], \n");
            queryStringBuilder.Append("             [nro_transac], \n");
            queryStringBuilder.Append("             [prefijo], \n");
            queryStringBuilder.Append("             [terminal_id], \n");
            queryStringBuilder.Append("             [tipo_transac_id], \n");
            queryStringBuilder.Append("             [usuario_id]) \n");
            queryStringBuilder.Append("VALUES      ( @id_copia_imp, \n");
            queryStringBuilder.Append("              @version, \n");
            queryStringBuilder.Append("              @cod_empresa, \n");
            queryStringBuilder.Append("              @cod_localidad, \n");
            queryStringBuilder.Append("              @cod_terminal, \n");
            queryStringBuilder.Append("              @cod_tipo_transac, \n");
            queryStringBuilder.Append("              @cod_usuario, \n");
            queryStringBuilder.Append("              @contenido, \n");
            queryStringBuilder.Append("              @date_created, \n");
            queryStringBuilder.Append("              @fecha_transac, \n");
            queryStringBuilder.Append("              @last_updated, \n");
            queryStringBuilder.Append("              @localidad_id, \n");
            queryStringBuilder.Append("              @modelo_impresora, \n");
            queryStringBuilder.Append("              @nro_factura, \n");
            queryStringBuilder.Append("              @nro_transac, \n");
            queryStringBuilder.Append("              @prefijo, \n");
            queryStringBuilder.Append("              @terminal_id, \n");
            queryStringBuilder.Append("              @tipo_transac_id, \n");
            queryStringBuilder.Append("              @usuario_id)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_copia_imp", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@cod_empresa", codEmpresa);
                oCmd.Parameters.AddWithValue("@cod_localidad", codLocalidad);
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_tipo_transac", codTipoTransac);
                oCmd.Parameters.AddWithValue("@cod_usuario", codUsuario);
                oCmd.Parameters.AddWithValue("@contenido", contenido);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //oCmd.Parameters.AddWithValue("@fecha_transac", fechaTransac.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@fecha_transac", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@localidad_id", localidadId);
                oCmd.Parameters.AddWithValue("@modelo_impresora", modeloImpresora);
                oCmd.Parameters.AddWithValue("@nro_factura", nroFactura);
                oCmd.Parameters.AddWithValue("@nro_transac", nroTransac);
                oCmd.Parameters.AddWithValue("@prefijo", prefijo);
                oCmd.Parameters.AddWithValue("@terminal_id", terminalId);
                oCmd.Parameters.AddWithValue("@tipo_transac_id", tipoTransacId);
                oCmd.Parameters.AddWithValue("@usuario_id", usuarioId);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearCopiaImpresion] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int ActualizarTerminal(string codTerminal, long factura, long transaccion)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("update dbo.terminal \n");
            queryStringBuilder.Append("set ultima_factura = @factura, \n");
            queryStringBuilder.Append("nro_ultima_transaccion = @transaccion \n");
            queryStringBuilder.Append("where cod_terminal = @codTerminal");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@factura", factura);
                oCmd.Parameters.AddWithValue("@transaccion", transaccion);
                oCmd.Parameters.AddWithValue("@codTerminal", codTerminal);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.ActualizarTerminal] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int ActualizarVentasArticulo(Guid idVentasArticulo)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("update ventas_articulo set usa_tabla_impuesto=1 where id_ventas_articulo = @id");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id", idVentasArticulo.ToString());

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.ActualizarVentasArticulo] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearVentaCambio(decimal valor, string idVenta)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_cambio] \n");
            queryStringBuilder.Append("           ([id_venta_cambio] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[valor] \n");
            queryStringBuilder.Append("           ,[id_venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_venta_cambio \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@valor \n");
            queryStringBuilder.Append("           ,@id_venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_venta_cambio", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@cod_empresa", "00");
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@valor", valor);
                oCmd.Parameters.AddWithValue("@id_venta", idVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearVentaCambio] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearVentasCliente(string id, string idVenta)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_cliente] \n");
            queryStringBuilder.Append("           ([id_ventas_cliente] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[cliente_id] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[puntos] \n");
            queryStringBuilder.Append("           ,[id_venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_ventas_cliente \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@cliente_id \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@puntos \n");
            queryStringBuilder.Append("           ,@id_venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ventas_cliente", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@cliente_id", id);
                oCmd.Parameters.AddWithValue("@cod_empresa", "00");
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@puntos", 0);
                oCmd.Parameters.AddWithValue("@id_venta", idVenta);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RVenta.CrearVentasCliente] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public DataRow DineroEnCaja(string codTerminal, string codUsuario)
        {
            DataTable dt = null;
            DataRow dr = null;

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("select case when sum(tm.total- rv.total_recog_efect) is null then 0 else sum(tm.total- rv.total_recog_efect) end total from registro_venta rv inner join totales_medio_pago tm on tm.id_registro_venta = rv.id_registro_venta where rv.date_created between convert(date,@fecha) and DATEADD(day,1,convert(date,@fecha)) and rv.cod_terminal = @terminal and rv.cod_usuario = @usuario and tm.id_medio = @medio", oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@usuario", codUsuario);
                oCmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@medio", "1");

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RVenta.DineroEnCaja] la consulta no produjo resultados");
                }

                foreach (DataRow d in dt.Rows)
                {
                    dr = d;
                    break;
                }

            }
            return dr;
        }

        public DataRow ImprimirUltima(string codTerminal, string codUsuario)
        {
            DataTable dt = null;
            DataRow dr = null;

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("select top 1 contenido from copia_impresion where cod_usuario = @usuario and cod_terminal = @terminal and tipo_transac_id not in (14, 7, 10, 15, 17) order by date_created desc", oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@usuario", codUsuario);

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RVenta.ImprimirUltima] la consulta no produjo resultados");
                }

                foreach (DataRow d in dt.Rows)
                {
                    dr = d;
                    break;
                }

            }
            return dr;
        }

        public DataTable BuscarFactura(string usuario, string terminal, string prefijo, string factura, string transaccion)
        {
            DataTable dt = null;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("select contenido from dbo.copia_impresion where cod_usuario = '@usuario'  \n");
            if (!terminal.Equals(""))
            {
                queryStringBuilder.Append("and cod_terminal = '@terminal' \n");
            }
            if (!factura.Equals(""))
            {
                queryStringBuilder.Append("and nro_factura = '@factura' \n");
            }
            if (!prefijo.Equals(""))
            {
                queryStringBuilder.Append("and prefijo = '@prefijo' \n");
            }

            if (!transaccion.Equals(""))
            {
                queryStringBuilder.Append("and nro_transac = '@transaccion' \n");
            }
            queryStringBuilder.Append("order by date_created desc");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@usuario", usuario);
                if (!terminal.Equals(""))
                {
                    oCmd.Parameters.AddWithValue("@terminal", terminal);
                }
                if (!factura.Equals(""))
                {
                    oCmd.Parameters.AddWithValue("@factura", factura);
                }
                if (!prefijo.Equals(""))
                {
                    oCmd.Parameters.AddWithValue("@prefijo", prefijo);
                }
                if (!transaccion.Equals(""))
                {
                    oCmd.Parameters.AddWithValue("@transaccion", transaccion);
                }

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RVenta.BuscarFactura] la consulta no produjo resultados");
                }
            }
            return dt;
        }

        public DataTable IdRegistroVenta(string codTerminal, string codUsuario)
        {
            DataTable dt = null;

            //Valida parametros
            if (string.IsNullOrEmpty(codTerminal))
            {
                //TODO, consultar el mensaje a través del código.
                //throw new ArgumentNullException("codTerminal");
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            if (string.IsNullOrEmpty(codUsuario))
            {
                //TODO, consultar el mensaje a través del código.
                //throw new ArgumentNullException("codUsuario");
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("select rv.id_registro_venta, tmp.id_tot_med_pago, tmp.id_medio from registro_venta rv \n");
            queryStringBuilder.Append("left join totales_medio_pago tmp on tmp.id_registro_venta = rv.id_registro_venta \n");
            queryStringBuilder.Append("where rv.date_created between convert(date,@fecha) \n");
            queryStringBuilder.Append("and DATEADD(day,1,convert(date,@fecha)) \n");
            queryStringBuilder.Append("and rv.cod_terminal = @terminal \n");
            queryStringBuilder.Append("and rv.cod_usuario = @usuario");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@usuario", codUsuario);

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RVenta.IdRegistroVenta] la consulta no produjo resultados");
                }
            }

            return dt;
        }
    }
}
