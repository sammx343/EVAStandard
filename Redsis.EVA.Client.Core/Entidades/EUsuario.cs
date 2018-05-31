using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EUsuario : IEquatable<EUsuario>
    {
        public string IdUsuario { get; private set; }
        public string Usuario { get; private set; }
        public string Nombre { get; private set; }
        public string Apellido { get; private set; }
        public string Clave { get; set; }
        public string ClaveSupervisor { get; set; }
        public EUsuario UsuarioSupervisor { get; set; }

        public EUsuario()
        {
            IdUsuario = "";
            Usuario = "";
            Nombre = "";
            Apellido = "";
            Clave = "";
            ClaveSupervisor = "";
        }

        public EUsuario(string idUsuario,string usuario, string nombre, string apellido, string clave, string claveSupervisor)
        {
            IdUsuario = idUsuario;
            Usuario = usuario;
            Nombre = nombre;
            Apellido = apellido;
            Clave = clave;
            ClaveSupervisor = claveSupervisor;
        }

        public override string ToString()
        {
            return string.Format(
                "[Usuario:{0}, Nombre {1}, Apellido:{2}, Clave:{3}]",
                Usuario,
                Nombre,
                Apellido,
                Clave
                );
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            EUsuario objUsuario = obj as EUsuario;
            if (objUsuario == null)
                return false;
            else
                return Equals(objUsuario);
        }

        public override int GetHashCode()
        {
            return Usuario.GetHashCode();
        }

        bool IEquatable<EUsuario>.Equals(EUsuario item)
        {
            if (item == null)
                return false;
            return (this.Usuario.Equals(item.Usuario));
        }

        public bool Autenticar(string clave)
        {
            /*string hash = sha256_hash(clave);
            if (String.Equals(hash, Clave, StringComparison.Ordinal))
            {
                return true;
            }
            else
            {
                return false;
            }*/
            return true;

        }

       /*public static String sha256_hash(String value)
        {
            using (SHA256 hash = SHA256Managed.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(value))
                  .Select(item => item.ToString("x2")));
            }
        }*/
    }
}
