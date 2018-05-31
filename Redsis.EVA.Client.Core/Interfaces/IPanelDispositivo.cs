using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelDispositivo : IPanelActivo
    {
        //bool ContinuarAplicacion { get; set; }

        EstadosComprobacionDispositivos EstadoComprobacion { get; set; }

        string TituloDispositivos { get; set; }

        string MensajeDispositivos { get; set; }

        void MostrarMensaje(string text);

        void MostrarTitulo(string text);

        List<IDetalleDispositivo> ListaDispositivos { get; set; }

        void RegistrarDispositivos(out Respuesta respuesta);

        void ComprobarEstadoDispositivos(out Respuesta respuesta);

        void AgregarDispositivo(IDetalleDispositivo detalleDispositivo);

        void ReiniciarAplicacion();

        Task<MessageResult> ConfirmarActualizacionTask(string mensaje);

        Task<MessageResult> VerificarActualizacion();

    }
}
