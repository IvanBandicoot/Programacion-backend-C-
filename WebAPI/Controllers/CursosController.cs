using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //http://localhost:5000/api/Cursos
    [Route("api/[Controller]")]
    [ApiController]
    public class CursosController : MiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<CursoDto>>> Get()
        {
            return await Mediator.Send(new Consulta.ListaCursos());
        }

        // http://localhost:5000/api/Cursos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> Detalle(Guid id)
        {
            return await Mediator.Send(new ConsultaId.CursosUnico{Id = id});
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
            return await Mediator.Send(data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id, Editar.Ejecuta data)
        {
            data.CursoId = id;
            return await Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await Mediator.Send(new Eliminar.Ejecuta{Id = id});
        }
    }
}