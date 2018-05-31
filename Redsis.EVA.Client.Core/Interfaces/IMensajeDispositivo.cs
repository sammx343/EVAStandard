using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IMensajeDispositivo<T>
    {
        string TituloMensaje { get; set; }

        string TextoMensaje { get; set; }

        string TextConfirmar { get; set; }

        string TextCancelar { get; set; }

        bool IsIndeterminate { get; set; }

        int PorcentajeProgreso { get; set; }

        void OcultarMensaje();

        T MostrarMensaje();

        T MostrarMensaje(string titulo, string mensaje);

        Task<T> MostrarMensajeAsync(string titulo, string mensaje);
    }
}
