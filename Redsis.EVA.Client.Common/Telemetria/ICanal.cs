using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Common.Telemetria
{
    public interface ICanal
    {
        long NumeroConexiones { get; }
        long NumeroExcepciones { get; }
        long MaxLatenciaEnMs { get; }
        string CustomerId { get; }

        void Enviar(string log, string msj);
    }
}
