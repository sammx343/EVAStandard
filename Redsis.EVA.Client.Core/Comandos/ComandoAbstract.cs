using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Core.Interfaces;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using Redsis.EVA.Client.Core.Persistencia;
using Redsis.EVA.Client.Core.Interfaces;

namespace Redsis.EVA.Client.Core.Comandos
{
    public abstract class ComandoAbstract : IComando
    {
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected Entorno Entorno = Entorno.Instancia;
        protected IVista iu = Entorno.Instancia.Vista;
        protected EParametros Parametros = Entorno.Instancia.Parametros;

        public ComandoAbstract(ISolicitud solicitud)
        {
        }

        public abstract void Ejecutar();

        public void LanzarApplicationException(string message)
        {
            throw new ApplicationException(message);
        }

    }
}
