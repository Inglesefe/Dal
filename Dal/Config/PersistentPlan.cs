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
    /// Realiza la persistencia de los planes en la base de datos
    /// </summary>
    public class PersistentPlan : PersistentBaseWithLog, IPersistentWithLog<Plan>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// </summary>
        /// <param name="connection">Conexión a la base de datos</param>
        public PersistentPlan(IDbConnection connection) : base(connection) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de planes desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de planes</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los planes</exception>
        public ListResult<Plan> List(string filters, string orders, int limit, int offset)
        {
            ListResult<Plan> result;
            try
            {
                QueryBuilder queryBuilder = new("idplan AS id, value, initial_fee as initialFee, installments_number as installmentsNumber, installment_value as installmentValue, active", "plan");
                OpenConnection();
                List<Plan> countries = _connection.Query<Plan>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = _connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                result = new(countries, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de planes", ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        /// <summary>
        /// Consulta un plan dado su identificador
        /// </summary>
        /// <param name="entity">Plan a consultar</param>
        /// <returns>Plan con los datos cargados desde la base de datos o null si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el plan</exception>
        public Plan Read(Plan entity)
        {
            try
            {
                OpenConnection();
                Plan result = _connection.QuerySingleOrDefault<Plan>("SELECT idplan AS id, value, initial_fee as initialFee, installments_number as installmentsNumber, installment_value as installmentValue, active FROM plan WHERE idplan = @Id", entity);
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
                throw new PersistentException("Error al consultar el plan", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Inserta un plan en la base de datos
        /// </summary>
        /// <param name="entity">Plan a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Plan insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el plan</exception>
        public Plan Insert(Plan entity, User user)
        {
            try
            {
                OpenConnection();
                entity.Id = _connection.QuerySingle<int>("INSERT INTO plan (value, initial_fee, installments_number, installment_value, active, description) VALUES (@Value, @InitialFee, @InstallmentsNumber, @InstallmentValue, @Active, @Description); SELECT LAST_INSERT_ID();", entity);
                LogInsert(
                    entity.Id,
                    "plan",
                    "INSERT INTO plan (value, initial_fee, installments_number, installment_value, active, description) VALUES (" + entity.Value + ", " + entity.InitialFee + ", " + entity.InstallmentsNumber + ", " + entity.InstallmentValue + ", " + (entity.Active ? "1" : "0") + ", '" + entity.Description + "')",
                    user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el plan", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Actualiza un plan en la base de datos
        /// </summary>
        /// <param name="entity">Plan a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Plan actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el plan</exception>
        public Plan Update(Plan entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("UPDATE plan SET value = @Value, initial_fee = @InitialFee, installments_number = @InstallmentsNumber, installment_value = @InstallmentValue, active = @Active WHERE idplan = @Id", entity);
                LogUpdate(entity.Id, "plan",
                    "UPDATE plan SET value = " + entity.Value + ", initial_fee = " + entity.InitialFee + ", installments_number = " + entity.InstallmentsNumber + ", installment_value = " + entity.InstallmentValue + ", active = " + (entity.Active ? "1" : "0") + " WHERE idplan = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el plan", ex);
            }
            finally
            {
                CloseConnection();
            }
            return entity;
        }

        /// <summary>
        /// Elimina un plan de la base de datos
        /// </summary>
        /// <param name="entity">Plan a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Plan eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el plan</exception>
        public Plan Delete(Plan entity, User user)
        {
            try
            {
                OpenConnection();
                _ = _connection.Execute("DELETE FROM plan WHERE idplan = @Id", entity);
                LogDelete(entity.Id, "plan", "DELETE FROM plan WHERE idplan = " + entity.Id, user.Id);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el plan", ex);
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
