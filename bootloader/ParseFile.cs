using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Windows.Forms;

namespace UDS上位机
{
    public class DataLine
    {
        public int address;
        public string data;
        public int count;   //每行数据的个数，偏移量用于计算下一个地址   
    }
    //对应一个block要发送的数据
    public class DataBlock
    {
        public int address;
        public List<byte> data;
        public DataBlock()
        {
            data = new List<byte>();
        }
    }
    public class BlockSequence
    {
        public int sequenceNumber;
        public byte[] data;
    }

    //class ParseBinFile
    //{
    //    public bool parseBin(string filename, ref List<DataBlock> BinBlockList)
    //    {
    //        FileStream BinFile = new FileStream(filename, FileMode.Open, FileAccess.Read);
    //        BinaryReader BinFileReader = new BinaryReader(BinFile);
    //        DataBlock BinDataBlock = new DataBlock();
    //        BinDataBlock.address = (int)MainUI.uiBTPara.BinFileStartAddress;
    //        try
    //        {
    //            //获取Bin文件数据到BinBlockList
    //            for (long k = 0; k < BinFile.Length; k++)
    //            {
    //                BinDataBlock.data.Add(BinFileReader.ReadByte());
    //            }
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //        BinBlockList.Add(BinDataBlock);
    //        return true;
    //    }
    //}
    class ParseS19File
    {
        List<DataLine> s19;
        public ParseS19File()
        {
            //S1数据.地址<>数据，键值对，保存从s19文件解析的每一行
            s19 = new List<DataLine>();

        }
        //解析S19文件
        //参数：filename：S19文件名  s19BlockList：解析完成的结果放入此s19BlockList
        public bool parseS19(string filename, ref List<DataBlock> s19BlockList)
        {
            //s19BlockList.Clear();
            s19.Clear();
            string sLine;
            StreamReader file = new StreamReader(filename);
            try
            {
                while ((sLine = file.ReadLine()) != null)
                {
                    if (sLine.StartsWith("S0"))
                    {
                        //s0不解析，不写入flash
                    }
                    else if (sLine.StartsWith("S1"))
                    {
                        //去掉S1标识头
                        string strNoS1 = sLine.Substring(2, sLine.Length - 2);
                        //取地址
                        int address = Int32.Parse(strNoS1.Substring(2, 4), System.Globalization.NumberStyles.HexNumber);
                        //取加法校验
                        int checkNum = Int32.Parse(strNoS1.Substring(strNoS1.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);
                        //取数据,去掉最后的2位校验
                        string data = strNoS1.Substring(0, strNoS1.Length - 2);
                        //校验和
                        if (checkSum(data, checkNum))
                        {
                            //add
                            DataLine s19Data = new DataLine();
                            s19Data.address = address;
                            //纯数据，6 = 总个数4 + 地址4。8 =  总个数4 + 地址4 + 校验2
                            s19Data.data = strNoS1.Substring(6, strNoS1.Length - 8);
                            //纯数据长度，去掉地址2位，校验1位 = 3
                            s19Data.count = Int32.Parse(strNoS1.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) - 3;
                            s19.Add(s19Data);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("s19文件校验失败，地址:{0}！", address), "测试", MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else if (sLine.StartsWith("S2"))
                    {
                        //去掉S2标识头
                        string strNoS2 = sLine.Substring(2, sLine.Length - 2);
                        //取地址
                        int address = Int32.Parse(strNoS2.Substring(2, 6), System.Globalization.NumberStyles.HexNumber);
                        //取加法校验
                        int checkNum = Int32.Parse(strNoS2.Substring(strNoS2.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);
                        //取数据,去掉最后的2位校验
                        string data = strNoS2.Substring(0, strNoS2.Length - 2);
                        //校验和
                        if (checkSum(data, checkNum))
                        {
                            //add
                            DataLine s19Data = new DataLine();
                            s19Data.address = address;
                            //纯数据，8 = 总个数2 + 地址6。10 =  总个数2 + 地址6 + 校验2
                            s19Data.data = strNoS2.Substring(8, strNoS2.Length - 10);
                            //纯数据长度，去掉地址3位，校验1位 = 4
                            s19Data.count = Int32.Parse(strNoS2.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) - 4;
                            s19.Add(s19Data);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("s19文件校验失败，地址:{0}！", address), "测试测试", MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else if (sLine.StartsWith("S3"))
                    {
                        //去掉S3标识头
                        string strNoS3 = sLine.Substring(2, sLine.Length - 2);
                        //取地址
                        int address = Int32.Parse(strNoS3.Substring(2, 8), System.Globalization.NumberStyles.HexNumber);
                        //取加法校验
                        int checkNum = Int32.Parse(strNoS3.Substring(strNoS3.Length - 2, 2), System.Globalization.NumberStyles.HexNumber);
                        //取数据,去掉最后的2位校验
                        string data = strNoS3.Substring(0, strNoS3.Length - 2);
                        //校验和
                        if (checkSum(data, checkNum))
                        {
                            //add
                            DataLine s19Data = new DataLine();
                            s19Data.address = address;
                            //纯数据，10 = 总个数2 + 地址8。12 =  总个数2 + 地址8 + 校验2
                            s19Data.data = strNoS3.Substring(10, strNoS3.Length - 12);
                            //纯数据长度，去掉地址4位，校验1位 = 5
                            s19Data.count = Int32.Parse(strNoS3.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) - 5;
                            s19.Add(s19Data);
                        }
                        else
                        {
                            MessageBox.Show(String.Format("s19文件校验失败，地址:{0}！", address), "测试", MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Warning);
                            return false;
                        }
                    }
                    else if (sLine.StartsWith("S8"))
                    {
                        //，不写入flash
                    }
                    else if (sLine.StartsWith("S9"))
                    {
                        //，不写入flash
                    }
                }

                //对S19排序，升序
                s19.Sort(compareTo);//按地址进行升序
                                    //把文件中每一行的数据组合成若干个地址开头的整块数据,放在s19Block里用于发送
                                    //如果文件中地址不连续，则分成多个块
                int nextAddress = 0xFFFF;
                DataBlock s19Block = null;
                for (int i = 0; i < s19.Count; i++)
                {
                    if (i == 0)
                    {
                        s19Block = new DataBlock();
                        nextAddress = getNextAddress(s19Block, s19, i);
                        if (i == s19.Count - 1)//数据量很少，只用一块s19Block
                        {
                            //已经循环到底，只有一个block
                            s19BlockList.Add(s19Block);
                        }
                    }
                    else
                    {
                        if (nextAddress == s19[i].address)//计算地址=读取地址，表示数据是连续的
                        {
                            joinS19BlockData(s19Block.data, s19[i].data);//继续给Block增加数据
                            nextAddress = s19[i].address + s19[i].count;
                            if (i == s19.Count - 1)
                            {
                                //已经循环到底，只有一个block
                                s19BlockList.Add(s19Block);
                            }
                        }
                        else//计算地址!=读取地址，表示数据是非连续，需要将数据放入新的块
                        {
                            s19BlockList.Add(s19Block);//将前面更新Block的加入BlockList
                            s19Block = new DataBlock();
                            nextAddress = getNextAddress(s19Block, s19, i);
                            if (i == s19.Count - 1)
                            {
                                //已经循环到底，只有一个block
                                s19BlockList.Add(s19Block);
                            }
                        }
                    }
                }
                file.Close();
            }

            catch (Exception ex)
            {
                //Console.WriteLine(ex);
                return false;
            }
            return true;
        }
        private int compareTo(DataLine x, DataLine y)
        {
            return x.address.CompareTo(y.address);
        }

        private int getNextAddress(DataBlock s19Block, List<DataLine> s19, int i)
        {
            s19Block.address = s19[i].address;
            joinS19BlockData(s19Block.data, s19[i].data);
            return s19[i].address + s19[i].count;
        }

        private void joinS19BlockData(List<byte> list, string dataLine)
        {
            for (int i = 0; i < dataLine.Length; i = i + 2)
            {
                byte value = byte.Parse(dataLine.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                list.Add(value);
            }
        }
        private bool checkSum(string data, int checkNum)
        {
            //校验
            int sum = 0;
            for (int i = 0; i < data.Length; i = i + 2)
            {
                byte value = byte.Parse(data.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                sum += value;
            }

            //取低字节，1位
            byte lowSum = (byte)sum;
            if ((0xFF - lowSum) == checkNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    class ParseHexFile
    {
        List<DataLine> Hex;
        public const byte DataRecord = 0x00;                                    //  用来记录数据，HEX文件的大部分记录都是数据记录
        public const byte EndOfFileRecord = 0x01;                               //  用来标识文件结束，放在文件的最后，标识HEX文件的结尾
        public const byte ExtendedSegmentAddressRecord = 0x02;                  //  用来标识扩展段地址的记录
        public const byte StartSegmentAddressRecord = 0x03;                     //  开始段地址记录
        public const byte ExtendedLinearAddressRecord = 0x04;                   //  用来标识扩展线性地址的记录
        public const byte StartLinearAddressRecord = 0x05;                      //  开始线性地址记录
        public const byte Unknown = 0x06;
        private int BaseAddress = 0x0;
        public ParseHexFile()
        {
            Hex = new List<DataLine>();
        }
        public bool ParseHexfile(string filename, ref List<DataBlock> HexBlockList)
        {
            try
            {
                StreamReader read = new StreamReader(filename);
                string str = "";
                int ncount = 0;
                //检查hex文件错误
                do
                {
                    str = read.ReadLine();
                    ncount++;
                    if (str == null)
                        continue;
                    else
                    {
                        if (!CheckCorrectness(str, ncount))
                        {
                            //Console.Write("Hex文件出错\r\n");
                            return false;
                        }
                    }
                } while (!read.EndOfStream);
                //while (str != null);

                read.BaseStream.Position = 0;
                //提取hex文件每行的数据
                do
                {
                    str = read.ReadLine();
                    //ncount++;
                    if (str == null)
                        continue;
                    else
                    {
                        switch (Convert.ToByte(str.Substring(7, 2), 16))
                        {
                            //DataLine
                            case DataRecord://00
                                DataLine HexData = new DataLine();
                                HexData.address = BaseAddress + Convert.ToInt32(str.Substring(3, 4), 16);
                                HexData.data = str.Substring(9, Convert.ToInt32(str.Substring(1, 2), 16) * 2);
                                HexData.count = Convert.ToInt32(str.Substring(1, 2), 16);
                                Hex.Add(HexData);
                                break;
                            case EndOfFileRecord://01
                                break;
                            case ExtendedSegmentAddressRecord://02
                                BaseAddress = Convert.ToInt32(str.Substring(9, 4), 16) << 4;
                                break;
                            case StartSegmentAddressRecord://03
                                //BaseAddress = Convert.ToInt32(str.Substring(9, 4), 16) << 4;
                                break;
                            case StartLinearAddressRecord://05
                                break;
                            case ExtendedLinearAddressRecord://04
                                BaseAddress = Convert.ToInt32(str.Substring(9, 4), 16) << 16;
                                break;
                            default:
                                break;
                        }
                    }
                } while (!read.EndOfStream);
                //将每行的数据打包成逻辑块
                int nextAddress = 0xFFFFFFF;
                DataBlock HexBlock = null;
                for (int i = 0; i < Hex.Count; i++)
                {
                    if (i == 0)
                    {
                        HexBlock = new DataBlock();
                        nextAddress = getNextAddress(HexBlock, Hex, i);
                        if (i == Hex.Count - 1)//数据量很少，只用一块s19Block
                        {
                            //已经循环到底，只有一个block
                            HexBlockList.Add(HexBlock);
                        }
                    }
                    else
                    {
                        if (nextAddress == Hex[i].address)//计算地址=读取地址，表示数据是连续的
                        {
                            joinHexBlockData(HexBlock.data, Hex[i].data);//继续给Block增加数据
                            nextAddress = Hex[i].address + Hex[i].count;
                            if (i == Hex.Count - 1)
                            {
                                //已经循环到底，只有一个block
                                HexBlockList.Add(HexBlock);
                            }
                        }
                        else//计算地址!=读取地址，表示数据是非连续，需要将数据放入新的块
                        {
                            HexBlockList.Add(HexBlock);//将前面更新Block的加入BlockList
                            HexBlock = new DataBlock();
                            nextAddress = getNextAddress(HexBlock, Hex, i);
                            if (i == Hex.Count - 1)
                            {
                                //已经循环到底，只有一个block
                                HexBlockList.Add(HexBlock);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Console.Write(ex);
                return false;
            }
            return true;
        }
        private int getNextAddress(DataBlock HexBlock, List<DataLine> Hex, int i)
        {
            HexBlock.address = Hex[i].address;
            joinHexBlockData(HexBlock.data, Hex[i].data);
            return Hex[i].address + Hex[i].count;
        }

        private void joinHexBlockData(List<byte> list, string dataLine)
        {
            for (int i = 0; i < dataLine.Length; i = i + 2)
            {
                byte value = byte.Parse(dataLine.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);
                list.Add(value);
            }
        }
        private static bool CheckCorrectness(string strText, int ncount)
        {
            try
            {
                //检查起始引号
                if (strText.Substring(0, 1) != ":")
                {
                    MessageBox.Show(String.Format("第" + ncount.ToString() + "行起始非引号"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);
                    return false;
                }
                //检查行长度与格式
                int datalength = Convert.ToByte(strText.Substring(1, 2), 16);
                if (datalength * 2 != (strText.Length - 1 - 2 - 4 - 2 - 2))
                {
                    MessageBox.Show(String.Format("第" + ncount.ToString() + "行长度与格式不匹配"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);
                    return false;
                }
                //检查CRC
                UInt32 u32 = 0;
                for (int i = 0; i < (strText.Length - 1) / 2; i++)
                {
                    u32 += Convert.ToByte(strText.Substring(1 + 2 * i, 2), 16);
                }
                if (u32 % 0x100 != 0)
                {
                    MessageBox.Show(String.Format("第" + ncount.ToString() + "行校验错误"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);
                    return false;
                }
                byte datatype = Convert.ToByte(strText.Substring(7, 2), 16);

                switch (datatype)
                {
                    case DataRecord:
                        return true;
                    case EndOfFileRecord:
                        if (strText != ":00000001FF")
                        {
                            MessageBox.Show(String.Format("第" + ncount.ToString() + "行不是正确文件结束记录"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);
                            return false;
                        }
                        else return true;
                    case ExtendedLinearAddressRecord:
                        if (datalength != 2)
                        {
                            MessageBox.Show(String.Format("第" + ncount.ToString() + "行是扩展段地址记录(02),但长度不是2"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);

                            return false;
                        }
                        return true;
                    case ExtendedSegmentAddressRecord:
                        if (datalength != 2)
                        {
                            MessageBox.Show(String.Format("第" + ncount.ToString() + "行是扩展段地址记录(04),但长度不是2"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);

                            return false;
                        }
                        return true;
                    case StartSegmentAddressRecord:
                        //HEX.strFault = "第" + ncount.ToString() + "行是开始段地址记录(03)";
                        //return false;
                        return true;
                    case StartLinearAddressRecord:
                        //HEX.strFault = "第" + ncount.ToString() + "行是开始线性地址记录(05)";
                        //return false;
                        return true;
                    default:
                        MessageBox.Show(String.Format("第" + ncount.ToString() + "行是未知无效记录"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);
                        return false;
                }
            }
            catch
            {
                MessageBox.Show(String.Format("第" + ncount.ToString() + "行格式错误"), "解析Hex失败", MessageBoxButtons.YesNoCancel,
                                            MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}
