using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace Redsis.EVA.Client.Core.Persistencia
{
    internal class PIntervencion
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public void GuardarIntervencion(EIntervencion intervencion, ETerminal terminal, EUsuario usuario, out Respuesta respuesta)
        {
            //1. Creamos el encabezado de la venta.
            string idVenta = Guid.NewGuid().ToString();
            RIntervencion rCajon = new RIntervencion();
            respuesta = new Respuesta(false);
            Dictionary<string, string> IdsAcumulados = Entorno.Instancia.IdsAcumulados;

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //if (IdsAcumulados == null)
                    //{
                    //    IdsAcumulados = new Dictionary<string, string>();
                    //    IdsAcumulados.Add("idIntervencion", Guid.NewGuid().ToString());
                    //    //Creamos el registro venta
                    //    rCajon.CrearRegistroVenta(IdsAcumulados["idIntervencion"], terminal.Codigo, usuario.IdUsuario);
                    //}

                    //Actualizo registro venta
                    //rCajon.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], rCajon.Valor);

                    int resul = rCajon.CrearRegistroIntervencion(Guid.NewGuid().ToString(), intervencion.id_venta, intervencion.claveSupervisor, intervencion.motivo, terminal.Codigo, intervencion.nro_transac, usuario.IdUsuario);
                    if (resul == 1)
                    {
                        //Actualizamos el terminal
                        rCajon.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);
                        rVenta.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);

                        respuesta.Valida = true;
                    }
                    else
                    {
                        throw new Exception("[GuardarIntervencion]: Transaccion no pudo ser guardada.");
                    }
                    scope.Complete();
                }
            }
            catch (SqlException e)
            {
                if (e.Number == -2 || e.Number == 121)
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Se perdió la conexión con el servidor.";
                    log.Error("[GuardarIntervencion]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[GuardarIntervencion]: No pudo ser guardada la intervención: " + e.Message);
                }

                //
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, " No pudo ser guardada la intervención.");
                log.Error("[GuardarIntervencion]: No pudo ser guardada la intervención. " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }
    }
}
