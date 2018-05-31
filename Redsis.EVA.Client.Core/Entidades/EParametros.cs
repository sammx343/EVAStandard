using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{

    /// <summary>
    /// Almacena los parametros.
    /// Trabaja en dos fases:
    /// I   carga todos los parametros de punto de venta al inicio de la aplicacion, 
    ///     antes de autenticar al usuario.
    /// II  Una vez autenticado el usuario se debe invocar el metodo 'computar' con la 
    ///     información del usuario para calcular los parémtros personalizados por usuario.
    /// </summary>
    public class EParametros
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, EParametro> ListaParamatros = new Dictionary<string, EParametro>();

        //TODO establecer funcionalidad para retornar el valor del parametro convertido a su tipo correspondiente. (Genericos)
        public EParametro Parametro(string nombre)
        {
            if (ListaParamatros.ContainsKey(nombre))
            {
                return ListaParamatros[nombre];
            }
            else
            {
                log.Info("[Parametro]: Parametro no encontrado.");
                return null;
            }

        }
        
        public T ObtenerValorParametro<T>(string nombre)
        {
            try
            {
                EParametro value = Parametro(nombre);
                return (T)Convert.ChangeType(value.Valor, typeof(T));
            }
            catch (Exception ex)
            {
                log.ErrorFormat("[ObtenerValorParametro] {0} / {1}", nombre, ex.Message);
                return default(T);
            }
        }

        public void Agregar(EParametro parametro)
        {
            ListaParamatros.Add(parametro.Nombre, parametro);
        }
    }
}
