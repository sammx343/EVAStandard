using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Solicitudes;
using Redsis.EVA.Client.Common;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPagarVentaDatafono : ComandoAbstract
    {
        Solicitudes.SolicitudPagoDatafono Solicitud;

        public CmdPagarVentaDatafono(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudPagoDatafono;
        }

        public override void Ejecutar()
        {
            try
            {
                if ((Solicitud.TipoSolicitud == Enums.Solicitud.PagoDatafono)
                       | (Solicitud.TipoSolicitud == Enums.Solicitud.ReintentarPago))
                {
                    bool obligaIngresarValor = Entorno.Instancia.Parametros.ObtenerValorParametro<bool>("pdv.pago.obliga_ingresar_valor");
                    if (string.IsNullOrEmpty(iu.PanelPago.VisorEntrada))
                    {
                        if (obligaIngresarValor)
                        {
                            log.Warn("[CmdPagarVentaDatafono.Ejecutar]  Debe ingrear un valor válido");
                            throw new ArgumentException("Debe ingresar un valor válido");
                        }
                    }
                    else
                    {
                        decimal valorPago = 0;
                        if (!decimal.TryParse(iu.PanelPago.VisorEntrada, out valorPago))
                        {
                            log.WarnFormat("[CmdPagarVentaDatafono.Ejecutar]  Valor ingresado inválido    [{0}]", iu.PanelPago.VisorEntrada);
                            throw new ArgumentException("Valor ingresado inválido");
                        }
                        else
                        {
                            if (valorPago <= 0)
                            {
                                log.WarnFormat("[CmdPagarVentaDatafono.Ejecutar]  Monto inválido    [{0}]", iu.PanelPago.VisorEntrada);
                                throw new ArgumentException("Monto no válido");
                            }
                        }
                    }

                    //
                    iu.PanelPago.PagoVentaDatafono();
                }
                else if (Solicitud.TipoSolicitud == Enums.Solicitud.PagoDatafonoManual)
                {
                    if (Config.ViewMode == InternalSettings.ModoConsola)
                    {
                        iu.PanelPagoManual.VisorMensaje = "Ingrese número de comprobante.";
                    }
                    else
                    {
                        iu.MostrarModalPanelPagoManual();
                    }
                }
            }
            catch (ArgumentException argEx)
            {
                iu.PanelPago.VisorMensaje = argEx.Message;

                if(Config.ViewMode == InternalSettings.ModoTouch)
                {
                    iu.PanelPago.VisorOperador = argEx.Message;
                }


                //
                SolicitudPanelPago volver = new SolicitudPanelPago(Enums.Solicitud.Pagar, argEx.Message);
                Reactor.Instancia.Procesar(volver);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[Ejecutar] {0}", ex.Message);

                //
                iu.PanelPago.VisorMensaje = "Ocurrió un problema al intentar pagar";

                //
                SolicitudVolver volver = new SolicitudVolver(Enums.Solicitud.Pagar);
                Reactor.Instancia.Procesar(volver);
            }
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
