using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    class RAjuste
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int CrearAjuste(string idAjuste, bool calcularCostoVenta, string codEmpresa, string codLocalidad, string estado, string idLocalidad, int nroAjuste, string signo, string tipoId, string tipoAjuste, decimal totalCosto, decimal totalVenta, string usuarioId, decimal totalImpuesto)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ajuste] \n");
            queryStringBuilder.Append("           ([id_ajuste] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[calcular_costo_venta] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_localidad] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[estado] \n");
            queryStringBuilder.Append("           ,[fecha_ajuste] \n");
            queryStringBuilder.Append("           ,[grupo] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[id_localidad] \n");
            queryStringBuilder.Append("           ,[nro_ajuste] \n");
            queryStringBuilder.Append("           ,[signo] \n");
            queryStringBuilder.Append("           ,[tipo_id] \n");
            queryStringBuilder.Append("           ,[tipo_ajuste] \n");
            queryStringBuilder.Append("           ,[total_costo] \n");
            queryStringBuilder.Append("           ,[total_venta] \n");
            queryStringBuilder.Append("           ,[usuario_id] \n");
            queryStringBuilder.Append("           ,[total_impuesto]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_ajuste \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@calcular_costo_venta \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@cod_localidad \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@estado \n");
            queryStringBuilder.Append("           ,@fecha_ajuste \n");
            queryStringBuilder.Append("           ,@grupo \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@id_localidad \n");
            queryStringBuilder.Append("           ,@nro_ajuste \n");
            queryStringBuilder.Append("           ,@signo \n");
            queryStringBuilder.Append("           ,@tipo_id \n");
            queryStringBuilder.Append("           ,@tipo_ajuste \n");
            queryStringBuilder.Append("           ,@total_costo \n");
            queryStringBuilder.Append("           ,@total_venta \n");
            queryStringBuilder.Append("           ,@usuario_id \n");
            queryStringBuilder.Append("           ,@total_impuesto)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.

                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ajuste", idAjuste);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@calcular_costo_venta", calcularCostoVenta);
                oCmd.Parameters.AddWithValue("@cod_empresa", codEmpresa);
                oCmd.Parameters.AddWithValue("@cod_localidad", codLocalidad);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@estado", estado);
                oCmd.Parameters.AddWithValue("@fecha_ajuste", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@grupo", DBNull.Value);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@id_localidad", idLocalidad);
                oCmd.Parameters.AddWithValue("@nro_ajuste", nroAjuste);
                oCmd.Parameters.AddWithValue("@signo", signo);
                oCmd.Parameters.AddWithValue("@tipo_id", tipoId);
                oCmd.Parameters.AddWithValue("@tipo_ajuste", tipoAjuste);
                oCmd.Parameters.AddWithValue("@total_costo", totalCosto);
                oCmd.Parameters.AddWithValue("@total_venta", totalVenta);
                oCmd.Parameters.AddWithValue("@usuario_id", usuarioId);
                oCmd.Parameters.AddWithValue("@total_impuesto", totalImpuesto);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RAjuste.CrearAjuste] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearAjusteDetalle(string idAjusteDetalle, string idAjuste, string idArticulo, decimal cant, string codEmpresa, string codImp, decimal costo, int nroAjuste, decimal venta, decimal impuesto, decimal pctjImpuesto)
        {
            int records = 0;

            StringBuilder queryStringBuilder = new StringBuilder();
            queryStringBuilder.Append("INSERT INTO [dbo].[ajuste_detalle] \n");
            queryStringBuilder.Append("           ([id_ajuste_detalle] \n");
            queryStringBuilder.Append("           ,[version] \n");
            queryStringBuilder.Append("           ,[id_ajuste] \n");
            queryStringBuilder.Append("           ,[id_articulo] \n");
            queryStringBuilder.Append("           ,[cant] \n");
            queryStringBuilder.Append("           ,[cod_empresa] \n");
            queryStringBuilder.Append("           ,[cod_imp] \n");
            queryStringBuilder.Append("           ,[costo] \n");
            queryStringBuilder.Append("           ,[date_created] \n");
            queryStringBuilder.Append("           ,[last_updated] \n");
            queryStringBuilder.Append("           ,[nro_ajuste] \n");
            queryStringBuilder.Append("           ,[venta] \n");
            queryStringBuilder.Append("           ,[impuesto] \n");
            queryStringBuilder.Append("           ,[pctj_impuesto]) \n");
            queryStringBuilder.Append("     VALUES \n");
            queryStringBuilder.Append("           (@id_ajuste_detalle \n");
            queryStringBuilder.Append("           ,@version \n");
            queryStringBuilder.Append("           ,@id_ajuste \n");
            queryStringBuilder.Append("           ,@id_articulo \n");
            queryStringBuilder.Append("           ,@cant \n");
            queryStringBuilder.Append("           ,@cod_empresa \n");
            queryStringBuilder.Append("           ,@cod_imp \n");
            queryStringBuilder.Append("           ,@costo \n");
            queryStringBuilder.Append("           ,@date_created \n");
            queryStringBuilder.Append("           ,@last_updated \n");
            queryStringBuilder.Append("           ,@nro_ajuste \n");
            queryStringBuilder.Append("           ,@venta \n");
            queryStringBuilder.Append("           ,@impuesto \n");
            queryStringBuilder.Append("           ,@pctj_impuesto)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.

                SqlCommand oCmd = new SqlCommand(queryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_ajuste_detalle", idAjusteDetalle);
                oCmd.Parameters.AddWithValue("@version", 0);
                oCmd.Parameters.AddWithValue("@id_ajuste", idAjuste);
                oCmd.Parameters.AddWithValue("@id_articulo", idArticulo);
                oCmd.Parameters.AddWithValue("@cant", cant);
                oCmd.Parameters.AddWithValue("@cod_empresa", codEmpresa);
                oCmd.Parameters.AddWithValue("@cod_imp", codImp);
                oCmd.Parameters.AddWithValue("@costo", costo);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@nro_ajuste", nroAjuste);
                oCmd.Parameters.AddWithValue("@venta", venta);
                oCmd.Parameters.AddWithValue("@impuesto", impuesto);
                oCmd.Parameters.AddWithValue("@pctj_impuesto", pctjImpuesto);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RAjuste.CrearAjusteDetalle] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public DataTable GetTiposAjuste()
        {
            DataTable dt = null;

            //Consulta de listado de CodigoDeRegocidas

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("SELECT [id_ajuste_tipo],[calcular_costo_venta],[cod_ajuste_tipo],[descripcion],[editar_costo_venta],[mostrar_costo_venta],[signo],[activo] FROM[dbo].[ajuste_tipo] WHERE activo is NULL or activo = 1", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RAjuste.GetAjusteTipo] la consulta no produjo resultados");
                }
            }

            return dt;
        }

        public DataRow GetNumeroAjuste()
        {
            DataTable dt = null;
            DataRow dr = null;

            //Consulta de listado de CodigoDeRegocidas

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("IF EXISTS(select top 1 nro_ajuste from ajuste) BEGIN select top 1 nro_ajuste  from ajuste order by nro_ajuste desc END ELSE BEGIN select 0 nro_ajuste END ", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RAjuste.GetAjusteTipo] la consulta no produjo resultados");
                }
                foreach (DataRow d in dt.Rows)
                {
                    dr = d;
                    break;
                }
            }

            return dr;
        }
    }
}
