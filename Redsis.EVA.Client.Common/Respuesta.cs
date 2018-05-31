using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Common
{
    public class Respuesta
    {
        public bool Valida { get; set; } = false;
        public string Mensaje { get; set; } = string.Empty;

        public Respuesta()
        {

        }

        public Respuesta(bool valida)
        {
            this.Valida = valida;
            this.Mensaje = string.Empty;
        }

        public Respuesta(bool valida, string mensaje)
        {
            this.Valida = valida;
            this.Mensaje = mensaje;
        }

        public void Documentar(bool valida, string mensaje)
        {
            this.Valida = valida;
            this.Mensaje = mensaje;
        }

        public override string ToString()
        {
            string ans = "";
            if (this != null)
            {
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);
            }

            return ans;
        }
    }
}
