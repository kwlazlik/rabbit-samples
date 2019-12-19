using System;
using System.Text;
using System.Threading;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.HelloWorld.Consumer
{
   internal class Program
   {
      private static readonly Random Random = new Random();

      public static void Main()
      {
         var factory = new ConnectionFactory();
         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "hello-world-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

         var consumer = new EventingBasicConsumer(channel);

         consumer.Received += (model, ea) =>
         {
            byte[] body = ea.Body;
            string message = Encoding.UTF8.GetString(body);

            Thread.Sleep(Random.Next(1500, 2000));

            Console.WriteLine("-- Message received: {0}", message);
         };

         channel.BasicConsume(queue: "hello-world-queue", autoAck: true, consumer: consumer);

         Console.WriteLine("-- Waiting for messages ...");
         Console.Read();
      }
   }
}