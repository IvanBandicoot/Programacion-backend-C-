using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;
using FluentValidation.AspNetCore;
using WebAPI.Middleware;
using Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Authentication;
using Aplicacion.Contratos;
using Seguridad.TokenSeguridad;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using AutoMapper;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //Confifuration tiene acceso al appsettings.json para usar la definicion que hicimos de la conexion a la base de datos
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {   //Se especifica la descripcion de la conexion con el servidor, en este caso, la base de datos
            services.AddDbContext<CursosOnlineContext>(opt => {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            services.AddMediatR(typeof(Consulta.Manejador).Assembly); //DECLARAR CONSTRUCTOR PARA LA CONSULTA GENERAL
            
            services.AddControllers();

            //Esta libreria nos permite validar de manera eficiente y tenemos que especifiar a que clase debe validar desde la aplicacion
            services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation( cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());
                            
    
            //registrar usuario
            //variable que represente la instancia de la clase usuario de dominio
            var builder = services.AddIdentityCore<Usuario>();
            //crear objeto de tipo identity builder
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);
            //vamos agregarle a identityBuilder la instancia de entityframework
            identityBuilder.AddEntityFrameworkStores<CursosOnlineContext>();
            //hay que indicar al identity que manejara el login
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();
            services.TryAddSingleton<ISystemClock, SystemClock>();

            //agregar seguridad a los controladores
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            services.AddAuthentication(Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            //Inyectar la interfaz y su clase que cumple su contrato
            services.AddScoped<IJwtGenerador, JWTGenerador>();
            services.AddScoped<IUsuarioSesion, UsuarioSesion>();
            //agregamos la interfaz del mapp
            services.AddAutoMapper(typeof(Consulta.Manejador));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //USAMOS EL MANEJADOR MIDDLEWARE QUE CREAMOS PERO ANTES COMENTADOS LA EXCEPTION QUE ESTA DENTRO DEL IF
            app.UseMiddleware<ManejadorErrorMiddleware>();
            if (env.IsDevelopment())
            {
                // app.UseDeveloperExceptionPage()
            }
            //es para ambiente de produccion y se necesita pagar un certificado de seguridad
            // app.UseHttpsRedirection();

            //ahora tenemeos que especificar la seguridad de los controles con el token que acabamos de agregar en configureservices
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
