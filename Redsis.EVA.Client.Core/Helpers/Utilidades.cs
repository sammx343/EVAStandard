using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Persistencia;
using System;
using System.Collections.Generic;

namespace Redsis.EVA.Client.Core.Helpers
{
    public class Utilidades
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void GenerarTransaccionApertura()
        {
            try
            {
                bool generaTransAperturaCajon = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.imprime_transaccion_abrir_cajon");

                // ¿debe generar transacción?
                if (!generaTransAperturaCajon)
                    return;

                string motivoIntervencion = string.Empty;

                if (Entorno.Instancia.Recogida != null)
                {

                    ERecogida recogidaActual = Entorno.Instancia.Recogida;
                    if (recogidaActual != null)
                    {
                        if (recogidaActual.listRecogidas.Count > 0 && recogidaActual.Valor > 0)
                            return;
                    }

                    motivoIntervencion = "Intervención Recogida";
                }
                else if (Entorno.Instancia.Prestamo != null)
                {
                    EPrestamo prestamoActual = Entorno.Instancia.Prestamo;
                    if (prestamoActual != null)
                    {
                        if (prestamoActual.ListPrestamos.Count > 0 && prestamoActual.Valor > 0)
                            return;
                    }

                    motivoIntervencion = "Intervención Prestamo";
                }

                //
                log.Info("[GenerarTransaccionApertura] Guardando transacción apertura cajón ...");
                PCajon pCajon = new PCajon();
                Dictionary<string, string> idsAcumulados = Entorno.Instancia.IdsAcumulados;

                // Generar tirilla de apertura de cajón
                Respuesta respuesta = new Respuesta();
                string factura = ProcesarPlantilla.AperturaCajon();

                var tiempoGuardarAperturaCajon = new MetricaTemporizador("AperturaCajon");
                pCajon.GuardarAperturaCajon(ref idsAcumulados, ((int)TipoTransaccion.AbrirCajon).ToString(), Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, factura ?? "contenido", out respuesta);
                string idVentaApertCajon = respuesta.Mensaje;

                if (!respuesta.Valida)
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarAperturaCajon.Para().AgregarPropiedad("Exitoso", false).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Error", respuesta.Mensaje));

                }
                else
                {
                    Telemetria.Instancia.AgregaMetrica(tiempoGuardarAperturaCajon.Para().AgregarPropiedad("Exitoso", true).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("CajonAbierto", respuesta.Mensaje));

                    respuesta = new Respuesta(false);

                    ETerminal terminal = new PTerminal().BuscarTerminalPorCodigo(Config.Terminal, out respuesta);
                    if (respuesta.Valida == true)
                    {
                        // guardar transacción si hubo
                        if (Entorno.Instancia.Usuario.UsuarioSupervisor != null)
                        {
                            PIntervencion pInterv = new PIntervencion();
                            EIntervencion eInterv = new EIntervencion();

                            eInterv.id_venta = idVentaApertCajon;
                            eInterv.claveSupervisor = Entorno.Instancia.Usuario.UsuarioSupervisor.ClaveSupervisor;
                            eInterv.motivo = motivoIntervencion;
                            eInterv.nro_transac = Convert.ToInt32(Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1);

                            pInterv.GuardarIntervencion(eInterv, Entorno.Instancia.Terminal, Entorno.Instancia.Usuario, out respuesta);
                        }


                        Entorno.Instancia.Terminal = terminal;
                        Entorno.Instancia.IdsAcumulados = idsAcumulados;
                        Entorno.Instancia.Usuario.UsuarioSupervisor = null;

                        Entorno.Instancia.Recogida = null;
                        Entorno.Instancia.Prestamo = null;

                        // Imprimir
                        Entorno.Instancia.Impresora.Imprimir(factura, cortarPapel: true, abrirCajon: false);
                        log.Info("[GenerarTransaccionApertura.AbrirCajon] Imprimir Operación: " + factura);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info($"[GenerarTransaccionApertura] Error: {ex.Message}");
            }
        }

        public static void EmitirAlerta()
        {
            try
            {
                // Validar parámetros
                bool emiteAlerta = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.emite_alerta_sonora");

                if (!emiteAlerta)
                    return;

                int repeticionAlerta = Entorno.Instancia.Parametros.ObtenerValorParametro<int>("pdv.repeticion_alerta_sonora");

                for (int i = 0; i < repeticionAlerta; i++)
                {
                    Console.Beep(3000, 300);
                }
            }
            catch (Exception ex)
            {

                log.Info($"[EmitirAlerta] Error: {ex.Message}");
            }
        }
    }
}
