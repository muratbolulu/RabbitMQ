
//Console.WriteLine("Hello, World!");

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://bxjaidjr:hf7zKNpwJDGxBjcAwBiXX_6gYrTgLq4k@toad.rmq.cloudamqp.com/bxjaidjr");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();
//channel.QueueDeclare("hello-queue", true, false, false);
//channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);
var randomQueueName = channel.QueueDeclare().QueueName;
//var randomQueueName = "log-database-save-queue";//channel.QueueDeclare().QueueName;
//channel.QueueDeclare(randomQueueName, true, false, false);

//bind ederek subscriber gidince kuyrukta gitmesi için bind edildi.
channel.QueueBind(randomQueueName,"logs-fanout","",null);

channel.BasicQos(0,1,false);
var consumer = new EventingBasicConsumer(channel);
//channel.BasicConsume("hello-queue", false, consumer);
channel.BasicConsume(randomQueueName, false, consumer);

Console.WriteLine("Loglar dinleniyor");

consumer.Received += (sender, args) =>
{
    var message = Encoding.UTF8.GetString(args.Body.ToArray());
    Thread.Sleep(1000);

    Console.WriteLine("Gelen Mesaj:" + message);

    channel.BasicAck(args.DeliveryTag, false);
};

Console.ReadLine();