using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PArticulo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public EArticulo BuscarArticuloPorCodigo(string codigo, bool soloCodigoBarra, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            EArticulo articulo = null;
            respuesta = new Respuesta(false);

            //Parametro de venta por PLU o codigo de barra llamando a la instancia de parametros en el entorno.
            var rarticulo = new RArticulo();
            var registro = rarticulo.BuscarArticuloPorCodigo(codigo, soloCodigoBarra);

            //
            if (registro == null)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "Artículo no encontrado.";
            }
            else
            {
                articulo = ArticuloUtil.InstanciarDesde(registro);
                if (implementaImpuestoCompuesto)
                {
                    var impuestos = rarticulo.BuscarImpuestosArticulo(articulo.Id);
                    List<EImpuestosArticulo> listaImpuestos = ArticuloUtil.InstanciarImpuestos(impuestos);
                    articulo.ListaImpuestos(listaImpuestos);
                }
                respuesta.Valida = true;
            }

            return articulo;
        }
    }

    public class ArticuloUtil
    {
        public static EArticulo InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ArgumentNullException("Datos de artículo nulos o vacíos.");
            }

            //
            var a = new EArticulo(
                   (string)registro["id_articulo"],
                   (string)registro["cod_imp"],
                   (string)registro["descrip1"],
                   (string)registro["descrip2"],
                   (decimal)registro["pre_venta1"],
                   (byte)registro["peso_req"] == 1, //todo: para que sirve esta comparación ? ¯\_(。_°)_/¯
                   (byte)registro["descodificado"] == 1,
                   (double)registro["impto1"]);
            return a;
        }


        //todo: esta método debería estar en la entidad o persistencia correspondiente a impuesto. (SRP)
        public static List<EImpuestosArticulo> InstanciarImpuestos(DataTable registros)
        {
            List<EImpuestosArticulo> imp = new List<EImpuestosArticulo>();

            foreach (DataRow registro in registros.Rows)
            {
                var a = new EImpuestosArticulo(
                   (int)registro["id_impuesto"],
                   (string)registro["nombre"],
                   (string)registro["identificador"],
                   (double)registro["porcentaje"],
                   (decimal)registro["valor"],
                   (Int16)registro["tipo_impuesto"]
                   );

                imp.Add(a);
            }
            return imp;
        }
    }
}
