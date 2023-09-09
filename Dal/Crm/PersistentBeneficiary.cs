using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Auth;
using Entities.Config;
using Entities.Crm;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Crm
{
    /// <summary>
    /// Realiza la persistencia de los tipos de beneficiarios en la base de datos
    /// </summary>
    public class PersistentBeneficiary : PersistentBaseWithLog<Beneficiary>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentBeneficiary(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de beneficiarios desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de beneficiarios</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los beneficiarios</exception>
        public override ListResult<Beneficiary> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idbeneficiary, name, identification, " +
                    "relationship, idowner, owner, " +
                    "owner_identification, owner_address_home, owner_address_office, " +
                    "owner_phone_home, owner_phone_office, owner_email, " +
                    "owner_ididentificationtype, owner_identificationtype, ididentificationtype, " +
                    "identificationtype",
                    "v_beneficiary");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Beneficiary> beneficiaries = connection.Query<Beneficiary, Owner, IdentificationType, IdentificationType, Beneficiary>(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    (b, o, ito, itb) =>
                    {
                        o.IdentificationType = ito;
                        b.Owner = o;
                        b.IdentificationType = itb;
                        return b;
                    },
                    splitOn: "idowner, owner_ididentificationtype, ididentificationtype"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(beneficiaries, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de beneficiarios", ex);
            }
        }

        /// <summary>
        /// Consulta un beneficiario dado su identificador
        /// </summary>
        /// <param name="entity">Beneficiario a consultar</param>
        /// <returns>Beneficiario con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el beneficiario</exception>
        public override Beneficiary Read(Beneficiary entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<Beneficiary> result = connection.Query<Beneficiary, Owner, IdentificationType, IdentificationType, Beneficiary>(
                    "SELECT " +
                    "idbeneficiary, name, identification, " +
                    "relationship, idowner, owner, " +
                    "owner_identification, owner_address_home, owner_address_office, " +
                    "owner_phone_home, owner_phone_office, owner_email, " +
                    "owner_ididentificationtype, owner_identificationtype, ididentificationtype, " +
                    "identificationtype " +
                    "FROM v_beneficiary " +
                    "WHERE idbeneficiary = @Id",
                    (b, o, ito, itb) =>
                    {
                        o.IdentificationType = ito;
                        b.Owner = o;
                        b.IdentificationType = itb;
                        return b;
                    },
                    entity,
                    splitOn: "idowner, owner_ididentificationtype, ididentificationtype"
                ).ToList();
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
                throw new PersistentException("Error al consultar el beneficiario", ex);
            }
        }

        /// <summary>
        /// Inserta un beneficiario en la base de datos
        /// </summary>
        /// <param name="entity">Beneficiario a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Beneficiario insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el beneficiario</exception>
        public override Beneficiary Insert(Beneficiary entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO beneficiary (idowner, name, ididentificationtype, identification, relationship) " +
                    "VALUES (@IdOwner, @Name, @IdIdentificationType, @Identification, @Relationship); SELECT LAST_INSERT_ID();",
                    new { IdOwner = entity.Owner.Id, entity.Name, IdIdentificationType = entity.IdentificationType.Id, entity.Identification, entity.Relationship }
                    );
                LogInsert(
                    entity.Id,
                    "beneficiary",
                    "INSERT INTO beneficiary (idowner, name, ididentificationtype, identification, relationship) VALUES " +
                    "(" + entity.Owner.Id + ", '" + entity.Name + "', " + entity.IdentificationType.Id + ", '" + entity.Identification + "', '" + entity.Relationship + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el beneficiario", ex);
            }
        }

        /// <summary>
        /// Actualiza un beneficiario en la base de datos
        /// </summary>
        /// <param name="entity">Beneficiario a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Beneficiario actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el beneficiario</exception>
        public override Beneficiary Update(Beneficiary entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "UPDATE beneficiary SET name = @Name, ididentificationtype = @IdIdentificationType, identification = @Identification, relationship = @Relationship " +
                    "WHERE idbeneficiary = @Id",
                    new { entity.Id, entity.Name, IdIdentificationType = entity.IdentificationType.Id, entity.Identification, entity.Relationship }
                    );
                LogUpdate(
                    entity.Id,
                    "beneficiary",
                    "UPDATE beneficiary SET name = '" + entity.Name + "', ididentificationtype = " + entity.IdentificationType.Id + ", identification = '" + entity.Identification + "', " +
                    "relationship = '" + entity.Relationship + "' WHERE idbeneficiary = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el beneficiario", ex);
            }
        }

        /// <summary>
        /// Elimina un beneficiario de la base de datos
        /// </summary>
        /// <param name="entity">Beneficiario a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Beneficiario eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el beneficiario</exception>
        public override Beneficiary Delete(Beneficiary entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM beneficiary WHERE idbeneficiary = @Id", entity);
                LogDelete(entity.Id, "beneficiary", "DELETE FROM beneficiary WHERE idbeneficiary = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el beneficiario", ex);
            }
        }
        #endregion
    }
}
