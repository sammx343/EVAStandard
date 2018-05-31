using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelAjuste
    {
        /// <summary>
        /// Código de la recogida.
        /// </summary>
        string CodigoAjuste { get; set; }

        /// <summary>
        /// Descripción de la Recogida
        /// </summary>
        string DescripcionAjuste { get; set; }

        /// <summary>
        /// Lista de Recogidas disponibles.
        /// </summary>
        List<ETipoAjuste> ListTiposAjuste { get; set; }

        /// <summary>
        /// Mensaje entregado al operador.
        /// </summary>
        string MensajeOperador { get; set; }
    }
}
