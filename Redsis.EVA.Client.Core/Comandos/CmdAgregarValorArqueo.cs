using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Common.Telemetria;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdAgregarValorArqueo : ComandoAbstract
    {
        Solicitudes.SolicitudAgregarValorArqueo Solicitud;

        public CmdAgregarValorArqueo(ISolicitud solicitud) : base(solicitud)
        {
            Solicitud = solicitud as Solicitudes.SolicitudAgregarValorArqueo;
        }

        public override void Ejecutar()
        {
            Respuesta respuesta = new Respuesta();
            respuesta = new Respuesta(false);
            EMedioPago medioPago = new PMediosPago().GetAllMediosPago().MedioPago(Solicitud.CodigoMedioPago);
            decimal valor = Solicitud.ValorMedioPago;

            //
            Entorno.Instancia.Vista.PanelArqueo.Caja.AgregarValor(medioPago, valor, out respuesta);

            //
            log.Info("[CmdAgregarValorArqueo] Valor de arqueo de medio de pago agregado. Valor: " + valor + " Medio de Pago: " + medioPago.Tipo);

            Telemetria.Instancia.AgregaMetrica(new Evento("AgregarValorArqueo").AgregarPropiedad("Transaccion", (Entorno.Instancia.Terminal.NumeroUltimaTransaccion + 1)).AgregarPropiedad("Valor",valor).AgregarPropiedad("MedioPago",medioPago.Tipo));


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
