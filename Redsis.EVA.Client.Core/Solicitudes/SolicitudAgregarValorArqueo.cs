using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Solicitudes
{
    public class SolicitudAgregarValorArqueo : ISolicitud
    {
        public string CodigoMedioPago { get; set; }

        public decimal ValorMedioPago { get; set; }

        public Solicitud TipoSolicitud
        {
            get;

            set;
        }

        public SolicitudAgregarValorArqueo(string codigoMedioPago, decimal valorMedioPago, Solicitud solicitud)
        {
            CodigoMedioPago = codigoMedioPago;
            ValorMedioPago = valorMedioPago;
            TipoSolicitud = solicitud;
        }
    }
}
