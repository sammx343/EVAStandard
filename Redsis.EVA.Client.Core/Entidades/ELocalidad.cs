using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ELocalidad
    {
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public string Ciudad { get; private set; }
        public string Direccion { get; private set; }
        public ELocalidad (string codigo,string nombre, string ciudad, string direccion)
        {
            Codigo = codigo;
            Nombre = nombre;
            Ciudad = ciudad;
            Direccion = direccion;
        }
    }
}
