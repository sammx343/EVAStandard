using EvaPOS;
using Redsis.EVA.Client.Core.DTOs;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PMediosPago
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EMediosPago GetAllMediosPago()
        {
            var repositorio = new RMedioPago();
            var medios = new EMediosPago();
            var registros = repositorio.GetAllMediosPago();
            foreach (DataRow registro in registros.Rows)
            {
                var medio = MedioPagoUtil.InstanciarDesde(registro);
                medios.ListaMediosPago.Add(medio);
            }
            return medios;
        }

        public List<DMedioPago> GetDTOsMediosPago()
        {
            List<DMedioPago> mediosPago = null;

            EMediosPago eMedioPago = GetAllMediosPago();
            if (!eMedioPago.ListaMediosPago.IsNullOrEmptyList())
            {
                mediosPago = (from m in eMedioPago.ListaMediosPago
                              select new DMedioPago
                              {
                                  CodigoMedioPago = m.Codigo,
                                  NombreMedioPago = m.Tipo
                              }).ToList();
            }

            return mediosPago;
        }
    }

    public class MedioPagoUtil
    {
        public static EMedioPago InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ApplicationException("Registro nulo o contiene campos nulos.");
            }
            var a = new EMedioPago(
                (string)registro["id_medio_pago"],
                (string)registro["tipo"],
                (string)registro["sub_tipo"],
                (byte)registro["solicita_autorizacion"],
                (byte)registro["solicita_banco"],
                (byte)registro["solicita_documento"],
                (byte)registro["solicita_fanqueo"],
                (byte)registro["solicita_nro_meses"],
                (byte)registro["permite_cambio"],
                (int)registro["valor_limite_cambio"],
                (decimal)registro["pctj_limite_cambio"],
                (string)registro["formato_franqueo"],
                (byte)registro["verifica_pago"],
                (byte)registro["abre_cajon"],
                (byte)registro["credito"]
                );
            return a;
        }
    }
}
