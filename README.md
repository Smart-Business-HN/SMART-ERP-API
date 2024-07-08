# SMART ERP-API
Desarrollo Backend del CRM **SMART BUSINESS** desarrollado en **C#** con **.Net Core 7**. Sirva la siguiente documentación para conocer la arquitectura general del sistema, las dependencias necesarias y las funciones que cumplen los servicios descritos a continuación.

## Arquitectura
El sistema fue desarrollado bajo una combinación de patrón de diseño DDD (*Domain Driven Design*), CQRS (*Command and Query Responsibility Segregation*) y arquitectura Onion. La aplicación cuenta con cuatro carpetas principales (API, Application, Domain, Infrastructure). 

 1. Siguiendo los principios de diseño DDD, la carpeta Domain contiene las entidades de la aplicación, es decir, todos aquellos objetos que se encuentran en nuestra base de datos. Esta carpeta no contiene referencias de ningún tipo ni dependencias, es la capa más pura del sistema. 
 2. En la capa externa a Domain se encuentra la carpeta Infrastructure la cual se encarga de ofrecer las conexiones a las bases de datos para realizar consultas y el contexto o relaciones de nuestras entidades. 
 3. La capa Application contiene los DTOs (*Data Transfer Objects*), los comandos para realizar creaciones, actualizaciones y ediciones de registros, los queries para extraer información, los servicios necesarios para ejecutar comandos y las excepciones generadas por el sistema.
 4. Por último, la capa API se encarga de exponer rutas o endpoints mediante controladores para ejecutar comandos y queries. 

<p align="center">
<img src="https://f4n3x6c5.stackpathcdn.com/article/onion-architecture-in-asp-net-core-6-web-api/Images/Onion%20Architecture%20in%20Aspdotnet%20Core%206%20Web%20API.png">
</p>

### CQRS
Denominamos **Comandos** todas aquellas funciones que se encargan de operar en los registros de la base de datos, ya sea mediante creación de nuevos registros o edición de registros existentes. Denominamos **Queries** todas aquellas funciones que se encargan solamente de leer los registros existentes y transformarlos en DTOs. Cada entidad de la capa Domain debe contener un **Feature** que contenga sus comandos y queries.

<p align="center">
<img src="https://learn.microsoft.com/en-us/azure/architecture/patterns/_images/command-and-query-responsibility-segregation-cqrs-basic.png">
</p>

## NuGets

 - MailKit 3.4.0: NuGet necesario para el envío de correos electrónicos
 - FluentValidation 11.2.0: NuGet necesario para validación de peticiones entrantes en comandos antes de ejecutar la lógica de dicho comando.
 - MediatR 10.0.1: NuGet mediador de Commands y Queries al recibir una consulta en la capa API.
 - AutoMapper 11.0.1: NuGet necesario para el mapeo de entidades a DTOs.
 - Quartz 3.6.0: NuGet necesario para la calendarización de Jobs o tareas que se ejecutan cada cierto tiempo. 
 - RestSharp 108.0.3: NuGet para peticiones HTTP/REST 
 - Ardalis 6.1.0: NuGet para especificaciones en Queries.
 - Google.Apis.Calendar.v3 1.58.0.2759: NuGet para crear eventos en Google Calendars.
 - Azure.Storage.Blobs 12.13.1: NuGet para la subida de archivos 

A excepción de MailKit, Google Calendar, RestSharp y Ardalis, cada NuGet debe instalarse con su DependencyInjection correspondiente.

## Referencias
Para poder acceder y conocer las entidades, funciones y dependencias desde otras capas es necesario realizar referencias entre proyectos en la solución. A continuación se muestran las referencias necesarias para cada capa.

| Capa | Referencias |
|--|--|
| API | Application, Infrastructure |
| Application | Domain |
| Infrastructure | Application, Domain |
| Domain| Ninguna |

Comandos en la base de datos:

