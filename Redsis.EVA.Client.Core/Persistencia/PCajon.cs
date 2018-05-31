using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PCajon
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public void GuardarAperturaCajon(ref Dictionary<string, string> IdsAcumulados, string tipo, ETerminal terminal, EUsuario usuario, string contenido, out Respuesta respuesta)
        {
            //1. Creamos el encabezado de la venta.
            string idVenta = Guid.NewGuid().ToString();
            RCajon rCajon = new RCajon();
            respuesta = new Respuesta(false);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rCajon.CrearTransAbrirCajon(idVenta, 0, terminal.Codigo, tipo, 1, terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario) == 1)
                    {
                        //3. Creamos totales de venta(registro_venta).
                        if (IdsAcumulados == null)
                        {
                            IdsAcumulados = new Dictionary<string, string>();
                            IdsAcumulados.Add("idRegistroVenta", Guid.NewGuid().ToString());
                            //Creamos el registro venta
                            rCajon.CrearRegistroVenta(IdsAcumulados["idRegistroVenta"], terminal.Codigo, usuario.IdUsuario);
                        }

                        //Actualizamos el terminal
                        rCajon.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);
                        rVenta.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);

                        rVenta.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, string.Empty, (long)terminal.NumeroUltimaFactura, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);
                        respuesta.Valida = true;
                        respuesta.Mensaje = idVenta;
                    }
                    else
                    {
                        throw new Exception("[GuardarRecogida]: Transaccion no pudo ser guardada.");
                    }
                    scope.Complete();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == -2 || e.Number == 121)
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Se perdió la conexión con el servidor.";
                    log.Error("[GuardarRecogida]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[Guardarrecogida]: No pudo ser guardada la venta: " + e.Message);
                }

                //
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, " No pudo ser guardada la recogida.");
                log.Error("[GuardarRecogida]: No pudo ser guardada la recogida. " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }
    }
}
