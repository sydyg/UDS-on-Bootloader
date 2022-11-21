using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UDS上位机;
namespace UDS上位机
{
    //UI设备参数
    public struct UI_DevicePara
    {
        public string CANDeviceType;//设备类型
        public string CANBaudRate;//波特率
        public string CANChanel;//CANChanel
    };
    //UI里面的BT参数
    public struct UI_BTPara
    {
        public string APPFile1Name; //appfilename1
        public string APPFile2Name;//appfilename2
        public string BTFileName;  //btfilename
        public string BinFileName;  //binfilename
        public string SelectedECUName;//SelectedECU             
        public int EraseMemType;//擦内存的方式，0：分段，1：整块
        public bool CheckCRC;
        public byte HasApp1;
        public byte HasApp2;
        public byte HasDri;
        public byte HasBin1;
        public byte FastDown;
        public long BinFileStartAddress;//Bin文件起始地址
        public long BinFileLenth;//Bin文件长度
        public byte SkapPrepProcessFlag;//跳过编程预条件
    };

    public struct ECUConfigInfo
    {
        public string ECUName;
        public uint RequestID;
        public uint ReponseID;
        public uint FuncID;
        public ulong Mask;
    };
    public partial class Form2 : Form
    {
        public static Boolean StandardFrame = true;
        public static int CANFrameType = 3;//扩展帧
        public static Boolean DeviceOpen = false;
        public static UI_BTPara uiBTPara;
        const int VCI_PCI5121 = 1;
        const int VCI_PCI9810 = 2;
        const int VCI_USBCAN1 = 3;
        const int VCI_USBCAN2 = 4;
        const int VCI_USBCAN2A = 4;
        const int VCI_PCI9820 = 5;
        const int VCI_CAN232 = 6;
        const int VCI_PCI5110 = 7;
        const int VCI_CANLITE = 8;
        const int VCI_ISA9620 = 9;
        const int VCI_ISA5420 = 10;
        const int VCI_PC104CAN = 11;
        const int VCI_CANETUDP = 12;
        const int VCI_CANETE = 12;
        const int VCI_DNP9810 = 13;
        const int VCI_PCI9840 = 14;
        const int VCI_PC104CAN2 = 15;
        const int VCI_PCI9820I = 16;
        const int VCI_CANETTCP = 17;
        const int VCI_PEC9920 = 18;
        const int VCI_PCI5010U = 19;
        const int VCI_USBCAN_E_U = 20;
        const int VCI_USBCAN_2E_U = 21;
        const int VCI_PCI5020U = 22;
        const int VCI_EG20T_CAN = 23;
        public static uint ACCCodeNum = 0x0;//过滤ACC码
        public static uint MaskNum = 0x00000fff;//Mask,0表示必须一致
        public static int startStatus = 0;
        public static ECUConfigInfo[] ECUConfigArray;//ECU配置信息的数组，最大40
        public static UI_DevicePara uiDevicePara = new UI_DevicePara { CANDeviceType = "USBCAN2EU", CANBaudRate = "500K", CANChanel = "CAN1" };
        public static ECUConfigInfo CurrentECUConfig = new ECUConfigInfo() { ECUName = "ECU", RequestID = 0x127, ReponseID = 0x128, Mask = 0x400400 };//选中的ECU的BT配置信息
        public static System.Windows.Forms.Timer Timer_WaitTimeOut;//延时定时器
        public static uint TimerWaitTimeout = 1;//超时标志置1
        public static uint TimerWaitTimeOutCount = 0;//时间计数置0
        public static uint TimerWaitTimeOutNum = 0;//超时时间
        public static System.Windows.Forms.Timer Timer_3E80;//3E80服务定时器
        public static int Mutex3E80 = 0;//3E80的互斥量，避免发送冲突
        public static uint USBCANDevicetype = 4;//ZLG系列USBCAN设备号
        public Form2()
        {
            InitializeComponent();
            components = new System.ComponentModel.Container();
            Timer_3E80 = new Timer(this.components);
            Timer_3E80.Enabled = false;
            Timer_3E80.Interval = 2000;
            Timer_3E80.Tick += new EventHandler(Timer3E80_Tick);
        }
        public void WritetoMsg(string msg)
        {
            this.textBox3.Text += string.Format(msg);
        }
        //USBCAN
        //USBCAN1
        private void uSBCAN1ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANDeviceType = "USBCAN1";
        }
        //USBCAN2
        private void uSBCAN2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANDeviceType = "USBCAN2";
        }
        //USBCAN2EU
        private void uSBCAN2EUToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANDeviceType = "USBCAN2EU";
        }

        private void pCANToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANDeviceType = "PCAN";
        }

        private void vN1600ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANDeviceType = "VN1600";
        }

        private void vspyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANDeviceType = "Vspy";
        }

        private void kvaserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.uSBCANToolStripMenuItem.Font = new Font(this.uSBCANToolStripMenuItem.Font, this.uSBCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN1ToolStripMenuItem.Font = new Font(this.uSBCAN1ToolStripMenuItem.Font, this.uSBCAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2ToolStripMenuItem.Font = new Font(this.uSBCAN2ToolStripMenuItem.Font, this.uSBCAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.uSBCAN2EUToolStripMenuItem.Font = new Font(this.uSBCAN2EUToolStripMenuItem.Font, this.uSBCAN2EUToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.pCANToolStripMenuItem.Font = new Font(this.pCANToolStripMenuItem.Font, this.pCANToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vN1600ToolStripMenuItem.Font = new Font(this.vN1600ToolStripMenuItem.Font, this.vN1600ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.vspyToolStripMenuItem.Font = new Font(this.vspyToolStripMenuItem.Font, this.vspyToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kvaserToolStripMenuItem.Font = new Font(this.kvaserToolStripMenuItem.Font, this.kvaserToolStripMenuItem.Font.Style | FontStyle.Bold);
            uiDevicePara.CANDeviceType = "Kvaser";
        }
        //CAN1
        private void cAN1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.cAN1ToolStripMenuItem.Font = new Font(this.cAN1ToolStripMenuItem.Font, this.cAN1ToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.cAN2ToolStripMenuItem.Font = new Font(this.cAN2ToolStripMenuItem.Font, this.cAN2ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANChanel = "CAN1";
        }
        //CAN2
        private void cAN2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.cAN1ToolStripMenuItem.Font = new Font(this.cAN1ToolStripMenuItem.Font, this.cAN1ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.cAN2ToolStripMenuItem.Font = new Font(this.cAN2ToolStripMenuItem.Font, this.cAN2ToolStripMenuItem.Font.Style | FontStyle.Bold);
            uiDevicePara.CANChanel = "CAN2";
        }
        //250K
        private void kToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.kToolStripMenuItem.Font = new Font(this.kToolStripMenuItem.Font, this.kToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.kToolStripMenuItem1.Font = new Font(this.kToolStripMenuItem1.Font, this.kToolStripMenuItem1.Font.Style & ~FontStyle.Bold);
            this.kToolStripMenuItem2.Font = new Font(this.kToolStripMenuItem2.Font, this.kToolStripMenuItem2.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANBaudRate = "250K";
        }
        //500K
        private void kToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.kToolStripMenuItem.Font = new Font(this.kToolStripMenuItem.Font, this.kToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kToolStripMenuItem1.Font = new Font(this.kToolStripMenuItem1.Font, this.kToolStripMenuItem1.Font.Style | FontStyle.Bold);
            this.kToolStripMenuItem2.Font = new Font(this.kToolStripMenuItem2.Font, this.kToolStripMenuItem2.Font.Style & ~FontStyle.Bold);
            uiDevicePara.CANBaudRate = "500K";
        }
        //1000K
        private void kToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            this.kToolStripMenuItem.Font = new Font(this.kToolStripMenuItem.Font, this.kToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.kToolStripMenuItem1.Font = new Font(this.kToolStripMenuItem1.Font, this.kToolStripMenuItem1.Font.Style & ~FontStyle.Bold);
            this.kToolStripMenuItem2.Font = new Font(this.kToolStripMenuItem2.Font, this.kToolStripMenuItem2.Font.Style | FontStyle.Bold);
            uiDevicePara.CANBaudRate = "1000K";
        }
        //标准帧
        private void 标准帧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.标准帧ToolStripMenuItem.Font = new Font(this.标准帧ToolStripMenuItem.Font, this.标准帧ToolStripMenuItem.Font.Style | FontStyle.Bold);
            this.扩展帧ToolStripMenuItem.Font = new Font(this.扩展帧ToolStripMenuItem.Font, this.扩展帧ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            StandardFrame = true;
            CANFrameType = 2;//扩展帧
        }
        //扩展帧
        private void 扩展帧ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.标准帧ToolStripMenuItem.Font = new Font(this.标准帧ToolStripMenuItem.Font, this.标准帧ToolStripMenuItem.Font.Style & ~FontStyle.Bold);
            this.扩展帧ToolStripMenuItem.Font = new Font(this.扩展帧ToolStripMenuItem.Font, this.扩展帧ToolStripMenuItem.Font.Style | FontStyle.Bold);
            StandardFrame = false;
            CANFrameType = 3;//扩展帧
        }
        //启动设备
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CANAPI.CANOpenDevice();
            DeviceOpen = true;
        }
        //关闭设备
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (DeviceOpen == true)
                CANAPI.CANCloseDevice();
            else
                MessageBox.Show("设备未打开");
            DeviceOpen = false;
        }
        private void Timer3E80_Tick(object sender, EventArgs e)
        {
            byte[] data3e = new byte[] { 0x02, 0x3E, 0x80 };
            //while (Mutex3E80 != 0) ;//添加自旋锁
            CANAPI.Output(CurrentECUConfig.FuncID, ref data3e, (uint)data3e.Length, CurrentECUConfig);

        }
        //APP
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog APPFile = new OpenFileDialog();
            APPFile.Filter = "S19|*.s19|hex|*.hex|all|*.*";
            APPFile.ShowDialog();
            this.textBox1.Text = "";
            this.textBox1.Text = APPFile.FileName;
        }
        //Driver
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog DriverFile = new OpenFileDialog();
            DriverFile.Filter = "S19|*.s19|hex|*.hex|all|*.*";
            DriverFile.ShowDialog();
            this.textBox2.Text = "";
            this.textBox2.Text = DriverFile.FileName;
        }
        //开始刷写
        private void button3_Click(object sender, EventArgs e)
        {
            if (Form2.DeviceOpen == false)
            {
                MessageBox.Show("未启动设备！", "失败",
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Bootloader BT = new Bootloader();
            //加载ECU配置
            BT.SelectECUConfig.ECUName = "ECU";
            BT.SelectECUConfig.RequestID = 0x127;
            BT.SelectECUConfig.ReponseID = 0x128;
            BT.SelectECUConfig.FuncID = 0x128;
            BT.SelectECUConfig.Mask = 0x400400;
            //解析app文件
            //S19文件
            if (this.textBox1.Text.EndsWith("S19") || this.textBox1.Text.EndsWith("s19"))
            {
                //Console.Write("\r\nS19 APPFile1\r\n");
                //MainProcss.mainui.BTStatetextBox.Text += string.Format("\r\nS19 APPFile1\r\n");
                BT.APP1FileType = 0;
                if (1 != BT.ParseS19file(this.textBox1.Text, ref BT.s19APP1BlockList))
                {
                    MessageBox.Show("解析APP刷写文件失败，检查文件！", "失败",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //统计数据长度
                for (int i = 0; i < BT.s19APP1BlockList.Count; i++)
                {
                    //Console.Write("Address:{0:x}\r\n", BT.s19APP1BlockList[i].address);
                    //Console.Write("Lenth:{0:d}\r\n", BT.s19APP1BlockList[i].data.Count);
                    //MainProcss.mainui.BTStatetextBox.Text += string.Format("Address:{0:x}\r\n", BT.s19APP1BlockList[i].address);
                    //MainProcss.mainui.BTStatetextBox.Text += string.Format("Lenth:{0:d}\r\n", BT.s19APP1BlockList[i].data.Count);
                    //Log.WriteToLog(String.Format("Address:{0:x}\r\n", BT.s19APP1BlockList[i].address));
                    //Log.WriteToLog(String.Format("Lenth:{0:d}\r\n", BT.s19APP1BlockList[i].data.Count));
                    TotalData += BT.s19APP1BlockList[i].data.Count;
                    /*foreach (byte d in BT.s19APP1BlockList[i].data)
                    {
                        //Console.Write("{0:x} ",d);
                    }*/
                    ////Console.Write("\r\r\n");
                }
            }
            //hex文件
            else if (this.textBox1.Text.EndsWith("hex") || this.textBox1.Text.EndsWith("HEX"))
            {
                //Console.Write("Hex APPFile1\r\n");
                //MainProcss.mainui.BTStatetextBox.Text += string.Format("Hex APPFile1\r\n");
                ////Console.Write("HasApp1:{0}", MainUI.uiBTPara.APPFile1Name);
                BT.APP1FileType = 1;
                if (1 != BT.ParseHexFile(this.textBox1.Text, ref BT.HexAPP1BlockList))
                {
                    MessageBox.Show("解析APP刷写文件失败，检查文件！", "失败",
                                MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                //统计数据长度
                for (int i = 0; i < BT.HexAPP1BlockList.Count; i++)
                {
                    //Console.Write("Address:{0:x}\r\n", BT.HexAPP1BlockList[i].address);
                    //Console.Write("Lenth:{0:d}\r\n", BT.HexAPP1BlockList[i].data.Count);
                    //Log.WriteToLog(String.Format("Address:{0:x}\r\n", BT.HexAPP1BlockList[i].address));
                    //Log.WriteToLog(String.Format("Lenth:{0:d}\r\n", BT.HexAPP1BlockList[i].data.Count));
                    TotalData += BT.HexAPP1BlockList[i].data.Count;
                    /*foreach (byte d in BT.HexAPP1BlockList[i].data)
                    {
                        //Console.Write("{0:x} ",d);
                    }
                    //Console.Write("\r\r\n");*/
                }
            }
            //解析driver文件，可为空
            if (this.textBox2.Text != "")
            {
                //S19文件
                if (this.textBox2.Text.EndsWith("S19") || this.textBox2.Text.EndsWith("s19"))
                {
                    //Console.Write("\r\nS19 DriFile\r\n");
                    ////Console.Write("S19........................");
                    ////Console.Write("HasBT:{0}", MainUI.uiBTPara.BTFileName);
                    BT.DriFileType = 0;
                    if (1 != BT.ParseS19file(this.textBox2.Text, ref BT.s19DriBlockList))
                    {
                        MessageBox.Show("解析Driver刷写文件失败，检查文件！", "失败",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //统计数据长度
                    for (int i = 0; i < BT.s19DriBlockList.Count; i++)
                    {
                        //Console.Write("Address:{0:x}\r\n", BT.s19DriBlockList[i].address);
                        //Console.Write("Lenth:{0:d}\r\n", BT.s19DriBlockList[i].data.Count);
                        //Log.WriteToLog(String.Format("Address:{0:x}\r\n", BT.s19DriBlockList[i].address));
                        //Log.WriteToLog(String.Format("Lenth:{0:d}\r\n", BT.s19DriBlockList[i].data.Count));
                        TotalData += BT.s19DriBlockList[i].data.Count;
                        /*foreach (byte d in BT.HexDriBlockList[i].data)
                        {
                            //Console.Write("{0:x} ", d);
                        }*/
                    }
                }
                else if (this.textBox2.Text.EndsWith("hex") || this.textBox2.Text.EndsWith("HEX"))
                {
                    //Console.Write("\r\nHex DriFile\r\n");
                    ////Console.Write("HasApp1:{0}", MainUI.uiBTPara.APPFile1Name);
                    BT.DriFileType = 1;
                    if (1 != BT.ParseHexFile(this.textBox2.Text, ref BT.HexDriBlockList))
                    {
                        MessageBox.Show("解析Driver刷写文件失败，检查文件！", "失败",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    //统计数据长度
                    for (int i = 0; i < BT.HexDriBlockList.Count; i++)
                    {
                        //Console.Write("Address:{0:x}\r\n", BT.HexDriBlockList[i].address);
                        //Console.Write("Lenth:{0:d}\r\n", BT.HexDriBlockList[i].data.Count);
                        //Log.WriteToLog(String.Format("Address:{0:x}\r\n", BT.HexDriBlockList[i].address));
                        //Log.WriteToLog(String.Format("Lenth:{0:d}\r\n", BT.HexDriBlockList[i].data.Count));
                        TotalData += BT.HexDriBlockList[i].data.Count;
                        /*foreach (byte d in BT.HexDriBlockList[i].data)
                        {
                            //Console.Write("{0:x} ", d);
                        }*/
                        ////Console.Write("\r\r\n");
                    }
                }
            }
            Form2.Timer_3E80.Enabled = true;//启动3E80服务
            BT.BTThread.Start();
        }

    }
}
