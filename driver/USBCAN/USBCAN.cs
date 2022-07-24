using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using UDS上位机;
using System.Threading;

namespace UDS上位机.Driver_.USBCAN
{
    /*------------兼容ZLG的数据类型---------------------------------*/

    //1.ZLGCAN系列接口卡信息的数据类型。
    public struct VCI_BOARD_INFO
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Reserved;
    }

    /////////////////////////////////////////////////////
    //2.定义CAN信息帧的数据类型。
    unsafe public struct VCI_CAN_OBJ  //使用不安全代码
    {
        public uint ID;               //ID
        public uint TimeStamp;        //时间标识
        public byte TimeFlag;         //是否使用时间标识
        public byte SendType;         //发送标志。保留，未用
        public byte RemoteFlag;       //是否是远程帧
        public byte ExternFlag;       //是否是扩展帧
        public byte DataLen;          //数据长度
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public fixed byte Data[8];    //数据
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public fixed byte Reserved[3];//保留位
    }

    //3.定义初始化CAN的数据类型
    public struct VCI_INIT_CONFIG
    {
        public UInt32 AccCode;
        public UInt32 AccMask;
        public UInt32 Reserved;
        public byte Filter;   //0或1接收所有帧。2标准帧滤波，3是扩展帧滤波。
        public byte Timing0;  //波特率参数，具体配置，请查看二次开发库函数说明书。
        public byte Timing1;
        public byte Mode;     //模式，0表示正常模式，1表示只听模式,2自测模式
    }

    /*------------其他数据结构描述---------------------------------*/
    //4.USB-CAN总线适配器板卡信息的数据类型1，该类型为VCI_FindUsbDevice函数的返回参数。
    public struct VCI_BOARD_INFO1
    {
        public UInt16 hw_Version;
        public UInt16 fw_Version;
        public UInt16 dr_Version;
        public UInt16 in_Version;
        public UInt16 irq_Num;
        public byte can_Num;
        public byte Reserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] str_Serial_Num;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] str_hw_Type;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] str_Usb_Serial;
    }

    /*------------数据结构描述完成---------------------------------*/

    public struct CHGDESIPANDPORT
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] szpwd;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] szdesip;
        public Int32 desport;

        public void Init()
        {
            szpwd = new byte[10];
            szdesip = new byte[20];
        }
    }
    //ID过滤
    public struct VCI_FILTER_RECORD
    {
        public int ExtFrame;
        public int Start;
        public int End;
    };
    unsafe class USBCAN
    {
        //usb-e-u 波特率
        public static UInt32[] GCanBrTab = new UInt32[10]{
                        0x060003, 0x060004, 0x060007,
                        0x1C0008, 0x1C0011, 0x160023,
                        0x1C002C, 0x1600B3, 0x1C00E0,
                        0x1C01C1
         };//0x060007--500Kbps 0x1C0008--250Kbps
        /*------------兼容ZLG的函数描述---------------------------------*/
        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_OpenDevice(UInt32 DeviceType, UInt32 DeviceInd, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_CloseDevice(UInt32 DeviceType, UInt32 DeviceInd);
        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_InitCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ReadBoardInfo(UInt32 DeviceType, UInt32 DeviceInd, ref VCI_BOARD_INFO pInfo);

        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_GetReceiveNum(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_ClearBuffer(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_StartCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);
        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_ResetCAN(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd);

        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_Transmit(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pSend, UInt32 Len);

        [DllImport("controlcan.dll")]
        public static extern Int32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, ref VCI_CAN_OBJ pReceive, UInt32 Len, Int32 WaitTime);

        /*------------其他函数描述---------------------------------*/

        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_ConnectDevice(UInt32 DevType, UInt32 DevIndex);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_UsbDeviceReset(UInt32 DevType, UInt32 DevIndex, UInt32 Reserved);
        [DllImport("controlcan.dll")]
        public static extern UInt32 VCI_FindUsbDevice(ref VCI_BOARD_INFO1 pInfo);
        [DllImport("controlcan.dll")]
        // public static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, void* pData);
        public static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, ref byte pData);
        /*------------函数描述结束---------------------------------*/
        public static List<CANAPI.CANFrame> USBCANCANFrameBuff = new List<CANAPI.CANFrame>();//线程接收报文缓冲区
        public static Thread USBCANRxThread;// = new Thread(new ThreadStart(USBCANRXThreadFun));
        public static VCI_CAN_OBJ[] CANMsgs = new VCI_CAN_OBJ[2500];//暂时分配2500 
        public static VCI_FILTER_RECORD Filter = new VCI_FILTER_RECORD();
        //接收线程
        unsafe public static void USBCANRXThreadFun()
        {
            int CANFrameLenth = 0;//指示收到的报文数目

            while (true)
            {

                if ((CANFrameLenth = VCI_Receive(Form2.USBCANDevicetype, 0, (uint)(Form2.uiDevicePara.CANChanel == "CAN1" ? 0 : 1), ref CANMsgs[0], (uint)1, 0)) > 0)
                {
                    CANAPI.CANFrame CANFrame = new CANAPI.CANFrame();
                    CANFrame.data = new byte[8];
                    byte[] DMData = new byte[8];

                    for (int k = 0; k < CANFrameLenth; k++)
                    {
                        //Console.WriteLine("ACC:{0:x},ID:{1:x}",Form2.ACCCodeNum, CANMsgs[k].ID);
                        //此处设置过滤
                        if ((CANMsgs[k].ID & (~Form2.MaskNum)) == Form2.ACCCodeNum)
                        {
                            CANFrame.ID = (uint)CANMsgs[k].ID;
                            fixed (VCI_CAN_OBJ* pCANMsg = &CANMsgs[k])
                            {
                                for (int n = 0; n < CANMsgs[k].DataLen; n++)
                                {

                                    CANFrame.data[n] = pCANMsg[k].Data[n];
                                }
                            }
                            CANFrame.dlc = CANMsgs[k].DataLen;
                            USBCANCANFrameBuff.Add(CANFrame);
                            Console.WriteLine("RXThread Data[0]:{0:x},Data[1]:{1:x}", CANFrame.data[0], CANFrame.data[1]);
                        }
                    }
                }
            }
        }
        public static int USBCANGetMessage(ref CANAPI.CANFrame[] CANFrame, uint FrameNum)
        {
            int i = 0;
            //CANFrameBuff,存放所有收到的过滤后的报文
            ////Console.Write("CANCase CANFrameBuff.Count:{0}\r\n", CANFrameBuff.Count);
            for (i = 0; i < Math.Min(USBCANCANFrameBuff.Count, FrameNum); i++)
            {
                CANFrame[i].ID = USBCANCANFrameBuff[i].ID;
                CANFrame[i].dlc = USBCANCANFrameBuff[i].dlc;
                for (int k = 0; k < USBCANCANFrameBuff[i].dlc; k++)
                {
                    CANFrame[i].data[k] = USBCANCANFrameBuff[i].data[k];
                }
                USBCANCANFrameBuff.RemoveAt(i);
            }
            /*for (int k = 0; k <CANFrameBuff.Count; k++)
            {
                //Console.WriteLine("data[0]{0:x},data[1]{1:x}", CANFrameBuff[k].data[0], CANFrameBuff[k].data[1]);
            }*/
            return i;
        }
    }
}
