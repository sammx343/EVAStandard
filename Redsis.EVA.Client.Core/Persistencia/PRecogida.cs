using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Common.Telemetria;
using Redsis.EVA.Client.Core.Entidades;
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
    public class PRecogida
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        RVenta rVenta = new RVenta();
        public void GuardarRecogida(ERecogida recogida, ref Dictionary<string, string> IdsAcumulados, string tipo, ETerminal terminal, EUsuario usuario, EMedioPago medioPago, string contenido, string modeloImpresora, out Respuesta respuesta)
        {
            //1. Creamos el encabezado de la venta.
            string idVenta = Guid.NewGuid().ToString();
            RRecogida rRecogida = new RRecogida();
            respuesta = new Respuesta(false);

            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (rRecogida.CrearRecogida(idVenta, recogida.Valor, terminal.Codigo, tipo, 1, terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, usuario.IdUsuario) == 1)
                    {
                        rRecogida.CrearVentaRecogida(recogida.CodigoRecogida.Codigo, recogida.CodigoRecogida.Descripcion, idVenta);

                        //3. Creamos totales de venta(registro_venta).
                        if (IdsAcumulados == null)
                        {
                            IdsAcumulados = new Dictionary<string, string>();
                            IdsAcumulados.Add("idRegistroVenta", Guid.NewGuid().ToString());
                            //Creamos el registro venta
                            rRecogida.CrearRegistroVenta(IdsAcumulados["idRegistroVenta"], terminal.Codigo, usuario.IdUsuario);
                        }
                        //Actualizo registro venta
                        rRecogida.ActualizarRegistroVenta(IdsAcumulados["idRegistroVenta"], recogida.Valor);

                        //4. Creamos el detalle de los medios de pago.
                        //List<EPago> pagos = re // prestamo.Pagos;

                        rRecogida.CrearMedioPago(medioPago.Codigo, terminal.Codigo, (long)terminal.NumeroUltimaTransaccion + 1, recogida.Valor, idVenta);

                        //Actualizamos el terminal
                        rRecogida.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);
                        rVenta.ActualizarTerminal(terminal.Codigo, (long)terminal.NumeroUltimaFactura, (long)terminal.NumeroUltimaTransaccion + 1);

                        rVenta.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, tipo, usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, (long)terminal.NumeroUltimaFactura, (int)terminal.NumeroUltimaTransaccion + 1, terminal.Prefijo, terminal.Codigo, tipo, usuario.IdUsuario);
                        respuesta.Valida = true;
                        respuesta.Mensaje = idVenta;
                    }
                    else
                    {
                        throw new Exception("[GuardarRecogida]: Transaccion no pudo ser guardada.");
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
                    log.Error("[GuardarRecogida]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[Guardarrecogida]: No pudo ser guardada la venta: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Documentar(false, " No pudo ser guardada la recogida.");
                log.Error("[GuardarRecogida]: No pudo ser guardada la recogida. " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }
        }

        public ECodigosRecogida GetCodigosRecogida(out Respuesta respuesta)
        {
            ECodigosRecogida codigosRecogida = new ECodigosRecogida();
            respuesta = new Respuesta(false);

            //Parametro de venta por PLU o codigo de barra llamando a la instancia de parametros en el entorno.
            var rRecogida = new RRecogida();
            var codigoDeRecogidas = rRecogida.GetAllCodigoDeRecogidas();

            //
            if (codigoDeRecogidas == null)
            {
                respuesta.Mensaje = "Codigos de Recogidas no encontrados.";
                respuesta.Valida = false;
            }
            else
            {
                foreach (DataRow dr in codigoDeRecogidas.Rows)
                {
                    codigosRecogida.CodigosRecogida.Add(RecogidaUtil.InstanciarDesde(dr));
                }
                respuesta.Valida = true;
            }

            return codigosRecogida;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class RecogidaUtil
    {
        public static ECodigoRecogida InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ArgumentNullException("Datos de CodigosRecogida nulos o vacíos.");
            }
            ECodigoRecogida codigoRecogida;
            string codigo, descripcion;
            codigo = (string)registro["id_codificacion"];
            descripcion = (string)registro["descripcion"];
            codigoRecogida = new ECodigoRecogida(codigo, descripcion);


            return codigoRecogida;
        }
    }
}
