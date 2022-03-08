using RabbitMQ.Client;
using System.Text;
using UdemyRabbitMQ.publisher;

var factory = new ConnectionFactory();
factory.Uri = new Uri(
    "amqps://jkqvnpvo:eXkyQt9XWMfL4QjfIenTHOOc3PDRUC9q@clam.rmq.cloudamqp.com/jkqvnpvo");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("logs-topic", durable:true, type:ExchangeType.Topic);

Random random = new Random();

Enumerable.Range(0, 50).ToList().ForEach(x =>
{
    LogNames log1 = (LogNames)random.Next(1, 5);
    LogNames log2 = (LogNames)random.Next(1, 5);
    LogNames log3 = (LogNames)random.Next(1, 5);

    var routeKey = $"{log1}.{log2}.{log3}";

    string message = $"log-type: {log1}-{log2}-{log3}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("logs-topic", routeKey, null, messageBody);
    Console.WriteLine($"Log Gönderilmiştir : {message}");
});

Console.ReadLine();