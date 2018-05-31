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
using System.Globalization;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdAgregarPrestamo : ComandoAbstract
    {
        decimal valor;
        Solicitudes.SolicitudAgregarPrestamo solicitud;

        public CmdAgregarPrestamo(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudAgregarPrestamo;
        }

        public override void Ejecutar()
        {
            try
            {
                //Valor de la pantalla
                if (string.IsNullOrEmpty(solicitud.ValorEntrada))
                    throw new Exception("El valor no puede ser vacío o igual a cero.");

                bool porDenominacion = solicitud.ValorEntrada.Contains('*');
                int cantidad = 0;
                double valorPrestamo = 0;

                if (porDenominacion)
                {
                    cantidad = Convert.ToInt32(solicitud.ValorEntrada.Split('*')[0]);
                    string valorDenominacion = solicitud.ValorEntrada.Split('*')[1];

                    if (string.IsNullOrEmpty(valorDenominacion) || Double.Parse(valorDenominacion) <= 0)
                    {
                        throw new Exception("El valor no puede ser vacío o igual a cero.");
                    }
                    else
                    {
                        if (cantidad == 0)
                            throw new Exception("El valor no puede ser vacío o igual a cero.");

                        valorPrestamo = Convert.ToDouble(valorDenominacion) * cantidad;
                    }

                }
                else
                {
                    //|| Double.Parse(solicitud.ValorEntrada, System.Globalization.NumberStyles.Currency) <= 0)
                    if (string.IsNullOrEmpty(solicitud.ValorEntrada))
                    {
                        throw new Exception("El valor no puede ser vacío o igual a cero.");
                    }
                    else
                    {
                        valorPrestamo = solicitud.ValorEntrada.ConvertToDoubleFromCustomCurrency();
                        if(valorPrestamo <= 0)
                        {
                            throw new Exception("El valor del prestamo no es válido.");
                        }
                    }
                }

                if (Entorno.Instancia.Prestamo == null)
                {
                    Entorno.Instancia.Prestamo = new EPrestamo(valor);

                    List<decimal> listPrestamos = new List<decimal>();
                    Entorno.Instancia.Prestamo.ListPrestamos = listPrestamos;

                }

                //
                valor = Convert.ToDecimal(valorPrestamo);
                log.Info("[CmdAgregarPrestamo] Agregar valor al prestamo : " + valorPrestamo);

                Entorno.Instancia.Prestamo.ListPrestamos.Add(valor);
                if (Config.ViewMode == InternalSettings.ModoTouch)
                    Entorno.Instancia.Prestamo.SumarAlPrestamo(valor);

                decimal totalRecogidas = Entorno.Instancia.Prestamo.ListPrestamos.Sum();

                if (Config.ViewMode == InternalSettings.ModoConsola)
                {
                    iu.PanelPrestamos.VisorEntrada = string.Empty;
                    iu.PanelPrestamos.VisorMensaje = string.Format("Valor Agregado: {0} [{1}]", valor.ToCustomCurrencyFormat(useThousandsLimit: false), totalRecogidas.ToCustomCurrencyFormat(useThousandsLimit: false));
                }
                else if (Config.ViewMode == InternalSettings.ModoTouch)
                {
                    //terminar prestamo.
                    Solicitudes.SolicitudTerminarPrestamo terminarPrestamo = new Solicitudes.SolicitudTerminarPrestamo(Solicitud.TerminarPrestamo);
                    Reactor.Instancia.Procesar(terminarPrestamo);
                }

                log.Info($"[CmdAgregarPrestamo] Agregar Prestamo: {valor.ToCustomCurrencyFormat(useThousandsLimit: false)} - Total: {totalRecogidas.ToCustomCurrencyFormat(useThousandsLimit: false)}");

                //
            }
            catch (Exception ex)
            {
                log.WarnFormat("[CmdAgregarPrestamo] {0}", ex.Message);
                iu.PanelOperador.MensajeOperador = ex.Message;
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
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
