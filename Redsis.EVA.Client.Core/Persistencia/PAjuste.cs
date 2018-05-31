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
    public class PAjuste
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // TODO : este requiere manejo transaccional!.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ajuste"></param>
        /// <param name="terminal"></param>
        /// <param name="usuario"></param>
        /// <param name="localidad"></param>
        /// <param name="numero"></param>
        /// <param name="tipo"></param>
        /// <param name="contenido"></param>
        /// <param name="modeloImpresora"></param>
        /// <param name="respuesta"></param>
        public void GuardarAjuste(EAjuste ajuste, ETerminal terminal, EUsuario usuario, ELocalidad localidad, ETipoAjuste tipo, string contenido, string modeloImpresora, out Respuesta respuesta)
        {
            respuesta = new Respuesta(false);
            //1. Creamos el encabezado de la venta.
            string idAjuste = Guid.NewGuid().ToString();
            RAjuste rajuste = new RAjuste();
            RVenta rventa = new RVenta();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    var reg = rajuste.GetNumeroAjuste();

                    // int numero = reg.Field<int>("nro_ajuste");
                    int numero = (int)reg["nro_ajuste"];

                    if (rajuste.CrearAjuste(idAjuste, tipo.CalcularCostoVenta, "00", localidad.Codigo, ajuste.Estado, localidad.Codigo, numero + 1, tipo.Signo, tipo.Id, tipo.Codigo, 0, ajuste.TotalVenta, usuario.IdUsuario, ajuste.TotalImpuesto()) == 1)
                    {

                        //2. Creamos cada detalle de la venta.
                        string idAjusteDetalle = Guid.NewGuid().ToString();
                        var tirilla = ajuste.CopiaTirilla;
                        foreach (EItemVenta detalle in tirilla)
                        {
                            respuesta.Valida = true;
                            rajuste.CrearAjusteDetalle(idAjusteDetalle, idAjuste, detalle.Articulo.Id, detalle.Cantidad, "00", detalle.Articulo.CodigoImpresion, 0, numero + 1, detalle.Articulo.PrecioVenta1, detalle.Impuesto, detalle.Articulo.Impuesto1);
                            idAjusteDetalle = Guid.NewGuid().ToString();
                        }
                        rventa.CrearCopiaImpresion("00", terminal.Localidad.Codigo, terminal.Codigo, ((int)Enums.TipoTransaccion.Ajuste).ToString(), usuario.IdUsuario, contenido, terminal.Localidad.Codigo, modeloImpresora, 0, numero + 1, terminal.Prefijo, terminal.Codigo, ((int)Enums.TipoTransaccion.Ajuste).ToString(), usuario.IdUsuario);

                    }
                    else
                    {
                        throw new Exception("[GuardarAjuste]: Transaccion no pudo ser guardada.");
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
                    log.Error("[GuardarAjuste]: No pudo ser guardada la transaccion: " + e.Message);
                }
                else
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Hubo un problema al momento de guardar la transaccion. Por favor contacte al administrador del sistema.";
                    log.Error("[GuardarAjuste]: No pudo ser guardada la transaccion: " + e.Message);
                }
                Telemetria.Instancia.AgregaMetrica(new Excepcion(e));
            }
            catch (Exception ex)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "[GuardarAjuste]: No pudo ser guardado el ajuste.";
                log.Error("[GuardarAjuste]: No pudo ser guardado el ajuste: " + ex.Message);
                Telemetria.Instancia.AgregaMetrica(new Excepcion(ex));
            }

        }
    }
}
