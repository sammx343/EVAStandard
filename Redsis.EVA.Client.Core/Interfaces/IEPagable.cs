using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    /// <summary>
    /// Implementada por transacciones que admiten medios de pago.
    /// </summary>
    public interface IEPagable
    {
        /// <summary>
        /// true si se requiere usar procesos de pago. 
        /// Usado para indicar si hace uso del Panel de Pagos.
        /// </summary>
        bool EsPagable { get; }

        /// <summary>
        /// true si ele estado de la clase esta listo para invocar medios de pago. 
        /// Usado para indicar si se puede pasar a Panel de Pagos.
        /// </summary>
        bool EsPermitidoPagar { get; }
    }
}
