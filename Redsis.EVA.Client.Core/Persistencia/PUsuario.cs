using Redsis.EVA.Client.Common;
using Redsis.EVA.Client.Core.Entidades;
using Redsis.EVA.Client.Core.Repositorio;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Redsis.EVA.Client.Core.Persistencia
{
    public class PUsuario
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public EUsuario Autenticar(string idUsuario, string clave, out Respuesta respuesta)
        {
            EUsuario usuario = null;
            respuesta = new Respuesta(false);

            var repositorio = new RUsuario();
            //
            //idUsuario = "";
            //
            var registro = repositorio.BuscarUsuarioPorIdentificacion(idUsuario);
            if (registro == null)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = "Usuario no encontrado.";
            }
            else
            {
                usuario = UsuarioUtil.InstanciarDesde(registro);
                bool autenticado = usuario.Autenticar(clave);
                if (!autenticado)
                {
                    log.Info("[PUsuario.Autenticar] Clave de usuario no válida");
                    //
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Clave de usuario no válida.";
                }
                else
                {
                    respuesta.Valida = true;
                }
            }

            return usuario;
        }

        public EUsuario ValidarClaveSupervisor(string idUsuario, string clave, out Respuesta respuesta)
        {
            EUsuario usuario = new EUsuario();
            respuesta = new Respuesta();
            try
            {
                var repositorio = new RUsuario();

                var registro = repositorio.BuscarUsuarioPorClaveSupervisor(idUsuario, clave);
                if (registro == null)
                {
                    respuesta.Valida = false;
                    respuesta.Mensaje = "Usuario no encontrado.";
                }
                else
                {
                    respuesta.Valida = true;
                    respuesta.Mensaje = "OK";
                }
            }
            catch (Exception ex)
            {
                respuesta.Valida = false;
                respuesta.Mensaje = $"[ValidarClaveSupervisor] {ex.Message}";
                throw;
            }
            return usuario;
        }
    }
    public class UsuarioUtil
    {
        public static EUsuario InstanciarDesde(DataRow registro)
        {
            if (registro == null)
            {
                throw new ArgumentNullException("Datos de usuario nulos o vacíos.");
            }

            var a = new EUsuario(
                (string)registro["id_usuario"],
                (string)registro["usuario"],
                (string)registro["nombre"],
                (string)registro["apellido"],
                (string)registro["password"],
                (string)registro["token_supervisor"]);
            return a;
        }
    }
}
