using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Common;

namespace Redsis.EVA.Client.Core.Comandos
{
    class CmdListarRecogidas : ComandoAbstract
    {
        public CmdListarRecogidas(ISolicitud solicitud) : base(solicitud)
        {
        }

        public override void Ejecutar()
        {
            Respuesta respuesta;
            new PRecogida().GetCodigosRecogida(out respuesta);
            throw new NotImplementedException();
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
