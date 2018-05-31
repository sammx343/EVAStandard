using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Enums
{
    public enum MediosPago
    {
        Efectivo = 1,
        PagoElectronicoDefault = 3,
        DebitoRedeban = 4,
        DebitoCredibanco = 5,
        CreditoCredibanco = 6,
        CreditoRedeban = 7,
        PagoManualCredibanco = 8,
        PagoManualRedeban = 2,
        DaviPay = 9
    }
}
