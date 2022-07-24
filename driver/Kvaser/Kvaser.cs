using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kvaser.Kvrlib;
using canlibCLSNET;
using System.Threading;
using System.Windows.Forms;

namespace UDS上位机.Driver_.Kvaser
{
    class Driver_Kvaser
    {
        //打开kvaser
        public static int OpenKvaser()
        {
            Kvrlib.Status status;
            Kvrlib.DiscoveryHnd discoveryHandle;
            String buf = "";
            Canlib.canInitializeLibrary();
            Kvrlib.InitializeLibrary();
            status = Kvrlib.DiscoveryOpen(out discoveryHandle);
            //Console.WriteLine("{0}",discoveryHandle);
            if (status.Equals(Kvrlib.Status.OK))
            {
                int channel_count;
                Canlib.canStatus status1;
                status1 = Canlib.canGetNumberOfChannels(out channel_count);
                if (!status1.Equals(Canlib.canStatus.canOK))
                {
                    //Console.WriteLine("ERROR: canGetNumberOfChannels failed " + status);
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("DiscoveryOpen() FAILED - " + buf);
                return -1;
            }
            status = SetupBroadcast(discoveryHandle);
            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("SetupBroadcast() FAILED - " + buf);
                return -2;
            }

            status = Kvrlib.DiscoveryClearDevicesAtExit(true);
            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("DiscoveryClearDevicesAtExit() FAILED - " + buf);
                return -4;
            }
            status = Kvrlib.DiscoveryClearDevicesAtExit(false);
            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("DiscoveryClearDevicesAtExit() FAILED - " + buf);
                return -5;
            }

