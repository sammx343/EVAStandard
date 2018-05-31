using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Repositorio
{
    public class DataRowUtil
    {
        public static bool TieneNulos(DataRow registro)
        {
            if (registro == null)
            {
                return true;
            }
            foreach (DataColumn columna in registro.Table.Columns)
            {
                if (registro[columna] == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
