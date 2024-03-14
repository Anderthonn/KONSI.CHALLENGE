namespace KONSI.CHALLENGE.QUEUE
{
    public interface IRabbitMQConsumer
    {
        void EnqueueCPFList(string cpf);
        Task<List<string>> ConsumeCPFAsync();
        void Dispose();
    }
}