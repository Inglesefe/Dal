using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Config;
using Entities.Cont;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Cont
{
    /// <summary>
    /// Realiza la persistencia de los números de cuenta contable en la base de datos
    /// </summary>
    public class PersistentAccountNumber : PersistentBaseWithLog<AccountNumber>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentAccountNumber(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de números de cuenta desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de números de cuenta</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los números de cuenta</exception>
        public override ListResult<AccountNumber> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idaccountnumber, number, idaccounttype, accounttype, idcity, city_code, city_name, idcountry, country_code, country_name",
                    "v_account_number");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<AccountNumber> numbers = connection.Query(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    GetTypes(),
                    Map(),
                    splitOn: "idaccounttype, idcity, idcountry"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(numbers, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de números de cuenta", ex);
            }
        }

        /// <summary>
        /// Consulta un número de cuenta dado su identificador
        /// </summary>
        /// <param name="entity">Número de cuenta a consultar</param>
        /// <returns>Número de cuenta con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el número de cuenta</exception>
        public override AccountNumber Read(AccountNumber entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<AccountNumber> result = connection.Query(
                    "SELECT idaccountnumber, number, idaccounttype, accounttype, idcity, city_code, city_name, idcountry, country_code, country_name " +
                    "FROM v_account_number WHERE idaccountnumber = @Id",
                    GetTypes(),
                    Map(),
                    entity,
                    splitOn: "idaccounttype, idcity, idcountry"
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
                throw new PersistentException("Error al consultar el número de cuenta", ex);
            }
        }

        /// <summary>
        /// Inserta un número de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Número de cuenta a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Número de cuenta insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el número de cuenta</exception>
        public override AccountNumber Insert(AccountNumber entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO account_number (idaccounttype, idcity, number) VALUES (@IdAccountType, @IdCity, @Number); SELECT LAST_INSERT_ID();",
                    new { IdAccountType = entity.AccountType.Id, IdCity = entity.City.Id, entity.Number });
                LogInsert(
                    entity.Id,
                    "account_number",
                    "INSERT INTO account_number (idaccounttype, idcity, number) VALUES (" + entity.AccountType.Id + ", " + entity.City.Id + ", '" + entity.Number + "')",
                    user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el número de cuenta", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un número de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Número de cuenta a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Número de cuenta actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el número de cuenta</exception>
        public override AccountNumber Update(AccountNumber entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE account_number SET idaccounttype = @IdAccountType, number = @Number WHERE idaccountnumber = @Id",
                    new { entity.Id, IdAccountType = entity.AccountType.Id, entity.Number });
                LogUpdate(
                    entity.Id,
                    "account_number",
                    "UPDATE account_number SET idaccounttype = " + entity.AccountType.Id + ", number = '" + entity.Number + "' WHERE idaccountnumber = " + entity.Id,
                    user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el número de cuenta", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina un número de cuenta de la base de datos
        /// </summary>
        /// <param name="entity">Número de cuenta a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Número de cuenta eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el número de cuenta</exception>
        public override AccountNumber Delete(AccountNumber entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM account_number WHERE idaccountnumber = @Id", entity);
                LogDelete(entity.Id, "account_number", "DELETE FROM account_number WHERE idaccountnumber = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el número de cuenta", ex);
            }
            return entity;
        }

        /// <summary>
        /// Trae los tipos de datos a mapear
        /// </summary>
        /// <returns>Tipos de datos a mapear</returns>
        private static Type[] GetTypes()
        {
            return new[] {
                typeof(AccountNumber),
                typeof(AccountType),
                typeof(City),
                typeof(Country)
            };
        }

        /// <summary>
        /// Realiza el mapeo
        /// </summary>
        /// <returns>Función de mapeo</returns>
        private static Func<object[], AccountNumber> Map()
        {
            return objects =>
            {
                AccountNumber an = (objects[0] as AccountNumber) ?? new AccountNumber();
                AccountType at = (objects[1] as AccountType) ?? new AccountType();
                City c = (objects[2] as City) ?? new City();
                Country co = (objects[3] as Country) ?? new Country();
                c.Country = co;
                an.AccountType = at;
                an.City = c;
                return an;
            };
        }
        #endregion
    }
}
