using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace RabbitSamples.Fanout.Consummer
{
   class Program
   {
      public static void Main()
      {
         var factory = new ConnectionFactory();

         using IConnection connection = factory.CreateConnection();
         using IModel channel = connection.CreateModel();

         channel.ExchangeDeclare(exchange: "sample-fanout-exchange", type: ExchangeType.Fanout);

         string queueName = channel.QueueDeclare().QueueName;
         channel.QueueBind(queue: queueName, exchange: "sample-fanout-exchange", routingKey: "");

         var consumer = new EventingBasicConsumer(channel);
         consumer.Received += (model, ea) =>
         {
            byte[] body = ea.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            Console.WriteLine("--- Message received: {0}", message);
         };

         channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

         Console.WriteLine("--- Waiting for messages ...");
         Console.Read();
      }
   }
}
