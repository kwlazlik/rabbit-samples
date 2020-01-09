using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

// ReSharper disable RedundantBlankLines
// ReSharper disable ArgumentsStyleOther
// ReSharper disable FunctionNeverReturns
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.DeadLetters.Producer
{
   internal static class Program
   {
      private static readonly Random Random = new Random();

      public static async Task Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.ExchangeDeclare("task-exchange", type: "direct", durable: false, autoDelete: false,
            arguments: new Dictionary<string, object>
            {
               { "alternate-exchange", "dl-exchange" }
            });

         channel.QueueDeclare("task-queue", durable: false, exclusive: false, autoDelete: false,
            arguments: new Dictionary<string, object>
            {
               { "x-dead-letter-exchange", "dl-exchange" }
            });

         channel.QueueBind(queue: "task-queue", exchange: "task-exchange", routingKey: "task-rk");

         channel.ExchangeDeclare("dl-exchange", type: "fanout", durable: false, autoDelete: false, arguments: null);
         channel.QueueDeclare("dl-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
         channel.QueueBind(queue: "dl-queue", exchange: "dl-exchange", routingKey: "");

         while (true)
         {
            string rk = Random.Next() % 10 == 0 ? "invalid-rk" : "task-rk";

            string message = $"Hello World! {rk} {DateTime.Now:o}";
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "task-exchange", routingKey: rk, basicProperties: null, body: body);

            Console.WriteLine("-- Message sent: {0}", message);

            await Task.Delay(Random.Next(500, 1000));
         }
      }
   }
}