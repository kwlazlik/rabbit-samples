﻿using System;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

// ReSharper disable FunctionNeverReturns
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.Ack.Producer
{
   internal class Program
   {
      private static readonly Random Random = new Random();

      public static async Task Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();

         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "durable-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

         IBasicProperties properties = channel.CreateBasicProperties();
         properties.Persistent = true;

         while (true)
         {
            string message = $"Hello World! {DateTime.Now:o}";
            byte[] body = Encoding.UTF8.GetBytes(s: message);

            channel.BasicPublish(exchange: "", routingKey: "durable-queue", basicProperties: properties, body: body);

            Console.WriteLine(format: "-- Message sent: {0}", arg0: message);

            await Task.Delay(Random.Next(500, 1000));
         }
      }
   }
}