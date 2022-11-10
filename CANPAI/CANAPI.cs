using System;
using UDS上位机.Driver_.USBCAN;
using System.Windows.Forms;
using System.Threading;
using UDS上位机.Driver_.VN1600;
using UDS上位机.Driver_.Kvaser;
using UDS上位机.Driver_.Vspy;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UDS上位机;
using System.Windows.Forms;

namespace UDS上位机
{
    class CANAPI
    {
        public static int chHndl = -1;
        public struct CANFrame
        {
            public uint ID;
            public ushort dlc;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] data;
        };
        private static int STmin = 0;
        //打开CAN设备
        //返回值：1,成功 0：打开设备失败 -1：未选择设备
        unsafe public static int CANOpenDevice()
        {
            int ret = 0;
            int Openflag = 0;
            int InitFlag = 0;
            //Vspy初始化
            if (Form2.uiDevicePara.CANDeviceType == "Vspy")
            {
                if (Vspy.OpenVspy() == 0)
                {
                    MessageBox.Show("打开设备失败,请检查设备连接（详情见控制台显示信息）");
                    return 0;
                }

                return 1;
            }
            //Kvaser的初始化
            //根据选择的设备、通道、波特率，初始化硬件
            if (Form2.uiDevicePara.CANDeviceType == "Kvaser")
            {
                //1.打开kvaser
                if (Driver_Kvaser.OpenKvaser() != 1)
                {
                    MessageBox.Show("打开设备失败,请检查设备连接（详情见控制台显示信息）");
                    return 0;
                }
                //2.选择通道、设置波特率
                int Channel1 = Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1;
                int Channel2 = Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1;
                int ChannelCANBaudRate1 = Form2.uiDevicePara.CANBaudRate == "250K" ? -3 : -2;
                int ChannelCANBaudRate2 = Form2.uiDevicePara.CANBaudRate == "250K" ? -3 : -2;
                //初始化通道
                chHndl = Driver_Kvaser.InitChannel(Channel1, ref chHndl, ChannelCANBaudRate1);
                if (chHndl == -1)
                {
                    //Console.WriteLine("打开通道失败,请检查设备是否正确连接");
                    Program.form2.textBox3.Text += string.Format("打开通道失败,请检查设备是否正确连接");
                    return 0;
                }
                if (chHndl == -2)
                {
                    //Console.WriteLine("设置波特率失败");
                    Program.form2.textBox3.Text += string.Format("设置波特率失败");
                    return 0;
                }
                /* string[] pasd1=new string[10];
                 pasd1[0] = "0";
                 Driver_Kvaser*/
                return 1;
            }

            else if (Form2.uiDevicePara.CANDeviceType == "Vspy")
            {
                /* string[] pasd1=new string[10];
                 pasd1[0] = "0";
                 Driver_Kvaser*/
                return 0;
            }
            //USBCAN的初始化
            else if ((Form2.uiDevicePara.CANDeviceType == "USBCAN1") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2EU"))
            {

                //1.打开设备
                //DWORD __stdcall VCI_OpenDevice(DWORD DevType,DWORD DevIndex,DWORD Reserved);
                //CANalyst -II的DevType=4,DevIndex=插入的USB设备索引号，默认设置索引0
                if (USBCAN.VCI_OpenDevice(Form2.USBCANDevicetype, 0, 0) == 1)
                {
                    Openflag = 1;
                }

                if (Openflag == 0)
                {
                    MessageBox.Show(string.Format("打开设备失败,请检查设备类型和设备索引号是否正确", Openflag), "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return 0;
                }

                //else
                {
                    //2.初始化设备CAN参数
                    VCI_INIT_CONFIG config = new VCI_INIT_CONFIG();
                    config.AccCode = 0;//unchecked((uint)Form2.ACCCodeNum << 3);//设置验收码unchecked((uint)(0x18D00000<<3))
                    config.AccMask = 0xffffffff;
                    config.Filter = (byte)Form2.CANFrameType;//帧类型
                    config.Mode = 1;
                    InitFlag = USBCAN.VCI_InitCAN(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref config);
                    if (1 == InitFlag)
                    {
                        ret = 1;
                    }
                    else if (0 == InitFlag)
                    {
                        MessageBox.Show("设备初始化CAN失败,请检查设备设备参数设置是否正确", "错误",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return 0;
                    }
                }
                USBCAN.USBCANRxThread = new Thread(new ThreadStart(USBCAN.USBCANRXThreadFun));
                //Console.WriteLine("RXThread Starting..............");
                Program.form2.WritetoMsg("USBCAN RXThread Starting..............\r\n");

                USBCAN.USBCANRxThread.Start();
            }
            //CANCaseXl、1600系列的初始化
            else if (Form2.uiDevicePara.CANDeviceType == "CANCaseXl" || Form2.uiDevicePara.CANDeviceType == "VN1600")
            {
                if (VN1600.Init() == 0)
                {
                    MessageBox.Show(string.Format("打开设备失败,请检查设备", Openflag), "错误",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return 0;
                }
                else
                    ret = 1;
            }

            return ret;
        }
        //关闭CAN设备
        public static int CANCloseDevice()
        {
            int ret = 0;
            if (Form2.uiDevicePara.CANDeviceType == "ValueCAN4")
            {
                if (Vspy.CloseVspy() == 1)
                {
                    Vspy.SpyRxThread.Abort();
                    Vspy.SpyCANFrameBuff.Clear();
                    return 1;
                }
                return 0;
                //Clear device type and open flag
            }
            //无线程
            if (Form2.uiDevicePara.CANDeviceType == "Kvaser")
            {
                int IsSuccess = Driver_Kvaser.CloseChannel(chHndl);
                if (IsSuccess == 0)
                {
                    return 0;
                }
                return 1;
            }
            else if ((Form2.uiDevicePara.CANDeviceType == "USBCAN1") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2EU"))
            {
                ret = USBCAN.VCI_CloseDevice(Form2.USBCANDevicetype, 0);
                USBCAN.USBCANRxThread.Abort();
                USBCAN.USBCANCANFrameBuff.Clear();
                ////Console.Write("ret:{0}",ret);
                if (ret != 1)
                {
                    MessageBox.Show("设备关闭失败,请检查设备设备是否连接正确", "错误",
                     MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return 0;
                }
                return ret;
            }
            else if (Form2.uiDevicePara.CANDeviceType == "CANCaseXl" || Form2.uiDevicePara.CANDeviceType == "VN1600")
            {
                VN1600.rxThread.Abort();
                VN1600.CANFrameBuff.Clear();
                //CANCaseXL.CANFrameQueue.Clear();
                return 1;
            }
            return -1;
        }


        //从内存缓冲区读取指定CAN数据帧,只读接收的数据，不读CAN盒子自己发出去的
        //返回ID=0,失败
        unsafe public static VCI_CAN_OBJ WaitCANMessage(uint CANID = 0, uint timeout = 10)
        {
            VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
            CANMsg.ID = 0;
            if (Form2.uiDevicePara.CANDeviceType == "ValueCAN4")
            {
                uint FrameNum = 10;//获取的最大帧数
                CANFrame[] CANFrames = new CANFrame[FrameNum];

                int count = 0;
                for (int n = 0; n < FrameNum; n++)
                {
                    CANFrames[n].data = new byte[8];
                }
                uint TimeOutCnt = timeout / 10;
                do
                {
                    count = Vspy.VspyGetMessage(ref CANFrames, FrameNum);
                    for (int j = 0; j < count; j++)
                    {
                        if (CANFrames[j].ID == CANID)
                        {
                            byte[] data = new byte[8];
                            CANMsg.ID = CANFrames[j].ID;
                            CANMsg.DataLen = (byte)CANFrames[j].dlc;
                            for (int i = 0; i < CANFrames[j].dlc; i++)
                            {
                                CANMsg.Data[i] = CANFrames[j].data[i];
                                data[i] = CANFrames[j].data[i];
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Rx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}\r\n", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANMsg.ID, CANMsg.Data[0], CANMsg.Data[1], CANMsg.Data[2], CANMsg.Data[3], CANMsg.Data[4], CANMsg.Data[5], CANMsg.Data[6], CANMsg.Data[7]).ToUpper());
                            ////ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANMsg.ID, "Rx", "8", data);
                            return CANMsg;
                        }
                    }

                    Thread.Sleep(10);
                } while ((TimeOutCnt--) != 0);

            }
            if ((Form2.uiDevicePara.CANDeviceType == "USBCAN1") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2EU"))
            {
                uint FrameNum = 10;//获取的最大帧数
                CANFrame[] CANFrames = new CANFrame[FrameNum];
                int count = 0;
                for (int n = 0; n < FrameNum; n++)
                {
                    CANFrames[n].data = new byte[8];
                }
                uint TimeOutCnt = timeout / 10;
                do
                {
                    count = USBCAN.USBCANGetMessage(ref CANFrames, FrameNum);
                    for (int j = 0; j < count; j++)
                    {
                        if (CANFrames[j].ID == CANID)
                        {
                            byte[] data = new byte[8];
                            CANMsg.ID = CANFrames[j].ID;
                            CANMsg.DataLen = (byte)CANFrames[j].dlc;
                            for (int i = 0; i < CANFrames[j].dlc; i++)
                            {
                                CANMsg.Data[i] = CANFrames[j].data[i];
                                data[i] = CANFrames[j].data[i];
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Rx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}\r\n", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANMsg.ID, CANMsg.Data[0], CANMsg.Data[1], CANMsg.Data[2], CANMsg.Data[3], CANMsg.Data[4], CANMsg.Data[5], CANMsg.Data[6], CANMsg.Data[7]).ToUpper());
                            ////ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANMsg.ID, "Rx", "8", data);
                            return CANMsg;
                        }
                    }
                    Thread.Sleep(10);
                } while ((TimeOutCnt--) != 0);
            }
            else if (Form2.uiDevicePara.CANDeviceType == "Kvaser")
            {
                uint FrameNum = 1;//获取的最大帧数
                int count = 0;
                CANFrame[] CANFrames = new CANFrame[FrameNum];
                for (int n = 0; n < FrameNum; n++)
                {
                    CANFrames[n].data = new byte[8];
                }
                count = Driver_Kvaser.DumpMessageLoop(ref CANFrames, chHndl, timeout, FrameNum, CANID);//kvaser读取报文函数
                for (int j = 0; j < count; j++)
                {
                    if (CANFrames[j].ID == CANID & CANID != 0)
                    {
                        CANMsg.ID = CANFrames[j].ID;
                        CANMsg.DataLen = (byte)CANFrames[j].dlc;
                        byte[] data0 = new byte[8];
                        for (int i = 0; i < CANFrames[j].dlc; i++)
                        {
                            CANMsg.Data[i] = CANFrames[j].data[i];
                            data0[i] = CANFrames[j].data[i];
                        }
                        //Log.WriteToLog(string.Format("{0} 1 {1:x}x Rx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}\r\n", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANMsg.ID, CANMsg.Data[0], CANMsg.Data[1], CANMsg.Data[2], CANMsg.Data[3], CANMsg.Data[4], CANMsg.Data[5], CANMsg.Data[6], CANMsg.Data[7]).ToUpper());
                        ////ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), chHndl + 1, CANMsg.ID, "Rx", "8", data0);
                        return CANMsg;
                    }
                }
                Thread.Sleep(10);
            }
            else if (Form2.uiDevicePara.CANDeviceType == "CANCaseXl" || Form2.uiDevicePara.CANDeviceType == "VN1600")
            {
                uint FrameNum = 1;//获取的最大帧数
                int count = 0;
                CANFrame[] CANFrames = new CANFrame[FrameNum];
                for (int n = 0; n < FrameNum; n++)
                {
                    CANFrames[n].data = new byte[8];
                }
                uint TimeOutCnt = timeout / 10;
                do
                {
                    count = VN1600.CANRecv(ref CANFrames, FrameNum);
                    for (int j = 0; j < count; j++)
                    {
                        if (CANFrames[j].ID == CANID)
                        {
                            byte[] data = new byte[8];
                            CANMsg.ID = CANFrames[j].ID;
                            CANMsg.DataLen = (byte)CANFrames[j].dlc;
                            for (int i = 0; i < CANFrames[j].dlc; i++)
                            {
                                CANMsg.Data[i] = CANFrames[j].data[i];
                                data[i] = CANMsg.Data[i];
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Rx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}\r\n", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANMsg.ID, CANMsg.Data[0], CANMsg.Data[1], CANMsg.Data[2], CANMsg.Data[3], CANMsg.Data[4], CANMsg.Data[5], CANMsg.Data[6], CANMsg.Data[7]).ToUpper());
                            ////ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANMsg.ID, "Rx", "8", data);
                            return CANMsg;
                        }
                    }

                    Thread.Sleep(10);
                } while ((TimeOutCnt--) != 0);

            }
            return CANMsg;
        }
        //延时函数
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                Application.DoEvents();
            }
        }
        //等待超时
        public static int WaitForTimeOut(int TimeOut = 10)
        {
            //启动定时器
            /*Form2.TimerWaitTimeOutCount = 0;
            Form2.Timer_WaitTimeOut.Enabled = true;
            if (Form2.TimerWaitTimeout == 1)
            {
                Form2.Timer_WaitTimeOut.Enabled = false;
                Form2.TimerWaitTimeout = 0;
                Form2.TimerWaitTimeOutCount = 0;
                return 1;
            }
            return 0;*/
            Thread.Sleep(TimeOut);
            return 1;
        }
        //安全算法实现，需要配置流程，可以联系作者帮忙修改
        public static int SecurityAlgorithm(byte SecurityLevel, ulong Seed, ulong Mask, ref ulong Key)
        {
            if (SecurityLevel == 0x01)
            {
                Key = Mask;
                return 1;
            }
            else if (SecurityLevel == 0x03)
            {
                Key = Mask;
                return 1;
            }
            else if (SecurityLevel == 0x09)
            {
                Key = Mask;
                return 1;
            }
            return 0;
        }

        //发送CANFrame
        //CANID,
        //Data：要发送的数据
        //Lenth,要发送数据的长度
        //DLC,CAN报文长度
        unsafe public static int Output(uint CANID, ref byte[] Data, uint Lenth, ECUConfigInfo ECUConfig, byte DLC = 8)
        {
            if (Form2.uiDevicePara.CANDeviceType == "Kvaser")
            {
                if (SendFrame_Kvaser(CANID, ref Data, Lenth, ECUConfig, DLC) == 0)
                {
                    //Console.Write("发送失败!\r\n");
                    Program.form2.textBox3.Text += "发送失败!\r\n";
                    return 0;
                }
            }
            if ((Form2.uiDevicePara.CANDeviceType == "USBCAN1") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2EU"))
            {
                if (SendFrame_USBCAN2(CANID, ref Data, Lenth, ECUConfig, DLC) == 0)
                {
                    //Console.Write("发送失败!\r\n");
                    Program.form2.textBox3.Text += "发送失败!\r\n";
                    return 0;
                }
            }
            else if (Form2.uiDevicePara.CANDeviceType == "CANCaseXl" || Form2.uiDevicePara.CANDeviceType == "VN1600")
            {
                if (SendFrame_VN1600(CANID, ref Data, Lenth, ECUConfig, DLC) == 0)
                {
                    //Console.Write("发送失败!\r\n");
                    Program.form2.textBox3.Text += "发送失败!\r\n";
                    return 0;
                }
            }
            else if (Form2.uiDevicePara.CANDeviceType == "ValueCAN4")
            {
                if (SendFrame_Vspy(CANID, ref Data, Lenth, ECUConfig, DLC) == 0)
                {
                    //Console.Write("发送失败!\r\n");
                    Program.form2.textBox3.Text += "发送失败!\r\n";
                    return 0;
                }
            }
            return 1;
        }
        unsafe private static int SendFrame_Vspy(uint CANID, ref byte[] Data, uint Lenth, ECUConfigInfo ECUConfig, byte DLC)
        {
            int ret = 0;
            if (Lenth <= 8)
            {
                byte[] data = new byte[8];
                byte[] tempData = new byte[8] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                for (int i = 0; i < Lenth; i++)
                {
                    tempData[i] = Data[i];
                    data[i] = tempData[i];
                }
                ret = Vspy.VspySendMessage(CANID, ref tempData, DLC);
                if (ret == 1)
                {
                    //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANID, tempData[0], tempData[1], tempData[2], tempData[3], tempData[4], tempData[5], tempData[6], tempData[7]).ToUpper());
                    ////ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANID, "Tx", "8", data);
                    return ret;
                }

                else
                    return ret;
            }
            else if (Lenth > 8)
            {
                byte[] tempData = new byte[8] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                int k = 1;
                long ResidueNum = Lenth;//剩余字节数
                long HaveSended = 0;//已发字节数
                VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
                byte[] data = new byte[8];
                //1.先发送首帧
                for (int i = 0; i < 8; i++)
                {
                    tempData[i] = Data[i];
                    data[i] = tempData[i];
                }
                if (0 == Vspy.VspySendMessage(CANID, ref tempData, 8))
                {
                    //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                    Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                    return 0;
                }
                //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANID, tempData[0], tempData[1], tempData[2], tempData[3], tempData[4], tempData[5], tempData[6], tempData[7]).ToUpper());
                //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANID, "Tx", "8", data);
                ResidueNum -= 8;
                HaveSended += 8;
                long cf_send_number = 0;//需要的连续帧数量
                long blocksize = 0;
                WaitForTimeOut(10);
                if ((ResidueNum) % 7 == 0)
                    cf_send_number = (ResidueNum) / 7;
                else
                    cf_send_number = (ResidueNum) / 7 + 1;
                //
                do
                {
                    CANMsg = WaitCANMessage(ECUConfig.ReponseID, 2000);//等待流控帧
                    if ((CANMsg.ID != 0) && (CANMsg.Data[0] == 0x30))
                    {
                        blocksize = CANMsg.Data[1];
                        //STmin = (CANMsg.Data[2] > 0) ? 1 : 0;
                        STmin = (Form2.uiBTPara.FastDown == 1) ? 0 : CANMsg.Data[2];
                        if (cf_send_number < blocksize || blocksize == 0)
                            blocksize = cf_send_number;//需要发送的连续帧数目<ECU要求的连续帧数，或ECU不要求连续帧数
                        for (int i = 0; i < blocksize; i++)//循环发送连续帧
                        {
                            int dataindex = 0;
                            WaitForTimeOut(STmin);

                            for (int n = 0; n < 8; n++)
                            {
                                tempData[n] = 0xAA;
                                data[n] = 0xAA;
                            }
                            tempData[0] = (byte)(0x20 + k);
                            data[0] = (byte)(0x20 + k);
                            for (dataindex = 0; dataindex < Math.Min(ResidueNum, 7); dataindex++)
                            {
                                tempData[dataindex + 1] = Data[HaveSended + dataindex];
                                data[dataindex + 1] = tempData[dataindex + 1];
                            }
                            if (0 == Vspy.VspySendMessage(CANID, ref tempData, 8))
                            {
                                //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                                Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                                return 0;
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANID, tempData[0], tempData[1], tempData[2], tempData[3], tempData[4], tempData[5], tempData[6], tempData[7]).ToUpper());
                            //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANID, "Tx", "8", data);
                            WaitForTimeOut(STmin);
                            //long wait = 1000000 / 4;
                            //while (wait-- > 0) ;

                            ResidueNum -= dataindex;
                            HaveSended += dataindex;
                            k++;
                            if (k == 0x10)
                                k = 0;
                        }
                        cf_send_number -= blocksize;//计算剩余流控帧数量
                    }
                    else
                        return 0;
                } while (cf_send_number > 0);
            }
            return 1;
        }
        //VN1600和CANCaseXL发送报文函数
        unsafe private static int SendFrame_VN1600(uint CANID, ref byte[] Data, uint Lenth, ECUConfigInfo ECUConfig, byte DLC)
        {
            int ret = 0;
            if (Lenth <= 8)
            {
                byte[] data = new byte[8];
                byte[] tempData = new byte[8] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                for (int i = 0; i < Lenth; i++)
                {
                    tempData[i] = Data[i];
                    data[i] = Data[i];
                }
                ret = VN1600.CANTransmit(CANID, ref tempData, DLC);
                if (ret == 1)
                {
                    //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANID, tempData[0], tempData[1], tempData[2], tempData[3], tempData[4], tempData[5], tempData[6], tempData[7]).ToUpper());
                    //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANID, "Tx", "8", data);
                    return ret;
                }
                else
                    return ret;
            }
            else if (Lenth > 8)
            {
                byte[] tempData = new byte[8] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                int k = 1;
                long ResidueNum = Lenth;//剩余字节数
                long HaveSended = 0;//已发字节数
                VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
                byte[] data = new byte[8];
                //1.先发送首帧
                for (int i = 0; i < 8; i++)
                {
                    tempData[i] = Data[i];
                    data[i] = Data[i];
                }
                if (0 == VN1600.CANTransmit(CANID, ref tempData, 8))
                {
                    //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                    Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                    return 0;
                }
                //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANID, tempData[0], tempData[1], tempData[2], tempData[3], tempData[4], tempData[5], tempData[6], tempData[7]).ToUpper());
                //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANID, "Tx", "8", data);
                ResidueNum -= 8;
                HaveSended += 8;
                long cf_send_number = 0;//需要的连续帧数量
                long blocksize = 0;
                //WaitForTimeOut(10);
                if ((ResidueNum) % 7 == 0)
                    cf_send_number = (ResidueNum) / 7;
                else
                    cf_send_number = (ResidueNum) / 7 + 1;
                //
                do
                {
                    CANMsg = WaitCANMessage(ECUConfig.ReponseID, 5000);//等待流控帧,等待5000ms
                    if ((CANMsg.ID != 0) && (CANMsg.Data[0] == 0x30))
                    {
                        blocksize = CANMsg.Data[1];
                        //STmin = (CANMsg.Data[2] > 0) ? 1 : 0;
                        STmin = (Form2.uiBTPara.FastDown == 1) ? 0 : CANMsg.Data[2];
                        if (cf_send_number < blocksize || blocksize == 0)
                            blocksize = cf_send_number;//需要发送的连续帧数目<ECU要求的连续帧数，或ECU不要求连续帧数
                        for (int i = 0; i < blocksize; i++)//循环发送连续帧
                        {
                            int dataindex = 0;
                            WaitForTimeOut(STmin);

                            for (int n = 0; n < 8; n++)
                            {
                                tempData[n] = 0xAA;
                                data[n] = 0xAA;
                            }
                            tempData[0] = (byte)(0x20 + k);
                            data[0] = (byte)(0x20 + k);
                            for (dataindex = 0; dataindex < Math.Min(ResidueNum, 7); dataindex++)
                            {
                                tempData[dataindex + 1] = Data[HaveSended + dataindex];
                                data[dataindex + 1] = tempData[dataindex + 1];
                            }
                            if (0 == VN1600.CANTransmit(CANID, ref tempData, 8))
                            {
                                //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                                Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                                return 0;
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), CANID, tempData[0], tempData[1], tempData[2], tempData[3], tempData[4], tempData[5], tempData[6], tempData[7]).ToUpper());
                            //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, CANID, "Tx", "8", data);
                            WaitForTimeOut(STmin);
                            //long wait = 1000000 / 4;
                            //while (wait-- > 0) ;

                            ResidueNum -= dataindex;
                            HaveSended += dataindex;
                            k++;
                            if (k == 0x10)
                                k = 0;
                        }
                        cf_send_number -= blocksize;//计算剩余流控帧数量
                    }
                    else
                    {
                        Program.form2.textBox3.Text += "流控帧接收超时\r\n";
                        return 0;
                    }

                } while (cf_send_number > 0);
            }
            return 1;
        }

        //CANalyst发送报文函数
        unsafe private static int SendFrame_USBCAN2(uint CANID, ref byte[] Data, uint Lenth, ECUConfigInfo ECUConfig, byte DLC)
        {
            if (Lenth <= 8)
            {
                byte[] tempData = new byte[8] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                //1.初始化要发送的数据
                VCI_CAN_OBJ vco = new VCI_CAN_OBJ();
                vco.ID = CANID;//ID
                vco.ExternFlag = 1;
                vco.RemoteFlag = 0;
                vco.DataLen = DLC;
                //填充位置为1
                for (int i = 0; i < Lenth; i++)
                {
                    tempData[i] = Data[i];
                }
                for (int i = 0; i < 8; i++)
                {
                    vco.Data[i] = tempData[i];
                }
                if (0 == USBCAN.VCI_Transmit(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref vco, 1))
                {
                    //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                    Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                    return 0;
                }
                //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]).ToUpper());
                //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, vco.ID, "Tx", "8", tempData);
                return 1;
            }
            else if (Lenth > 8)
            {
                VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
                byte[] data = new byte[8];
                int k = 1;
                long ResidueNum = Lenth;//剩余字节数
                long HaveSended = 0;//已发字节数
                                    //1.先发送首帧
                VCI_CAN_OBJ vco = new VCI_CAN_OBJ();
                vco.ID = CANID;//ID
                vco.ExternFlag = 1;
                vco.RemoteFlag = 0;
                vco.DataLen = 8;
                for (int i = 0; i < 8; i++)
                {
                    vco.Data[i] = Data[i];
                    data[i] = Data[i];
                }
                if (0 == USBCAN.VCI_Transmit(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref vco, 1))
                {
                    //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                    Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                    return 0;
                }
                //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]).ToUpper());
                //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, vco.ID, "Tx", "8", data);
                ResidueNum -= 8;
                HaveSended += 8;
                long cf_send_number = 0;//需要的连续帧数量
                long blocksize = 0;
                WaitForTimeOut(10);
                if ((ResidueNum) % 7 == 0)
                    cf_send_number = (ResidueNum) / 7;
                else
                    cf_send_number = (ResidueNum) / 7 + 1;
                //
                do
                {
                    CANMsg = WaitCANMessage(ECUConfig.ReponseID, 2000);//等待流控帧
                    if ((CANMsg.ID != 0) && (CANMsg.Data[0] == 0x30))
                    {
                        blocksize = CANMsg.Data[1];
                        //STmin = (CANMsg.Data[2] > 0) ? 1 : 0;
                        STmin = (Form2.uiBTPara.FastDown == 1) ? 0 : CANMsg.Data[2];
                        if (cf_send_number < blocksize || blocksize == 0)
                            blocksize = cf_send_number;//需要发送的连续帧数目<ECU要求的连续帧数，或ECU不要求连续帧数
                        for (int i = 0; i < blocksize; i++)//循环发送连续帧
                        {
                            int dataindex = 0;
                            WaitForTimeOut(STmin);
                            for (int n = 0; n < 8; n++)
                            {
                                vco.Data[n] = 0xAA;
                                data[n] = vco.Data[n];
                            }
                            vco.Data[0] = (byte)(0x20 + k);
                            data[0] = (byte)(0x20 + k);
                            for (dataindex = 0; dataindex < Math.Min(ResidueNum, 7); dataindex++)
                            {
                                vco.Data[dataindex + 1] = Data[HaveSended + dataindex];
                                data[dataindex + 1] = Data[HaveSended + dataindex];
                            }
                            if (0 == USBCAN.VCI_Transmit(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref vco, 1))
                            {
                                //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                                Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                                return 0;
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]).ToUpper());
                            //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, vco.ID, "Tx", "8", data);
                            WaitForTimeOut(STmin);
                            //long wait = 1000000 / 4;
                            //while (wait-- > 0) ;

                            ResidueNum -= dataindex;
                            HaveSended += dataindex;
                            k++;
                            if (k == 0x10)
                                k = 0;
                        }
                        cf_send_number -= blocksize;//计算剩余流控帧数量
                    }
                    else
                        return 0;
                } while (cf_send_number > 0);
            }
            return 1;
        }

        //Kvaser发送报文函数
        unsafe private static int SendFrame_Kvaser(uint CANID, ref byte[] Data, uint Lenth, ECUConfigInfo ECUConfig, byte DLC)
        {
            if (Lenth <= 8)
            {
                byte[] data = new byte[8];
                byte[] tempData = new byte[8] { 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
                //1.初始化要发送的数据
                VCI_CAN_OBJ vco = new VCI_CAN_OBJ();
                vco.ID = CANID;//ID
                vco.ExternFlag = 1;
                int flag = 4;
                vco.RemoteFlag = 0;
                vco.DataLen = DLC;
                //填充位置为1
                for (int i = 0; i < Lenth; i++)
                {
                    tempData[i] = Data[i];
                }
                for (int i = 0; i < 8; i++)
                {
                    vco.Data[i] = tempData[i];
                    data[i] = tempData[i];
                }
                if (0 == Driver_Kvaser.SendMessage(ref vco, data, flag))
                {
                    //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                    Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                    return 0;
                }
                //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]).ToUpper());
                //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), chHndl + 1, vco.ID, "Tx", "8", data);
                return 1;
            }
            else if (Lenth > 8)
            {
                VCI_CAN_OBJ CANMsg = new VCI_CAN_OBJ();
                byte[] data = new byte[8];
                int k = 1;
                long ResidueNum = Lenth;//剩余字节数
                long HaveSended = 0;//已发字节数
                                    //1.先发送首帧
                VCI_CAN_OBJ vco = new VCI_CAN_OBJ();
                vco.ID = CANID;//ID
                vco.ExternFlag = 1;
                int flag = 4;
                vco.RemoteFlag = 0;
                vco.DataLen = 8;
                for (int i = 0; i < 8; i++)
                {
                    vco.Data[i] = Data[i];
                    data[i] = Data[i];
                }
                if (0 == Driver_Kvaser.SendMessage(ref vco, data, flag))
                {
                    //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                    Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                    return 0;
                }
                //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]).ToUpper());
                //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, vco.ID, "Tx", "8", data);
                ResidueNum -= 8;
                HaveSended += 8;
                long cf_send_number = 0;//需要的连续帧数量
                long blocksize = 0;
                WaitForTimeOut(10);
                if ((ResidueNum) % 7 == 0)
                    cf_send_number = (ResidueNum) / 7;
                else
                    cf_send_number = (ResidueNum) / 7 + 1;
                do
                {
                    CANMsg = WaitCANMessage(ECUConfig.ReponseID, 2000);//等待流控帧
                    if ((CANMsg.ID != 0) && (CANMsg.Data[0] == 0x30))
                    {
                        blocksize = CANMsg.Data[1];
                        //STmin = (CANMsg.Data[2] > 0) ? 1 : 0;
                        STmin = (Form2.uiBTPara.FastDown == 1) ? 0 : CANMsg.Data[2];
                        if (cf_send_number < blocksize || blocksize == 0)
                            blocksize = cf_send_number;//需要发送的连续帧数目<ECU要求的连续帧数，或ECU不要求连续帧数
                        for (int i = 0; i < blocksize; i++)//循环发送连续帧
                        {
                            int dataindex = 0;
                            WaitForTimeOut(STmin);
                            for (int n = 0; n < 8; n++)
                            {
                                vco.Data[n] = 0xAA;
                                data[n] = 0xAA;
                            }
                            vco.Data[0] = (byte)(0x20 + k);
                            data[0] = (byte)(0x20 + k);
                            for (dataindex = 0; dataindex < Math.Min(ResidueNum, 7); dataindex++)
                            {
                                vco.Data[dataindex + 1] = Data[HaveSended + dataindex];
                                data[dataindex + 1] = Data[HaveSended + dataindex];
                            }
                            if (0 == Driver_Kvaser.SendMessage(ref vco, data, flag))
                            {
                                //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
                                Program.form2.textBox3.Text += "CAN发送失败,请检查设备设备是否连接正确\r\n";
                                return 0;
                            }
                            //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, data[0], data[1], data[2], data[3], data[4], data[5], data[6], data[7]).ToUpper());
                            //ShowMsg.ShowConfig(ShowMsg.MsgTable, DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), 1, vco.ID, "Tx", "8", data);
                            WaitForTimeOut(STmin);
                            //long wait = 1000000/4;
                            //while (wait--<0) ;
                            ResidueNum -= dataindex;
                            HaveSended += dataindex;
                            k++;
                            if (k == 0x10)
                                k = 0;
                        }
                        cf_send_number -= blocksize;//计算剩余流控帧数量
                    }
                    else
                        return 0;
                } while (cf_send_number > 0);
            }
            return 1;
        }
        //      //2.等待首个流控帧
        //      int ContinueFrameNum = -1;//流控帧要求的连续帧数目,默认连续发送连续帧
        //      WaitForTimeOut(10);
        //      int time = 50;//N_Bs:1S
        //      do
        //      {
        //          CANMsg = WaitCANMessage(ECUConfig.ReponseID, 10);
        //          //STmin = CANMsg.Data[2];
        //          if ((CANMsg.ID != 0) && (CANMsg.Data[0] == 0x30))
        //          {
        //              ContinueFrameNum = CANMsg.Data[1];
        //              //STmin = CANMsg.Data[2] == 0 ? 2 : CANMsg.Data[2];
        //              STmin = CANMsg.Data[2];
        //              break;
        //          }
        //          WaitForTimeOut(10);
        //          if ((time--) <= 0)
        //          {
        //              //Console.Write("未收到流控帧\r\n");
        //              return 0;
        //          }

        //      } while (true);
        //      //发送连续帧
        //      while (ResidueNum > 0)//还有未发送字节
        //      {

        //          int dataindex = 0;
        //          //不再有流控帧，连续发送
        //          if (ContinueFrameNum == 0)
        //          {
        //              do
        //              {
        //                  //未使用字节填充1
        //                  for (int n = 0; n < 8; n++)
        //                  {
        //                      vco.Data[n] = 0xAA;
        //                  }
        //                  vco.Data[0] = (byte)(0x20 + k);
        //                  for (dataindex = 0; dataindex < Math.Min(ResidueNum, 7); dataindex++)
        //                  {
        //                      vco.Data[dataindex + 1] = Data[HaveSended + dataindex];
        //                  }
        //                  if (0 == Driver_CANalyst.VCI_Transmit(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref vco, 1))
        //                  {
        //                      //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
        //                      return 0;
        //                  }
        //                  //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]).ToUpper());
        //                  WaitForTimeOut(STmin);
        //                  //long wait = 1000000/7;
        //                  //while (wait--<0) ;
        //                  ResidueNum -= dataindex;
        //                  HaveSended += dataindex;
        //                  k++;
        //                  if (k == 0x10)
        //                      k = 0;

        //              } while (ResidueNum > 0);

        //              ////Console.WriteLine("time 1 {0:x} Tx d 8 {1:x} {2:x} {3:x} {4:x} {5:x} {6:x} {7:x} {8:x}", vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]);
        //          }
        //          else
        //          {
        //              //流控帧数不为0，发送要求的流控帧数
        //              do
        //              {
        //                  //未使用字节填充1
        //                  for (int n = 0; n < 8; n++)
        //                  {
        //                      vco.Data[n] = 0xAA;
        //                  }
        //                  vco.Data[0] = (byte)(0x20 + k);
        //                  for (dataindex = 0; dataindex < Math.Min(ResidueNum, 7); dataindex++)
        //                  {
        //                      vco.Data[dataindex + 1] = Data[HaveSended + dataindex];
        //                  }
        //                  if (0 == Driver_CANalyst.VCI_Transmit(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref vco, 1))
        //                  {
        //                      //Console.Write("CAN发送失败,请检查设备设备是否连接正确\r\n");
        //                      return 0;
        //                  }
        //                  ////Console.Write("{0} 1 {1:x}x  Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]);
        //                  //Log.WriteToLog(string.Format("{0} 1 {1:x}x Tx d 8 {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}", DateTime.Now.ToString("O").Split(':')[2].Substring(0, 7), vco.ID, vco.Data[0], vco.Data[1], vco.Data[2], vco.Data[3], vco.Data[4], vco.Data[5], vco.Data[6], vco.Data[7]).ToUpper());
        //                  WaitForTimeOut(STmin);
        //                  ResidueNum -= dataindex;
        //                  HaveSended += dataindex;
        //                  k++;
        //                  if (k == 0x10)
        //                      k = 0;

        //              } while ((--ContinueFrameNum > 0) && (ResidueNum > 0));
        //              //还有未发送字节，则等待新的流控帧
        //              int time2 = 50;
        //              do
        //              {
        //                  if (ResidueNum > 0)//还有未发送字节，才等待流控帧
        //                  {
        //                      CANMsg = WaitCANMessage(ECUConfig.ReponseID, 10);
        //                      if ((CANMsg.ID != 0) && (CANMsg.Data[0] == 0x30))
        //                      {
        //                          ContinueFrameNum = CANMsg.Data[1];
        //                          //STmin = CANMsg.Data[2] == 0 ? 2 : CANMsg.Data[2];
        //                          STmin = CANMsg.Data[2];
        //                          break;
        //                      }
        //                      WaitForTimeOut(10);
        //                      if ((time2--) <= 0)
        //                      {
        //                          //Console.Write("未收到流控帧\r\n");
        //                          return 0;
        //                      }
        //                  }
        //                  else
        //                  {
        //                      break;
        //                  }

        //              } while (true);
        //          }
        //      }
        //      return 1;
        //   }
        //   return 1;
        //}

        //获取ECU配置，诊断ID，Mask等
        //参数：ECUName，ECUConfig的引用，作为传出参数
        //成功：返回1
        public static int GetECUConfig(string ECUName, ref ECUConfigInfo ECUConfig)
        {
            //遍历ECUConfigArray
            for (int i = 0; i < Form2.ECUConfigArray.Length; i++)
            {
                if (Form2.ECUConfigArray[i].ECUName == ECUName)
                {
                    ECUConfig.ECUName = Form2.ECUConfigArray[i].ECUName;
                    ECUConfig.Mask = Form2.ECUConfigArray[i].Mask;
                    ECUConfig.ReponseID = Form2.ECUConfigArray[i].ReponseID;
                    ECUConfig.RequestID = Form2.ECUConfigArray[i].RequestID;
                    ECUConfig.FuncID = Form2.ECUConfigArray[i].FuncID;
                    return 1;
                }
            }
            return 0;
        }
        //清除DTC
        unsafe public static int ClearDTC(ref ECUConfigInfo SelectECUConfig, bool FuncAddr = true)
        {
            VCI_CAN_OBJ CANMsg;
            //1.发送10 03
            byte[] data = new byte[5];
            data[0] = 0x04;
            data[1] = 0x14;
            data[2] = 0xFF;
            data[3] = 0xFF;
            data[4] = 0xFF;
            ////Console.Write("SelectECUConfig.FuncID:{0:x}\r\n", SelectECUConfig.FuncID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);
            //3.接受应答
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            return DealRespose(ref CANMsg, SelectECUConfig, 0x14);
        }
        //将ECU切换到默认会话模式,默认物理寻址，其他服务也是如此
        unsafe public static int changeToDefaultMode(ref ECUConfigInfo SelectECUConfig, bool FuncAddr = true)
        {
            VCI_CAN_OBJ CANMsg;
            //1.发送10 03
            byte[] data = new byte[3];
            data[0] = 0x02;
            data[1] = 0x10;
            data[2] = 0x01;
            ////Console.Write("SelectECUConfig.FuncID:{0:x}\r\n", SelectECUConfig.FuncID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);
            //3.接受应答
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            return DealRespose(ref CANMsg, SelectECUConfig, 0x10);
        }
        //将ECU切换到扩展会话模式,默认物理寻址，其他服务也是如此
        unsafe public static int changeToExtendedMode(ref ECUConfigInfo SelectECUConfig, bool FuncAddr = true)
        {
            VCI_CAN_OBJ CANMsg;
            //1.发送10 03
            byte[] data = new byte[3];
            data[0] = 0x02;
            data[1] = 0x10;
            data[2] = 0x03;
            ////Console.Write("SelectECUConfig.FuncID:{0:x}\r\n", SelectECUConfig.FuncID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);//50
                               //3.接受应答
                               ////Console.Write("{0:x}",SelectECUConfig.ReponseID);
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            return DealRespose(ref CANMsg, SelectECUConfig, 0x10);
        }

        //将ECU切换到编程会话模式,物理寻址
        unsafe public static int changeToProgrammingMode(ref ECUConfigInfo SelectECUConfig, bool FuncAddr = false)
        {
            VCI_CAN_OBJ CANMsg;
            //1.发送10 02
            byte[] data = new byte[3];
            data[0] = 0x02;
            data[1] = 0x10;
            data[2] = 0x02;
            ////Console.Write("SelectECUConfig.FuncID:{0:x}\r\n", SelectECUConfig.FuncID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);
            //3.接受应答
            ////Console.Write("SelectECUConfig.ResposeID:{0:x}\r\n", SelectECUConfig.ReponseID);
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            return DealRespose(ref CANMsg, SelectECUConfig, 0x10);
        }
        //历程控制，物理寻址
        unsafe public static int RoutineControl(ref ECUConfigInfo SelectECUConfig, ushort RID, bool FuncAddr = false)
        {
            VCI_CAN_OBJ CANMsg;
            byte[] data = new byte[5];
            data[0] = 0x04;
            data[1] = 0x31;
            data[2] = 0x01;
            data[3] = (byte)(RID >> 8);
            data[4] = (byte)(RID & 0xff);
            ////Console.Write("SelectECUConfig.RequestID:{0:x}\r\n", SelectECUConfig.RequestID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);
            //3.接受应答
            ////Console.Write("SelectECUConfig.ResposeID:{0:x}\r\n", SelectECUConfig.ReponseID);
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            if (DealRespose(ref CANMsg, SelectECUConfig, 0x31) == 0)
            {
                //Log.WriteToLog("//检查编程预条件失败\r\n");
                //Console.WriteLine("检查编程完整性失败");
                Program.form2.textBox3.Text += string.Format("检查编程预条件失败\r\n");
                return 0;
            }
            else
            {
                if (CANMsg.Data[5] != 0x00)
                {
                    //Log.WriteToLog("//检查编程完整性失败\r\n");
                    //Console.WriteLine("检查编程完整性失败");
                    Program.form2.textBox3.Text += string.Format("检查编程预条件失败\r\n");
                    return 0;
                }
                else
                {
                    //Log.WriteToLog("//检查编程完整性成功\r\n");
                    //Console.WriteLine("检查编程完整性成功");
                    Program.form2.textBox3.Text += string.Format("检查编程预条件成功\r\n");
                    return 1;
                }
            }

        }
        //控制DTC设置，功能寻址
        unsafe public static int ControlDTCSetting(ref ECUConfigInfo SelectECUConfig, byte ControlType, bool FuncAddr = true)
        {
            //VCI_CAN_OBJ CANMsg;
            //1.发送 02 10 03
            byte[] data = new byte[3];
            data[0] = 0x02;
            data[1] = 0x85;
            data[2] = (byte)(ControlType + 0x80);
            ////Console.Write("SelectECUConfig.RequestID:{0:x}\r\n", SelectECUConfig.RequestID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);
            //3.接受应答
            ////Console.Write("SelectECUConfig.ResposeID:{0:x}\r\n", SelectECUConfig.ReponseID);
            //CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            //return DealRespose(ref CANMsg, SelectECUConfig, 0x85);
            return 1;
        }
        //通信控制，功能寻址,0:使能收发 1：只收 2：只发 3：不发不收
        unsafe public static int ControlCommunication(ref ECUConfigInfo SelectECUConfig, byte CommType, byte ControlType, bool FuncAddr = true)
        {
            //VCI_CAN_OBJ CANMsg;
            //1.发送 02 10 03
            byte[] data = new byte[4];
            data[0] = 0x03;
            data[1] = 0x28;
            data[2] = (byte)(ControlType + 0x80);
            data[3] = CommType;//禁止网络管理和应用报文
                               ////Console.Write("SelectECUConfig.RequestID:{0:x}\r\n", SelectECUConfig.RequestID);
            Output((FuncAddr == true ? SelectECUConfig.FuncID : SelectECUConfig.RequestID), ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(50);
            //3.接受应答
            ////Console.Write("SelectECUConfig.ResposeID:{0:x}\r\n", SelectECUConfig.ReponseID);
            //CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            //return DealRespose(ref CANMsg, SelectECUConfig, 0x28);
            return 1;
        }
        //安全访问,Level取值：1，3，9
        unsafe public static int SecuritySession(ref ECUConfigInfo SelectECUConfig, byte Level)
        {
            //请求种子
            VCI_CAN_OBJ CANMsg;
            ulong key = 0;
            ulong seed = 0;
            Console.WriteLine("e3");
            //1.发送 02 27 level
            byte[] data = new byte[3];
            data[0] = 0x02;
            data[1] = 0x27;
            data[2] = Level;
            ////Console.Write("RequestID:{0:x},data[0]={1:x},data[1]={2:x},data[2]={3:x},\r\n", SelectECUConfig.RequestID, data[0], data[1], data[2]);
            Output(SelectECUConfig.RequestID, ref data, (uint)data.Length, SelectECUConfig);
            WaitForTimeOut(500);
            //3.接受应答
            ////Console.Write("SelectECUConfig.ResposeID:{0:x}\r\n", SelectECUConfig.ReponseID);
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            if (DealRespose(ref CANMsg, SelectECUConfig, 0x27) == 1)
            {
                //Program.form2.textBox3.Text.Text += string.Format("ResposeID = {0:x},安全访问-请求种子成功!\r\r\n", CANMsg.ID);
                //Console.Write("ResposeID = {0:x},安全访问-请求种子成功!\r\n", CANMsg.ID);
            }

            else
            {
                //Console.Write("ResposeID = {0:x},安全访问-请求种子失败!\r\n", CANMsg.ID);
                //Program.form2.textBox3.Text.Text += string.Format("ResposeID = {0:x},安全访问-请求种子失败!\r\n", CANMsg.ID);
                return 0;
            }

            //计算种子
            seed = CANMsg.Data[3];
            seed = (seed << 8) + CANMsg.Data[4];
            seed = (seed << 8) + CANMsg.Data[5];
            seed = (seed << 8) + CANMsg.Data[6];
            ////Console.Write("ResposeID = {0:x},seed = {1:x},data[0]={2:x},data[1]={3:x},data[2]={4:x},data[3]={5:x},安全访问-请求种子成功!\r\n", CANMsg.ID, seed, CANMsg.Data[3], CANMsg.Data[4], CANMsg.Data[5], CANMsg.Data[6]);
            //计算秘钥
            SecurityAlgorithm(Level, seed, SelectECUConfig.Mask, ref key);
            //发送秘钥
            byte[] keydata = new byte[7];
            keydata[0] = 0x6;
            keydata[1] = 0x27;
            keydata[2] = (byte)(Level + 1);
            keydata[6] = (byte)(key & 0xff);
            keydata[5] = (byte)((key >> 8) & 0xff);
            keydata[4] = (byte)((key >> 16) & 0xff);
            keydata[3] = (byte)((key >> 24) & 0xff);
            Output(SelectECUConfig.RequestID, ref keydata, (uint)keydata.Length, SelectECUConfig);
            WaitForTimeOut(50);
            CANMsg = WaitCANMessage(SelectECUConfig.ReponseID, 50);
            return DealRespose(ref CANMsg, SelectECUConfig, 0x27);
        }
        //对响应的处理,返回1：肯定响应 0：否定响应
        unsafe public static int DealRespose(ref VCI_CAN_OBJ RCANMsg, ECUConfigInfo ECUConfig, int ServerType)
        {


            return 0;
        }
        public static void ClearCANBuff()
        {
            if (Form2.uiDevicePara.CANDeviceType == "Kvaser")
            {

            }
            else if ((Form2.uiDevicePara.CANDeviceType == "USBCAN1") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2") || (Form2.uiDevicePara.CANDeviceType == "USBCAN2E-U"))
            {

            }
            else if ((Form2.uiDevicePara.CANDeviceType == "CANCaseXl") || (Form2.uiDevicePara.CANDeviceType == "VN1600A"))
            {
                VN1600.CANFrameBuff.Clear();
                //CANCaseXL.CANFrameQueue.Clear();
            }
            else if (Form2.uiDevicePara.CANDeviceType == "ValueCAN4")
            {
                Vspy.SpyCANFrameBuff.Clear();
            }
        }
    }
}

