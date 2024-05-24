using Practica4.Modelos.Especificaciones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Practica4.AccesoDatos.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        Task<T> obtener(int id);
        Task<IEnumerable<T>> obtenerTodos(
            Expression<Func<T, bool>> filtro = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string incluirPropiedades = null,
            bool isTracking = true
            );
        PagesList<T> ObtenerTodosPaginado(Parametros parametros,
    Expression<Func<T, bool>> filtro = null, //Sirve para ver si viene o no informacion
    Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, //Sirve para Ordenar
    string incluirPropiedades = null,
    bool isTracking = true  //Sirve para reservar un pequeño espacio en memoria }
    );
        Task<T> obtenerPrimero(
            Expression<Func<T, bool>> filtro = null,
            string incluirPropiedades = null,
            bool isTracking = true
            );
        Task Agregar(T entidad);
        void Remover(T entidad);
        void RemoverRango(IEnumerable<T> entidad);

    }
}
