
//Console.WriteLine("Hello, World!");

using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri(
    "amqps://bxjaidjr:hf7zKNpwJDGxBjcAwBiXX_6gYrTgLq4k@toad.rmq.cloudamqp.com/bxjaidjr");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();
//channel.QueueDeclare("hello-queue", true, false, false);

channel.ExchangeDeclare("logs-fanout", durable:true, type:ExchangeType.Fanout);

Enumerable.Range(0, 50).ToList().ForEach(x =>
{
    string message = $"Message {x}";

    var messageBody = Encoding.UTF8.GetBytes(message);
    //channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);
    channel.BasicPublish("logs-fanout","", null, messageBody);
    Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
});

Console.ReadLine();