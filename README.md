# TaskProcessor - Sistema de Processamento de Tarefas

Sistema de processamento de tarefas em background com API REST, workers assíncronos e filas RabbitMQ.

## Tecnologias

- .NET 
- MongoDB
- RabbitMQ
- Docker

## Estrutura do Projeto
- Domain
- Infrasctructure
- Application
- Api
- Worker

## Instruções 
- git clone https://github.com/giovannebraga10/TaskProcessor.git
- Navegue até a pasta em que está o docker-compose e digite `docker-compose up -d`  
  Isso subirá os containers com RabbitMQ e MongoDb
- `dotnet run` em TaskProcessor.API e TaskProcessor.Worker

## Endpoints

- **POST** | localhost:7293/api/tasks

**BODY:**  
// Apenas um exemplo, pois o serviço em si, não foi implementado.

```json
{
  "payload": {  
    "title": "Enviar relatório diário",
    "description": "Gerar e enviar o relatório de vendas do dia por e-mail",
    "priority": "High",
    "dueDate": "2025-09-05T17:00:00",
    "metadata": {
      "department": "Vendas",
      "reportType": "Diário"
    },
    "typeTask": 0  // 0 ou 1 para definir o tipo da tarefa (SendEmail, GenerateReport)
  }
}
```


- GET | localhost:7293/api/tasks/{id}
  Retornará status da atividade (pending, confirmed, canceled)


## Futuras Melhorias

- Adicionar logs estruturados para API e Workers.
- Implementação de serviços.
- Workers e filas dedicadas para garantir responsabilidade.
- Adicionar conceitos como idempotencia, para garantir resieliencia
- Criar dijuntor como CircuitBreaker que impede a repetição contínua de chamadas a serviços falhos.

