using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using RabbitMQ.Client;

namespace PublisherConfirms.Consumer
{
   internal class ConsumerService
   {
      private const int MESSAGE_COUNT = 50_000;



      public void Run()
      {
         HandlePublishConfirmsAsynchronously();
      }
   }
}