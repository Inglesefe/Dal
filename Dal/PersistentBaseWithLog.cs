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
            SqlMapper.SetTypeMap(typeof(AccountExecutive), new CustomPropertyTypeMap(typeof(AccountExecutive), GetMapColumnsAccountExecutive()));
            SqlMapper.SetTypeMap(typeof(Registration), new CustomPropertyTypeMap(typeof(Registration), GetMapColumnsRegistration()));
            SqlMapper.SetTypeMap(typeof(Application), new CustomPropertyTypeMap(typeof(Application), GetMapColumnsApplication()));
            SqlMapper.SetTypeMap(typeof(Role), new CustomPropertyTypeMap(typeof(Role), GetMapColumnsRole()));
            SqlMapper.SetTypeMap(typeof(User), new CustomPropertyTypeMap(typeof(User), GetMapColumnsUser()));
            SqlMapper.SetTypeMap(typeof(City), new CustomPropertyTypeMap(typeof(City), GetMapColumnsCity()));
            SqlMapper.SetTypeMap(typeof(Country), new CustomPropertyTypeMap(typeof(Country), GetMapColumnsCountry()));
            SqlMapper.SetTypeMap(typeof(IdentificationType), new CustomPropertyTypeMap(typeof(IdentificationType), GetMapColumnsIdentificationType()));
            SqlMapper.SetTypeMap(typeof(IncomeType), new CustomPropertyTypeMap(typeof(IncomeType), GetMapColumnsIncomeType()));
            SqlMapper.SetTypeMap(typeof(Office), new CustomPropertyTypeMap(typeof(Office), GetMapColumnsOffice()));
            SqlMapper.SetTypeMap(typeof(Parameter), new CustomPropertyTypeMap(typeof(Parameter), GetMapColumnsParameter()));
            SqlMapper.SetTypeMap(typeof(Plan), new CustomPropertyTypeMap(typeof(Plan), GetMapColumnsPlan()));
            SqlMapper.SetTypeMap(typeof(Scale), new CustomPropertyTypeMap(typeof(Scale), GetMapColumnsScale()));
            SqlMapper.SetTypeMap(typeof(AccountNumber), new CustomPropertyTypeMap(typeof(AccountNumber), GetMapColumnsAccountNumber()));
            SqlMapper.SetTypeMap(typeof(AccountType), new CustomPropertyTypeMap(typeof(AccountType), GetMapColumnsAccountType()));
            SqlMapper.SetTypeMap(typeof(ConsecutiveNumber), new CustomPropertyTypeMap(typeof(ConsecutiveNumber), GetMapColumnsConsecutiveNumber()));
            SqlMapper.SetTypeMap(typeof(ConsecutiveType), new CustomPropertyTypeMap(typeof(ConsecutiveType), GetMapColumnsConsecutiveType()));
            SqlMapper.SetTypeMap(typeof(Beneficiary), new CustomPropertyTypeMap(typeof(Beneficiary), GetMapColumnsBeneficiary()));
            SqlMapper.SetTypeMap(typeof(Owner), new CustomPropertyTypeMap(typeof(Owner), GetMapColumnsOwner()));
            SqlMapper.SetTypeMap(typeof(Notification), new CustomPropertyTypeMap(typeof(Notification), GetMapColumnsNotification()));
            SqlMapper.SetTypeMap(typeof(Template), new CustomPropertyTypeMap(typeof(Template), GetMapColumnsTemplate()));
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
        /// Retorna el mapeo de las columnas para la clase AccountExecutive
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase AccountExecutive</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsAccountExecutive()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Registration
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Registration</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsRegistration()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idregistration" => type.GetProperty("Id"),
                    "date" => type.GetProperty("Date"),
                    "contract_number" => type.GetProperty("ContractNumber"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Application
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Application</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsApplication()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idapplication" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "application" => type.GetProperty("Name"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Role
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Role</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsRole()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idrole" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "role" => type.GetProperty("Name"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase User
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase User</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsUser()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase City
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase City</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsCity()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Country
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Country</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsCountry()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase IdentificationType
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase IdentificationType</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsIdentificationType()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase IncomeType
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase IncomeType</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsIncomeType()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idincometype" => type.GetProperty("Id"),
                    "code" => type.GetProperty("Code"),
                    "name" => type.GetProperty("Name"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Office
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Office</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsOffice()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Parameter
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Parameter</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsParameter()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idparameter" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "value" => type.GetProperty("Value"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Plan
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Plan</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsPlan()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Scale
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Scale</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsScale()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase AccountNumber
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase AccountNumber</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsAccountNumber()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idaccountnumber" => type.GetProperty("Id"),
                    "number" => type.GetProperty("Number"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase AccountType
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase AccountType</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsAccountType()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idaccounttype" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "accounttype" => type.GetProperty("Name"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase ConsecutiveNumber
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase ConsecutiveNumber</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsConsecutiveNumber()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idconsecutivenumber" => type.GetProperty("Id"),
                    "number" => type.GetProperty("Number"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase ConsecutiveType
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase ConsecutiveType</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsConsecutiveType()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idconsecutivetype" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "consecutivetype" => type.GetProperty("Name"),
                    _ => null,
                };
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Beneficiary
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Beneficiary</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsBeneficiary()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Owner
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Owner</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsOwner()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Notification
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Notification</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsNotification()
        {
            return (type, columnName) =>
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
            };
        }

        /// <summary>
        /// Retorna el mapeo de las columnas para la clase Template
        /// </summary>
        /// <returns>Mapeo de las columnas para la clase Template</returns>
        private static Func<Type, string, PropertyInfo> GetMapColumnsTemplate()
        {
            return (type, columnName) =>
            {
                return columnName switch
                {
                    "idtemplate" => type.GetProperty("Id"),
                    "name" => type.GetProperty("Name"),
                    "content" => type.GetProperty("Content"),
                    _ => null,
                };
            };
        }
        #endregion
    }
}
