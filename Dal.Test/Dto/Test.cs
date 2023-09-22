using Dal.Dto;
using Entities.Log;

namespace Dal.Test.Dto
{
    /// <summary>
    /// Prueba el resultado de un listado de objetos
    /// </summary>
    [Collection("Tests")]
    public class Test
    {
        #region Methods
        /// <summary>
        /// Prueba la creación de un resultado de un listado de registros 
        /// </summary>
        [Fact]
        public void CreateListResult()
        {
            Assert.IsType<ListResult<LogDb>>(new ListResult<LogDb>(new List<LogDb>(), 0));
        }
        #endregion
    }
}
