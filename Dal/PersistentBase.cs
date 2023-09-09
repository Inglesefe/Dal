using Dal.Dto;
using Dapper;
using Entities;
using Entities.Log;
using System.Reflection;

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
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        protected PersistentBase(string connString)
        {
            _connString = connString;
            SqlMapper.SetTypeMap(typeof(LogComponent), new CustomPropertyTypeMap(typeof(LogComponent), GetMapColumnsLogComponent()));
            SqlMapper.SetTypeMap(typeof(LogDb), new CustomPropertyTypeMap(typeof(LogDb), GetMapColumnsLogDb()));
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


        /// <summary>
        /// Retorna el mapeo de las columnas para la clase LogComponent
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase LogComponent</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsLogComponent()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase LogDb
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase LogDb</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsLogDb()
        {
            return (type, columnName) =>
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
            };
        }
        #endregion
    }
}
