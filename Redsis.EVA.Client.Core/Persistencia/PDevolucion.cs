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
    public class PDevolucion
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO : este requiere manejo transaccional!.
        public void GuardarDevolucion(EDevolucion devolucion, ref Dictionary<string, string> IdsAcumulados, ETerminal terminal, EUsuario usuario, string tipo, string contenido, string modeloImpresora, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            //

            //devolucion.DescontarCambio();
            //1. Creamos el encabezado de la venta.
            string idVenta = Guid.NewGuid().ToString();
            RVenta rventa = new RVenta();
            respuesta = new Respuesta(false);
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rventa.CrearVenta(idVenta, devolucion.BrutoNegativo, devolucion.BrutoPositivo, terminal.Codigo, tipo, 1, (long)terminal.NumeroUltimaFactura + 1,
                        (long)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario, 0) == 1)
                    {

                        //2. Creamos cada detalle de la venta.
                        int consecutivo = 0;
                        var tirilla = devolucion.tirilla;
                        foreach (EItemVenta detalle in tirilla)
                        {
                            var idVentasArticulo = Guid.NewGuid();
                            consecutivo += 1;
                            rventa.CrearDetalleVenta(idVentasArticulo, detalle.Articulo.Id, detalle.Articulo.CodigoImpresion, terminal.Codigo, consecutivo, detalle.Impuesto,
                                (long)terminal.NumeroUltimaTransaccion + 1, detalle.Articulo.Impuesto1, detalle.Peso, usuario.IdUsuario, detalle.Valor, idVenta, detalle.Cantidad, detalle.CodigoLeido);
                            if (implementaImpuestoCompuesto)
                            {
                                rventa.ActualizarVentasArticulo(idVentasArticulo);
                                foreach (KeyValuePair<EImpuestosArticulo, decimal> entry in detalle.Impuestos)
                                {
                                    rventa.CrearVentasArticuloImpuesto(idVentasArticulo, entry.Key.Descripcion, entry.Key.Identificador, entry.Key.Porcentaje, entry.Key.Valor, entry.Key.TipoImpuesto, entry.Value, entry.Key.Id);
                                }
                            }
                        }

                        //3. Creamos totales de venta(registro_venta).
                        if (IdsAcumulados == null)
                        {
                            IdsAcumulados = new Dictionary<string, string>();
                            IdsAcumulados.Add("idRegistroVenta", Guid.NewGuid().ToString());
                            //Creamos el registro venta
                            rventa.CrearRegistroVenta(IdsAcumulados["idRegistroVenta"], terminal.Codigo, usuario.IdUsuario);
                        }
                        //Actualizo registro venta
                        rventa.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], devolucion.BrutoPositivo, devolucion.BrutoNegativo, devolucion.NumeroDeItemsVenta, 0, devolucion.TotalVenta, 1, devolucion.TotalVenta, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

                        //4. Creamos el detalle de los medios de pago.
                        consecutivo = 1;
                        rventa.CrearMedioPago("1", terminal.Codigo, consecutivo, (long)terminal.NumeroUltimaTransaccion + 1, devolucion.TotalVenta, idVenta, 0, "00", "00", 0);

                        //5. Creamos totales de medios de pago por cada medio de pago.
                        if (!IdsAcumulados.ContainsKey("1"))
                        {
                            IdsAcumulados.Add("1", Guid.NewGuid().ToString());
                            rventa.CrearTotalesMedioPago(IdsAcumulados["1"], IdsAcumulados["idRegistroVenta"], "1");
                        }

                        //Actualizamos el medio de pago
                        rventa.ActualizarTotalesMedioPago(IdsAcumulados["1"], devolucion.TotalVenta);
                        if (devolucion.Cliente != null)
                        {
                            rventa.CrearVentasCliente(devolucion.Cliente.Id, idVenta);
                        }
                        rventa.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura + 1, (long)terminal.NumeroUltimaTransaccion + 1);
                        //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(contenido);
                        //contenido = System.Convert.ToBase64String(plainTextBytes);
                        rventa.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, (long)terminal.NumeroUltimaFactura + 1, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);
                        respuesta.Valida = true;

                    }
                    else
                    {
                        throw new Exception("[GuardarDevolucion]: Transaccion no pudo ser guardada.");
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
                    log.Error("[GuardarDevolucion]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[GuardarDevolucion]: No pudo ser guardada la transaccion: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, "[GuardarDevolucion]: No pudo ser guardada la devolucion. ");
                log.Error("[GuardarDevolucion]: No pudo ser guardada la devolucion. " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }
    }
}