Primer comando: Este es para crear el seed de la compañia

  INSERT INTO [SmartERP].[dbo].[Company] (Name,Email,PhoneNumber,Address, AboutUs, CreationDate, CreatedBy, ModificationDate, ModificatedBy,IsActive) VALUES ('Smart Business S. de R.L.','ventas@smartbusiness.site','8817-7765','Jutiapa, Atlantida, Aldea Agua Dulce Fte. A Esc. Danubia Ramirez','','2023-07-29','Jose Cubas','2023-07-29','Jose Cubas',1)

Segundo comando: Seeds de los Roles de usuario
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('SuperAdmin',	'SuperAdmin',	1)
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('Comunity Manager',	'Comunity Manager',	1)
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('Sales Advisor',	'SalesAdvisor',	1)
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('Manager',	'Manager',	1)
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('After Sale',	'AfterSale',	1)
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('Credit Manager',	'CreditManager',	1)
    INSERT INTO [SmartERP].[dbo].[Role] (Name, Selector, IsActive) VALUES ('Admin',	'Admin',	1)

Tercer Comando: Seed de Genero

    INSERT INTO [SmartERP].[dbo].[Gender] (Name, IsActive) Values ('Masculino',1)
    INSERT INTO [SmartERP].[dbo].[Gender] (Name, IsActive) Values ('Femenino',1)

Cuarto Comando: Seed de Tipos de Estado
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Producto',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Cotización',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Lista de Deseos',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Actividad',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Factura',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Orden de Compra',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Factura de compra',1)
    INSERT INTO [SmartERP].[dbo].[TypeStatus] (Name,IsActive) VALUES ('Gasto no Declarable',1)

