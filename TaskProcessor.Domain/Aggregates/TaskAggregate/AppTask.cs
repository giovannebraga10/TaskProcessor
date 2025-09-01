using Newtonsoft.Json;

namespace TaskProcessor.Domain.Aggregates.TaskAggregate
{
    public class AppTask
    {
        [JsonProperty]
        public Guid Id { get; private set; }

        [JsonProperty]
        public string Payload { get; private set; }

        [JsonProperty]
        public ETaskType Type { get; private set; }

        [JsonProperty]
        public DateTime CreatedAt { get; private set; }

        [JsonProperty]
        public ETaskStatus Status { get; private set; }
        public AppTask() { }

        public AppTask(Guid id, string payload, ETaskType type, DateTime createdAt, ETaskStatus status)
        {
            Id = id;
            Payload = payload;
            Type = type;
            CreatedAt = createdAt;
            Status = status;
        }

        private AppTask(ETaskType type, string payload)
        {
            Id = Guid.NewGuid();
            Payload = payload;
            Type = type;
            CreatedAt = DateTime.UtcNow;
            Status = ETaskStatus.Pending;
        }

        public static AppTask Create(ETaskType type, string payload)
        {
            return new AppTask(type, payload);
        }

        public void MarkAsCompleted()
        {
            if (Status != ETaskStatus.Pending)
                throw new InvalidOperationException("Task already completed ou cancelled.");

            Status = ETaskStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == ETaskStatus.Completed)
                throw new InvalidOperationException("Completed task cannot be cancelled.");

            Status = ETaskStatus.Cancelled;
        }
    }
}
