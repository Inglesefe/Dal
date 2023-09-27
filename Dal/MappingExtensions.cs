using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Entities.Cont;
using Entities.Crm;
using Entities.Log;
using Entities.Noti;
using System.Reflection;

namespace Dal
{
    /// <summary>
    /// Mapeo de las propiedades delas entidades con los campos de base de datos
    /// </summary>
    public static class MappingExtensions
    {
        #region Methods
        /// <summary>
        /// Mapeos de la entidad AccountExecutive
        /// </summary>
        /// <param name="obj">Objeto de la clase AccountExecutive</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this AccountExecutive obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Registration
        /// </summary>
        /// <param name="obj">Objeto de la clase Registration</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Registration obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idregistration" => type.GetProperty("Id"),
                "date" => type.GetProperty("Date"),
                "contract_number" => type.GetProperty("ContractNumber"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Application
        /// </summary>
        /// <param name="obj">Objeto de la clase Application</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Application obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idapplication" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "application" => type.GetProperty("Name"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Role
        /// </summary>
        /// <param name="obj">Objeto de la clase Role</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Role obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idrole" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "role" => type.GetProperty("Name"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad User
        /// </summary>
        /// <param name="obj">Objeto de la clase User</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this User obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad City
        /// </summary>
        /// <param name="obj">Objeto de la clase City</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this City obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Country
        /// </summary>
        /// <param name="obj">Objeto de la clase Country</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Country obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad IdentificationType
        /// </summary>
        /// <param name="obj">Objeto de la clase IdentificationType</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this IdentificationType obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad IncomeType
        /// </summary>
        /// <param name="obj">Objeto de la clase IncomeType</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this IncomeType obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idincometype" => type.GetProperty("Id"),
                "code" => type.GetProperty("Code"),
                "incometype_code" => type.GetProperty("Code"),
                "name" => type.GetProperty("Name"),
                "incometype" => type.GetProperty("Name"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Office
        /// </summary>
        /// <param name="obj">Objeto de la clase Office</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Office obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Parameter
        /// </summary>
        /// <param name="obj">Objeto de la clase Parameter</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Parameter obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idparameter" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "value" => type.GetProperty("Value"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Plan
        /// </summary>
        /// <param name="obj">Objeto de la clase Plan</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Plan obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Scale
        /// </summary>
        /// <param name="obj">Objeto de la clase Scale</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Scale obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idscale" => type.GetProperty("Id"),
                "code" => type.GetProperty("Code"),
                "scale_code" => type.GetProperty("Code"),
                "name" => type.GetProperty("Name"),
                "scale" => type.GetProperty("Name"),
                "comission" => type.GetProperty("Comission"),
                "scale_comission" => type.GetProperty("Comission"),
                "order" => type.GetProperty("Order"),
                "scale_order" => type.GetProperty("Order"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad AccountNumber
        /// </summary>
        /// <param name="obj">Objeto de la clase AccountNumber</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this AccountNumber obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idaccountnumber" => type.GetProperty("Id"),
                "number" => type.GetProperty("Number"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad AccountType
        /// </summary>
        /// <param name="obj">Objeto de la clase AccountType</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this AccountType obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idaccounttype" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "accounttype" => type.GetProperty("Name"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad ConsecutiveNumber
        /// </summary>
        /// <param name="obj">Objeto de la clase AccountType</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this ConsecutiveNumber obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idconsecutivenumber" => type.GetProperty("Id"),
                "number" => type.GetProperty("Number"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad ConsecutiveType
        /// </summary>
        /// <param name="obj">Objeto de la clase ConsecutiveType</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this ConsecutiveType obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idconsecutivetype" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "consecutivetype" => type.GetProperty("Name"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Beneficiary
        /// </summary>
        /// <param name="obj">Objeto de la clase Beneficiary</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Beneficiary obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Owner
        /// </summary>
        /// <param name="obj">Objeto de la clase Owner</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Owner obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Notification
        /// </summary>
        /// <param name="obj">Objeto de la clase Notification</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Notification obj, string columnName)
        {
            Type type = obj.GetType();
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

        /// <summary>
        /// Mapeos de la entidad Template
        /// </summary>
        /// <param name="obj">Objeto de la clase Template</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Template obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idtemplate" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "content" => type.GetProperty("Content"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad LogDb
        /// </summary>
        /// <param name="obj">Objeto de la clase LogDb</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this LogDb obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idlog" => type.GetProperty("Id"),
                "date" => type.GetProperty("Date"),
                "action" => type.GetProperty("Action"),
                "idtable" => type.GetProperty("IdTable"),
                "table" => type.GetProperty("Table"),
                "sql" => type.GetProperty("Sql"),
                "iduser" => type.GetProperty("User"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad LogComponent
        /// </summary>
        /// <param name="obj">Objeto de la clase LogComponent</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this LogComponent obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idlog" => type.GetProperty("Id"),
                "date" => type.GetProperty("Date"),
                "type" => type.GetProperty("Type"),
                "controller" => type.GetProperty("Controller"),
                "method" => type.GetProperty("Method"),
                "input" => type.GetProperty("Input"),
                "output" => type.GetProperty("Output"),
                "iduser" => type.GetProperty("User"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Fee
        /// </summary>
        /// <param name="obj">Objeto de la clase Fee</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Fee obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idfee" => type.GetProperty("Id"),
                "value" => type.GetProperty("Value"),
                "number" => type.GetProperty("Number"),
                "dueDate" => type.GetProperty("DueDate"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad PaymentType
        /// </summary>
        /// <param name="obj">Objeto de la clase PaymentType</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this PaymentType obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idpaymenttype" => type.GetProperty("Id"),
                "name" => type.GetProperty("Name"),
                "paymenttype" => type.GetProperty("Name"),
                _ => null,
            };
        }

        /// <summary>
        /// Mapeos de la entidad Payment
        /// </summary>
        /// <param name="obj">Objeto de la clase Payment</param>
        /// <param name="columnName">Nombre de la columna</param>
        /// <returns>Información de la propiedad mapeada</returns>
        public static PropertyInfo? GetMapping(this Payment obj, string columnName)
        {
            Type type = obj.GetType();
            return columnName switch
            {
                "idpayment" => type.GetProperty("Id"),
                "value" => type.GetProperty("Value"),
                "date" => type.GetProperty("Date"),
                "invoice" => type.GetProperty("Invoice"),
                "proof" => type.GetProperty("Proof"),
                _ => null,
            };
        }
        #endregion
    }
}