            status = Kvrlib.DiscoveryClose(discoveryHandle);
            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("DiscoveryClose() FAILED - " + buf);
                return -6;
            }
            Kvrlib.UnloadLibrary();
            return 1;
        }

        /*
         * Sets up the addresses for discovery
         */
        static Kvrlib.Status SetupBroadcast(Kvrlib.DiscoveryHnd discoveryHandle)
        {
            string buf = "";
            Kvrlib.Status status;
            Kvrlib.Address[] addr_list = new Kvrlib.Address[20];
            int no_addrs = 0;

            status = Kvrlib.DiscoveryGetDefaultAddresses(out addr_list, Kvrlib.AddressTypeFlag.ALL);

            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("kvrDiscoveryGetDefaultAddresses() FAILED - " + buf);
                return status;
            }

            //Add an address to the array (strictly not necessary, just for demonstration purposes)
            Array.Resize<Kvrlib.Address>(ref addr_list, addr_list.Length + 1);
            String tmp_addr = "192.168.3.66";
            status = Kvrlib.AddressFromString(Kvrlib.AddressType.IPV4, out addr_list[addr_list.Length - 1], tmp_addr);
            if (!status.Equals(Kvrlib.Status.OK))
            {
                //Console.WriteLine("ERROR: kvrAddressFromString(" + no_addrs + ", " + tmp_addr + ") failed");
                return status;
            }

            for (int i = 0; i < addr_list.Length; i++)
            {
                status = Kvrlib.StringFromAddress(out buf, addr_list[i]);
                //Console.WriteLine("Looking for device using: " + buf);
            }

            status = Kvrlib.DiscoverySetAddresses(discoveryHandle, addr_list);
            if (!status.Equals(Kvrlib.Status.OK))
            {
                Kvrlib.GetErrorText(status, out buf);
                //Console.WriteLine("DiscoverySetAddresses() FAILED - " + buf);
                return status;
            }
            return status;
        }

        /*
     ** Check a status code and issue an error message if the code isn't canOK.
     */
        public static int ErrorDump(string id, Canlib.canStatus stat, bool quit)
        {
            string buf = "";
            if (stat != Canlib.canStatus.canOK)
            {
                Canlib.canGetErrorText(stat, out buf);
                //Console.WriteLine("{0}: failed, stat={1} ({2})", id, (int)stat, buf);
                //Thread.Sleep(5000);
                //if (quit) Environment.Exit(1);
                return 0;
            }
            return 1;
        }  // end of ErrorDump()

        /*
        **  Prepare channel regardless of method
        */
        //初始化通道，选择通道，设置波特率
        public static int InitChannel(int chNum, ref int chHndl, int BaudRate)
        {
            Canlib.canStatus status;
            // Because we are working on timing we want the channel exclusive
            //chHndl = Canlib.canOpenChannel(chNum, Canlib.canOPEN_REQUIRE_INIT_ACCESS);可用
            chHndl = Canlib.canOpenChannel(chNum, Canlib.canOPEN_REQUIRE_INIT_ACCESS);
            if (chHndl < 0)
            {
                int IsError = ErrorDump("canOpenChannel", (Canlib.canStatus)(chHndl), true);
                if (IsError == 0)
                    return -1;
            }
            //设置波特率
            status = Canlib.canSetBusParams(chHndl, BaudRate, 0, 0, 0, 0, 0);
            int IsError2 = ErrorDump("canSetBusParams", status, true);
            if (IsError2 == 0)
                return -2;
            //屏蔽
            int AccCode = (int)(Form2.ACCCodeNum);//设置验收码unchecked((uint)(0x18D00000<<3))
            int AccMask = Convert.ToInt32(Form2.MaskNum);//设置屏蔽码（0：相关、1：无关）
            Canlib.canSetAcceptanceFilter(chHndl, AccCode, ~AccMask, 1);//Kvaser的屏蔽码0：无关，1相关
            //Takes the channel on bus
            ////Console.WriteLine("Going on bus");
            status = Canlib.canBusOn(chHndl);
            CheckStatus(status, "canBusOn");
            return chHndl;
            /*
            // decided to use the default 500 kBits/sec
            status = Canlib.canSetBusParams(chHndl, Canlib.canBITRATE_500K, 0, 0, 0, 0, 0);
            ErrorDump("canSetBusParams", status, true);
            */
        }  // end of InitChannel()

        /*
        **  Shuts down an open channel
        */
        //关闭一个通道
        public static int CloseChannel(int chHndl)
        {
            Canlib.canStatus status;
            chHndl = 0;
            // take the channel offline
            status = Canlib.canBusOff(chHndl);
            int IsError = ErrorDump("canBusOff", status, false);

            // free the channel handle
            status = Canlib.canClose(chHndl);
            int IsError2 = ErrorDump("canClose", status, false);
            if (IsError == 0 | IsError2 == 0)
            {
                return 0;
            }
            return 1;
        }  // end of CloseChannel()

        public static int SendMessage(ref USBCAN.VCI_CAN_OBJ dataSend, byte[] Data, int flag = 4)
        {
            //int handle;
            int chHndl = CANAPI.chHndl;
            int id = Convert.ToInt32(dataSend.ID);
            int lenth = Convert.ToInt32(dataSend.DataLen);
            Canlib.canStatus status;
            //Send a message to the channel
            status = Canlib.canWrite(chHndl, id, Data, lenth, 4);//flag=4为扩展帧，flag=2为标准帧,错误帧为32，远程帧为1
            if (status < 0)
            {
                return 0;
            }
            return 1;
        }

        //接收数据
        public static int DumpMessageLoop(ref CANAPI.CANFrame[] CANFrames, int handle, uint timeout = 10, uint FrameNum = 0, uint CANID = 0)
        {
            Canlib.canStatus status;
            bool finished = false;
            //These variables hold the incoming message
            byte[] data = new byte[8];
            int id;
            int dlc;
            int flags;
            long time;
            string id16;
            int i = 0;
            while (!finished)
            {
                //Wait for up to 100 ms for a message on the channel
                status = Canlib.canReadWait(handle, out id, data, out dlc, out flags, out time, 1);
                //status = Canlib.canRead(handle, out id, data, out dlc, out flags, out time);
                id16 = Convert.ToString(id, 16);
                if (CANID == Convert.ToUInt32(id16, 16))
                {
                    CANFrames[0].ID = Convert.ToUInt32(id16, 16);
                    CANFrames[0].dlc = Convert.ToUInt16(dlc);
                    CANFrames[0].data = data;
                    return 1;
                }
                if (i == timeout)
                {
                    return 0;
                }
                i++;
            }
            return 0;
        }
        //接收数据显示子函数
        /*public static void DumpMessage(int id, byte[] data, int dlc, int flags, long time)
        {
            if ((flags & Canlib.canMSG_ERROR_FRAME) != 0)
            {
                //Console.WriteLine("Error Frame received ****");
            }
            else
            {
                //Console.WriteLine("{0}  {1}  {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}    {10}",
                                                 id, dlc, data[0], data[1], data[2], data[3], data[4],
                                                 data[5], data[6], data[7], time);
            }
        }*/
        /*
        public static void Sendmessage1(string[] args)
        {
            int handle;
            byte[] message = { 0, 1, 2, 3, 4, 5, 6, 7 };
            Canlib.canStatus status;

            //Initializes library so we can call Canlib
            //Console.WriteLine("Initializing Canlib");
            Canlib.canInitializeLibrary();

            //Gets a handle to channel 0
            //Console.WriteLine("Opening channel 0");
            handle = Canlib.canOpenChannel(0, Canlib.canOPEN_ACCEPT_VIRTUAL);
            CheckStatus((Canlib.canStatus)handle, "canSetBusParams");

            //Sets the bitrate for the bus to 250 kb/s
            //Console.WriteLine("Setting channel bitrate");
            status = Canlib.canSetBusParams(handle, Canlib.canBITRATE_250K, 0, 0, 0, 0, 0);
            CheckStatus(status, "canSetBusParams");

            //Takes the channel on bus
            //Console.WriteLine("Going on bus");
            status = Canlib.canBusOn(handle);
            CheckStatus(status, "canBusOn");

            //Send a message to the channel
            //Console.WriteLine("Writing a message to the channel");
            status = Canlib.canWrite(handle, 123, message, 8, 0);
            CheckStatus(status, "canWrite");

            //Wait until the message is sent
            //Console.WriteLine("Waiting for the message to be transmitted");
            status = Canlib.canWriteSync(handle, 1000);
            CheckStatus(status, "canWriteSync");

            //Takes the channel off bus
            //Console.WriteLine("Going off bus");
            status = Canlib.canBusOff(handle);
            CheckStatus(status, "canBusOff");

            //Closes the channel
            //Console.WriteLine("Closing channel 0");
            status = Canlib.canClose(handle);
            CheckStatus(status, "canClose");

            //Wait for the user to press a key before exiting, in case the console closes automatically on exit.
            //Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }*/

        //This method prints an error if something goes wrong
        private static void CheckStatus(Canlib.canStatus status, string method)
        {
            if (status < 0)
            {
                string errorText;
                Canlib.canGetErrorText(status, out errorText);
                //Console.WriteLine(method + " failed: " + errorText);
            }
        }


        /*public static void Init()
        {
            Canlib.canStatus status;
            int txChannel = 0;
            int rxChannel = 1;
            int BaudRate = -2;//LI,临时
            bool mmPrecision = false;
            // prepare the library
            Canlib.canInitializeLibrary();
            InitChannel(txChannel, ref txChannel, BaudRate);
            Canlib.canOpenChannel(0, Canlib.canOPEN_ACCEPT_VIRTUAL);
            // open the receive channel
            InitChannel(rxChannel, ref rxChanHndl, BaudRate);
            Canlib.canOpenChannel(1, Canlib.canOPEN_ACCEPT_VIRTUAL);
             /*for the receive channel we want to up the resolution returned on
         ** on the time stamp to 1/10 of a millisecond.  This changes the resolution,
         ** but the accuracy still depends on the Kvaser hardware used.*/
        /*
           object timeObj = (UInt32)100;
           status = Canlib.canIoCtl(rxChanHndl, Canlib.canIOCTL_SET_TIMER_SCALE, ref timeObj);
           ErrorDump("canIoCtl", status, true);

           // go ahead and start receiving messages into the buffer
           status = Canlib.canBusOn(rxChanHndl);
           ErrorDump("canBusOn", status, true);
       }*/
    }
}
