using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.DTOs
{
    public class DItemMedioPago : IMedioPagoUI
    {
        public string CodigoMedioPago
        {
            get;

            set;
        }

        public string NombreMedioPago
        {
            get;

            set;
        }

        public decimal ValorMedioPago
        {
            get;

            set;
        }
    }
}
