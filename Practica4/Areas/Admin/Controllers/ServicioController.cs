using Microsoft.AspNetCore.Mvc;
using Practica4.AccesoDatos.Repositorio.IRepositorio;
using Practica4.Modelos;
using Practica4.Modelos.ViewModels;
using Practica4.Utilidades;

namespace Practica4.Areas.Admin.Controllers
{
    [Area("Admin")] //Le decimos a que Area pertecene
    public class ServicioController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ServicioController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        //Método get del upsert
        public async Task<IActionResult>Upsert(int? id)
        {
            ServicioVM servicioVM = new ServicioVM()
            {
                Servicio = new Servicio(),
                CategoriaLista = _unidadTrabajo.Servicio.ObtenerTodosDropdownList("Categoria"),
                PadreLista=_unidadTrabajo.Servicio.ObtenerTodosDropdownList("Servicio")
            };
            if (id == null)
            {
                //Crear nuevo Producto
                return View(servicioVM);
            }
            else
            {
                //Vamos a actualizar a producto
                servicioVM.Servicio = await _unidadTrabajo.Servicio.obtener(id.GetValueOrDefault());
                if (servicioVM.Servicio == null)
                {
                    return NotFound();
                }
                return View(servicioVM);
            }
        }



        #region API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ServicioVM servicioVM)
        {
            if(ModelState.IsValid)
            {
                //Definir la variable para obtener los archivos desde el formulario
                //En este caso es una sola imagen
                var files = HttpContext.Request.Form.Files;
                //Definir una variable para contruir la ruta del directorio wwwroot
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (servicioVM.Servicio.Id == 0)
                {
                    //Creacion de un producto nuevo
                    //Definir la ruta completa de donde se guardara la imagen
                    string upload = webRootPath + DS.ImagenRuta;
                    //Crear un ID unico de la imagen
                    string fileName=Guid.NewGuid().ToString();
                    //Crear la variable con el tipo de archivo de mi maquina (extension)
                    string extension = Path.GetExtension(files[0].FileName);
                    //Ahora habilitamos el manejo del streaming de los archivos 
                    using(var fileStream=new FileStream(Path.Combine(upload,fileName+extension),FileMode.Create))
                    {
                        //Copiar el archivo de la memoria del navegador a la carpeta del servidor
                        files[0].CopyTo(fileStream);
                    }
                    //Asignamos el nombre de la imagen del producto hacia su registro
                    servicioVM.Servicio.ImagenUrl = fileName + extension;
                    //Preparamos al modelo para ser guardado
                    await _unidadTrabajo.Servicio.Agregar(servicioVM.Servicio);
                }
                else
                {
                    //Actualizar un Producto existente
                    //Hacemos la consulta del registro a modificar
                    var objServicio = await _unidadTrabajo.Servicio.obtenerPrimero(p=>p.Id== servicioVM.Servicio.Id,
                        isTracking:false);
                    //Saber si el usuario eligio otra imagen
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        //Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objServicio.ImagenUrl);
                        //Verificar si la imagen existe
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        //Creamos la nueva imagen
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        //Asignamos la nueva imagen al registro del producto
                        servicioVM.Servicio.ImagenUrl = fileName + extension;
                    }
                    //Si no se eligio otra imagen
                    else
                    {
                        servicioVM.Servicio.ImagenUrl = objServicio.ImagenUrl;
                    }
                    _unidadTrabajo.Servicio.Actualizar(servicioVM.Servicio);
                }
                TempData[DS.Exitosa] = "Servicio registrado con éxito";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            //Si el modelState es inválido
            TempData[DS.Error] = "Algo salió MAL :(";
            servicioVM.CategoriaLista = _unidadTrabajo.Servicio.ObtenerTodosDropdownList("Categoria");
            servicioVM.PadreLista = _unidadTrabajo.Servicio.ObtenerTodosDropdownList("Servicio");
            return View(servicioVM);
        }

        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos=await _unidadTrabajo.Servicio.obtenerTodos(incluirPropiedades:"Categoria");
            return Json(new {data=todos});  //data es el nombre que tiene que tener la tabla por defecto para crear el JSON
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var servicioDB = await _unidadTrabajo.Servicio.obtener(id);
            if (servicioDB == null)
            {
                return Json(new { success = false, message = "Error al borrar Servicio." });
            }
            //Eliminar la imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, servicioDB.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }
            _unidadTrabajo.Servicio.Remover(servicioDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Servicio borrado exitósamente." });
        }


        [ActionName("ValidarNombre")]
        public async Task<IActionResult>ValidarNombre(string serie,int id = 0)
        {
            bool valor=false;
            var lista = await _unidadTrabajo.Servicio.obtenerTodos();
            if (id == 0)
            {
                valor=lista.Any(b=>b.Nombre.ToLower().Trim()== serie.ToLower().Trim());
            }else
            {
                valor = lista.Any(b=>b.Nombre.ToLower().Trim()== serie.ToLower().Trim() && b.Id!=id);
            }
            if (valor)
            {
                return Json(new {success=true});
            }
			return Json(new { success = false });
		}

        #endregion
    }
}
