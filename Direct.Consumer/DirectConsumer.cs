﻿using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSamples.Direct.Consumer
{
   internal static class DirectConsumer
   {
      public static void Main()
      {
         var factory = new ConnectionFactory
         {
            UserName = ConnectionFactory.DefaultUser,
            Password = ConnectionFactory.DefaultPass,
            VirtualHost = ConnectionFactory.DefaultVHost,
            HostName = "localhost",
            Port = AmqpTcpEndpoint.UseDefaultPort
         };

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.QueueDeclare(queue: "sample-direct-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

         var consumer = new EventingBasicConsumer(channel);

         consumer.Received += (model, ea) =>
         {
            ReadOnlyMemory<byte> body = ea.Body;
            string message = Encoding.UTF8.GetString(body.Span);

            Console.WriteLine("--- Message received: {0}", message);
         };

         channel.BasicConsume(queue: "sample-direct-queue", autoAck: true, consumer: consumer);

         Console.WriteLine("--- Waiting for messages ...");
         Console.Read();
      }
   }
}