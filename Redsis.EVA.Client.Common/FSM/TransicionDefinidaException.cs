using System;
using System.Collections.Generic;
using System.Text;

namespace PRUEBA1.FSM
{
    public class TransicionDefinidaException : Exception
    {
        public TransicionDefinidaException() : base() { }

        public TransicionDefinidaException(string txt) : base(txt) { }

    }
}
