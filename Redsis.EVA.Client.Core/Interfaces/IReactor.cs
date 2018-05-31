using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IReactor
    {
        void Procesar(ISolicitud s);
    }
}
