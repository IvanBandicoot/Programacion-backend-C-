using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursosUnico : IRequest<CursoDto>
        {
            public Guid Id {get;set;}
        }

        public class Manejador : IRequestHandler<CursosUnico, CursoDto>
        {
            private readonly CursosOnlineContext _context;
            private readonly IMapper _mapper;
            public Manejador(CursosOnlineContext context, IMapper mapper)
            {
                this._context = context;
                this._mapper = mapper;
            }

            public async Task<CursoDto> Handle(CursosUnico request, CancellationToken cancellationToken)
            {
                var curso = await _context.Curso.Include(x=>x.InstructoresLink).ThenInclude(y=>y.Instructor).FirstOrDefaultAsync(a => a.CursoId == request.Id);

                if(curso == null)
                {
                    //throw new Exception("No se pudo eliminar el curso");
                    //PONEMOS LA EXCEPTION QUE CREAMOS EN LA CARPETA MANEJADORERROR
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {Mensaje = "No se encontro el curso"});
                }

                var cursoDto = _mapper.Map<Curso, CursoDto>(curso);

                return cursoDto;
            }
        }
    }
}