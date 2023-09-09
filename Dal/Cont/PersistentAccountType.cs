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
    /// Realiza la persistencia de los tipos de cuenta contable en la base de datos
    /// </summary>
    public class PersistentAccountType : PersistentBaseWithLog<AccountType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentAccountType(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de cuenta desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de tipos de cuenta</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los tipos de cuenta</exception>
        public override ListResult<AccountType> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idaccounttype, name", "account_type");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<AccountType> types = connection.Query<AccountType>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(types, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de tipos de cuenta", ex);
            }
        }

        /// <summary>
        /// Consulta un tipo de cuenta dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de cuenta a consultar</param>
        /// <returns>Tipo de cuenta con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el tipo de cuenta</exception>
        public override AccountType Read(AccountType entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                AccountType result = connection.QuerySingleOrDefault<AccountType>("SELECT idaccounttype, name FROM account_type WHERE idaccounttype = @Id", entity);
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
                throw new PersistentException("Error al consultar el tipo de cuenta", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de cuenta a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Tipo de cuenta insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el tipo de cuenta</exception>
        public override AccountType Insert(AccountType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO account_type (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(
                    entity.Id,
                    "account_type",
                    "INSERT INTO account_type (name) VALUES ('" + entity.Name + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el tipo de cuenta", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de cuenta a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de cuenta actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el tipo de cuenta</exception>
        public override AccountType Update(AccountType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE account_type SET name = @Name WHERE idaccounttype = @Id", entity);
                LogUpdate(
                    entity.Id,
                    "account_type",
                    "UPDATE account_type SET name = '" + entity.Name + "' WHERE idaccounttype = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el tipo de cuenta", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de cuenta de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de cuenta a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de cuenta eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el tipo de cuenta</exception>
        public override AccountType Delete(AccountType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM account_type WHERE idaccounttype = @Id", entity);
                LogDelete(entity.Id, "account_type", "DELETE FROM account_type WHERE idaccounttype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el tipo de cuenta", ex);
            }
        }
        #endregion
    }
}
