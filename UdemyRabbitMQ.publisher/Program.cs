using RabbitMQ.Client;
using System.Text;
using UdemyRabbitMQ.publisher;

var factory = new ConnectionFactory();
factory.Uri = new Uri(
    "amqps://jkqvnpvo:eXkyQt9XWMfL4QjfIenTHOOc3PDRUC9q@clam.rmq.cloudamqp.com/jkqvnpvo");

using var connection = factory.CreateConnection();

var channel = connection.CreateModel();

channel.ExchangeDeclare("header-exchange", durable:true, type:ExchangeType.Headers);

Dictionary<string, object> headers = new Dictionary<string, object>();
headers.Add("format", "pdf");
headers.Add("shape", "a4");

var properties = channel.CreateBasicProperties();
properties.Headers = headers;
properties.Persistent = true; //mesajlarımın kalıcı kale gelmesi içindir.

//var message = Encoding.UTF8.GetString(e.Body.ToArray());
channel.BasicPublish("header-exchange", string.Empty,properties
                    ,Encoding.UTF8.GetBytes("header mesajım"));

Console.WriteLine("Mesaj gönderilmiştir.");
Console.ReadLine();