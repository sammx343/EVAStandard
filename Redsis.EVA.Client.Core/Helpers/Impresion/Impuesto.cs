using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Helpers.Impresion
{
    class Impuesto
    {
        public string Id 
        {
            get;
            set;
        }
        public string Porcentaje
        {
            get;
            set;
        }

        public decimal Total_Dec { get; set; }
        public string Total
        {
            get;
            set;
        }

        public decimal Base_Dec { get; set; }
        public string Base
        {
            get;
            set;
        }

        public decimal IVA_Dec { get; set; }
        public string IVA
        {
            get;
            set;
        }


        public string TotalT
        {
            get;
            set;
        }
        public string BaseT
        {
            get;
            set;
        }
        public string IVAT
        {
            get;
            set;
        }
    }
}
