using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Interfaces;
using EvaPOS.Enums;

namespace Redsis.EVA.Client.Core.DTOs
{
    public class DEstadoActual : IEstadoActual
    {
        public string EstadoActual
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public EstadosFSM EstadoFSM
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string EstadoValido
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        EstadosFSM IEstadoActual.EstadoFSM { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
