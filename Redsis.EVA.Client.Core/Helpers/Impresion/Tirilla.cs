using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Helpers.Impresion
{
    class Tirilla
    {
        #region POS Characters

        public char EscCut
        {
            get
            {
                return '\u001A';
            }
        }

        public char EscCut2
        {
            get
            {
                return '\u0019';
            }
        }

        public char Esc
        {
            get
            {
                return '\u001b';
            }
        }

        public char FS
        {
            get
            {
                return '\u001c';
            }
        }

        public char ASCII0
        {
            get
            {
                return '\u0000';
            }
        }

        public char ASCII1
        {
            get
            {
                return '\u0001';
            }
        }

        public char C45
        {
            get
            {
                return '\u0045';
            }
        }

        public char C14
        {
            get
            {
                return '\u0014';
            }
        }

        public char C4
        {
            get
            {
                return '\u0009';
            }
        }

        public char CImp1
        {
            get
            {
                return '\u0018';
            }
        }

        public char CImp2
        {
            get
            {
                return '\u0009';
            }
        }

        public char CImp3
        {
            get
            {
                return '\u0009';
            }
        }

        public char C24
        {
            get
            {
                return '\u001D';
            }
        }

        public char C27
        {
            get
            {
                return '\u0025';
            }
        }

        public char C61
        {
            get
            {
                return '\u0061';
            }
        }

        public char C50
        {
            get
            {
                return '\u0050';
            }
        }

        public char C10
        {
            get
            {
                return '\u0010';
            }
        }

        public char C2
        {
            get
            {
                return '\u0002';
            }
        }


        public char C21
        {
            get
            {
                return '\u0021';
            }
        }

        public char C44
        {
            get
            {
                return '\u0044';
            }
        }

        public char C27C
        {
            get
            {
                return '\u0027';
            }
        }

        public char C20
        {
            get
            {
                return '\u0020';
            }
        }

        public char C24C
        {
            get
            {
                return '\u0024';
            }
        }

        public char C1D
        {
            get
            {
                return '\u001d';
            }
        }
        public char C5O
        {
            get
            {
                return '\u0050';
            }
        }

        public char C9
        {
            get
            {
                return '\u0009';
            }
        }

        public char C1
        {
            get
            {
                return '\u0001';
            }
        }

        public char C0
        {
            get
            {
                return '\u0000';
            }
        }

        public char C32
        {
            get
            {
                return '\u0020';
            }
        }

        public char C8
        {
            get
            {
                return '\u0008';
            }
        }

        public char CFA
        {
            get
            {
                return '\u00FA';
            }
        }


        public char CDblQuote
        {
            get
            {
                return '\u201c';
            }
        }

        public char CFS
        {
            get
            {
                return '\u001c';
            }
        }

        public char C93
        {
            get
            {
                return '\u0093';
            }
        }


        #endregion

        public Encabezado Encabezado
        {
            get;
            set;
        }
        public List<DetalleFactura> Detalle
        {
            get;
            set;
        }
        public Pie Pie
        {
            get;
            set;
        }
        public List<Impuesto> listImpuesto
        {
            get;
            set;
        }
        public string TotImpuesto
        {
            get;
            set;
        }
        public string TotBase
        {
            get;
            set;
        }
        public string TotIva
        {
            get;
            set;
        }
        public string TotImtoString { get; set; }

        // Propiedades para imprimir texto completo en impresora K3

        public List<string> listLineaDetalle { get; set; }
        //public List<string> lineaDetalleCantidad { get; set; }

    }
}