Quinto Comando: Seed de Estados con sus respectivos tipos de estado
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Nuevo',1,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Por Encargo',1,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Disponible',1,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('No Disponible',1,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Guardada',2,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Enviada',2,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Aceptada',2,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Rechazada',2,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',2,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('En Carrito',3,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Solicitada la Cotizacion',3,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',3,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('En Proceso',4,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',4,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Finalizada',4,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Guardada',5,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Enviada',5,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Pendiente de Pago',5,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Pagada',5,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',5,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Guardada',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Facturada / Sin Recibir',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Recibida / Sin Facturar',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Completa',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Recibida',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',6,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name, TypeStatusId, IsActive) VALUES ('Pendiente de pago',7,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name, TypeStatusId, IsActive) VALUES ('Pagada',7,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name, TypeStatusId, IsActive) VALUES ('Pendiente de pago',8,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name, TypeStatusId, IsActive) VALUES ('Pagado',8,1)

Sexto comando: Datos demograficos de Honduras

    INSERT INTO [SmartERP].[dbo].[Country] (Name,Abbreviation, PhoneNumberCode,IsActive) VALUES ('Honduras','HND','504', 1)
    INSERT INTO [SmartERP].[dbo].[Region] (Name,CountryId,IsActive) VALUES ('Occidente',1, 1)
    INSERT INTO [SmartERP].[dbo].[Region] (Name,CountryId,IsActive) VALUES ('Noroccidente',1, 1)
    INSERT INTO [SmartERP].[dbo].[Region] (Name,CountryId,IsActive) VALUES ('Nororiental',1, 1)
    INSERT INTO [SmartERP].[dbo].[Region] (Name,CountryId,IsActive) VALUES ('Centro Occidental',1, 1)
    INSERT INTO [SmartERP].[dbo].[Region] (Name,CountryId,IsActive) VALUES ('Centro Oriental',1, 1)
    INSERT INTO [SmartERP].[dbo].[Region] (Name,CountryId,IsActive) VALUES ('Sur',1, 1)

    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Atlántida',1, 1, 3)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Colón',1, 1, 3)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Comayagua',1, 1, 4)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Copán',1, 1, 1)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Cortés',1, 1, 2)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Choluteca',1, 1, 6)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('El Paraíso',1, 1, 5)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Francisco Morazán',1, 1, 5)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Gracias a Dios',1, 1, 3)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Intibucá',1, 1, 4)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Islas de la Bahía',1, 1, 3)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('La Paz',1, 1, 4)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Lempira',1, 1, 1)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Ocotepeque',1, 1, 1)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Olancho',1, 1, 5)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Santa Bárbara',1, 1,2)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Valle',1, 1, 6)
    INSERT INTO [SmartERP].[dbo].[Department] (Name,IsActive,CountryId, RegionId) VALUES ('Yoro',1, 1, 2)

    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Ceiba',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Porvenir',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Tela',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Jutiapa',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Masica',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Arizona',1, 1)
    INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Esparta',1, 1)

     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Trujillo',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Balfate',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Iriona',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Limón',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Sabá',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Fe',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Rosa de Aguán',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Sonaguera',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Tocoa',1, 2)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Bonito Oriental',1, 2)

     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Comayagua',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Ajuterique',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Rosario',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Esquias',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Humuya',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Libertad',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Lamaní',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Trinidad',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Lejamaní',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Meámbar',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Minas de Oro',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Ojos de Agua',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Jerónimo',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San José de Comayagua',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San José de Potrero',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Luis',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Sebastián',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Siguatepeque',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Villa de San Antonio',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Las Lajas',1, 3)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Taulabé',1, 3)

     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Rosa de Copán',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Cabañas',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concepción',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Copán Ruinas',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Corquín',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Cucuyagua',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Dolores',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Dulce Nombre',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Paraíso',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Florida',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Jigua',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Unión',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Nueva Arcadia',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Agustín',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Jerónimo',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San José',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Juan de Opoa',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Nicolás',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Pedro',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Rita',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Trinidad de Copán',1, 4)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Veracruz',1, 4)

     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Pedro Sula',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Choloma',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Omoa',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Pimienta',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Poterillos',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Puerto Cortés',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio de Cortés',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco de Yojoa',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Manuel',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Cruz de Yojoa',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Villanueva',1, 5)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Lima',1, 5)

     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Choluteca',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Apacilagua',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concepción de María',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Duyure',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Corpus',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Triunfo',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Marcovia',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Morolica',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Namasigüe',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Orocuina',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Pespire',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio de Flores',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Isidro',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San José',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Marcos de Colón',1, 6)
     INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Ana de Yusguare',1, 6)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yuscarán',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Alauca',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Danlí',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Paraíso',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Güinope',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Jacaleapa',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Liure',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Morocelí',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Oropolí',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Potrerillos',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio de Flores',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Lucas',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Matías',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Soledas',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Teupasenti',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Texiguat',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Vado Ancho',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yauyupe',1, 7)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Trojes',1, 7)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Tegucigalpa',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Alubarén',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Cedros',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Curarén',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Porvenir',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guaimaca',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Libertad',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Venta',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Lepaterique',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Maraita',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Marale',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Nueva Armenia',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Ojojona',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Orica',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Reitoca',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Sabanagrande',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio de Oriente',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Buenaventura',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Ignacio',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Juan de Flores',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Miguelito',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Ana',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Lucía',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Talanga',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Tatumbla',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Valle de Ángeles',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Villa de San Francisco',1, 8)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Vallecillo',1, 8)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Puerto Lempira',1, 9)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Brus Laguna',1, 9)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Ahuas',1, 9)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco Bulnes',1, 9)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Villeda Morales',1, 9)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Wampusirpe',1, 9)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Esperanza',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Camasca',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Colomancagua',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concepción',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Dolores',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Intibucá',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Jesús de Otoro',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Magdalena',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Masaguara',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Isidro',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Juan',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Marcos de la Sierra',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Miguelito',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Lucia',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yamaranguila',1, 10)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco de Opalaca',1, 10)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Roatán',1, 11)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guanaja',1, 11)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('José Santos Guardiola',1, 11)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Utila',1, 11)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Paz',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Aguaqueterique',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Cabañas',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Cane',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Chinacla',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guajiquiro',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Lauterique',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Marcala',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Mercedes de Oriente',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Opatoro',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Antonio del Norte',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San José',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Juan',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Pedro de Tutule',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Ana',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Elena',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa María',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santiago de Puringla',1, 12)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yarula',1, 12)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Gracias',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Belén',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Candelaria',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Cololaca',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Erandique',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Gualcince',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guarita',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Campa',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Iguala',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Las Flores',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Unión',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Virtud',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Lepaera',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Mapulaca',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Piraera',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Andrés',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Juan Guarita',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Manuel Colohete',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Rafael',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Sebastian',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Cruz',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Talgua',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Tambla',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Tomalá',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Valladolid',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Virginia',1, 13)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Marcos de Caiquín',1, 13)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Ocotepeque',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Belén Gualcho',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concepción',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Dolores Merendón',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Fraternidad',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Encarnación',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Labor',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Lucerna',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Mercedes',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Fernando',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco del Valle',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Jorge',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Marcos',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Fe',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Sensenti',1, 14)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Sinuapa',1, 14)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Juticalpa',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Campamento',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Catacamas',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concordia',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Dulce Nombre Culmí',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Rosario',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Esquipulas del Norte',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Gualaco',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guarizama',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guata',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Guayape',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Jano',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('La Unión ',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Mangulile',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Manto',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Salamá',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Esteban',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco de Becerra',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francos de la Paz',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa María del Real',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Silca',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yocón',1, 15)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Patuca',1, 15)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Bárbara',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Arada',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Atima',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Azacualpa',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Ceguaca',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concepción del Norte',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Concepción del Sur',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Chinda',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Níspero',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Gualala',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Llama',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Las Vegas',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Macualizo',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Naranjito',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Nuevo Celilac',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Nueva Frontera',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Petoa',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Protección',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Quimistán',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco de Ojuera',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San José de las Colinas',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Luis',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Marcos',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Nicolás',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Pedro Zacapa',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Vicente Centenario',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Rita',1, 16)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Trinidad',1, 16)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Nacaome',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Alianza',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Amapala',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Aramecina',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Caridad',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Goascorán',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Langue',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Francisco de Coray',1, 17)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('San Lorenzo',1, 17)

      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yoro',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Arenal',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Negrito',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('El Progreso',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Jocón',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Morazán',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Olanchito',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Santa Rita',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Sulaco',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Victoria',1, 18)
      INSERT INTO [SmartERP].[dbo].[City] (Name,IsActive,DepartmentId) VALUES ('Yorito',1, 18)

      Septimo comando: Documentos internos
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Factura',1)
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Cotización',1)
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Orden de Compra',1)
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Factura de Compra',1)
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Gasto no Declarable',1)
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Entrada de Inventario',1)
      INSERT INTO [SmartERP].[dbo].[InternalDocument] (Name, IsActive) VALUES ('Salida de Inventario',1)


      Octavo comando: Razones sociales
      INSERT INTO [SmartERP].[dbo].[SocialReason] (Name, IsActive) VALUES ('Natural',1)
      INSERT INTO [SmartERP].[dbo].[SocialReason] (Name, IsActive) VALUES ('Juridica',1)

      Noveno comando: Principales Rubros de Smart Business
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('No Aplica',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Telecomunicaciones',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Construcción',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Medicina',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Ganaderia',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Agricultura',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Tecnologia',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Alimentos Procesados',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Fabricación y distribución de Materias Primas',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Banca y Finanzas',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Transporte',1)
      INSERT INTO [SmartERP].[dbo].[Heading] (Name,IsActive) VALUES ('Servicios Profesionales',1)

      Decimo Comando: Tipos de Clientes
      INSERT INTO [SmartERP].[dbo].[CustomerType] (Name,IsActive) VALUES ('Basico',1)
      INSERT INTO [SmartERP].[dbo].[CustomerType] (Name,IsActive) VALUES ('Recurrente',1)
      INSERT INTO [SmartERP].[dbo].[CustomerType] (Name,IsActive) VALUES ('Mayorista',1)
      INSERT INTO [SmartERP].[dbo].[CustomerType] (Name,IsActive) VALUES ('Integrador',1)
      INSERT INTO [SmartERP].[dbo].[CustomerType] (Name,IsActive) VALUES ('Corporativo',1)
      INSERT INTO [SmartERP].[dbo].[CustomerType] (Name,IsActive) VALUES ('Empleado',1)

      Undecimo comando: Niveles de interes de los clientes
      INSERT INTO [SmartERP].[dbo].[InterestLevel] (Name,IsActive,CreationDate) VALUES ('Bajo',1,'2023-07-29')
      INSERT INTO [SmartERP].[dbo].[InterestLevel] (Name,IsActive,CreationDate) VALUES ('Intermedio',1,'2023-07-29')
      INSERT INTO [SmartERP].[dbo].[InterestLevel] (Name,IsActive,CreationDate) VALUES ('Alto',1,'2023-07-29')

      Duodecimo commando: Tipos de Forma de Pago (Para uso interno)
      INSERT INTO [SmartERP].[dbo].[TypeOfPaymentMethod] (Name,IsActive) VALUES ('Efectivo',1)
      INSERT INTO [SmartERP].[dbo].[TypeOfPaymentMethod] (Name,IsActive) VALUES ('T
      ransferencia Bancaria',1)
      INSERT INTO [SmartERP].[dbo].[TypeOfPaymentMethod] (Name,IsActive) VALUES ('Link de pago',1)

      Treceabo commando: Incertar los tipos de proveedores: 
        INSERT INTO [dbo].[TypeProvider] ([Name]) VALUES ('Proveedor')
        INSERT INTO [dbo].[TypeProvider] ([Name]) VALUES ('Acreedor')
        INSERT INTO [dbo].[TypeProvider] ([Name]) VALUES ('Ambos')

      Catorceavo comando: Insertar los tipos de forma de pago de factura
        INSERT INTO [dbo].[InvoicePaymentType] ([Name]) VALUES ('Contado')
        INSERT INTO [dbo].[InvoicePaymentType] ([Name]) VALUES ('Credito')

      Quinceavo comand: Insertar los tipos de entrada de inventario
        INSERT INTO [dbo].[InventoryInputType] ([Name],[IsActive]) VALUES ('Compra de Equipo',1)
        INSERT INTO [dbo].[InventoryInputType] ([Name],[IsActive]) VALUES ('Regalias',1)
        INSERT INTO [dbo].[InventoryInputType] ([Name],[IsActive]) VALUES ('Sobrante de Inventario',1)
        INSERT INTO [dbo].[InventoryInputType] ([Name],[IsActive]) VALUES ('Devoluciones',1)

      Dieciseisavo comando: Insertar los prefijos
        INSERT INTO [dbo].[Prefix] ([Format],[InternalDocumentId],[ItIsTaken]) VALUES ('000-000-01-',1,1)
        INSERT INTO [dbo].[Prefix] ([Format],[InternalDocumentId],[ItIsTaken]) VALUES ('SO',2,1)
        INSERT INTO [dbo].[Prefix] ([Format],[InternalDocumentId],[ItIsTaken]) VALUES ('PO',3,1)
        INSERT INTO [dbo].[Prefix] ([Format],[InternalDocumentId],[ItIsTaken]) VALUES ('PB',4,1)
        INSERT INTO [dbo].[Prefix] ([Format],[InternalDocumentId],[ItIsTaken]) VALUES ('IE',5,1)
        INSERT INTO [dbo].[Prefix] ([Format],[InternalDocumentId],[ItIsTaken]) VALUES ('IO',6,1)
      