# Users

An ASP.NET application for users.

As we are going to implement a CQRS approach, I have split the read and write parts of the API into separate projects. They can now be deployed and scaled independently. 

For HATEOAS see https://en.wikipedia.org/wiki/HATEOAS

## Read API

### GET

1. User
2. Users (Page)

1. User

Expected JSON

```
{
    "Id":"6149ed6f-1fed-49a5-bee3-1e9bfcd6ee57",
    "FirstName": "John",
    "LastName": "Doe",
    "Sex": 3,
    "Gender": 2,
    "DateOfBirth": "2000-04-01",
    "_links": [
        {"hypertextReference":"http://localhost/api/v1/users/6149ed6f-1fed-49a5-bee3-1e9bfcd6ee57","relationship":"self","method":"GET"},
        {"hypertextReference":"http://localhost/api/v1/users/6149ed6f-1fed-49a5-bee3-1e9bfcd6ee57","relationship":"self","method":"DELETE"}
    ]
}
```


## Write API

### POST
### PUT
### PATCH
### DELETE

## Tests

### Unit Tests

### Integration Tests

### System Tests


