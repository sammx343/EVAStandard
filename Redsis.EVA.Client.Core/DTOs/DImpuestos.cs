using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.DTOs
{
    public class DMedioPago
    {
        public Enums.MediosPago MedioPago { get; set; }
        public string CodigoMedioPago { get; set; }
        public string NombreMedioPago { get; set; }
    }
}
