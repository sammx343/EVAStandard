using Redsis.EVA.Client.Core.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelRecogidas : IPanelActivo
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
        string CodigoRecogida { get; set; }

        /// <summary>
        /// Descripción de la Recogida
        /// </summary>
        string DescripcionRecogida { get; set; }

        /// <summary>
        /// Lista de Recogidas disponibles.
        /// </summary>
        List<ECodigoRecogida> ListRecogidas { get; set; }

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
