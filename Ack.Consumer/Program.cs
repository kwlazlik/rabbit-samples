using System;
using System.Text;
using System.Threading;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

// ReSharper disable AccessToDisposedClosure
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

         channel.QueueDeclare(queue: "durable-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

         channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

         Console.WriteLine("-- Waiting for messages ...");

         var consumer = new EventingBasicConsumer(channel);

         consumer.Received += (model, ea) =>
         {
            byte[] body = ea.Body;
            string message = Encoding.UTF8.GetString(body);

            Thread.Sleep(2500);

            Console.WriteLine("-- Message received: {0}", message);

            channel.BasicAck(ea.DeliveryTag, multiple: false);
         };

         channel.BasicConsume(queue: "durable-queue", autoAck: false, consumer);

         Console.ReadLine();
      }
   }
}