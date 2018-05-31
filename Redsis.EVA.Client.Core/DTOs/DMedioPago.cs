using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.DTOs
{
    public class DImpuestos : IImpuestosUI
    {
        public string Tipo { get; set; }
        public decimal Compra { get; set; }
        public decimal Base { get; set; }
        public decimal Impuestos { get; set; }
    }
}
