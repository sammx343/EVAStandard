using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;

namespace Redsis.EVA.Client.Core.Comandos
{
    public class CmdPantallaCliente : ComandoAbstract
    {
        public CmdPantallaCliente(ISolicitud solicitud) : base(solicitud)
        {

        }

        public override void Ejecutar()
        {
            iu.MostrarPantallaCliente();
        }

        public override string ToString()
        {
            string ans = "";

            if (this != null)
                ans = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);

            return ans;
        }
    }
}
