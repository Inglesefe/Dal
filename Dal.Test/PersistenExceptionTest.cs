using Dal.Exceptions;

namespace Dal.Test
{
    public class PersistenExceptionTest
    {
        /// <summary>
        /// Prueba la creación de una excepción de persistencia por defecto
        /// </summary>
        [Fact]
        public void CreateExceptionDefault()
        {
            Assert.IsType<PersistentException>(new PersistentException());
        }

        /// <summary>
        /// Prueba la creación de una excepción de persistencia con un mensaje
        /// </summary>
        [Fact]
        public void CreateExceptionWithMessage()
        {
            Assert.IsType<PersistentException>(new PersistentException("Excepción de prueba"));
        }
    }
}
