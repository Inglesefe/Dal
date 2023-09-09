using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Config
{
    /// <summary>
    /// Realiza la persistencia de las oficinas en la base de datos
    /// </summary>
    public class PersistentOffice : PersistentBaseWithLog<Office>, IPersistentOffice
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        /// </summary>
        public PersistentOffice(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de oficinas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de oficinas</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las oficinas</exception>
        public override ListResult<Office> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new("idoffice, name, address, idcity, city, city_code, idcountry, country_name, country_code", "v_office");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Office> offices = connection.Query<Office, City, Country, Office>(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    (o, ci, co) =>
                    {
                        ci.Country = co;
                        o.City = ci;
                        return o;
                    },
                    splitOn: "idcity, idcountry"
                    ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(offices, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de oficinas", ex);
            }
        }

        /// <summary>
        /// Consulta una oficina dado su identificador
        /// </summary>
        /// <param name="entity">Oficina a consultar</param>
        /// <returns>Oficina con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la oficina</exception>
        public override Office Read(Office entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<Office> result = connection.Query<Office, City, Country, Office>(
                    "SELECT idoffice, name, address, idcity, city, city_code, idcountry, country_name, country_code FROM v_office WHERE idoffice = @Id",
                    (o, ci, co) =>
                    {
                        ci.Country = co;
                        o.City = ci;
                        return o;
                    },
                    entity,
                    splitOn: "idcity, idcountry");
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
                throw new PersistentException("Error al consultar la aplicación", ex);
            }
        }

        /// <summary>
        /// Inserta una oficina en la base de datos
        /// </summary>
        /// <param name="entity">Oficina a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Oficina insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la oficina</exception>
        public override Office Insert(Office entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>("INSERT INTO office (idcity, name, address, phone, active) VALUES (@IdCity, @Name, @Address, @Phone, @Active); SELECT LAST_INSERT_ID();",
                    new { @IdCity = entity.City.Id, entity.Name, entity.Address, entity.Phone, entity.Active });
                LogInsert(entity.Id, "office", "INSERT INTO office (idcity, name, address, phone, active) VALUES " +
                    "(" + entity.City.Id + ", '" + entity.Name + "', '" + entity.Address + "', '" + entity.Phone + "', " + (entity.Active ? "1" : "0") + ")", user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la oficina", ex);
            }
        }

        /// <summary>
        /// Actualiza una oficina en la base de datos
        /// </summary>
        /// <param name="entity">Oficina a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Oficina actualizada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la oficina</exception>
        public override Office Update(Office entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("UPDATE office SET name = @Name, address = @Address, phone = @Phone, active = @Active WHERE idoffice = @Id", entity);
                LogUpdate(entity.Id, "office",
                    "UPDATE office SET name = '" + entity.Name + "', address = '" + entity.Address + "', phone = '" + entity.Phone + "', " +
                    "active = " + (entity.Active ? "1" : "0") + " WHERE idoffice = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la oficina", ex);
            }
        }

        /// <summary>
        /// Elimina una oficina de la base de datos
        /// </summary>
        /// <param name="entity">Oficina a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Oficina eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la oficina</exception>
        public override Office Delete(Office entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM office WHERE idoffice = @Id", entity);
                LogDelete(entity.Id, "office", "DELETE FROM office WHERE idoffice = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la oficina", ex);
            }
        }

        /// <summary>
        /// Trae un listado de ejecutivos de cuenta asignados a una oficina desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="office">Oficina a la que se le consultan los ejecutivos de cuenta asignados</param>
        /// <returns>Listado de ejecutivos de cuenta asignados a la oficina</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        public ListResult<AccountExecutive> ListAccountExecutives(string filters, string orders, int limit, int offset, Office office)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idaccountexecutive, accountexecutive, identification, ididentificationtype, identificationtype",
                    "v_executive_office");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<AccountExecutive> executives = connection.Query<AccountExecutive, IdentificationType, AccountExecutive>(
                    queryBuilder.GetSelectForList("idoffice = " + office.Id + (filters != "" ? " AND " : "") + filters, orders, limit, offset),
                    (ae, it) =>
                    {
                        ae.IdentificationType = it;
                        return ae;
                    },
                    splitOn: "ididentificationtype").ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList("idoffice = " + office.Id + (filters != "" ? " AND " : "") + filters, orders));
                return new(executives, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de ejecutivos de cuenta asignados a la oficina", ex);
            }
        }

        /// <summary>
        /// Trae un listado de ejecutivos de cuenta no asignados a una oficina desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <param name="office">Oficina a la que se le consultan los ejecutivos de cuenta no asignados</param>
        /// <returns>Listado de ejecutivos de cuenta no asignados a la oficina</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar los ejecutivos de cuenta</exception>
        public ListResult<AccountExecutive> ListNotAccountExecutives(string filters, string orders, int limit, int offset, Office office)
        {
            try
            {
                QueryBuilder queryBuilder = new("idaccountexecutive, name, ididentificationtype, identificationtype", "v_account_executive");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<AccountExecutive> roles = connection.Query<AccountExecutive, IdentificationType, AccountExecutive>(
                        queryBuilder.GetSelectForList(
                            "idaccountexecutive NOT IN (SELECT idaccountexecutive FROM executive_office WHERE idoffice = " + office.Id + ")" + (filters != "" ? " AND " : "") + filters,
                            orders,
                            limit,
                            offset),
                        (ae, it) =>
                        {
                            ae.IdentificationType = it;
                            return ae;
                        },
                        splitOn: "ididentificationtype").ToList();
                int total = connection.ExecuteScalar<int>(
                    queryBuilder.GetCountTotalSelectForList(
                        "idaccountexecutive NOT IN (SELECT idaccountexecutive FROM executive_office WHERE idoffice = " + office.Id + ")" + (filters != "" ? " AND " : "") + filters,
                        orders));
                return new(roles, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de ejecutivos de cuenta no asignados a la oficina", ex);
            }
        }

        /// <summary>
        /// Asigna un ejecutivo de cuenta a una oficina en la base de datos
        /// </summary>
        /// <param name="executive">Ejecutivo de cuenta que se asigna a la oficina</param>
        /// <param name="office">Oficina a la que se le asigna el ejecutivo de cuenta</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Ejecutivo de cuenta asignado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al asignar el ejecutivo de cuenta a la oficina</exception>
        public AccountExecutive InsertAccountExecutive(AccountExecutive executive, Office office, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "INSERT INTO executive_office (idoffice, idaccountexecutive) VALUES (@IdOffice, @IdAccountExecutive)",
                    new { IdOffice = office.Id, IdAccountExecutive = executive.Id });
                LogInsert(0, "executive_office", "INSERT INTO executive_office (idoffice, idaccountexecutive) VALUES (" + office.Id + ", " + executive.Id + ")", user.Id);
                return executive;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al asignar el ejecutivo de cuenta a la oficina", ex);
            }
        }

        /// <summary>
        /// Elimina un ejecutivo de cuenta de una oficina de la base de datos
        /// </summary>
        /// <param name="executive">Ejecutivo de cuenta a eliminarle a la oficina</param>
        /// <param name="office">Oficina al que se le elimina el ejecutivo de cuenta</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Ejecutivo de cuenta eliminado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar el ejecutivo de cuenta de la oficina</exception>
        public AccountExecutive DeleteAccountExecutive(AccountExecutive executive, Office office, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "DELETE FROM executive_office WHERE idoffice = @IdOffice AND idaccountexecutive = @IdAccountExecutive",
                    new { IdOffice = office.Id, IdAccountExecutive = executive.Id });
                LogDelete(0, "executive_office", "DELETE FROM executive_office WHERE idoffice = " + office.Id + " AND idaccountexecutive = " + executive.Id, user.Id);
                return executive;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar el ejecutivo de cuenta de la oficina", ex);
            }
        }
        #endregion
    }
}
