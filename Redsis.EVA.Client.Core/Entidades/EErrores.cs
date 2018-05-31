using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class EErrores
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<EError> ListaErrores { get; set; }
        public EErrores()
        {
            ListaErrores = new List<EError>();
        }

        public EError Error(int codigo)
        {
            EError particular = ListaErrores.Find(x => x.Codigo == codigo);
            if(particular != null)
            {
                return particular;
            }
            else
            {
                log.Info("[Error]: Error no encontrado.");
                throw new Exception("Error no encontrado.");
            }
            
        }
    }
    
}
