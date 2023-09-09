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
    /// Realiza la persistencia de los tipos de ingresos en la base de datos
    /// </summary>
    public class PersistentIncomeType : PersistentBaseWithLog<IncomeType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentIncomeType(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de ingresos desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de tipos de ingresos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los tipos de ingresos</exception>
        public override ListResult<IncomeType> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idincometype, code, name", "income_type");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<IncomeType> types = connection.Query<IncomeType>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(types, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de tipos de ingresos", ex);
            }
        }

        /// <summary>
        /// Consulta un tipo de ingresos dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de ingresos a consultar</param>
        /// <returns>Tipo de ingresos con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el tipo de ingresos</exception>
        public override IncomeType Read(IncomeType entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IncomeType result = connection.QuerySingleOrDefault<IncomeType>("SELECT idincometype, code, name FROM income_type WHERE idincometype = @Id", entity);
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
                throw new PersistentException("Error al consultar el tipo de ingresos", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de ingresos en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de ingresos a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Tipo de ingresos insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el tipo de ingresos</exception>
        public override IncomeType Insert(IncomeType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO income_type (code, name) VALUES (@Code, @Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "income_type", "INSERT INTO income_type (code, name) VALUES ('" + entity.Code + "', '" + entity.Name + "')", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el tipo de ingresos", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de ingresos en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de ingresos a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de ingresos actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el tipo de ingresos</exception>
        public override IncomeType Update(IncomeType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE income_type SET code = @Code, name = @Name WHERE idincometype = @Id", entity);
                LogUpdate(entity.Id, "income_type", "UPDATE income_type SET code = '" + entity.Code + "' name = '" + entity.Name + "' WHERE idincometype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el tipo de ingresos", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de ingresos de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de ingresos a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de ingresos eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el tipo de ingresos</exception>
        public override IncomeType Delete(IncomeType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM income_type WHERE idincometype = @Id", entity);
                LogDelete(entity.Id, "income_type", "DELETE FROM income_type WHERE idincometype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el tipo de ingresos", ex);
            }
        }
        #endregion
    }
}
