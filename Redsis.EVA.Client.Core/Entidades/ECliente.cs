using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ECliente // : IEquatable<ECliente>
    {
        public string Id { get; private set; }
        public string Codigo { get; private set; }
        public string Tipo { get; private set; }
        public string TelefonoResidencia { get; private set; }
        public string PrimerNombre { get; private set; }
        public string SegundoNombre { get; private set; }
        public string PrimerApellido { get; private set; }
        public string SegundoApellido { get; private set; }
        public string Direccion { get; private set; }
        public string Pais { get; private set; }
        public string Departamento { get; private set; }
        public string Ciudad { get; private set; }

        public ECliente()
        {
            Id = "";
            Codigo = "";
            Tipo = "";
            TelefonoResidencia = "";
            PrimerNombre = "";
            SegundoNombre = "";
            PrimerApellido = "";
            SegundoApellido = "";
            Direccion = "";
            Pais = "";
            Departamento = "";
            Ciudad = "";
        }

        public ECliente(string id, string codigo, string tipo, string telefonoResidencia, string primerNombre, string segundoNombre, string primerApellido, string segundoApellido, string direccion,
            string pais, string departamento, string ciudad)
        {
            Id = id;
            Codigo = codigo;
            Tipo = tipo;
            TelefonoResidencia = telefonoResidencia;
            PrimerNombre = primerNombre;
            SegundoNombre = segundoNombre;
            PrimerApellido = primerApellido;
            SegundoApellido = segundoApellido;
            Direccion = direccion;
            Pais = pais;
            Departamento = departamento;
            Ciudad = ciudad;
        }

        public override string ToString()
        {
            return string.Format(
                "Medio Pago [Id:{0}, Codigo:{1}, Tipo {2}, TelefonoResidencia:{3}, PrimerNombre:{4}, PrimerApellido:{5}, Direccion:{6}, Pais:{7}, Departamento:{8}, Ciudad:{9}]",
                Id,
                Codigo,
                Tipo,
                TelefonoResidencia,
                PrimerNombre,
                PrimerApellido,
                Direccion,
                Pais,
                Departamento,
                Ciudad
                );
        }

        //public override bool Equals(object obj)
        //{
        //    if (obj == null)
        //        return false;
        //    else
        //    {
        //        return true;                
        //    }
        //}

        //public override int GetHashCode()
        //{
        //    return Codigo.GetHashCode();
        //}

        //bool IEquatable<ECliente>.Equals(ECliente item)
        //{
        //    if (item == null)
        //        return false;
        //    return (this.Codigo.Equals(item.Codigo));
        //}
    }
}
