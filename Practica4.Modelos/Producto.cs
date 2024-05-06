using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.Modelos
{
    public class Producto
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "El número de serie es requerido")]
        [MaxLength(60, ErrorMessage = "EL maximo es de 60 caracteres")]
        public string NumeroSerie { get; set; }
        [Required(ErrorMessage = "La descripcion es requerido")]
        [MaxLength(100, ErrorMessage = "EL maximo es de 100 caracteres")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El precio es requerido")]
        public double Precio { get; set; }
        [Required(ErrorMessage = "El costo es requerido")]
        public double Costo { get; set; }
        public string ImagenUrl { get; set; }
        [Required(ErrorMessage = "El Estado es requerido")]
        public bool Estado { get; set; }

        //Realizar la relacion con la tabla Categoria y con la tabla Marca
        [Required(ErrorMessage = "La categoria es requerido")]
        public int CategoriaId { get; set; }
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; }

        [Required(ErrorMessage = "La marca es requerido")]
        public int MarcaId { get; set; }
        [ForeignKey("MarcaId")]
        public Marca Marca { get; set; }

        //Recursividad al padre
        public int? PadreId { get; set; }
        public virtual Producto Padre { get; set; }
    }
}
