using System;
using System.Collections.Generic;
using System.Text;

namespace PRUEBA1.FSM
{
    public class EstadoNoDefinidoException : Exception
    {
        public EstadoNoDefinidoException()
        {
        }

        public EstadoNoDefinidoException(string message) : base(message)
        {
        }
    }
}
