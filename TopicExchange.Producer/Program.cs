using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using RabbitMQ.Client;

// ReSharper disable FunctionNeverReturns
// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.TopicExchange.Producer
{
   internal static class Program
   {
      private static readonly Random Random = new Random();

      public static async Task Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.ExchangeDeclare(exchange: "topic-exchange", type: ExchangeType.Topic, durable: false, autoDelete: false, arguments: null);

         string[] colors =
         {
            "red",
            "green",
            "yellow"
         };

         string[] taste =
         {
            "sweet",
            "spicy"
         };

         string[] vegetables =
         {
            "carrot",
            "tomato",
            "pepper"
         };

         while (true)
         {
            string routingKey = $"{colors.Pick()}.{taste.Pick()}.{vegetables.Pick()}";
            string message = $"New fresh {routingKey.Replace('.', ' ')} {DateTime.Now:o}";
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "topic-exchange", routingKey: routingKey, basicProperties: null, body: body);

            Console.WriteLine("-- Message sent: {0}", message);

            await Task.Delay(Random.Next(500, 1000));
         }
      }

      public static T Pick<T>(this IReadOnlyList<T> list) => list[Random.Next(list.Count)];
   }
}