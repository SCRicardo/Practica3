using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.Modelos.ViewModels
{
    public class ServicioVM
    {
        public Servicio Servicio { get; set; }
        public IEnumerable<SelectListItem> CategoriaLista { get; set; } //IEnumerable es el tipo de dato para Listas
        public IEnumerable<SelectListItem> PadreLista { get; set; }
    }
}
