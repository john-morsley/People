# Infrastructure

To test the application locally you must create the necessary infrastructure:

- MongoDB (Read)
- MongoDB (Write)
- RabbitMQ
- Redis Cache (Read)
- Redis Cache GUI
- Jaeger
- Zipkin

To create the infrastructure:

```
docker-compose up
```

To destroy the infrastructure:

```
docker-compose down
```

