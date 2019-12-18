using System;
using System.Text;

using RabbitMQ.Client;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.HelloWorld.Producer
{
   internal class Program
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();
         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

         const string exchange = "";
         const string routingKey = "hello";
         const IBasicProperties basicProperties = null;
         const string message = "Hello World!";
         byte[] body = Encoding.UTF8.GetBytes(message);
         channel.BasicPublish(exchange, routingKey, basicProperties, body);

         Console.WriteLine($"-- Message sent: {message}");
         Console.Read();
      }
   }
}