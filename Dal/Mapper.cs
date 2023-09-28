using Dapper;
using Entities.Admon;
using Entities.Auth;
using Entities.Config;
using Entities.Cont;
using Entities.Crm;
using Entities.Log;
using Entities.Noti;

namespace Dal
{
    public class Mapper
    {
        #region Attribute
        /// <summary>
        /// Única instancia de la clase
        /// </summary>
        public static Mapper? Instance { get; private set; } = null;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea un nuevo mapeador
        /// </summary>
        private Mapper()
        {
            SqlMapper.SetTypeMap(typeof(LogComponent), new CustomPropertyTypeMap(typeof(LogComponent), (type, columnName) => new LogComponent().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(LogDb), new CustomPropertyTypeMap(typeof(LogDb), (type, columnName) => new LogDb().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(AccountExecutive), new CustomPropertyTypeMap(typeof(AccountExecutive), (type, columnName) => new AccountExecutive().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Registration), new CustomPropertyTypeMap(typeof(Registration), (type, columnName) => new Registration().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Application), new CustomPropertyTypeMap(typeof(Application), (type, columnName) => new Application().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Role), new CustomPropertyTypeMap(typeof(Role), (type, columnName) => new Role().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(User), new CustomPropertyTypeMap(typeof(User), (type, columnName) => new User().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(City), new CustomPropertyTypeMap(typeof(City), (type, columnName) => new City().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Country), new CustomPropertyTypeMap(typeof(Country), (type, columnName) => new Country().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(IdentificationType), new CustomPropertyTypeMap(typeof(IdentificationType), (type, columnName) => new IdentificationType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(IncomeType), new CustomPropertyTypeMap(typeof(IncomeType), (type, columnName) => new IncomeType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Office), new CustomPropertyTypeMap(typeof(Office), (type, columnName) => new Office().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Parameter), new CustomPropertyTypeMap(typeof(Parameter), (type, columnName) => new Parameter().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Plan), new CustomPropertyTypeMap(typeof(Plan), (type, columnName) => new Plan().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Scale), new CustomPropertyTypeMap(typeof(Scale), (type, columnName) => new Scale().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(AccountNumber), new CustomPropertyTypeMap(typeof(AccountNumber), (type, columnName) => new AccountNumber().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(AccountType), new CustomPropertyTypeMap(typeof(AccountType), (type, columnName) => new AccountType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(ConsecutiveNumber), new CustomPropertyTypeMap(typeof(ConsecutiveNumber), (type, columnName) => new ConsecutiveNumber().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(ConsecutiveType), new CustomPropertyTypeMap(typeof(ConsecutiveType), (type, columnName) => new ConsecutiveType().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Beneficiary), new CustomPropertyTypeMap(typeof(Beneficiary), (type, columnName) => new Beneficiary().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Owner), new CustomPropertyTypeMap(typeof(Owner), (type, columnName) => new Owner().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Notification), new CustomPropertyTypeMap(typeof(Notification), (type, columnName) => new Notification().GetMapping(columnName)));
            SqlMapper.SetTypeMap(typeof(Template), new CustomPropertyTypeMap(typeof(Template), (type, columnName) => new Template().GetMapping(columnName)));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retorna la única instancia de Mapper
        /// </summary>
        /// <returns></returns>
        public static Mapper GetInstance()
        {
            return Instance ??= new Mapper();
        }
        #endregion
    }
}
