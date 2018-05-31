using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelArqueo
    {
        ECaja Caja { get; set; }

        void CargarCaja();
    }
}
