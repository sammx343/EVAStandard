using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Solicitudes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdAgregarClienteVentaEspecialSinMedioPago : ComandoAbstract
    {
        private SolicitudAgregarCliente solicitud;
        public string CodigoCliente { get; private set; }

        public CmdAgregarClienteVentaEspecialSinMedioPago(ISolicitud Solicitud) : base(Solicitud)
        {
            this.solicitud = Solicitud as SolicitudAgregarCliente;
            CodigoCliente = solicitud.ValorEntrada;
        }

        public override void Ejecutar()
        {
            log.Info("[CmdAgregarClienteVentaespecialSinMedioPago] Agregar cliente");
            Respuesta res = new Respuesta();

            if (!string.IsNullOrEmpty(CodigoCliente))
            {
                ECliente cliente = Entorno.Instancia.Clientes.Cliente(CodigoCliente, out res);
                if (res.Valida)
                {
                    Entorno.Instancia.VentaEspecialSinMedioPago.AgregarCliente(cliente);
                    iu.PanelOperador.CodigoCliente = cliente.Codigo;

                    log.InfoFormat("[CmdAgregarClienteVentaEspecialSinMedioPago] Cliente Agregado: {0}  Transaccion: {1}, Factura {2}", cliente.Codigo + " " + cliente.PrimerNombre + " " + cliente.SegundoApellido, (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1), (Entorno.Instancia.Terminal.NumeroUltimaFactura + 1));

                    Telemetria.Instancia.AgregaMetrica(new Evento("ClienteAgregadoVentaEspecial").AgregarPropiedad("Cliente", cliente.Id).AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)));
                }
                else
                {
                    iu.PanelVentas.VisorMensaje = "Cliente no encontrado.";
                }
            }
        }
    }
}
