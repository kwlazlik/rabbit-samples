using System;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

// ReSharper disable FunctionNeverReturns
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.HelloWorld.Producer
{
   internal class Program
   {
      private static readonly Random Random = new Random();

      public static async Task Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "hello-world-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

         while (true)
         {
            string message = $"Hello World! {DateTime.Now:o}";
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: "hello-world-queue", basicProperties: null, body);

            Console.WriteLine($"-- Message sent: {message}");

            await Task.Delay(Random.Next(500, 1000));
         }
      }
   }
}