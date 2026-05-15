# backend-coding-task


Task 1. Code Refactoring and Improvements:
My Favourite IDE: Visual Studio .NET 2022/ Visual Studio (2026)
I have rewritten the Program.cs file to improve readability and maintainability by using extension methods for both IServiceCollection and WebApplication. 
This modular approach makes the code easier to understand and manage.
Currently used a tuple for dbconnections but may consider using a more structured approach 
of creating a class/list to hold series of database connection information in a collection 
in the future for better clarity and maintainability.

IServiceCollection Extensions:
- AddControllerswithJsonOptions: 
- This extension method configures the controllers to use JSON options, 
	- such as setting the property naming policy to camel case and ignoring null values during serialization.
Have seperate extension methods for adding all AddScopedServices and AddSingletonServices to keep the Program.cs clean and organized.
In case, there are more , add them in extensions for better separation of concerns and maintainability.
WebApplication Extensions: MigrateAuditContext: 

Have seperate folders for Extensions, Services and Repositories with interface abstraction 
to keep the project organized and maintainable.

Have gone ahead with ControllerBase for the controllers to leverage the built-in features of ASP.NET Core MVC, such as model binding, 
validation, and action results, which can simplify the code and improve readability,
though minimal apis can be used for simple endpoints and are light-weight, 
using ControllerBase allows for better scalability and maintainability as the application grows in complexity.

Current Project Layer :
Claims/
├── Controllers/         ← using ControllerBase for better scalability and maintainability and logging as well.
├── Services/            ← application logic
├── Validation/          ← business rules
├── Repositories/        ← persistence abstractions + EF/Mongo implementations
├── Data/                ← ClaimsContext (Mongo)
├── Auditing/            ← AuditContext (SQL) + entities
├── Messaging/           ← bus + background processor
├── Filters/             ← Handling exceptions
├── Extensions/          ← composition root helpers
├── Claim.cs, Cover.cs   ← domain models (at root)
└── Program.cs

In future, if this is going to be a large project, I would consider further organizing the code as below:
Claims.Domain          → Claim, Cover, CoverType, ClaimType
                         Validation/ (ICoverValidator, ClaimValidator…)
                         Messaging/AuditMessage 

Claims.Application     → Services/ (IClaimsService, ClaimsService…)
                         Repositories/ interfaces
                         Messaging/ IAuditMessageBus  (interface only)
                         depends on: Claims.Domain

Claims.Infrastructure  → Data/ClaimsContext, Repositories/* impls
                         Auditing/AuditContext + entities
                         Messaging/InMemoryAuditMessageBus, AuditMessageProcessor
                         depends on: Claims.Application

Claims.Api             → Controllers/, Filters/, Program.cs, Extensions/
                         depends on: Claims.Application, Claims.Infrastructure

Claims.Tests           → unit tests, other unit tests,  depends on Claims.Domain + Claims.Application
Claims.IntegrationTests → WebApplicationFactory, Testcontainers.
Also plan to include APIVersioning, CQRS, dtos (if needed with more endpoints), multiple messaging systems in the future.

Migrations
Had to run below commands in powershell , also I moved existing migrations in migrations_old and created my new one.
Running application in Rancher desktop.
dotnet ef database update --context AuditContext
dotnet ef migrations add Migration_AbhangTest --context AuditContext --output-dir Migrations
dotnet ef migrations list --context AuditContext


Task 2. Have created a new folder named "Validation" to hold the custom validation attributes 
and logic for validating the incoming request data.
Have two seperate interfaces for validating Claims and Coverage, IClaimsValidator and ICoverageValidator, 
to adhere to the Single Responsibility Principle and keep the validation logic organized and maintainable.
with methods ValidateAsync and Validate methods to handle both synchronous and asynchronous validation scenarios 
for Claims and Coverage respectively,
these methods are directly throwing ValidationException (derived from System.Exception). 

Task 3. Have abstracted messaging in seperate messaging folder and 
implemented InMemoryAuditMessageBus and AuditMessageProcessor (BackgroundService) to handle the auditing messages in-memory for simplicity,
inside InMemoryAuditMessageBus, we use Channels (System.Threading.Channels) to create an in-memory message queue for handling audit messages.
though we can easily swap it out with a more robust solution like RabbitMQ or Azure Service Bus in the future if needed,
currently it will allow testing and development easily with test data without the need for an external message broker.

Task 4. Implemented Feature Flag to have AzureMessageBus as an option for the message bus implementation, 
or use InMemoryAuditMessageBus based on the configuration, by default we use InMemoryAuditMessageBus.

Task 5. Implemented IPremiumCalculator interface 
with a simple PremiumCalculator implementation (ComputePremium) 
that calculates the premium based on the claim amount and coverage type accessing it as endpoint in the API.
