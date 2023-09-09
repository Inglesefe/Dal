using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Config;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Admon
{
    /// <summary>
    /// Realiza la persistencia de los tipos de consecutivo contable en la base de datos
    /// </summary>
    public class PersistentConsecutiveType : PersistentBaseWithLog<ConsecutiveType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentConsecutiveType(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de consecutivo desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se consecutivo el número de registros</param>
        /// <returns>Listado de tipos de consecutivo</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los tipos de consecutivo</exception>
        public override ListResult<ConsecutiveType> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idconsecutivetype, name", "consecutive_type");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<ConsecutiveType> types = connection.Query<ConsecutiveType>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(types, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de tipos de consecutivo", ex);
            }
        }

        /// <summary>
        /// Consulta un tipo de consecutivo dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de consecutivo a consultar</param>
        /// <returns>Tipo de consecutivo con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el tipo de consecutivo</exception>
        public override ConsecutiveType Read(ConsecutiveType entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                ConsecutiveType result = connection.QuerySingleOrDefault<ConsecutiveType>("SELECT idconsecutivetype, name FROM consecutive_type WHERE idconsecutivetype = @Id", entity);
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
                throw new PersistentException("Error al consultar el tipo de consecutivo", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de consecutivo en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de consecutivo a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Tipo de consecutivo insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el tipo de consecutivo</exception>
        public override ConsecutiveType Insert(ConsecutiveType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO consecutive_type (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(
                    entity.Id,
                    "consecutive_type",
                    "INSERT INTO consecutive_type (name) VALUES ('" + entity.Name + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el tipo de consecutivo", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de consecutivo en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de consecutivo a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de consecutivo actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el tipo de consecutivo</exception>
        public override ConsecutiveType Update(ConsecutiveType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE consecutive_type SET name = @Name WHERE idconsecutivetype = @Id", entity);
                LogUpdate(
                    entity.Id,
                    "consecutive_type",
                    "UPDATE consecutive_type SET name = '" + entity.Name + "' WHERE idconsecutivetype = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el tipo de consecutivo", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de consecutivo de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de consecutivo a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de consecutivo eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el tipo de consecutivo</exception>
        public override ConsecutiveType Delete(ConsecutiveType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM consecutive_type WHERE idconsecutivetype = @Id", entity);
                LogDelete(entity.Id, "consecutive_type", "DELETE FROM account_type WHERE idconsecutivetype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el tipo de consecutivo", ex);
            }
        }
        #endregion
    }
}
