namespace PublisherConfirms.Consumer
{
   internal class Program
   {
      public static void Main()
      {
         var cs = new ConsumerService();
         cs.Run();
      }
   }
}