using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EImpuestos
    {
        public List<EImpuesto> Impuestos { get; private set; } = new List<EImpuesto>();
        public void Poblar(EParametros parametros)
        {
            for (int i = 1; i < 7; i++)
            {
                try {
                    Impuestos.Add(new EImpuesto(parametros.ObtenerValorParametro<string>("pdv.identificador_impuesto_" + i),
                    parametros.ObtenerValorParametro<string>("pdv.descripcion_impuesto_" + i),
                    parametros.ObtenerValorParametro<float>("pdv.porcentaje_impuesto_" + i),
                    i
                    ));
                }
                catch
                {
                }
            }
        }
    }
}
