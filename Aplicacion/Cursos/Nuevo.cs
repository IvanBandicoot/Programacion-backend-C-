using System;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Persistencia;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using System.Collections.Generic;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        //CLASE CABECERA
        public class Ejecuta : IRequest
        {
            // [Required(ErrorMessage="Por favor ingrese el Titulo del curso")]

            public string Titulo {get;set;}
            
            public string Descripcion {get;set;}
            
            public DateTime? FechaPublicacion {get;set;}

            public List<Guid> ListaInstructor { get; set; }
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

        // CLASE DE TRANSACCION
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context)
            {
                this._context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Se agregar los parametros al objeto
                Guid _cursoId = Guid.NewGuid();

                var curso = new Curso {
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion
                };

                _context.Curso.Add(curso); //Agrega el curso al context que esta instanciada a la conexion con la BD

                //llamar a la lista de instructores
                if (request.ListaInstructor!=null)
                {
                    foreach(var id in request.ListaInstructor)
                    {
                        var cursoInstructor = new CursoInstructor
                        {
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        _context.CursoInstructor.Add(cursoInstructor);
                    }
                }

                var valor = await _context.SaveChangesAsync(); //devuelve el estado de la transaccion
                
                if(valor > 0)
                {
                    return Unit.Value;
                }

                throw new Exception("No se pudo insertar el curso");
                
            }
        }
    }
}