using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ERecogida
    {
        public ECodigoRecogida CodigoRecogida { get; private set; }
        public decimal Valor { get; private set; }
        public List<decimal> listRecogidas { get; set; }
        public bool EstaAbierta { get; internal set; } = false;

        public ERecogida(ECodigoRecogida codigoRecogida, decimal valor)
        {
            CodigoRecogida = codigoRecogida;
            Valor = valor;
        }

        public ERecogida(ECodigoRecogida codigoRecogida, List<decimal> listRecogidas)
        {
            CodigoRecogida = codigoRecogida;
            listRecogidas = new List<decimal>();
        }

        public void AgregarValor(decimal valor)
        {
            Valor += valor;
        }
    }
}

