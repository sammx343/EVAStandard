using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EPrestamo
    {
        public decimal Valor { get; private set; }
        public List<decimal> ListPrestamos { get; set; }
        public bool EstaAbierta { get; set; }
        public EPrestamo(decimal valor = 0)
        {
            Valor = valor;
        }
        public void SumarAlPrestamo(decimal valor)
        {
            Valor += valor;
        }
    }

}
