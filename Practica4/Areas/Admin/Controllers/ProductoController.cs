using Microsoft.AspNetCore.Mvc;
using Practica4.AccesoDatos.Repositorio.IRepositorio;
using Practica4.Modelos;
using Practica4.Modelos.ViewModels;
using Practica4.Utilidades;

namespace Practica4.Areas.Admin.Controllers
{
    [Area("Admin")] //Le decimos a que Area pertecene
    public class ProductoController : Controller
    {
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
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
            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca"),
                PadreLista=_unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto")
            };
            if (id == null)
            {
                //Crear nuevo Producto
                return View(productoVM);
            }
            else
            {
                //Vamos a actualizar a producto
                productoVM.Producto = await _unidadTrabajo.Producto.obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }
        }



        #region API
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            if(ModelState.IsValid)
            {
                //Definir la variable para obtener los archivos desde el formulario
                //En este caso es una sola imagen
                var files = HttpContext.Request.Form.Files;
                //Definir una variable para contruir la ruta del directorio wwwroot
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (productoVM.Producto.Id == 0)
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
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    //Preparamos al modelo para ser guardado
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    //Actualizar un Producto existente
                    //Hacemos la consulta del registro a modificar
                    var objProducto = await _unidadTrabajo.Producto.obtenerPrimero(p=>p.Id==productoVM.Producto.Id,
                        isTracking:false);
                    //Saber si el usuario eligio otra imagen
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        //Borrar la imagen anterior
                        var anteriorFile = Path.Combine(upload, objProducto.ImagenUrl);
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
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }
                    //Si no se eligio otra imagen
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }
                TempData[DS.Exitosa] = "Producto registrado con éxito";
                await _unidadTrabajo.Guardar();
                return View("Index");
            }
            //Si el modelState es inválido
            TempData[DS.Error] = "Algo salió MAL :(";
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropdownList("Producto");
            return View(productoVM);
        }

        [HttpGet]
        public async Task<IActionResult> obtenerTodos()
        {
            var todos=await _unidadTrabajo.Producto.obtenerTodos(incluirPropiedades:"Categoria,Marca");
            return Json(new {data=todos});  //data es el nombre que tiene que tener la tabla por defecto para crear el JSON
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDB = await _unidadTrabajo.Producto.obtener(id);
            if (productoDB == null)
            {
                return Json(new { success = false, message = "Error al borrar Producto." });
            }
            //Eliminar la imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload,productoDB.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }
            _unidadTrabajo.Producto.Remover(productoDB);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitósamente." });
        }


        [ActionName("ValidarNombre")]
        public async Task<IActionResult>ValidarNombre(string serie,int id = 0)
        {
            bool valor=false;
            var lista = await _unidadTrabajo.Producto.obtenerTodos();
            if (id == 0)
            {
                valor=lista.Any(b=>b.NumeroSerie.ToLower().Trim()== serie.ToLower().Trim());
            }else
            {
                valor = lista.Any(b=>b.NumeroSerie.ToLower().Trim()== serie.ToLower().Trim() && b.Id!=id);
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
