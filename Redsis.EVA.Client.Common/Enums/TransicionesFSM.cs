﻿using System;
using System.Collections.Generic;
using System.Text;

namespace EvaPOS.Enums
{
    public enum TransicionesFSM
    {
        IniciarSesion,
        InicioSesion,
        Vender,
        AgregarItem,
        Pagar,
        PagoEfectivo,
        VerTirilla,
        Volver,
        TerminarDevolucion,
        RegistrarDispositivos,
        ComprobarEstadoDispositivos,
        TerminalAsegurada,
        Prestamos,
        Recogida,
        RegistrarRecogida,
        CancelarItem,
        EstadoDevolucion,
        AgregarPrestamo,
        PagoDatafono,
        ConsultarPrecio,
        EstadoConsultarPrecio,
        AgregarItemDevolucion,
        PagarVentaDatafono,
        VerTirillaDevolucion,
        ReintentarPago,
        CancelarItemDevolucion,
        CancelarTransaccion,
        CancelarDevolucion,
        PantallaCliente,
        LimpiarVisor,
        ImprimirUltima,
        VentaEspecial,
        TerminarVentaEspecial,
        RegistrarVentaEspecial,
        AgregarItemVentaEspecial,
        RegistrarDegustacion,
        RegistrarVentaEspecialSinMedioPago,
        TerminarVentaEspecialSinMedioPago,
        AgregarItemVentaEspecialSinMedioPago,
        CancelarItemVentaEspecialSinMedioPago,
        CancelarVentaEspecialSinMedioPago,
        AgregarCliente,
        PagoDatafonoManual,
        CierreDatafono,
        PanelCierreDatafono,
        Ajustes,
        TerminarAjuste,
        AgregarAjuste,
        CancelarItemAjuste,
        CancelarTransaccionAjuste,
        AgregarItemAjuste,
        Arqueo,
        AgregarClienteVentaEspecialSinMedioPago,
        AgregarValorArqueo,
        GuardarArqueo,
        CancelarVentaEspecial,
        CancelarPago,
        CancelarOperacion,
        NoCancelar,
        CancelarTransaccionRecogida,
        SolicitarIntervencionRecogida,
        ValidarIntervencionRecogida,
        TerminarRecogida,
        SolicitarIntervencionPrestamo,
        ValidarIntervencionPrestamo,
        TerminarPrestamo
    }
}
