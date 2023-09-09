using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Admon
{
    /// <summary>
    /// Realiza la persistencia de los ejecutivos de cuenta en la base de datos
    /// </summary>
    public class PersistentAccountExecutive : PersistentBaseWithLog<AccountExecutive>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentAccountExecutive(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de ejecutivos de cuenta desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de ejecutivos de cuenta</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        public override ListResult<AccountExecutive> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idaccountexecutive, name, identification, ididentificationtype, identificationtype", "v_account_executive");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<AccountExecutive> executives = connection.Query<AccountExecutive, IdentificationType, AccountExecutive>(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    (ae, it) =>
                    {
                        ae.IdentificationType = it;
                        return ae;
                    },
                    splitOn: "ididentificationtype"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(executives, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de ejecutivos de cuenta", ex);
            }
        }

        /// <summary>
        /// Consulta un ejecutivo de cuenta dado su identificador
        /// </summary>
        /// <param name="entity">Ejecutivo de cuenta a consultar</param>
        /// <returns>Ejecutivo de cuenta con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el ejecutivo de cuenta</exception>
        public override AccountExecutive Read(AccountExecutive entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<AccountExecutive> result = connection.Query<AccountExecutive, IdentificationType, AccountExecutive>(
                    "SELECT idaccountexecutive, name, identification, ididentificationtype, identificationtype FROM v_account_executive WHERE idaccountexecutive = @Id",
                    (ae, it) =>
                    {
                        ae.IdentificationType = it;
                        return ae;
                    },
                    entity,
                    splitOn: "ididentificationtype"
                );
                if (result.Any())
                {
                    return result.First();
                }
                else
                {
                    return new();
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el ejecutivo de cuenta", ex);
            }
        }

        /// <summary>
        /// Inserta un ejecutivo de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Ejecutivo de cuenta a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Ejecutivo de cuenta insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el ejecutivo de cuenta</exception>
        public override AccountExecutive Insert(AccountExecutive entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO account_executive (name, ididentificationtype, identification) VALUES (@Name, @IdIdentificationType, @Identification); SELECT LAST_INSERT_ID();",
                    new { entity.Name, IdIdentificationType = entity.IdentificationType.Id, entity.Identification });
                LogInsert(
                    entity.Id,
                    "account_executive",
                    "INSERT INTO account_executive (name, ididentificationtype, identification) VALUES ('" + entity.Name + "', " + entity.IdentificationType.Id + ", '" + entity.Name + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el ejecutivo de cuenta", ex);
            }
        }

        /// <summary>
        /// Actualiza un ejecutivo de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Ejecutivo de cuenta a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Ejecutivo de cuenta actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el ejecutivo de cuenta</exception>
        public override AccountExecutive Update(AccountExecutive entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "UPDATE account_executive SET name = @Name, ididentificationtype = @IdIdentificationType, identification = @Identification WHERE idaccountexecutive = @Id",
                    new { entity.Id, entity.Name, IdIdentificationType = entity.IdentificationType.Id, entity.Identification });
                LogUpdate(
                    entity.Id,
                    "account_executive",
                    "UPDATE account_executive SET name = '" + entity.Name + "', ididentificationtype = " + entity.IdentificationType.Id + ", identification = '" + entity.Identification + "' WHERE idaccountexecutive = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el ejecutivo de cuenta", ex);
            }
        }

        /// <summary>
        /// Elimina un ejecutivo de cuenta de la base de datos
        /// </summary>
        /// <param name="entity">Ejecutivo de cuenta a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Ejecutivo de cuenta eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el ejecutivo de cuenta</exception>
        public override AccountExecutive Delete(AccountExecutive entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM account_executive WHERE idaccountexecutive = @Id", entity);
                LogDelete(entity.Id, "account_executive", "DELETE FROM account_executive WHERE idaccountexecutive = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el ejecutivo de cuenta", ex);
            }
        }
        #endregion
    }
}
