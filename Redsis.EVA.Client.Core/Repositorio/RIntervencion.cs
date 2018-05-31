using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    internal class RIntervencion
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public int CrearTransAbrirCajon(string idVenta, decimal valor, string codTerminal, string tipo, int diaTransac, long nroTransac, string prefijo, string usuario)
        {
            int records = rVenta.CrearVenta(idVenta, valor, 0, codTerminal, tipo, diaTransac, 0, nroTransac, prefijo, usuario, 0);

            if (records <= 0)
            {
                log.Info("[RCajon.CrearTransAbrirCajon] la consulta no produjo resultados");
            }

            return records;
        }

        internal int CrearRegistroIntervencion(string idRegistroIntervencion, string idVenta, string claveSupervisor, string motivoIntervencion, string codTerminal, long nroTransac, string usuario)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.AppendLine(" INSERT INTO [dbo].[ventas_intervencion]");
            queryStringBuilder.AppendLine("([id_ventas_supervisor]");
            queryStringBuilder.AppendLine(",[version]");
            queryStringBuilder.AppendLine(",[clave_sup]");
            queryStringBuilder.AppendLine(",[cod_empresa]");
            queryStringBuilder.AppendLine(",[cod_terminal]");
            queryStringBuilder.AppendLine(",[cod_usuario]");
            queryStringBuilder.AppendLine(",[consecutivo_art] ");
            queryStringBuilder.AppendLine(",[consecutivo_pago]");
            queryStringBuilder.AppendLine(",[date_created]");
            queryStringBuilder.AppendLine(",[last_updated]");
            queryStringBuilder.AppendLine(",[motivo]");
            queryStringBuilder.AppendLine(",[nro_transac]");
            queryStringBuilder.AppendLine(",[id_venta])");
            queryStringBuilder.AppendLine(" VALUES");
            queryStringBuilder.AppendLine("(@id_ventas_supervisor");
            queryStringBuilder.AppendLine(",@version");
            queryStringBuilder.AppendLine(",@clave_sup");
            queryStringBuilder.AppendLine(",@cod_empresa");
            queryStringBuilder.AppendLine(",@cod_terminal");
            queryStringBuilder.AppendLine(",@cod_usuario");
            queryStringBuilder.AppendLine(",@consecutivo_art ");
            queryStringBuilder.AppendLine(",@consecutivo_pago");
            queryStringBuilder.AppendLine(",@date_created");
            queryStringBuilder.AppendLine(",@last_updated");
            queryStringBuilder.AppendLine(",@motivo");
            queryStringBuilder.AppendLine(",@nro_transac");
            queryStringBuilder.AppendLine(",@id_venta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = System.Data.CommandType.Text;

                oCmd.Parameters.AddWithValue("@id_ventas_supervisor", idRegistroIntervencion);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@clave_sup", claveSupervisor);
                oCmd.Parameters.AddWithValue("@cod_empresa", "0");
                oCmd.Parameters.AddWithValue("@cod_terminal", codTerminal);
                oCmd.Parameters.AddWithValue("@cod_usuario", usuario);
                oCmd.Parameters.AddWithValue("@consecutivo_art", "0");
                oCmd.Parameters.AddWithValue("@consecutivo_pago", "0");
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@motivo", motivoIntervencion);
                oCmd.Parameters.AddWithValue("@nro_transac", nroTransac);
                oCmd.Parameters.AddWithValue("@id_venta", idVenta);

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
            int records = rVenta.CrearRegistroVenta(idRegistroVenta, codTerminal, usuario);

            if (records <= 0)
            {
                log.Info("[RCajon.CrearRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int ActualizarTerminal(string codTerminal, long factura, long transaccion)
        {
            int records = rVenta.ActualizarTerminal(codTerminal, factura, transaccion);

            if (records <= 0)
            {
                log.Info("[RCajon.ActualizarTerminal] la consulta no produjo resultados");
            }

            return records;
        }
    }
}
