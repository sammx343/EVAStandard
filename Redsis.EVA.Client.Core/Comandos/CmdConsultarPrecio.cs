using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Solicitudes;
using System;
using Redsis.EVA.Client.Common.Enums;
using System.Linq;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Helpers;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdConsultarPrecio : ComandoAbstract
    {
        #region Properties
        private SolicitudAgregarArticulo solicitud;
        public string CodigoArticulo { get; private set; }
        #endregion

        #region Constructor
        public CmdConsultarPrecio(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as SolicitudAgregarArticulo;
        }
        #endregion

        #region Methods

        public override void Ejecutar()
        {
            //
            log.Info("[CmdConsultarPrecio] Consultando Precio: " + solicitud.ValorEntrada);

            //Búsqueda del artículo.
            //TODO modificar consulta de valores de parametros.
            bool ventaSoloCodArticulo = false;
            if (!bool.TryParse(Parametros.Parametro("server.ventaSoloPorArticuloCod").Valor, out ventaSoloCodArticulo))
            {
                log.Error("[CmdAgregarArticulo] El valor del parametro server.ventaSoloPorArticuloCod no es válido");
            }

            bool implementaImpuestoCompuesto = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.definicion_impuesto_compuesta");
            /*if (!bool.TryParse(Parametros.Parametro("pdv.definicion_impuesto_compuesta").Valor, out implementaImpuestoCompuesto))
            {
                implementaImpuestoCompuesto = false;
            }*/

            #region validar ingreso de artículo

            //
            string entrada = "";
            string articulo = "";

            entrada = solicitud.ValorEntrada;
            articulo = entrada;
            this.CodigoArticulo = articulo;

            #endregion

            Respuesta respuesta = new Respuesta();
            var tiempoConsultarPrecio = new MetricaTemporizador("ConsultarPrecio");
            EArticulo eArticulo = new PArticulo().BuscarArticuloPorCodigo(this.CodigoArticulo, ventaSoloCodArticulo, implementaImpuestoCompuesto, out respuesta);
            if (respuesta.Valida)
            {
                Telemetria.Instancia.AgregaMetrica(tiempoConsultarPrecio.Para().AgregarPropiedad("Encontrado", true).AgregarPropiedad("Codigo", eArticulo.CodigoImpresion).AgregarPropiedad("Descripcion", eArticulo.DescripcionLarga).AgregarPropiedad("Impuesto", eArticulo.Impuesto1).AgregarPropiedad("Valor", eArticulo.PrecioVenta1));

                log.DebugFormat("[CmdConsultarPrecio] Articulo encontrado: {0}, Transaccion: {1}, Factura {2}", eArticulo.ToString(), (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                Procesar(eArticulo);
                log.DebugFormat("[CmdConsultarPrecio] {0}", this.ToString());
            }
            else
            {
                Telemetria.Instancia.AgregaMetrica(tiempoConsultarPrecio.Para().AgregarPropiedad("Encontrado", false).AgregarPropiedad("Codigo", this.CodigoArticulo).AgregarPropiedad("Mensaje", respuesta.Mensaje));

                iu.PanelVentas.VisorMensaje = respuesta.Mensaje;
                iu.PanelVentas.VisorEntrada = string.Empty;

                // Emitir sonido
                Utilidades.EmitirAlerta();
            }

            SolicitudPanelVenta solicitudOperacion = new SolicitudPanelVenta(Enums.Solicitud.Vender);
            Reactor.Instancia.Procesar(solicitudOperacion);

        }


        private void Procesar(EArticulo articulo)
        {
            //
            iu.PanelVentas.VisorCliente.Descripcion = articulo.DescripcionLarga;
            iu.PanelVentas.VisorCliente.ValorItem = articulo.PrecioVenta1;

            //
            iu.PanelVentas.VisorMensaje = string.Empty;
            iu.PanelVentas.VisorEntrada = string.Empty;

            ///
            iu.MostrarDisplayCliente(Enums.DisplayCliente.ConsultarPrecio);
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }

        #endregion
    }
}
