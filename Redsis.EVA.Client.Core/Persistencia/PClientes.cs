using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PClientes
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EClientes GetAllClientes()
        {
            var repositorio = new RCliente();
            var clientes = new EClientes();
            var registros = repositorio.GetAllClientes();
            foreach (DataRow registro in registros.Rows)
            {
                var cliente = ClienteUtil.InstanciarDesde(registro);
                clientes.ListaClientes.Add(cliente);
            }
            return clientes;
        }

        public void GuardarCliente(ECliente cliente)
        {
            var rcliente = new RCliente();
            if (rcliente.CrearCliente(cliente.Codigo, 0, 0, cliente.Tipo) == 1)
            {
                rcliente.CrearDireccion(0, cliente.Ciudad, "00", "00", cliente.Direccion, "", "", cliente.Departamento, cliente.Pais);
                rcliente.CrearCliente(cliente.Codigo, 0, 0, cliente.Tipo);
                rcliente.CrearPersona(0, "", "", "00", "", null, cliente.PrimerApellido, cliente.PrimerNombre, cliente.SegundoApellido, cliente.SegundoNombre, "", cliente.TelefonoResidencia, "", cliente.Tipo, "", "");
            }
            else
            {
                log.Error("[GuardarCliente]: No pudo ser guardado el cliente.");
            }
        }
    }

    public class ClienteUtil
    {
        public static ECliente InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ApplicationException("Registro nulo o contiene campos nulos.");
            }
            var a = new ECliente(
                ((string)registro["id_cliente"]).Trim(),
                (string)registro["cod_cliente"],
                (string)registro["tipo_cliente"],
                (string)registro["tel_residencia"],
                (string)registro["primer_nombre"],
                (string)registro["segundo_nombre"],
                (string)registro["primer_apellido"],
                (string)registro["segundo_apellido"],
                (string)registro["dir1"],
                (string)registro["pais"],
                (string)registro["dpto"],
                (string)registro["ciudad"]
                );
            return a;
        }
    }
}
