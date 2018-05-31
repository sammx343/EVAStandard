using System;
using System.Collections.Generic;
using System.Text;

namespace PRUEBA1
{
    static class Valida
    {
        public static T ArgumentoNoNulo<T>(T argumento, string descripcion)
           where T : class
        {
            if (argumento == null)
                throw new ArgumentNullException(descripcion);

            return argumento;
        }

        public static void TextoNoNuloVacio(string texto, string descripcion)
        {
            if (string.IsNullOrEmpty(texto))
                throw new ArgumentException(
                    string.Format("{0} {1} nula o vacía.",
                    descripcion, texto));
        }
    }
}
