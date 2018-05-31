using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    internal class CreadorComandos
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CreadorComandos()
        {

        }

        public IComando Crear(ISolicitud s)
        {
            IComando c = null;
            switch (s.TipoSolicitud)
            {
                case Solicitud.RegistrarDispositivos:
                    c = new CmdRegistrarDispositivos(s);
                    break;
                case Solicitud.ComprobarEstadoDispositivos:
                    c = new CmdComprobarEstadoDispositivos(s);
                    break;
                case Solicitud.AgregarItem:
                    c = new CmdAgregarArticulo(s);
                    break;
                case Solicitud.InicioSesion:
                    c = new CmdPanelInicioSesion(s);
                    break;
                case Solicitud.IniciarSesion:
                    c = new CmdIniciarSesion(s);
                    break;
                case Solicitud.Vender:
                    c = new CmdPanelVenta(s);
                    break;
                case Solicitud.Pagar:
                    c = new CmdPanelPago(s);
                    break;
                case Solicitud.CambiarVista:
                    c = new CmdCambiarVista(s);
                    break;
                case Solicitud.PagoEfectivo:
                case Solicitud.PagarVentaDatafono:
                    c = new CmdPagarVenta(s);
                    break;
                case Solicitud.VerTirilla:
                    c = new CmdVerTirilla(s);
                    break;
                case Solicitud.VerTirillaDevolucion:
                    c = new CmdVerTirilla(s);
                    break;
                case Solicitud.TerminalAsegurada:
                    c = new CmdTerminalAsegurada(s);
                    break;
                case Solicitud.Prestamos:
                    c = new CmdPrestamos(s);
                    break;
                case Solicitud.Volver:
                    c = new CmdVolver(s);
                    break;
                case Solicitud.CancelarItem:
                    c = new CmdCancelarArticulo(s);
                    break;
                case Solicitud.AgregarPrestamo:
                    c = new CmdAgregarPrestamo(s);
                    break;
                case Solicitud.AgregarCliente:
                    c = new CmdAgregarCliente(s);
                    break;
                case Solicitud.AgregarClienteVentaEspecialSinMedioPago:
                    c = new CmdAgregarClienteVentaEspecialSinMedioPago(s);
                    break;
                case Solicitud.Recogida:
                    c = new CmdPanelRecogida(s);
                    break;
                case Solicitud.RegistrarRecogida:
                    c = new CmdAgregarRecogida(s);
                    break;
                case Solicitud.ConsultarPrecio:
                    c = new CmdConsultarPrecio(s);
                    break;
                case Solicitud.EstadoConsultarPrecio:
                    c = new CmdEstadoConsultarPrecio(s);
                    break;
                case Solicitud.EstadoDevolucion:
                    c = new CmdEstadoDevolucion(s);
                    break;
                case Solicitud.AgregarItemDevolucion:
                    c = new CmdAgregarArticuloDevolucion(s);
                    break;
                case Solicitud.TerminarDevolucion:
                    c = new CmdTerminarDevolucion(s);
                    break;
                case Solicitud.PagoDatafono:
                case Solicitud.ReintentarPago:
                case Solicitud.PagoDatafonoManual:
                    c = new CmdPagarVentaDatafono(s);
                    break;
                case Solicitud.CancelarTransaccion:
                    c = new CmdCancelarTransaccion(s);
                    break;
                case Solicitud.CancelarDevolucion:
                    c = new CmdCancelarDevolucion(s);
                    break;
                case Solicitud.CancelarVenta:
                    c = new CmdAnularVenta(s);
                    break;
                case Solicitud.CancelarConsultaPrecio:
                    c = new CmdCancelarConsultaPrecio(s);
                    break;
                case Solicitud.PantallaCliente:
                    c = new CmdPantallaCliente(s);
                    break;
                case Solicitud.LimpiarVisor:
                    c = new CmdLimpiarVisor(s);
                    break;
                case Solicitud.ImprimirUltima:
                    c = new CmdImprimirUltima(s);
                    break;
                case Solicitud.VentaEspecial:
                    c = new CmdPanelVentaEspecial(s);
                    break;
                case Solicitud.RegistrarVentaEspecialSinMedioPago:
                    c = new CmdAgregarVentaEspecialSinMedioPago(s);
                    break;
                case Solicitud.AgregarItemVentaEspecialSinMedioPago:
                    c = new CmdAgregarArticuloVentaEspecialSinMedioPago(s);
                    break;
                case Solicitud.TerminarVentaEspecialSinMedioPago:
                    c = new CmdTerminarVentaEspecialSinMedioPago(s);
                    break;
                case Solicitud.CancelarVentaEspecialSinMedioPago:
                    c = new CmdCancelarVentaEspecialSinMedioPago(s);
                    break;
                case Solicitud.Ajustes:
                    c = new CmdPanelAjuste(s);
                    break;
                case Solicitud.AgregarAjuste:
                    c = new CmdAgregarAjuste(s);
                    break;
                case Solicitud.TerminarAjuste:
                    c = new CmdTerminarAjuste(s);
                    break;
                case Solicitud.CierreDatafono:
                    c = new CmdCierreDatafono(s);
                    break;
                case Solicitud.PanelCierreDatafono:
                    c = new CmdPanelCierreDatafono(s);
                    break;
                case Solicitud.ListarClientes:
                    c = new CmdPanelCliente(s);
                    break;
                case Solicitud.AgregarArticuloAjuste:
                case Solicitud.AgregarItemAjuste:
                    c = new CmdAgregarArticuloAjuste(s);
                    break;
                case Solicitud.CancelarTransaccionAjuste:
                    c = new CmdCancelarTransaccionAjuste(s);
                    break;
                case Solicitud.Arqueo:
                    c = new CmdEstadoArqueo(s);
                    break;
                case Solicitud.AgregarValorArqueo:
                    c = new CmdAgregarValorArqueo(s);
                    break;
                case Solicitud.GuardarArqueo:
                    c = new CmdGuardarArqueo(s);
                    break;
                case Solicitud.CancelarPago:
                    c = new CmdCancelarPago(s);
                    break;
                case Solicitud.CancelarTransaccionRecogida:
                    c = new CmdCancelarTransaccionRecogida(s);
                    break;
                //case Solicitud.CancelarOperacion:
                //    c = new CmdCancelarOperacion(s);
                //    break;
                case Solicitud.CancelarCancelarItem:
                    c = new CmdCancelar_CancelarItem(s);
                    break;
                case Solicitud.CancelarVentaEspecial:
                    c = new CmdCancelarVentaEspecialSinMedioPago(s);
                    break;
                case Solicitud.SolicitarIntervencionRecogida:
                case Solicitud.SolicitarIntervencionPrestamo:
                    c = new CmdPanelIntervencion(s);
                    break;
                case Solicitud.ValidarIntervencionRecogida:
                case Solicitud.ValidarIntervencionPrestamo:
                    c = new CmdValidarIntervencion(s);
                    break;
                case Solicitud.TerminarRecogida:
                    c = new CmdTerminarRecogida(s);
                    break;
                case Solicitud.TerminarPrestamo:
                    c = new CmdTerminarPrestamo(s);
                    break;
                default:
                    throw new EvaApplicationException("Solicitud no reconocida.");
            }
            return c;
        }
    }
}
