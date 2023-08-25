using Dal.Dto;
using Entities;
using Entities.Auth;
using System.Data;

namespace Dal
{
    /// <summary>
    /// Define los métodos que debe tener toda persistencia
    /// </summary>
    public interface IPersistentWithLog<T> where T : EntityBase
    {
        /// <summary>
        /// Trae un listado de registros aplicando filtros, ordenamientos y límites de registros a traer
        /// </summary>
        /// <param name="filters">Filtros a aplicar en la consulta</param>
        /// <param name="orders">Ordenamientos a aplicar en la consulta</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Registro inicial desde el que se cuenta el límite de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de registros y el total que traería sin aplicar límites</returns>
        ListResult<T> List(string filters, string orders, int limit, int offset, IDbConnection connection);

        /// <summary>
        /// Trae una entidad con los datos cargados desde la base de datos
        /// </summary>
        /// <param name="entity">Entidad a cargar dado su identificador en la base de datos</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad con los datos cargados desde la base de datos</returns>
        T Read(T entity, IDbConnection connection);

        /// <summary>
        /// Inserta una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a insertar en la base de datos</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad insertada en la base de datos, con el id generado en ella si aplica</returns>
        T Insert(T entity, User user, IDbConnection connection);

        /// <summary>
        /// Actualiza una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a actualizar en la base de datos</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad actualizada en la base de datos</returns>
        T Update(T entity, User user, IDbConnection connection);

        /// <summary>
        /// Elimina una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar en la base de datos</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Entidad eliminada en la base de datos</returns>
        T Delete(T entity, User user, IDbConnection connection);
    }
}
