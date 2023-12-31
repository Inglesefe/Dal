﻿using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de tipos de ingreso
    /// </summary>
    [Collection("Tests")]
    public class IncomeTypeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de los tipos de ingreso
        /// </summary>
        private readonly PersistentIncomeType _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public IncomeTypeTest()
        {
            //Arrange
            _configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddEnvironmentVariables()
                .Build();
            _persistent = new(_configuration.GetConnectionString("golden") ?? "");
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prueba la consulta de un listado de tipos de ingreso con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<IncomeType> list = _persistent.List("idincometype = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de tipos de ingreso con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idtipoingreso = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 1 };

            //Act
            incomeType = _persistent.Read(incomeType);

            //Assert
            Assert.Equal("CI", incomeType.Code);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 10 };

            //Act
            incomeType = _persistent.Read(incomeType);

            //Assert
            Assert.Equal(0, incomeType.Id);
        }

        /// <summary>
        /// Prueba la consulta de un tipo de ingreso con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de un tipo de ingreso
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            IncomeType incomeType = new() { Code = "CF", Name = "Cheques posfechados" };

            //Act
            incomeType = _persistent.Insert(incomeType, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, incomeType.Id);
        }

        /// <summary>
        /// Prueba la inserción de un tipo de ingreso con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de un tipo de ingreso
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 2, Code = "CT", Name = "Otro ingreso" };
            IncomeType incomeType2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(incomeType, new() { Id = 1 });
            incomeType2 = _persistent.Read(incomeType2);

            //Assert
            Assert.NotEqual("Credito cartera", incomeType2.Name);
        }

        /// <summary>
        /// Prueba la actualización de un tipo de ingreso con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de ingreso
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            IncomeType incomeType = new() { Id = 3 };
            IncomeType incomeType2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(incomeType, new() { Id = 1 });
            incomeType2 = _persistent.Read(incomeType2);

            //Assert
            Assert.Equal(0, incomeType2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de un tipo de ingreso con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }
        #endregion
    }
}
