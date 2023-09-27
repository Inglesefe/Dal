using Dal.Dto;
using Entities;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades
    /// </summary>
    public abstract class PersistentBase<T> : IPersistent<T> where T : Entity
    {
        #region Attributes
        /// <summary>
        /// Conexión a la base de datos
        /// </summary>
        protected readonly string _connString;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        protected PersistentBase(string connString)
        {
            _connString = connString;
            Mapper.GetInstance();
        }
        #endregion

        #region Methods
        /// <inheritdoc />
        public abstract ListResult<T> List(string filters, string orders, int limit, int offset);

        /// <inheritdoc />
        public abstract T Read(T entity);

        /// <inheritdoc />
        public abstract T Insert(T entity);

        /// <inheritdoc />
        public abstract T Update(T entity);

        /// <inheritdoc />
        public abstract T Delete(T entity);
        #endregion
    }
}
