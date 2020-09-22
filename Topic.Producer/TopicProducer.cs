using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace RabbitSamples.Topic.Producer
{
   internal static class TopicProducer
   {
      public static async Task Main()
      {
         var factory = new ConnectionFactory();

         using var connection = factory.CreateConnection();
         using var channel = connection.CreateModel();

         channel.ExchangeDeclare("sample-topic-exchange", ExchangeType.Topic, false, false, null);

         while (true)
         {
            (string messageText, string key) = PickMessageAndKey(); // ("red sweet apple", "red.sweet.apple")

            var body = Encoding.UTF8.GetBytes(messageText);

            channel.BasicPublish("sample-topic-exchange", key, null, body);

            Console.WriteLine("--- Message sent: {0}", messageText);

            await Task.Delay(3000);
         }
      }

      private static readonly Random Random = new Random(123);

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

         var messageText = $"{key.Replace('.', ' ')} {DateTime.Now:HH:mm:ss.fff}";

         return (messageText, key);
      }
   }
}