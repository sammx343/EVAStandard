using System;
using System.Collections.Generic;
using System.Text;

namespace PRUEBA1.FSM
{
    public class TransicionNoDefinidaException : Exception
    {
        public TransicionNoDefinidaException(string message) : base(message)
        {
        }
    }
}
