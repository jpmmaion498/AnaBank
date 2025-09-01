# AnaBank - Arquitetura e Design

## ??? Vis�o Geral da Arquitetura

O AnaBank foi desenvolvido seguindo os princ�pios de **Domain-Driven Design (DDD)** e **CQRS (Command Query Responsibility Segregation)**, implementado em **.NET 8** com microsservi�os independentes.

## ?? Diagrama de Arquitetura

```mermaid
graph TB
    Client[Cliente/Frontend] --> Nginx[Nginx Load Balancer]
    
    Nginx --> AccountsAPI[Accounts API :8081]
    Nginx --> TransfersAPI[Transfers API :8082]
    
    AccountsAPI --> AccountsDB[(SQLite Accounts)]
    TransfersAPI --> TransfersDB[(SQLite Transfers)]
    
    TransfersAPI --> AccountsAPI
    
    TransfersAPI --> Kafka[Apache Kafka]
    Kafka --> FeesWorker[Fees Worker]
    FeesWorker --> FeesDB[(SQLite Fees)]
    FeesWorker --> Kafka
    Kafka --> AccountsAPI
    
    AccountsAPI --> Redis[(Redis Cache)]
    TransfersAPI --> Redis
```

## ??? Camadas da Arquitetura (DDD)

### **Domain Layer**
- **Entities**: CurrentAccount, Movement, Transfer
- **Value Objects**: CPF, AccountNumber
- **Domain Services**: Valida��es de neg�cio
- **Interfaces**: Contratos para infraestrutura

### **Application Layer** 
- **Commands**: RegisterAccount, MakeMovement, MakeTransfer
- **Queries**: GetBalance
- **Handlers**: L�gica de aplica��o usando MediatR
- **Validators**: FluentValidation para regras
- **Behaviors**: Cross-cutting concerns (valida��o, logging)

### **Infrastructure Layer**
- **Repositories**: Implementa��o com Dapper + SQLite
- **External Services**: JWT, HTTP Clients, Kafka
- **Data Access**: Connection Factory, Migrations

### **API Layer**
- **Controllers**: Endpoints REST
- **Middleware**: JWT, Idempot�ncia, Exception Handling
- **Configuration**: DI, Swagger, CORS

## ?? Padr�es Implementados

### **CQRS (Command Query Responsibility Segregation)**
- **Commands**: Opera��es que modificam estado
- **Queries**: Opera��es de leitura
- **Handlers**: Separa��o clara de responsabilidades
- **MediatR**: Desacoplamento entre controllers e l�gica

### **DDD (Domain-Driven Design)**
- **Ubiquitous Language**: Linguagem comum entre neg�cio e c�digo
- **Bounded Contexts**: Accounts, Transfers, Fees
- **Aggregates**: Entidades com consist�ncia transacional
- **Domain Events**: Kafka para comunica��o ass�ncrona

### **Microservices Patterns**
- **Database per Service**: Cada servi�o tem seu banco
- **API Gateway**: Nginx como proxy reverso
- **Service Communication**: HTTP s�ncrono + Kafka ass�ncrono
- **Health Checks**: Monitoramento de sa�de

## ?? Seguran�a

### **Autentica��o e Autoriza��o**
- **JWT Bearer Tokens**: Stateless authentication
- **Token Validation**: Middleware customizado
- **Claim-based Authorization**: Identifica��o de usu�rios
- **HTTPS**: Comunica��o segura (produ��o)

### **Prote��o de Dados**
- **Password Hashing**: BCrypt com salt �nico
- **CPF Validation**: Algoritmo padr�o brasileiro
- **Input Validation**: FluentValidation + Model binding
- **SQL Injection**: Prote��o via Dapper parametrizado

## ?? Idempot�ncia

### **Implementa��o**
- **Idempotency-Key Header**: Cliente envia chave �nica
- **Middleware**: Intercepta e valida requisi��es
- **Storage**: SQLite para cache de respostas
- **Retry Safety**: Opera��es seguras para reenvio

## ?? Observabilidade

### **Logging**
- **Structured Logging**: JSON format
- **Log Levels**: Debug, Info, Warning, Error
- **Correlation IDs**: Rastreamento de requisi��es
- **Performance Metrics**: Tempo de resposta

### **Health Checks**
- **Application Health**: `/health` endpoints
- **Database Connectivity**: Verifica��o de conex�es
- **External Services**: Status de depend�ncias
- **Docker Health**: Container monitoring

## ??? Persist�ncia

### **Database Design**
- **SQLite**: Simplicidade e portabilidade
- **Schema per Service**: Isolamento de dados
- **ACID Transactions**: Consist�ncia garantida
- **Connection Pooling**: Performance otimizada

### **Data Access**
- **Dapper**: Micro-ORM perform�tico
- **Repository Pattern**: Abstra��o de dados
- **Connection Factory**: Gerenciamento de conex�es
- **Migration Scripts**: Versionamento de schema

## ?? Qualidade e Testes

### **Testes Automatizados**
- **Unit Tests**: xUnit + FluentAssertions
- **Integration Tests**: WebApplicationFactory
- **Test Isolation**: Banco em mem�ria
- **Coverage Reports**: Cobertura de c�digo

### **Valida��o**
- **Business Rules**: Domain layer validation
- **Input Validation**: FluentValidation
- **API Contracts**: Swagger/OpenAPI
- **Error Handling**: ProblemDetails RFC

## ?? Containeriza��o

### **Docker Strategy**
- **Multi-stage Builds**: Otimiza��o de imagens
- **Container per Service**: Isolamento e escalabilidade
- **Docker Compose**: Orquestra��o local
- **Health Checks**: Container monitoring

### **Production Ready**
- **Environment Variables**: Configura��o externa
- **Secrets Management**: Configura��o segura
- **Volume Mounts**: Persist�ncia de dados
- **Network Isolation**: Seguran�a de rede

## ?? Escalabilidade

### **Horizontal Scaling**
- **Stateless Services**: Facilita replica��o
- **Load Balancing**: Nginx como proxy
- **Database Scaling**: Read replicas (futuro)
- **Cache Layer**: Redis para performance

### **Performance**
- **Async Operations**: Non-blocking I/O
- **Connection Pooling**: Reutiliza��o de conex�es
- **Response Caching**: HTTP caching headers
- **Compression**: Gzip para payloads

## ?? DevOps e CI/CD

### **Build Pipeline**
- **Multi-stage Dockerfile**: Build otimizado
- **Test Automation**: Execu��o autom�tica
- **Quality Gates**: Coverage e linting
- **Artifact Publishing**: Container registry

### **Deployment**
- **Environment Separation**: Dev/Staging/Prod
- **Configuration Management**: appsettings per env
- **Secret Management**: Environment variables
- **Rolling Updates**: Zero-downtime deployment