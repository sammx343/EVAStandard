using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Solicitudes;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPanelCierreDatafono : ComandoAbstract
    {
        Solicitudes.SolicitudPanelCierreDatafono Solicitud;

        public CmdPanelCierreDatafono(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudPanelCierreDatafono;
        }

        public override void Ejecutar()
        {
            if (InternalSettings.EntidadDatafono != Common.Enums.EntidadFinanciera.CREDIBANCO)
            {
                iu.PanelVentas.VisorMensaje = "El datafono no permite la operación solicitada.";

                //
                SolicitudVolver volver = new SolicitudVolver(Enums.Solicitud.Volver);
                Reactor.Instancia.Procesar(volver);
            }
            else
            {
                //
                log.Info("[CmdCierreDatafono.Ejecutar]");

                if (Config.ViewMode == InternalSettings.ModoConsola)
                {
                    iu.MostrarPanelCierreDatafono();
                    iu.PanelCierreDatafono.VisorOperador = "Cierre de Datafono";
                    iu.PanelCierreDatafono.VisorMensaje = "Confirmar cierre ?  [1 = Sí, 2 = No]";
                }
                else if(Config.ViewMode == InternalSettings.ModoTouch)
                {
                    iu.MostrarModalCierreDatafono();
                }
                else
                {
                    log.Error("[CmdCierreDatafono.Ejecutar] Modo visual no admitido");
                    iu.PanelOperador.MensajeOperador = "Modo visual no admitido";
                }
            }
        }
    }
}
