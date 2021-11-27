Users - Read API
================

This API will handle queries:

- GET --> Get a single user
- GET --> Gets a page of users

To see the Open API specification, simply hit:

```http://localhost/```


GET - Single User
-----------------

To get a single user simply call the endpoint with the user's Id:

```http://localhost/api/[VERSION]/users/[GUID]```

i.e. 

```http://localhost/api/v1/users/9852281d-f49f-4c3a-b28b-aad86f2741ee```

GET - Multiple Users
-----------------

To get a page of multiple users simply call the endpoint with the optional parameters:

```http://localhost/api/[VERSION]/users/```

These parameters are:

- Start Page [Defaults to 1]
- Page Size [Defaults to 25]
- Search
- Filter

i.e. 

```http://localhost/api/v1/users```

---

An event consumer will listen for user related events and update the database accordingly.

- UserCreatedEvent
- UserUpdatedEvent
- UserDeletedEvent