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
    public class PVenta
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void GuardarVenta(EVenta venta, ref Dictionary<string, string> IdsAcumulados, ETerminal terminal, EUsuario usuario, String tipo, string contenido, string modeloImpresora, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            respuesta = new Respuesta(false);

            try
            {

                venta.DescontarCambio();
                //1. Creamos el encabezado de la venta.
                string idVenta = Guid.NewGuid().ToString();
                RVenta rventa = new RVenta();
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rventa.CrearVenta(idVenta, venta.BrutoNegativo, venta.BrutoPositivo, terminal.Codigo, tipo, 1, (long)terminal.NumeroUltimaFactura + 1, (long)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario, venta.PorPagar * -1) == 1)
                    {

                        //2. Creamos cada detalle de la venta.
                        int consecutivo = 0;
                        var tirilla = venta.tirilla;
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
                        rventa.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], venta.BrutoPositivo, venta.BrutoNegativo, venta.NumeroDeItemsVenta, venta.NumeroDeItemsNegativo, venta.TotalVenta, 0, 0, 0, 0, venta.BrutoNegativo, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);

                        //4. Creamos el detalle de los medios de pago.
                        consecutivo = 0;
                        List<EPago> pagos = venta.Pagos;
                        foreach (EPago pago in pagos)
                        {
                            consecutivo += 1;
                            rventa.CrearMedioPago(pago.MedioPago.Codigo, terminal.Codigo, consecutivo, (long)terminal.NumeroUltimaTransaccion + 1, pago.Valor, idVenta, pago.NumeroDocumento, pago.CodBanco, pago.NumeroCuenta, pago.MesesPlazo);

                            //5. Creamos totales de medios de pago por cada medio de pago.
                            if (!IdsAcumulados.ContainsKey(pago.MedioPago.Codigo))
                            {
                                IdsAcumulados.Add(pago.MedioPago.Codigo, Guid.NewGuid().ToString());
                                rventa.CrearTotalesMedioPago(IdsAcumulados[pago.MedioPago.Codigo], IdsAcumulados["idRegistroVenta"], pago.MedioPago.Codigo);
                            }
                            //Actualizamos el medio de pago
                            rventa.ActualizarTotalesMedioPago(IdsAcumulados[pago.MedioPago.Codigo], pago.Valor);
                        }
                        rventa.CrearVentaCambio(venta.PorPagar * -1, idVenta);
                        if (venta.Cliente != null)
                        {
                            rventa.CrearVentasCliente(venta.Cliente.Id, idVenta);
                        }
                        rventa.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura + 1, (long)terminal.NumeroUltimaTransaccion + 1);
                        respuesta.Valida = true;
                        //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(contenido);
                        //contenido = System.Convert.ToBase64String(plainTextBytes);
                        rventa.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, (long)terminal.NumeroUltimaFactura + 1, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);

                    }
                    else
                    {
                        throw new Exception("[GuardarVenta]: Transaccion no pudo ser guardada.");
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
                    log.Error("[GuardarVenta]: No pudo ser guardada la venta: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[GuardarVenta]: No pudo ser guardada la venta: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception e)
            {
                //venta.RevertirDescontarCambio();
                respuesta.Valida = false;
                respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                log.Error("[GuardarVenta]: No pudo ser guardada la venta: " + e.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
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
                ImprimirIds(registros);
                Dictionary<string, string> acumulados = new Dictionary<string, string>();
                acumulados.Add("idRegistroVenta", (string)registros.Rows[0]["id_registro_venta"]);
                //System.Diagnostics.Debug.WriteLine(registros.Rows[0].Field<string>("id_registro_venta"));
                //System.Diagnostics.Debug.WriteLine(registros.Rows[0].Field<string>("id_tot_med_pago"));
                foreach (DataRow registro in registros.Rows)
                {
                    if ((string)registro["id_medio"] != null)
                    {
                        //System.Diagnostics.Debug.WriteLine(registro.Field<string>("id_medio"));
                        acumulados.Add((string)registro["id_medio"], (string)registro["id_tot_med_pago"]);
                    }

                }
                return acumulados;
            }

        }

        public string ImprimirUltima(string codTerminal, string codUsuario, out Respuesta respuesta)
        {
            string contenido = "";
            RVenta rventa = new RVenta();
            respuesta = new Respuesta(false);
            var registro = rventa.ImprimirUltima(codTerminal, codUsuario);

            if (registro == null)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "Ultima factura no encontrada";
            }
            else
            {
                contenido = (string)registro["contenido"];
                respuesta.Valida = true;
            }

            return contenido;

        }

        public List<string> BuscarFactura(string codUsuario, string codTerminal, string nroTransaccion, string nroFactura, string prefijo, out Respuesta respuesta)
        {
            List<string> contenidos = new List<string>();

            RVenta rventa = new RVenta();
            respuesta = new Respuesta(false);
            var registros = rventa.BuscarFactura(codUsuario, codTerminal, prefijo, nroFactura, nroTransaccion);
            foreach (DataRow registro in registros.Rows)
            {
                string contenido = (string)registro["contenido"];
                contenidos.Add(contenido);
            }
            return contenidos;
        }

        public Decimal DineroEnCaja(string codTerminal, string codUsuario, out Respuesta respuesta)
        {
            RVenta rventa = new RVenta();
            respuesta = new Respuesta(true);
            var registro = rventa.DineroEnCaja(codTerminal, codUsuario);
            Decimal valor = (decimal)registro["total"];
            return valor;
        }

        public void CancelarVenta(ETransaccion transaccion, ref Dictionary<string, string> IdsAcumulados, ETerminal terminal, EUsuario usuario, String tipo, out Respuesta respuesta)
        {
            respuesta = new Respuesta(true);
            string idVenta = Guid.NewGuid().ToString();
            RVenta rventa = new RVenta();
            decimal bruto = transaccion.BrutoNegativo > transaccion.BrutoPositivo ? transaccion.BrutoNegativo : transaccion.BrutoPositivo;
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rventa.CrearVenta(idVenta, bruto, bruto, terminal.Codigo, tipo, 1, 00, (long)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario, 00) == 1)
                    {
                        //3. Creamos totales de venta(registro_venta).
                        if (IdsAcumulados == null)
                        {
                            IdsAcumulados = new Dictionary<string, string>();
                            IdsAcumulados.Add("idRegistroVenta", Guid.NewGuid().ToString());
                            //Creamos el registro venta
                            rventa.CrearRegistroVenta(IdsAcumulados["idRegistroVenta"], terminal.Codigo, usuario.IdUsuario);
                        }
                        //Actualizo registro venta
                        rventa.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], bruto, bruto, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, bruto);

                        rventa.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);
                    }
                    else
                    {
                        throw new Exception("[CancelarVenta]: Transaccion no pudo ser guardada.");
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
                    log.Error("[CancelarVenta]: No pudo ser guardada la transacion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[CancelarVenta]: No pudo ser guardada la venta: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception e)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "[CancelarVenta]: No pudo ser guardada la transaccion.";
                log.Error("[CancelarVenta]: No pudo ser guardada la venta. " + e.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }


        }

        public void ImprimirIds(DataTable ids)
        {
            foreach (DataRow dr in ids.Rows)
            {
                log.DebugFormat("{0}: {1}", "Id medio pago:", (string)dr["id_medio"]);
                log.DebugFormat("{0}: {1}", "ID total medio pago: ", (string)dr["id_tot_med_pago"]);

            }
        }
    }
}
