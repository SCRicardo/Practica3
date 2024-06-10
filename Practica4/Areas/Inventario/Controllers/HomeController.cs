using Microsoft.AspNetCore.Mvc;
using Practica4.AccesoDatos.Repositorio.IRepositorio;
using Practica4.Modelos.ViewModels;
using Practica4.Modelos.Especificaciones;
using System.Diagnostics;


namespace Practica4.Areas.Inventario.Controllers
{
    [Area("Inventario")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnidadTrabajo _unidadTrabajo;

        public HomeController(ILogger<HomeController> logger, IUnidadTrabajo unidadTrabajo)
        {
            _logger = logger;
            _unidadTrabajo = unidadTrabajo;
        }

        public IActionResult Index(int pageNumber = 1, string busqueda = "", string busquedaActual = "")
        {
            if (!String.IsNullOrEmpty(busqueda)) //Feedback para no perder informacion
            {
                pageNumber = 1;
            }
            else
            {
                busqueda = busquedaActual;
            }
            ViewData["BusquedaActual"] = busqueda;
            if (pageNumber < 1) { pageNumber = 1; }
            Parametros parametros = new Parametros()
            {
                PageNumber = pageNumber,
                PageSize = 4 //El PageSize es la cantidad de productos que se va a mostrar, en este caso es 3
            };
            var resultado = _unidadTrabajo.Servicio.ObtenerTodosPaginado(parametros);
            if (!String.IsNullOrEmpty(busqueda))
            {
                resultado = _unidadTrabajo.Servicio.ObtenerTodosPaginado(parametros, p => p.Nombre.Contains(busqueda));
            }
            ViewData["TotalPaginas"] = resultado.MetaData.TotalPage;
            ViewData["TotalRegistros"] = resultado.MetaData.TotalCount;
            ViewData["PageSize"] = resultado.MetaData.PageSize;
            ViewData["PageNumber"] = pageNumber;
            ViewData["Previo"] = "disabled";
            ViewData["Siguiente"] = "";
            if (pageNumber > 1) { ViewData["Previo"] = ""; }
            if (resultado.MetaData.TotalPage <= pageNumber)
            {
                ViewData["Siguiente"] = "disabled";
            }
            return View(resultado);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
