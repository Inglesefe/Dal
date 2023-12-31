﻿using Dal.Config;
using Dal.Dto;
using Dal.Exceptions;
using Entities.Admon;
using Entities.Config;
using Microsoft.Extensions.Configuration;

namespace Dal.Test.Config
{
    /// <summary>
    /// Realiza las pruebas sobre la clase de persistencia de oficinas
    /// </summary>
    [Collection("Tests")]
    public class OfficeTest
    {
        #region Attributes
        /// <summary>
        /// Configuración de la aplicación de pruebas
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Administrador de la persistencia de las oficinas
        /// </summary>
        private readonly PersistentOffice _persistent;
        #endregion

        #region Constructors
        /// <summary>
        /// Inicializa la configuración de la prueba
        /// </summary>
        public OfficeTest()
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
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListTest()
        {
            //Act
            ListResult<Office> list = _persistent.List("idoffice = 1", "name", 1, 0);

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de oficinas con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.List("idoficina = 1", "name", 1, 0));
        }

        /// <summary>
        /// Prueba la consulta de una oficina dada su identificador
        /// </summary>
        [Fact]
        public void ReadTest()
        {
            //Arrange
            Office office = new() { Id = 1 };

            //Act
            office = _persistent.Read(office);

            //Assert
            Assert.Equal("Castellana", office.Name);
        }

        /// <summary>
        /// Prueba la consulta de una oficina que no existe dado su identificador
        /// </summary>
        [Fact]
        public void ReadNotFoundTest()
        {
            //Arrange
            Office office = new() { Id = 10 };

            //Act
            office = _persistent.Read(office);

            //Assert
            Assert.Equal(0, office.Id);
        }

        /// <summary>
        /// Prueba la consulta de una oficina con error
        /// </summary>
        [Fact]
        public void ReadWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Read(null));
        }

        /// <summary>
        /// Prueba la inserción de una oficina
        /// </summary>
        [Fact]
        public void InsertTest()
        {
            //Arrange
            Office office = new() { City = new() { Id = 1 }, Name = "Madelena", Address = "Calle 59 sur", Phone = "3202134589", Active = true };

            //Act
            office = _persistent.Insert(office, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, office.Id);
        }

        /// <summary>
        /// Prueba la inserción de una oficina con error
        /// </summary>
        [Fact]
        public void InsertWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Insert(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la actualización de una oficina
        /// </summary>
        [Fact]
        public void UpdateTest()
        {
            //Arrange
            Office city = new() { Id = 2, City = new() { Id = 1 }, Name = "Santa Librada", Address = "Calle 78 sur", Phone = "3202134590", Active = false };
            Office city2 = new() { Id = 2 };

            //Act
            _ = _persistent.Update(city, new() { Id = 1 });
            city2 = _persistent.Read(city2);

            //Assert
            Assert.NotEqual("Kennedy", city2.Name);
        }

        /// <summary>
        /// Prueba la actualización de una oficina con error
        /// </summary>
        [Fact]
        public void UpdateWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Update(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de una oficina
        /// </summary>
        [Fact]
        public void DeleteTest()
        {
            //Arrange
            Office office = new() { Id = 3 };
            Office office2 = new() { Id = 3 };

            //Act
            _ = _persistent.Delete(office, new() { Id = 1 });
            office2 = _persistent.Read(office2);

            //Assert
            Assert.Equal(0, office2.Id);
        }

        /// <summary>
        /// Prueba la eliminación de una oficina con error
        /// </summary>
        [Fact]
        public void DeleteWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.Delete(null, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta de una oficina con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListAccountExecutivesTest()
        {
            //Act
            ListResult<AccountExecutive> list = _persistent.ListAccountExecutives("", "idaccountexecutive asc", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta de una oficina con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListAccountExecutivesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListAccountExecutives("idejecutivo = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta no asignados a una oficina con filtros, ordenamientos y límite
        /// </summary>
        [Fact]
        public void ListNotAccountExecutivesTest()
        {
            //Act
            ListResult<AccountExecutive> list = _persistent.ListNotAccountExecutives("", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.NotEmpty(list.List);
            Assert.True(list.Total > 0);
        }

        /// <summary>
        /// Prueba la consulta de un listado de ejecutivos de cuenta no asignados a una oficina con filtros, ordenamientos y límite y con errores
        /// </summary>
        [Fact]
        public void ListNotAccountExecutivesWithErrorTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.ListNotAccountExecutives("idejecutivo = 1", "name", 10, 0, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta de una oficina
        /// </summary>
        [Fact]
        public void InsertAccountExecutiveTest()
        {
            //Act
            AccountExecutive executive = _persistent.InsertAccountExecutive(new() { Id = 2 }, new() { Id = 2 }, new() { Id = 1 });

            //Assert
            Assert.NotEqual(0, executive.Id);
        }

        /// <summary>
        /// Prueba la inserción de un ejecutivo de cuenta de una oficina duplicado
        /// </summary>
        [Fact]
        public void InsertAccountExecutiveDuplicateTest()
        {
            //Act, Assert
            _ = Assert.Throws<PersistentException>(() => _persistent.InsertAccountExecutive(new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }));
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta de una oficina
        /// </summary>
        [Fact]
        public void DeleteAccountExecutiveTest()
        {
            //Act
            _ = _persistent.DeleteAccountExecutive(new() { Id = 2 }, new() { Id = 1 }, new() { Id = 1 });
            ListResult<AccountExecutive> list = _persistent.ListAccountExecutives("idaccountexecutive = 2", "", 10, 0, new() { Id = 1 });

            //Assert
            Assert.Equal(0, list.Total);
        }

        /// <summary>
        /// Prueba la eliminación de un ejecutivo de cuenta de una oficina con error
        /// </summary>
        [Fact]
        public void DeleteAccountExecutiveWithErrorTest()
        {
            //Act, Assert
            Assert.Throws<PersistentException>(() => _persistent.DeleteAccountExecutive(null, new() { Id = 1 }, new() { Id = 1 }));
        }
        #endregion
    }
}
