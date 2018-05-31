using Redsis.EVA.Client.Common;
using Newtonsoft.Json;
using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Enums;
using Redsis.EVA.Client.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvaPOS;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public abstract class ETransaccion : IEPagable
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [JsonProperty]
        public decimal BrutoPositivo { get; protected set; } = 0;
        [JsonProperty]
        public decimal BrutoNegativo { get; protected set; } = 0;
        [JsonProperty]
        public ECliente Cliente { get; protected set; } = null;
        [JsonProperty]
        public List<EItemVenta> tirilla { get; protected set; } = new List<EItemVenta>();
        [JsonProperty]
        public decimal TotalVenta { get; protected set; } = 0;//Algunos esto puede ser cero
                                                              //ToDo: Metodo para cerra tranc y verifica que de verdad se pueda cerrar.
        [JsonProperty]
        public bool EstaAbierta = false;

        public Dictionary<EImpuesto, List<decimal>> ImpuestosIncluidos { set; get; } = new Dictionary<EImpuesto, List<decimal>>();

        /// <summary>
        /// Obtiene o establece diccionario de impuestos de la venta que se usa para la impresión del detalle de impuestos.
        /// </summary>
        public Dictionary<EImpuestosArticulo, List<decimal>> ImpuestosIncluidosCompuestos { set; get; } = new Dictionary<EImpuestosArticulo, List<decimal>>();

        protected void IniciarTransaccion()
        {
            BrutoPositivo = 0;
            BrutoNegativo = 0;
            Cliente = null;
            tirilla.Clear();
            TotalVenta = 0;
            ImpuestosIncluidos = new Dictionary<EImpuesto, List<decimal>>();
            ImpuestosIncluidosCompuestos = new Dictionary<EImpuestosArticulo, List<decimal>>();
            EstaAbierta = true;
        }
        public abstract EItemVenta AgregarArticulo(EArticulo articulo, int cantidad, string codigoLeido, List<EImpuesto> impuestos, bool implementaImpuestoCompuesto, out Respuesta respuesta);

        public decimal CalcularImpuesto(decimal valor, decimal porcentaje)
        {
            decimal impuesto = valor - (valor / (1 + (porcentaje / 100)));
            impuesto = Math.Round(impuesto, 0, MidpointRounding.AwayFromZero);
            return impuesto;
        }

        protected decimal CalcularValor(decimal valor, int cantidad)
        {

            decimal total = valor * cantidad;
            return total;
        }

        protected void ActualizarImpuestosIncluidos(EItemVenta item, List<EImpuesto> impuestos)
        {
            EImpuesto impuesto = impuestos.Find(x => x.Porcentaje == (float)item.Articulo.Impuesto1);
            if (ImpuestosIncluidos.ContainsKey(impuesto))
            {
                ImpuestosIncluidos[impuesto][0] += item.Valor;
                ImpuestosIncluidos[impuesto][1] += (item.Valor - item.Impuesto);
                ImpuestosIncluidos[impuesto][2] += item.Impuesto;
            }
            else
            {
                ImpuestosIncluidos.Add(impuesto, new List<decimal> { item.Valor, item.Valor - item.Impuesto, item.Impuesto });

            }
        }

        /// <summary>
        /// Genera el detalle de los impuestos del tiquete de la venta, calcula los valores para "Compra", "Base / Impuesto", "Impuesto" que serán impresos en la tirilla.
        /// </summary>
        /// <param name="item"></param>
        protected void ActualizarImpuestosIncluidosCompuestos(EItemVenta item)
        {
            foreach (KeyValuePair<EImpuestosArticulo, decimal> entry in item.Impuestos)
            {
                EImpuestosArticulo impuesto = ImpuestosIncluidosCompuestos.Where(d => d.Key.Id == entry.Key.Id).FirstOrDefault().Key;
                //impuesto = impuesto !=null? impuesto: entry.Key;
                decimal compra = entry.Key.TipoImpuesto == 2 ? 0 : item.Valor;
                decimal compraBase = entry.Key.TipoImpuesto == 2 ? 0 : (item.Valor - item.Impuesto);
                decimal compraI = entry.Value;
                if (impuesto != null)
                {
                    ImpuestosIncluidosCompuestos[impuesto][0] += compra;
                    ImpuestosIncluidosCompuestos[impuesto][1] += compraBase;
                    ImpuestosIncluidosCompuestos[impuesto][2] += compraI;
                }
                else
                {
                    ImpuestosIncluidosCompuestos.Add(entry.Key, new List<decimal> { compra, compraBase, compraI });

                }
            }
        }

        protected void ActualizarTotales(int cantidad, decimal valor)
        {
            if (cantidad > 0)
            {
                BrutoPositivo += valor;
            }
            else
            {
                BrutoNegativo += valor * -1;
            }
            TotalVenta = BrutoPositivo - BrutoNegativo;
        }

        public void ImpuestosIncluidosFromJson(string json)
        {
            this.ImpuestosIncluidos = JsonConvert.DeserializeObject<Dictionary<EImpuesto, List<decimal>>>(json);
        }


        public void AgregarItemVenta(EItemVenta item)
        {
            tirilla.Add(item);
        }

        // TODO hay que hacer un caso de prueba para esto.
        public List<EItemVenta> CopiaTirilla
        {
            get
            {
                //EItemVenta[] tirillaCopia = new EItemVenta[tirilla.Count];
                //// No se usa List<T>.CopyTo(T) porque copia por referencia.
                //int i = 0;
                //foreach (EItemVenta item in tirilla)
                //{
                //    tirillaCopia[i] = item.Clone() as EItemVenta;
                //    i++;
                //}
                //return tirillaCopia;

                return (from t in tirilla
                        select t.Clone() as EItemVenta).ToList();
            }
        }

        protected bool EsDiferenteCero(int cantidad, ref Respuesta respuesta)
        {
            if (cantidad == 0)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "Cantidad invalida.";
                log.Info("[EsDiferenteCero]: Cantidad es menor a cero.");
                return false;
            }
            else
            {
                return true;
            }
        }

        protected bool EsValidoCancelarItem(EArticulo articulo, int cantidad, ref Respuesta respuesta)
        {
            bool esValido = true;

            int total = tirilla.Where(t => t.Articulo.Equals(articulo)).Sum(a => a.Cantidad);

            if (Math.Abs(cantidad) > Math.Abs(total))
            {
                log.Info("[ValidarCancelarItem.2]: Cantidad a cancelar mayor a la de la tirilla.");
                respuesta.Mensaje = "La cantidad a cancelar no es válida";
                respuesta.Valida = false;
                esValido = false;
            }
            else if (((Math.Abs(total) - Math.Abs(cantidad)) < 0) || !EstaAbierta)
            {
                log.Info("[ValidarCancelarItem]: Cantidad a cancelar mayor a la de la tirilla.");
                respuesta.Valida = false;
                if (total == 0)
                {
                    respuesta.Mensaje = "El articulo a cancelar no se encuentra en la tirilla.";
                }
                else
                {
                    respuesta.Mensaje = "La cantidad a cancelar no es valida.";
                }
                esValido = false;
            }
            return esValido;

        }

        protected bool EsValidoImpuesto(EArticulo articulo, List<EImpuesto> impuestos, ref Respuesta respuesta)
        {
            var ans = true;

            //todo: incluir try cath para todo el método

            var impuestosFind = (from i in impuestos
                                 where i.Porcentaje == (float)articulo.Impuesto1
                                 select i).ToList();


            if (!impuestosFind.IsNullOrEmptyList())
            {
                if (impuestosFind.Count > 1)
                {
                    log.Warn("[EsValidoImpuesto]: Impuesto invalido.");
                    //
                    respuesta.Valida = false;
                    respuesta.Mensaje = $"Se encontró varios impuestos configurados con porcentaje {articulo.Impuesto1}.";
                    ans = false;
                }
            }
            else
            {
                log.Warn("[EsValidoImpuesto]: Impuesto invalido.");
                //
                respuesta.Valida = false;
                respuesta.Mensaje = "Valor del impuesto del artículo no ha sido definido.";
                ans = false;
            }



            return ans;
        }
        protected bool EsValidoImpuestoCompuesto(EArticulo articulo, ref Respuesta respuesta)
        {
            var ans = true;
            if (articulo.Impuestos.IsNullOrEmptyList())
            {
                log.Warn("[EsValidoImpuestoCompuesto]: Articulo no tiene impuestos definidos.");
                //
                respuesta.Valida = false;
                respuesta.Mensaje = "El artículo no tiene impuestos definidos.";
                ans = false;
            }
            return ans;
        }

        public int NumeroDeItemsVenta
        {
            get
            {
                return tirilla.Sum(t => t.Cantidad);
            }
        }

        //ToDo: Numero de registros en la tirilla.
        //ToDo: Numero de registros de pagos.
        public int NumeroDeItemsPositivo
        {
            get
            {
                return tirilla.Where(h => h.Cantidad > 0).Sum(t => t.Cantidad);
            }
        }

        public int NumeroDeItemsNegativo
        {
            get
            {
                return (tirilla.Where(h => h.Cantidad < 0).Sum(t => t.Cantidad)) * -1;
            }
        }

        public bool EsPagable { get; } = true;

        public bool EsPermitidoPagar
        {
            get
            {
                return tirilla.Count > 0;
            }
        }

        public void AgregarCliente(ECliente cliente)
        {
            Cliente = cliente;
        }

        public void EliminarCliente()
        {
            if (Cliente != null)
            {
                Cliente = null;
            }
        }

        public List<EItemVenta> tirillaLimpia()
        {
            List<EItemVenta> limpia = new List<EItemVenta>();
            foreach (var item in this.tirilla)
            {
                EItemVenta encontrado = limpia.Where(t => t.Articulo.CodigoImpresion == item.Articulo.CodigoImpresion).FirstOrDefault();
                if (encontrado != null)
                {
                    encontrado.Cantidad += item.Cantidad;
                    encontrado.Valor += item.Valor;
                    if (encontrado.Cantidad == 0)
                    {
                        limpia.Remove(encontrado);
                    }
                }
                else
                {
                    limpia.Add(item.Clone() as EItemVenta);
                }
            }
            return limpia;
        }


    }
}
