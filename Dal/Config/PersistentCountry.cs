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
    /// Realiza la persistencia de los paises en la base de datos
    /// </summary>
    public class PersistentCountry : PersistentBaseWithLog<Country>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public PersistentCountry(IDbConnection connection) : base(connection) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de paises desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de paises</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los paises</exception>
        public override ListResult<Country> List(string filters, string orders, int limit, int offset)
        {
            ListResult<Country> result;
            try
            {
                QueryBuilder queryBuilder = new("idcountry AS id, code, name", "country");
                OpenConnection();
                List<Country> countries = _connection.Query<Country>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                result = new(countries, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de paises", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Consulta un país dado su identificador
        /// </summary>
        /// <param name="entity">País a consultar</param>
        /// <returns>País con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el país</exception>
        public override Country Read(Country entity)
        {
            try
            {
                OpenConnection();
                Country result = _connection.QuerySingleOrDefault<Country>("SELECT idcountry AS id, code, name FROM country WHERE idcountry = @Id", entity);
                if (result == null)
                {
                    entity = new();
                }
                else
                {
                    entity = result;
                }
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el país", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Inserta un país en la base de datos
        /// </summary>
        /// <param name="entity">País a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>País insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el país</exception>
        public override Country Insert(Country entity, User user)
        {
            try
            {
                OpenConnection();
                entity.Id = _connection.QuerySingle<int>("INSERT INTO country (code, name) VALUES (@Code, @Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "country", "INSERT INTO country (code, name) VALUES ('" + entity.Code + "', '" + entity.Name + "')", user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el país", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un país en la base de datos
        /// </summary>
        /// <param name="entity">País a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>País actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el país</exception>
        public override Country Update(Country entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("UPDATE country SET code = @Code, name = @Name WHERE idcountry = @Id", entity);
                LogUpdate(entity.Id, "country", "UPDATE country SET code = '" + entity.Code + "', name = '" + entity.Name + "' WHERE idcountry = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el país", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Elimina un país de la base de datos
        /// </summary>
        /// <param name="entity">País a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>País eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el país</exception>
        public override Country Delete(Country entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("DELETE FROM country WHERE idcountry = @Id", entity);
                LogDelete(entity.Id, "country", "DELETE FROM country WHERE idcountry = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el país", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }
        #endregion
    }
}
