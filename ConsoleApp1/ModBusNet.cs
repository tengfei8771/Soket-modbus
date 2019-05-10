using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleApp1
{
    public class ModBusNet
    {
        public Socket client;
        public bool IsConneted { get; set; }
        private string IP { get; set; }
        private int port { get; set; }
        public string Msg { get; set; }
        public bool IsZero = false;//起始位置是否为0，默认为1起始
        public ModBusNet(string IP,int port)
        {
            this.IP = IP;
            this.port = port;
            Connected();
        }
        /// <summary>
        /// 初始化连接ModBus方法
        /// </summary>
        public void Connected()
        {
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IP.Trim()), port);
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
        }

        private  void AlwaysSend(string Adress, int CoinNumber = 1)//原参数object source, System.Timers.ElapsedEventArgs e
        {
            //byte[] data = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0x01, 0x00, 0x14, 0x00, 0x13 };
            byte[] data = ReadCoil(Adress, CoinNumber);
            client.Send(data);
        }

        public string ReceiveMsg()
        {
            while (true)
            {
                //List<int> list = new List<int>();
                byte[] data = new byte[1024];//定义数据接收数组
                client.Receive(data);//接收数据到data数组
                int length = data[5];//读取数据长度
                byte[] datashow = new byte[length + 6];//定义所要显示的接收的数据的长度
                for (int i = 0; i <= length + 5; i++)//将要显示的数据存放到数组datashow中
                {
                    datashow[i] = data[i];
                }
                AnalyisMsg(datashow);
                string stringdata = BitConverter.ToString(datashow);//把数组转换成16进制字符串
                return stringdata;
            }
        }
        public void GetTestMsg(string Adress, int CoinNumber = 1)
        {
            AlwaysSend(Adress,CoinNumber);
            Msg = ReceiveMsg();
        }
        
        /// <summary>
        /// 报文头打包指令（前七位）
        /// </summary>
        /// <param name="address"></param>
        /// <param name="CoinNumber"></param>
        /// <returns></returns>
        public byte[] PackHeader(int CommandLength)
        {
            byte[] Header = new byte[7];
            Header[0] = ModBusInfoCode.TransactionFlag[0];//事务元标识符
            Header[1] = ModBusInfoCode.TransactionFlag[1];//事务元标识符
            Header[2] = ModBusInfoCode.AgreementFlag[0];//协议标识符
            Header[3] = ModBusInfoCode.AgreementFlag[1];//协议标识符
            Header[4] = 0x00;
            Header[5] = BinaryHelper.TenToSixteen(CommandLength + 1)[0];//计算命令长度
            Header[6] = ModBusInfoCode.UnitFlag;
            return Header;

        }


        /// <summary>
        /// 打包真实指令(后五位字节)
        /// </summary>
        /// <param name="adress">设备地址</param>
        /// <param name="CoinNumber">线圈数量</param>
        /// <returns></returns>
        public byte[] PackCommand(int adress,int Flag, int CoinNumber = 1)
        {
            byte[] Command = new byte[5];
            byte[] add = BinaryHelper.TenToSixteen(adress);
            byte[] num = BinaryHelper.TenToSixteen(CoinNumber);
            switch (Flag)//为不同的操作设置功能码
            {
                case 1:
                    Command[0] = ModBusInfoCode.ReadCoil;
                    break;
                case 2:
                    Command[0] = ModBusInfoCode.ReadInput;
                    break;
                case 3:
                    Command[0] = ModBusInfoCode.ReadInputRegister;
                    break;
                case 5:
                    Command[0] = ModBusInfoCode.ForceCoil;
                    break;
                default:
                    throw new Exception("未知操作！");

            }
           
            if (add.Length > 1)
            {
                Command[1] = add[0];
                Command[2] = add[1];
            }
            else
            {
                Command[1] = 0x00;
                Command[2] = add[0];
            }
            if (num.Length > 1)
            {
                Command[3] = num[0];
                Command[4] = num[1];
            }
            else
            {
                Command[3] = 0x00;
                Command[4] = num[0];
            }
            return Command;
            
        }
        /// <summary>
        /// 写入多个线圈打包方法
        /// </summary>
        /// <param name="Adress">地址</param>
        /// <param name="Number">数量</param>
        /// <param name="Value">值</param>
        /// <param name="Flag">标示位</param>
        /// <returns></returns>
        public byte[] PackCommand1(int Adress, int Number, int Value,int Flag)
        {
            byte[] Add = BinaryHelper.TenToSixteen(Adress);
            byte[] Num = BinaryHelper.TenToSixteen(Number);
            byte[] InputValue = BinaryHelper.TenToSixteen(Value);
            byte[] Command = new byte[6 + InputValue.Length];
            switch (Flag)
            {
                case 15:
                    Command[0] = 0x0F;
                    break;
                default:
                    break;
            }
            if (Add.Length > 1)
            {
                Command[1] = Add[0];
                Command[2] = Add[1];
            }
            else
            {
                Command[2] = Add[0];
            }
            if (Num.Length > 1)
            {
                Command[3] = Num[0];
                Command[4] = Num[1];
            }
            else
            {
                Command[4] = Num[0];
            }
            Command[5] = BinaryHelper.TenToSixteen(InputValue.Length)[0];
            for(int i = 6; i < Command.Length; i++)
            {
                Command[i] = InputValue[i - 6];
            }
            return Command;
        }
        /// <summary>
        /// 写入多个寄存器值
        /// </summary>
        /// <param name="Adress">起始地址</param>
        /// <param name="Number">寄存器数量</param>
        /// <param name="Value">输入值</param>
        /// <param name="Flag">标志位</param>
        /// <returns></returns>
        public byte[] PackCommand2(int Adress,int[] Value, int Flag)
        {
            byte[] Add = BinaryHelper.TenToSixteen(Adress);
            byte[] Num = BinaryHelper.TenToSixteen(Value.Length);
            byte[] Command = new byte[6 + Value.Length*2];
            switch (Flag)
            {
                case 16:
                    Command[0] = 0x10;
                    break;
                default:
                    break;
            }
            if (Add.Length > 1)
            {
                Command[1] = Add[0];
                Command[2] = Add[1];
            }
            else
            {
                Command[2] = Add[0];
            }
            if (Num.Length > 1)
            {
                Command[3] = Num[0];
                Command[4] = Num[1];
            }
            else
            {
                Command[4] = Num[0];
            }
            Command[5] = BinaryHelper.TenToSixteen(Value.Length*2)[0];
            for (int i = 6; i < Command.Length; i++)
            {
                byte[] bt = BinaryHelper.TenToSixteen(Value[i - 6]);
                if (bt.Length > 1)
                {
                    Command[i] = bt[0];
                    Command[i + 1] = bt[1];
                }
                else
                {
                    Command[i + 1] = bt[0];
                }
            }
            return Command;
        }
        /// <summary>
        /// 读取线圈方法
        /// </summary>
        /// <param name="Adress">地址</param>
        /// <param name="CoinNumber">读取数量，默认1</param>
        /// <returns></returns>
        public byte[] ReadCoil(string Adress, int CoinNumber = 1)
        {
            int Flag = 1;//操作标志位，用于打包指令判断
            byte[] command = PackCommand(Convert.ToInt32(Adress), Flag, CoinNumber);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7+length];
            for (int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }
            
            for (int i = 7; i < request.Length; i++)
            {
                request[i] = command[i - 7];
            }
            return request;
        }


        /// <summary>
        /// 读取离散输入方法
        /// </summary>
        /// <param name="Adress">地址</param>
        /// <param name="Number">读取数量</param>
        /// <returns></returns>

        public byte[] ReadDiscrete(string Adress,int Number)
        {
            int Flag = 2;//功能标志位
            byte[] command = PackCommand(Convert.ToInt32(Adress), Flag, Number);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7 + length];
            for (int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }

            for (int i = 7; i < request.Length; i++)
            {
                request[i] = command[i - 7];
            }
            return request;
        }
        /// <summary>
        /// 读取寄存器数据，
        /// </summary>
        /// <param name="Adress">起始地址</param>
        /// <param name="Number">读取寄存器数量</param>
        /// <returns></returns>
        public byte[] ReadRegister(string Adress, int Number)
        {
            int Flag = 3;//功能标志位
            byte[] command = PackCommand(Convert.ToInt32(Adress), Flag, Number);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7 + length];
            for (int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }

            for (int i = 7; i < request.Length; i++)
            {
                request[i] = command[i - 7];
            }
            return request;

        }
        /// <summary>
        /// 向线圈内写入数值，类似于OFF,ON
        /// </summary>
        /// <param name="Adress">地址</param>
        /// <param name="value">写入值最大值0xFF00</param>
        /// <returns></returns>

        public byte[] ForceCoil(string Adress,int value)
        {
            int Flag = 5;//功能标志位
            byte[] command = PackCommand(Convert.ToInt32(Adress), Flag, value);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7 + length];
            for (int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }

            for (int i = 7; i < request.Length; i++)
            {
                request[i] = command[i - 7];
            }
            return request;
        }
        /// <summary>
        /// 向多个寄存器内写入数据
        /// </summary>
        /// <param name="Adress">地址</param>
        /// <param name="value">数量</param>
        /// <returns></returns>
        public byte[] WriteRegister(string Adress, int value)
        {
            int Flag = 6;//功能标志位
            byte[] command = PackCommand(Convert.ToInt32(Adress), Flag, value);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7 + length];
            for (int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }

            for (int i = 7; i < request.Length; i++)
            {
                request[i] = command[i - 7];
            }
            return request;
        }


        public byte[] ForceCoilList(string Adress,int Number,int Value)
        {
            int Flag = 15;
            byte[] command = PackCommand1(Convert.ToInt32(Adress), Number, Value, Flag);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7 + length];
            for(int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }
            for(int i = bt.Length; i < request.Length; i++)
            {
                request[i] = command[i - bt.Length];
            }
            return request;

        }

        public byte[] WriteRegisterList(string Adress,int[] Value)
        {
            int Flag = 16;
            byte[] command = PackCommand2(Convert.ToInt32(Adress), Value, Flag);
            int length = command.Length;
            byte[] bt = PackHeader(length);
            byte[] request = new byte[7 + length];
            for (int i = 0; i < bt.Length; i++)
            {
                request[i] = bt[i];
            }
            for (int i = bt.Length; i < request.Length; i++)
            {
                request[i] = command[i - bt.Length];
            }
            return request;

        }
        /// <summary>
        /// 解析收到的数据
        /// </summary>
        /// <param name="bt"></param>
        public byte[] AnalyisMsg(byte[] bt)
        {
            byte FunctionCode = bt[7];//回应报文第八位为功能码，需要判断功能码对其报文进行解析
            byte[] data = new byte[1024];
            switch (FunctionCode)
            {
                case 0x01:
                    data= ReadDataByte(bt);
                    break;
                case 0x02:
                    data = ReadDataByte(bt);
                    break;
                case 0x03:
                    data = ReadDataByte(bt);
                    break;
                case 0x05:
                    data = WriteDateByte(bt);
                    break;
                case 0x06:
                    data = WriteDateByte(bt);
                    break;
                default:
                    break;

            }
            return data;
        }
        /// <summary>
        /// 获取返回数据字节位
        /// </summary>
        /// <param name="bt"></param>
        /// <returns></returns>
        public byte[] ReadDataByte (byte[] bt)
        {
            int DataLength = bt.Length - 9;//获取有几个byte长度
            byte[] DataArr = new byte[DataLength];
            for(int i = 9; i < bt.Length; i++)
            {
                DataArr[i - 9] = bt[i];
            }
            return DataArr;
        }

        public byte[] WriteDateByte(byte[] bt)
        {
            int DataLength = bt.Length - 10;
            byte[] DataArr = new byte[DataLength];
            for(int i = 10; i < bt.Length; i++)
            {
                DataArr[i - 10] = bt[i];
            }
            return DataArr;
        }

    }
}
