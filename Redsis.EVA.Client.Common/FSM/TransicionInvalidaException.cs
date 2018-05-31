using System;
using System.Collections.Generic;
using System.Text;

namespace PRUEBA1.FSM
{
    public class TransicionInvalidaException : Exception
    {
        public TransicionInvalidaException(string message) : base(message)
        {
        }
    }
}
