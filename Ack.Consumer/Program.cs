using System;
using System.Text;
using System.Threading;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// ReSharper disable ArgumentsStyleNamedExpression
// ReSharper disable ArgumentsStyleLiteral
// ReSharper disable ArgumentsStyleStringLiteral

namespace RabbitSamples.Ack.Consumer
{
   internal class Program
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();

         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "durable_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

         channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

         Console.WriteLine("-- Waiting for messages ...");

         var consumer = new EventingBasicConsumer(channel);

         consumer.Received += (model, ea) =>
         {
            byte[] body = ea.Body;
            string message = Encoding.UTF8.GetString(body);

            Console.WriteLine("-- Receiving message: {0}", message);

            Thread.Sleep(5000);

            Console.WriteLine("-- Message received");

            channel.BasicAck(ea.DeliveryTag, multiple: false);
         };

         channel.BasicConsume(queue: "durable_queue", autoAck: false, consumer);

         Console.ReadLine();
      }
   }
}