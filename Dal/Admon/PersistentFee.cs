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
    /// Realiza la persistencia de las cuotas de las matrículas en la base de datos
    /// </summary>
    public class PersistentFee : PersistentBaseWithLog<Fee>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentFee(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de cuotas de matrículas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de cuotas de matrículas</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los cuotas de matrículas</exception>
        public override ListResult<Fee> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idfee, value, number, dueDate, idregistration, idincometype, incometype_code, incometype", "v_fee");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Fee> fees = connection.Query<Fee, Registration, IncomeType, Fee>(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    (f, r, it) =>
                    {
                        f.Registration = r;
                        f.IncomeType = it;
                        return f;
                    },
                    splitOn: "idregistration, idincometype"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(fees, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de cuotas de matrículas", ex);
            }
        }

        /// <summary>
        /// Consulta una cuota de matrícula dado su identificador
        /// </summary>
        /// <param name="entity">Cuota de matrícula a consultar</param>
        /// <returns>Cuota de matrícula con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la cuota de matrícula</exception>
        public override Fee Read(Fee entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<Fee> result = connection.Query<Fee, Registration, IncomeType, Fee>(
                    "SELECT idfee, value, number, dueDate, idregistration, idincometype, incometype_code, incometype FROM v_fee WHERE idfee = @Id",
                    (f, r, it) =>
                    {
                        f.Registration = r;
                        f.IncomeType = it;
                        return f;
                    },
                    entity,
                    splitOn: "idregistration, idincometype"
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
                throw new PersistentException("Error al consultar la cuota de matrícula", ex);
            }
        }

        /// <summary>
        /// Inserta una cuota de matrícula en la base de datos
        /// </summary>
        /// <param name="entity">Cuota de matrícula a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Cuota de matrícula insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la cuota de matrícula</exception>
        public override Fee Insert(Fee entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO fee (idregistration, value, number, idincometype, dueDate) VALUES (@Idregistration, @Value, @Number, @IdIncomeType, @DueDate); SELECT LAST_INSERT_ID();",
                    new { Idregistration = entity.Registration.Id, entity.Value, entity.Number, IdIncomeType = entity.IncomeType.Id, entity.DueDate });
                LogInsert(
                    entity.Id,
                    "fee",
                    "INSERT INTO fee (idregistration, value, number, idincometype, dueDate) VALUES (" + entity.Registration.Id + ", " +
                    "" + entity.Value + ", " + entity.Number + ", " + entity.IncomeType.Id + ", '" + entity.DueDate.ToString("yyyy-MM-dd") + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la cuota de matrícula", ex);
            }
        }

        /// <summary>
        /// Actualiza una cuota de matrícula en la base de datos
        /// </summary>
        /// <param name="entity">Cuota de matrícula a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Cuota de matrícula actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la cuota de matrícula</exception>
        public override Fee Update(Fee entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "UPDATE fee SET value = @Value, number = @Number, idincometype = @IdIncomeType, dueDate = @DueDate WHERE idfee = @Id",
                    new { entity.Id, entity.Value, entity.Number, IdIncomeType = entity.IncomeType.Id, entity.DueDate });
                LogUpdate(
                    entity.Id,
                    "fee",
                    "UPDATE fee SET value = " + entity.Value + ", number = " + entity.Number + ", idincometype = " + entity.IncomeType.Id + ", dueDate = '" + entity.DueDate.ToString("yyyy-MM-dd") + "' WHERE idfee = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la cuota de matrícula", ex);
            }
        }

        /// <summary>
        /// Elimina una cuota de matrícula de la base de datos
        /// </summary>
        /// <param name="entity">Cuota de matrícula a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Cuota de matrícula eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la cuota de matrícula</exception>
        public override Fee Delete(Fee entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM fee WHERE idfee = @Id", entity);
                LogDelete(entity.Id, "fee", "DELETE FROM fee WHERE idfee = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la cuota de matrícula", ex);
            }
        }
        #endregion
    }
}
