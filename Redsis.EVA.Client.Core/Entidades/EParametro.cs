using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EParametro : IEquatable<EParametro>
    {
        #region Atributos
        public string Nombre { get; private set; }
        public string Tipo { get; private set; }
        public string Valor { get; private set; }
        public string Ambito { get; private set; }
        public string IdAmbito { get; private set; }

        #endregion

        #region Constructores
        public EParametro()
        {
            Nombre = "";
            Tipo = "";
            Valor = "";
            Ambito = "";
            IdAmbito = "";
        }
        public EParametro(string nombre,string tipo, string valor, string ambito, string idambito)
        {
            Nombre = nombre;
            Tipo = tipo;
            Valor = valor;
            Ambito = ambito;
            IdAmbito = idambito;
        }
        #endregion
        bool IEquatable<EParametro>.Equals(EParametro other)
        {
            if (other == null)
                return false;
            return (this.Nombre.Equals(other.Nombre));
        }
        public override int GetHashCode()
        {
            return Nombre.GetHashCode();
        }
    }
}
