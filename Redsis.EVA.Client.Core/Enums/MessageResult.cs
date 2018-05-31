using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Enums
{
    public enum MessageResult
    {
        None = -1,
        Default = -2,
        Negative = 0,
        Affirmative = 1,
        FirstAuxiliary = 2,
        SecondAuxiliary = 3,
    }
}
