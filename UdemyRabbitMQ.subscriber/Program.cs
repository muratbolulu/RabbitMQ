using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System.Text;
using System.Text.Json;

var factory = new ConnectionFactory();
factory.Uri = new Uri(
    "amqps://jkqvnpvo:eXkyQt9XWMfL4QjfIenTHOOc3PDRUC9q@clam.rmq.cloudamqp.com/jkqvnpvo");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();
channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

var queueName = channel.QueueDeclare().QueueName;

Dictionary<string,object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");
//headers.Add("x-match", "all");//tüm headers parametrelerine bakar.
headers.Add("x-match", "any");// bir adet headers parametresi uysa yeterli.

channel.QueueBind(queueName, "header-exchange","",headers);

channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar dinleniyor..");

consumer.Received += (sender, e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());

    var product = JsonSerializer.Deserialize<Product>(message);

    Thread.Sleep(1000);

    Console.WriteLine($"Gelen Mesaj: - {product.ID} - {product.Name} - {product.Price} - {product.Stock}");

    channel.BasicAck(e.DeliveryTag, true);
};
//kuyruktaki mesajlardan 10 varsa 10 işliyor fakat ekrana 9 yazdırıyor.
String consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();