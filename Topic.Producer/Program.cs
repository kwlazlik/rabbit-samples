using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitSamples.TopicExchange.Producer
{
   internal static class Program
   {
      public static async Task Main()
      {
         var factory = new ConnectionFactory();

         using var connection = factory.CreateConnection();
         using var channel = connection.CreateModel();

         channel.ExchangeDeclare("sample-topic-exchange", ExchangeType.Topic, false, false, null);

         while (true)
         {
            (string message, string key) = PickMessageAndKey();

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("sample-topic-exchange", key, null, body);

            Console.WriteLine("--- Message sent: {0}", message);

            await Task.Delay(Random.Next(2500, 5000));
         }
      }

      private static readonly Random Random = new Random();

      private static T Pick<T>(this IReadOnlyList<T> list) => list[Random.Next(list.Count)];

      private static (string, string) PickMessageAndKey()
      {
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

         var key = $"{colors.Pick()}.{taste.Pick()}.{vegetables.Pick()}";

         var message = $"{key.Replace('.', ' ')} {DateTime.Now:HH:mm:ss.fff}";

         return (message, key);
      }
   }
}