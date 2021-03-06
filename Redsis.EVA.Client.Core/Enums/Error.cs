﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Redsis.EVA.Client.Core.Enums
{
    public enum Errores
    {
        usuario_clave_incorrectos = 700,
        usuario_inactivo = 701,
        usuario_autenticado = 702,
        usuario_sesion_cerrada = 703,
        terminal_no_encontrada = 704,
        informacion_faltante = 705,
        formato_fecha_incorrecto = 707,
        num_factura_limite = 708,
        num_factura_fecha_limite = 709,
        num_factura_agotada = 710,
        banco_no_encontrado = 711,
        vendedor_no_encontrado = 712,
        medio_pago_no_encontrado = 713,
        descuento_no_encontrado = 714,
        descuento2_no_encontrado = 715,
        credito_no_encontrado = 716,
        articulo_no_encontrado = 717,
        imagen_art_no_encontrado = 718,
        transac_no_encontrado = 719,
        atriculo_no_anulado = 720,
        pago_no_anulado = 721,
        descuento_no_anulado = 722,
        no_agrega_convenio = 723,
        transac_ya_abierta = 724,
        imp_art_no_definido = 725,
        excede_limite_vueltas = 726,
        excede_porcentaje_vueltas = 727,
        medio_pago_no_vueltas = 728,
        no_medio_pago_efectivo = 729,
        cuenta_inactiva = 730,
        cupo_insuficiente = 731,
        no_copia_impresion = 732,
        valor_no_valido = 733,
        valor_negativo_cero = 734,
        cliente_no_encontrado = 735,
        no_porcentaje_imp =736,
        no_permisos_aut =737,
        clave_no_encontrada =738,
        descuento_mayor_total = 739,
        descuento_mayor_art = 740,
        descuento_pago_hecho = 741,
        art_no_descuento = 742,
        porcentaje_descuento_max = 743,
        descuento_por_item_max = 744,
        valor_max_excedido = 745,
        art_cantidad_decimal = 746,
        num_decimales_max = 747,
        convenio_no_encontrado = 748,
        subconven_no_encontrado = 749,
        medico_no_encontrado = 750,
        benefi_no_encontrado = 751,
        transac_valor_max = 752,
        art_no_convenio = 753,
        pendiente_no_encontrado = 754,
        no_genera_pendientes = 755,
        error_sevidor_impresion = 760,
        error_proceso_impresion = 761,
        no_conexion_impresion = 762,
        titulo_max_longitud = 770,
        primer_nombre_vacio = 771,
        primer_nombre_max_long = 772,
        segundo_nombre_vacio = 773,
        primer_apellido_vacio = 774,
        primer_apellido_max_long = 775,
        error_formato_correo = 776,
        codigo_cliente_vacio = 777,
        codigo_cliente_existente = 778,
        segundo_apellido_max_long = 779,
        sufijo_max_long = 780,
        tel_residencia_max_long = 781,
        tel_trabajo_max_long = 782,
        cel_personal_max_long = 783,
        cel_trabajo_max_long = 784,
        articulo_descodificado = 785,
        no_respuesta_servidor = 800,
        servidor_no_encontrado = 801,
        error_solicitud_servidor = 802,
        error_respuesta_servidor = 803,
        error_ejecucion = 900

    }
}
