using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IDetalleDispositivo
    {
        string NombreDispositivo { get; set; }
        string MensajeDispositivo { get; set; }
        bool EstadoDispositivo { get; set; }

        bool EsVerificable { get; set; }

        void ComprobarEstadoDispositivo(out Respuesta respuesta);
    }
}
