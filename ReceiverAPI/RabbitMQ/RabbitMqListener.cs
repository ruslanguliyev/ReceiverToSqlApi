using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReceiverAPI.Data;
using ReceiverAPI.Models;
using System.Diagnostics;
using System.Text;

namespace ReceiverAPI.RabbitMQ
{
    public class RabbitMqListener : BackgroundService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<RabbitMqListener> _logger;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMqListener()
        {
            var factory = new ConnectionFactory() { Uri = new Uri("amqps://ffyblixq:WrJErT5jwSUNkYFBmJrZgMyoUIGL1akT@stingray.rmq.cloudamqp.com/ffyblixq") };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(queue: "message_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var user = JsonConvert.DeserializeObject<User>(content);




                _dbContext.Users.AddRange(user);
                _dbContext.SaveChanges();

                //Debug.WriteLine($"Получено сообщение: {content}");


                _channel.BasicAck(ea.DeliveryTag, false);
            };

            _channel.BasicConsume("message_queue", false, consumer);

            return Task.CompletedTask;
        }
        

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
