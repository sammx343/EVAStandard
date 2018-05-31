using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RUsuario
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataRow BuscarUsuarioPorIdentificacion(string usuario)
        {
            DataTable dt = null;
            DataRow dr = null;

            //Valida parametros
            if (string.IsNullOrEmpty(usuario))
            {
                //throw new ArgumentNullException("usuario");
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            //Consulta de Usuario por usuario.
            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                SqlCommand oCmd = new SqlCommand("SELECT a.* FROM dbo.usuario a WHERE a.usuario = @usu", oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@usu", usuario);

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RUsuario.BuscarUsuarioPorIdentificacion] la consulta no produjo resultados");
                }

                // TODO: hay mejor forma de sacar la primera y que debe ser unica, fila?
                foreach (DataRow d in dt.Rows)
                {
                    dr = d;
                    break;
                }
            }

            return dr;
        }

        public DataRow BuscarUsuarioPorClaveSupervisor(string idUsuario, string clave)
        {
            DataTable dt = null;
            DataRow dr = null;

            //Valida parametros
            if (string.IsNullOrEmpty(clave))
            {
                //throw new ArgumentNullException("usuario");
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            //Consulta de Usuario por usuario.
            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                SqlCommand oCmd = new SqlCommand("SELECT a.* FROM dbo.usuario a WHERE a.token_supervisor = @token and a.usuario <> @usu", oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@usu", idUsuario);
                oCmd.Parameters.AddWithValue("@token", clave);

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RUsuario.BuscarUsuarioPorIdentificacion] la consulta no produjo resultados");
                }

                // TODO: hay mejor forma de sacar la primera y que debe ser unica, fila?
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
