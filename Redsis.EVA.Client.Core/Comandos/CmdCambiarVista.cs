using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;

namespace Redsis.EVA.Client.Core.Comandos
{
    internal class CmdCambiarVista : ComandoAbstract
    {
        Solicitudes.SolicitudVista solicitud;

        public CmdCambiarVista(ISolicitud solicitud) : base(solicitud)
        {
            this.solicitud = solicitud as Solicitudes.SolicitudVista;
        }

        public override void Ejecutar()
        {
            log.Info("[Ejecutar] " + this);

            //
            switch (solicitud.Panel)
            {
                case Enums.Paneles.PanelPago:
                    iu.MostrarPanelPagos();
                    iu.PanelPago.VisorOperador = "Ingrese Valor";
                    iu.PanelPago.VisorCliente.Total = iu.PanelVentas.VisorCliente.Total;
                    iu.PanelPago.VisorCliente.Items = iu.PanelVentas.Tirilla.Count;                    
                    break;

                case Enums.Paneles.PanelTirilla:
                    iu.MostrarTirilla();
                    break;

                case Enums.Paneles.PanelVenta:
                    iu.MostrarPanelVenta();
                    break;

                case Enums.Paneles.PanelEspera:
                    iu.MostrarPanelTerminalAsegurada();
                    break;

                case Enums.Paneles.PanelInicioSesion:
                    iu.MostrarPanelInicioSesion();
                    break;                
                case Enums.Paneles.PanelPagoTouch:

                    break;
                default:
                    break;
            }
        }
    }
}
