using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using UDS上位机.Driver_.USBCAN;
namespace UDS上位机
{
    public partial class Form2 : Form
    {
        //进度条
        private static double deta = 0;
        public static long TotalData = 0;
        public void BTprogressBarInit()
        {
            this.progressBar1.Value = 0;
        }
        //叠加到1才加
        public void BTprogressBarAdd(double value)
        {
            int process = 0;
            deta += value;
            if (deta >= 1.0)
            {
                process = (int)deta;
                deta = 0;
            }
            if (this.progressBar1.Value + process < 100)
                this.progressBar1.Value += process;
            else
                this.progressBar1.Value = 100;
        }
        public void BTprogressBarAdd(int value)
        {
            if (this.progressBar1.Value + value < 100)
                this.progressBar1.Value += value;
            else
                this.progressBar1.Value = 100;
        }
        public void BTprogressBarClear()
        {
            this.progressBar1.Value = 0;
        }
    };
    class Bootloader
    {
        ParseS19File S19FileParser;//S19文件解析器
        ParseHexFile HexFileParser;//Hex文件解析器
        //ParseBinFile BinFileParser;//Bin文件解析器
        public List<DataBlock> s19APP1BlockList;//S19APP1数据块列表
        public List<DataBlock> s19APP2BlockList;//S19APP2数据块列表
        public List<DataBlock> s19DriBlockList;//S19Dri数据块列表
        public List<DataBlock> HexAPP1BlockList;//HexAPP1数据块列表
        public List<DataBlock> HexAPP2BlockList;//HexAPP2数据块列表
        public List<DataBlock> HexDriBlockList;//HexDri数据块列表
        public List<DataBlock> BinBlockList;//Bin数据块列表
        public ECUConfigInfo SelectECUConfig = new ECUConfigInfo();//选择的ECU
        public Thread BTThread;//BT刷写线程
        public int APP1FileType = 0;//APP1文件类型，0：s19 1：Hex 2：Bin
        public int APP2FileType = 0;//APP2文件类型，0：s19 1：Hex 2：Bin
        public int DriFileType = 0;//Dri文件类型，0：s19 1：Hex 2：Bin

