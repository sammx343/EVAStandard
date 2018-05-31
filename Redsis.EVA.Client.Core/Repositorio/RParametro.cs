using EvaPOS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class RParametro
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //ToDo: Misma función que reciba ambito y idAmbito.
        public DataTable ObtenerParametros()
        {
            DataTable dt = null;

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                //ToDo: Modificar consulta si recibo o no ambito y idAmbito.
                SqlCommand oCmd = new SqlCommand("Select nombre, max(tipo) tipo, max(valor) valor, max(ambito) ambito, max(id_ambito) id_ambito from dbo.parametro group by nombre", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RParametro.ObtenerParametros] la consulta no produjo resultados");
                }
            }

            return dt;
        }

        public DataRow ValidarExisteTablaImpuestos()
        {
            DataTable dt = null;
            DataRow dr = null;

            using (SqlConnection oConn = new SqlConnection(CadenaConexionDB.Instancia))
            {
                //Configuración de la consulta hacia la base de datos, se adicionan los parametros requeridos por la consulta.
                //ToDo: Modificar consulta si recibo o no ambito y idAmbito.
                SqlCommand oCmd = new SqlCommand("IF OBJECT_ID('articulo_impuesto') IS NOT NULL BEGIN select 1 as Respuesta END ELSE BEGIN select 0 as Respuesta END", oConn);
                oCmd.CommandType = CommandType.Text;

                //Apertura de la conexión
                oConn.Open();

                //Carga de información en datatable.
                dt = new DataTable();
                dt.Load(oCmd.ExecuteReader(CommandBehavior.CloseConnection));

                //Valida contenido
                if (dt.IsNullOrEmptyTable())
                {
                    log.Info("[RParametro.ValidarExisteTablaImpuestos] la consulta no produjo resultados");
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
