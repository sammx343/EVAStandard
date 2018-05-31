using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Helpers;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarRecogida : ComandoAbstract
    {
        Solicitudes.SolicitudAgregarRecogida solicitud;
        public string codigoRecogida { get; private set; }

        public CmdAgregarRecogida(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudAgregarRecogida;

        }

        public override void Ejecutar()
        {

            try
            {
                if (Config.ViewMode == InternalSettings.ModoTouch)
                {
                    if (string.IsNullOrEmpty(Entorno.Instancia.Vista.ModalRecogidas.CodigoRecogida))
                        return;
                }

                //
                AgregarRecogida();
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
                iu.PanelOperador.MensajeOperador = ex.Message;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        private void AgregarRecogida()
        {
            if (Entorno.Instancia.Recogida == null)
            {
                // código de la recogida
                string codigo = string.Empty;
                if (Entorno.Instancia.Vista.ModalRecogidas != null)
                    codigo = Entorno.Instancia.Vista.ModalRecogidas.CodigoRecogida;
                else
                    codigo = Entorno.Instancia.Vista.PanelRecogidas.CodigoRecogida;

                ECodigoRecogida eCodigo = Entorno.Instancia.CodigosRecogida.CodigoRecogida(codigo);

                //
                List<decimal> listRecogidas = new List<decimal>();
                Entorno.Instancia.Recogida = new Entidades.ERecogida(eCodigo, listRecogidas);
                Entorno.Instancia.Recogida.listRecogidas = new List<decimal>();
            }

            if (!Entorno.Instancia.Recogida.EstaAbierta)
                Entorno.Instancia.Recogida.EstaAbierta = true;

            //            
            bool porDenominacion = solicitud.ValorRecogida.Contains('*');
            int cantidad = 0;

            double valorRecogida = 0;
            if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                if (string.IsNullOrEmpty(solicitud.ValorRecogida))
                {
                    throw new Exception("El valor no puede ser vacío o igual a cero.");
                }
                else
                {
                    valorRecogida = Extensions.UnformatCurrencyValue(solicitud.ValorRecogida);

                    //
                    if (valorRecogida <= 0)
                        throw new Exception("El valor no puede ser vacío o igual a cero.");
                }
            }
            else
            {

                if (porDenominacion)
                {
                    cantidad = Convert.ToInt32(solicitud.ValorRecogida.Split('*')[0]);
                    string valorDenominacion = solicitud.ValorRecogida.Split('*')[1];

                    if (string.IsNullOrEmpty(valorDenominacion) || Double.Parse(valorDenominacion) <= 0)
                    {
                        throw new Exception("El valor no puede ser vacío o igual a cero.");
                    }
                    else
                    {
                        if (cantidad == 0)
                            throw new Exception("El valor no puede ser vacío o igual a cero.");

                        valorRecogida = Convert.ToDouble(valorDenominacion) * cantidad;
                    }

                }
                else
                {
                    if (string.IsNullOrEmpty(solicitud.ValorRecogida) || Double.Parse(solicitud.ValorRecogida) <= 0)
                    {
                        throw new Exception("El valor no puede ser vacío o igual a cero.");
                    }
                    else
                    {
                        valorRecogida = Convert.ToDouble(solicitud.ValorRecogida);
                    }
                }
            }

            log.Info("[CmdAgregarRecogida] Guardando recogida ...");

            //Valor de la pantalla
            decimal valor = 0;
            valor = Convert.ToDecimal(valorRecogida);
            Entorno.Instancia.Recogida.listRecogidas.Add(valor);

            if (Config.ViewMode == InternalSettings.ModoTouch)
                Entorno.Instancia.Recogida.AgregarValor(valor);

            decimal totalRecogidas = Entorno.Instancia.Recogida.listRecogidas.Sum();

            if (Entorno.Instancia.Vista.PanelRecogidas != null)
            {
                Entorno.Instancia.Vista.PanelRecogidas.VisorEntrada = string.Empty;
                iu.PanelRecogidas.VisorEntrada = string.Empty;
                iu.PanelRecogidas.VisorMensaje = string.Format("Valor Agregado: {0} [{1}]", valor.ToCustomCurrencyFormat(useThousandsLimit: false), totalRecogidas.ToCustomCurrencyFormat(useThousandsLimit: false));
            }

            log.Info($"[CmdAgregarRecogida] Agregar Recogida: {valor.ToString("C")} - Total: {totalRecogidas.ToString("C")}");

            //Terminar recogida touch
            if (Config.ViewMode == InternalSettings.ModoTouch)
            {
                Solicitudes.SolicitudTerminarRecogida terminarRecogida = new Solicitudes.SolicitudTerminarRecogida(Solicitud.TerminarRecogida);
                Reactor.Instancia.Procesar(terminarRecogida);
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
