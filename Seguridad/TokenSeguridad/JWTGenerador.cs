using Aplicacion.Contratos;
using Dominio;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Seguridad.TokenSeguridad
{
    public class JWTGenerador : IJwtGenerador
    {
        public string CrearToker(Usuario usuario)
        {
            //paso1: claims es la data del usuario que tu quieres compartir con tus clientes
            var claims = new List<Claim> {
                    new Claim(JwtRegisteredClaimNames.NameId, usuario.UserName)
            };
            //paso2: key es crear las credenciales del acceso
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            //paso3: crear las credenciales
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            //paso4: crear la descripcion del token
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(30), //tiempo del token
                SigningCredentials = credenciales,//acceso a las credenciales
            };
            //paso5: agregar un tokenHandler para describirlo
            var tokenManejador = new JwtSecurityTokenHandler();
            var token = tokenManejador.CreateToken(tokenDescription);

            //paso 6:
            return tokenManejador.WriteToken(token);

        }
    }
}
