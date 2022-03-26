[![LICENSE](https://img.shields.io/badge/license-MIT-lightgrey.svg)](https://raw.githubusercontent.com/dpedwards/dotnet-core-blockchain-advanced/master/LICENSE)
[![Swagger](https://img.shields.io/badge/Swagger-lightgreen.svg)](https://swagger.io/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-orange.svg)](https://www.rabbitmq.com/download.html)
[![CQRS pattern](https://img.shields.io/badge/CQRS-pattern-blue.svg)](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
[![Mediator pattern](https://img.shields.io/badge/Mediator-pattern-blue.svg)](https://en.wikipedia.org/wiki/Mediator_pattern)
[![DDD pattern](https://img.shields.io/badge/DDD-pattern-blue.svg)](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)

# People

An ASP.NET application for managing people.

As we are going to implement a CQRS approach, I have split the read and write parts of the API into separate projects. They can now be deployed and scaled independently. 

For HATEOAS see https://en.wikipedia.org/wiki/HATEOAS

## Read API

### GET

1. Person -> /api/person/id - Where id is a GUID
2. People (Page) -> /api/people/

Person JSON

```
{
    "Id":"6149ed6f-1fed-49a5-bee3-1e9bfcd6ee57",
    "FirstName": "John",
    "LastName": "Doe",
    "Sex": "Male",
    "Gender": "Giscender",
    "DateOfBirth": "2000-04-01",
    "_links": [
        {"hypertextReference":"http://localhost/api/person/6149ed6f-1fed-49a5-bee3-1e9bfcd6ee57","relationship":"self","method":"GET"},
        {"hypertextReference":"http://localhost/api/person/6149ed6f-1fed-49a5-bee3-1e9bfcd6ee57","relationship":"self","method":"DELETE"}
    ]
}
```

People JSON

```
ToDo
```

## Write API

### POST
### PUT
### PATCH
### DELETE

## Logging

The application utilises Serilog to perform some pretty cool logging features.

### SEQ

SEQ is a ?

To use Seq, you must first install it: https://datalust.co/download/

The default endpoint is: http://localhost:5341/

Useername: admin/admin

## Tests

### Unit Tests

### Integration Tests

### System Tests


