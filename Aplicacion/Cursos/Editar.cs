using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        public class Ejecuta : IRequest
        {
            public Guid CursoId {get;set;}
            public string Titulo {get;set;}
            public string Descripcion {get;set;}
            public DateTime? FechaPublicacion {get;set;}
            public List<Guid> ListaInstructores { get; set; }
        }
        
        //CLASE VALIDACION
        public class EjecutaValidacion : AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                this._context = context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var curso = await  _context.Curso.FindAsync(request.CursoId);

                if(curso == null)
                {
                    //throw new Exception("No se pudo eliminar el curso");
                    //PONEMOS LA EXCEPTION QUE CREAMOS EN LA CARPETA MANEJADORERROR
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {Mensaje = "No se encontro el curso al actualizar"});
                }

                curso.Titulo = request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;

                if (request.ListaInstructores != null)
                {
                    if (request.ListaInstructores.Count > 0)
                    {
                        //Eliminar los instructores actuales del curso en la base de datos
                        var instructorBD = _context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();
                        foreach (var instructorEliminar in instructorBD)
                        {
                            _context.CursoInstructor.Remove(instructorEliminar);
                        }
                        //Fin del procedimiento para eliminar instructores

                        //El procedimiento para agregar instructores que provienen del cliente
                        foreach (var id in request.ListaInstructores)
                        {
                            var nuevoInstructor = new CursoInstructor
                            {
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            _context.CursoInstructor.Add(nuevoInstructor);
                        }
                        //Fin del procedimiento
                    }
                }
                
                var valor = await _context.SaveChangesAsync();
            
                if(valor > 0)
                {
                    return Unit.Value;
                }
                throw new Exception("No se guardaron los cambios en el curso");
            
            }
        }
    }
}