using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using EvaPOS;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Transactions;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PArqueo
    {
        private static readonly log4net.ILog log =
           log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();

        public ECaja obtenerEcaja(ETerminal terminal, EUsuario usuario, EMediosPago mediosPago, out Respuesta respuesta)
        {
            ECaja caja = null;
            respuesta = new Respuesta(false);

            RArqueo rArqueo = new RArqueo();
            var arqueo = rArqueo.ObtenerArqueo(terminal.Codigo, usuario.IdUsuario);

            if (arqueo.IsNullOrEmptyTable())
            {
                respuesta.Mensaje = "Codigos de Recogidas no encontrados.";
                respuesta.Valida = false;
            }
            else
            {
                Dictionary<EMedioPago, decimal> dictionary = new Dictionary<EMedioPago, decimal>();

                foreach (DataRow dr in arqueo.Rows)
                {
                    var mediopaog = (string)dr["id_medio_pago"];
                    dictionary.Add(mediosPago.MedioPago(mediopaog), (decimal)dr["total"]);
                }

                caja = new ECaja(dictionary);
                respuesta.Valida = true;
            }

            return caja;
        }

        public void GuardarArqueo(ECaja caja, ref Dictionary<string, string> IdsAcumulados, ETerminal terminal, EUsuario usuario, string tipo, string contenido, string modeloImpresora, out Respuesta respuesta)
        {
            //1. Creamos el encabezado de la venta.
            string idArqueo = Guid.NewGuid().ToString();
            RArqueo rArqueo = new RArqueo();
            respuesta = new Respuesta(false);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rArqueo.CrearArquero(idArqueo, terminal.Codigo, tipo, 1, (long)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario) == 1)
                    {
                        //3. Creamos totales de venta(registro_venta).
                        if (IdsAcumulados == null)
                        {
                            IdsAcumulados = new Dictionary<string, string>();
                            IdsAcumulados.Add("idRegistroVenta", Guid.NewGuid().ToString());
                            //Creamos el registro venta
                            rArqueo.CrearRegistroVenta(IdsAcumulados["idRegistroVenta"], terminal.Codigo, usuario.IdUsuario);
                        }
                        //Actualizo registro venta
                        rArqueo.ActualizarRegistroArqueo(IdsAcumulados["idRegistroVenta"]);

                        Dictionary<EMedioPago, List<decimal>> arqueos = caja.Arqueo;

                        foreach (var arqueo in arqueos)
                        {
                            rArqueo.GuardarArqueo(arqueo.Key.Codigo, (int)terminal.NumeroUltimaTransaccion + 1, arqueo.Value[1], arqueo.Value[2], arqueo.Value[0], idArqueo);
                        }
                        rVenta.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura + 1, (long)terminal.NumeroUltimaTransaccion + 1);
                        //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(contenido);
                        //contenido = System.Convert.ToBase64String(plainTextBytes);
                        rVenta.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, (long)terminal.NumeroUltimaFactura + 1, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);
                        respuesta.Valida = true;
                    }
                    else
                    {
                        throw new Exception("[GuardarArqueo]: Transaccion no pudo ser guardada.");
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
                    log.Error("[GuardarArqueo]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[GuardarArqueo]: No pudo ser guardada la transaccion: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, "No pudo ser guardada el arqueo.");
                log.Error("[GuardarArqueo]: No pudo ser guardada el arqueo. " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }
    }
}
