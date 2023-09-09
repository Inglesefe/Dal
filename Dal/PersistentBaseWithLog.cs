using Dal.Dto;
using Dal.Exceptions;
using Dal.Log;
using Dapper;
using Entities;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Entities.Crm;
using Entities.Noti;
using System.Reflection;

namespace Dal
{
    /// <summary>
    /// Clase base de la jerarquía de persistencias de entidades y que registra un log de auditoría en las inserciones, actualizaciones o eliminaciones
    /// </summary>
    /// <typeparam name="T">Tipo de la entidad a persistir</typeparam>
    public abstract class PersistentBaseWithLog<T> : IPersistentWithLog<T> where T : IEntity
    {
        #region Attributes
        /// <summary>
        /// Conexión a la base de datos
        /// </summary>
        protected readonly string _connString;

        /// <summary>
        /// Persistencia en base de datos de los logs de base de datos
        /// </summary>
        protected readonly PersistentLogDb persistentLogDb;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la conexión a la base de datos
        /// </summary>
        /// <param name="connString">Cadena de conexión a la base de datos</param>
        protected PersistentBaseWithLog(string connString)
        {
            _connString = connString;
            persistentLogDb = new PersistentLogDb(connString);
            SqlMapper.SetTypeMap(typeof(AccountExecutive), new CustomPropertyTypeMap(typeof(AccountExecutive), GetProperty));
            SqlMapper.SetTypeMap(typeof(Registration), new CustomPropertyTypeMap(typeof(Registration), GetProperty));
            SqlMapper.SetTypeMap(typeof(Application), new CustomPropertyTypeMap(typeof(Application), GetProperty));
            SqlMapper.SetTypeMap(typeof(Role), new CustomPropertyTypeMap(typeof(Role), GetProperty));
            SqlMapper.SetTypeMap(typeof(User), new CustomPropertyTypeMap(typeof(User), GetProperty));
            SqlMapper.SetTypeMap(typeof(City), new CustomPropertyTypeMap(typeof(City), GetProperty));
            SqlMapper.SetTypeMap(typeof(Country), new CustomPropertyTypeMap(typeof(Country), GetProperty));
            SqlMapper.SetTypeMap(typeof(IdentificationType), new CustomPropertyTypeMap(typeof(IdentificationType), GetProperty));
            SqlMapper.SetTypeMap(typeof(IncomeType), new CustomPropertyTypeMap(typeof(IncomeType), GetProperty));
            SqlMapper.SetTypeMap(typeof(Office), new CustomPropertyTypeMap(typeof(Office), GetProperty));
            SqlMapper.SetTypeMap(typeof(Parameter), new CustomPropertyTypeMap(typeof(Parameter), GetProperty));
            SqlMapper.SetTypeMap(typeof(Plan), new CustomPropertyTypeMap(typeof(Plan), GetProperty));
            SqlMapper.SetTypeMap(typeof(Scale), new CustomPropertyTypeMap(typeof(Scale), GetProperty));
            SqlMapper.SetTypeMap(typeof(AccountNumber), new CustomPropertyTypeMap(typeof(AccountNumber), GetProperty));
            SqlMapper.SetTypeMap(typeof(AccountType), new CustomPropertyTypeMap(typeof(AccountType), GetProperty));
            SqlMapper.SetTypeMap(typeof(ConsecutiveNumber), new CustomPropertyTypeMap(typeof(ConsecutiveNumber), GetProperty));
            SqlMapper.SetTypeMap(typeof(ConsecutiveType), new CustomPropertyTypeMap(typeof(ConsecutiveType), GetProperty));
            SqlMapper.SetTypeMap(typeof(Beneficiary), new CustomPropertyTypeMap(typeof(Beneficiary), GetProperty));
            SqlMapper.SetTypeMap(typeof(Owner), new CustomPropertyTypeMap(typeof(Owner), GetProperty));
            SqlMapper.SetTypeMap(typeof(Notification), new CustomPropertyTypeMap(typeof(Notification), GetProperty));
            SqlMapper.SetTypeMap(typeof(Template), new CustomPropertyTypeMap(typeof(Template), GetProperty));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Inserta un log de auditoría de base de datos
        /// </summary>
        /// <param name="action">Acción ejecutada I - Insertar, U - Actualizar, D - Eliminar</param>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        private void Log(string action, long id, string table, string sql, long user)
        {
            try
            {
                _ = persistentLogDb.Insert(new() { Action = action, IdTable = id, Table = table, Sql = sql, User = (int)user });
            }
            catch (PersistentException)
            {
                //No hacer nada
            }
        }

        /// <summary>
        /// Inserta un log de auditoría de una inserción en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        protected void LogInsert(long id, string table, string sql, long user)
        {
            Log("I", id, table, sql, user);
        }

        /// <summary>
        /// Inserta un log de auditoría de una actualización en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        protected void LogUpdate(long id, string table, string sql, long user)
        {
            Log("U", id, table, sql, user);
        }

        /// <summary>
        /// Inserta un log de auditoría de una eliminación en la base de datos
        /// </summary>
        /// <param name="id">Ientificador único en la tabla</param>
        /// <param name="table">Nombre de la tabla afectada</param>
        /// <param name="sql">SQL ejecutado</param>
        /// <param name="user">Ientificador del usuario que realiza la acción</param>
        protected void LogDelete(long id, string table, string sql, long user)
        {
            Log("D", id, table, sql, user);
        }

        /// <inheritdoc />
        public abstract ListResult<T> List(string filters, string orders, int limit, int offset);

        /// <inheritdoc />
        public abstract T Read(T entity);

        /// <inheritdoc />
        public abstract T Insert(T entity, User user);

        /// <inheritdoc />
        public abstract T Update(T entity, User user);

        /// <inheritdoc />
        public abstract T Delete(T entity, User user);

        /// <summary>
        /// Trae la propiedad correspondiente según la clase y nombre de la columna de la base de datos
        /// </summary>
        /// <param name="type">Clase a mapear</param>
        /// <param name="columnName">Nombre del campo de la base de datos</param>
        /// <returns>Propiedad de la clase mapeada</returns>
        private static PropertyInfo? GetProperty(Type type, string columnName)
        {
            if (type == typeof(AccountExecutive))
            {
                return columnName switch
                {
                    "idaccountexecutive" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "accountexecutive" => type.GetProperty("Name"),
                    "identification" => type.GetProperty("Identification"),
                    "account_executive_identification" => type.GetProperty("Identification"),
                    _ => null,
                };
            }
            if (type == typeof(Registration))
            {
                return columnName switch
                {
                    "idregistration" => type.GetProperty("Id"),
                    "date" => type.GetProperty("Date"),
                    "contract_number" => type.GetProperty("ContractNumber"),
                    _ => null,
                };
            }
            if (type == typeof(Application))
            {
                return columnName switch
                {
                    "idapplication" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "application" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(Role))
            {
                return columnName switch
                {
                    "idrole" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "role" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(User))
            {
                return columnName switch
                {
                    "iduser" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "user" => type.GetProperty("Name"),
                    "login" => type.GetProperty("Login"),
                    "active" => type.GetProperty("Active"),
                    _ => null,
                };
            }
            if (type == typeof(City))
            {
                return columnName switch
                {
                    "idcity" => type.GetProperty("Id"),
                    "office_idcity" => type.GetProperty("Id"),
                    "code" => type.GetProperty("Code"),
                    "city_code" => type.GetProperty("Code"),
                    "office_city_code" => type.GetProperty("Code"),
                    "name" => type.GetProperty("Name"),
                    "city_name" => type.GetProperty("Name"),
                    "office_city_name" => type.GetProperty("Name"),
                    "city" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(Country))
            {
                return columnName switch
                {
                    "idcountry" => type.GetProperty("Id"),
                    "office_idcountry" => type.GetProperty("Id"),
                    "code" => type.GetProperty("Code"),
                    "country_code" => type.GetProperty("Code"),
                    "office_country_code" => type.GetProperty("Code"),
                    "country_name" => type.GetProperty("Name"),
                    "office_country_name" => type.GetProperty("Name"),
                    "name" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(IdentificationType))
            {
                return columnName switch
                {
                    "ididentificationtype" => type.GetProperty("Id"),
                    "owner_ididentificationtype" => type.GetProperty("Id"),
                    "beneficiary1_ididentificationtype" => type.GetProperty("Id"),
                    "beneficiary2_ididentificationtype" => type.GetProperty("Id"),
                    "account_executive_ididentificationtype" => type.GetProperty("Id"),
                    "identificationtype" => type.GetProperty("Name"),
                    "beneficiary1_identificationtype" => type.GetProperty("Name"),
                    "beneficiary2_identificationtype" => type.GetProperty("Name"),
                    "owner_identificationtype" => type.GetProperty("Name"),
                    "account_executive_identificationtype" => type.GetProperty("Name"),
                    "name" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(IncomeType))
            {
                return columnName switch
                {
                    "idincometype" => type.GetProperty("Id"),
                    "code" => type.GetProperty("Code"),
                    "name" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(Office))
            {
                return columnName switch
                {
                    "idoffice" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "office" => type.GetProperty("Name"),
                    "address" => type.GetProperty("Address"),
                    "office_address" => type.GetProperty("Address"),
                    _ => null,
                };
            }
            if (type == typeof(Parameter))
            {
                return columnName switch
                {
                    "idparameter" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "value" => type.GetProperty("Value"),
                    _ => null,
                };
            }
            if (type == typeof(Plan))
            {
                return columnName switch
                {
                    "idplan" => type.GetProperty("Id"),
                    "value" => type.GetProperty("Value"),
                    "plan_value" => type.GetProperty("Value"),
                    "initial_fee" => type.GetProperty("InitialFee"),
                    "plan_initial_fee" => type.GetProperty("InitialFee"),
                    "installments_number" => type.GetProperty("InstallmentsNumber"),
                    "plan_installments_number" => type.GetProperty("InstallmentsNumber"),
                    "installment_value" => type.GetProperty("InstallmentValue"),
                    "plan_installment_value" => type.GetProperty("InstallmentValue"),
                    "active" => type.GetProperty("Active"),
                    "plan_active" => type.GetProperty("Active"),
                    "description" => type.GetProperty("Description"),
                    "plan_description" => type.GetProperty("Description"),
                    _ => null,
                };
            }
            if (type == typeof(Scale))
            {
                return columnName switch
                {
                    "idscale" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "scale" => type.GetProperty("Name"),
                    "comission" => type.GetProperty("Comission"),
                    "scale_comission" => type.GetProperty("Comission"),
                    "validity" => type.GetProperty("Validity"),
                    "scale_validity" => type.GetProperty("Validity"),
                    _ => null,
                };
            }
            if (type == typeof(AccountNumber))
            {
                return columnName switch
                {
                    "idaccountnumber" => type.GetProperty("Id"),
                    "number" => type.GetProperty("Number"),
                    _ => null,
                };
            }
            if (type == typeof(AccountType))
            {
                return columnName switch
                {
                    "idaccounttype" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "accounttype" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(ConsecutiveNumber))
            {
                return columnName switch
                {
                    "idconsecutivenumber" => type.GetProperty("Id"),
                    "number" => type.GetProperty("Number"),
                    _ => null,
                };
            }
            if (type == typeof(ConsecutiveType))
            {
                return columnName switch
                {
                    "idconsecutivetype" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "consecutivetype" => type.GetProperty("Name"),
                    _ => null,
                };
            }
            if (type == typeof(Beneficiary))
            {
                return columnName switch
                {
                    "idbeneficiary" => type.GetProperty("Id"),
                    "idbeneficiary1" => type.GetProperty("Id"),
                    "idbeneficiary2" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "beneficiary1" => type.GetProperty("Name"),
                    "beneficiary2" => type.GetProperty("Name"),
                    "identification" => type.GetProperty("Identification"),
                    "beneficiary1_identification" => type.GetProperty("Identification"),
                    "beneficiary2_identification" => type.GetProperty("Identification"),
                    "relationship" => type.GetProperty("Relationship"),
                    "beneficiary1_relationship" => type.GetProperty("Relationship"),
                    "beneficiary2_relationship" => type.GetProperty("Relationship"),
                    _ => null,
                };
            }
            if (type == typeof(Owner))
            {
                return columnName switch
                {
                    "idowner" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "owner" => type.GetProperty("Name"),
                    "identification" => type.GetProperty("Identification"),
                    "owner_identification" => type.GetProperty("Identification"),
                    "address_home" => type.GetProperty("AddressHome"),
                    "owner_address_home" => type.GetProperty("AddressHome"),
                    "address_office" => type.GetProperty("AddressOffice"),
                    "owner_address_office" => type.GetProperty("AddressOffice"),
                    "phone_home" => type.GetProperty("PhoneHome"),
                    "owner_phone_home" => type.GetProperty("PhoneHome"),
                    "phone_office" => type.GetProperty("PhoneOffice"),
                    "owner_phone_office" => type.GetProperty("PhoneOffice"),
                    "email" => type.GetProperty("Email"),
                    "owner_email" => type.GetProperty("Email"),
                    _ => null,
                };
            }
            if (type == typeof(Notification))
            {
                return columnName switch
                {
                    "idnotification" => type.GetProperty("Id"),
                    "date" => type.GetProperty("Date"),
                    "to" => type.GetProperty("To"),
                    "subject" => type.GetProperty("Subject"),
                    "content" => type.GetProperty("Content"),
                    "iduser" => type.GetProperty("User"),
                    _ => null,
                };
            }
            if (type == typeof(Template))
            {
                return columnName switch
                {
                    "idtemplate" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "content" => type.GetProperty("Content"),
                    _ => null,
                };
            }
            return null;
        }
        #endregion
    }
}
