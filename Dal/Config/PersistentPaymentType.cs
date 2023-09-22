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
    /// Realiza la persistencia de los tipos de pagos en la base de datos
    /// </summary>
    public class PersistentPaymentType : PersistentBaseWithLog<PaymentType>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la bse de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentPaymentType(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de tipos de pagos desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de tipos de pagos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los tipos de pagos</exception>
        public override ListResult<PaymentType> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idpaymenttype, name", "payment_type");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<PaymentType> types = connection.Query<PaymentType>(queryBuilder.GetSelectForList(filters, orders, limit, offset)).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(types, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de tipos de pagos", ex);
            }
        }

        /// <summary>
        /// Consulta un tipo de pago dado su identificador
        /// </summary>
        /// <param name="entity">Tipo de pago a consultar</param>
        /// <returns>Tipo de pago con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el tipo de pago</exception>
        public override PaymentType Read(PaymentType entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                PaymentType result = connection.QuerySingleOrDefault<PaymentType>("SELECT idpaymenttype, name FROM payment_type WHERE idpaymenttype = @Id", entity);
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
                throw new PersistentException("Error al consultar el tipo de pago", ex);
            }
        }

        /// <summary>
        /// Inserta un tipo de pago en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de pago a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Tipo de pago insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el tipo de pago</exception>
        public override PaymentType Insert(PaymentType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO payment_type (name) VALUES (@Name); SELECT LAST_INSERT_ID();", entity);
                LogInsert(entity.Id, "payment_type", "INSERT INTO payment_type (name) VALUES ('" + entity.Name + "')", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el tipo de pago", ex);
            }
        }

        /// <summary>
        /// Actualiza un tipo de pago en la base de datos
        /// </summary>
        /// <param name="entity">Tipo de pago a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Tipo de pago actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el tipo de pago</exception>
        public override PaymentType Update(PaymentType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE payment_type SET name = @Name WHERE idpaymenttype = @Id", entity);
                LogUpdate(entity.Id, "payment_type", "UPDATE payment_type SET name = '" + entity.Name + "' WHERE idpaymenttype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el tipo de pago", ex);
            }
        }

        /// <summary>
        /// Elimina un tipo de pago de la base de datos
        /// </summary>
        /// <param name="entity">Tipo de pago a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Tipo de pago eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el tipo de pago</exception>
        public override PaymentType Delete(PaymentType entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM payment_type WHERE idpaymenttype = @Id", entity);
                LogDelete(entity.Id, "payment_type", "DELETE FROM payment_type WHERE idpaymenttype = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el tipo de pago", ex);
            }
        }
        #endregion
    }
}
