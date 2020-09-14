using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSamples.Topic.Consumer
{
   internal static class TopicConsumer
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.ExchangeDeclare(exchange: "sample-topic-exchange", type: ExchangeType.Topic, durable: false, autoDelete: false, arguments: null);

         string queueName = channel.QueueDeclare().QueueName;

         string key = PickKey();

         channel.QueueBind(queue: queueName, exchange: "sample-topic-exchange", routingKey: key);

         Console.WriteLine($"--- Waiting for messages matches: {key}");

         EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
         consumer.Received += (model, ea) =>
         {
            ReadOnlyMemory<byte> body = ea.Body;
            string message = Encoding.UTF8.GetString(body.Span);
            string routingKey = ea.RoutingKey;

            Console.WriteLine($"--- Message received: '{routingKey}':'{message}'");
         };

         channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

         Console.WriteLine("--- Waiting for messages ...");
         Console.Read();
      }

      private static readonly Random Random = new Random();

      private static string PickKey()
      {
         string[] keys =
         {
            "*.spicy.pepper",
            "green.*.tomato",
            "red.#",
            "#.carrot"
         };

         string key = keys[Random.Next(keys.Length)];
         return key;
      }
   }
}