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
    /// Realiza la persistencia de los tipos de titulares en la base de datos
    /// </summary>
    public class PersistentOwner : PersistentBaseWithLog<Owner>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentOwner(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de titulares desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de titulares</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los titulares</exception>
        public override ListResult<Owner> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idowner, name, identification, " +
                    "address_home, address_office, phone_home, " +
                    "phone_office, email, ididentificationtype, identificationtype",
                    "v_owner");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Owner> owners = connection.Query<Owner, IdentificationType, Owner>(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    (o, it) =>
                    {
                        o.IdentificationType = it;
                        return o;
                    },
                    splitOn: "ididentificationtype"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(owners, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de titulares", ex);
            }
        }

        /// <summary>
        /// Consulta un titular dado su identificador
        /// </summary>
        /// <param name="entity">Titular a consultar</param>
        /// <returns>Titular con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar el titular</exception>
        public override Owner Read(Owner entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<Owner> result = connection.Query<Owner, IdentificationType, Owner>(
                    "SELECT " +
                    "idowner, name, identification, " +
                    "address_home, address_office, phone_home, " +
                    "phone_office, email, ididentificationtype, identificationtype " +
                    "FROM v_owner WHERE idowner = @Id",
                    (o, it) =>
                    {
                        o.IdentificationType = it;
                        return o;
                    },
                    entity,
                    splitOn: "ididentificationtype");
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
                throw new PersistentException("Error al consultar el titular", ex);
            }
        }

        /// <summary>
        /// Inserta un titular en la base de datos
        /// </summary>
        /// <param name="entity">Titular a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Titular insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar el titular</exception>
        public override Owner Insert(Owner entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO owner (name, identification, ididentificationtype, address_home, address_office, phone_home, phone_office, email) " +
                    "VALUES " +
                    "(@Name, @Identification, @IdIdentificationType, @AddressHome, @AddressOffice, @PhoneHome, @PhoneOffice, @Email); SELECT LAST_INSERT_ID();",
                    new
                    {
                        entity.Name,
                        entity.Identification,
                        IdIdentificationType = entity.IdentificationType.Id,
                        entity.AddressHome,
                        entity.AddressOffice,
                        entity.PhoneHome,
                        entity.PhoneOffice,
                        entity.Email
                    });
                LogInsert(
                    entity.Id,
                    "owner",
                    "INSERT INTO owner (name, identification, ididentificationtype, address_home, address_office, phone_home, phone_office, email) " +
                    "VALUES " +
                    "('" + entity.Name + "', '" + entity.Identification + "', " + entity.IdentificationType.Id + ", '" + entity.AddressHome + "', '" + entity.AddressOffice + "', " +
                    "'" + entity.PhoneHome + "', '" + entity.PhoneOffice + "', '" + entity.Email + "')",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar el titular", ex);
            }
        }

        /// <summary>
        /// Actualiza un titular en la base de datos
        /// </summary>
        /// <param name="entity">Titular a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Titular actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar el titular</exception>
        public override Owner Update(Owner entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "UPDATE owner " +
                    "SET name = @Name, identification = @Identification, ididentificationtype = @IdIdentificationType, " +
                    "address_home = @AddressHome, address_office = @AddressOffice, phone_home = @PhoneHome, " +
                    "phone_office = @PhoneOffice, email = @Email " +
                    "WHERE idowner = @Id",
                    new
                    {
                        entity.Id,
                        entity.Name,
                        entity.Identification,
                        IdIdentificationType = entity.IdentificationType.Id,
                        entity.AddressHome,
                        entity.AddressOffice,
                        entity.PhoneHome,
                        entity.PhoneOffice,
                        entity.Email
                    });
                LogUpdate(
                    entity.Id,
                    "owner",
                    "UPDATE owner " +
                    "SET name = '" + entity.Name + "', identification = '" + entity.Identification + "', ididentificationtype = " + entity.IdentificationType.Id + ", " +
                    "address_home = '" + entity.AddressHome + "', address_office = '" + entity.AddressOffice + "', phone_home = '" + entity.PhoneHome + "', " +
                    "phone_office = '" + entity.PhoneOffice + "', email = '" + entity.Email + "' " +
                    "WHERE idowner = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar el titular", ex);
            }
        }

        /// <summary>
        /// Elimina un titular de la base de datos
        /// </summary>
        /// <param name="entity">Titular a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Titular eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el titular</exception>
        public override Owner Delete(Owner entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM owner WHERE idowner = @Id", entity);
                LogDelete(entity.Id, "owner", "DELETE FROM owner WHERE idowner = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el titular", ex);
            }
        }
        #endregion
    }
}
