using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Win32.SafeHandles;
using vxlapi_NET;
using UDS上位机.Driver_.USBCAN;
using UDS上位机;
using System.Windows.Forms;
namespace UDS上位机.Driver_.VN1600
{
    class VN1600
    {
        // -----------------------------------------------------------------------------------------------
        // Global variables
        // -----------------------------------------------------------------------------------------------
        // Driver access through XLDriver (wrapper)
        public static XLDriver CANDemo = new XLDriver();
        public static String appName = "Easy-Diag";

        // Driver configuration
        private static XLClass.xl_driver_config driverConfig = new XLClass.xl_driver_config();

        // Variables required by XLDriver
        private static XLDefine.XL_HardwareType hwType = XLDefine.XL_HardwareType.XL_HWTYPE_NONE;
        private static uint hwIndex = 0;
        private static uint hwChannel = 0;
        private static int portHandle = -1;
        private static UInt64 accessMask = 0;
        private static UInt64 permissionMask = 0;
        private static UInt64 txMask = 0x0;
        private static UInt64 rxMask = 0x0;
        private static int txCi = -1;
        private static int rxCi = -1;
        private static EventWaitHandle xlEvWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, null);
        public static List<CANAPI.CANFrame> CANFrameBuff = new List<CANAPI.CANFrame>();//线程接收报文缓冲区
        //public static Queue<CANAPI.CANFrame> CANFrameQueue = new Queue<CANAPI.CANFrame>();//线程接收报文队列
        public static int CANFrameBuff_Busy = 0;
        public static Thread rxThread;// = new Thread(new ThreadStart(RXThread));
        // RX thread
        // private static Thread rxThread;
        //private static bool blockRxThread = false;
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// MAIN
        /// 
        /// Sends and receives CAN messages using main methods of the "XLDriver" class.
        /// This demo requires two connected CAN channels (Vector network interface). 
        /// The configuration is read from Vector Hardware Config (vcanconf.exe).
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        //[STAThread]
        public static int ReInit()
        {
            XLDefine.XL_Status status;
            status = CANDemo.XL_OpenDriver();
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;

            // Get XL Driver configuration
            status = CANDemo.XL_GetDriverConfig(ref driverConfig);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;

            // If the application name cannot be found in VCANCONF...
            if ((CANDemo.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                (CANDemo.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS))
            {
                //...create the item with two CAN channels
                CANDemo.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDemo.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                PrintAssignErrorAndPopupHwConf();
            }

            // Request the user to assign channels until both CAN1 (Tx) and CAN2 (Rx) are assigned to usable channels
            while (!GetAppChannelAndTestIsOk(0, ref txMask, ref txCi) || !GetAppChannelAndTestIsOk(1, ref rxMask, ref rxCi))
            {
                //return 0;
                PrintAssignErrorAndPopupHwConf();
            }

            accessMask = (txMask) | rxMask;
            permissionMask = accessMask;

            // Open port
            status = CANDemo.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;
            // Check port
            status = CANDemo.XL_CanRequestChipState(portHandle, accessMask);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;
            // Activate channel
            status = CANDemo.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;
            // Initialize EventWaitHandle object with RX event handle provided by DLL
            int tempInt = -1;
            status = CANDemo.XL_SetNotification(portHandle, ref tempInt, 1);
            xlEvWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(tempInt), true);

            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;
            // Reset time stamp clock
            status = CANDemo.XL_ResetClock(portHandle);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
                return 0;
            return 1;
        }
        public static int Init()//string[] args
        {
            XLDefine.XL_Status status;
            rxThread = new Thread(new ThreadStart(RXThread));
            //Console.WriteLine("-------------------------------------------------------------------");
            //Console.WriteLine("                     xlCANdemo.NET C# V11.0                        ");
            //Console.WriteLine("Copyright (c) 2019 by Vector Informatik GmbH.  All rights reserved.");
            //Console.WriteLine("-------------------------------------------------------------------\r\n");

            // print .NET wrapper version
            //Console.WriteLine("vxlapi_NET        : " + typeof(XLDriver).Assembly.GetName().Version);

            // Open XL Driver
            status = CANDemo.XL_OpenDriver();
            //Console.WriteLine("Open Driver       : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();


            // Get XL Driver configuration
            status = CANDemo.XL_GetDriverConfig(ref driverConfig);
            //Console.WriteLine("Get Driver Config : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();


            // Convert the dll version number into a readable string
            //Console.WriteLine("DLL Version       : " + CANDemo.VersionToString(driverConfig.dllVersion));


            // Display channel count
            //Console.WriteLine("Channels found    : " + driverConfig.channelCount);


            // Display all found channels
            for (int i = 0; i < driverConfig.channelCount; i++)
            {
                //Console.WriteLine("\r\n                   [{0}] " + driverConfig.channel[i].name, i);
                //Console.WriteLine("                    - Channel Mask    : " + driverConfig.channel[i].channelMask);
                //Console.WriteLine("                    - Transceiver Name: " + driverConfig.channel[i].transceiverName);
                //Console.WriteLine("                    - Serial Number   : " + driverConfig.channel[i].serialNumber);
            }

            // If the application name cannot be found in VCANCONF...
            if ((CANDemo.XL_GetApplConfig(appName, 0, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS) ||
                (CANDemo.XL_GetApplConfig(appName, 1, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN) != XLDefine.XL_Status.XL_SUCCESS))
            {
                //...create the item with two CAN channels
                CANDemo.XL_SetApplConfig(appName, 0, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                CANDemo.XL_SetApplConfig(appName, 1, XLDefine.XL_HardwareType.XL_HWTYPE_NONE, 0, 0, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
                PrintAssignErrorAndPopupHwConf();
            }

            // Request the user to assign channels until both CAN1 (Tx) and CAN2 (Rx) are assigned to usable channels
            while (!GetAppChannelAndTestIsOk(0, ref txMask, ref txCi) || !GetAppChannelAndTestIsOk(1, ref rxMask, ref rxCi))
            {
                // MessageBox.Show("设置失败，请重试☺", "失败",
                //           MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //return 0;
                PrintAssignErrorAndPopupHwConf();
            }

            PrintConfig();

            accessMask = (txMask) | rxMask;
            permissionMask = accessMask;

            // Open port
            status = CANDemo.XL_OpenPort(ref portHandle, appName, accessMask, ref permissionMask, 1024, XLDefine.XL_InterfaceVersion.XL_INTERFACE_VERSION, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            //Console.WriteLine("\r\n\r\nOpen Port             : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Check port
            status = CANDemo.XL_CanRequestChipState(portHandle, accessMask);
            //Console.WriteLine("Can Request Chip State: " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Activate channel
            status = CANDemo.XL_ActivateChannel(portHandle, accessMask, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN, XLDefine.XL_AC_Flags.XL_ACTIVATE_NONE);
            //Console.WriteLine("Activate Channel      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Initialize EventWaitHandle object with RX event handle provided by DLL
            int tempInt = -1;
            status = CANDemo.XL_SetNotification(portHandle, ref tempInt, 1);
            xlEvWaitHandle.SafeWaitHandle = new SafeWaitHandle(new IntPtr(tempInt), true);

            //Console.WriteLine("Set Notification      : " + status);
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();

            // Reset time stamp clock
            status = CANDemo.XL_ResetClock(portHandle);
            //Console.WriteLine("Reset Clock           : " + status + "\r\n\r\n");
            if (status != XLDefine.XL_Status.XL_SUCCESS) PrintFunctionError();
            //if (XLDefine.XL_Status.XL_SUCCESS != XLCANSetFilter(((Form2.ACCCodeNum|(0x80000000))&0xffffff00), ((Form2.ACCCodeNum | (0x80000000)) | 0xffffffff)));//(0x98DAFA00, 0x98DAFAFF))
            //  //Console.Write("Set Filter Sucess\r\n");
            // Run Rx Thread
            //Console.WriteLine("Start Rx thread...");
            Program.form2.textBox3.Text += string.Format("XL RXThread Starting..............\r\n");
            //启动接受线程
            rxThread.Start();

            // User information
            ////Console.WriteLine("Press <ENTER> to transmit CAN messages \r\n  <b>, <ENTER> to block Rx thread for rx-overrun-test \r\n  <B>, <ENTER> burst of CAN TX messages \r\n  <x>, <ENTER> to exit");

            // Transmit CAN data
            /*while (true)
            {
                if (blockRxThread) //Console.WriteLine("Rx thread blocked.");


                // Read user input
                string str = Console.ReadLine();
                if (str == "b") blockRxThread = !blockRxThread;
                else if (str == "B")
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        // Burst of CAN frames
                        CANTransmitDemo();
                    }
                }
                else if (str == "x") break;
                else
                {
                    // Send CAN frames
                    CANTransmitDemo();
                }
            }*/

            // Kill Rx thread
            //rxThread.Abort();
            ////Console.WriteLine("Close Port                     : " + CANDemo.XL_ClosePort(portHandle));
            ////Console.WriteLine("Close Driver                   : " + CANDemo.XL_CloseDriver());

            return 1;
        }
        // -----------------------------------------------------------------------------------------------
        //接收报文的线程
        unsafe private static void RXThread()
        {

            // Create new object containing received data 
            XLClass.xl_event receivedEvent = new XLClass.xl_event();
            // receivedEvent.tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;
            // Result of XL Driver function calls
            XLDefine.XL_Status xlStatus = XLDefine.XL_Status.XL_SUCCESS;
            // Note: this thread will be destroyed by MAIN
            byte[] DMData = new byte[8];

            while (true)
            {
                xlStatus = XLDefine.XL_Status.XL_SUCCESS;
                xlStatus = CANDemo.XL_Receive(portHandle, ref receivedEvent);

                if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                {

                    CANAPI.CANFrame CANFrame = new CANAPI.CANFrame();
                    CANFrame.data = new byte[8];
                    if (Form2.CANFrameType == 3)//扩展帧
                        CANFrame.ID = receivedEvent.tagData.can_Msg.id - 0x80000000;
                    else
                        CANFrame.ID = receivedEvent.tagData.can_Msg.id;
                    CANFrame.dlc = receivedEvent.tagData.can_Msg.dlc;

                    //软件过滤，收到的诊断报文
                    if ((CANFrame.ID & (~Form2.MaskNum)) == Form2.ACCCodeNum)
                    {
                        CANFrameBuff_Busy = 1;
                        for (int i = 0; i < receivedEvent.tagData.can_Msg.dlc; i++)
                        {
                            CANFrame.data[i] = receivedEvent.tagData.can_Msg.data[i];
                        }
                        //Console.WriteLine("RXThread Data[0]:{0:x},Data[1]:{1:x}", CANFrame.data[0], CANFrame.data[1]);
                        CANFrameBuff.Add(CANFrame);
                        //上锁，写入时不能读取
                        //lock(CANFrameQueue)
                        //{
                        //    List<CANAPI.CANFrame> ListCANFrame = new List<CANAPI.CANFrame>();
                        //    CANFrameQueue.Enqueue(CANFrame);
                        //    ListCANFrame = CANFrameQueue.ToList<CANAPI.CANFrame>();
                        //    for (int i = 0; i < CANFrameQueue.Count; i++)
                        //    {
                        //        Console.WriteLine("Index:{2},RXThread Data[0]:{0:x},Data[1]:{1:x}", ListCANFrame[i].data[0], ListCANFrame[i].data[1], i);
                        //    }
                        //}

                        CANFrameBuff_Busy = 0;
                    }


                    //return 1;
                }

                else if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                                == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                {
                    //Console.WriteLine("ERROR FRAME");
                    //return 0;
                }
                else if ((receivedEvent.flags & XLDefine.XL_MessageFlags.XL_EVENT_FLAG_OVERRUN) != 0)
                {
                    //Console.WriteLine("-- XL_EVENT_FLAG_OVERRUN --");
                }
                // Thread.Sleep(1);
            }
        }

        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message/exit in case of a functional call does not return XL_SUCCESS
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private static int PrintFunctionError()
        {
            //Console.WriteLine("\r\nERROR: Function call failed!\r\nPress any key to continue...");
            Console.ReadKey();
            return 0;
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Displays the Vector Hardware Configuration.
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        private static void PrintConfig()
        {
            //Console.WriteLine("\r\n\r\nAPPLICATION CONFIGURATION");

            foreach (int channelIndex in new int[] { txCi, rxCi })
            {
                //Console.WriteLine("-------------------------------------------------------------------");
                //Console.WriteLine("Configured Hardware Channel : " + driverConfig.channel[channelIndex].name);
                //Console.WriteLine("Hardware Driver Version     : " + CANDemo.VersionToString(driverConfig.channel[channelIndex].driverVersion));
                //Console.WriteLine("Used Transceiver            : " + driverConfig.channel[channelIndex].transceiverName);
            }

            //Console.WriteLine("-------------------------------------------------------------------\r\n");
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Error message if channel assignment is not valid and popup VHwConfig, so the user can correct the assignment
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public static void PrintAssignErrorAndPopupHwConf()
        {
            //Console.WriteLine("\r\nPlease check application settings of \"" + appName + " CAN1/CAN2\",\r\nassign them to available hardware channels and press enter.");
            CANDemo.XL_PopupHwConfig(null, 300000);//等5min
            //Console.ReadKey();
        }
        // -----------------------------------------------------------------------------------------------
        //设置硬件过滤
        private static XLDefine.XL_Status XLCANSetFilter(uint first_id, uint last_id)
        {
            XLDefine.XL_Status xlStatus;
            //Form2.MaskNum,Form2.ACCCodeNum|0x80000000,第四个参数，0是不管,1是要一致，txmask可能也是一样
            xlStatus = CANDemo.XL_CanSetChannelAcceptance(portHandle, accessMask, Form2.ACCCodeNum | 0x80000000, ~(Form2.MaskNum), XLDefine.XL_AcceptanceFilter.XL_CAN_EXT);
            xlStatus = CANDemo.XL_CanAddAcceptanceRange(portHandle, accessMask, first_id, last_id);
            return xlStatus;
        }
        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Retrieve the application channel assignment and test if this channel can be opened
        /// </summary>
        // -----------------------------------------------------------------------------------------------
        public static bool GetAppChannelAndTestIsOk(uint appChIdx, ref UInt64 chMask, ref int chIdx)
        {
            XLDefine.XL_Status status = CANDemo.XL_GetApplConfig(appName, appChIdx, ref hwType, ref hwIndex, ref hwChannel, XLDefine.XL_BusTypes.XL_BUS_TYPE_CAN);
            if (status != XLDefine.XL_Status.XL_SUCCESS)
            {
                //Console.WriteLine("XL_GetApplConfig      : " + status);
                PrintFunctionError();
            }

            chMask = CANDemo.XL_GetChannelMask(hwType, (int)hwIndex, (int)hwChannel);
            chIdx = CANDemo.XL_GetChannelIndex(hwType, (int)hwIndex, (int)hwChannel);
            if (chIdx < 0 || chIdx >= driverConfig.channelCount)
            {
                // the (hwType, hwIndex, hwChannel) triplet stored in the application configuration does not refer to any available channel.
                return false;
            }

            // test if CAN is available on this channel
            return (driverConfig.channel[chIdx].channelBusCapabilities & XLDefine.XL_BusCapabilities.XL_BUS_ACTIVE_CAP_CAN) != 0;
        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// Sends some CAN messages.
        /// </summary>
        // ----------------------------------------------------------------------------------------------- 
        public static int CANTransmit(uint CANID, ref byte[] Data, byte DLC)
        {
            XLDefine.XL_Status txStatus;

            // Create an event collection with 2 messages (events)
            XLClass.xl_event_collection xlEventCollection = new XLClass.xl_event_collection(1);

            if (Form2.CANFrameType == 2)//标准帧
                xlEventCollection.xlEvent[0].tagData.can_Msg.id = CANID;
            else if (Form2.CANFrameType == 3)//扩展帧
                xlEventCollection.xlEvent[0].tagData.can_Msg.id = CANID | 0x80000000;
            xlEventCollection.xlEvent[0].tagData.can_Msg.dlc = DLC;//(ushort)lenth;
            for (int i = 0; i < 8; i++)
            {
                xlEventCollection.xlEvent[0].tagData.can_Msg.data[i] = Data[i];
            }

            xlEventCollection.xlEvent[0].tag = XLDefine.XL_EventTags.XL_TRANSMIT_MSG;


            // Transmit events
            txStatus = CANDemo.XL_CanTransmit(portHandle, txMask, xlEventCollection);
            ////Console.WriteLine("Transmit Message      : " + txStatus);
            if (txStatus != XLDefine.XL_Status.XL_SUCCESS)
            {
                return 0;
            }
            return 1;

        }
        // -----------------------------------------------------------------------------------------------




        // -----------------------------------------------------------------------------------------------
        /// <summary>
        /// EVENT THREAD (RX)
        /// 
        /// RX thread waits for Vector interface events and displays filtered CAN messages.
        /// </summary>

        // ----------------------------------------------------------------------------------------------- 
        //返回读取缓冲区的报文数
        public static int CANRecv(ref CANAPI.CANFrame[] CANFrames, uint lenth)
        {
            int i = 0;
            //CANFrameBuff,存放所有收到的过滤后的报文
            //将缓冲区的数据全部输出
            ////Console.Write("CANCase CANFrameBuff.Count:{0}\r\n", CANFrameBuff.Count);
            for (i = 0; i < Math.Min(CANFrameBuff.Count, lenth); i++)
            {
                while (CANFrameBuff_Busy == 1) ;
                CANFrames[i].ID = CANFrameBuff[i].ID;
                CANFrames[i].dlc = CANFrameBuff[i].dlc;
                for (int k = 0; k < CANFrameBuff[i].dlc; k++)
                {
                    CANFrames[i].data[k] = CANFrameBuff[i].data[k];
                }
                //Console.Write("CANCase CANFrameBuff[{0}].Data[0]:{1:x},Data[1]:{2:x}\r\n", i, CANFrameBuff[i].data[0], CANFrameBuff[i].data[1]);
                CANFrameBuff.RemoveAt(i);
            }
            //上锁，读的时候不能写
            //lock (CANFrameQueue)
            //{
            //    long k = Math.Min(CANFrameQueue.Count, lenth);
            //    for (i = 0; i < k; i++)
            //    {
            //        while (CANFrameBuff_Busy == 1) ;
            //        CANFrames[i] = CANFrameQueue.Dequeue();
            //    }
            //}

            /*for (int k = 0; k <CANFrameBuff.Count; k++)
            {
                //Console.WriteLine("data[0]{0:x},data[1]{1:x}", CANFrameBuff[k].data[0], CANFrameBuff[k].data[1]);
            }*/
            return i;
            // afterwards: while hw queue is not empty...
            /* while (xlStatus != XLDefine.XL_Status.XL_ERR_QUEUE_IS_EMPTY)
             {
                 // ...block RX thread to generate RX-Queue overflows
                 while (blockRxThread) { Thread.Sleep(1000); }

                 // ...receive data from hardware.

                 //  If receiving succeed....
                 if (xlStatus == XLDefine.XL_Status.XL_SUCCESS)
                 {
                     if ((receivedEvent.flags & XLDefine.XL_MessageFlags.XL_EVENT_FLAG_OVERRUN) != 0)
                     {
                         //Console.WriteLine("-- XL_EVENT_FLAG_OVERRUN --");
                     }

                     // ...and data is a Rx msg...
                     if (receivedEvent.tag == XLDefine.XL_EventTags.XL_RECEIVE_MSG)
                     {
                         if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_OVERRUN) != 0)
                         {
                             //Console.WriteLine("-- XL_CAN_MSG_FLAG_OVERRUN --");
                         }

                         // ...check various flags
                         if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                             == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_ERROR_FRAME)
                         {
                             //Console.WriteLine("ERROR FRAME");
                         }

                         else if ((receivedEvent.tagData.can_Msg.flags & XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                             == XLDefine.XL_MessageFlags.XL_CAN_MSG_FLAG_REMOTE_FRAME)
                         {
                             //Console.WriteLine("REMOTE FRAME");
                         }

                         else
                         {
                             //Console.WriteLine(CANDemo.XL_GetEventString(receivedEvent));
                         }
                     }
                 }*/
        }

    }
    // -----------------------------------------------------------------------------------------------
}

