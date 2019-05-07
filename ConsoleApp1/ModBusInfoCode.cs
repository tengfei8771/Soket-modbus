using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class ModBusInfoCode
    {
       /// <summary>
       /// ModBus请求由MBAP报文头+功能码+起始地址Hi，起始地址Lo，输出地址Hi+输出地址Lo组成
       /// </summary>
        #region modbus MBAP报文头 事务元标识符2byte，协议标识符2byte，长度为数据长度+1byte，单元标识符1byte。
        public static readonly byte[] TransactionFlag =new byte[2] { 0x00, 0x01 };//事务元标识符，现使用0x00和0x01表示，可以用标识符解决数据传输混乱问题
        public static readonly byte[] AgreementFlag = new byte[2] { 0x00, 0x00 };//协议标识符。ModBus为0x00,0x00
        public static byte RequestLength;//数据长度（包含单元标识符）
        public const byte UnitFlag = 0x01;//单元标识符。
        #endregion

        #region 以下向modbus发送请求的报文头的功能码，它代表了发送请求的目的，返回的回应报文代码一定和发送的一致
        public const byte ReadCoil = 0x01;//读取线圈，获取逻辑线圈的状态
        public const byte ReadInput = 0x02;//读取输入状态，获取开关的状态
        public const byte ReadRegister = 0x03;//读取保持寄存器的二进制数值
        public const byte ReadInputRegister = 0x04;//读取输入寄存器的二进制数值
        public const byte ForceCoil = 0x05;//强制一个逻辑线圈的通断状态
        public const byte WriteRegister = 0x06;//向寄存器内写入二进制数值
        public const byte ReadError = 0x07;//读取ModBus的异常状态
        public const byte ReadDiagnosticState = 0x08;//读取ModBus的诊断状态
        public const byte Programming484 = 0x09;//484专用编程码
        public const byte Query484 = 0x10;//484专用轮询代码
        public const byte ReadEventCount = 0x11;//读取事件发生数
        public const byte ReadMsgCount = 0x12;//读取通讯事件发生数,
        public const byte Programming = 0x13;//184/384/484/584专用编程码
        public const byte Query = 0x14;//184/384/484/584专用查询码
        public const byte ForceCoilList = 0x15;//强制中断一串线圈的状态
        public const byte WriteRegisterList = 0x0F;//像一连串寄存器内输入二进制数值
        #endregion

        public bool IsZero = false;//代表起始位是否从0开始，默认false
    }
}
