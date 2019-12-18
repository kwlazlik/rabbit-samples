using System;
using System.Text;
using System.Threading;

using RabbitMQ.Client;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.Ack.Producer
{
   internal class Program
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();

         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "durable_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

         IBasicProperties properties = channel.CreateBasicProperties();
         properties.Persistent = true;

         Random random = new Random(123);

         while (true)
         {
            string message = $"Hello World! {random.Next(1000)}";
            byte[] body = Encoding.UTF8.GetBytes(s: message);
            channel.BasicPublish(exchange: "", routingKey: "durable_queue", basicProperties: properties, body: body);

            Console.WriteLine(format: "-- Message sent: {0}", arg0: message);

            Thread.Sleep(1000);
         }
      }
   }
}