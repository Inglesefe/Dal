﻿using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Config;
using System.Data;

namespace Dal.Config
{
    /// <summary>
    /// Realiza la persistencia de las ciudades en la base de datos
    /// </summary>
    public class PersistentCity : PersistentBaseWithLog, IPersistentWithLog<City>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        public PersistentCity() : base() { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de ciudades desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Listado de ciudades</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las ciudades</exception>
        public ListResult<City> List(string filters, string orders, int limit, int offset, IDbConnection connection)
        {
            ListResult<City> result;
            try
            {
                QueryBuilder queryBuilder = new(
                    "ci.idcity AS id, ci.code as code, ci.name as name, co.idcountry as id, co.name as name, co.code as code",
                    "city ci inner join country co on ci.idcountry = co.idcountry");
                using (connection)
                {
                    List<City> cities = connection.Query<City, Country, City>(
                        queryBuilder.GetSelectForList(filters, orders, limit, offset),
                        (ci, co) =>
                        {
                            ci.Country = co;
                            return ci;
                        },
                        splitOn: "id"
                        ).ToList();
                    int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                    result = new(cities, total);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de ciudades", ex);
            }
            return result;
        }

        /// <summary>
        /// Consulta una ciudad dado su identificador
        /// </summary>
        /// <param name="entity">Ciudad a consultar</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Ciudad con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la ciudad</exception>
        public City Read(City entity, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    IEnumerable<City> result = connection.Query<City, Country, City>(
                    "SELECT ci.idcity AS id, ci.code as code, ci.name as name, co.idcountry as id, co.name as name, co.code as code FROM city ci inner join country co on ci.idcountry = co.idcountry WHERE ci.idcity = @Id",
                    (ci, co) =>
                    {
                        ci.Country = co;
                        return ci;
                    },
                    new { entity.Id },
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
        /// Inserta una ciudad en la base de datos
        /// </summary>
        /// <param name="entity">Ciudad a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Ciudad insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la ciudad</exception>
        public City Insert(City entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    entity.Id = connection.QuerySingle<int>("INSERT INTO city (idcountry, code, name) VALUES (@Id, @Code, @Name); SELECT LAST_INSERT_ID();",
                    new { entity.Country.Id, entity.Code, entity.Name });
                    LogInsert(entity.Id, "city", "INSERT INTO city (idcountry, code, name) VALUES (" + entity.Country.Id + ", '" + entity.Code + "', '" + entity.Name + "')", user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la ciudad", ex);
            }
            return entity;
        }

        /// <summary>
        /// Actualiza una ciudad en la base de datos
        /// </summary>
        /// <param name="entity">Ciudad a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Ciudad actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la ciudad</exception>
        public City Update(City entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("UPDATE city SET code = @Code, name = @Name WHERE idcity = @Id", entity);
                    LogUpdate(entity.Id, "city", "UPDATE city SET code = '" + entity.Code + "', name = '" + entity.Name + "' WHERE idcity = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la ciudad", ex);
            }
            return entity;
        }

        /// <summary>
        /// Elimina una ciudad de la base de datos
        /// </summary>
        /// <param name="entity">Ciudad a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <param name="connection">Conexión a la base de datos</param>
        /// <returns>Ciudad eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la ciudad</exception>
        public City Delete(City entity, User user, IDbConnection connection)
        {
            try
            {
                using (connection)
                {
                    _ = connection.Execute("DELETE FROM city WHERE idcity = @Id", entity);
                    LogDelete(entity.Id, "city", "DELETE FROM city WHERE idcity = " + entity.Id, user.Id, connection);
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la ciudad", ex);
            }
            return entity;
        }
        #endregion
    }
}
