using RabbitMQ.Client;
using System.Text;
using UdemyRabbitMQ.publisher;

var factory = new ConnectionFactory();
factory.Uri = new Uri(
    "amqps://bxjaidjr:hf7zKNpwJDGxBjcAwBiXX_6gYrTgLq4k@toad.rmq.cloudamqp.com/bxjaidjr");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("logs-direct", durable:true, type:ExchangeType.Direct);

Enum.GetNames(typeof(LogNames)).ToList().ForEach(x =>
{
    var routeKey = $"route-{x}";
    var queueName = $"direct-queue-{x}";
    channel.QueueDeclare(queueName, true,false,false);

    channel.QueueBind(queueName, "logs-direct", routeKey,null);
});

Enumerable.Range(0, 50).ToList().ForEach(x =>
{
    LogNames log = (LogNames)new Random().Next(1,5);

    string message = $"log-type: {log}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    var routeKey = $"route-{log}";
    channel.BasicPublish("logs-direct", routeKey, null, messageBody);
    Console.WriteLine($"Log Gönderilmiştir : {message}");
});

Console.ReadLine();