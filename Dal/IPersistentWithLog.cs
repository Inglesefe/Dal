using Entities;
using Entities.Auth;

namespace Dal
{
    /// <summary>
    /// Define los métodos que debe tener toda persistencia
    /// </summary>
    public interface IPersistentWithLog<T> : IPersistentRead<T> where T : IEntity
    {
        /// <summary>
        /// Inserta una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a insertar en la base de datos</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Entidad insertada en la base de datos, con el id generado en ella si aplica</returns>
        T Insert(T entity, User user);

        /// <summary>
        /// Actualiza una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a actualizar en la base de datos</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Entidad actualizada en la base de datos</returns>
        T Update(T entity, User user);

        /// <summary>
        /// Elimina una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar en la base de datos</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Entidad eliminada en la base de datos</returns>
        T Delete(T entity, User user);
    }
}
