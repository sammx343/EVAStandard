using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Helpers.Impresion
{
    public class Prestamo
    {
        #region Caracteres POS
        public char EscCut
        {
            get
            {
                return '\u001A';
            }
        }

        public char Esc
        {
            get
            {
                return '\u001b';
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
                return '\u0024';
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
        #endregion

        public string Encabezado
        {
            get;
            set;
        }
        public string Valor
        {
            get;
            set;
        }
        public string MensajeValor
        {
            get;
            set;
        }
        public string Mensaje
        {
            get;
            set;
        }
        public string Fecha
        {
            get;
            set;
        }
        public string Concepto
        {
            get;
            set;
        }
        public string MensajeTitulo
        {
            get;
            set;
        }
        public string Cajero
        {
            get;
            set;
        }
        public string Local
        {
            get;
            set;
        }
        public string Terminal
        {
            get;
            set;
        }
        public string Trx
        {
            get;
            set;
        }
        public bool impresoraNCR { get; set; } = false;

    }
}
