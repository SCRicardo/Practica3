using Practica4.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.AccesoDatos.Repositorio.IRepositorio
{
    public interface IUnidadTrabajo : IDisposable
    {
        ICategoriaRepositorio Categoria { get; }
        IServicioRepositorio Servicio { get;}
        Task Guardar();
    }
}
