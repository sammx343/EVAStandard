﻿using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using System.Linq;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS.Enums;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarArticuloDevolucion : ComandoAbstract
    {
        #region Properties
        private SolicitudAgregarArticulo solicitud;
        public string CodigoArticulo { get; private set; }
        public int CantidadArticulo { get; private set; }
        #endregion

        #region Constructor
        public CmdAgregarArticuloDevolucion(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as SolicitudAgregarArticulo;
        }
        #endregion

        #region Methods

        public override void Ejecutar()
        {
            //
            log.Info("[CmdAgregarArticuloDevolucion] Articulo Ingresado: " + solicitud.ValorEntrada);

            //Búsqueda del artículo.
            //TODO modificar consulta de valores de parametros.
            bool ventaSoloCodArticulo = false;
            if (!bool.TryParse(Parametros.Parametro("server.ventaSoloPorArticuloCod").Valor, out ventaSoloCodArticulo))
            {
                log.Error("[CmdAgregarArticuloDevolucion] El valor del parametro server.ventaSoloPorArticuloCod no es válido");
            }

            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
            /*if (!bool.TryParse(Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
            {
                implementaImpuestoCompuesto = false;
            }*/
            #region validar ingreso de artículo

            //
            string[] entrada = new string[] { };
            string articulo = "";
            int cantidad = 1;
            bool cancelarItem = Reactor.Instancia.EstadoFSMActual == EstadosFSM.CancelarItemDevolucion;

            //Valida si se debe agregar el último artículo de la lista o no.
            if (string.IsNullOrEmpty(solicitud.ValorEntrada))
            {
                //Se busca el último artículo agregado.
                if (Entorno.Instancia.Vista.PanelVentas.Tirilla.IsNullOrEmptyList())
                {
                    log.Warn("[AgregarArticuloDevolucion] No hay artículos para agregar");
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
                    if (Entorno.Instancia.Devolucion.tirilla.IsNullOrEmptyList())
                    {
                        log.Warn("[CmdAgregarArticulo] no hay artículos para agregar");
                        iu.PanelVentas.VisorMensaje = "No hay artículos en la lista";
                        iu.PanelVentas.VisorEntrada = string.Empty;
                        return;
                    }
                    else
                    {
                        var itemArt = Entorno.Instancia.Devolucion.tirilla.FirstOrDefault();
                        articulo = itemArt.CodigoLeido;
                    }
                }


                if (!int.TryParse(entrada[0], out cantidad))
                {
                    string msj = string.Format("El valor ingresado para la cantidad es inválido. [{0}]", entrada[0]);
                    log.WarnFormat("[AgregarArticulo] {0}", msj);
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
            this.CantidadArticulo = cancelarItem ? cantidad : -cantidad;

            #endregion

            Respuesta respuesta = new Respuesta();
            var tiempoBusquedaDev = new MetricaTemporizador("BuscarArticuloDevolucion");
            EArticulo eArticulo = new PArticulo().BuscarArticuloPorCodigo(this.CodigoArticulo, ventaSoloCodArticulo, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoBusquedaDev.Para().AgregarPropiedad("Encontrado", true).AgregarPropiedad("Codigo", eArticulo.CodigoImpresion).AgregarPropiedad("Descripcion", eArticulo.DescripcionLarga).AgregarPropiedad("Impuesto", eArticulo.Impuesto1).AgregarPropiedad("Valor", eArticulo.PrecioVenta1));

                log.InfoFormat("[CmdAgregarArticuloDevolucion] Articulo encontrado: {0}, Transaccion: {1}, Factura {2}", eArticulo.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                //--> porque se debe abrir una devolución si copiatirilla es cero ?
                //if (Entorno.Instancia.Devolucion.CopiaTirilla.Count == 0)
                //{
                //    Entorno.Instancia.Devolucion.EstaAbierta = true;
                //}

                //
                Procesar(eArticulo);

                log.DebugFormat("[CmdAgregarArticuloDevolucion] {0}", this.ToString());
            }
            else
            {
                Telemetria.Instancia.AgregaMetrica(tiempoBusquedaDev.Para().AgregarPropiedad("Encontrado", false).AgregarPropiedad("Codigo", this.CodigoArticulo).AgregarPropiedad("Mensaje", respuesta.Mensaje));
                iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
                iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
                iu.PanelVentas.VisorEntrada = string.Empty;

                // Emitir sonido
                Utilidades.EmitirAlerta();
            }

            if (cancelarItem)
            {
                SolicitudVolver solicitudOperacion = new SolicitudVolver(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(solicitudOperacion);
            }
        }


        private void Procesar(EArticulo articulo)
        {
            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
            /*if (!bool.TryParse(Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
            {
                implementaImpuestoCompuesto = false;
            }*/
            //
            Respuesta respuesta = new Respuesta();
            Respuesta respuesta2 = new Respuesta();
            bool estaAbierta = Entorno.Instancia.Devolucion.EstaAbierta;
            bool checkFactura = true;
            bool checkFecha = true;

            if (!estaAbierta)
            {
                checkFactura = Entorno.Instancia.Terminal.VerificarLimiteNumeracion(out respuesta);
                checkFecha = Entorno.Instancia.Terminal.VerificarFechaAutorizacion(out respuesta2);
            }

            if (checkFactura && checkFecha)
            {
                EItemVenta item = Entorno.Instancia.Devolucion.AgregarArticulo(articulo, this.CantidadArticulo, this.CodigoArticulo, Entorno.Instancia.Impuestos.Impuestos, implementaImpuestoCompuesto, out respuesta);
                if (respuesta.Valida)
                {
                    //
                    iu.PanelVentas.AgregarItemTirilla(new DItemTirilla(
                        item.CodigoLeido,
                        item.Articulo.DescripcionCorta,
                        item.Articulo.PrecioVenta1,
                        item.Cantidad,
                        item.Valor
                    ));

                    iu.PanelVentas.Tirilla.Insert(0, new DItemTirilla(
                        item.CodigoLeido,
                        item.Articulo.DescripcionCorta,
                        item.Articulo.PrecioVenta1,
                        item.Cantidad,
                        item.Valor
                    ));


                    string descripcionArt = articulo.DescripcionLarga;
                    decimal cantidadActualArt = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Cantidad);

                    if (cantidadActualArt != -1 || cantidadActualArt > 0)
                    {
                        decimal newCantidad = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Cantidad);

                        if (newCantidad != 0)
                            descripcionArt = string.Format("{0} [{1}]", articulo.DescripcionLarga, newCantidad);
                    }

                    decimal newValor = articulo.PrecioVenta1;

                    //if (item.Cantidad != 1 || cantidadActualArt > 0)
                    //{
                    //    decimal newCantidad = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Cantidad);

                    //    if (newCantidad != 0)
                    //        descripcionArt = string.Format("{0} [{1}]", articulo.DescripcionLarga, newCantidad);
                    //}

                    newValor = iu.PanelVentas.Tirilla.Where(i => i.Codigo.Equals(item.CodigoLeido)).Sum(i => i.Subtotal);


                    //
                    iu.PanelVentas.VisorCliente.Descripcion = descripcionArt;
                    iu.PanelVentas.VisorCliente.ValorItem = newValor;
                    iu.PanelVentas.VisorCliente.Total = Entorno.Instancia.Devolucion.TotalVenta;
                    iu.PanelVentas.VisorCliente.Items = Entorno.Instancia.Devolucion.NumeroDeItemsVenta;


                    //Pantalla cliente.
                    iu.MostrarDisplayCliente(Enums.DisplayCliente.DisplayVenta);

                    //
                    iu.PanelVentas.VisorMensaje = string.Empty;
                    iu.PanelVentas.VisorEntrada = string.Empty;

                    if (!estaAbierta)
                    {
                        log.InfoFormat("[CmdAgregarArticuloDevolucion] --> Devolucion Iniciada: Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        Telemetria.Instancia.AgregaMetrica(new Evento("DevolucionIniciada").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));
                    }

                    if (item.Cantidad > 0)
                    {
                        //Log archivo
                        log.InfoFormat("[CmdAgregarArticuloDevolucion] Articulo cancelado: {0}, Transaccion: {1}, Factura {2}", item.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        //Log a azure
                        Telemetria.Instancia.AgregaMetrica(new Evento("DevolucionArticuloCancelado").AgregarPropiedad("Codigo", item.Articulo.CodigoImpresion).AgregarPropiedad("Descripcion", item.Articulo.DescripcionLarga).AgregarPropiedad("Impuesto", item.Articulo.Impuesto1).AgregarPropiedad("Valor", item.Articulo.PrecioVenta1).AgregarPropiedad("TotalImpuesto", item.Impuesto).AgregarPropiedad("Cantidad", item.Cantidad).AgregarPropiedad("TotalVenta", item.Valor).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)));
                    }
                    else
                    {
                        //Log archivo
                        log.InfoFormat("[CmdAgregarArticuloDevolucion] Articulo agregado: {0}, Transaccion: {1}, Factura {2}", item.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        //Log a azure
                        Telemetria.Instancia.AgregaMetrica(new Evento("DevolucionArticuloAgregado").AgregarPropiedad("Codigo", item.Articulo.CodigoImpresion).AgregarPropiedad("Descripcion", item.Articulo.DescripcionLarga).AgregarPropiedad("Impuesto", item.Articulo.Impuesto1).AgregarPropiedad("Valor", item.Articulo.PrecioVenta1).AgregarPropiedad("TotalImpuesto", item.Impuesto).AgregarPropiedad("Cantidad", item.Cantidad).AgregarPropiedad("TotalVenta", item.Valor).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)));
                    }

                    //log.Debug("Artículo agregado");

                    if (Reactor.Instancia.EstadoFSMActual == EstadosFSM.CancelarItemDevolucion)
                    {
                        iu.PanelVentas.VisorMensaje = "Articulo cancelado exitosamente.";
                    }
                }
                else
                {
                    iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
                    iu.PanelVentas.VisorEntrada = string.Empty;

                    //Valida si la transacción está abierta sin items en la devolución, debe cancelar la transacción
                    if (iu.PanelVentas.Tirilla.IsNullOrEmptyList())
                    {
                        Entorno.Instancia.Devolucion.EstaAbierta = false;
                        estaAbierta = false;
                    }

                }
            }
            else
            {
                if (!checkFactura)
                {
                    System.Console.WriteLine(respuesta.Mensaje);
                    iu.PanelVentas.VisorMensaje = "ERROR - " + respuesta.Mensaje;
                }
                else if (!checkFecha)
                {
                    System.Console.WriteLine(respuesta2.Mensaje);
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
