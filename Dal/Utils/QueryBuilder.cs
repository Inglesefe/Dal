using System.Text;

namespace Dal.Utils
{
    /// <summary>
    /// Construye sentencias SQL de acuerdo a los campos, tablas y demás datos ingresados,
    /// y de acuerdo al tipo de sentencia que se desea obtener
    /// </summary>
    public class QueryBuilder
    {
        #region Attributes
        /// <summary>
        /// Campos en la consulta
        /// </summary>
        private readonly string _fields;

        /// <summary>
        /// Tablas a operar
        /// </summary>
        private readonly string _tables;
        #endregion

        #region Constructors
        /// <summary>
        /// Crea un constructor de consultas con los campos a tratar y las tablas sobre
        /// las cuales se va a realizar la consulta
        /// </summary>
        /// <param name="fields">Campos a tratar</param>
        /// <param name="tables">Tablas sobre las cuales se realiza la consulta</param>
        public QueryBuilder(string fields, string tables)
        {
            _fields = Sanitize(fields);
            _tables = Sanitize(tables);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Elimina los posibles caracteres que puedan provocar inyección de SQL
        /// </summary>
        /// <param name="input">Entrada a desinfectar</param>
        /// <returns>Cadena desinfectada</returns>
        private static string Sanitize(string input) => input.Replace("''", "'").Replace(";", "");

        /// <summary>
        /// Crea una sentencia SELECT con filtros, ordenamientos, y límite de registros a traer
        /// </summary>
        /// <param name="filters">Filtros aplicados</param>
        /// <param name="orders">Ordenamientos aplicados</param>
        /// <param name="limit">Límite de registros a traer</param>
        /// <param name="offset">Corrimiento desde el que cuenta los registros a traer</param>
        /// <returns>Sentencia para taer un listado</returns>
        public string GetSelectForList(string filters, string orders, int limit, int offset)
        {
            StringBuilder sql = new("SELECT " + _fields + " FROM " + _tables);
            if (!string.IsNullOrEmpty(filters))
            {
                sql.Append(" WHERE " + Sanitize(filters));
            }
            if (!string.IsNullOrEmpty(orders))
            {
                sql.Append(" ORDER BY " + Sanitize(orders));
            }
            if (limit != 0)
            {
                sql.Append(" LIMIT " + offset + ", " + limit);
            }
            return sql.ToString();
        }

        /// <summary>
        /// Crea una sentencia de conteo total de registros de un SELECT con filtros, ordenamientos, y límite de registros a traer
        /// </summary>
        /// <param name="filters">Filtros aplicados</param>
        /// <param name="orders">Ordenameintos aplicados</param>
        /// <returns>Sentencia para taer el conteo total de registros de un listado</returns>
        public string GetCountTotalSelectForList(string filters, string orders)
        {
            StringBuilder sql = new("SELECT COUNT(1) AS total FROM " + _tables);
            if (!string.IsNullOrEmpty(filters))
            {
                sql.Append(" WHERE " + Sanitize(filters));
            }
            if (!string.IsNullOrEmpty(orders))
            {
                sql.Append(" ORDER BY " + Sanitize(orders));
            }
            return sql.ToString();
        }
        #endregion
    }
}
