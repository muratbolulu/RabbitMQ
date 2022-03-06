
//Console.WriteLine("Hello, World!");

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://bxjaidjr:hf7zKNpwJDGxBjcAwBiXX_6gYrTgLq4k@toad.rmq.cloudamqp.com/bxjaidjr");

var connection = factory.CreateConnection();

var channel = connection.CreateModel();
//channel.QueueDeclare("hello-queue", true, false, false);

channel.BasicQos(0,1,false);
var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume("hello-queue", false, consumer);

consumer.Received += (sender, args) =>
{
    var message = Encoding.UTF8.GetString(args.Body.ToArray());
    Thread.Sleep(1000);

    Console.WriteLine("Gelen Mesaj:" + message);

    channel.BasicAck(args.DeliveryTag, false);
};

Console.ReadLine();