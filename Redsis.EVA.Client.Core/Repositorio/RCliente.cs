using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RCliente
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataTable GetAllClientes()
        {
            DataTable dt = null;

            //Consulta de listado de Medios de pago.
            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos.
                SqlCommand oCmd = new SqlCommand("select c.id_cliente,c.cod_cliente, c.tipo_cliente, p.tel_residencia, p.primer_nombre, p.segundo_nombre, p.primer_apellido, p.segundo_apellido, d.dir1, d.pais, d.dpto, d.ciudad from cliente c inner join persona p on p.id_persona = c.id_cliente left join direccion d on d.id_direccion = p.id_direccion ", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RCliente.GetAllClientes] la consulta no produjo resultados");
                }
            }

            return dt;
        }

        public int CrearDireccion(int version, string ciudad, string codEmpresa, string codPostal, string dir1, string dir2, string dir3, string dpto, string pais)
        {
            int records = 0;

            StringBuilder QueryStringBuilder = new StringBuilder();
            QueryStringBuilder.Append("INSERT INTO [dbo].[direccion] \n");
            QueryStringBuilder.Append("            ([id_direccion], \n");
            QueryStringBuilder.Append("             [version], \n");
            QueryStringBuilder.Append("             [ciudad], \n");
            QueryStringBuilder.Append("             [cod_empresa], \n");
            QueryStringBuilder.Append("             [cod_postal], \n");
            QueryStringBuilder.Append("             [date_created], \n");
            QueryStringBuilder.Append("             [dir1], \n");
            QueryStringBuilder.Append("             [dir2], \n");
            QueryStringBuilder.Append("             [dir3], \n");
            QueryStringBuilder.Append("             [dpto], \n");
            QueryStringBuilder.Append("             [last_updated], \n");
            QueryStringBuilder.Append("             [pais]) \n");
            QueryStringBuilder.Append("VALUES      ( @id_direccion, \n");
            QueryStringBuilder.Append("              @version, \n");
            QueryStringBuilder.Append("              @ciudad, \n");
            QueryStringBuilder.Append("              @cod_empresa, \n");
            QueryStringBuilder.Append("              @cod_postal, \n");
            QueryStringBuilder.Append("              @date_created, \n");
            QueryStringBuilder.Append("              @dir1, \n");
            QueryStringBuilder.Append("              @dir2, \n");
            QueryStringBuilder.Append("              @dir3, \n");
            QueryStringBuilder.Append("              @dpto, \n");
            QueryStringBuilder.Append("              @last_updated, \n");
            QueryStringBuilder.Append("              @pais)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(QueryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_direccion", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@version", version);
                oCmd.Parameters.AddWithValue("@ciudad", ciudad);
                oCmd.Parameters.AddWithValue("@cod_empresa", codEmpresa);
                oCmd.Parameters.AddWithValue("@cod_postal", codPostal);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@dir1", dir1);
                oCmd.Parameters.AddWithValue("@dir2", dir2);
                oCmd.Parameters.AddWithValue("@dir3", dir3);
                oCmd.Parameters.AddWithValue("@dpto", dpto);
                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@pais", pais);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RCliente.CrearDireccion] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearCliente(string codCliente, int nroCompras, int puntos, string tipoCliente)
        {
            int records = 0;

            StringBuilder QueryStringBuilder = new StringBuilder();
            QueryStringBuilder.Append("INSERT[dbo].[cliente] \n");
            QueryStringBuilder.Append("      ([id_cliente], \n");
            QueryStringBuilder.Append("       [cod_cliente], \n");
            QueryStringBuilder.Append("       [nro_compras], \n");
            QueryStringBuilder.Append("       [puntos], \n");
            QueryStringBuilder.Append("       [tipo_cliente]) \n");
            QueryStringBuilder.Append("VALUES( @id_cliente, \n");
            QueryStringBuilder.Append("        @cod_cliente, \n");
            QueryStringBuilder.Append("        @nro_compras, \n");
            QueryStringBuilder.Append("        @puntos, \n");
            QueryStringBuilder.Append("        @tipo_cliente)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(QueryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_cliente", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@cod_cliente", codCliente);
                oCmd.Parameters.AddWithValue("@nro_compras", nroCompras);
                oCmd.Parameters.AddWithValue("@puntos", puntos);
                oCmd.Parameters.AddWithValue("@tipo_cliente", tipoCliente);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RCliente.CrearCliente] la consulta no produjo resultados");
                }
            }

            return records;
        }

        public int CrearPersona(int version, string celPersona, string celTrabajo, string codEmpresa, string correo, string idDireccion, string primerApellido, string primerNombre, string segundoApellido, string segundoNombre, string sufijo, string telResidencia, string telTrabajo, string tipoIdentificacion, string titulo, string razonSocial)
        {
            int records = 0;

            StringBuilder QueryStringBuilder = new StringBuilder();
            QueryStringBuilder.Append("INSERT [dbo].[persona] \n");
            QueryStringBuilder.Append("       ([id_persona], \n");
            QueryStringBuilder.Append("        [version], \n");
            QueryStringBuilder.Append("        [cel_persona], \n");
            QueryStringBuilder.Append("        [cel_trabajo], \n");
            QueryStringBuilder.Append("        [cod_empresa], \n");
            QueryStringBuilder.Append("        [correo], \n");
            QueryStringBuilder.Append("        [date_created], \n");
            QueryStringBuilder.Append("        [id_direccion], \n");
            QueryStringBuilder.Append("        [last_updated], \n");
            QueryStringBuilder.Append("        [primer_apellido], \n");
            QueryStringBuilder.Append("        [primer_nombre], \n");
            QueryStringBuilder.Append("        [segundo_apellido], \n");
            QueryStringBuilder.Append("        [segundo_nombre], \n");
            QueryStringBuilder.Append("        [sufijo], \n");
            QueryStringBuilder.Append("        [tel_residencia], \n");
            QueryStringBuilder.Append("        [tel_trabajo], \n");
            QueryStringBuilder.Append("        [tipo_identificacion], \n");
            QueryStringBuilder.Append("        [titulo], \n");
            QueryStringBuilder.Append("        [razon_social]) \n");
            QueryStringBuilder.Append("VALUES ( @id_persona, \n");
            QueryStringBuilder.Append("         @version, \n");
            QueryStringBuilder.Append("         @cel_persona, \n");
            QueryStringBuilder.Append("         @cel_trabajo, \n");
            QueryStringBuilder.Append("         @cod_empresa, \n");
            QueryStringBuilder.Append("         @correo, \n");
            QueryStringBuilder.Append("         @date_created, \n");
            QueryStringBuilder.Append("         @id_direccion, \n");
            QueryStringBuilder.Append("         @last_updated, \n");
            QueryStringBuilder.Append("         @primer_apellido, \n");
            QueryStringBuilder.Append("         @primer_nombre, \n");
            QueryStringBuilder.Append("         @segundo_apellido, \n");
            QueryStringBuilder.Append("         @segundo_nombre, \n");
            QueryStringBuilder.Append("         @sufijo, \n");
            QueryStringBuilder.Append("         @tel_residencia, \n");
            QueryStringBuilder.Append("         @tel_trabajo, \n");
            QueryStringBuilder.Append("         @tipo_identificacion, \n");
            QueryStringBuilder.Append("         @titulo, \n");
            QueryStringBuilder.Append("         @razon_social)");

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                SqlCommand oCmd = new SqlCommand(QueryStringBuilder.ToString(), oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id_persona", Guid.NewGuid());
                oCmd.Parameters.AddWithValue("@version", version);
                oCmd.Parameters.AddWithValue("@cel_persona", celPersona);
                oCmd.Parameters.AddWithValue("@cel_trabajo", celTrabajo);
                oCmd.Parameters.AddWithValue("@cod_empresa", codEmpresa);
                oCmd.Parameters.AddWithValue("@correo", correo);
                oCmd.Parameters.AddWithValue("@date_created", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (idDireccion == null)
                    oCmd.Parameters.AddWithValue("@id_direccion", DBNull.Value);
                else
                    oCmd.Parameters.AddWithValue("@id_direccion", idDireccion);

                oCmd.Parameters.AddWithValue("@last_updated", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                oCmd.Parameters.AddWithValue("@primer_apellido", primerApellido);
                oCmd.Parameters.AddWithValue("@primer_nombre", primerNombre);
                oCmd.Parameters.AddWithValue("@segundo_apellido", segundoApellido);
                oCmd.Parameters.AddWithValue("@segundo_nombre", segundoNombre);
                oCmd.Parameters.AddWithValue("@sufijo", sufijo);
                oCmd.Parameters.AddWithValue("@tel_residencia", telResidencia);
                oCmd.Parameters.AddWithValue("@tel_trabajo", telTrabajo);
                oCmd.Parameters.AddWithValue("@tipo_identificacion", tipoIdentificacion);
                oCmd.Parameters.AddWithValue("@titulo", titulo);
                oCmd.Parameters.AddWithValue("@razon_social", razonSocial);

                //Apertura de la conexión
                oConn.Open();

                //Ejecuta instrucciones SQL y retorna los registros afectados.
                records = oCmd.ExecuteNonQuery();

                //Valida contenido
                if (records <= 0)
                {
                    log.Info("[RCliente.CrearPersona] la consulta no produjo resultados");
                }
            }

            return records;
        }

    }
}
