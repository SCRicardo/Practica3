using Microsoft.AspNetCore.Mvc.Rendering;
using Practica4.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.AccesoDatos.Repositorio.IRepositorio
{
    public interface IServicioRepositorio : IRepositorio<Servicio>
    {
        void Actualizar(Servicio servicio);
        IEnumerable<SelectListItem>ObtenerTodosDropdownList(string obj);
    }
}
