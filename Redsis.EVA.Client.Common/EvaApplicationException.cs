using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Common
{
    public class EvaApplicationException : Exception
    {
        public int Codigo { get; } = 0;

        public bool EnBitacora { get; set; } = false;

        public EvaApplicationException()
        {
        }

        public EvaApplicationException(string mensaje) : base(mensaje)
        {
        }

        public EvaApplicationException(Exception inner) : base("", inner)
        {

        }

        public EvaApplicationException(string mensaje, Exception inner) : base(mensaje, inner)
        {
        }

        public EvaApplicationException(int codigo, string mensaje) : base(mensaje)
        {
            Codigo = codigo;
        }

        public EvaApplicationException(int codigo, string mensaje, Exception inner) : base(mensaje, inner)
        {
            Codigo = codigo;
        }
    }
}
