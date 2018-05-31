using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Redsis.EVA.Client.Core.Entidades
{
    [JsonObject(MemberSerialization.OptIn)]
    public class EArticulo : IEquatable<EArticulo>
    {
        [JsonProperty]
        public string Id { get; private set; }
        [JsonProperty]
        public string CodigoImpresion { get; private set; }
        [JsonProperty]
        public string DescripcionCorta { get; private set; }
        [JsonProperty]
        public string DescripcionLarga { get; private set; }
        [JsonProperty]
        public decimal PrecioVenta1 { get; private set; }
        [JsonProperty]
        public bool PesoRequerido { get; private set; }
        [JsonProperty]
        public bool Descodificado { get; private set; }
        [JsonProperty]
        public decimal Impuesto1 { get; set; }
        [JsonProperty]
        public List<String> CodigosPorArticulo { get; private set; }
        [JsonProperty]
        public List<EImpuestosArticulo> Impuestos { get; private set; }

        public EArticulo()
        {
            Id = "";
            CodigoImpresion = "";
            DescripcionCorta = "";
            DescripcionLarga = "";
            PrecioVenta1 = 0;
            PesoRequerido = false;
            Descodificado = false;
            CodigosPorArticulo = null;
            Impuesto1 = 0;
        }

        public EArticulo(string id, string codigoImpresion, string descripcionLarga,string descripcionCorta, decimal precioVenta1, bool requierePeso, bool descodificado, double impuesto1)
        {
            Id = id;
            CodigoImpresion = codigoImpresion;
            DescripcionCorta = descripcionCorta;
            DescripcionLarga = descripcionLarga;
            PrecioVenta1 = precioVenta1;
            PesoRequerido = requierePeso;
            Descodificado = descodificado;
            CodigosPorArticulo = null;
            Impuesto1 = (decimal)impuesto1;
        }

        public override string ToString()
        {
            return string.Format(
                "EArticulo [Id:{0}, CodigoImpresion {1}, Descripcion:{2}, PrecioVenta1:{3},PesoRequerido:{4},Descodificado: {5}]",
                Id,
                CodigoImpresion,
                DescripcionLarga,
                PrecioVenta1,
                PesoRequerido,
                Descodificado
                );
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            EArticulo objArticulo = obj as EArticulo;
            if (objArticulo == null)
                return false;
            else
                return Equals(objArticulo);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        // TODO : validar de se debe usar CodigoImpresion como identificador unico, versus Id.
        public bool Equals(EArticulo item)
        {
            if (item == null)
                return false;
            return (String.Equals(this.CodigoImpresion, item.CodigoImpresion, StringComparison.Ordinal));
        }

        public void ListaImpuestos(List<EImpuestosArticulo> impuestos)
        {
            this.Impuestos = impuestos;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        /**
        * ToDo: Implementar.
        * */
        /*EArticulo Copia()
        {
            return new EArticulo(
                string.Copy(this.Id),
                string.Copy(this.CodigoImpresion),
                string.Copy(this.DescripcionLarga),
                PrecioVenta1);
        }
        */
    }
}
