using System;
using System.Text;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.HelloWorld.Consumer
{
   internal class Program
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();
         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

         var consumer = new EventingBasicConsumer(channel);

         consumer.Received += (model, ea) =>
         {
            byte[] body = ea.Body;
            string message = Encoding.UTF8.GetString(body);

            Console.WriteLine("-- Message received: {0}", message);
         };

         channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

         Console.WriteLine("-- Waiting for messages ...");
         Console.Read();
      }
   }
}