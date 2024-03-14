using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace KONSI.CHALLENGE.QUEUE
{
    public class RabbitMQConsumer : IRabbitMQConsumer, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

            var url = _configuration.GetSection("Rabbitmq").GetSection("RabbitmqAddress").Value ?? "";

            var factory = new ConnectionFactory { Uri = new Uri(url) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "cpf_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void EnqueueCPFList(string cpf)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(cpf);
                _channel.BasicPublish(exchange: "", routingKey: "cpf_queue", basicProperties: null, body: body);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao enfileirar CPF: " + ex.Message);
            }
        }

        public async Task<List<string>> ConsumeCPFAsync()
        {
            var cpfList = new List<string>();
            var completionSource = new TaskCompletionSource<List<string>>();

            try
            {
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (model, ea) =>
                {
                    var bodyCpf = Encoding.UTF8.GetString(ea.Body.ToArray());
                    cpfList.Add(bodyCpf);

                    if (cpfList.Count == 10) // Supondo que queremos receber 10 mensagens
                    {
                        completionSource.SetResult(cpfList);
                    }
                };

                _channel.BasicConsume(queue: "cpf_queue", autoAck: false, consumer: consumer);

                return await completionSource.Task;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consumir CPF: " + ex.Message);
            }
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
