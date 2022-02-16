Users - Read API
================

This API will handle the following methods:

- OPTIONS
- GET -> Single user
- GET -> Page of users
- HEAD -> 
- HEAD -> 

To see the Open API specification, simply hit:

```http://localhost:[PORT NUMBER]```

---

### GET - Single User

---

To get a single user simply call the endpoint with the user's Id:

```http://localhost/api/[VERSION]/users/[GUID]```

i.e. 

```http://localhost/api/v1/users/9852281d-f49f-4c3a-b28b-aad86f2741ee```

Can be used with the following parameters:

- Fields

#### Fields

By defaults all fields are returned. However, this can be controlled by specifying a field list. Id is always returned.

```http://localhost/api/[VERSION]/users/[GUID]?fields=[FIELD LIST]```

i.e.

```http://localhost/api/v1/users/9852281d-f49f-4c3a-b28b-aad86f2741ee?fields=firstname,lastname```

---

### GET - Multiple Users

---

To get a page of multiple users simply call the endpoint with the optional parameters:

```http://localhost/api/[VERSION]/users```

i.e. 

```http://localhost/api/v1/users```

Can be used with the following parameters:

- Page Number [Defaults to 1]
- Page Size [Defaults to 25]
- Fields
- Filter
- Search
- Sort

#### Pagination -> Page Number and Page Size

To control the number of results that are returned, the results are paginated via page number and page size.  
If these parameters aren't supplied then they default to a page number of 1 and page size of 10.

```http://localhost/api/[VERSION]/users?pageNumber=1&pageSize=25```

i.e. 

```http://localhost/api/[VERSION]/users?pageNumber=1&pageSize=25```

#### Fields

By defaults all fields are returned. However, this can be controlled by specifying a field list. Id is always returned.

```http://localhost/api/[VERSION]/users?fields=[FIELD LIST]```

i.e.

```http://localhost/api/v1/users?fields=firstname,lastname```

#### Filter

A filter can be used to reduce the results. It can be applied to any field.

```http://localhost:[PORT]/api/[VERSION]/users?filter=[FIELD NAME]:[FIELD VALUE]```

i.e. 

```http://localhost/api/[VERSION]/users?filter=sex:male```

#### Search

Searches for all occurrances of a string in all text searchable fields.

```http://localhost:[PORT]/api/[VERSION]/users?search=[VALUE]```

i.e. 

```http://localhost/api/[VERSION]/users?search=aardvark```

#### Sort

This sorts the results into a particular order.

```http://localhost:[PORT]/api/[VERSION]/users?sort=[FIELD NAME]:[FIELD VALUE]```

i.e. 

```http://localhost/api/[VERSION]/users?filter=sex:male```

---

An event consumer will listen for user related events and update the database accordingly.

- UserCreatedEvent
- UserUpdatedEvent
- UserDeletedEvent