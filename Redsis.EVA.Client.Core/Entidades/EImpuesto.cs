using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EImpuesto
    {
        public int Orden { get; private set; }
        public String Identificador { get; private set; }
        public String Descripcion { get; private set; }
        public float Porcentaje { get; private set; }
        public EImpuesto (String identificador, String descripcion, float porcentaje, int orden)
        {
            Orden = orden;
            Identificador = identificador;
            Descripcion = descripcion;
            Porcentaje = porcentaje;
        }
    }
}
