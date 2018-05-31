using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelCierreDatafono: IPanelActivo
    {
        string VisorOperador { get; set; }

        string VisorMensaje { get; set; }

        string VisorEntrada { get; set; }

        void LimpiarOperacionFinalizada();

        void CierreIntegrado();
    }
}
