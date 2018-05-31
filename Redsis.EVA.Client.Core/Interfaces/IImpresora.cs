using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IImpresora
    {
        string TextoImprimir { get; set; }
        string Marca { get; set; }
        bool ImpideOperacion { get; set; }

        //PrinterState EstadoImpresora { get; }
        //CashDrawer CajonMonedero { get; set; }

        Respuesta Configurar();
        Respuesta Imprimir(string texto, bool cortarPapel, bool abrirCajon);
        Respuesta Imprimir();
        Respuesta Reiniciar();
        Respuesta ValidarEstado();

        void AbrirCajon();
    }
}
