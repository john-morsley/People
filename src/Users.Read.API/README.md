# Users --> Read API

This API will handle queries:

- GET --> Get a single users
- GET --> Gets a page of users

An event consumer will listen for user related events and update the database accordingly.

- UserCreatedEvent
- UserUpdatedEvent
- UserDeletedEvent