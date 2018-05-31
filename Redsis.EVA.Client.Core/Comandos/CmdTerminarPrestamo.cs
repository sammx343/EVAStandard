using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Enums;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdTerminarPrestamo : ComandoAbstract
    {

        public CmdTerminarPrestamo(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            EMedioPago medioPago = new PMediosPago().GetAllMediosPago().MedioPago("1");
            Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;
            PPrestamo pPrestamo = new PPrestamo();
            Respuesta respuesta = new Respuesta();
            EPrestamo ePrestamo = Entorno.Instancia.Prestamo;
            string factura = ProcesarPlantilla.Prestamos(ePrestamo);
            string modeloImpresora = Entorno.Instancia.Impresora.Marca ?? "impresora";

            var tiempoGuardarPrestamo = new MetricaTemporizador("PrestamoAgregado");
            pPrestamo.GuardarPrestamo(Entorno.Instancia.Prestamo, ref idsAcumulados, ((int)TipoTransaccion.Prestamo).ToString(), Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, medioPago, factura, modeloImpresora, out respuesta);

            //
            string idVentaPrestamo = respuesta.Mensaje;

            if (respuesta.Valida == false)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoGuardarPrestamo.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Valor", (Entorno.Instancia.Prestamo.Valor)).AgregarPropiedad("Error", respuesta.Mensaje));
                throw new Exception(respuesta.Mensaje);
            }

            Telemetria.Instancia.AgregaMetrica(tiempoGuardarPrestamo.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Valor", (Entorno.Instancia.Prestamo.Valor)));

            ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Common.Config.Terminal, out respuesta);
            if (respuesta.Valida == false)
            {
                throw new Exception(respuesta.Mensaje);
            }

            if (Entorno.Instancia.Usuario.UsuarioSupervisor != null)
            {
                PIntervencion pInterv = new PIntervencion();
                EIntervencion eInterv = new EIntervencion();

                eInterv.id_venta = idVentaPrestamo;
                eInterv.claveSupervisor = Entorno.Instancia.Usuario.UsuarioSupervisor.ClaveSupervisor;
                eInterv.motivo = "Intervención prestamo";
                eInterv.nro_transac = Convert.ToInt32(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                pInterv.GuardarIntervencion(eInterv, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, out respuesta);
            }

            Entorno.Instancia.Terminal = terminal;
            Entorno.Instancia.Prestamo = null;
            Entorno.Instancia.IdsAcumulados = idsAcumulados;
            Entorno.Instancia.Usuario.UsuarioSupervisor = null;

            iu.PanelVentas.VisorMensaje = "Prestamo registrado correctamente.";
            iu.PanelVentas.VisorEntrada = string.Empty;

            // si es panel touch
            if (Config.ViewMode == InternalSettings.ModoTouch)
                Entorno.Instancia.Vista.PanelVentas.LimpiarOperacion();


            log.Info("[CmdGuardarPrestamo] Prestamo registrado correctamente.");

            // Imprimir
            Entorno.Instancia.Impresora.Imprimir(factura, true, false);

            log.Info("[CmdAgregarPrestamo] Imprimir Operación: " + factura);
            ePrestamo = null;

            if (Config.ViewMode == InternalSettings.ModoConsola)
                iu.PanelPrestamos.VisorEntrada = string.Empty;

            Solicitudes.SolicitudPanelVenta volver = new Solicitudes.SolicitudPanelVenta(Enums.Solicitud.Vender);
            Reactor.Instancia.Procesar(volver);

            iu.MostrarPanelVenta();
        }
    }
}
