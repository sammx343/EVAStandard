using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PPrestamo
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public void GuardarPrestamo(EPrestamo prestamo, ref Dictionary<string, string> IdsAcumulados, string tipo, ETerminal terminal, EUsuario usuario, EMedioPago medioPago, string contenido, string modeloImpresora, out Respuesta respuesta)
        {
            //1. Creamos el encabezado de la venta.
            string idPrestamo = Guid.NewGuid().ToString();
            RPrestamo rPrestamo = new RPrestamo();
            respuesta = new Respuesta(false);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rPrestamo.CrearPrestamo(idPrestamo, prestamo.Valor, terminal.Codigo, tipo, 1, (long)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario) == 1)
                    {
                        //3. Creamos totales de venta(registro_venta).
                        if (IdsAcumulados == null)
                        {
                            IdsAcumulados = new Dictionary<string, string>();
                            IdsAcumulados.Add("idRegistroVenta", Guid.NewGuid().ToString());
                            //Creamos el registro venta
                            rPrestamo.CrearRegistroVenta(IdsAcumulados["idRegistroVenta"], terminal.Codigo, usuario.IdUsuario);
                        }
                        //Actualizo registro venta
                        rPrestamo.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], prestamo.Valor);

                        rPrestamo.CrearMedioPago(medioPago.Codigo, terminal.Codigo, (long)terminal.NumeroUltimaTransaccion + 1, prestamo.Valor, idPrestamo);
                        rVenta.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);

                        rVenta.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, (long)terminal.NumeroUltimaFactura, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);
                        respuesta.Valida = true;
                        respuesta.Mensaje = idPrestamo;
                    }
                    else
                    {
                        throw new Exception("[GuardarPrestamo]: Transaccion no pudo ser guardada.");
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
                    log.Error("[GuardarPrestamo]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[GuardarPrestamo]: No pudo ser guardada la transaccion: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, "No pudo ser guardado el prestamo.");
                log.Error("[GuardarPrestamo]: No pudo ser guardada el prestamo. " + ex);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }

        public Dictionary<string, string> IdsAcumulados(EUsuario usuario, ETerminal terminal)
        {
            var repositorio = new RVenta();
            DataTable registros = repositorio.IdRegistroVenta(terminal.Codigo, usuario.IdUsuario);
            if (registros.IsNullOrEmptyTable())
            {
                return null;
            }
            else
            {
                Dictionary<string, string> acumulados = new Dictionary<string, string>();
                acumulados.Add("idRegistroVenta", (string)registros.Rows[0]["id_registro_venta"]);
                //System.Diagnostics.Debug.WriteLine(registros.Rows[0].Field<string>("id_registro_venta"));
                //System.Diagnostics.Debug.WriteLine(registros.Rows[0].Field<string>("id_tot_med_pago"));
                foreach (DataRow registro in registros.Rows)
                {
                    if ((string)registro["id_medio"] != null)
                    {
                        System.Diagnostics.Debug.WriteLine((string)registro["id_medio"]);
                        acumulados.Add((string)registro["id_medio"], (string)registro["id_tot_med_pago"]);
                    }

                }
                return acumulados;
            }

        }
    }
}
