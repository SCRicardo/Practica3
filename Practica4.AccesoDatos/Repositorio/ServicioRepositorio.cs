using Microsoft.AspNetCore.Mvc.Rendering;
using Practica4.AccesoDatos.Data;
using Practica4.AccesoDatos.Repositorio.IRepositorio;
using Practica4.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.AccesoDatos.Repositorio
{
    public class ServicioRepositorio : Repositorio<Servicio>, IServicioRepositorio
    {
        private readonly ApplicationDbContext _db;
        public ServicioRepositorio(ApplicationDbContext db): base(db) //heredan todo eso a sus papas
        {
            _db = db; 
        }
        public void Actualizar(Servicio servicio)
        {
            var servicioBD = _db.Servicios.FirstOrDefault(b => b.Id == servicio.Id);
            if (servicioBD != null)
            {
                if (servicioBD.ImagenUrl != null)
                {
                    servicioBD.ImagenUrl = servicio.ImagenUrl;
                }
                servicioBD.Nombre = servicio.Nombre;
                servicioBD.Descripcion= servicio.Descripcion;
                servicioBD.CategoriaId = servicio.CategoriaId;
                servicioBD.PadreId = servicio.PadreId;
                servicioBD.Estado= servicio.Estado;
                _db.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> ObtenerTodosDropdownList(string obj)
        {
            if (obj == "Categoria")
            {
                return _db.Categorias.Where(c => c.Estado == true).Select(c => new SelectListItem {
                    Text =c.Nombre,
                    Value=c.Id.ToString()
                });      
            }
            if (obj == "Servicio")
            {
                return _db.Servicios.Where(c => c.Estado == true).Select(c => new SelectListItem
                {
                    Text = c.Descripcion,
                    Value = c.Id.ToString()
                });
            }
            return null;
        }
    }
}
