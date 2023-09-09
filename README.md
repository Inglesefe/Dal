# Dal

- dev: [![dev](https://github.com/Inglesefe/Dal/actions/workflows/build.yml/badge.svg?branch=dev)](https://github.com/Inglesefe/Dal/actions/workflows/build.yml)  
- test: [![test](https://github.com/Inglesefe/Dal/actions/workflows/build.yml/badge.svg?branch=test)](https://github.com/Inglesefe/Dal/actions/workflows/build.yml)  
- main: [![main](https://github.com/Inglesefe/Dal/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/Inglesefe/Dal/actions/workflows/build.yml)

Persistencia de las entidades necesarias para el sistema

Los diagramas de base de datos son los siguientes:

Diagrama entidad relaci�n de administraci�n
![ER_Admon](./docs/er_admon.png "Diagrama entidad relaci�n de administraci�n")

Diagrama entidad relaci�n de autenticaci�n
![ER_Auth](./docs/er_auth.png "Diagrama entidad relaci�n de autenticaci�n")

Diagrama entidad relaci�n de configuraci�n general
![ER_Conf](./docs/er_conf.png "Diagrama entidad relaci�n de configuraci�n general")

Diagrama entidad relaci�n de contabilidad
![ER_Conf](./docs/er_cont.png "Diagrama entidad relaci�n de contabilidad")

Diagrama entidad relaci�n de crm
![ER_Conf](./docs/er_crm.png "Diagrama entidad relaci�n de crm")

Diagrama entidad relaci�n de log
![ER_Log](./docs/er_log.png "Diagrama entidad relaci�n de log")

Diagrama entidad relaci�n de notificaci�n
![ER_Noti](./docs/er_noti.png "Diagrama entidad relaci�n de notificaci�n")

## Gu�a de inicio

Estas instrucciones le dar�n una copia del proyecto funcionando en su m�quina local con fines de desarrollo y prueba.
Consulte implementaci�n para obtener notas sobre la implementaci�n del proyecto en un sistema en vivo.

### Prerequisitos

Este proyecto est� desarrollado en .net core 7, usando el ORM Dapper, el paquete de Entities y el conector a Mysql  
[<img src="https://adrianwilczynski.gallerycdn.vsassets.io/extensions/adrianwilczynski/asp-net-core-switcher/2.0.2/1577043327534/Microsoft.VisualStudio.Services.Icons.Default" width="50px" height="50px" />](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)  
[<img src="https://z2c2b4z9.stackpathcdn.com/images/logo256X256.png" width="50px" height="50px" />](https://github.com/DapperLib/Dapper)  
[Entities](https://github.com/Inglesefe/Entities/pkgs/nuget/Entities)  
[Mysql.Data](https://www.nuget.org/packages/MySql.Data)

## Pruebas

Para ejecutar las pruebas unitarias, es necesario tener instalado MySQL en el ambiente y ejecutar el script db-test.sql que se encuentra en el proyecto de pruebas.
La conexi�n se realiza con los datos del archivo appsettings.json del proyecto de pruebas, o con variables de entorno del equipo con los mismos nombres.

## Despliegue

El proyecto se despliega como un paquete NuGet en el repositorio [NuGet de GitHub](https://github.com/Inglesefe/Dal/pkgs/nuget/Dal)
