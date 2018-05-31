using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Common
{
    /// <summary>
    /// Indica solicitud no aceptada por estado actual de la máquina de estados.
    /// </summary>
    public class EvaSolicitudNoPermitidaException : EvaApplicationException
     {
        public EvaSolicitudNoPermitidaException() : base("Solicitud no permita en el estado actual.")
        {

        }
    }
}
