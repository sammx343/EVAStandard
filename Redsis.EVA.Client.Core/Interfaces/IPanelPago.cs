using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelPago : IPanelActivo
    {
        string VisorOperador { get; set; }

        string VisorMensaje { get; set; }

        string VisorEntrada { get; set; }

        IVisorCliente VisorCliente { get; }

        IEstadoVenta VisorEstadoPago { get; }

        void LimpiarVentaFinalizada();

        void PagoVentaDatafono();

        void AgregarMedioPagoUI(IMedioPagoUI item);

        void AgregarImpuestosUI(ObservableCollection<IImpuestosUI> listImpuestos);
    }
}
