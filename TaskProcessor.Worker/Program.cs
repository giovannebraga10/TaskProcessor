using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using TaskProcessor.Application.Services;
using TaskProcessor.Domain.Aggregates.TaskAggregate.Interfaces;
using TaskProcessor.Infrastructure.Messaging;
using TaskProcessor.Infrastructure.Persistence;
using TaskProcessor.Infrastructure.Persistence.Interfaces;
using TaskProcessor.Infrastructure.Persistence.Repositories;

var builder = Host.CreateApplicationBuilder(args);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection("RabbitMqSettings"));

builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();

builder.Services.AddScoped<ITaskProcessorService, TaskProcessorService>();

builder.Services.AddSingleton<TaskWorker>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<TaskWorker>());

builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMqSettings>>().Value);

var host = builder.Build();
host.Run();
