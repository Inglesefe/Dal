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
    /// Realiza la persistencia de los números de consecutivo contable en la base de datos
    /// </summary>
    public class PersistentConsecutiveNumber : PersistentBaseWithLog<ConsecutiveNumber>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentConsecutiveNumber(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de números de consecutivo desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se consecutivo el número de registros</param>
        /// <returns>Listado de números de consecutivo</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los números de consecutivo</exception>
        public override ListResult<ConsecutiveNumber> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idconsecutivenumber, number, idconsecutivetype, consecutivetype, idcity, city_code, city_name, idcountry, country_code, country_name",
                    "v_consecutive_number");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<ConsecutiveNumber> numbers = connection.Query(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    new[]
                    {
                    typeof(ConsecutiveNumber),
                    typeof(ConsecutiveType),
                    typeof(City),
                    typeof(Country)
                    },
                    objects =>
                    {
                        ConsecutiveNumber cn = (objects[0] as ConsecutiveNumber) ?? new ConsecutiveNumber();
                        ConsecutiveType ct = (objects[1] as ConsecutiveType) ?? new ConsecutiveType();
                        City c = (objects[2] as City) ?? new City();
                        Country co = (objects[3] as Country) ?? new Country();
                        c.Country = co;
                        cn.ConsecutiveType = ct;
                        cn.City = c;
                        return cn;
                    },
                    splitOn: "idconsecutivetype, idcity, idcountry"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(numbers, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de números de consecutivo", ex);
            }
        }

        /// <summary>
        /// Consulta un número de consecutivo dado su identificador
        /// </summary>
        /// <param name="entity">Número de consecutivo a consultar</param>
        /// <returns>Número de consecutivo con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el número de consecutivo</exception>
        public override ConsecutiveNumber Read(ConsecutiveNumber entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<ConsecutiveNumber> result = connection.Query(
                    "SELECT idconsecutivenumber, number, idconsecutivetype, consecutivetype, idcity, city_code, city_name, idcountry, country_code, country_name " +
                    "FROM v_consecutive_number WHERE idconsecutivenumber = @Id",
                    new[]
                    {
                    typeof(ConsecutiveNumber),
                    typeof(ConsecutiveType),
                    typeof(City),
                    typeof(Country)
                    },
                    objects =>
                    {
                        ConsecutiveNumber cn = (objects[0] as ConsecutiveNumber) ?? new ConsecutiveNumber();
                        ConsecutiveType ct = (objects[1] as ConsecutiveType) ?? new ConsecutiveType();
                        City c = (objects[2] as City) ?? new City();
                        Country co = (objects[3] as Country) ?? new Country();
                        c.Country = co;
                        cn.ConsecutiveType = ct;
                        cn.City = c;
                        return cn;
                    },
                    entity,
                    splitOn: "idconsecutivetype, idcity, idcountry"
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
                throw new PersistentException("Error al consultar el número de consecutivo", ex);
            }
        }

        /// <summary>
        /// Inserta un número de consecutivo en la base de datos
        /// </summary>
        /// <param name="entity">Número de consecutivo a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Número de consecutivo insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el número de consecutivo</exception>
        public override ConsecutiveNumber Insert(ConsecutiveNumber entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO consecutive_number (idconsecutivetype, idcity, number) VALUES (@IdConsecutiveType, @IdCity, @Number); SELECT LAST_INSERT_ID();",
                    new { IdConsecutiveType = entity.ConsecutiveType.Id, IdCity = entity.City.Id, entity.Number });
                LogInsert(
                    entity.Id,
                    "consecutive_number",
                    "INSERT INTO consecutive_number (idconsecutivetype, idcity, number) VALUES (" + entity.ConsecutiveType.Id + ", " + entity.City.Id + ", '" + entity.Number + "')",
                    user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el número de consecutivo", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un número de consecutivo en la base de datos
        /// </summary>
        /// <param name="entity">Número de consecutivo a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Número de consecutivo actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el número de consecutivo</exception>
        public override ConsecutiveNumber Update(ConsecutiveNumber entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE consecutive_number SET idconsecutivetype = @IdConsecutiveType, number = @Number WHERE idconsecutivenumber = @Id",
                    new { entity.Id, IdConsecutiveType = entity.ConsecutiveType.Id, entity.Number });
                LogUpdate(
                    entity.Id,
                    "consecutive_number",
                    "UPDATE consecutive_number SET idconsecutivetype = " + entity.ConsecutiveType.Id + ", number = '" + entity.Number + "' WHERE idconsecutivenumber = " + entity.Id,
                    user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el número de consecutivo", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina un número de consecutivo de la base de datos
        /// </summary>
        /// <param name="entity">Número de consecutivo a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Número de consecutivo eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el número de consecutivo</exception>
        public override ConsecutiveNumber Delete(ConsecutiveNumber entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM consecutive_number WHERE idconsecutivenumber = @Id", entity);
                LogDelete(entity.Id, "consecutive_number", "DELETE FROM consecutive_number WHERE idconsecutivenumber = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el número de consecutivo", ex);
            }
            return entity;
        }
        #endregion
    }
}
