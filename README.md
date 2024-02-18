# Learn Microservices architecture with .NET Core MVC .NET 8

In this project, we will study the foundational elements of microservices by incrementally building a real microservices based application. We will be building multiple microservices and for authentication and authorization we will be using clean architecture with the latest .NET 8 and the following tools :

- .NET 8 **Microservices** Architecture
- Implementing 7 microservices using .NET 8
- .NET API with Authentication and Authorization
- Identity Server integration
- Role based authorization with Identity Server
- Async and Sync communication between Microservices
- **Azure Blob Storage** basics
- **Azure Service Bus** - Topics and Queues
- Gateways in Microservices
- Implementing **Ocelot gateway**
- Swagger Open API implementation
- N-Layer implementation with Repository Pattern
- ASPNET Core Web Application with Bootstrap 5
- Entity Framework Core with SQL Server Database



## General architecture

<img src="/pictures/architecture.png" title="architecture"  width="900">

## Coupon MicroService

### Nuget Packages

Make sure you check **Include prerelease**

```
Automapper.Extensions.Microsoft.DependencyInjection
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.AspNetCore.Authentication.JwtBearer
```

### Migration

```
add-migration coupon
update-database
```



## AuthAPI MicroService

### Migration

```
add-migration authAPI
update-database
```



## ServiceBus MicroService

### Migration

```
Azure.Messaging.ServiceBus
```

### Service Bus

- create namespace
  <img src="/pictures/servicebus.png" title="service bus"  width="900">

- create queue
  <img src="/pictures/servicebus1.png" title="service bus"  width="900">

- get connection string. Default policy is ok
  <img src="/pictures/servicebus2.png" title="service bus"  width="900">

- send email and see that message is posted on the queue
  <img src="/pictures/servicebus30.png" title="service bus"  width="900">
  <img src="/pictures/servicebus3.png" title="service bus"  width="900">




## Order MicroService

### Nuget Packages

```
Stripe.net
```



## Rewards MicroService

If you want to have multiple receivers on a queue, you need to have topics.

- on *mangoalexei*, create a topic *ordercreated*
  <img src="/pictures/rewards.png" title="rewards"  width="900">

- on *ordercreated*, create a subscription *OrderCreatedRewardsUpdate*, as well as a subscription *OrderCreatedEmail*. Leave defaults for both
  <img src="/pictures/rewards1.png" title="rewards"  width="900">
  <img src="/pictures/rewards2.png" title="rewards"  width="900">
