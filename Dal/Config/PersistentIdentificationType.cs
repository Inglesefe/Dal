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
    /// Realiza la persistencia de los tipos de identificación en la base de datos
    /// </summary>
    public class PersistentIdentificationType : PersistentBaseWithLog<IdentificationType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentIdentificationType(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de identificación desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de tipos de identificación</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los tipos de identificación</exception>
        public override ListResult<IdentificationType> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("ididentificationtype, name", "identification_type");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<IdentificationType> types = connection.Query<IdentificationType>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(types, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de tipos de identificación", ex);
            }

        }

        /// <summary>
        /// Consulta un tipo de identificación dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de identificación a consultar</param>
        /// <returns>Tipo de identificación con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el tipo de identificación</exception>
        public override IdentificationType Read(IdentificationType entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IdentificationType result = connection.QuerySingleOrDefault<IdentificationType>("SELECT ididentificationtype, name FROM identification_type WHERE ididentificationtype = @Id", entity);
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
                throw new PersistentException("Error al consultar el tipo de identificación", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de identificación en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de identificación a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Tipo de identificación insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el tipo de identificación</exception>
        public override IdentificationType Insert(IdentificationType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO identification_type (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "identification_type", "INSERT INTO identification_type (name) VALUES ('" + entity.Name + "')", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el tipo de identificación", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de identificación en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de identificación a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de identificación actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el tipo de identificación</exception>
        public override IdentificationType Update(IdentificationType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE identification_type SET name = @Name WHERE ididentificationtype = @Id", entity);
                LogUpdate(entity.Id, "identification_type", "UPDATE identification_type SET name = '" + entity.Name + "' WHERE ididentificationtype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el tipo de identificación", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de identificación de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de identificación a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de identificación eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el tipo de identificación</exception>
        public override IdentificationType Delete(IdentificationType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM identification_type WHERE ididentificationtype = @Id", entity);
                LogDelete(entity.Id, "identification_type", "DELETE FROM identification_type WHERE ididentificationtype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el tipo de identificación", ex);
            }
        }
        #endregion
    }
}
