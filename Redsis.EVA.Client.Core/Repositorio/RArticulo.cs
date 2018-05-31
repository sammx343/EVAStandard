using Dapper;
using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RArticulo
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataRow BuscarArticuloPorCodigo(string codigo, bool soloCodigoBarra)
        {
            DataTable dt = null;
            DataRow dr = null;

            //Valida parametros
            if (string.IsNullOrEmpty(codigo))
            {
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            //Consulta de artículo por código.
            //string cadena = ConfigurationManager.ConnectionStrings["EVAConnectionString"].ConnectionString;
            string query;
            if (soloCodigoBarra)
            {
                query = "select a.* from articulo a inner join articulo_cod ac on ac.id_articulo = a.id_articulo where ac.cod_articulo = @cod ";
            }
            else
            {
                query = "select top 1 a.* from dbo.articulo a left join dbo.articulo_cod ac on ac.id_articulo = a.id_articulo where a.cod_imp = @cod or ac.cod_articulo = @cod ";
            }

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                SqlCommand oCmd = new SqlCommand(query, oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@cod", codigo);

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RArticulo.BuscarArticuloPorCodigo] la consulta no produjo resultados");
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

        public DataTable BuscarImpuestosArticulo(string id)
        {
            DataTable dt = null;
            //Valida parametros
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            string query = "SELECT cast(i.id_impuesto as int) id_impuesto, i.nombre, i.identificador, i.porcentaje, i.valor, i.tipo_impuesto FROM articulo_impuesto ai LEFT JOIN impuesto i ON(ai.id_impuesto = i.id_impuesto) WHERE ai.id_articulo = @id";

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                SqlCommand oCmd = new SqlCommand(query, oConn);
                oCmd.CommandType = CommandType.Text;
                oCmd.Parameters.AddWithValue("@id", id);

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RArticulo.BuscarArticuloPorCodigo] la consulta no produjo resultados");
                }
            }

            return dt;
        }
    }
}
