using System;
using System.Collections.Generic;
using System.Text;

namespace Redsis.EVA.Client.Core.Interfaces
{
    public interface IPanelActivo
    {
        string NombrePanel { get; set; }
        bool IsFocused { get; set; }
        void SetTextBoxFocus();
    }
}
