using Dal.Dto;
using Dapper;
using Entities;
using Entities.Log;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades
    /// </summary>
    public abstract class PersistentBase<T> : IPersistent<T> where T : IEntity
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
        /// <param name="connection">Cadena de conexión a la base de datos</param>
        protected PersistentBase(string connString)
        {
            _connString = connString;
            SqlMapper.SetTypeMap(typeof(LogComponent), new CustomPropertyTypeMap(typeof(LogComponent),
                (type, columnName) =>
                {
                    return columnName switch
                    {
                        "idlog" => type.GetProperty("Id"),
                        "date" => type.GetProperty("Date"),
                        "type" => type.GetProperty("Type"),
                        "controller" => type.GetProperty("Controller"),
                        "method" => type.GetProperty("Method"),
                        "input" => type.GetProperty("Input"),
                        "output" => type.GetProperty("Output"),
                        "iduser" => type.GetProperty("User"),
                        _ => null,
                    };
                }));
            SqlMapper.SetTypeMap(typeof(LogDb), new CustomPropertyTypeMap(typeof(LogDb),
                (type, columnName) =>
                {
                    return columnName switch
                    {
                        "idlog" => type.GetProperty("Id"),
                        "date" => type.GetProperty("Date"),
                        "action" => type.GetProperty("Action"),
                        "idtable" => type.GetProperty("IdTable"),
                        "table" => type.GetProperty("Table"),
                        "sql" => type.GetProperty("Sql"),
                        "iduser" => type.GetProperty("User"),
                        _ => null,
                    };
                }));
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
