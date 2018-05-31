using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    /// <summary>
    /// Recupera cadena conexion DB definida en App.config.
    /// Singleton con lazy inicialización.
    /// La primera invocación al SingletonCreador.instancia instancia la clase interna.
    /// </summary>
    public class CadenaConexionDB
    {
        string cadena = null;

        private CadenaConexionDB()
        {
            cadena = Entorno.Instancia.BaseDato.CadenaConexion;
            if (cadena == null)
            {
                throw new ApplicationException("EVAConnectionString no definia en archivo App.config.");
            }
        }

        class SingletonCreador
        {
            static SingletonCreador()
            {
            }
            internal static readonly CadenaConexionDB instancia = new CadenaConexionDB();
        }

        public static string Instancia
        {
            get
            {
                return SingletonCreador.instancia.cadena;
                //return Entorno.Instancia.BaseDato.CadenaConexion;
            }
        }
    }
}
