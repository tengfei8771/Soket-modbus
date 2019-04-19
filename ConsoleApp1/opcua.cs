using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public class opcua
    {
        public Socket client;
        public bool IsConneted { get; set; }
        private string _IP { get; set; }
        private int _port { get; set; }
        public string Msg { get; set; }
        public opcua(string IP,int port)
        {
            _IP = IP;
            _port = port;
            Connected();
        }
        public void Connected()
        {
            byte[] data = new byte[1024];
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(_IP.Trim()), _port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                client.Connect(iPEndPoint);
                IsConneted = true;
            }
            catch(SocketException e)
            {
                IsConneted = false;
                throw e;
            }
            TimerSender();
            Msg = ReceiveMsg();
        }

        public void TimerSender()
        {
            byte[] data = new byte[] { 0x00, 0x0f, 0x00, 0x00, 0x00, 0x06, 0x01, 0x04, 0x00, 0x00, 0x00, 0x01 };
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Enabled = true;
            timer.Interval = 5000;
            timer.Start();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(AlwaysSend);
        }
        private  void AlwaysSend(object source, System.Timers.ElapsedEventArgs e)
        {
            byte[] data = new byte[] { 0x00, 0x0f, 0x00, 0x00, 0x00, 0x06, 0x01, 0x04, 0x00, 0x00, 0x00, 0x01 };
            client.Send(data);
        }

        public string ReceiveMsg()
        {
            while (true)
            {
                byte[] data = new byte[1024];//定义数据接收数组
                client.Receive(data);//接收数据到data数组
                int length = data[5];//读取数据长度
                Byte[] datashow = new byte[length + 6];//定义所要显示的接收的数据的长度
                for (int i = 0; i <= length + 5; i++)//将要显示的数据存放到数组datashow中
                    datashow[i] = data[i];
                string stringdata = BitConverter.ToString(datashow);//把数组转换成16进制字符串
                return stringdata;
            }
        }
    }
}
