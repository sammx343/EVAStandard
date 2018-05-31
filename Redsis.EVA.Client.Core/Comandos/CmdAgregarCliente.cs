using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Solicitudes;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarCliente : ComandoAbstract
    {
        private SolicitudAgregarCliente solicitud;
        public string CodigoCliente { get; private set; }

        public CmdAgregarCliente(ISolicitud Solicitud) : base(Solicitud)
        {
            this.solicitud = Solicitud as SolicitudAgregarCliente;
            CodigoCliente = solicitud.ValorEntrada;
        }

        public override void Ejecutar()
        {
            log.Info("[CmdAgregarCliente] Agregar cliente");
            Respuesta res = new Respuesta();

            if (!string.IsNullOrEmpty(CodigoCliente))
            {
                if (Entorno.Instancia.Venta.EstaAbierta)
                {
                    if (Entorno.Instancia.Venta.Cliente != null)
                    {
                        log.InfoFormat("[CmdAgregarCliente] Cliente Eliminado: {0}  Transaccion: {1}, Factura {2}", Entorno.Instancia.Venta.Cliente.Codigo + " " + Entorno.Instancia.Venta.Cliente.PrimerNombre + " " + Entorno.Instancia.Venta.Cliente.SegundoApellido, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                        Telemetria.Instancia.AgregaMetrica(new Evento("ClienteEliminado").AgregarPropiedad("Cliente", Entorno.Instancia.Venta.Cliente.Id).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));

                        Entorno.Instancia.Venta.EliminarCliente();
                        iu.PanelOperador.CodigoCliente = "";
                    }
                    else
                    {
                        ECliente cliente = Entorno.Instancia.Clientes.Cliente(CodigoCliente, out res);
                        if (res.Valida)
                        {
                            Entorno.Instancia.Venta.AgregarCliente(cliente);
                            iu.PanelOperador.CodigoCliente = (cliente.Codigo + " " + cliente.PrimerNombre + " " + cliente.SegundoApellido);

                            log.InfoFormat("[CmdAgregarCliente] Cliente Agregado: {0}  Transaccion: {1}, Factura {2}", cliente.Codigo + " " + cliente.PrimerNombre + " " + cliente.SegundoApellido, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                            Telemetria.Instancia.AgregaMetrica(new Evento("ClienteAgregado").AgregarPropiedad("Cliente", cliente.Id).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));
                        }
                        else
                        {
                            log.Warn("[CmdAgregarCliente] Cliente no encontrado.");
                            iu.PanelVentas.VisorMensaje = "Cliente no encontrado.";
                        }

                    }
                }
                else if (Entorno.Instancia.Devolucion != null)
                {
                    if (Entorno.Instancia.Devolucion.EstaAbierta)
                    {
                        if (Entorno.Instancia.Devolucion.Cliente != null)
                        {
                            log.InfoFormat("[CmdAgregarCliente] Cliente Eliminado: {0}  Transaccion: {1}, Factura {2}", Entorno.Instancia.Devolucion.Cliente.Codigo + " " + Entorno.Instancia.Devolucion.Cliente.PrimerNombre + " " + Entorno.Instancia.Devolucion.Cliente.SegundoApellido, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                            Telemetria.Instancia.AgregaMetrica(new Evento("ClienteEliminado").AgregarPropiedad("Cliente", Entorno.Instancia.Devolucion.Cliente.Id).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));

                            Entorno.Instancia.Devolucion.EliminarCliente();
                            iu.PanelOperador.CodigoCliente = "";

                        }
                        else
                        {
                            ECliente cliente = Entorno.Instancia.Clientes.Cliente(CodigoCliente, out res);
                            if (res.Valida)
                            {
                                Entorno.Instancia.Devolucion.AgregarCliente(cliente);
                                iu.PanelOperador.CodigoCliente = cliente.Codigo;

                                log.InfoFormat("[CmdAgregarCliente] Cliente Agregado: {0}  Transaccion: {1}, Factura {2}", cliente.Codigo + " " + cliente.PrimerNombre + " " + cliente.SegundoApellido, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                                Telemetria.Instancia.AgregaMetrica(new Evento("ClienteAgregado").AgregarPropiedad("Cliente", cliente.Id).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));
                            }
                            else
                            {
                                log.Warn("[CmdAgregarCliente] Cliente no encontrado.");
                                iu.PanelVentas.VisorMensaje = "Cliente no encontrado.";
                            }
                        }
                    }
                    else
                    {
                        log.Warn("[CmdAgregarCliente] No hay transaccion abierta.");
                        iu.PanelVentas.VisorMensaje = "No hay transaccion abierta.";
                    }
                }
                else
                {
                    log.Warn("[CmdAgregarCliente] No hay transaccion abierta.");
                    iu.PanelVentas.VisorMensaje = "No hay transaccion abierta.";
                }
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
