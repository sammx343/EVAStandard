using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelPrestamos : IPanelActivo
    {
        /// <summary>
        /// 
        /// </summary>
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
        string TipoPrestamo { get; set; }

        /// <summary>
        /// Valor ingresado a la recogida.
        /// </summary>
        string ValorRecogida { get; set; }

        /// <summary>
        /// Mensaje entregado al operador.
        /// </summary>
        string MensajeOperador { get; set; }

        /// <summary>
        /// Limpia la operación de préstamo finalizada.
        /// </summary>
        void LimpiarPrestamoFinalizado();

    }
}
