using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using RabbitMQ.Client;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.TopicExchange.Producer
{
   internal class Program
   {
      public static void Main()
      {
         ConnectionFactory factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();

         using IModel channel = connection.CreateModel();

         channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);

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
            string message = $"New fresh {routingKey.Replace('.', ' ')}";
            byte[] body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "topic_logs", routingKey: routingKey, basicProperties: null, body: body);

            Console.WriteLine("Message sent: {0}", message);

            Thread.Sleep(1000);
         }
      }
   }

   internal static class ListExtensions
   {
      private static readonly Random Random = new Random();

      public static T Pick<T>(this IReadOnlyList<T> list)
      {
         return list[Random.Next(list.Count)];
      }
   }
}