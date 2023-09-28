using Entities;

namespace Dal
{
    /// <summary>
    /// Define los métodos que debe tener toda persistencia
    /// </summary>
    public interface IPersistent<T> : IPersistentRead<T> where T : Entity
    {
        /// <summary>
        /// Inserta una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a insertar en la base de datos</param>
        /// <returns>Entidad insertada en la base de datos, con el id generado en ella si aplica</returns>
        T Insert(T entity);

        /// <summary>
        /// Actualiza una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a actualizar en la base de datos</param>
        /// <returns>Entidad actualizada en la base de datos</returns>
        T Update(T entity);

        /// <summary>
        /// Elimina una entidad en la base de datos
        /// </summary>
        /// <param name="entity">Entidad a eliminar en la base de datos</param>
        /// <returns>Entidad eliminada en la base de datos</returns>
        T Delete(T entity);
    }
}
