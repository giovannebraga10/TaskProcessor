using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TaskProcessor.Domain.Aggregates.TaskAggregate;
using TaskProcessor.Infrastructure.Persistence.Interfaces;

namespace TaskProcessor.Infrastructure.Persistence
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<AppTask> Tasks => _database.GetCollection<AppTask>("Tasks");
    }
}
