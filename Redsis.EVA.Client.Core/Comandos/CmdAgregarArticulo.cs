using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using System.Linq;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS.Enums;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    public class CmdAgregarArticulo : ComandoAbstract
    {
        #region Properties
        private SolicitudAgregarArticulo solicitud;
        public string CodigoArticulo { get; private set; }
        public int CantidadArticulo { get; private set; }
        #endregion

        #region Constructor
        public CmdAgregarArticulo(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as SolicitudAgregarArticulo;
        }
        #endregion

        #region Methods

        //Búsqueda del artículo.
        public override void Ejecutar()
        {
            //
            log.Info("[CmdAgregarArticulo] Articulo Ingresado: " + solicitud.ValorEntrada);

            bool ventaSoloCodArticulo = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("server.ventaSoloPorArticuloCod"); ;
            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");

            #region validar ingreso de artículo

            //
            string[] entrada = new string[] { };
            string articulo = "";
            int cantidad = 1;
            bool cancelarItem = Reactor.Instancia.EstadoFSMActual == EstadosFSM.CancelarItem;

            //Valida si se debe agregar el último artículo de la lista o no.
            if (string.IsNullOrEmpty(solicitud.ValorEntrada))
            {
                //Se busca el último artículo agregado.
                if (Entorno.Instancia.Vista.PanelVentas.Tirilla.IsNullOrEmptyList())
                {
                    log.Warn("[CmdAgregarArticulo] no hay artículos para agregar");
                    iu.PanelVentas.VisorMensaje = "No hay artículos en la lista";
                    iu.PanelVentas.VisorEntrada = string.Empty;
                    return;
                }
                else
                {
                    var itemArt = Entorno.Instancia.Vista.PanelVentas.Tirilla.FirstOrDefault();
                    entrada = new string[] { itemArt.Codigo };
                }
            }
            else
            {
                entrada = solicitud.ValorEntrada.Split('*');
            }

            //valida la entrada ingresada pro el usuario.
            if (entrada.Length > 1)
            {
                //articulo
                articulo = entrada[1];
                if (string.IsNullOrEmpty(articulo))
                {
                    //Se busca el último artículo agregado.
                    if (Entorno.Instancia.Vista.PanelVentas.Tirilla.IsNullOrEmptyList())
                    {
                        log.Warn("[CmdAgregarArticulo] no hay artículos para agregar");
                        iu.PanelVentas.VisorMensaje = "No hay artículos en la lista";
                        iu.PanelVentas.VisorEntrada = string.Empty;
                        return;
                    }
                    else
                    {
                        var itemArt = Entorno.Instancia.Vista.PanelVentas.Tirilla.FirstOrDefault();
                        articulo = itemArt.Codigo;
                    }
                }

                //
                if (!int.TryParse(entrada[0], out cantidad))
                {
                    string msj = string.Format("El valor ingresado para la cantidad es inválido. [{0}]", entrada[0]);
                    log.WarnFormat("[CmdAgregarArticulo] {0}", msj);
                    iu.PanelVentas.VisorMensaje = msj;
                }
            }
            else if (entrada.Length == 1)
            {
                if (string.IsNullOrEmpty(entrada[0]))
                {
                    if (Entorno.Instancia.Vista.PanelVentas.Tirilla.Count > 0)
                    {
                        articulo = iu.PanelVentas.Tirilla.FirstOrDefault().Codigo;
                    }
                }
                else
                {
                    articulo = entrada[0];
                }
            }

            this.CodigoArticulo = articulo;
            this.CantidadArticulo = cancelarItem ? -cantidad : cantidad;

            #endregion

            Respuesta respuesta = new Respuesta();
            var tiempoBusquedaArt = new MetricaTemporizador("BuscarArticuloVenta");
            //todo: Para buscar un artículo, no es necesario instanciar
            EArticulo eArticulo = new PArticulo().BuscarArticuloPorCodigo(this.CodigoArticulo, ventaSoloCodArticulo, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoBusquedaArt.Para().AgregarPropiedad("Encontrado", true).AgregarPropiedad("Codigo", eArticulo.CodigoImpresion).AgregarPropiedad("Descripcion", eArticulo.DescripcionLarga).AgregarPropiedad("Impuesto", eArticulo.Impuesto1).AgregarPropiedad("Valor", eArticulo.PrecioVenta1));

                log.InfoFormat("[CmdAgregarArticulo] Articulo encontrado: {0}, Transaccion: {1}, Factura {2}", eArticulo.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                Procesar(eArticulo);
                log.DebugFormat("[CmdAgregarArticulo] {0}", this.ToString());
            }
            else
            {
                Telemetria.Instancia.AgregaMetrica(tiempoBusquedaArt.Para().AgregarPropiedad("Encontrado", false).AgregarPropiedad("Codigo", this.CodigoArticulo).AgregarPropiedad("Mensaje", respuesta.Mensaje));
                iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
                iu.PanelVentas.VisorEntrada = string.Empty;

                // Emitir sonido
                //SystemSounds.Asterisk.Play();

                // TODO: Parametrizar
                Utilidades.EmitirAlerta();
            }

            if (cancelarItem)
            {
                SolicitudVolver solicitudOperacion = new SolicitudVolver(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(solicitudOperacion);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="articulo"></param>
        private void Procesar(EArticulo articulo)
        {
            //todo: incluir try catch para todo el método
            //todo: realizar todas las validaciones referentes al artículo y evitar agregar alguno con alguna propiedad faltante (ej. que no tenga impuestos)

            Respuesta respuesta = new Respuesta();

            //todo: cambiar nombre, por uno más descriptivo.
            Respuesta respuesta2 = new Respuesta();

            bool estaAbierta = Entorno.Instancia.Venta.EstaAbierta;
            bool checkFactura = true;
            bool checkFecha = true;

            if (!estaAbierta)
            {
                checkFactura = Entorno.Instancia.Terminal.VerificarLimiteNumeracion(out respuesta);
                checkFecha = Entorno.Instancia.Terminal.VerificarFechaAutorizacion(out respuesta2);
            }

            if (checkFactura && checkFecha)
            {
                bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");

                //todo: antes de agregar el artículo debería validar su listado de impuesto vs el listado de impeustos configurado.
                EItemVenta item = Entorno.Instancia.Venta.AgregarArticulo(articulo, this.CantidadArticulo, this.CodigoArticulo, Entorno.Instancia.Impuestos.Impuestos, implementaImpuestoCompuesto, out respuesta);
                if (respuesta.Valida)
                {
                    // obtener cantidad actual de artículo
                    decimal cantidadActualArt = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Cantidad);
                    string descripcionArt = articulo.DescripcionLarga;

                    //
                    iu.PanelVentas.AgregarItemTirilla(new DItemTirilla(
                        item.CodigoLeido,
                        item.Articulo.DescripcionCorta,
                        item.Articulo.PrecioVenta1,
                        item.Cantidad,
                        item.Valor
                    ));

                    iu.PanelVentas.Tirilla.Insert(0, (new DItemTirilla(
                        item.CodigoLeido,
                        item.Articulo.DescripcionCorta,
                        item.Articulo.PrecioVenta1,
                        item.Cantidad,
                        item.Valor
                    )));

                    decimal newValor = articulo.PrecioVenta1;

                    if (item.Cantidad != 1 || cantidadActualArt > 0)
                    {
                        decimal newCantidad = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Cantidad);

                        if (newCantidad != 0)
                            descripcionArt = string.Format("{0} [{1}]", articulo.DescripcionLarga, newCantidad);
                    }

                    newValor = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Subtotal);

                    //
                    iu.PanelVentas.VisorCliente.Descripcion = descripcionArt;
                    iu.PanelVentas.VisorCliente.ValorItem = newValor;

                    decimal pagosVenta = Entorno.Instancia.Venta.Pagos.Sum(x => x.Valor);

                    if (pagosVenta > 0)
                        iu.PanelVentas.VisorCliente.Total = Entorno.Instancia.Venta.TotalVenta - pagosVenta;
                    else
                        iu.PanelVentas.VisorCliente.Total = Entorno.Instancia.Venta.TotalVenta;

                    iu.PanelVentas.VisorCliente.Items = Entorno.Instancia.Venta.NumeroDeItemsVenta;

                    //Pantalla cliente.
                    iu.MostrarDisplayCliente(Enums.DisplayCliente.DisplayVenta);

                    //
                    iu.PanelVentas.VisorMensaje = string.Empty;
                    iu.PanelVentas.VisorEntrada = string.Empty;
                    Entorno.Instancia.Vista.PanelOperador.MensajeOperador = string.Empty;


                    if (!estaAbierta)
                    {
                        Respuesta respuestaEstadoImpresora = Entorno.Instancia.Impresora.ValidarEstado();
                        if (!respuestaEstadoImpresora.Valida)
                        {
                            //todo: mostrar mensaje de impresora.
                        }

                        log.InfoFormat("[CmdAgregarArticulo] --> Venta Iniciada: Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        Telemetria.Instancia.AgregaMetrica(new Evento("VentaIniciada").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)));

                        Entorno.Instancia.Impresora.ValidarEstado();
                        if (!respuestaEstadoImpresora.Valida)
                        {
                            //todo: mostrar mensaje de impresora.
                        }
                    }

                    if (item.Cantidad < 0)
                    {
                        //Log archivo
                        log.InfoFormat("[CmdAgregarArticulo] Articulo cancelado: {0}, Transaccion: {1}, Factura {2}", item.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        //Log a azure
                        Telemetria.Instancia.AgregaMetrica(new Evento("VentaArticuloCancelado").AgregarPropiedad("Codigo", item.Articulo.CodigoImpresion).AgregarPropiedad("Descripcion", item.Articulo.DescripcionLarga).AgregarPropiedad("Impuesto", item.Articulo.Impuesto1).AgregarPropiedad("Valor", item.Articulo.PrecioVenta1).AgregarPropiedad("TotalImpuesto", item.Impuesto).AgregarPropiedad("Cantidad", item.Cantidad).AgregarPropiedad("TotalVenta", item.Valor).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)));
                    }
                    else
                    {
                        //Log archivo
                        log.InfoFormat("[CmdAgregarArticulo] Articulo agregado: {0}, Transaccion: {1}, Factura {2}", item.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        //Log a azure
                        Telemetria.Instancia.AgregaMetrica(new Evento("VentaArticuloAgregado").AgregarPropiedad("Codigo", item.Articulo.CodigoImpresion).AgregarPropiedad("Descripcion", item.Articulo.DescripcionLarga).AgregarPropiedad("Impuesto", item.Articulo.Impuesto1).AgregarPropiedad("Valor", item.Articulo.PrecioVenta1).AgregarPropiedad("TotalImpuesto", item.Impuesto).AgregarPropiedad("Cantidad", item.Cantidad).AgregarPropiedad("TotalVenta", item.Valor).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)));
                    }
                    //log.Debug("Artículo agregado");

                    if (Reactor.Instancia.EstadoFSMActual == EstadosFSM.CancelarItem)
                    {
                        iu.PanelVentas.VisorMensaje = "Articulo cancelado exitosamente.";
                    }

                }
                else
                {
                    log.WarnFormat("[CmdAgregarArticulo.Procesar] {0}", respuesta.Mensaje);
                    iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
                    iu.PanelVentas.VisorEntrada = string.Empty;
                }
            }
            else
            {
                if (!checkFactura)
                {
                    log.WarnFormat("[CmdAgregarArticulo.Procesar] {0}", respuesta.Mensaje);
                    iu.PanelVentas.VisorMensaje = "ERROR - " + respuesta.Mensaje;
                }
                else if (!checkFecha)
                {
                    log.WarnFormat("[CmdAgregarArticulo.Procesar] {0}", respuesta.Mensaje);
                    iu.PanelVentas.VisorMensaje = "ERROR - " + respuesta2.Mensaje;
                }
                iu.PanelVentas.VisorEntrada = string.Empty;
            }


            ///
        }

        public string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }

        #endregion
    }
}
