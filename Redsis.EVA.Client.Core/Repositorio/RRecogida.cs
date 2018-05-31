using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RRecogida
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public DataTable GetAllCodigoDeRecogidas()
        {
            DataTable dt = null;

            //Consulta de listado de CodigoDeRegocidas

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("SELECT * FROM codificacion WHERE padre = 100", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RRecogida.GetAllCodigoDeRecogidas] la consulta no produjo resultados");
                }
            }

            return dt;
        }

        public int CrearRecogida(string idVenta, decimal valor, string codTerminal, string tipo, int diaTransac, long nroTransac, string prefijo, string usuario)
        {
            int records = rVenta.CrearVenta(idVenta, valor, 0, codTerminal, tipo, diaTransac, 0, nroTransac, prefijo, usuario, 0);

            if (records <= 0)
            {
                log.Info("[RRecogida.CrearRecogida] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearRegistroVenta(string idRegistroVenta, string codTerminal, string usuario)
        {
            int records = rVenta.CrearRegistroVenta(idRegistroVenta, codTerminal, usuario);

            if (records <= 0)
            {
                log.Info("[RRecogida.CrearRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int ActualizarRegistroVenta(string idRegistroVenta, decimal valor)
        {
            int records = rVenta.ActualizarRegistroVenta(idRegistroVenta, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, valor, 0);

            if (records <= 0)
            {
                log.Info("[RRecogida.ActualizarRegistroVenta] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearMedioPago(string codMedioPago, string codTerminal, long nroTransac, decimal valor, string idVenta)
        {
            int records = rVenta.CrearMedioPago(codMedioPago, codTerminal, 0, nroTransac, valor, idVenta, 0, "00", "00", 0);

            if (records <= 0)
            {
                log.Info("[RRecogida.CrearMedioPago] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearTotalesMedioPago(string idtotalesMedioPago, string idREgistroVenta, string idMedioPago)
        {
            int records = rVenta.CrearTotalesMedioPago(idtotalesMedioPago, idREgistroVenta, idMedioPago);

            if (records <= 0)
            {
                log.Info("[RRecogida.CrearTotalesMedioPago] la consulta no produjo resultados");
            }

            return records;
        }

        public int ActualizarTotalesMedioPago(string id, decimal total)
        {
            int records = rVenta.ActualizarTotalesMedioPago(id, total);

            if (records <= 0)
            {
                log.Info("[RRecogida.ActualizarTotalesMedioPago] la consulta no produjo resultados");
            }

            return records;
        }

        public int CrearVentaRecogida(string idCodificacion, string descrip, string idVenta)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ventas_recogida] \n");
            queryStringBuilder.Append("           ([id_ventas_recogida] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[id_codificacion] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[descrip] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[id_venta]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@generado \n");
            queryStringBuilder.Append("           ,0 \n");
            queryStringBuilder.Append("           ,@codEmpresa \n");
            queryStringBuilder.Append("           ,@idCodificacion \n");
            queryStringBuilder.Append("           ,@fecha \n");
            queryStringBuilder.Append("           ,@descrip \n");
            queryStringBuilder.Append("           ,@fecha \n");
            queryStringBuilder.Append("           ,@idVenta)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@generado", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@codEmpresa", "00");
                oCmd.Parameters.AddWithValue("@idCodificacion", idCodificacion);
                oCmd.Parameters.AddWithValue("@fecha", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@descrip", descrip);
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

        public int ActualizarTerminal(string codTerminal, long factura, long transaccion)
        {
            int records = rVenta.ActualizarTerminal(codTerminal, factura, transaccion);

            if (records <= 0)
            {
                log.Info("[RRecogida.ActualizarTerminal] la consulta no produjo resultados");
            }

            return records;
        }
    }
}
