using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicacion.Seguridad
{
    //ESTA CLASE ES PARA FILTRAR LOS DATOS DEL USUARIO DESPUES DE HACER LOGIN
    public class UsuarioData
    {
        public string NombreCompleto { get; set; }
        public string Token { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Imagen { get; set; }
    }
}
