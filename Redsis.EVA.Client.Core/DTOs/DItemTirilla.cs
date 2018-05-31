using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Redsis.EVA.Client.Core.DTOs
{
    public class DItemTirilla : IItemTirillaIU, INotifyPropertyChanged
    {
        private string codigo;
        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        private string descripcion;
        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; }
        }

        public decimal PrecioVentaUnidad { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Subtotal { get; set; }

        public DItemTirilla()
        {

        }

        public DItemTirilla(
            string codigo,
            string descripcion,
            decimal precioVentaUnd,
            decimal cantidad,
            decimal subtotal)
        {
            Codigo = codigo;
            Descripcion = descripcion;
            PrecioVentaUnidad = precioVentaUnd;
            Cantidad = cantidad;
            Subtotal = subtotal;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
