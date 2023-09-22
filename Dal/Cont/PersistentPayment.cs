using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Entities.Cont;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Cont
{
    /// <summary>
    /// Realiza la persistencia de los pagos en la base de datos
    /// </summary>
    public class PersistentPayment : PersistentBaseWithLog<Payment>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentPayment(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de pagos desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se consecutivo el número de registros</param>
        /// <returns>Listado de pagos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los pagos</exception>
        public override ListResult<Payment> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idpayment, value, date, invoice, proof, idpaymenttype, paymenttype, idfee", "v_payment");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Payment> types = connection.Query<Payment, PaymentType, Fee, Payment>(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    (p, pt, f) =>
                    {
                        p.PaymentType = pt;
                        p.Fee = f;
                        return p;
                    },
                    splitOn: "idpaymenttype, idfee"
                    ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(types, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de pagos", ex);
            }
        }

        /// <summary>
        /// Consulta un pago dado su identificador
        /// </summary>
        /// <param name="entity">Pago a consultar</param>
        /// <returns>Pago con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el pago</exception>
        public override Payment Read(Payment entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<Payment> result = connection.Query<Payment, PaymentType, Fee, Payment>(
                    "SELECT idpayment, value, date, invoice, proof, idpaymenttype, paymenttype, idfee FROM v_payment WHERE idpayment = @Id",
                    (p, pt, f) =>
                    {
                        p.PaymentType = pt;
                        p.Fee = f;
                        return p;
                    },
                    entity,
                    splitOn: "idpaymenttype, idfee");
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
                throw new PersistentException("Error al consultar el pago", ex);
            }
        }

        /// <summary>
        /// Inserta un pago en la base de datos
        /// </summary>
        /// <param name="entity">Pago a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Pago insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el pago</exception>
        public override Payment Insert(Payment entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO payment (idpaymenttype, idfee, value, date, invoice, proof) VALUES (@IdPaymentType, @IdFee, @Value, @Date, @Invoice, @Proof); SELECT LAST_INSERT_ID();",
                    new { IdPaymentType = entity.PaymentType.Id, IdFee = entity.Fee.Id, entity.Value, entity.Date, entity.Invoice, entity.Proof });
                LogInsert(
                    entity.Id,
                    "payment",
                    "INSERT INTO payment (idpaymenttype, idfee, value, date, invoice, proof) " +
                    "VALUES (" + entity.PaymentType.Id + ", " + entity.Fee.Id + ", " + entity.Value + ", '" + entity.Date.ToString("yyyy-MM-dd") + "', '" + entity.Invoice + "', '" + entity.Proof + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el pago", ex);
            }
        }

        /// <summary>
        /// Actualiza un pago en la base de datos
        /// </summary>
        /// <param name="entity">Pago a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Pago actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el pago</exception>
        public override Payment Update(Payment entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE payment SET idpaymenttype = @IdPaymentType, value = @Value, date = @Date, invoice = @Invoice, proof = @Proof WHERE idpayment = @Id",
                    new { entity.Id, IdPaymentType = entity.PaymentType.Id, entity.Value, entity.Date, entity.Invoice, entity.Proof });
                LogUpdate(
                    entity.Id,
                    "payment",
                    "UPDATE payment SET idpaymenttype = " + entity.PaymentType.Id + ", value = " + entity.Value + ", date = '" + entity.Date.ToString("yyyy-MM-dd") + "', " +
                    "invoice = '" + entity.Invoice + "', proof = '" + entity.Proof + "' WHERE idpayment = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el pago", ex);
            }
        }

        /// <summary>
        /// Elimina un pago de la base de datos
        /// </summary>
        /// <param name="entity">Pago a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Pago eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el pago</exception>
        public override Payment Delete(Payment entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM payment WHERE idpayment = @Id", entity);
                LogDelete(entity.Id, "payment", "DELETE FROM payment WHERE idpayment = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el pago", ex);
            }
        }
        #endregion
    }
}
