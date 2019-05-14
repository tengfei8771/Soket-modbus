using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class BinaryHelper
    {
        public static byte[] TenToSixteen(int value)
        {
            List<int> NumList = new List<int>() { 10, 11, 12, 13, 14, 15 };
            List<string> StrList = new List<string>() { "A", "B", "C", "D", "E", "F" };
            string TotalStr = string.Empty;//将短除所得的数字转为str保存
            int Temp;//余数
            int index;//数组索引
            while (value > 15)
            {
                Temp = value % 16;
                if (Temp >= 10)
                {
                    index = NumList.FindIndex(t => t.Equals(Temp));//查询定义数字列表中数字的索引
                    TotalStr += StrList[index];
                }
                else
                {
                    TotalStr += Temp.ToString();
                }
                value = (value - Temp) / 16;
            }
            if (value >= 10)
            {
                index = NumList.FindIndex(t => t.Equals(value));//查询定义数字列表中数字的索引
                TotalStr += StrList[index];
            }
            else
            {
                TotalStr += value.ToString();
            }
            //调用下面的方法之前，totalstr是反的
            TotalStr = ReverseStr(TotalStr);
            byte[] res = strToToHexByte(TotalStr);
            return res;

        }

        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="str"></param>
        public static string ReverseStr(string str)
        {
            string NewStr = "";
            for (int i = str.Length - 1; i >= 0; i--)
            {
                NewStr += str[i];
            }
            return NewStr;
        }

        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
            {
                hexString = hexString.Insert(0, 0.ToString());//转出来的字符串不是偶数在最前面补0
            }               
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

    }
}
