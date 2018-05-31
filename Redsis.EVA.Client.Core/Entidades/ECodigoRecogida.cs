using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ECodigoRecogida
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public ECodigoRecogida()
        {
            Codigo = "";
            Descripcion = "";
        }

        public ECodigoRecogida(string codigo, string descripcion)
        {
            Codigo = codigo;
            Descripcion = descripcion;
        }
    }
}
