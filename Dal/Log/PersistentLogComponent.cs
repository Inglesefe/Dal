using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Log;
using System.Data;

namespace Dal.Log
{
    /// <summary>
    /// Realiza la persistencia de los registros de componentes en la base de datos
    /// </summary>
    public class PersistentLogComponent : IPersistentBase<LogComponent>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        public PersistentLogComponent() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de registros de componentes desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de registros</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los registros</exception>
        public ListResult<LogComponent> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<LogComponent> result;
            try
            {
                QueryBuilder queryBuilder = new("idlog AS id, date, type, component, description, iduser as user", "log_component");
                using (connection)
                {
                    List<LogComponent> logs = connection.Query<LogComponent>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(logs, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de registros", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta un registro dado su identificador
        /// </summary>
        /// <param name="entity">Registro a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Registro con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el registro</exception>
        public LogComponent Read(LogComponent entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    LogComponent result = connection.QuerySingleOrDefault<LogComponent>(
                        "SELECT idlog AS id, date, type, component, description, iduser as user FROM log_component WHERE idlog = @Id",
                        entity);
                    if (result == null)
                    {
                        entity = new();
                    }
                    else
                    {
                        entity = result;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el registro", ex);
            }
            return entity;
        }

        /// <summary>
        /// Inserta un registro en la base de datos
        /// </summary>
        /// <param name="entity">Registro a insertar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Registro insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el registro</exception>
        public LogComponent Insert(LogComponent entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingle<long>(
                        "INSERT INTO log_component (date, type, component, description, iduser) VALUES (NOW(), @Type, @Component, @Description, @User); SELECT LAST_INSERT_ID();",
                        entity);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el registro", ex);
            }
            return entity;
        }

        /// <summary>
        /// No hace nada con la actualización de un log
        /// </summary>
        /// <param name="entity">Log a actualizar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Log actualizado</returns>
        public LogComponent Update(LogComponent entity, IDbConnection connection)
        {
            return entity;
        }

        /// <summary>
        /// No hace nada con la eliminación de un log
        /// </summary>
        /// <param name="entity">Log a eliminar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Log eliminado</returns>
        public LogComponent Delete(LogComponent entity, IDbConnection connection)
        {
            return entity;
        }
        #endregion
    }
}