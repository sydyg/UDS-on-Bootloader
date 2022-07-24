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

        //目前可支持1801的CCM，BCM，PEPS，IC，DAS，DSM，BSD
        public int LoadECUConfig()
        {
            if (SelectECUConfig.ECUName == null)//||((SelectECUConfig.ECUName!="BCM")&& (SelectECUConfig.ECUName != "CCM")&& (SelectECUConfig.ECUName != "PEPS") && (SelectECUConfig.ECUName != "BSD")))
            {
                return 0;
            }
            return 1;
        }
        //预编程
        //失败：返回0，成功，返回1
        private int PreProgramming()
        {
            //1.进入扩展会话,功能寻址
            if (CANAPI.changeToExtendedMode(ref SelectECUConfig) == 0)
            {
                ////Console.Write("Step1:预编程失败!\r\n");
                return 0;
            }
            CANAPI.WaitForTimeOut(50);
            //APP有效,则检查编程预条件
            //if (Form2.uiBTPara.AppValid == 1)
            {
                //历程控制，检查编程预条件，物理寻址
                if (CANAPI.RoutineControl(ref SelectECUConfig, 0xDF01) == 0)
                {
                    //Console.Write("检查编程预条件失败!\r\n");
                    Program.form2.textBox3.Text += string.Format("检查编程预条件失败!\r\n");
                    return 0;
                }

            }
            //控制DTC设置，1:开启，2：关闭，功能寻址
            if (CANAPI.ControlDTCSetting(ref SelectECUConfig, 2) == 0)
            {
                //Console.Write("控制DTC设置失败!\r\n");
                Program.form2.textBox3.Text += string.Format("控制DTC设置失败!\r\n");
                //return 0;
            }
            //通信控制，1：应用报文，2：NM报文，3：应用和NM报文，功能寻址
            if (CANAPI.ControlCommunication(ref SelectECUConfig, 3, 3) == 0)
            {
                //Console.Write("通信控制失败!\r\n");
                Program.form2.textBox3.Text += string.Format("通信控制失败!\r\n");
                //return 0;
            }
            return 1;
        }
        //主编程
        //失败：返回0，成功，返回1
        private int MainProgramming()
        {
            CANAPI.WaitForTimeOut(100);
            //1.进入编程会话
            if (CANAPI.changeToProgrammingMode(ref SelectECUConfig) == 0)
            {
                ////Console.Write("Step1:预编程失败!\r\n");
                return 0;
            }

            CANAPI.WaitForTimeOut(2000);
            //2.安全解锁Level3
            if (CANAPI.SecuritySession(ref SelectECUConfig, 9) == 0)
            {
                return 0;
            }

            //3.FlashDriver下载,有flashdriver的话
            if (Form2.uiBTPara.HasDri == 1)
            {
                //Log.WriteTo//Log(string.Format("//FlashDriver:共{0}块\r\n", s19DriBlockList.Count));
                if (DriFileType == 0)//S19类型的驱动文件
                {
                    if (BTDataDonwload(ref s19DriBlockList) == 0)
                    {
                        return 0;
                    }
                    if (checkProgrammingIntegrity(ref s19DriBlockList) == 0)
                    {
                        return 0;
                    }
                }
                else if (DriFileType == 1)//Hex类型的驱动文件
                {
                    if (BTDataDonwload(ref HexDriBlockList) == 0)
                    {
                        return 0;
                    }
                    if (checkProgrammingIntegrity(ref HexDriBlockList) == 0)
                    {
                        return 0;
                    }
                }
            }

            //写指纹，擦除内存和下载APP
            if (Form2.uiBTPara.HasApp1 == 1)
            {
                //写指纹
                if (WriteFigure() == 0)
                {
                    return 0;
                }
                if (APP1FileType == 0)//S19
                {
                    //Console.WriteLine("EraseMem:S19");
                    Program.form2.textBox3.Text += string.Format("EraseMem:S19");
                    if (EraseMem(ref s19APP1BlockList, Form2.uiBTPara.EraseMemType) == 0)
                    {
                        return 0;
                    }
                }
                else if (APP1FileType == 1)//Hex
                {
                    //Console.WriteLine("EraseMem:Hex");
                    Program.form2.textBox3.Text += string.Format("EraseMem:Hex");
                    if (EraseMem(ref HexAPP1BlockList, Form2.uiBTPara.EraseMemType) == 0)
                    {
                        return 0;
                    }
                }
                //下载APP1
                if (APP1FileType == 0)//S19
                {
                    //Log.WriteTo//Log(string.Format("//APP1File:共{0}块\r\n", s19APP1BlockList.Count));
                    if (BTDataDonwload(ref s19APP1BlockList) == 0)
                    {
                        return 0;
                    }
                    if (checkProgrammingIntegrity(ref s19APP1BlockList) == 0)
                    {
                        return 0;
                    }
                }
                else if (APP1FileType == 1)//Hex
                {
                    //Log.WriteTo//Log(string.Format("//APP1File:共{0}块\r\n", HexAPP1BlockList.Count));
                    if (BTDataDonwload(ref HexAPP1BlockList) == 0)
                    {
                        return 0;
                    }
                    if (checkProgrammingIntegrity(ref HexAPP1BlockList) == 0)
                    {
                        return 0;
                    }
                }
            }
            if (Form2.uiBTPara.HasApp2 == 1)
            {
                //写指纹
                if (WriteFigure() == 0)
                {
                    return 0;
                }
                if (APP2FileType == 0)//S19
                {
                    if (EraseMem(ref s19APP2BlockList, Form2.uiBTPara.EraseMemType) == 0)
                    {
                        return 0;
                    }
                }
                if (APP2FileType == 1)//Hex
                {
                    if (EraseMem(ref HexAPP2BlockList, Form2.uiBTPara.EraseMemType) == 0)
                    {
                        return 0;
                    }
                }
                if (APP2FileType == 0)//S19
                {
                    //Log.WriteTo//Log(string.Format("//APP2File:共{0}块\r\n", s19APP2BlockList.Count));
                    if (BTDataDonwload(ref s19APP2BlockList) == 0)
                    {
                        return 0;
                    }
                    if (checkProgrammingIntegrity(ref s19APP2BlockList) == 0)
                    {
                        return 0;
                    }
                }
                if (APP2FileType == 1)//Hex
                {
                    //Log.WriteTo//Log(string.Format("//APP2File:共{0}块\r\n", HexAPP2BlockList.Count));
                    if (BTDataDonwload(ref HexAPP2BlockList) == 0)
                    {
                        return 0;
                    }
                    if (checkProgrammingIntegrity(ref HexAPP2BlockList) == 0)
                    {
                        return 0;
                    }
                }
            }
            if (Form2.uiBTPara.HasBin1 == 1)
            {
                //写指纹
                if (WriteFigure() == 0)
                {
                    return 0;
                }
                if (EraseMem(ref BinBlockList, Form2.uiBTPara.EraseMemType) == 0)
                {
                    return 0;
                }
                //Log.WriteTo//Log(string.Format("//BinFile:共{0}块\r\n", BinBlockList.Count));
                if (BTDataDonwload(ref BinBlockList) == 0)
                {
                    return 0;
                }
                if (checkProgrammingIntegrity(ref BinBlockList) == 0)
                {
                    return 0;
                }
            }
            //检查编程依赖性
            if (checkProgrammingDependences() == 0)
            {
                //return 0;
            }

            return 1;
        }
        //后编程
        private int PostProgram()
        {
            //1.ECU复位
            if (ECUReset(ref SelectECUConfig) == 0)
            {
                return 0;
            }
            //2.进入扩展会话
            if (CANAPI.changeToExtendedMode(ref SelectECUConfig) == 0)
            {
                //Console.Write("进入扩展会话失败!\r\n");
                Program.form2.textBox3.Text += string.Format("进入扩展会话失败!\r\n");
                //return 0;
            }
            //控制DTC设置，1:开启，2：关闭，功能寻址
            if (CANAPI.ControlDTCSetting(ref SelectECUConfig, 1) == 0)
            {
                //Console.Write("控制DTC设置失败!\r\n");
                Program.form2.textBox3.Text += string.Format("控制DTC设置失败!\r\n");
                //return 0;
            }
            //通信控制，1：应用报文，2：NM报文，3：应用和NM报文，功能寻址,开启通信
            if (CANAPI.ControlCommunication(ref SelectECUConfig, 3, 0) == 0)
            {
                //Console.Write("通信控制失败!\r\n");
                Program.form2.textBox3.Text += string.Format("通信控制失败!\r\n");
                //return 0;
            }
            //2.进入默认会话
            if (CANAPI.changeToDefaultMode(ref SelectECUConfig) == 0)
            {
                return 0;
            }
            //3.清故障信息
            if (CANAPI.ClearDTC(ref SelectECUConfig) == 0)
            {
                return 0;
            }
            Form2.Timer_3E80.Enabled = false;//停止3E80服务
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
        //下载数据到ECU,DataBlockList为要下载的数据
        //一个Block启动一次34服务，即一组连续的数据
        unsafe private int BTDataDonwload(ref List<DataBlock> DataBlockList)
        {
            //Log.WriteTo//Log("//DonwloadData Start............\r\n");

            VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
            int BlockNum = 0;
            //计算需要下载的块数
            for (; BlockNum < DataBlockList.Count; BlockNum++)
            {
                //3.1 请求下载34h，物理寻址
                byte[] data1 = new byte[0xd];
                data1[0] = 0x10;
                data1[1] = 0x0B;
                data1[2] = 0x34;
                data1[3] = 0x00;
                data1[4] = 0x44;
                data1[5] = (byte)(DataBlockList[BlockNum].address >> 24);//地址MSB
                data1[6] = (byte)(DataBlockList[BlockNum].address >> 16);
                data1[7] = (byte)(DataBlockList[BlockNum].address >> 8);
                data1[8] = (byte)(DataBlockList[BlockNum].address);//地址LSB
                data1[9] = (byte)(DataBlockList[BlockNum].data.Count >> 24);//长度MSB
                data1[0xA] = (byte)(DataBlockList[BlockNum].data.Count >> 16);
                data1[0xB] = (byte)(DataBlockList[BlockNum].data.Count >> 8);
                data1[0xC] = (byte)(DataBlockList[BlockNum].data.Count);//长度MSB
                                                                        //发送数据
                CANAPI.Output(SelectECUConfig.RequestID, ref data1, (uint)data1.Length, SelectECUConfig);
                CANAPI.WaitForTimeOut(10);
                CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                //处理响应
                if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x34) == 0)
                {
                    //Console.Write("34h数据传输失败\r\n");
                    Program.form2.textBox3.Text += string.Format("34h数据传输失败\r\n");
                    return 0;
                }
                //Log.WriteTo//Log(string.Format("//请求下载成功,ECU每段支持大小：0x{0:x2}{1:x2}!\r\n", CANMsg.Data[3], CANMsg.Data[4]));
                //Console.Write("请求下载成功,ECU每段支持大小：0x{0:x2}{1:x2}!\r\n", CANMsg.Data[3], CANMsg.Data[4]);
                Program.form2.textBox3.Text += string.Format("请求下载成功,ECU每段支持大小：0x{0:x2}{1:x2}!\r\n", CANMsg.Data[3], CANMsg.Data[4]);
                ////Log.Flush//Log();
                //3.2 传输数据36h，物理寻址
                //根据74h的数据，确定36h的参数，如04 74 20 00 80
                int LenthFormat = (CANMsg.Data[2] >> 4);//获取字节长度所占字节数
                if (LenthFormat > 4)
                {
                    //Log.WriteTo//Log(string.Format("//74h_LenthFormat:请求下载字节数不支持：0x{0:x}字节格式", LenthFormat));
                    Program.form2.textBox3.Text += string.Format("74h_LenthFormat:请求下载字节数不支持：0x{0:x}字节格式", LenthFormat);
                    return 0;
                }

                int Lenth = 0;//获取每次36h最大可以下载的字节数，15765-2，规定网络层最大4096字节
                              //for (int i = 0,j=3; i < Math.Min(LenthFormat, 2); i++,j++)//长度最多支持2字节
                if (LenthFormat == 2)
                {
                    Lenth = CANMsg.Data[3];
                    Lenth = Lenth << 8;
                    Lenth |= CANMsg.Data[4];//Lenth=(36 x + data).lenth
                }
                //06 74 40 00 00 20 20
                else if (LenthFormat == 4)
                {
                    if (CANMsg.Data[3] != 0 && CANMsg.Data[4] != 0)
                    {
                        //Log.WriteTo//Log(string.Format("//74h_LenthFormat:请求下载数据块长度不支持：0x{0:x}字节格式", LenthFormat));
                        Program.form2.textBox3.Text += string.Format("74h_LenthFormat:请求下载字节数不支持：0x{0:x}字节格式", LenthFormat);
                        return 0;
                    }
                    Lenth = CANMsg.Data[5];
                    Lenth = Lenth << 8;
                    Lenth |= CANMsg.Data[6];//Lenth=(36 x + data).lenth
                }
                if (Lenth > 0xfff)
                {
                    //Log.WriteTo//Log(string.Format("//74h_MaxBlockSize:请求下载字节数超出4096字节：0x{0:x}字节格式", Lenth));
                    return 0;
                }
                byte[] data2;
                long DataBlockResidueNum = DataBlockList[BlockNum].data.Count;//每个块的剩余字节数
                long DownLowndCnt = (DataBlockResidueNum % Lenth == 0) ? (DataBlockResidueNum / Lenth) : (DataBlockResidueNum / Lenth + 1);
                //根据每块的大小来决定36的次数
                if (DataBlockResidueNum + 2 <= Lenth)//
                {
                    if ((DataBlockResidueNum + 2) > 7)//剩余字节数+服务字节数>7，多帧
                    {
                        data2 = new byte[DataBlockResidueNum + 4];//加上1x xx 36 xx
                        data2[0] = (byte)(0x10 | (((DataBlockResidueNum + 2) >> 8) & 0xf));//遇到0x7FE这种数据导致的Bug！！
                        data2[1] = (byte)((byte)(DataBlockResidueNum) + 2);//溢出的Bug
                        data2[2] = 0x36;
                        data2[3] = 1;//
                        for (int k = 0; k < DataBlockResidueNum; k++)//填充数据
                        {
                            data2[4 + k] = DataBlockList[BlockNum].data[k];
                        }
                        DataBlockResidueNum = 0;//剩余字节数清零
                    }
                    else//剩余字节数+服务字节数<=7，单帧
                    {
                        data2 = new byte[DataBlockResidueNum + 3];//加上0x 36 xx
                        data2[0] = (byte)(DataBlockResidueNum + 2);//
                        data2[1] = 0x36;
                        data2[2] = 1;
                        for (int k = 0; k < DataBlockResidueNum; k++)//填充数据
                        {
                            data2[3 + k] = DataBlockList[BlockNum].data[k];
                        }
                        DataBlockResidueNum = 0;//剩余字节数清零
                    }
                    //Console.Write("开始传输数据块:{0}-第{1}段，传输数据大小:0x{2:x}\r\n", BlockNum, 1, data2.Length - 2);
                    //Log.WriteTo//Log(string.Format("开始传输数据块:{0}-第{1}段，传输数据大小:0x{2:x}\r\n", BlockNum, 1, data2.Length - 2));
                    //Program.form2.textBox3.Text += string.Format("开始传输数据块:{0}-第{1}段，传输数据大小:0x{2:x}\r\n", BlockNum, 1, data2.Length - 2);
                    //Console.Write(string.Format("开始传输数据块:{0}-第{1}段，传输数据大小:0x{2:x}\r\n", BlockNum, 1, data2.Length - 2));
                    if (SendData(ref data2) == 0)
                    {
                        return 0;
                    }
                }


                CANAPI.WaitForTimeOut(20);
                //3.3 传输结束37h，物理寻址
                byte[] data3 = new byte[] { 0x01, 0x37 };
                if (CANAPI.Output(SelectECUConfig.RequestID, ref data3, (uint)data3.Length, SelectECUConfig) == 1)
                {
                    CANAPI.WaitForTimeOut(10);
                    //3.接受应答
                    CANMsg = CANAPI.WaitCANMessage(SelectECUConfig.ReponseID, 50);
                    if (CANAPI.DealRespose(ref CANMsg, SelectECUConfig, 0x37) == 0)
                    {
                        //Console.Write("37h数据传输失败\r\n");
                        Program.form2.textBox3.Text += string.Format("37h数据传输失败\r\n");
                        return 0;
                    }
                }
            }

            //Log.WriteTo//Log("//DownLoadData Sucess.........\r\n");
            Program.form2.textBox3.Text += string.Format("DownLoadData Sucess.........\r\n");
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
