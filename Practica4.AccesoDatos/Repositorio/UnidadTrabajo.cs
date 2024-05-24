using Practica4.AccesoDatos.Repositorio;
using Practica4.AccesoDatos.Repositorio.IRepositorio;
using Practica4.AccesoDatos.Data;
using Practica4.AccesoDatos.Repositorio.IRepositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.AccesoDatos.Repositorio
{
    public class UnidadTrabajo : IUnidadTrabajo
    {
        private readonly ApplicationDbContext _db;
        public ICategoriaRepositorio Categoria { get; private set; }
        public IServicioRepositorio Servicio { get; private set; }
        public UnidadTrabajo(ApplicationDbContext db)
        {
            _db = db;
            Categoria = new CategoriaRepositorio(db);
            Servicio = new ServicioRepositorio(db);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public async Task Guardar()
        {
            await _db.SaveChangesAsync();
        }
    }
}
