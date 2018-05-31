using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{

    public class EError : IEquatable<EError>
    {
        #region Atributos
        public int Codigo { get; private set; }
        public string Mensaje { get; private set; }
        public string Descripcion { get; private set; }
        public string Explicacion { get; private set; }
        public string Solucion { get; private set; }
        #endregion

        #region Constructores
        public EError()
        {
            Codigo = 0;
            Mensaje = "";
            Descripcion = "";
            Explicacion = "";
            Solucion = "";
        }
        public EError(int Codigo, string Mensaje, string Descripcion, string Solucion)
        {
            this.Codigo = Codigo;
            this.Mensaje = Mensaje;
            this.Descripcion = Descripcion;
            this.Explicacion = Explicacion;
            this.Solucion = Solucion;
        } 
        #endregion

        #region CompararObejtos
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            EError objError = obj as EError;
            if (objError == null)
                return false;
            else
                return Equals(objError);
        }
        bool IEquatable<EError>.Equals(EError item)
        {
            if (item == null)
                return false;
            return (this.Codigo.Equals(item.Codigo));
        }
        public override int GetHashCode()
        {
            return Codigo.GetHashCode();
        } 
        #endregion


    }
}
