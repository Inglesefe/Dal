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
    /// Realiza la persistencia de las oficinas en la base de datos
    /// </summary>
    public class PersistentOffice : PersistentBaseWithLog, IPersistentWithLog<Office>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        public PersistentOffice() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de oficinas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de oficinas</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las oficinas</exception>
        public ListResult<Office> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<Office> result;
            try
            {
                QueryBuilder queryBuilder = new(
                    "o.idoffice AS id, o.name as name, o.address as address, ci.idcity as id, ci.name as name, ci.code as code, co.idcountry as id, co.name as name, co.code as code",
                    "office o inner join city ci on o.idcity = ci.idcity inner join country co on ci.idcountry = co.idcountry");
                using (connection)
                {
                    List<Office> offices = connection.Query(
                        queryBuilder.GetSelectForList(filters, orders, limit, offset),
                        new[]
                        {
                        typeof(Office),
                        typeof(City),
                        typeof(Country)
                        },
                        objects =>
                        {
                            Office o = (objects[0] as Office) ?? new Office();
                            City ci = (objects[1] as City) ?? new City();
                            Country co = (objects[2] as Country) ?? new Country();
                            ci.Country = co;
                            o.City = ci;
                            return o;
                        },
                        splitOn: "id"
                        ).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(offices, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de oficinas", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta una oficina dado su identificador
        /// </summary>
        /// <param name="entity">Oficina a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Oficina con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la oficina</exception>
        public Office Read(Office entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    IEnumerable<Office> result = connection.Query(
                    "SELECT o.idoffice AS id, o.name as name, o.address as address, ci.idcity as id, ci.name as name, ci.code as code, co.idcountry as id, co.name as name, co.code as code " +
                    "FROM office o inner join city ci on o.idcity = ci.idcity inner join country co on ci.idcountry = co.idcountry WHERE o.idoffice = @Id",
                    new[]
                    {
                        typeof(Office),
                        typeof(City),
                        typeof(Country)
                    },
                    objects =>
                    {
                        Office o = (objects[0] as Office) ?? new Office();
                        City ci = (objects[1] as City) ?? new City();
                        Country co = (objects[2] as Country) ?? new Country();
                        ci.Country = co;
                        o.City = ci;
                        return o;
                    },
                    entity,
                    splitOn: "id");
                    if (result.Any())
                    {
                        return result.First();
                    }
                    else
                    {
                        return new();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar la aplicación", ex);
            }
        }

        /// <summary>
        /// Inserta una oficina en la base de datos
        /// </summary>
        /// <param name="entity">Oficina a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Oficina insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la oficina</exception>
        public Office Insert(Office entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingle<int>("INSERT INTO office (idcity, name, address) VALUES (@IdCity, @Name, @Address); SELECT LAST_INSERT_ID();",
                    new { @IdCity = entity.City.Id, entity.Name, entity.Address });
                    LogInsert(entity.Id, "office", "INSERT INTO office (idcity, name, address) VALUES (" + entity.City.Id + ", '" + entity.Name + "', '" + entity.Address + "')", user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la oficina", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza una oficina en la base de datos
        /// </summary>
        /// <param name="entity">Oficina a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Oficina actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la oficina</exception>
        public Office Update(Office entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("UPDATE office SET name = @Name, address = Address WHERE idoffice = @Id", entity);
                    LogUpdate(entity.Id, "office", "UPDATE office SET name = '" + entity.Name + "', address = '" + entity.Address + "' WHERE idoffice = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la oficina", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina una oficina de la base de datos
        /// </summary>
        /// <param name="entity">Oficina a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Oficina eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la oficina</exception>
        public Office Delete(Office entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM office WHERE idoffice = @Id", entity);
                    LogDelete(entity.Id, "office", "DELETE FROM office WHERE idoffice = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la oficina", ex);
            }
            return entity;
        }
        #endregion
    }
}
