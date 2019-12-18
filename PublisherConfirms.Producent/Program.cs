using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using RabbitMQ.Client;

namespace PublisherConfirms
{
   internal class Program
   {
      private const int MESSAGE_COUNT = 9000;

      public static void Main()
      {
         //PublishMessages();
         HandlePublishConfirmsAsynchronously();
      }

      private static void PublishMessages()
      {
         using IConnection connection = new ConnectionFactory().CreateConnection();
         using IModel channel = connection.CreateModel();

         string queueName = channel.QueueDeclare().QueueName;
         channel.ConfirmSelect();

         byte[] body = GetMessageBody(128.0);

         var timer = Stopwatch.StartNew();

         for (var i = 0; i < 1; i++)
         {
            channel.BasicPublish("", queueName, null, body);
         }

         Stopwatch sw = Stopwatch.StartNew();

         bool noNack = channel.WaitForConfirms(new TimeSpan(0, 0, 0, 0, 200), out bool timedOut);
         //channel.WaitForConfirmsOrDie(new TimeSpan(0, 0, 0, 1, 1));

         sw.Stop();
         long miliseconds = sw.ElapsedMilliseconds;

         timer.Stop();

         Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages individually in {timer.ElapsedMilliseconds:N0} ms");
      }

      private static void HandlePublishConfirmsAsynchronously()
      {
         using IConnection connection = new ConnectionFactory().CreateConnection();
         using IModel channel = connection.CreateModel();

         string queueName = channel.QueueDeclare().QueueName;
         channel.ConfirmSelect();

         var outstandingConfirms = new ConcurrentDictionary<ulong, string>();

         void CleanOutstandingConfirms(ulong sequenceNumber, bool multiple)
         {
            if (multiple)
            {
               IEnumerable<KeyValuePair<ulong, string>> confirmed = outstandingConfirms.Where(k => k.Key <= sequenceNumber);

               foreach (KeyValuePair<ulong, string> entry in confirmed)
               {
                  outstandingConfirms.TryRemove(entry.Key, out _);
               }
            }
            else
            {
               outstandingConfirms.TryRemove(sequenceNumber, out _);
            }
         }

         channel.BasicAcks += (sender, ea) => CleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);

         channel.BasicNacks += (sender, ea) =>
         {
            outstandingConfirms.TryGetValue(ea.DeliveryTag, out string body);
            Console.WriteLine($"Message with body {body} has been nack-ed. Sequence number: {ea.DeliveryTag}, multiple: {ea.Multiple}");
            CleanOutstandingConfirms(ea.DeliveryTag, ea.Multiple);
         };

         byte[] body = GetMessageBody(10);

         var timer = new Stopwatch();
         timer.Start();

         for (var i = 0; i < MESSAGE_COUNT; i++)
         {
            outstandingConfirms.TryAdd(channel.NextPublishSeqNo, i.ToString());
            channel.BasicPublish("", queueName, null, body);
         }

         if (!WaitUntil(1, () => outstandingConfirms.IsEmpty))
         {
            throw new Exception("All messages could not be confirmed in 60 seconds");
         }

         timer.Stop();
         Console.WriteLine($"Published {MESSAGE_COUNT:N0} messages and handled confirm asynchronously {timer.ElapsedMilliseconds:N0} ms");
      }

      private static byte[] GetMessageBody(double megaBytes)
      {
         byte[] body = new byte[(int)(megaBytes * 1024 * 1024)];
         new Random().NextBytes(body);
         return body;
      }

      private static bool WaitUntil(int numberOfSeconds, Func<bool> condition)
      {
         var waited = 0;

         while (!condition() && waited < numberOfSeconds * 1000)
         {
            Thread.Sleep(100);
            waited += 100;
         }

         return condition();
      }
   }
}