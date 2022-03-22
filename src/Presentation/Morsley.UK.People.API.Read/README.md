People - Read API
=================

This API will handle the following methods:

- OPTIONS
- GET -> Person
- GET -> People
- HEAD -> Person
- HEAD -> People

To see the Open API specification, simply hit:

```http://localhost:[PORT NUMBER]```

---

### GET -> Person

---

To get a single person simply call the endpoint with the person's Id:

```http://localhost/api/people/[GUID]```

i.e. 

```http://localhost/api/v1/people/9852281d-f49f-4c3a-b28b-aad86f2741ee```

Can be used with the following parameters:

- Fields

#### Fields

By defaults all fields are returned. However, this can be controlled by specifying a field list. Id is always returned.

```http://localhost/api/people/[GUID]?fields=[FIELD LIST]```

i.e.

```http://localhost/api/v1/people/9852281d-f49f-4c3a-b28b-aad86f2741ee?fields=firstname,lastname```

---

### GET -> People

---

To get a page of people simply call the endpoint with the optional parameters:

```http://localhost/api/people```

i.e. 

```http://localhost/api/people```

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

```http://localhost/api/people?pageNumber=1&pageSize=25```

i.e. 

```http://localhost/api/people?pageNumber=1&pageSize=25```

#### Fields

By defaults all fields are returned. However, this can be controlled by specifying a field list. Id is always returned.

```http://localhost/api/people?fields=[FIELD LIST]```

i.e.

```http://localhost/api/v1/people?fields=firstname,lastname```

#### Filter

A filter can be used to reduce the results. It can be applied to any field.

```http://localhost:[PORT]/api/people?filter=[FIELD NAME]:[FIELD VALUE]```

i.e. 

```http://localhost/api/people?filter=sex:male```

#### Search

Searches for all occurrances of a string in all text searchable fields.

```http://localhost:[PORT]/api/people?search=[VALUE]```

i.e. 

```http://localhost/api/people?search=aardvark```

#### Sort

This sorts the results into a particular order.

```http://localhost:[PORT]/api/people?sort=[FIELD NAME]:[FIELD VALUE]```

i.e. 

```http://localhost/api/people?filter=sex:male```

---

An event consumer will listen for person related events and update the database accordingly.

- PersonCreatedEvent
- PersonUpdatedEvent
- PersonDeletedEvent