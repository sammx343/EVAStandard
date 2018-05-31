using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelVentaTouch : IPanelActivo
    {
        string VisorOperador { get; set; }

        string VisorMensaje { get; set; }

        string VisorEntrada { get; set; }

        IEstadoVenta VisorEstadoVenta { get; }

        IVisorCliente VisorCliente { get; }

        List<IItemTirillaIU> Tirilla { get; }

        void AgregarItemTirilla(IItemTirillaIU item);

        void LimpiarVentaFinalizada();

    }
}
