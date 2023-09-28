using Entities;

namespace Dal.Dto
{
    /// <summary>
    /// Estructura que maneja los resultados de una consulta a la base de datos
    /// con múltiples registros y a la cual se le aplicaron límites de registros
    /// a retornar
    /// </summary>
    public class ListResult<T> where T : Entity
    {
        #region Attributes
        /// <summary>
        /// Listado de registros a retornar
        /// </summary>
        public IList<T> List { get; private set; }

        /// <summary>
        /// Cantidad de registros que traería si no se aplican los límites
        /// </summary>
        public int Total { get; private set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Crea un resultado de una consulta de tipo listado
        /// </summary>
        /// <param name="list">Listado de registros a retornar</param>
        /// <param name="total">Cantidad de registros que traería si no se aplican los límites</param>
        public ListResult(IList<T> list, int total)
        {
            List = list;
            Total = total;
        }
        #endregion
    }
}
