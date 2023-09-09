using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Log;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Log
{
    /// <summary>
    /// Realiza la persistencia de los registros de componentes en la base de datos
    /// </summary>
    public class PersistentLogComponent : PersistentBase<LogComponent>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentLogComponent(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de registros de componentes desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de registros</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los registros</exception>
        public override ListResult<LogComponent> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idlog, date, type, controller, method, input, output, iduser", "log_component");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<LogComponent> logs = connection.Query<LogComponent>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(logs, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de registros", ex);
            }
        }

        /// <summary>
        /// Consulta un registro dado su identificador
        /// </summary>
        /// <param name="entity">Registro a consultar</param>
        /// <returns>Registro con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el registro</exception>
        public override LogComponent Read(LogComponent entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                LogComponent result = connection.QuerySingleOrDefault<LogComponent>(
                    "SELECT idlog, date, type, controller, method, input, output, iduser FROM log_component WHERE idlog = @Id",
                    entity);
                if (result == null)
                {
                    entity = new();
                }
                else
                {
                    entity = result;
                }
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el registro", ex);
            }
        }

        /// <summary>
        /// Inserta un registro en la base de datos
        /// </summary>
        /// <param name="entity">Registro a insertar</param>
        /// <returns>Registro insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el registro</exception>
        public override LogComponent Insert(LogComponent entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<long>(
                    "INSERT INTO log_component (date, type, controller, method, input, output, iduser) VALUES (NOW(), @Type, @Controller, @Method, @Input, @Output, @User); SELECT LAST_INSERT_ID();",
                    entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el registro", ex);
            }
        }

        /// <summary>
        /// No hace nada con la actualización de un log
        /// </summary>
        /// <param name="entity">Log a actualizar</param>
        /// <returns>Log actualizado</returns>
        public override LogComponent Update(LogComponent entity)
        {
            return entity;
        }

        /// <summary>
        /// No hace nada con la eliminación de un log
        /// </summary>
        /// <param name="entity">Log a eliminar</param>
        /// <returns>Log eliminado</returns>
        public override LogComponent Delete(LogComponent entity)
        {
            return entity;
        }
        #endregion
    }
}