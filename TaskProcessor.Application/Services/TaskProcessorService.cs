namespace TaskProcessor.Application.Services
{
    public class TaskProcessorService : ITaskProcessorService
    {
        public async Task ProcessSendEmail(string payload)
        {      
            await Task.Delay(100); // simula processamento
        }

        public async Task ProcessGenerateReport(string payload)
        { 
            await Task.Delay(200); // simula processamento
        }
    }
}

