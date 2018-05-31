using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelModalPagoManual : IPanelPagoManual
    {
        void PagarVentaManual();
        void MostrarModalPagoManual();
    }
}
