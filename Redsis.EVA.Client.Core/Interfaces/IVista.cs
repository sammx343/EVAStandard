using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IVista
    {
        IPanelOperador PanelOperador { get; set; }

        IPanelVentas PanelVentas { get; set; }

        IPanelEspera PanelEspera { get; set; }

        IPanelLogin PanelLogin { get; set; }

        IPanelPago PanelPago { get; set; }

        IEstadoActual EstadoApp { get; set; }

        IPanelActivo PanelActivo { get; set; }

        //IPanelTirilla PanelTirilla { get; set; }

        IPanelDispositivo PanelDispositivo { get; set; }

        IPanelPrestamos PanelPrestamos { get; set; }

        IPanelRecogidas PanelRecogidas { get; set; }

        IPantallaCliente PantallaCliente { get; set; }

        IPanelRecogidas ModalRecogidas { get; set; }

        IPanelVentaEspecial ModalVentaEspecial { get; set; }

        IPanelAjuste ModalAjustes { get; set; }

        IPanelPagoManual PanelPagoManual { get; set; }

        IPanelModalPagoManual ModalPagoManual { get; set; }

        IPanelCliente ModalClientes { get; set; }

        IMensajeDispositivo<MessageResult> MensajeUsuario { get; set; }

        IPanelCierreDatafono PanelCierreDatafono { get; set; }

        IPanelArqueo PanelArqueo { get; set; }

        IPanelIntervencion PanelIntervencion { get; set; }

        void MostrarPanelInicioSesion();

        void MostrarPanelVenta();

        void MostrarPanelPagos();

        void MostrarTirilla();

        void MostrarPanelDispositivos();

        void MostrarPanelTerminalAsegurada();

        void MostrarPanelPrestamos();

        void MostrarPantallaCliente();

        void MostarPanelPagoManual();

        void MostrarModalPanelPagoManual();

        void MostrarPanelCierreDatafono();

        void MostrarModalCierreDatafono();

        void MostrarPanelRecogida(string codigo);

        void MostrarModalRecogida();

        void MostrarModalAjustes();

        void MostrarModalVentaEspecial();

        void MostrarModalArqueo();

        void MostrarDisplayCliente(DisplayCliente display);

        void MostrarModalClientes();

        void LimpiarPantallaCliente();

        void MostrarPanelIntervencion(string codigo);
    }
}
