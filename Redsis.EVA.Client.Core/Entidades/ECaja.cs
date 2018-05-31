using Redsis.EVA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Entidades
{
    public class ECaja
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<EMedioPago, List<decimal>> Arqueo { get; private set; } = new Dictionary<EMedioPago, List<decimal>>();

        public ECaja(Dictionary<EMedioPago, decimal> enCaja)
        {
            foreach (var item in enCaja)
            {
                Arqueo.Add(item.Key, new List<decimal> { item.Value, 0, item.Value });
            }
        }

        /// <summary>
        /// Agrega un valor al medio de pago seleccionado
        /// </summary>
        /// <param name="medioPago"></param>
        /// <param name="valor"></param>
        /// <param name="respuesta"></param>
        public void AgregarValor(EMedioPago medioPago, decimal valor, out Respuesta respuesta)
        {
            respuesta = new Respuesta(true);
            try
            {
                Arqueo[medioPago][1] += valor;
                Arqueo[medioPago][2] -= valor;
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, "Error: " + ex.Message);
            }
        }
    }
}
