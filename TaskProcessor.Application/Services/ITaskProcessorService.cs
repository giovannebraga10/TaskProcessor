namespace TaskProcessor.Application.Services
{
    public interface ITaskProcessorService
    {
        Task ProcessSendEmail(string payload);
        Task ProcessGenerateReport(string payload);
    }
}