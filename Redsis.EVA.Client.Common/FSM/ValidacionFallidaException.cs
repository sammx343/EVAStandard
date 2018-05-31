using System;
using System.Collections.Generic;
using System.Text;

namespace PRUEBA1.FSM
{
    public class CondicionFallidaException : Exception
    {
        public CondicionFallidaException(string message) : base(message)
        {
        }
    }
}
