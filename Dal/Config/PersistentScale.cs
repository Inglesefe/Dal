using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Config;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Config
{
    /// <summary>
    /// Realiza la persistencia de las escalas en la base de datos
    /// </summary>
    public class PersistentScale : PersistentBaseWithLog<Scale>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentScale(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de escalas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de escalas</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los escalas</exception>
        public override ListResult<Scale> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idscale, code, name, comission, `order`", "scale");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Scale> scales = connection.Query<Scale>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(scales, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de escalas", ex);
            }
        }

        /// <summary>
        /// Consulta una escala dado su identificador
        /// </summary>
        /// <param name="entity">Escala a consultar</param>
        /// <returns>Escala con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la escala</exception>
        public override Scale Read(Scale entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                Scale result = connection.QuerySingleOrDefault<Scale>("SELECT idscale, code, name, comission, `order` FROM scale WHERE idscale = @Id", entity);
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
                throw new PersistentException("Error al consultar la escala", ex);
            }
        }

        /// <summary>
        /// Inserta una escala en la base de datos
        /// </summary>
        /// <param name="entity">Escala a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Escala insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la escala</exception>
        public override Scale Insert(Scale entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO scale (code, name, comission, `order`) VALUES (@Code, @Name, @Comission, @Order); SELECT LAST_INSERT_ID();", entity);
                LogInsert(
                    entity.Id,
                    "plan",
                    "INSERT INTO scale (code, name, comission, `order`) VALUES ('" + entity.Code + "', '" + entity.Name + "', " + entity.Comission + ", " + entity.Order + ")",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la escala", ex);
            }
        }

        /// <summary>
        /// Actualiza una escala en la base de datos
        /// </summary>
        /// <param name="entity">Escala a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Escala actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la escala</exception>
        public override Scale Update(Scale entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE scale SET code = @Code, name = @Name, comission = @Comission, `order` = @Order WHERE idscale = @Id", entity);
                LogUpdate(entity.Id, "scale",
                    "UPDATE scale SET code = '" + entity.Code + "', name = '" + entity.Name + "', comission = " + entity.Comission + ", `order` = " + entity.Order + " WHERE idscale = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la escala", ex);
            }
        }

        /// <summary>
        /// Elimina una escala de la base de datos
        /// </summary>
        /// <param name="entity">Escala a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Escala eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la escala</exception>
        public override Scale Delete(Scale entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM scale WHERE idscale = @Id", entity);
                LogDelete(entity.Id, "scale", "DELETE FROM scale WHERE idscale = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la escala", ex);
            }
        }
        #endregion
    }
}
