using System;
using System.Collections.Generic;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.TopicExchange.Consumer
{
   internal static class Program
   {
      private static readonly Random Random = new Random();

      public static void Main()
      {
         ConnectionFactory factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.ExchangeDeclare(exchange: "topic-exchange", type: "topic", durable: false, autoDelete: false, arguments: null);
         string queueName = channel.QueueDeclare().QueueName;

         string[] bindingKeys =
         {
            "*.spicy.pepper",
            "green.*.tomato",
            "red.#",
            "#.carrot"
         };

         string bindingKey = bindingKeys.Pick();

         channel.QueueBind(queue: queueName,
            exchange: "topic-exchange",
            routingKey: bindingKey);

         Console.WriteLine($"-- Waiting for messages matches: {bindingKey}");

         EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
         consumer.Received += (model, ea) =>
         {
            byte[] body = ea.Body;
            string message = Encoding.UTF8.GetString(body);
            string routingKey = ea.RoutingKey;

            Console.WriteLine($"-- Message received: '{routingKey}':'{message}'");
         };

         channel.BasicConsume(queue: queueName,
            autoAck: true,
            consumer: consumer);

         Console.ReadLine();
      }

      public static T Pick<T>(this IReadOnlyList<T> list) => list[Random.Next(list.Count)];
   }
}