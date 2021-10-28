# Users --> Write API

This API will handle commands:

- POST --> Create
- PUT --> Update
- PATCH --> Update
- DELETE --> Delete

The above commands will be used to keep the database up to date.
An event producer will publish events accordingly.

- UserCreatedEvent
- UserUpdatedEvent
- UserDeletedEvent