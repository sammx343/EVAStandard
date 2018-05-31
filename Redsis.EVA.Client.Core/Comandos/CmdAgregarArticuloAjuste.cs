using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS;
using EvaPOS.Enums;
using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarArticuloAjuste : ComandoAbstract
    {
        #region Properties
        private SolicitudAgregarArticulo solicitud;
        public string CodigoArticulo { get; private set; }
        public int CantidadArticulo { get; private set; }
        #endregion

        #region Constructor
        public CmdAgregarArticuloAjuste(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as SolicitudAgregarArticulo;
        }
        #endregion

        #region Methods

        public override void Ejecutar()
        {
            //
            log.Info("[CmdAgregarArticuloAjuste] Agregar artículo ajuste");

            //Búsqueda del artículo.
            //TODO modificar consulta de valores de parametros.
            bool ventaSoloCodArticulo = false ;
            if (!bool.TryParse(Parametros.Parametro("server.ventaSoloPorArticuloCod").Valor, out ventaSoloCodArticulo))
            {
                log.Error("[CmdAgregarArticuloAjuste] El valor del parametro server.ventaSoloPorArticuloCod no es válido");
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
            bool cancelarItem = Reactor.Instancia.EstadoFSMActual == EstadosFSM.CancelarItemAjuste;

            //Valida si se debe agregar el último artículo de la lista o no.
            if (string.IsNullOrEmpty(solicitud.ValorEntrada))
            {
                //Se busca el último artículo agregado.
                if (Entorno.Instancia.Vista.PanelVentas.Tirilla.IsNullOrEmptyList())
                {
                    log.Warn("[AgregarArticuloAjuste] no hay artículos para agregar");
                    iu.PanelVentas.VisorMensaje = "No hay artículos en la lista";
                    iu.PanelVentas.VisorEntrada = string.Empty;
                    return;
                }
                else
                {
                    var itemArt = Entorno.Instancia.Vista.PanelVentas.Tirilla.LastOrDefault();
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

                //
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
            this.CantidadArticulo = !cancelarItem ? cantidad : -cantidad;

            #endregion

            Respuesta respuesta = new Respuesta();
            var tiempoBusquedaAjuste = new MetricaTemporizador("BuscarArticuloAjuste");
            EArticulo eArticulo = new PArticulo().BuscarArticuloPorCodigo(this.CodigoArticulo, ventaSoloCodArticulo, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoBusquedaAjuste.Para().AgregarPropiedad("Encontrado", true).AgregarPropiedad("Codigo", eArticulo.CodigoImpresion).AgregarPropiedad("Descripcion", eArticulo.DescripcionLarga).AgregarPropiedad("Impuesto", eArticulo.Impuesto1).AgregarPropiedad("Valor", eArticulo.PrecioVenta1));

                log.InfoFormat("[CmdAgregarArticuloAjuste] Articulo encontrado: {0}, Transaccion: {1}, Factura {2}", eArticulo.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                if (Entorno.Instancia.Ajuste.CopiaTirilla.Count == 0)
                {
                    Entorno.Instancia.Ajuste.EstaAbierta = true;
                }
                Procesar(eArticulo);
                log.DebugFormat("[CmdAgregarArticuloAjuste] {0}", this.ToString());
            }
            else
            {
                Telemetria.Instancia.AgregaMetrica(tiempoBusquedaAjuste.Para().AgregarPropiedad("Encontrado", false).AgregarPropiedad("Codigo", this.CodigoArticulo).AgregarPropiedad("Mensaje", respuesta.Mensaje));
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
            Respuesta respuesta;
            bool estaAbierta = Entorno.Instancia.Ajuste.EstaAbierta;
            EItemVenta item = Entorno.Instancia.Ajuste.AgregarArticulo(articulo, this.CantidadArticulo, this.CodigoArticulo, Entorno.Instancia.Impuestos.Impuestos, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida)
            {
                //
                iu.PanelVentas.VisorCliente.Descripcion = articulo.DescripcionLarga;
                iu.PanelVentas.VisorCliente.ValorItem = articulo.PrecioVenta1;
                iu.PanelVentas.VisorCliente.Total = Entorno.Instancia.Ajuste.TotalVenta;
                iu.PanelVentas.VisorCliente.Items = Entorno.Instancia.Ajuste.NumeroDeItemsVenta;

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

                //
                iu.PanelVentas.VisorMensaje = string.Empty;
                iu.PanelVentas.VisorEntrada = string.Empty;

                if (!estaAbierta)
                {
                    log.InfoFormat("[CmdAgregarArticuloAjuste] --> Ajuste Iniciado: Transaccion: {0}, Factura {1}", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                    Telemetria.Instancia.AgregaMetrica(new Evento("AjusteIniciado").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));
                }

                //Log archivo
                log.InfoFormat("[CmdAgregarArticuloAjuste] Articulo agregado: {0}, Transaccion: {1}, Factura {2}", item.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                //Log a azure
                Telemetria.Instancia.AgregaMetrica(new Evento("AjusteArticuloAgregado").AgregarPropiedad("Codigo", item.Articulo.CodigoImpresion).AgregarPropiedad("Descripcion", item.Articulo.DescripcionLarga).AgregarPropiedad("Impuesto", item.Articulo.Impuesto1).AgregarPropiedad("Valor", item.Articulo.PrecioVenta1).AgregarPropiedad("TotalImpuesto", item.Impuesto).AgregarPropiedad("Cantidad", item.Cantidad).AgregarPropiedad("TotalVenta", item.Valor).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Factura", (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1)));
                

                //log.Debug("Artículo agregado");
                
                if (Reactor.Instancia.EstadoFSMActual == EstadosFSM.CancelarItemAjuste)
                {
                    iu.PanelVentas.VisorMensaje = "Articulo cancelado correctamente.";
                }
            }
            else
            {
                System.Console.WriteLine(respuesta.Mensaje);
                iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
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