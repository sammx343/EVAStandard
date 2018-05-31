using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelCliente : IPanelActivo
    {

        /// <summary>
        /// 
        /// </summary>
        string CodigoCliente { get; set; }

        /// <summary>
        /// 
        /// </summary>
        List<ECliente> ListClientes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        string MensajeOperador { get; set; }
    }
}
