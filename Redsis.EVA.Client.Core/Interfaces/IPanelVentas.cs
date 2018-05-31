using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelVentas : IPanelActivo
    {
        string VisorOperador { get; set; }

        string VisorMensaje { get; set; }

        string VisorEntrada { get; set; }

        string VisorFechaActual { get; set; }

        IEstadoVenta VisorEstadoVenta { get; }

        IVisorCliente VisorCliente { get; }

        List<IItemTirillaIU> Tirilla { get; }

        void AgregarItemTirilla(IItemTirillaIU item);

        void LimpiarVentaFinalizada();
        void LimpiarOperacion();

        bool CancelarTransaccion();

        Task<MessageResult> CancelarOperacion(string textoMensaje);

    }
}
