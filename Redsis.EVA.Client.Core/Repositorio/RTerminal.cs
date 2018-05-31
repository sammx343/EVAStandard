using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RTerminal
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DataRow BuscarTerminalPorCodigo(string codigo)
        {
            DataTable dt = null;
            DataRow dr = null;

            //Valida parametros
            if (string.IsNullOrEmpty(codigo))
            {
                //throw new ArgumentNullException("codigo");
                throw new ArgumentNullException(Entorno.Instancia.getMensajeError((int)Enums.Errores.informacion_faltante));
            }

            //Consulta de Terminal por codigo.
            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                SqlCommand oCmd = new SqlCommand("select factura_final,primera_factura, cod_terminal, prefijo, facturas_aviso, nro_autorizacion, ultima_factura, nro_ultima_transaccion, fecha_autorizacion, rango_alarma_nro_fac, l.descrip, l.cod_localidad, c.descrip ciudad, d.dir1 from terminal t left join localidad l on l.cod_localidad = t.localidad_id left join direccion d on d.id_direccion = l.id_direccion left join ciudad c on c.id_ciudad = l.id_ciudad where t.cod_terminal =  @cod ", oConn);
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
                    log.Info("[Rterminal.BuscarTerminalPorCodigo] la consulta no produjo resultados para codigo " + codigo);
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
