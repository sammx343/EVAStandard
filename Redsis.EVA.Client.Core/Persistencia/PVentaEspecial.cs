using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Transactions;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PVentaEspecial
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EVentasEspecial GetAllVentaEspecial(EClientes clientes)
        {
            var repositorio = new RVentaEspecial();
            var ventas = new EVentasEspecial();
            var registros = repositorio.GetAllVentaEspecial();
            EVentaEspecial anterior = null;
            foreach (DataRow registro in registros.Rows)
            {
                var venta = VentaEspecialUtil.InstanciarDesde(registro, clientes);
                if (anterior != null)
                {
                    if (!String.Equals(anterior.Id, venta.Id, StringComparison.Ordinal))
                    {
                        ventas.ListaVentas.Add(anterior);
                    }
                    else
                    {
                        foreach (ECliente c in anterior.Clientes.ListClientes)
                        {
                            venta.Clientes.ListClientes.Add(c);
                        }
                    }
                }
                anterior = venta;
            }
            ventas.ListaVentas.Add(anterior);
            return ventas;
        }

        public void GuardarVentaEspecial(EFacturaVentaEspecialSinMedioPago venta, ref Dictionary<string, string> IdsAcumulados, ETerminal terminal, EUsuario usuario, String tipo, string contenido, string modeloImpresora, bool implementaImpuestoCompuesto, out Respuesta respuesta)
        {
            //venta.DescontarCambio();
            respuesta = new Respuesta(false);
            //1. Creamos el encabezado de la venta.
            string idVenta = Guid.NewGuid().ToString();
            RVenta rventa = new RVenta();
            RVentaEspecial rventaespecial = new RVentaEspecial();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rventa.CrearVenta(idVenta, venta.BrutoNegativo, venta.BrutoPositivo, terminal.Codigo, tipo, 1, (long)terminal.NumeroUltimaFactura + 1, (long)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario, 0) == 1)
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
                        //rventa.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], venta.BrutoPositivo, venta.BrutoNegativo, venta.NumeroDeItemsVenta, venta.NumeroDeItemsNegativo, venta.TotalVenta);
                        rventaespecial.ActualizarRegistroVentaEspecial(venta.BrutoNegativo, venta.BrutoPositivo, venta.NumeroDeItemsVenta, venta.NumeroDeItemsNegativo, 1, venta.BrutoNegativo, venta.TotalVenta, 0, 0, IdsAcumulados["idRegistroVenta"]);
                        rventaespecial.AsociarTipoVentaConEncabezado(venta.TipoVentaEspecial.Id, idVenta);

                        if (venta.Cliente != null)
                        {
                            rventa.CrearVentasCliente(venta.Cliente.Id, idVenta);
                        }
                        rventa.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura + 1, (long)terminal.NumeroUltimaTransaccion + 1);
                        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(contenido);
                        //contenido = System.Convert.ToBase64String(plainTextBytes);
                        rventa.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, (long)terminal.NumeroUltimaFactura + 1, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);
                        respuesta.Valida = true;
                    }
                    else
                    {
                        throw new Exception("[GuardarVenta]: Transaccion no pudo ser guardada.");
                    }
                    scope.Complete();
                }
            }
            catch (TransactionAbortedException ex)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "[GuardarVentaEspecial]: No pudo ser guardada la venta.";
                log.Error("[GuardarVentaEspecial]: No pudo ser guardada la venta. " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }

        public class VentaEspecialUtil
        {
            public static EVentaEspecial InstanciarDesde(DataRow registro, EClientes clientes)
            {
                //log.Info("[InstanciarDesde] Parametros "+registro.Field<string>("id_cliente"));
                Respuesta respuesta = new Respuesta();
                if (registro == null)
                {

                    respuesta.Valida = false;
                    respuesta.Mensaje = "Registro nulo o contiene campos nulos.";
                    return null;
                    //throw new ApplicationException("Registro nulo o contiene campos nulos.");
                }
                EVentaEspecial a;
                if (Convert.ToBoolean((byte)registro["registra_cliente"]))
                {
                    EClientesVentaEspecial clientesVenta;
                    if (Convert.ToBoolean((byte)registro["especifica_clientes"]))
                    {
                        string clientid = ((string)registro["id_cliente"]).Trim();
                        var c = clientes.Cliente(clientid, out respuesta);
                        clientesVenta = new EClientesVentaEspecial(
                        Convert.ToBoolean((byte)registro["diligencia_tipo_identificacion"]),
                        Convert.ToBoolean((byte)registro["diligencia_identificacion"]),
                        Convert.ToBoolean((byte)registro["diligencia_primer_nombre"]),
                        Convert.ToBoolean((byte)registro["diligencia_segundo_nombre"]),
                        Convert.ToBoolean((byte)registro["diligencia_primer_apellido"]),
                        Convert.ToBoolean((byte)registro["diligencia_segundo_apellido"]),
                        Convert.ToBoolean((byte)registro["diligencia_tipo_cliente"]),
                        Convert.ToBoolean((byte)registro["diligencia_ciudad"]),
                        Convert.ToBoolean((byte)registro["diligencia_pais"]),
                        Convert.ToBoolean((byte)registro["diligencia_celular"]),
                        Convert.ToBoolean((byte)registro["diligencia_telefono"]),
                        Convert.ToBoolean((byte)registro["diligencia_correo"]),
                        Convert.ToBoolean((byte)registro["diligencia_direccion"])
                        );
                    }
                    else
                    {
                        clientesVenta = new EClientesVentaEspecial(
                        Convert.ToBoolean((byte)registro["diligencia_tipo_identificacion"]),
                        Convert.ToBoolean((byte)registro["diligencia_identificacion"]),
                        Convert.ToBoolean((byte)registro["diligencia_primer_nombre"]),
                        Convert.ToBoolean((byte)registro["diligencia_segundo_nombre"]),
                        Convert.ToBoolean((byte)registro["diligencia_primer_apellido"]),
                        Convert.ToBoolean((byte)registro["diligencia_segundo_apellido"]),
                        Convert.ToBoolean((byte)registro["diligencia_tipo_cliente"]),
                        Convert.ToBoolean((byte)registro["diligencia_ciudad"]),
                        Convert.ToBoolean((byte)registro["diligencia_pais"]),
                        Convert.ToBoolean((byte)registro["diligencia_celular"]),
                        Convert.ToBoolean((byte)registro["diligencia_telefono"]),
                        Convert.ToBoolean((byte)registro["diligencia_correo"]),
                        Convert.ToBoolean((byte)registro["diligencia_direccion"])
                        );
                    }

                    a = new EVentaEspecial(
                    (string)registro["id_tipo_venta"],
                    (string)registro["descripcion"],
                    Convert.ToBoolean((byte)registro["impto1"]),
                    Convert.ToBoolean((byte)registro["impto2"]),
                    Convert.ToBoolean((byte)registro["impto3"]),
                    Convert.ToBoolean((byte)registro["impto4"]),
                    (string)registro["mensaje"],
                    Convert.ToBoolean((byte)registro["registra_pago"]),
                    Convert.ToBoolean((byte)registro["registra_cliente"]),
                    clientesVenta
                    );
                }
                else
                {
                    a = new EVentaEspecial(
                    (string)registro["id_tipo_venta"],
                    (string)registro["descripcion"],
                    Convert.ToBoolean((byte)registro["impto1"]),
                    Convert.ToBoolean((byte)registro["impto2"]),
                    Convert.ToBoolean((byte)registro["impto3"]),
                    Convert.ToBoolean((byte)registro["impto4"]),
                    (string)registro["mensaje"],
                    Convert.ToBoolean((byte)registro["registra_pago"]),
                    Convert.ToBoolean((byte)registro["registra_cliente"])
                    );
                }

                return a;
            }
        }
    }
}
