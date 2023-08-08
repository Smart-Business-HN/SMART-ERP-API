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

## Platino HN
Platino Motors fue desarrollado en conjunto con la plataforma Platino HN, por lo cual referencias de dicho proyecto se pueden encontrar en la solución. Los proyectos de Platino HN contienen la misma lógica y arquitectura de Platino Motors.

<p align="center">
<img width="30%" src="https://grupoplatino.github.io/PlatinoHN/static/media/icons.91fd43b49334b5df02da05fae0a3efe8.svg">
</p>

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
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',6,1)

    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Guardada',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Enviada',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Pendiente de Pago',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Pagada',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Recibida',6,1)
    INSERT INTO [SmartERP].[dbo].[Status] (Name,TypeStatusId,IsActive) VALUES ('Cancelada',6,1)