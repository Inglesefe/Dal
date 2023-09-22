using Dal.Dto;
using Dal.Exceptions;
using Dal.Utils;
using Dapper;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Entities.Crm;
using MySql.Data.MySqlClient;
using System.Data;

namespace Dal.Admon
{
    /// <summary>
    /// Realiza la persistencia de las matrículas en la base de datos
    /// </summary>
    public class PersistentRegistration : PersistentBaseWithLog<Registration>
    {
        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        public PersistentRegistration(string connString) : base(connString) { }
        #endregion

        #region Methods
        /// <summary>
        /// Trae un listado de matrículas desde la base de datos
        /// </summary>
        /// <param name="filters">Filtros aplicados a la consulta</param>
        /// <param name="orders">Ordenamientos aplicados a la base de datos</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que se cuenta el número de registros</param>
        /// <returns>Listado de matrículas</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar las matrículas</exception>
        public override ListResult<Registration> List(string filters, string orders, int limit, int offset)
        {
            try
            {
                QueryBuilder queryBuilder = new(
                    "idregistration, date, contract_number, idoffice, office, office_address, office_phone, office_active, office_idcity, office_city_code, office_city_name, " +
                    "office_idcountry, office_country_code, office_country_name, idowner, owner, owner_identification, owner_address_home, owner_address_office, owner_phone_home, " +
                    "owner_phone_office, owner_email, owner_ididentificationtype, owner_identificationtype, idbeneficiary1, beneficiary1, beneficiary1_identification, " +
                    "beneficiary1_relationship, beneficiary1_ididentificationtype, beneficiary1_identificationtype, idbeneficiary2, beneficiary2, beneficiary2_identification, " +
                    "beneficiary2_relationship, beneficiary2_ididentificationtype, beneficiary2_identificationtype, idplan, plan_value, plan_initial_fee, plan_installments_number, " +
                    "plan_installment_value, plan_active, plan_description",
                    "v_registration");
                using IDbConnection connection = new MySqlConnection(_connString);
                List<Registration> executives = connection.Query(
                    queryBuilder.GetSelectForList(filters, orders, limit, offset),
                    GetTypes(),
                    Map(),
                    splitOn: "idoffice, office_idcity, office_idcountry, " +
                    "idowner, owner_ididentificationtype, idbeneficiary1, " +
                    "beneficiary1_ididentificationtype, idbeneficiary2, beneficiary2_ididentificationtype, " +
                    "idplan"
                ).ToList();
                int total = connection.ExecuteScalar<int>(queryBuilder.GetCountTotalSelectForList(filters, orders));
                return new(executives, total);
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al consultar el listado de matrículas", ex);
            }
        }

        /// <summary>
        /// Consulta una matrícula dado su identificador
        /// </summary>
        /// <param name="entity">Matrícula a consultar</param>
        /// <returns>Matrícula con los datos cargados desde la base de datos o datos por defecto si no lo pudo encontrar</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al consultar la matrícula</exception>
        public override Registration Read(Registration entity)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                IEnumerable<Registration> result = connection.Query(
                    "SELECT " +
                    "idregistration, date, contract_number, idoffice, office, office_address, office_phone, office_active, office_idcity, office_city_code, office_city_name, " +
                    "office_idcountry, office_country_code, office_country_name, idowner, owner, owner_identification, owner_address_home, owner_address_office, owner_phone_home, " +
                    "owner_phone_office, owner_email, owner_ididentificationtype, owner_identificationtype, idbeneficiary1, beneficiary1, beneficiary1_identification, " +
                    "beneficiary1_relationship, beneficiary1_ididentificationtype, beneficiary1_identificationtype, idbeneficiary2, beneficiary2, beneficiary2_identification, " +
                    "beneficiary2_relationship, beneficiary2_ididentificationtype, beneficiary2_identificationtype, idplan, plan_value, plan_initial_fee, plan_installments_number, " +
                    "plan_installment_value, plan_active, plan_description " +
                    "FROM v_registration WHERE idregistration = @Id",
                    GetTypes(),
                    Map(),
                    entity,
                    splitOn: "idoffice, office_idcity, office_idcountry, " +
                    "idowner, owner_ididentificationtype, idbeneficiary1, " +
                    "beneficiary1_ididentificationtype, idbeneficiary2, beneficiary2_ididentificationtype, " +
                    "idplan"
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
                throw new PersistentException("Error al consultar la matrícula", ex);
            }
        }

        /// <summary>
        /// Inserta una matrícula en la base de datos
        /// </summary>
        /// <param name="entity">Matrícula a insertar</param>
        /// <param name="user">Usuario que realiza la inserción</param>
        /// <returns>Matrícula insertado con el id generado por la base de datos</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al insertar la matrícula</exception>
        public override Registration Insert(Registration entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                entity.Id = connection.QuerySingle<int>(
                    "INSERT INTO registration " +
                    "(idoffice, date, contract_number, idowner, idbeneficiary1, idbeneficiary2, idplan) " +
                    "VALUES " +
                    "(@IdOffice, @Date, @ContractNumber, @IdOwner, @IdBeneficiary1, @IdBeneficiary2, @IdPlan); " +
                    "SELECT LAST_INSERT_ID();",
                    new
                    {
                        IdOffice = entity.Office.Id,
                        entity.Date,
                        entity.ContractNumber,
                        IdOwner = entity.Owner.Id,
                        IdBeneficiary1 = entity.Beneficiary1?.Id,
                        IdBeneficiary2 = entity.Beneficiary2?.Id,
                        IdPlan = entity.Plan.Id
                    });
                LogInsert(
                    entity.Id,
                    "registration",
                    "INSERT INTO registration (idoffice, date, contract_number, idowner, idbeneficiary1, idbeneficiary2, idplan) " +
                    "VALUES " +
                    "(" + entity.Office.Id + ", '" + entity.Date.ToString("yyyy-MM-dd") + "', '" + entity.ContractNumber + "', " +
                    entity.Owner.Id + ", " + (entity.Beneficiary1?.Id.ToString() ?? "NULL") + ", " + (entity.Beneficiary2?.Id.ToString() ?? "NULL") + ", " + entity.Plan.Id + ")",
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al insertar la matrícula", ex);
            }
        }

