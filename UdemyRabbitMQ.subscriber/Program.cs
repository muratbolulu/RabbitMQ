using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri(
    "amqps://jkqvnpvo:eXkyQt9XWMfL4QjfIenTHOOc3PDRUC9q@clam.rmq.cloudamqp.com/jkqvnpvo");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);

var queueName = channel.QueueDeclare().QueueName;
//var routeKey = "*.*.Critical";
var routeKey = "Critical.#";
channel.QueueBind(queueName, "logs-topic", routeKey);

channel.BasicConsume(queueName, false, consumer);

Console.WriteLine("Loglar dinleniyor..");

consumer.Received += (sender, e) =>
{
    var message = Encoding.UTF8.GetString(e.Body.ToArray());
    Thread.Sleep(1000);

    Console.WriteLine("Gelen Mesaj:" + message);

    //File.AppendAllText("log-critical.txt", message + "\n");

    channel.BasicAck(e.DeliveryTag, true);
    //String consumerTag = channel.BasicConsume(queueName, false, consumer);
};
//kuyruktaki mesajlardan 10 varsa 10 işliyor fakat ekrana 9 yazdırıyor.
String consumerTag = channel.BasicConsume(queueName, false, consumer);

Console.ReadLine();