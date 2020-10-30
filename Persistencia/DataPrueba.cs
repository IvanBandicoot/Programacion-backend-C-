using Dominio;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistencia
{
    public class DataPrueba
    {
        public static async Task InsertarData(CursosOnlineContext conetext, UserManager<Usuario> usuarioManager)
        {
            if (!usuarioManager.Users.Any())
            {
                var usuario = new Usuario {NombreCompleto = "Ivan Moreno", UserName = "Ivan.Moreo", Email = "ivanalejandro1996@gmail.com" };
                await usuarioManager.CreateAsync(usuario, "Gorila13$");
            }
        }
    }
}
