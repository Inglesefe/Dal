using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Config;
using System.Data;

namespace Dal.Config
{
    /// <summary>
    /// Realiza la persistencia de los parámetros en la base de datos
    /// </summary>
    public class PersistentParameter : PersistentBaseWithLog, IPersistentWithLog<Parameter>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentParameter() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de parámetros desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de parámetros</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los parámetros</exception>
        public ListResult<Parameter> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<Parameter> result;
            try
            {
                QueryBuilder queryBuilder = new("idparameter AS id, name, value", "parameter");
                using (connection)
                {
                    List<Parameter> countries = connection.Query<Parameter>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(countries, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de parámetros", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta un parámetro dado su identificador
        /// </summary>
        /// <param name="entity">Parámetro a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Parámetro con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el parámetro</exception>
        public Parameter Read(Parameter entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    Parameter result = connection.QuerySingleOrDefault<Parameter>("SELECT idparameter AS id, name, value FROM parameter WHERE idparameter = @Id", entity);
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
                throw new PersistentException("Error al consultar el parámetro", ex);
            }
            return entity;
        }

        /// <summary>
        /// Inserta un parámetro en la base de datos
        /// </summary>
        /// <param name="entity">Parámetro a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Parámetro insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el parámetro</exception>
        public Parameter Insert(Parameter entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingle<int>("INSERT INTO parameter (name, value) VALUES (@Name, @Value); SELECT LAST_INSERT_ID();", entity);
                    LogInsert(entity.Id, "parameter", "INSERT INTO parameter (name, value) VALUES ('" + entity.Name + "', '" + entity.Value + "')", user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el parámetro", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un parámetro en la base de datos
        /// </summary>
        /// <param name="entity">Parámetro a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Parámetro actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el parámetro</exception>
        public Parameter Update(Parameter entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("UPDATE parameter SET name = @Name, value = @Value WHERE idparameter = @Id", entity);
                    LogUpdate(entity.Id, "parameter", "UPDATE parameter SET name = '" + entity.Name + "', value = '" + entity.Value + "' WHERE idparameter = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el parámetro", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina un parámetro de la base de datos
        /// </summary>
        /// <param name="entity">Parámetro a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Parámetro eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el parámetro</exception>
        public Parameter Delete(Parameter entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM parameter WHERE idparameter = @Id", entity);
                    LogDelete(entity.Id, "parameter", "DELETE FROM parameter WHERE idparameter = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el parámetro", ex);
            }
            return entity;
        }
        #endregion
    }
}
