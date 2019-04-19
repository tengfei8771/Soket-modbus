using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            opcua o = new opcua("127.0.0.1", 502);
            Console.WriteLine(o.Msg);
            Console.ReadKey();
        }
    }
}
