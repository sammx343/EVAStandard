using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ETirilla
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<EItemVenta> tirilla = new List<EItemVenta>();

        public decimal TotalVenta { get; private set; }

        // Agrega un item a la venta, incrementa valor total.
        // retorna true/false segun resultado de operación.
        //
        // Validaciones:
        // Si ingresamos item valor - es anulacion, pero en tal caso, valida que el item
        // exista, y con suficiente cantidad (en tal caso como manejar las facturas de anulación?).
        //public EItemVenta AgregarItem(EArticulo articulo, decimal cantidad)
        //{
        //    decimal subtotal = cantidad * articulo.PrecioVenta1;
        //    EItemVenta item = new EItemVenta(
        //        articulo.Id,
        //        articulo.Descripcion,
        //        articulo.PrecioVenta1,
        //        cantidad,
        //        subtotal);

        //    tirilla.Add(item);

        //    TotalVenta += subtotal;

        //    log.Info("ETirilla, agregado " + item);

        //    return item.Copia();
        //}

        public void LimpiarVentaFinalizada()
        {
            tirilla.Clear();
            TotalVenta = 0;
        }
    }
}