        public Bootloader()
        {
            s19APP1BlockList = new List<DataBlock>();//S19APP1数据块列表
            s19APP2BlockList = new List<DataBlock>();//S19APP2数据块列表
            s19DriBlockList = new List<DataBlock>();//S19Dri数据块列表
            HexAPP1BlockList = new List<DataBlock>();//S19APP1数据块列表
            HexAPP2BlockList = new List<DataBlock>();//S19APP2数据块列表
            HexDriBlockList = new List<DataBlock>();//S19Dri数据块列表
            BinBlockList = new List<DataBlock>();//Bin数据块列表
            BTThread = new Thread(new ThreadStart(BTProcess));
        }
        //BT执行流程
        public void BTProcess()
        {
            Program.form2.BTprogressBarInit();
            //Program.form2.timer100ms.Enabled = true;
            //3.1 预编程
            if (PreProgramming() == 0)
            {
                //Console.Write("STEP1:预编程失败\r\n");
                ////Log.WriteTo//Log("//STEP1:预编程失败\r\n");
                Program.form2.textBox3.Text += string.Format("STEP1:预编程失败\r\n");
                Form2.Timer_3E80.Enabled = false;//停止3E80服务
                                                 //清除显示
                MessageBox.Show("刷写失败", "失败",
                                   MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Program.form2.textBox3.Clear();
                Program.form2.BTprogressBarClear();
                // Program.form2.EableDisableStartBT(true);
                return;
            }
            //Console.Write("STEP1:预编程成功\r\n");
            ////Log.WriteTo//Log("//STEP1:预编程成功\r\n");
            Program.form2.textBox3.Text += string.Format("STEP1:预编程成功\r\n");
            Program.form2.BTprogressBarAdd(5);
            //3.2主编程

            if (MainProgramming() == 0)
            {
                //Console.Write("STEP2:主编程失败\r\n");
                ////Log.WriteTo//Log("//STEP2:主编程失败\r\n");
                Program.form2.textBox3.Text += string.Format("STEP2:主编程失败\r\n");
                Form2.Timer_3E80.Enabled = false;//停止3E80服务
                MessageBox.Show("刷写失败", "失败",
                                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Program.form2.textBox3.Clear();
                Program.form2.BTprogressBarClear();
                //Program.form2.EableDisableStartBT(true);
                return;
            }

            //Console.Write("STEP2:主编程成功\r\n");
            //Log.WriteTo//Log("//STEP2:主编程成功\r\n");
            Program.form2.textBox3.Text += string.Format("STEP2:主编程成功\r\n");
            //3.3后编程
            if (PostProgram() == 0)
            {
                //Console.Write("STEP3:后编程失败\r\n");
                //Log.WriteTo//Log("//STEP3:后编程失败\r\n");
                Program.form2.textBox3.Text += string.Format("STEP3:后编程失败\r\n");
                Form2.Timer_3E80.Enabled = false;//停止3E80服务
                MessageBox.Show("刷写失败", "失败",
                                  MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Program.form2.textBox3.Clear();
                Program.form2.BTprogressBarClear();
                //Program.form2.EableDisableStartBT(true);
                return;
            }
            //Console.Write("STEP3:后编程成功\r\n");
            //Log.WriteTo//Log("//STEP3:后编程成功\r\n");
            Program.form2.textBox3.Text += string.Format("STEP3:后编程成功\r\n");
            Program.form2.BTprogressBarAdd(100);//进度条充满
            Form2.Timer_3E80.Enabled = false;//停止3E80服务
            MessageBox.Show("刷写成功", "成功",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //Program.form2.timer100ms.Enabled = false;
            // Program.form2.EableDisableStartBT(true);
            //this.BTThread.Abort();//终止线程

        }
        public int ParseS19file(string filename, ref List<DataBlock> s19BlockList)
        {
            if (filename == null)
            {
                return 0;
            }
            S19FileParser = new ParseS19File();
            if (S19FileParser.parseS19(filename, ref s19BlockList) == false)
                return 0;

            return 1;
        }
        //解析Hex文件
        public int ParseHexFile(string filename, ref List<DataBlock> HexBlockList)
        {
            if (filename == null)
            {
                return 0;
            }
            HexFileParser = new ParseHexFile();
            if (HexFileParser.ParseHexfile(filename, ref HexBlockList) == false)
                return 0;

            return 1;
        }
        public int LoadECUConfig()
        {
            if (SelectECUConfig.ECUName == null)
            {
                return 0;
            }
            return 1;
        }
        //预编程，需要配置流程，可以联系作者帮忙修改
        //失败：返回0，成功，返回1
        private int PreProgramming()
        {

            return 1;
        }
        //主编程,需要配置流程，可以联系作者帮忙修改
        //失败：返回0，成功，返回1
        private int MainProgramming()
        {


            return 1;
        }
        //后编程，需要配置流程，可以联系作者帮忙修改
        private int PostProgram()
        {

            return 1;
        }
        //ECU复位
        unsafe private int ECUReset(ref ECUConfigInfo SelectECUConfig)
        {
            VCI_CAN_OBJ CANMsg;
            //1.发送11 03
            byte[] data = new byte[3];
            data[0] = 0x02;
            data[1] = 0x11;
            data[2] = 0x01;
            ////Console.Write("SelectECUConfig.FuncID:{0:x}\r\n", SelectECUConfig.FuncID);
            CANAPI.Output(SelectECUConfig.RequestID, ref data, (uint)data.Length, SelectECUConfig);
            CANAPI.WaitForTimeOut(500);
            //3.接受应答
            CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
            //CANAPI.WaitForTimeOut(40);
            //处理响应
            if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x11) == 0)
            {
                //Console.Write("ECU复位失败!\r\n");
                //Log.WriteTo//Log("ECU复位失败!\r\n");
                Program.form2.textBox3.Text += string.Format("ECU复位失败!\r\n");
                return 0;
            }
            //Console.Write("ECU复位成功!\r\n", CANMsg.ID);
            Program.form2.textBox3.Text += string.Format("ECU复位成功!\r\n", CANMsg.ID);
            CANAPI.WaitForTimeOut(3000);//等待3s
            return 1;
        }

        //检查编程依赖性
        unsafe private int checkProgrammingDependences()
        {
            VCI_CAN_OBJ CANMsg;
            //3.4 检查编程依赖性31h，物理寻址
            //Log.WriteTo//Log("//检查编程依赖性\r\n");
            byte[] data4 = new byte[5];
            data4[0] = 0x04;
            data4[1] = 0x31;
            data4[2] = 0x01;
            data4[3] = 0x00;
            data4[4] = 0x01;

            if (CANAPI.Output(SelectECUConfig.RequestID, ref data4, (uint)data4.Length, SelectECUConfig) == 1)
            {
                CANAPI.WaitForTimeOut(20);
                CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                //处理响应
                if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x31) == 1)
                {
                    if (CANMsg.Data[5] != 0x00)
                    {
                        //Console.Write("31h检查编程依赖性失败\r\n");
                        //Log.WriteTo//Log("31h检查编程依赖性失败\r\n");
                        Program.form2.textBox3.Text += string.Format("31h检查编程依赖性失败\r\n");
                        return 0;
                    }

                }
            }
            else
            {
                //Log.WriteTo//Log("//31h检查编程依赖性报文发送失败\r\n");
                return 0;
            }
            //Console.Write("31h检查编程依赖性成功\r\n");
            //Log.WriteTo//Log("31h检查编程依赖性成功\r\n");
            Program.form2.textBox3.Text += string.Format("31h检查编程依赖性成功\r\n");
            return 1;
        }
        unsafe private int EraseMem(ref List<DataBlock> DataBlockList, int EraseType = 0)
        {
            //Log.WriteTo//Log("//EraseMem Start............\r\n");
            VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
            int BlockNum = 0;
            int EraseLenth = 0;
            //整块擦内存
            if (EraseType == 1)
            {
                for (; BlockNum < DataBlockList.Count; BlockNum++)
                {
                    EraseLenth += DataBlockList[BlockNum].data.Count;
                }
                byte[] data1 = new byte[0xf];
                data1[0] = 0x10;
                data1[1] = 0x0D;
                data1[2] = 0x31;
                data1[3] = 0x01;
                data1[4] = 0x00;
                data1[5] = 0x00;
                data1[6] = 0x44;
                data1[7] = (byte)(DataBlockList[0].address >> 24);//地址MSB
                data1[8] = (byte)(DataBlockList[0].address >> 16);
                data1[9] = (byte)(DataBlockList[0].address >> 8);
                data1[0xA] = (byte)(DataBlockList[0].address);//地址LSB
                data1[0xB] = (byte)(EraseLenth >> 24);//长度MSB
                data1[0xC] = (byte)(EraseLenth >> 16);
                data1[0xD] = (byte)(EraseLenth >> 8);
                data1[0xE] = (byte)(EraseLenth);//长度LSB
                CANAPI.Output(SelectECUConfig.RequestID, ref data1, (uint)data1.Length, SelectECUConfig);
                CANAPI.WaitForTimeOut(5);
                CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                //处理响应
                if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x31) == 0)
                {
                    //Console.Write("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                    //Log.WriteTo//Log(string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]));
                    Program.form2.textBox3.Text += string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                    return 0;
                }
                else if (CANMsg.Data[5] != 0x00)
                {
                    //Console.Write("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                    //Log.WriteTo//Log(string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]));
                    Program.form2.textBox3.Text += string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                    return 0;
                }
                //Log.WriteTo//Log(string.Format("//擦除内存成功：地址：{0:x2}{1:x2}{2:x2}{3:x2}，长度:{4:x2}{5:x2}{6:x2}{7:x2}\r\n", data1[7], data1[8], data1[9], data1[0xA], data1[0xB], data1[0xC], data1[0xD], data1[0xE]));
                //Console.WriteLine("擦除内存成功：地址：{0:x2}{1:x2}{2:x2}{3:x2}，长度:{4:x2}{5:x2}{6:x2}{7:x2}\r\n", data1[7], data1[8], data1[9], data1[0xA], data1[0xB], data1[0xC], data1[0xD], data1[0xE]);
                Program.form2.textBox3.Text += string.Format("擦除内存成功：地址：{0:x2}{1:x2}{2:x2}{3:x2}，长度:{4:x2}{5:x2}{6:x2}{7:x2}\r\n", data1[7], data1[8], data1[9], data1[0xA], data1[0xB], data1[0xC], data1[0xD], data1[0xE]);
            }
            //分段擦内存
            else
            {
                for (; BlockNum < DataBlockList.Count; BlockNum++)
                {
                    byte[] data1 = new byte[0xf];
                    data1[0] = 0x10;
                    data1[1] = 0x0D;
                    data1[2] = 0x31;
                    data1[3] = 0x01;
                    data1[4] = 0x00;
                    data1[5] = 0x00;
                    data1[6] = 0x44;
                    data1[7] = (byte)(DataBlockList[BlockNum].address >> 24);//地址MSB
                    data1[8] = (byte)(DataBlockList[BlockNum].address >> 16);
                    data1[9] = (byte)(DataBlockList[BlockNum].address >> 8);
                    data1[0xA] = (byte)(DataBlockList[BlockNum].address);//地址LSB
                    data1[0xB] = (byte)(DataBlockList[BlockNum].data.Count >> 24);//长度MSB
                    data1[0xC] = (byte)(DataBlockList[BlockNum].data.Count >> 16);
                    data1[0xD] = (byte)(DataBlockList[BlockNum].data.Count >> 8);
                    data1[0xE] = (byte)(DataBlockList[BlockNum].data.Count);//长度MSB
                    CANAPI.Output(SelectECUConfig.RequestID, ref data1, (uint)data1.Length, SelectECUConfig);
                    CANAPI.WaitForTimeOut(10);
                    CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                    //处理响应
                    if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x31) == 0)
                    {
                        //Console.Write("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                        //Log.WriteTo//Log(string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]));
                        Program.form2.textBox3.Text += string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                        return 0;
                    }
                    else if (CANMsg.Data[5] != 0x00)
                    {
                        //Console.Write("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                        //Log.WriteTo//Log(string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]));
                        Program.form2.textBox3.Text += string.Format("31h擦除内存失败：{0:x2}{1:x2}{2:x2}{3:x2}!\r\n", data1[7], data1[8], data1[9], data1[0xA]);
                        return 0;
                    }
                    //Log.WriteTo//Log(string.Format("//擦除内存成功：地址：{0:x2}{1:x2}{2:x2}{3:x2}，长度:{4:x2}{5:x2}{6:x2}{7:x2}\r\n", data1[7], data1[8], data1[9], data1[0xA], data1[0xB], data1[0xC], data1[0xD], data1[0xE]));
                    //Console.WriteLine("擦除内存成功：地址：{0:x2}{1:x2}{2:x2}{3:x2}，长度:{4:x2}{5:x2}{6:x2}{7:x2}\r\n", data1[7], data1[8], data1[9], data1[0xA], data1[0xB], data1[0xC], data1[0xD], data1[0xE]);
                    Program.form2.textBox3.Text += string.Format("擦除内存成功：地址：{0:x2}{1:x2}{2:x2}{3:x2}，长度:{4:x2}{5:x2}{6:x2}{7:x2}\r\n", data1[7], data1[8], data1[9], data1[0xA], data1[0xB], data1[0xC], data1[0xD], data1[0xE]);
                }
            }
            return 1;
        }
        private int SendData(ref byte[] senddata)
        {
            VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
            Form2.Mutex3E80 = 1;//占用发送设备
            if (CANAPI.Output(SelectECUConfig.RequestID, ref senddata, (uint)senddata.Length, SelectECUConfig) == 1)
            {
                Form2.Mutex3E80 = 0;//取消占用发送设备  
                Program.form2.BTprogressBarAdd((senddata.Length / (double)Form2.TotalData) * 85);
                ////Console.Write("data2.Length:{0},TotalData:{1}\r\n", data2.Length, Form2.TotalData);
                CANAPI.WaitForTimeOut(5);
                CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                //3.接受应答
                if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x36) == 0)
                {
                    ////Console.Write("36h数据传输失败\r\n");
                    return 0;
                }

            }
            else
            {
                ////Console.Write("36h报文发送失败\r\n");
                return 0;
            }
            return 1;
        }
        //下载数据到ECU,DataBlockList为要下载的数据,需要配置流程，可以联系作者帮忙修改
        //一个Block启动一次34服务，即一组连续的数据
        unsafe private int BTDataDonwload(ref List<DataBlock> DataBlockList)
        {
            //Log.WriteTo//Log("//DonwloadData Start............\r\n");


            return 1;
        }
        //检查编程完整性
        unsafe private int checkProgrammingIntegrity(ref List<DataBlock> DataBlockList)
        {
            VCI_CAN_OBJ CANMsg;
            int BlockNum = 0;
            List<byte> downloaddata = new List<byte>();
            //将下载的数据何在一起，计算CRC32
            for (; BlockNum < DataBlockList.Count; BlockNum++)
            {
                for (int i = 0; i < DataBlockList[BlockNum].data.Count; i++)
                {
                    downloaddata.Add(DataBlockList[BlockNum].data[i]);

                }
            }
            /* foreach (int i in downloaddata)
             {
                 //Console.Write("{0:x} ",i);
             }*/
            //3.4 检查编程完整性31h，物理寻址
            //flash 数据校验
            //Log.WriteTo//Log("//检查编程完整性\r\n");
            CRC32_C crc32 = new CRC32_C();
            //uint value = CRC32_C.CRC32(0xffffffff, (uint)downloaddata.Count, downloaddata.ToArray());
            uint value = crc32.Bzip2_Update(downloaddata.ToArray());
            ////Console.WriteLine("CRC32:{0:x} Value1:{1:x}  Value2:{2:x}", value, downloaddata.ToArray()[1], downloaddata.ToArray()[downloaddata.ToArray().Length-2]);
            byte[] data4 = new byte[10];
            data4[0] = 0x10;
            data4[1] = 0x8;
            data4[2] = 0x31;
            data4[3] = 0x01;
            data4[4] = 0x00;
            data4[5] = 0x00;
            data4[6] = (byte)(value >> 24);
            data4[7] = (byte)(value >> 16);
            data4[8] = (byte)(value >> 8);
            data4[9] = (byte)value;
            if (CANAPI.Output(SelectECUConfig.RequestID, ref data4, (uint)data4.Length, SelectECUConfig) == 1)
            {
                CANAPI.WaitForTimeOut(50);
                //3.接受应答
                CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x31) == 1)
                {
                    if (CANMsg.Data[5] != 0x00)
                    {
                        //Log.WriteTo//Log("//检查编程完整性失败\r\n");
                        //Console.WriteLine("检查编程完整性失败");
                        Program.form2.textBox3.Text += string.Format("检查编程完整性失败\r\n");
                        return 0;
                    }
                    else
                    {
                        //Log.WriteTo//Log("//检查编程完整性成功\r\n");
                        //Console.WriteLine("检查编程完整性成功");
                        Program.form2.textBox3.Text += string.Format("检查编程完整性成功\r\n");
                        return 1;
                    }
                }

            }
            else
            {
                //Log.WriteTo//Log("//31h检查编程完整性报文发送失败\r\n");
                return 0;
            }
            return 1;
        }
        //写指纹
        unsafe private int WriteFigure()
        {
            VCI_CAN_OBJ CANMsg;
            DateTime dt = DateTime.Now;
            //3.4 检查编程完整性31h，物理寻址
            //flash 数据校验
            string[] YMD = dt.ToString("d").Split('/');
            //Log.WriteTo//Log("//写指纹数据F10A\r\n");
            byte[] data4 = new byte[15];
            data4[0] = 0x10;
            data4[1] = 0x0D;
            data4[2] = 0x2E;
            data4[3] = 0x00;
            data4[4] = 0x00;
            data4[5] = (byte)((Convert.ToByte(dt.ToString("s").Substring(0, 1)) << 4) + (Convert.ToByte(dt.ToString("s").Substring(1, 1))));//2019-11-26
            data4[6] = (byte)((Convert.ToByte(dt.ToString("s").Substring(2, 1)) << 4) + (Convert.ToByte(dt.ToString("s").Substring(3, 1))));//2019-12-02;
            data4[7] = (byte)((Convert.ToByte(dt.ToString("s").Substring(5, 1)) << 4) + (Convert.ToByte(dt.ToString("s").Substring(6, 1))));//2019-12-02;
            data4[8] = (byte)((Convert.ToByte(dt.ToString("s").Substring(8, 1)) << 4) + (Convert.ToByte(dt.ToString("s").Substring(9, 1))));//2019-12-02;
            data4[9] = 0x00;
            data4[10] = 0x01;
            data4[11] = 0x02;
            data4[12] = 0x03;
            data4[13] = 0x04;
            data4[14] = 0x05;
            if (CANAPI.Output(SelectECUConfig.RequestID, ref data4, (uint)data4.Length, SelectECUConfig) == 1)
            {
                CANAPI.WaitForTimeOut(50);
                //3.接受应答
                CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                return CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x2E);
            }
            else
            {
                //Log.WriteTo//Log("//31h写指纹报文发送失败\r\n");
                return 0;
            }
        }

    }
}
