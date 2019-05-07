using System;
using System.Threading;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            ModBusNet o = new ModBusNet("127.0.0.1", 502);
            while (true)
            {
                o.GetTestMsg("20",19);
                Console.WriteLine(o.Msg);
                Thread.Sleep(1000);
            }
            //o.ReadCoil("16");
            //BinaryHelper.TenToSixteen(15);
            //o.ReadCoil("15");
            //o.PackCommand(200);
            


        }
    }
}
