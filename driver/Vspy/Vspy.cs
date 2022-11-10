using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace UDS上位机.Driver_.Vspy
{
    class Vspy
    {
        static IntPtr m_hObject = (IntPtr)0;
        static int iNetworkID;
        public static List<CANAPI.CANFrame> SpyCANFrameBuff = new List<CANAPI.CANFrame>();//线程接收报文缓冲区
        public static Thread SpyRxThread;// = new Thread(new ThreadStart(SpyRXThread));
        public static int OpenVspy()
        {

            //3.启动接收线程
            SpyRxThread = new Thread(new ThreadStart(SpyRXThread));
            //Console.WriteLine("RXThread Starting..............");
            Program.form2.textBox3.Text += "Vspy RXThread Starting..............\r\n";
            SpyRxThread.Start();
            return 1;
        }
        public static int CloseVspy()
        {
            int iResult;
            int iNumberOfErrors = 0;

            //close the port
            iResult = icsNeoDll.icsneoClosePort(m_hObject, ref iNumberOfErrors);
            if (iResult == 1)
            {
                return 1;
            }
            return 0;
        }
        public static int VspySendMessage(uint CANID, ref byte[] Data, byte DLC)
        {
            long lResult;
            icsSpyMessage stMessagesTx = new icsSpyMessage();

            //lNetworkID = icsNeoDll.GetNetworkIDfromString(ref sTempString);

            // load the message structure

            if (Form2.CANFrameType == 3)//扩展帧
            {
                //Make id Extended
                stMessagesTx.StatusBitField = Convert.ToInt16(eDATA_STATUS_BITFIELD_1.SPY_STATUS_XTD_FRAME);
            }
            else
            {
                //Use Normal ID
                stMessagesTx.StatusBitField = 0;
            }
            stMessagesTx.ArbIDOrHeader = (int)CANID;            // The ArbID
            stMessagesTx.NumberBytesData = DLC;         // The number of Data Bytes
            if (stMessagesTx.NumberBytesData > 8) stMessagesTx.NumberBytesData = 8; // You can only have 8 databytes with CAN
                                                                                    // Load all of the data bytes in the structure
                                                                                    //当DLC<8会如何，不知道？？？？
            stMessagesTx.Data1 = Data[0];
            stMessagesTx.Data2 = Data[1];
            stMessagesTx.Data3 = Data[2];
            stMessagesTx.Data4 = Data[3];
            stMessagesTx.Data5 = Data[4];
            stMessagesTx.Data6 = Data[5];
            stMessagesTx.Data7 = Data[6];
            stMessagesTx.Data8 = Data[7];

            // Transmit the assembled message
            lResult = icsNeoDll.icsneoTxMessages(m_hObject, ref stMessagesTx, iNetworkID, 1);
            // Test the returned result
            if (lResult != 1)
            {
                return 0;
            }
            return 1;
        }
        //接收报文的线程
        unsafe public static void SpyRXThread()
        {
            int lNumberOfMessages = 0;//指示收到的报文数目
            int lNumberOfErrors = 0;
            icsSpyMessage[] stMessages = new icsSpyMessage[100];   //TempSpace for messages

            while (true)
            {
                if (icsNeoDll.icsneoGetMessages(m_hObject, ref stMessages[0], ref lNumberOfMessages, ref lNumberOfErrors) == 1)
                {
                    CANAPI.CANFrame CANFrame = new CANAPI.CANFrame();
                    CANFrame.data = new byte[8];
                    byte[] DMData = new byte[8];
                    for (int k = 0; k < lNumberOfMessages; k++)
                    {

                        //此处设置过滤
                        if ((stMessages[k].ArbIDOrHeader & (~Form2.MaskNum)) == Form2.ACCCodeNum)
                        {
                            CANFrame.ID = (uint)stMessages[k].ArbIDOrHeader;
                            CANFrame.data[0] = stMessages[k].Data1;
                            CANFrame.data[1] = stMessages[k].Data2;
                            CANFrame.data[2] = stMessages[k].Data3;
                            CANFrame.data[3] = stMessages[k].Data4;
                            CANFrame.data[4] = stMessages[k].Data5;
                            CANFrame.data[5] = stMessages[k].Data6;
                            CANFrame.data[6] = stMessages[k].Data7;
                            CANFrame.data[7] = stMessages[k].Data8;
                            CANFrame.dlc = 8;
                            SpyCANFrameBuff.Add(CANFrame);
                            ////Console.WriteLine("RXThread Data[0]:{0:x},Data[1]:{1:x}", CANFrame.data[0], CANFrame.data[1]);
                        }
                    }

                }
                //Thread.Sleep(2);
            }
        }
        public static int VspyGetMessage(ref CANAPI.CANFrame[] CANFrame, uint FrameNum)
        {
            int i = 0;
            //CANFrameBuff,存放所有收到的过滤后的报文
            ////Console.Write("CANCase CANFrameBuff.Count:{0}\r\n", CANFrameBuff.Count);
            for (i = 0; i < Math.Min(SpyCANFrameBuff.Count, FrameNum); i++)
            {
                CANFrame[i].ID = SpyCANFrameBuff[i].ID;
                CANFrame[i].dlc = SpyCANFrameBuff[i].dlc;
                for (int k = 0; k < SpyCANFrameBuff[i].dlc; k++)
                {
                    CANFrame[i].data[k] = SpyCANFrameBuff[i].data[k];
                }
                SpyCANFrameBuff.RemoveAt(i);
            }
            /*for (int k = 0; k <CANFrameBuff.Count; k++)
            {
                //Console.WriteLine("data[0]{0:x},data[1]{1:x}", CANFrameBuff[k].data[0], CANFrameBuff[k].data[1]);
            }*/
            return i;
        }
    }
}
