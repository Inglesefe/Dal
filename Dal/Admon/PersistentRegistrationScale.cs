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
    /// Realiza la persistencia de las relaciones entre matrículas, escalas y ejecutivos de cuenta en la base de datos
    /// </summary>
    public class PersistentRegistrationScale : PersistentBaseWithLog<RegistrationScale>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentRegistrationScale(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de relaciones entre matrículas, escalas y ejecutivos de cuenta desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de relaciones entre matrículas, escalas y ejecutivos de cuenta</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las relaciones entre matrículas, escalas y ejecutivos de cuenta</exception>
        public override ListResult<RegistrationScale> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idregistrationscale, idregistration, idscale, scale, scale_comission, scale_validity, " +
                    "idaccountexecutive, account_executive_identification, account_executive_ididentificationtype, account_executive_identificationtype",
                    "v_registration_scale");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<RegistrationScale> executives = connection.Query(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    new[]
                    {
                    typeof(RegistrationScale),
                    typeof(Registration),
                    typeof(Scale),
                    typeof(AccountExecutive),
                    typeof(IdentificationType)
                    },
                    objects =>
                    {
                        RegistrationScale rs = (objects[0] as RegistrationScale) ?? new RegistrationScale();
                        Registration r = (objects[1] as Registration) ?? new Registration();
                        Scale s = (objects[2] as Scale) ?? new Scale();
                        AccountExecutive ae = (objects[3] as AccountExecutive) ?? new AccountExecutive();
                        IdentificationType it = (objects[4] as IdentificationType) ?? new IdentificationType();
                        ae.IdentificationType = it;
                        rs.Registration = r;
                        rs.Scale = s;
                        rs.AccountExecutive = ae;
                        return rs;
                    },
                    splitOn: "idregistration, idscale, idaccountexecutive, account_executive_ididentificationtype"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(executives, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de relaciones entre matrículas, escalas y ejecutivos de cuenta", ex);
            }
        }

        /// <summary>
        /// Consulta una relación entre matrícula, escala y ejecutivo de cuenta dado su identificador
        /// </summary>
        /// <param name="entity">Relación entre matrícula, escala y ejecutivo de cuenta a consultar</param>
        /// <returns>Relación entre matrícula, escala y ejecutivo con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la relación entre matrícula, escala y ejecutivo de cuenta</exception>
        public override RegistrationScale Read(RegistrationScale entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<RegistrationScale> result = connection.Query(
                    "SELECT " +
                    "idregistrationscale, idregistration, idscale, scale, scale_comission, scale_validity, " +
                    "idaccountexecutive, account_executive_identification, account_executive_ididentificationtype, account_executive_identificationtype " +
                    "FROM v_registration_scale WHERE idregistrationscale = @Id",
                    new[]
                    {
                    typeof(RegistrationScale),
                    typeof(Registration),
                    typeof(Scale),
                    typeof(AccountExecutive),
                    typeof(IdentificationType)
                    },
                    objects =>
                    {
                        RegistrationScale rs = (objects[0] as RegistrationScale) ?? new RegistrationScale();
                        Registration r = (objects[1] as Registration) ?? new Registration();
                        Scale s = (objects[2] as Scale) ?? new Scale();
                        AccountExecutive ae = (objects[3] as AccountExecutive) ?? new AccountExecutive();
                        IdentificationType it = (objects[4] as IdentificationType) ?? new IdentificationType();
                        ae.IdentificationType = it;
                        rs.Registration = r;
                        rs.Scale = s;
                        rs.AccountExecutive = ae;
                        return rs;
                    },
                    entity,
                    splitOn: "idregistration, idscale, idaccountexecutive, account_executive_ididentificationtype"
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
                throw new PersistentException("Error al consultar la relación entre matrícula, escala y ejecutivo", ex);
            }
        }

        /// <summary>
        /// Inserta una relación entre matrícula, escala y ejecutivo de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Relación entre matrícula, escala y ejecutivo de cuenta a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Relación entre matrícula, escala y ejecutivo de cuenta insertada con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la relación entre matrícula, escala y ejecutivo de cuenta</exception>
        public override RegistrationScale Insert(RegistrationScale entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO registration_scale " +
                    "(idregistration, idscale, idaccountexecutive) " +
                    "VALUES " +
                    "(@IdRegistration, @IdScale, @IdAccountExecutive); " +
                    "SELECT LAST_INSERT_ID();",
                    new
                    {
                        IdRegistration = entity.Registration.Id,
                        IdScale = entity.Scale.Id,
                        IdAccountExecutive = entity.AccountExecutive.Id
                    });
                LogInsert(
                    entity.Id,
                    "registration_scale",
                    "INSERT INTO registration_scale (idregistration, idscale, idaccountexecutive) " +
                    "VALUES (" + entity.Registration.Id + ", " + entity.Scale.Id + ", " + entity.AccountExecutive.Id + ")",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la relación entre matrícula, escala y ejecutivo de cuenta", ex);
            }
        }

        /// <summary>
        /// Actualiza una relación entre matrícula, escala y ejecutivo de cuenta en la base de datos
        /// </summary>
        /// <param name="entity">Relación entre matrícula, escala y ejecutivo de cuenta a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Relación entre matrícula, escala y ejecutivo de cuenta actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la relación entre matrícula, escala y ejecutivo de cuenta</exception>
        public override RegistrationScale Update(RegistrationScale entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "UPDATE registration_scale SET idscale = @IdScale, idaccountexecutive = @IdAccountExecutive WHERE idaccountexecutive = @Id",
                    new
                    {
                        entity.Id,
                        IdScale = entity.Scale.Id,
                        IdAccountExecutive = entity.AccountExecutive.Id
                    });
                LogUpdate(
                    entity.Id,
                    "registration_scale",
                    "UPDATE registration_scale SET idscale = " + entity.Scale.Id + ", idaccountexecutive = '" + entity.AccountExecutive.Id + " WHERE idaccountexecutive = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la relación entre matrícula, escala y ejecutivo de cuenta", ex);
            }
        }

        /// <summary>
        /// Elimina una relación entre matrícula, escala y ejecutivo de cuenta de la base de datos
        /// </summary>
        /// <param name="entity">Relación entre matrícula, escala y ejecutivo de cuenta a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Relación entre matrícula, escala y ejecutivo de cuenta eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la relación entre matrícula, escala y ejecutivo de cuenta</exception>
        public override RegistrationScale Delete(RegistrationScale entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM registration_scale WHERE idregistrationscale = @Id", entity);
                LogDelete(entity.Id, "registration_scale", "DELETE FROM registration_scale WHERE idregistrationscale = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la relación entre matrícula, escala y ejecutivo de cuenta", ex);
            }
        }
        #endregion
    }
}
