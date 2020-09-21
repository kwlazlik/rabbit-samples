﻿using System;
using System.Text;
using System.Threading;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSamples.Ack.Consumer
{
   internal static class AckConsumer
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();

         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "durable-queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

         channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

         Console.WriteLine("--- Waiting for messages ...");

         var consumer = new EventingBasicConsumer(channel);

         consumer.Received += (model, ea) =>
         {
            ReadOnlyMemory<byte> body = ea.Body;
            string message = Encoding.UTF8.GetString(body.Span);

            Thread.Sleep(2500);

            Console.WriteLine("--- Message received: {0}", message);

            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
         };

         channel.BasicConsume(queue: "durable-queue", autoAck: false, consumer);

         Console.WriteLine("--- Waiting for messages ...");
         Console.Read();
      }
   }
}