        /// <summary>
        /// Actualiza una matrícula en la base de datos
        /// </summary>
        /// <param name="entity">Matrícula a actualizar</param>
        /// <param name="user">Usuario que realiza la actualización</param>
        /// <returns>Matrícula actualizado</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al actualizar la matrícula</exception>
        public override Registration Update(Registration entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute(
                    "UPDATE registration SET " +
                    "idoffice = @IdOffice, date = @Date, contract_number = @ContractNumber, " +
                    "idowner = @IdOwner, idbeneficiary1 = @IdBeneficiary1, idbeneficiary2 = @IdBeneficiary2, " +
                    "idplan = @IdPlan WHERE idregistration = @Id",
                    new
                    {
                        entity.Id,
                        IdOffice = entity.Office.Id,
                        entity.Date,
                        entity.ContractNumber,
                        IdOwner = entity.Owner.Id,
                        IdBeneficiary1 = entity.Beneficiary1?.Id,
                        IdBeneficiary2 = entity.Beneficiary2?.Id,
                        IdPlan = entity.Plan.Id
                    });
                LogUpdate(
                    entity.Id,
                    "registration",
                    "UPDATE registration SET idoffice = " + entity.Office.Id + ", date = '" + entity.Date.ToString("yyyy-MM-dd") + "', contract_number = '" + entity.ContractNumber + "', " +
                    "idowner = " + entity.Owner.Id + ", idbeneficiary1 = " + (entity.Beneficiary1?.Id.ToString() ?? "NULL") + ", idbeneficiary2 = " + (entity.Beneficiary2?.Id.ToString() ?? "NULL") + ", " +
                    "idplan = " + entity.Plan.Id + " WHERE idregistration = " + entity.Id,
                    user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al actualizar la matrícula", ex);
            }
        }

        /// <summary>
        /// Elimina una matrícula de la base de datos
        /// </summary>
        /// <param name="entity">Matrícula a eliminar</param>
        /// <param name="user">Usuario que realiza la eliminación</param>
        /// <returns>Matrícula eliminada</returns>
        /// <exception cref="PersistentException">Si hubo una excepción al eliminar la matrícula</exception>
        public override Registration Delete(Registration entity, User user)
        {
            try
            {
                using IDbConnection connection = new MySqlConnection(_connString);
                _ = connection.Execute("DELETE FROM registration WHERE idregistration = @Id", entity);
                LogDelete(entity.Id, "registration", "DELETE FROM registration WHERE idregistration = " + entity.Id, user.Id);
                return entity;
            }
            catch (Exception ex)
            {
                throw new PersistentException("Error al eliminar la matrícula", ex);
            }
        }

        /// <summary>
        /// Trae los tipos de datos a mapear
        /// </summary>
        /// <returns>Tipos de datos a mapear</returns>
        private static Type[] GetTypes()
        {
            return new[] {
                typeof(Registration),
                typeof(Office),
                typeof(City),
                typeof(Country),
                typeof(Owner),
                typeof(IdentificationType),
                typeof(Beneficiary),
                typeof(IdentificationType),
                typeof(Beneficiary),
                typeof(IdentificationType),
                typeof(Plan)
            };
        }

        /// <summary>
        /// Realiza el mapeo
        /// </summary>
        /// <returns>Función de mapeo</returns>
        private static Func<object[], Registration> Map()
        {
            return objects =>
            {
                Registration r = (objects[0] as Registration) ?? new Registration();
                Office o = (objects[1] as Office) ?? new Office();
                City ci = (objects[2] as City) ?? new City();
                Country co = (objects[3] as Country) ?? new Country();
                Owner ow = (objects[4] as Owner) ?? new Owner();
                IdentificationType it = (objects[5] as IdentificationType) ?? new IdentificationType();
                Beneficiary b1 = (objects[6] as Beneficiary) ?? new Beneficiary();
                IdentificationType itb1 = (objects[7] as IdentificationType) ?? new IdentificationType();
                Beneficiary b2 = (objects[8] as Beneficiary) ?? new Beneficiary();
                IdentificationType itb2 = (objects[9] as IdentificationType) ?? new IdentificationType();
                Plan p = (objects[10] as Plan) ?? new Plan();
                ci.Country = co;
                o.City = ci;
                r.Office = o;
                ow.IdentificationType = it;
                b1.IdentificationType = itb1;
                r.Beneficiary1 = b1;
                b2.IdentificationType = itb2;
                r.Beneficiary2 = b2;
                r.Owner = ow;
                r.Plan = p;
                return r;
            };
        }
        #endregion
    }
}
