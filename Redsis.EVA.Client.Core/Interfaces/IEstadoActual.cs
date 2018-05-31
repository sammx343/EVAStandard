using EvaPOS.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IEstadoActual
    {
        string EstadoActual { get; set; }
        string EstadoValido { get; set; }
        EstadosFSM EstadoFSM { get; set; }
    }
}
