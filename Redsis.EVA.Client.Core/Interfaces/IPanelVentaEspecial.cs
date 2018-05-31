using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelVentaEspecial : IPanelActivo
    {
        string VisorOperador { get; set; }

        /// <summary>
        /// Texto de mensajes al operador
        /// </summary>
        string VisorMensaje { get; set; }

        /// <summary>
        /// Texto de entrada 
        /// </summary>
        string VisorEntrada { get; set; }

        /// <summary>
        /// Código de la recogida.
        /// </summary>
        string CodigoVenta { get; set; }

        /// <summary>
        /// Descripción de la Recogida
        /// </summary>
        string DescripcionVenta { get; set; }

        /// <summary>
        /// Lista de Recogidas disponibles.
        /// </summary>
        List<EVentaEspecial> ListVentasEspeciales { get; set; }

        /// <summary>
        /// Mensaje entregado al operador.
        /// </summary>
        string MensajeOperador { get; set; }

        /// <summary>
        /// 
        /// </summary>
        List<ECliente> ListClientes { get; set; }

        void LimpiarOperacion();
    }
}
