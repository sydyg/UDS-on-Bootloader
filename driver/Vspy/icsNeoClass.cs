using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


static class Constants
{
    // Constants used to calculate the timestamp
    public const double NEOVI_TIMESTAMP_2 = 0.1048576;
    public const double NEOVI_TIMESTAMP_1 = 0.0000016;

    public const double NEOVIPRO_VCAN_TIMESTAMP_2 = 0.065536;
    public const double NEOVIPRO_VCAN_TIMESTAMP_1 = 0.000001;

    public const double NEOVI6_VCAN_TIMESTAMP_2 = 0.065536;
    public const double NEOVI6_VCAN_TIMESTAMP_1 = 0.000001;

    public const double NEOVI_RED_TIMESTAMP_2_25NS = 107.3741824;
    public const double NEOVI_RED_TIMESTAMP_1_25NS = 0.000000025;

    public const double NEOVI_RED_TIMESTAMP_2_10NS = 429.4967296;
    public const double NEOVI_RED_TIMESTAMP_1_10NS = 0.000000010;
}

namespace UDS上位机
{
    public enum eHardwareTypes : uint
    {
        NEODEVICE_UNKNOWN = 0,
        NEODEVICE_BLUE = 1,
        NEODEVICE_SW_VCAN = 2,
        NEODEVICE_DW_VCAN = 4,
        NEODEVICE_VCAN41 = 7,
        NEODEVICE_FIRE = 8,
        NEODEVICE_VCAN3 = 16,
        NEODEVICE_YELLOW = 32,  //Deprecated
        NEODEVICE_RED = 64,
        NEODEVICE_ECU = 128,
        NEODEVICE_IEVB = 256,
        NEODEVICE_PENDANT = 512,
        NEODEVICE_VIRTUAL_NEOVI = 1024,
        NEODEVICE_ECUCHIP_UART = 2048,
        NEODEVICE_PLASMA_1_11 = 0x1000,
        NEODEVICE_PLASMA_1_12 = 0x10000,
        NEODEVICE_PLASMA_1_13 = 0x20000,
        NEODEVICE_ANY_PLASMA = (NEODEVICE_PLASMA_1_11 | NEODEVICE_FIRE_VNET | NEODEVICE_PLASMA_1_12 | NEODEVICE_PLASMA_1_13),
        NEODEVICE_FIRE_VNET = 0x2000,
        NEODEVICE_NEOANALOG = 0x4000,
        NEODEVICE_ION_2 = 0x40000,
        NEODEVICE_ION_3 = 0x100000,
        NEODEVICE_ANY_ION = (NEODEVICE_ION_2 | NEODEVICE_ION_3),
        NEODEVICE_RADSTAR = 0x80000,
        NEODEVICE_VCANFD = 0x200000,
        NEODEVICE_VCAN42 = 0x400000,
        NEODEVICE_ECU25 = 0x800000,
        NEODEVICE_EEVB = 0x1000000,
        NEODEVICE_VCANRF = 0x2000000,
        NEODEVICE_FIRE2 = 0x4000000,
        NEODEVICE_FLEX = 0x8000000,
        NEODEVICE_RADGALAXY = 0x10000000,
        NEODEVICE_RADSTAR2 = 0x20000000,
        NEODEVICE_VIVIDCAN = 0x40000000,
        NEODEVICE_OBD2_SIM = 0x80000000,
        NEODEVICE_ALL = 0xFFFFBFFF,
    }

    public enum eDATA_STATUS_BITFIELD_1 : uint  //: long 
    {
        SPY_STATUS_GLOBAL_ERR = 0x01,
        SPY_STATUS_TX_MSG = 0x02,
        SPY_STATUS_XTD_FRAME = 0x04,
        SPY_STATUS_REMOTE_FRAME = 0x08,

        SPY_STATUS_CRC_ERROR = 0x10,
        SPY_STATUS_CAN_ERROR_PASSIVE = 0x20,
        SPY_STATUS_INCOMPLETE_FRAME = 0x40,
        SPY_STATUS_LOST_ARBITRATION = 0x80,

        SPY_STATUS_UNDEFINED_ERROR = 0x100,
        SPY_STATUS_CAN_BUS_OFF = 0x200,
        SPY_STATUS_CAN_ERROR_WARNING = 0x400,
        SPY_STATUS_BUS_SHORTED_PLUS = 0x800,

        SPY_STATUS_BUS_SHORTED_GND = 0x1000,
        SPY_STATUS_CHECKSUM_ERROR = 0x2000,
        SPY_STATUS_BAD_MESSAGE_BIT_TIME_ERROR = 0x4000,
        SPY_STATUS_IFR_DATA = 0x8000,

        SPY_STATUS_COMM_IN_OVERFLOW = 0x10000,
        SPY_STATUS_COMM_OUT_OVERFLOW = 0x20000,
        SPY_STATUS_COMM_MISC_ERROR = 0x40000,
        SPY_STATUS_BREAK = 0x80000,

        SPY_STATUS_AVSI_REC_OVERFLOW = 0x100000,
        SPY_STATUS_TEST_TRIGGER = 0x200000,
        SPY_STATUS_AUDIO_COMMENT = 0x400000,
        SPY_STATUS_GPS_DATA = 0x800000,

        SPY_STATUS_ANALOG_DIGITAL_INPUT = 0x1000000,
        SPY_STATUS_TEXT_COMMENT = 0x2000000,
        SPY_STATUS_NETWORK_MESSAGE_TYPE = 0x4000000,
        SPY_STATUS_VSI_TX_UNDERRUN = 0x8000000,

        SPY_STATUS_VSI_IFR_CRC_BIT = 0x10000000,
        SPY_STATUS_INIT_MESSAGE = 0x20000000,
        SPY_STATUS_HIGH_SPEED = 0x40000000,
        SPY_STATUS_FLEXRAY_SECOND_STARTUP_FRAME = 0x40000000,
        SPY_STATUS_EXTENDED = 0x80000000, // if this bit is set than decode status bitfield3 in the ackbytes
    }

    public enum eDATA_STATUS_BITFIELD_2 : uint
    {
        SPY_STATUS2_HAS_VALUE = 1,
        SPY_STATUS2_VALUE_IS_BOOLEAN = 2,
        SPY_STATUS2_HIGH_VOLTAGE = 4,
        SPY_STATUS2_LONG_MESSAGE = 8,
        SPY_STATUS2_GLOBAL_CHANGE = 0x10000,
        SPY_STATUS2_ERROR_FRAME = 0x20000,
        SPY_STATUS2_END_OF_LONG_MESSAGE = 0x100000, //for ISO and J1708,
        //LIN/ISO Specific - check protocol before handling 
        SPY_STATUS2_LIN_ERR_RX_BREAK_NOT_0 = 0x200000,
        SPY_STATUS2_LIN_ERR_RX_BREAK_TOO_SHORT = 0x400000,
        SPY_STATUS2_LIN_ERR_RX_SYNC_NOT_55 = 0x800000,
        SPY_STATUS2_LIN_ERR_RX_DATA_GREATER_8 = 0x1000000,
        SPY_STATUS2_LIN_ERR_TX_RX_MISMATCH = 0x2000000,
        SPY_STATUS2_LIN_ERR_MSG_ID_PARITY = 0x4000000,
        SPY_STATUS2_ISO_FRAME_ERROR = 0x8000000,
        SPY_STATUS2_LIN_SYNC_FRAME_ERROR = 0x8000000,
        SPY_STATUS2_ISO_OVERFLOW_ERROR = 0x10000000,
        SPY_STATUS2_LIN_ID_FRAME_ERROR = 0x10000000,
        SPY_STATUS2_ISO_PARITY_ERROR = 0x20000000,
        SPY_STATUS2_LIN_SLAVE_BYTE_ERROR = 0x20000000,
        SPY_STATUS2_RX_TIMEOUT_ERROR = 0x40000000,
        SPY_STATUS2_LIN_NO_SLAVE_DATA = 0x80000000,
        //MOST Specific - check protocol before handling
        SPY_STATUS2_MOST_PACKET_DATA = 0x200000,
        SPY_STATUS2_MOST_STATUS = 0x400000, //reflects changes in light/lock/MPR/SBC/etc.
        PY_STATUS2_MOST_LOW_LEVEL = 0x800000, //MOST low level message, allocs, deallocs, remote requests.
        SPY_STATUS2_MOST_CONTROL_DATA = 0x1000000,
        SPY_STATUS2_MOST_MHP_USER_DATA = 0x2000000, //MOST HIGH User Data Frame
        SPY_STATUS2_MOST_MHP_CONTROL_DATA = 0x4000000, //MOST HIGH Control Data
        SPY_STATUS2_MOST_I2S_DUMP = 0x8000000,
        SPY_STATUS2_MOST_TOO_SHORT = 0x10000000,
        SPY_STATUS2_MOST_MOST50 = 0x20000000, //absence of MOST50 and MOST150 implies it's MOST25
        SPY_STATUS2_MOST_MOST150 = 0x40000000,
        SPY_STATUS2_MOST_CHANGED_PAR = 0x80000000, //first byte in ack reflects what changed
        //Ethernet Specific - check protocol before handling
        SPY_STATUS2_ETHERNET_CRC_ERROR = 0x200000,
        SPY_STATUS2_ETHERNET_FRAME_TOO_SHORT = 0x400000,
        SPY_STATUS2_ETHERNET_FCS_AVAILABLE = 0x800000, //This frame contains FCS (4 bytes) obtained from ICS Ethernet hardware (ex. RAD-STAR)
    }

    public enum eDATA_STATUS_BITFIELD_3
    {
        SPY_STATUS3_LIN_JUST_BREAK_SYNC = 1,
        SPY_STATUS3_LIN_SLAVE_DATA_TOO_SHORT = 2,
        SPY_STATUS3_LIN_ONLY_UPDATE_SLAVE_TABLE_ONCE = 4,
    }

    public enum icsConfigSetup : short
    {
        NEO_CFG_MPIC_HS_CAN_CNF1 = 512 + 10,
        NEO_CFG_MPIC_HS_CAN_CNF2 = 512 + 9,
        NEO_CFG_MPIC_HS_CAN_CNF3 = 512 + 8,
        NEO_CFG_MPIC_HS_CAN_MODE = 512 + 54,

        // med speed CAN
        NEO_CFG_MPIC_MS_CAN_CNF1 = 512 + 22,
        NEO_CFG_MPIC_MS_CAN_CNF2 = 512 + 21,
        NEO_CFG_MPIC_MS_CAN_CNF3 = 512 + 20,

        NEO_CFG_MPIC_SW_CAN_CNF1 = 512 + 34,
        NEO_CFG_MPIC_SW_CAN_CNF2 = 512 + 33,
        NEO_CFG_MPIC_SW_CAN_CNF3 = 512 + 32,

        NEO_CFG_MPIC_LSFT_CAN_CNF1 = 512 + 46,
        NEO_CFG_MPIC_LSFT_CAN_CNF2 = 512 + 45,
        NEO_CFG_MPIC_LSFT_CAN_CNF3 = 512 + 44,
    }

    // Network ID
    public enum eNETWORK_ID : int
    {
        NETID_DEVICE = 0,
        NETID_HSCAN = 1,
        NETID_MSCAN = 2,
        NETID_SWCAN = 3,
        NETID_LSFTCAN = 4,
        NETID_FORDSCP = 5,
        NETID_J1708 = 6,
        NETID_JVPW = 8,
        NETID_ISO = 9,
        NETID_ISO2 = 14,
        NETID_ISO14230 = 15,
        NETID_LIN = 16,
        NETID_ISO3 = 41,
        NETID_HSCAN2 = 42,
        NETID_HSCAN3 = 44,
        NETID_ISO4 = 47,
        NETID_LIN2 = 48,
        NETID_LIN3 = 49,
        NETID_LIN4 = 50,
        NETID_LIN5 = 84,
        NETID_MOST = 51,
        NETID_CGI = 53,
        NETID_HSCAN4 = 61,
        NETID_HSCAN5 = 62,
        NETID_UART = 64,
        NETID_UART2 = 65,
        NETID_UART3 = 66,
        NETID_UART4 = 67,
        NETID_SWCAN2 = 68, //changed from ANALOG,
        NETID_ETHERNET_DAQ = 69, //NETID_WIZNET - plasma/ ion daq port ethernet,
        NETID_TEXTAPI_TO_HOST = 71,
        NETID_FLEXRAY1A = 80,
        NETID_FLEXRAY1B = 81,
        NETID_FLEXRAY2A = 82,
        NETID_FLEXRAY2B = 83,
        NETID_FLEXRAY = 85,
        NETID_MOST25 = 90,
        NETID_MOST50 = 91,
        NETID_MOST150 = 92,
        NETID_ETHERNET = 93,
        NETID_GMFSA = 94,
        NETID_TCP = 95,
        NETID_HSCAN6 = 96,
        NETID_HSCAN7 = 97,
        NETID_LIN6 = 98,
        NETID_LSFTCAN2 = 99,
        NETID_OP_ETHERNET1 = 17,
        NETID_OP_ETHERNET2 = 18,
        NETID_OP_ETHERNET3 = 19,
        NETID_OP_ETHERNET4 = 45,
        NETID_OP_ETHERNET5 = 46,
        NETID_OP_ETHERNET6 = 73,
        NETID_OP_ETHERNET7 = 75,
        NETID_OP_ETHERNET8 = 76,
        NETID_OP_ETHERNET9 = 77,
        NETID_OP_ETHERNET10 = 78,
        NETID_OP_ETHERNET11 = 79,
        NETID_OP_ETHERNET12 = 87,
    }

    //Vnet Enum for SlaveVnet types
    public enum icsSpySlaveVNETAB : int
    {
        eNoVnet = 0,
        eAinVnet = 2,
        eFlexRayVnet = 3,
        eSlaveFireVnet = 4,
        eSlaveFireVnetEP = 5,
        eSlaveFire2Vnet = 6,
        eSlaveFire2VnetZ = 7,
        eSlaveMost150 = 8,
    }

    //ISO15765 Constants
    public enum eISO15765_NetConst : int
    {
        ISO15765_2_NETWORK_HSCAN = 0x01,
        ISO15765_2_NETWORK_MSCAN = 0x02,
        ISO15765_2_NETWORK_HSCAN2 = 0x04,
        ISO15765_2_NETWORK_HSCAN3 = 0x08,
        ISO15765_2_NETWORK_SWCAN = 0x10,
        ISO15765_2_NETWORK_HSCAN4 = 0x14,
        ISO15765_2_NETWORK_HSCAN5 = 0x18,
        ISO15765_2_NETWORK_HSCAN6 = 0x1C,
        ISO15765_2_NETWORK_HSCAN7 = 0x20,
        ISO15765_2_NETWORK_SWCAN2 = 0x24,
    }

    //CoreMini Status
    public enum eScriptStatus : int
    {
        SCRIPT_STATUS_STOPPED = 0,
        SCRIPT_STATUS_RUNNING = 1,
    }

    //CoreMini Location
    public enum eScriptLocation : int
    {
        SCRIPT_LOCATION_FLASH_MEM = 0,   //(Valid only on a neoVI Fire or neoVI Red)
        SCRIPT_LOCATION_SDCARD = 1,  //(Valid only on a neoVI Fire or neoVI Red)
        SCRIPT_LOCATION_VCAN3_MEM = 2,   //(Valid only on a ValueCAN 3 device)
    }


    // ePROTOCOL
    public enum ePROTOCOL : int
    {
        SPY_PROTOCOL_CUSTOM = 0,
        SPY_PROTOCOL_CAN = 1,
        SPY_PROTOCOL_GMLAN = 2,
        SPY_PROTOCOL_J1850VPW = 3,
        SPY_PROTOCOL_J1850PWM = 4,
        SPY_PROTOCOL_ISO9141 = 5,
        SPY_PROTOCOL_Keyword2000 = 6,
        SPY_PROTOCOL_GM_ALDL_UART = 7,
        SPY_PROTOCOL_CHRYSLER_CCD = 8,
        SPY_PROTOCOL_CHRYSLER_SCI = 9,
        SPY_PROTOCOL_FORD_UBP = 10,
        SPY_PROTOCOL_BEAN = 11,
        SPY_PROTOCOL_LIN = 12,
        SPY_PROTOCOL_J1708 = 13,
        SPY_PROTOCOL_CHRYSLER_JVPW = 14,
        SPY_PROTOCOL_J1939 = 15,
        SPY_PROTOCOL_FLEXRAY = 16,
        SPY_PROTOCOL_MOST = 17,
        SPY_PROTOCOL_CGI = 18,
        SPY_PROTOCOL_GME_CIM_SCL_KLINE = 19,
        SPY_PROTOCOL_SPI = 20,
        SPY_PROTOCOL_I2C = 21,
        SPY_PROTOCOL_GENERIC_UART = 22,
        SPY_PROTOCOL_JTAG = 23,
        SPY_PROTOCOL_UNIO = 24,
        SPY_PROTOCOL_DALLAS_1WIRE = 25,
        SPY_PROTOCOL_GENERIC_MANCHSESTER = 26,
        SPY_PROTOCOL_SENT_PROTOCOL = 27,
        SPY_PROTOCOL_UART = 28,
        SPY_PROTOCOL_ETHERNET = 29,
        SPY_PROTOCOL_CANFD = 30,
        SPY_PROTOCOL_GMFSA = 31,
        SPY_PROTOCOL_TCP = 32,
    }

    public enum icsEthernetLinkType : short
    {
        OPETH_LINK_AUTO = 0,
        OPETH_LINK_MASTER = 1,
        OPETH_LINK_SLAVE = 2,
    }

    // Driver Type Constants
    public enum eDRIVER_TYPE : short
    {
        INTREPIDCS_DRIVER_STANDARD = 0,
        INTREPIDCS_DRIVER_TEST = 1,
    }

    // Port Type Constants
    public enum ePORT_TYPE : short
    {
        NEOVI_COMMTYPE_RS232 = 0,
        NEOVI_COMMTYPE_USB_BULK = 1,
        NEOVI_COMMTYPE_USB_ISO_DONT_USE = 2,
        NEOVI_COMMTYPE_TCPIP = 3,
        NEOVI_COMMTYPE_FIRE_USB = 5,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct icsSpyMessage   //reff
    {
        public Int32 StatusBitField; //4
        public Int32 StatusBitField2; //new '4
        public Int32 TimeHardware; // 4
        public Int32 TimeHardware2; //new ' 4
        public Int32 TimeSystem; // 4
        public Int32 TimeSystem2;
        public byte TimeStampHardwareID; //new ' 1
        public byte TimeStampSystemID;
        public byte NetworkID; //new ' 1
        public byte NodeID;
        public byte Protocol;
        public byte MessagePieceID; // 1
        public byte ExtraDataPtrEnabled; //1
        public byte NumberBytesHeader; // 1
        public byte NumberBytesData; // 1
        public Int16 DescriptionID; // 2
        public Int32 ArbIDOrHeader; // Holds (up to 3 byte 1850 header or 29 bit CAN header) '4
        //public byte[] Data = new byte[8]; //(1 To 8); //8
        public byte Data1;
        public byte Data2;
        public byte Data3;
        public byte Data4;
        public byte Data5;
        public byte Data6;
        public byte Data7;
        public byte Data8;
        public Int32 StatusBitField3;
        public Int32 StatusBitField4;
        //public byte[] AckBytes = new byte[8]; //(1 To 8); //new '8
        public IntPtr iExtraDataPtr; // As Single ' 4 or 8 depending on system
        public byte MiscData;
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct VSBSpyMessage   //reff
    {
        public Int32 StatusBitField; //4
        public Int32 StatusBitField2; //new '4
        public Int32 TimeHardware; // 4
        public Int32 TimeHardware2; //new ' 4
        public Int32 TimeSystem; // 4
        public Int32 TimeSystem2;
        public byte TimeStampHardwareID; //new ' 1
        public byte TimeStampSystemID;
        public byte NetworkID; //new ' 1
        public byte NodeID;
        public byte Protocol;
        public byte MessagePieceID; // 1
        public byte ExtraDataPtrEnabled; //1
        public byte NumberBytesHeader; // 1
        public byte NumberBytesData; // 1
        public Int16 DescriptionID; // 2
        public Int32 ArbIDOrHeader; // Holds (up to 3 byte 1850 header or 29 bit CAN header) '4
        //public byte[] Data = new byte[8]; //(1 To 8); //8
        public byte Data1;
        public byte Data2;
        public byte Data3;
        public byte Data4;
        public byte Data5;
        public byte Data6;
        public byte Data7;
        public byte Data8;
        public Int32 StatusBitField3;
        public Int32 StatusBitField4;
        //public byte[] AckBytes = new byte[8]; //(1 To 8); //new '8
        public UInt32 iExtraDataPtr; //  4 
        public byte MiscData;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct stAPIFirmwareInfo
    {
        public Int32 iType;  // 1,2,3 for Generation of HW.

        // Date and Time type 1 and 2
        public Int32 iMainFirmDateDay;
        public Int32 iMainFirmDateMonth;
        public Int32 iMainFirmDateYear;
        public Int32 iMainFirmDateHour;
        public Int32 iMainFirmDateMin;
        public Int32 iMainFirmDateSecond;
        public Int32 iMainFirmChkSum;

        // Version data (only valid for type 3)
        public byte iAppMajor;
        public byte iAppMinor;
        public byte iManufactureDay;
        public byte iManufactureMonth;
        public UInt16 iManufactureYear;
        public byte iBoardRevMajor;
        public byte iBoardRevMinor;
        public byte iBootLoaderVersionMajor;
        public byte iBootLoaderVersionMinor;
    }




    [StructLayout(LayoutKind.Sequential)]
    public struct icsSpyMessageLong
    {
        public Int32 StatusBitField; // 4
        public Int32 StatusBitField2; //new '4
        public Int32 TimeHardware;
        public Int32 TimeHardware2; //new ' 4
        public Int32 TimeSystem; //4
        public Int32 TimeSystem2;
        public byte TimeStampHardwareID; //new ' 1
        public byte TimeStampSystemID;
        public byte NetworkID; //new ' 1
        public byte NodeID;
        public byte Protocol;
        public byte MessagePieceID; // 1
        public byte ExtraDataPtrEnabled; // 1
        public byte NumberBytesHeader; //
        public byte NumberBytesData; //2
        public Int16 DescriptionID; //2
        public Int32 ArbIDOrHeader;// Holds (up to 3 byte 1850 header or 29 bit CAN header)
        public Int32 DataMsb;
        public Int32 DataLsb;
        public Int32 StatusBitField3;
        public Int32 StatusBitField4;
        public Single Value; // As Single
        public byte MiscData;

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct icsSpyMessageJ1850
    {
        public Int32 StatusBitField; //4
        public Int32 StatusBitField2; //new '4
        public Int32 TimeHardware; //4
        public Int32 TimeHardware2; //new ' 4
        public Int32 TimeSystem; //4
        public Int32 TimeSystem2;
        public byte TimeStampHardwareID; //new ' 1
        public byte TimeStampSystemID;
        public byte NetworkID; //new ' 1
        public byte NodeID;
        public byte Protocol;
        public byte MessagePieceID; // 1 new
        public byte ExtraDataPtrEnabled; // 1
        public byte NumberBytesHeader; //1
        public byte NumberBytesData; //1
        public Int16 DescriptionID; //2
        public byte Header1;  //Holds (up to 3 byte 1850 header or 29 bit CAN header)
        public byte Header2;
        public byte Header3;
        public byte Header4;
        public byte Data1;
        public byte Data2;
        public byte Data3;
        public byte Data4;
        public byte Data5;
        public byte Data6;
        public byte Data7;
        public byte Data8;
        public byte ACK1;
        public byte ACK2;
        public byte ACK3;
        public byte ACK4;
        public byte ACK5;
        public byte ACK6;
        public byte ACK7;
        public byte ACK8;
        public IntPtr iExtraDataPtr; // As Single '4 or 8 depending on system
        public byte MiscData;
    }

    //Structure for neoVI device types
    [StructLayout(LayoutKind.Sequential)]
    public struct NeoDevice
    {
        public Int32 DeviceType;
        public Int32 Handle;
        public Int32 NumberOfClients;
        public Int32 SerialNumber;
        public Int32 MaxAllowedClients;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NeoDeviceEx
    {
        public NeoDevice neoDevice;
        public UInt32 FirmwareMajor;
        public UInt32 FirmwareMinor;
        public UInt32 Status; // bit 1 = CoreMini Running Status, bit 2 = In BootLoader?
        public UInt32 Options; //Use icsSpyAttachedVNEts Enum for states

        public IntPtr pAvailWIFINetwork;
        public IntPtr pWIFIInterfaceInfo;
        public Int32 isEthernetDevice;

        public byte MACAddress0;
        public byte MACAddress1;
        public byte MACAddress2;
        public byte MACAddress3;
        public byte MACAddress4;
        public byte MACAddress5;

        public UInt16 hardwareRev;
        public UInt16 revReserved;

        public UInt32 Reserved0;// may be expanded in future revisions
        public UInt32 Reserved1;
        public UInt32 Reserved2;
        public UInt32 Reserved3;
        public UInt32 Reserved4;
        public UInt32 Reserved5;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct OptionsNeoEx
    {
        Int32 CANOptions;
        Int32 Reserved00;
        Int32 Reserved01;
        Int32 Reserved02;
        Int32 Reserved03;
        Int32 Reserved04;
        Int32 Reserved05;
        Int32 Reserved06;
        Int32 Reserved07;
        Int32 Reserved08;
        Int32 Reserved09;
        Int32 Reserved10;
        Int32 Reserved11;
        Int32 Reserved12;
        Int32 Reserved13;
        Int32 Reserved14;
        Int32 Reserved15;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct CAN_SETTINGS
    {
        public byte Mode;
        public byte SetBaudrate;
        public byte Baudrate;
        public byte Transceiver_Mode;
        public byte TqSeg1;
        public byte TqSeg2;
        public byte TqProp;
        public byte TqSync;
        public UInt16 BRP;
        public byte auto_baud;
        public byte innerFrameDelay25us; // 0 - 375us, 25us resolution
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct CANFD_SETTINGS
    {
        public byte FDMode;     /* mode, secondary baudrate for canfd */
        public byte FDBaudrate;
        public byte FDTqSeg1;
        public byte FDTqSeg2;
        public byte FDTqProp;
        public byte FDTqSync;
        public UInt16 FDBRP;
        public byte FDTDC;
        public byte reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SWCAN_SETTINGS
    {
        public byte Mode;
        public byte SetBaudrate;
        public byte Baudrate;
        public byte transceiver_mode;
        public byte TqSeg1;
        public byte TqSeg2;
        public byte TqProp;
        public byte TqSync;
        public UInt16 BRP;
        public UInt16 high_speed_auto_switch;
        public byte auto_baud;
        public byte Reserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SVCAN3Settings
    {
        public CAN_SETTINGS Can1;
        public CAN_SETTINGS Can2;
        public UInt16 Network_enables;
        public UInt16 Network_enabled_on_boot;
        public Int16 Iso15765_separation_time_offset;
        public UInt16 Perf_en;
        public UInt16 Misc_io_initial_ddr;
        public UInt16 Misc_io_initial_latch;
        public UInt16 Misc_io_report_period;
        public UInt16 Misc_io_on_report_events;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SVCAN412Settings
    {
        /* Performance Test */
        public UInt16 perf_en;

        public CAN_SETTINGS can1;
        public CANFD_SETTINGS canfd1;
        public CAN_SETTINGS can2;
        public CANFD_SETTINGS canfd2;

        public UInt64 network_enables;
        public UInt64 termination_enables;

        public UInt32 pwr_man_timeout;
        public UInt32 pwr_man_enable;

        public UInt16 network_enabled_on_boot;

        /* ISO15765-2 Transport Layer */
        public Int16 iso15765_separation_time_offset;

        public STextAPISettings text_api;
        public UInt32 reserved;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct LIN_SETTINGS
    {
        public UInt32 Baudrate;
        public UInt16 spbrg;
        public byte brgh;
        public byte NumBitsDelay;
        public byte MasterResistor;
        public byte Mode;
    }

    // --- UART Settings
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct UART_SETTINGS
    {
        public UInt16 Baudrate;
        public UInt16 spbrg;
        public UInt16 brgh;
        public UInt16 parity;
        public UInt16 stop_bits;
        public byte flow_control; // 0- off, 1 - Simple CTS RTS,
        public byte reserved_1;
        public UInt32 bOptions; //AND to combind these values  invert_tx = 1 invert_rx = 2  half_duplex = 4
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ETHERNET_SETTINGS
    {
        public byte duplex; //* 0 = half, 1 = full */
        public byte link_speed;
        public byte auto_neg;
        public byte led_mode;
        public byte rsvd0;
        public byte rsvd1;
        public byte rsvd2;
        public byte rsvd3;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SNeoMostGatewaySettings
    {
        public UInt16 netId;        //Netid of CAN network to use.
        public byte zero;
        public byte Config;
        //Bit 0: enable bit to enalbe most
        //Bit 1-3: index of which miscio to use for timestamp sync. 0 => MISC1
        //Bit 4:  Echo to CAN enable
        //Bit 5-7: Reserve
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct OP_ETH_GENERAL_SETTINGS
    {
        byte ucInterfaceType;
        byte reserved0;
        byte reserved1;
        byte reserved2;
        UInt16 tapPair0;
        UInt16 tapPair1;
        UInt16 tapPair2;
        UInt16 tapPair3;
        UInt16 tapPair4;
        UInt16 tapPair5;
        UInt32 flags;    // Bit field unsigned (bTapEnSwitch: 1; bTapEnPtp: 2; bEnReportLinkQuality: 4)
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct OP_ETH_SETTINGS
    {
        byte ucConfigMode;
        byte preemption_en;
        byte reserved0;
        byte reserved1;
        byte reserved2;
        byte reserved3;
        byte reserved4;
        byte reserved5;
        byte reserved6;
        byte reserved7;
        byte reserved8;
        byte reserved9;
        byte reserved10;
        byte reserved11;
        byte reserved12;
        byte reserved13;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct J1708_SETTINGS
    {
        public UInt16 enable_convert_mode;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SVCANRFSettings
    {
        public CAN_SETTINGS can1;
        public CAN_SETTINGS can2;
        public CAN_SETTINGS can3;
        public CAN_SETTINGS can4;

        public LIN_SETTINGS lin1;
        public LIN_SETTINGS lin2;

        public UInt16 network_enables;
        public UInt16 network_enabled_on_boot;

        public UInt32 pwr_man_timeout;
        public UInt16 pwr_man_enable; // 0 - off, 1 - sleep enabled, 2- idle enabled (fast wakeup)

        public UInt16 misc_io_initial_ddr;
        public UInt16 misc_io_initial_latch;
        public UInt16 misc_io_analog_enable;
        public UInt16 misc_io_report_period;
        public UInt16 misc_io_on_report_events;

        //ISO 15765-2 Transport Layer
        public Int16 iso15765_separation_time_offset;

        //ISO9141 - KEYWORD 2000 1
        public Int16 iso9141_kwp_enable_reserved;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings;

        //Performance Test
        public UInt16 perf_en;

        //ISO9141 - Parity
        public UInt16 iso_parity; // 0 - no parity, 1 - event, 2 - odd
        public UInt16 iso_msg_termination; // 0 - use inner frame time, 1 - GME CIM-SCL
        public UInt16 iso_tester_pullup_enable;

        //Additional network enables
        public UInt16 network_enables_2;

        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_2;
        public UInt16 iso_parity_2; 							// 0 - no parity, 1 - event, 2 - odd
        public UInt16 iso_msg_termination_2; 				// 0 - use inner frame time, 1 - GME CIM-SCL

        public UInt16 idle_wakeup_network_enables_1;
        public UInt16 idle_wakeup_network_enables_2;

        public UInt16 reservedZero;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SEEVBSettings
    {
        public UInt32 ecu_id;

        public CAN_SETTINGS can1;

        public UInt16 network_enables;
        public UInt16 network_enabled_on_boot;

        // ISO 15765-2 Transport layer
        public UInt16 iso15765_separation_time_offset;

        // Performance test
        public UInt16 perf_en;

        // Analog input
        public UInt16 ain_sample_period;
        public UInt16 ain_threshold;

        public UInt32 rsvd;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct STextAPISettings
    {
        public UInt32 can1_tx_id;
        public UInt32 can1_rx_id;
        public UInt32 can1_options; // Set to 1 for Extended, 0 for standard

        public UInt32 can2_tx_id;
        public UInt32 can2_rx_id;
        public UInt32 can2_options; // Set to 1 for Extended, 0 for standard

        public UInt32 network_enables;

        public UInt32 can3_tx_id3;
        public UInt32 can3_rx_id3;
        public UInt32 can3_options; // Set to 1 for Extended, 0 for standard

        public UInt32 can4_tx_id4;
        public UInt32 can4_rx_id4;
        public UInt32 can4_options; // Set to 1 for Extended, 0 for standard

        public Int32 Reserved0;
        public Int32 Reserved1;
        public Int32 Reserved2;
        public Int32 Reserved3;
        public Int32 Reserved4;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ISO9141_KEYWORD2000_SETTINGS
    {
        public UInt32 Baudrate;
        public UInt16 spbrg;
        public UInt16 brgh;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_0;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_1;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_2;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_3;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_4;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_5;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_6;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_7;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_8;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_9;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_10;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_11;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_12;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_13;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_14;
        public ISO9141_KEYWORD2000__INIT_STEP Init_Step_15;
        public byte init_step_count;
        public UInt16 p2_500us;
        public UInt16 p3_500us;
        public UInt16 p4_500us;
        public UInt16 chksum_enabled;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct ISO9141_KEYWORD2000__INIT_STEP
    {
        public UInt16 time_500us;
        public UInt16 k;
        public UInt16 l;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SRADGalaxySettings
    {
        public UInt16 perf_en;

        public OP_ETH_GENERAL_SETTINGS opEthGen;
        public OP_ETH_SETTINGS opEth1;
        public OP_ETH_SETTINGS opEth2;
        public OP_ETH_SETTINGS opEth3;
        public OP_ETH_SETTINGS opEth4;
        public OP_ETH_SETTINGS opEth5;
        public OP_ETH_SETTINGS opEth6;
        public OP_ETH_SETTINGS opEth7;
        public OP_ETH_SETTINGS opEth8;
        public OP_ETH_SETTINGS opEth9;
        public OP_ETH_SETTINGS opEth10;
        public OP_ETH_SETTINGS opEth11;
        public OP_ETH_SETTINGS opEth12;

        //CAN 
        public CAN_SETTINGS Can1;
        public CANFD_SETTINGS Canfd1;
        public CAN_SETTINGS Can2;
        public CANFD_SETTINGS Canfd2;
        public CAN_SETTINGS Can3;
        public CANFD_SETTINGS Canfd3;
        public CAN_SETTINGS Can4;
        public CANFD_SETTINGS Canfd4;
        public CAN_SETTINGS Can5;
        public CANFD_SETTINGS Canfd5;
        public CAN_SETTINGS Can6;
        public CANFD_SETTINGS Canfd6;
        public CAN_SETTINGS Can7;
        public CANFD_SETTINGS Canfd7;
        public CAN_SETTINGS Can8;
        public CANFD_SETTINGS Canfd8;

        /* Native CAN are either LS1/LS2 or SW1/SW2 */
        public SWCAN_SETTINGS swcan1;
        public UInt16 network_enables;
        public SWCAN_SETTINGS swcan2;
        public UInt16 network_enables_2;

        public LIN_SETTINGS lin1;
        public UInt16 misc_io_initial_ddr;
        public UInt16 misc_io_initial_latch;
        public UInt16 misc_io_report_period;
        public UInt16 misc_io_on_report_events;
        public UInt16 misc_io_analog_enable;
        public UInt16 ain_sample_period;
        public UInt16 ain_threshold;

        public UInt32 pwr_man_timeout;
        public UInt16 pwr_man_enable;

        public UInt16 network_enabled_on_boot;

        public UInt16 iso15765_separation_time_offset;

        public UInt16 iso_9141_kwp_enable_reserved;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_1;
        public UInt16 iso_parity_1;

        public UInt16 iso_msg_termination_1;

        public UInt16 idle_wakeup_network_enables_1;
        public UInt16 idle_wakeup_network_enables_2;

        public UInt16 network_enables_3;
        public UInt16 idle_wakeup_network_enables_3;

        public UInt16 can_switch_mode;
        public STextAPISettings text_api;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SOBD2SimSettings
    {
        public UInt16 can1;
        public UInt16 can2;
        public UInt16 network_enables;
        public UInt16 network_enabled_on_boot;
        public UInt16 iso15765_separation_time_offset;
        public UInt16 perf_en;
        public UInt16 misc_io_initial_ddr;
        public UInt16 misc_io_initial_latch;
        public UInt16 misc_io_report_period;
        public UInt16 misc_io_on_report_events;
        public UInt16 misc_io_analog_enable;
        public UInt16 ain_sample_period;
        public UInt16 ain_threshold;
        public UInt16 canfd1;
        public UInt16 canfd2;
        public UInt16 network_enables_2;
        public UInt16 text_api;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SVividCANSettings
    {
        public UInt32 ecu_id;
        public CAN_SETTINGS can1;
        public SWCAN_SETTINGS swcan1;
        public CAN_SETTINGS lsftcan1;
        public UInt16 network_enables;
        public UInt16 network_enabled_on_boot;
        public UInt16 iso15765_separation_time_offset;
        public UInt16 perf_en;
        public UInt32 pwr_man_timeout;
        public UInt16 pwr_man_enable;
        public UInt16 can_switch_mode;
        public UInt16 rsvd;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SRADStar2Settings
    {
        public UInt16 perf_en;
        public OP_ETH_GENERAL_SETTINGS opEthGen;
        public OP_ETH_SETTINGS opEth1;
        public OP_ETH_SETTINGS opEth2;
        public CAN_SETTINGS can1;
        public CANFD_SETTINGS canfd1;
        public CAN_SETTINGS can2;
        public CANFD_SETTINGS canfd2;
        public UInt16 network_enables;
        public UInt16 network_enables_2;
        public LIN_SETTINGS lin1;
        public UInt16 misc_io_initial_ddr;
        public UInt16 misc_io_initial_latch;
        public UInt16 misc_io_report_period;
        public UInt16 misc_io_on_report_events;
        public UInt16 misc_io_analog_enable;
        public UInt16 ain_sample_period;
        public UInt16 ain_threshold;
        public UInt32 pwr_man_timeout;
        public UInt16 pwr_man_enable;
        public UInt16 network_enabled_on_boot;
        public UInt16 iso15765_separation_time_offset;
        public UInt16 iso_9141_kwp_enable_reserved;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_1;
        public UInt16 iso_parity_1;
        public UInt16 iso_msg_termination_1;
        public UInt16 idle_wakeup_network_enables_1;
        public UInt16 idle_wakeup_network_enables_2;
        public UInt16 network_enables_3;
        public UInt16 idle_wakeup_network_enables_3;
        public UInt16 can_switch_mode;
        public STextAPISettings text_api;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SFireSettings
    {
        public CAN_SETTINGS can1;
        public CAN_SETTINGS can2;
        public CAN_SETTINGS can3;
        public CAN_SETTINGS can4;
        public SWCAN_SETTINGS swcan;
        public CAN_SETTINGS lsftcan;
        public LIN_SETTINGS lin1;
        public LIN_SETTINGS lin2;
        public LIN_SETTINGS lin3;
        public LIN_SETTINGS lin4;

        public UInt16 cgi_enable_reserved;
        public UInt16 cgi_baud;
        public UInt16 cgi_tx_ifs_bit_times;
        public UInt16 cgi_rx_ifs_bit_times;
        public UInt16 cgi_chksum_enable;

        public UInt16 network_enables;
        public UInt16 network_enabled_on_boot;

        public UInt32 pwm_man_timeout;
        public UInt16 pwr_man_enable;

        public UInt16 misc_io_initial_ddr;
        public UInt16 misc_io_initial_latch;
        public UInt16 misc_io_analog_enable;
        public UInt16 misc_io_report_period;
        public UInt16 misc_io_on_report_events;
        public UInt16 ain_sample_period;
        public UInt16 ain_threshold;

        //ISO 15765-2 Transport Layer
        public UInt16 iso15765_separation_time_offset;

        //ISO9141 - KEYWORD 2000
        public UInt16 iso9141_kwp_enable_reserved;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings;

        //Performance Test
        public UInt16 perf_en;

        //ISO9141 - Parity
        public UInt16 iso_parity;  // 0 - no parity, 1 - event, 2 - odd
        public UInt16 iso_msg_termination;  // 0 - use inner frame time, 1 - GME CIM-SCL
        public UInt16 iso_tester_pullup_enable;

        //Additional network enables
        public UInt16 network_enables_2;

        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_2;
        public UInt16 iso_parity_2;        // 0 - no parity, 1 - event, 2 - odd
        public UInt16 iso_msg_termination_2;     // 0 - use inner frame time, 1 - GME CIM-SCL
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_3;
        public UInt16 iso_parity_3;        // 0 - no parity, 1 - event, 2 - odd
        public UInt16 iso_msg_termination_3;     // 0 - use inner frame time, 1 - GME CIM-SCL
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_4;
        public UInt16 iso_parity_4;        // 0 - no parity, 1 - event, 2 - odd
        public UInt16 iso_msg_termination_4;     // 0 - use inner frame time, 1 - GME CIM-SCL    
        public UInt16 fast_init_network_enables_1;
        public UInt16 fast_init_network_enables_2;
        public UART_SETTINGS uart;
        public UART_SETTINGS uart2;
        public STextAPISettings text_api;
        public SNeoMostGatewaySettings neoMostGateway;
        public UInt16 vnetBits;  //First bit enables Android Messages
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct SFire2Settings
    {
        public UInt16 perf_en;
        public CAN_SETTINGS can1;
        public CANFD_SETTINGS canfd1;
        public CAN_SETTINGS can2;
        public CANFD_SETTINGS canfd2;
        public CAN_SETTINGS can3;
        public CANFD_SETTINGS canfd3;
        public CAN_SETTINGS can4;
        public CANFD_SETTINGS canfd4;
        public CAN_SETTINGS can5;
        public CANFD_SETTINGS canfd5;
        public CAN_SETTINGS can6;
        public CANFD_SETTINGS canfd6;
        public CAN_SETTINGS can7;
        public CANFD_SETTINGS canfd7;
        public CAN_SETTINGS can8;
        public CANFD_SETTINGS canfd8;
        public SWCAN_SETTINGS swcan1;
        public UInt16 network_enables;
        public SWCAN_SETTINGS swcan2;
        public UInt16 network_enables_2;
        public CAN_SETTINGS lsftcan1;
        public CAN_SETTINGS lsftcan2;
        public LIN_SETTINGS lin1;
        public UInt16 misc_io_initial_ddr;
        public LIN_SETTINGS lin2;
        public UInt16 misc_io_initial_latch;
        public LIN_SETTINGS lin3;
        public UInt16 misc_io_report_period;
        public LIN_SETTINGS lin4;
        public UInt16 misc_io_on_report_events;
        public LIN_SETTINGS lin5;
        public UInt16 misc_io_analog_enable;
        public UInt16 ain_sample_period;
        public UInt16 ain_threshold;
        public UInt32 pwr_man_timeout;
        public UInt16 pwr_man_enable;
        public UInt16 network_enabled_on_boot;
        public UInt16 iso15765_separation_time_offset;
        public UInt16 iso_9141_kwp_enable_reserved;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_1;
        public UInt16 iso_parity_1;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_2;
        public UInt16 iso_parity_2;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_3;
        public UInt16 iso_parity_3;
        public ISO9141_KEYWORD2000_SETTINGS iso9141_kwp_settings_4;
        public UInt16 iso_parity_4;
        public UInt16 iso_msg_termination_1;
        public UInt16 iso_msg_termination_2;
        public UInt16 iso_msg_termination_3;
        public UInt16 iso_msg_termination_4;
        public UInt16 idle_wakeup_network_enables_1;
        public UInt16 idle_wakeup_network_enables_2;
        public UInt16 network_enables_3;
        public UInt16 idle_wakeup_network_enables_3;
        public UInt16 can_switch_mode;
        public STextAPISettings text_api;
        public UInt64 termination_enables;
        public LIN_SETTINGS lin6;
        public ETHERNET_SETTINGS ethernet;
        public UInt16 slaveVnetA;
        public UInt16 slaveVnetB;
        public UInt32 flags;
        public UInt16 digitalIoThresholdTicks;
        public UInt16 digitalIoThresholdEnable;
    }

    //_stChipVersions
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct fire_versions
    {
        public Byte mpic_maj;
        public Byte mpic_min;
        public Byte upic_maj;
        public Byte upic_min;
        public Byte lpic_maj;
        public Byte lpic_min;
        public Byte jpic_maj;
        public Byte jpic_min;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct plasma_fire_vnet
    {
        public Byte mpic_maj;
        public Byte mpic_min;
        public Byte core_maj;
        public Byte core_min;
        public Byte lpic_maj;
        public Byte lpic_min;
        public Byte hid_maj;
        public Byte hid_min;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct vcan3_versions
    {
        public Byte mpic_maj;
        public Byte mpic_min;
        public UInt32 Reserve;
        public UInt16 Reserve2;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct stCM_ISO157652_TxMessage
    {
        //transmit message
        public UInt16 vs_netid; //< The netid of the message (determines which network to transmit on),  not supported
        public byte padding; //< The padding byte to use to fill the unused portion of
        //  transmitted CAN frames (single frame, first frame, consecutive frame).
        public byte reserved2;
        public UInt32 id;  //< arbId of transmitted frames (CAN id to transmit to).
        public UInt32 fc_id;  //< flow control arb id filter value (response id from receiver).
        public UInt32 fc_id_mask; //< The flow control arb filter mask (response id from receiver).
        public byte stMin;//< Overrides the stMin that the receiver reports, see overrideSTmin. Set to J2534's STMIN_TX if <= 0xFF.
        public byte blockSize; //< Overrides the block size that the receiver reports, see overrideBlockSize.   Set to J2534's BS_TX if <= 0xFF.
        public byte flowControlExtendedAddress;//< Expected Extended Address byte of response from receiver.  see fc_ext_address_enable, not supported.
        public byte extendedAddress;//< Extended Address byte of transmitter. see ext_address_enable, not supported.

        //flow control timeouts
        public UInt16 fs_timeout;    //< max timeout (ms) for waiting on flow control respons. Set this to N_BS_MAX's value if J2534.
        public UInt16 fs_wait; //< max timeout (ms) for waiting on flow control response after receiving flow control
        ///with flow status set to WAIT.   Set this to N_BS_MAX's value if J2534.
        //******************************************************************************************************************
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4095)] public byte[] data;
        // call: stCM_ISO157652_TxMessage.data = new byte(4096)
        //******************************************************************************************************************
        public UInt32 num_bytes; //< Number of data bytes
        //option bits
        public UInt16 flags;
        //To set the flags, AND the parameter you want from the stCM_ISO157652_TxMessage_Flags Enum
        public byte Reserved;
        public byte tx_dl;
    }

    public enum stCM_ISO157652_TxMessage_Flags : int
    {
        id_29_bit_enable = 1, //< Enables 29 bit arbId for transmitted frames.  Set to 1 so transmitted frames use 29 bit ids, not supported.
        fc_id_29_bit_enable = 2, //< Enables 29 bit arbId for Flow Control filter.  Set to 1 if receiver response uses 29 bit ids, not supported.
        ext_address_enable = 4, //< Enables Extended Addressing, Set to 1 if transmitted frames should have extended addres byte, not supported.
        fc_ext_address_enable = 8, //< Enables Extended Addressing for Flow Control filter.  Set to 1 if receiver responds with extended address byte, not supported.
        overrideSTmin = 16, //< Uses member stMin and not receiver's flow control's stMin.
        overrideBlockSize = 32, //< Uses member BlockSize and not receiver's flow control's BlockSize.
        paddingEnable = 64, //< Enable's padding
        iscanFD = 128,  //Enables CAN FD
        isBSREnabled = 256, //Enables BSR
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    public struct stCM_ISO157652_RxMessage
    {
        //transmit message
        public UInt16 vs_netid; //< The netid of the message (determines which network to decode receives),  not supported
        public byte padding;//< The padding byte to use to fill the unused portion of
        ///  transmitted CAN frames (flow control), see paddingEnable.
        public UInt32 id; //< ArbId filter value for frames from transmitter (from ECU to neoVI).
        public UInt32 id_mask; //< ArbId filter mask for frames from transmitter (from ECU to neoVI).
        public UInt32 fc_id;  //< flow control arbId to transmit in flow control (from neoVI to ECU).
        public byte flowControlExtendedAddress; //< Extended Address byte used in flow control (from neoVI to ECU). see fc_ext_address_enable.
        public byte extendedAddress; //< Expected Extended Address byte of frames sent by transmitter (from ECU to neoVI).  see ext_address_enable.
        public byte blockSize; //< Block Size to report in flow control response.
        public byte stMin; //< Minimum seperation time (between consecutive frames) to report in flow control response.
        //flow control timeouts
        public UInt16 cf_timeout;    //< max timeout (ms) for waiting on consecutive frame.  Set this to N_CR_MAX's value in J2534.
        //option bits
        public UInt32 flags;
        //To set the flags, AND the parameter you want from the stCM_ISO157652_RxMessage_Flags Enum
        public byte reserved0;
        public byte reserved1;
        public byte reserved2;
        public byte reserved3;
        public byte reserved4;
        public byte reserved5;
        public byte reserved6;
        public byte reserved7;
        public byte reserved8;
        public byte reserved9;
        public byte reserved10;
        public byte reserved11;
        public byte reserved12;
        public byte reserved13;
        public byte reserved14;
        public byte reserved15;
    }


    public enum stCM_ISO157652_RxMessage_Flags : int
    {
        id_29_bit_enable = 1, //< Enables 29 bit arbId filter for frames (from ECU to neoVI).
        fc_id_29_bit_enable = 2, //< Enables 29 bit arbId for Flow Control (from neoVI to ECU).
        ext_address_enable = 4, //< Enables Extended Addressing (from ECU to neoVI).
        fc_ext_address_enable = 8, //< Enables Extended Addressing (from neoVI to ECU).
        enableFlowControlTransmission = 16, //< Enables Flow Control frame transmission (from neoVI to ECU).
        paddingEnable = 32, //< Enable's padding
        iscanFD = 64,
        isBRSEnabled = 128,
    }



    public class icsNeoDll
    {
        public const double NEOVI_TIMEHARDWARE2_SCALING = 0.1048576;
        public const double NEOVI_TIMEHARDWARE_SCALING = 0.0000016;

        public const double NEOVIPRO_VCAN_TIMEHARDWARE2_SCALING = 0.065536;
        public const double NEOVIPRO_VCAN_TIMEHARDWARE_SCALING = 0.000001;

        // med speed CAN
        public const Int16 NEO_CFG_MPIC_MS_CAN_CNF1 = 512 + 22;
        public const Int16 NEO_CFG_MPIC_MS_CAN_CNF2 = 512 + 21;
        public const Int16 NEO_CFG_MPIC_MS_CAN_CNF3 = 512 + 20;

        public const Int16 NEO_CFG_MPIC_SW_CAN_CNF1 = 512 + 34;
        public const Int16 NEO_CFG_MPIC_SW_CAN_CNF2 = 512 + 33;
        public const Int16 NEO_CFG_MPIC_SW_CAN_CNF3 = 512 + 32;

        public const Int16 NEO_CFG_MPIC_LSFT_CAN_CNF1 = 512 + 46;
        public const Int16 NEO_CFG_MPIC_LSFT_CAN_CNF2 = 512 + 45;
        public const Int16 NEO_CFG_MPIC_LSFT_CAN_CNF3 = 512 + 44;

        // Protocols
        public const Int16 SPY_PROTOCOL_CUSTOM = 0;
        public const Int16 SPY_PROTOCOL_CAN = 1;
        public const Int16 SPY_PROTOCOL_GMLAN = 2;
        public const Int16 SPY_PROTOCOL_J1850VPW = 3;
        public const Int16 SPY_PROTOCOL_J1850PWM = 4;
        public const Int16 SPY_PROTOCOL_ISO9141 = 5;
        public const Int16 SPY_PROTOCOL_Keyword2000 = 6;
        public const Int16 SPY_PROTOCOL_GM_ALDL_UART = 7;
        public const Int16 SPY_PROTOCOL_CHRYSLER_CCD = 8;
        public const Int16 SPY_PROTOCOL_CHRYSLER_SCI = 9;
        public const Int16 SPY_PROTOCOL_FORD_UBP = 10;
        public const Int16 SPY_PROTOCOL_BEAN = 11;
        public const Int16 SPY_PROTOCOL_LIN = 12;



        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoFindNeoDevices(UInt32 DeviceTypes, ref NeoDevice pNeoDevice, ref Int32 pNumDevices);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoFindDevices(ref NeoDeviceEx pNeoDevice, ref Int32 pNumDevices, UInt32 DeviceTypes, UInt32 numDeviceTypes, ref OptionsNeoEx pOptionsFindNeoEx, UInt32 reserved);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoOpenNeoDevice(ref NeoDevice pNeoDevice, ref IntPtr hObject, ref byte bNetworkIDs, Int32 bConfigRead, Int32 bSyncToPC);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoClosePort(IntPtr hObject, ref Int32 pNumberOfErrors);
        [DllImport("icsneo40.dll")]
        public static extern void icsneoFreeObject(IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoOpenPortEx(Int32 lPortNumber, Int32 lPortType, Int32 lDriverType, Int32 lIPAddressMSB, Int32 lIPAddressLSBOrBaudRate, Int32 bConfigRead, ref byte bNetworkID, ref IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetMessages(IntPtr hObject, ref icsSpyMessage pMsg, ref Int32 pNumberOfMessages, ref Int32 pNumberOfErrors);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoTxMessages(IntPtr hObject, ref icsSpyMessageJ1850 pMsg, Int32 lNetworkID, Int32 lNumMessages);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoTxMessagesEx(IntPtr hObject, ref icsSpyMessage pMsg, UInt32 lNetworkID, UInt32 lNumMessages, ref UInt32 NumTxed, UInt32 reserved);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoTxMessages(IntPtr hObject, ref icsSpyMessage pMsg, Int32 lNetworkID, Int32 lNumMessages);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoWaitForRxMessagesWithTimeOut(IntPtr hObject, UInt32 iTimeOut);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoEnableNetworkRXQueue(IntPtr hObject, Int32 iEnable);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetTimeStampForMsg(IntPtr hObject, ref icsSpyMessage pMsg, ref double pTimeStamp);
        [DllImport("icsneo40.dll")]
        public static extern void icsneoGetISO15765Status(IntPtr hObject, Int32 lNetwork, Int32 lClearTxStatus, Int32 lClearRxStatus, ref Int32 lTxStatus, ref Int32 lRxStatus);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetConfiguration(IntPtr hObject, ref byte pData, ref Int32 lNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSendConfiguration(IntPtr hObject, ref byte pData, Int32 lNumBytes);


        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetFireSettings(IntPtr hObject, ref SFireSettings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetFireSettings(IntPtr hObject, ref SFireSettings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetVCAN3Settings(IntPtr hObject, ref SVCAN3Settings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetVCAN3Settings(IntPtr hObject, ref SVCAN3Settings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);

        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetVCAN412Settings(IntPtr hObject, ref SVCAN412Settings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetVCAN412Settings(IntPtr hObject, ref SVCAN412Settings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);

        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetFire2Settings(IntPtr hObject, ref SFire2Settings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetFire2Settings(IntPtr hObject, ref SFire2Settings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);

        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetRADStar2Settings(IntPtr hObject, ref SRADStar2Settings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetRADStar2Settings(IntPtr hObject, ref SRADStar2Settings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);

        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetRADGalaxySettings(IntPtr hObject, ref SRADGalaxySettings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetRADGalaxySettings(IntPtr hObject, ref SRADGalaxySettings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);

        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetVCANRFSettings(IntPtr hObject, ref SVCANRFSettings pSettings, Int32 iNumBytes);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetVCANRFSettings(IntPtr hObject, ref SVCANRFSettings pSettings, Int32 iNumBytes, Int32 bSaveToEEPROM);


        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetBitRate(IntPtr hObject, Int32 BitRate, Int32 NetworkID);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetDeviceParameters(IntPtr hObject, ref char pParameter, ref char pValues, Int16 ValuesLength);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSetDeviceParameters(IntPtr hObject, ref char pParmValue, ref Int32 pErrorIndex, Int32 bSaveToEEPROM);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetLastAPIError(IntPtr hObject, ref UInt32 pErrorNumber);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetErrorMessages(IntPtr hObject, ref Int32 pErrorMsgs, ref Int32 pNumberOfErrors);
        [DllImport("icsneo40.dll")]
        public static extern int icsneoGetErrorInfo(int iErrorNumber, StringBuilder sErrorDescriptionShort, StringBuilder sErrorDescriptionLong, ref int iMaxLengthShort, ref int iMaxLengthLong, ref int lErrorSeverity, ref int lRestartNeeded);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoValidateHObject(IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoGetDLLVersion();
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoStartSockServer(IntPtr hObject, Int32 iPort);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoStopSockServer(IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptStart(IntPtr hObject, Int32 iLocation);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptStop(IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptLoad(IntPtr hObject, ref byte bin, UInt32 len_bytes, Int32 iLocation);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptClear(IntPtr hObject, Int32 iLocation);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptStartFBlock(IntPtr hObject, UInt32 fb_index);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptGetFBlockStatus(IntPtr hObject, UInt32 fb_index, ref Int32 piRunStatus);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptStopFBlock(IntPtr hObject, UInt32 fb_index);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoScriptGetScriptStatus(IntPtr hObject, ref Int32 piStatus);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoOpenPort(Int32 lPortNumber, Int32 lPortType, Int32 lDriverType, ref byte bNetworkID, ref byte bSCPIDs, ref IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoEnableNetworkCom(IntPtr hObject, Int32 Enable);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoISO15765_DisableNetworks(IntPtr hObject);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoISO15765_EnableNetworks(IntPtr hObject, Int32 Network);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoFindAllCOMDevices(Int32 lDriverType, Int32 lGetSerialNumbers, Int32 lStopAtFirst, Int32 lUSBCommOnly, ref Int32 p_lDeviceTypes, ref Int32 p_lComPorts, ref Int32 p_lSerialNumbers, ref Int32 lNumDevices);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoISO15765_TransmitMessage(IntPtr hObject, Int32 ulNetworkID, ref stCM_ISO157652_TxMessage pMsg, Int32 ulBlockingTimeout);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoISO15765_ReceiveMessage(IntPtr hObject, Int32 ulNetworkID, ref stCM_ISO157652_RxMessage pMsg);
        [DllImport("icsneo40.dll")]
        public static extern Int32 icsneoSerialNumberToString(UInt32 Serial, ref byte Data, ref UInt32 DataSize);

        public static double icsneoGetTimeStamp(long TimeHardware, long TimeHardware2)
        {
            return NEOVI_TIMEHARDWARE2_SCALING * TimeHardware2 + NEOVI_TIMEHARDWARE_SCALING * TimeHardware;
        }

        public static void ConvertCANtoJ1850Message(ref icsSpyMessage icsCANStruct, ref icsSpyMessageJ1850 icsJ1850Struct)
        {
            icsJ1850Struct.StatusBitField = icsCANStruct.StatusBitField;
            icsJ1850Struct.StatusBitField2 = icsCANStruct.StatusBitField2;
            icsJ1850Struct.TimeHardware = icsCANStruct.TimeHardware;
            icsJ1850Struct.TimeHardware2 = icsCANStruct.TimeHardware2;
            icsJ1850Struct.TimeSystem = icsCANStruct.TimeSystem;
            icsJ1850Struct.TimeSystem2 = icsCANStruct.TimeSystem2;
            icsJ1850Struct.TimeStampHardwareID = icsCANStruct.TimeStampHardwareID;
            icsJ1850Struct.TimeStampSystemID = icsCANStruct.TimeStampSystemID;
            icsJ1850Struct.NetworkID = icsCANStruct.NetworkID;
            icsJ1850Struct.NodeID = icsCANStruct.NodeID;
            icsJ1850Struct.Protocol = icsCANStruct.Protocol;
            icsJ1850Struct.MessagePieceID = icsCANStruct.MessagePieceID;
            icsJ1850Struct.ExtraDataPtrEnabled = icsCANStruct.ExtraDataPtrEnabled;
            icsJ1850Struct.NumberBytesHeader = icsCANStruct.NumberBytesHeader;
            icsJ1850Struct.NumberBytesData = icsCANStruct.NumberBytesData;
            icsJ1850Struct.DescriptionID = icsCANStruct.DescriptionID;
            icsJ1850Struct.Header1 = Convert.ToByte(icsCANStruct.ArbIDOrHeader & 0xff);
            icsJ1850Struct.Header2 = Convert.ToByte((0xFF00 & icsCANStruct.ArbIDOrHeader) / 256);
            icsJ1850Struct.Header3 = Convert.ToByte((0xFF0000 & icsCANStruct.ArbIDOrHeader) / 65536);
            icsJ1850Struct.Data1 = icsCANStruct.Data1;
            icsJ1850Struct.Data2 = icsCANStruct.Data2;
            icsJ1850Struct.Data3 = icsCANStruct.Data3;
            icsJ1850Struct.Data4 = icsCANStruct.Data4;
            icsJ1850Struct.Data5 = icsCANStruct.Data5;
            icsJ1850Struct.Data6 = icsCANStruct.Data6;
            icsJ1850Struct.Data7 = icsCANStruct.Data7;
            icsJ1850Struct.Data8 = icsCANStruct.Data8;
            icsJ1850Struct.ACK4 = Convert.ToByte((icsCANStruct.StatusBitField3 & 0xFF000000) >> 24);
            icsJ1850Struct.ACK3 = Convert.ToByte((icsCANStruct.StatusBitField3 & 0xFF0000) >> 16);
            icsJ1850Struct.ACK2 = Convert.ToByte((icsCANStruct.StatusBitField3 & 0xFF00) >> 8);
            icsJ1850Struct.ACK1 = Convert.ToByte((icsCANStruct.StatusBitField3 & 0xFF));
            icsJ1850Struct.ACK8 = Convert.ToByte((icsCANStruct.StatusBitField4 & 0xFF000000) >> 24);
            icsJ1850Struct.ACK7 = Convert.ToByte((icsCANStruct.StatusBitField4 & 0xFF0000) >> 16);
            icsJ1850Struct.ACK6 = Convert.ToByte((icsCANStruct.StatusBitField4 & 0xFF00) >> 8);
            icsJ1850Struct.ACK5 = Convert.ToByte((icsCANStruct.StatusBitField4 & 0xFF));
            icsJ1850Struct.iExtraDataPtr = icsCANStruct.iExtraDataPtr;
            icsJ1850Struct.MiscData = icsCANStruct.MiscData;
        }

        public static void ConvertJ1850toCAN(ref icsSpyMessage icsCANStruct, ref icsSpyMessageJ1850 icsJ1850Struct)
        {
            //Becuse memcopy is not available.  
            icsCANStruct.StatusBitField = icsJ1850Struct.StatusBitField;
            icsCANStruct.StatusBitField2 = icsJ1850Struct.StatusBitField2;
            icsCANStruct.TimeHardware = icsJ1850Struct.TimeHardware;
            icsCANStruct.TimeHardware2 = icsJ1850Struct.TimeHardware2;
            icsCANStruct.TimeSystem = icsJ1850Struct.TimeSystem;
            icsCANStruct.TimeSystem2 = icsJ1850Struct.TimeSystem2;
            icsCANStruct.TimeStampHardwareID = icsJ1850Struct.TimeStampHardwareID;
            icsCANStruct.TimeStampSystemID = icsJ1850Struct.TimeStampSystemID;
            icsCANStruct.NetworkID = icsJ1850Struct.NetworkID;
            icsCANStruct.NodeID = icsJ1850Struct.NodeID;
            icsCANStruct.Protocol = icsJ1850Struct.Protocol;
            icsCANStruct.MessagePieceID = icsJ1850Struct.MessagePieceID;
            icsCANStruct.ExtraDataPtrEnabled = icsJ1850Struct.ExtraDataPtrEnabled;
            icsCANStruct.NumberBytesHeader = icsJ1850Struct.NumberBytesHeader;
            icsCANStruct.NumberBytesData = icsJ1850Struct.NumberBytesData;
            icsCANStruct.DescriptionID = icsJ1850Struct.DescriptionID;
            icsCANStruct.ArbIDOrHeader = (icsJ1850Struct.Header3 * 65536) + (icsJ1850Struct.Header2 * 256) + icsJ1850Struct.Header1;
            icsCANStruct.Data1 = icsJ1850Struct.Data1;
            icsCANStruct.Data2 = icsJ1850Struct.Data2;
            icsCANStruct.Data3 = icsJ1850Struct.Data3;
            icsCANStruct.Data4 = icsJ1850Struct.Data4;
            icsCANStruct.Data5 = icsJ1850Struct.Data5;
            icsCANStruct.Data6 = icsJ1850Struct.Data6;
            icsCANStruct.Data7 = icsJ1850Struct.Data7;
            icsCANStruct.Data8 = icsJ1850Struct.Data8;
            icsCANStruct.StatusBitField3 = (icsJ1850Struct.ACK1 << 24) + (icsJ1850Struct.ACK2 << 16) + (icsJ1850Struct.ACK3 << 8) + (icsJ1850Struct.ACK4);
            icsCANStruct.StatusBitField4 = (icsJ1850Struct.ACK5 << 24) + (icsJ1850Struct.ACK6 << 16) + (icsJ1850Struct.ACK7 << 8) + (icsJ1850Struct.ACK8);
            icsCANStruct.iExtraDataPtr = icsJ1850Struct.iExtraDataPtr;
            icsCANStruct.MiscData = icsJ1850Struct.MiscData;
        }

        public static string ConvertToHex(string sInput)
        {
            string sOut;
            uint uiDecimal = 0;

            try
            {
                //Convert text string to unsigned Int32eger
                uiDecimal = checked((uint)System.Convert.ToUInt32(sInput));
            }
            catch (System.OverflowException)
            {
                sOut = "Overflow";
                return sOut;
            }
            //Format unsigned Int32eger value to hex 
            sOut = String.Format("{0:x2}", uiDecimal);
            return sOut;
        }

        public static Int32 ConvertFromHex(string num)
        {
            //To hold our converted unsigned Int32eger32 value
            uint uiHex = 0;
            try
            {
                // Convert hex string to unsigned Int32eger
                uiHex = System.Convert.ToUInt32(num, 16);
            }
            catch (System.OverflowException)
            {
                //
            }
            return Convert.ToInt32(uiHex);
        }

        public static Int32 GetNetworkIDfromString(ref string NetworkName)
        {
            switch (NetworkName)
            {
                case "DEVICE":
                    return ((int)eNETWORK_ID.NETID_DEVICE);
                case "HSCAN":
                    return ((int)eNETWORK_ID.NETID_HSCAN);
                case "MSCAN":
                    return ((int)eNETWORK_ID.NETID_MSCAN);
                case "SWCAN":
                    return ((int)eNETWORK_ID.NETID_SWCAN);
                case "LSFTCAN":
                    return ((int)eNETWORK_ID.NETID_LSFTCAN);
                case "FORDSCP":
                    return ((int)eNETWORK_ID.NETID_FORDSCP);
                case "J1708":
                    return ((int)eNETWORK_ID.NETID_J1708);//No longer Supported
                case "JVPW":
                    return ((int)eNETWORK_ID.NETID_JVPW);//No longer Supported
                case "ISO":
                    return ((int)eNETWORK_ID.NETID_ISO);
                case "ISO2":
                    return ((int)eNETWORK_ID.NETID_ISO2);
                case "ISO14230":
                    return ((int)eNETWORK_ID.NETID_ISO14230);
                case "LIN":
                    return ((int)eNETWORK_ID.NETID_LIN);
                case "ISO3":
                    return ((int)eNETWORK_ID.NETID_ISO3);
                case "HSCAN2":
                    return ((int)eNETWORK_ID.NETID_HSCAN2);
                case "HSCAN3":
                    return ((int)eNETWORK_ID.NETID_HSCAN3);
                case "ISO4":
                    return ((int)eNETWORK_ID.NETID_ISO4);
                case "LIN2":
                    return ((int)eNETWORK_ID.NETID_LIN2);
                case "LIN3":
                    return ((int)eNETWORK_ID.NETID_LIN3);
                case "LIN4":
                    return ((int)eNETWORK_ID.NETID_LIN4);
                case "LIN5":
                    return ((int)eNETWORK_ID.NETID_LIN5);
                case "MOST":
                    return ((int)eNETWORK_ID.NETID_MOST);
                case "CGI":
                    return ((int)eNETWORK_ID.NETID_CGI);
                case "HSCAN4":
                    return ((int)eNETWORK_ID.NETID_HSCAN4);
                case "HSCAN5":
                    return ((int)eNETWORK_ID.NETID_HSCAN5);
                case "UART":
                    return ((int)eNETWORK_ID.NETID_UART);
                case "UART2":
                    return ((int)eNETWORK_ID.NETID_UART2);
                case "UART3":
                    return ((int)eNETWORK_ID.NETID_UART3);
                case "UART4":
                    return ((int)eNETWORK_ID.NETID_UART4);
                case "SWCAN2":
                    return ((int)eNETWORK_ID.NETID_SWCAN2);
                case "ETHERNE":
                    return ((int)eNETWORK_ID.NETID_ETHERNET_DAQ);
                case "TEXTAP":
                    return ((int)eNETWORK_ID.NETID_TEXTAPI_TO_HOST);
                case "FLEXRAY1A":
                    return ((int)eNETWORK_ID.NETID_FLEXRAY1A);
                case "FLEXRAY1B":
                    return ((int)eNETWORK_ID.NETID_FLEXRAY1B);
                case "FLEXRAY2A":
                    return ((int)eNETWORK_ID.NETID_FLEXRAY2A);
                case "FLEXRAY2B":
                    return ((int)eNETWORK_ID.NETID_FLEXRAY2B);
                case "FLEXRAY":
                    return ((int)eNETWORK_ID.NETID_FLEXRAY);
                case "MOST25":
                    return ((int)eNETWORK_ID.NETID_MOST25);
                case "MOST50":
                    return ((int)eNETWORK_ID.NETID_MOST50);
                case "MOST150":
                    return ((int)eNETWORK_ID.NETID_MOST150);
                case "ETHERNET":
                    return ((int)eNETWORK_ID.NETID_ETHERNET);
                case "GMFSA":
                    return ((int)eNETWORK_ID.NETID_GMFSA);
                case "TCP":
                    return ((int)eNETWORK_ID.NETID_TCP);
                case "HSCAN6":
                    return ((int)eNETWORK_ID.NETID_HSCAN6);
                case "HSCAN7":
                    return ((int)eNETWORK_ID.NETID_HSCAN7);
                case "LIN6":
                    return ((int)eNETWORK_ID.NETID_LIN6);
                case "LSFTCAN2":
                    return ((int)eNETWORK_ID.NETID_LSFTCAN2);
                case "OP_ETHERNET1":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET1);
                case "OP_ETHERNET2":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET2);
                case "OP_ETHERNET3":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET3);
                case "OP_ETHERNET4":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET4);
                case "OP_ETHERNET5":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET5);
                case "OP_ETHERNET6":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET6);
                case "OP_ETHERNET7":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET7);
                case "OP_ETHERNET8":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET8);
                case "OP_ETHERNET9":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET9);
                case "OP_ETHERNET10":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET10);
                case "OP_ETHERNET11":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET11);
                case "OP_ETHERNET12":
                    return ((int)eNETWORK_ID.NETID_OP_ETHERNET12);
            }
            return (-1);
        }


        public static string GetStringForNetworkID(Int16 lNetworkID)
        {
            string sTempOutput = "N/A";
            switch (lNetworkID)
            {
                case (short)eNETWORK_ID.NETID_DEVICE:
                    sTempOutput = "DEVICE";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN:
                    sTempOutput = "HSCAN";
                    break;
                case (short)eNETWORK_ID.NETID_MSCAN:
                    sTempOutput = "MSCAN";
                    break;
                case (short)eNETWORK_ID.NETID_SWCAN:
                    sTempOutput = "SWCAN";
                    break;
                case (short)eNETWORK_ID.NETID_LSFTCAN:
                    sTempOutput = "LSFTCAN";
                    break;
                case (short)eNETWORK_ID.NETID_FORDSCP:
                    sTempOutput = "FORDSCP";
                    break;
                case (short)eNETWORK_ID.NETID_J1708://No longer Supported
                    sTempOutput = "J1708";
                    break;
                case (short)eNETWORK_ID.NETID_JVPW://No longer Supported
                    sTempOutput = "JVPW";
                    break;
                case (short)eNETWORK_ID.NETID_ISO:
                    sTempOutput = "ISO";
                    break;
                case (short)eNETWORK_ID.NETID_ISO2:
                    sTempOutput = "ISO2";
                    break;
                case (short)eNETWORK_ID.NETID_ISO14230:
                    sTempOutput = "ISO14230";
                    break;
                case (short)eNETWORK_ID.NETID_LIN:
                    sTempOutput = "LIN";
                    break;
                case (short)eNETWORK_ID.NETID_ISO3:
                    sTempOutput = "ISO3";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN2:
                    sTempOutput = "HSCAN2";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN3:
                    sTempOutput = "HSCAN3";
                    break;
                case (short)eNETWORK_ID.NETID_ISO4:
                    sTempOutput = "ISO4";
                    break;
                case (short)eNETWORK_ID.NETID_LIN2:
                    sTempOutput = "LIN2";
                    break;
                case (short)eNETWORK_ID.NETID_LIN3:
                    sTempOutput = "LIN3";
                    break;
                case (short)eNETWORK_ID.NETID_LIN4:
                    sTempOutput = "LIN4";
                    break;
                case (short)eNETWORK_ID.NETID_LIN5:
                    sTempOutput = "LIN5";
                    break;
                case (short)eNETWORK_ID.NETID_MOST:
                    sTempOutput = "MOST";
                    break;
                case (short)eNETWORK_ID.NETID_CGI:
                    sTempOutput = "CGI";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN4:
                    sTempOutput = "HSCAN4";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN5:
                    sTempOutput = "HSCAN5";
                    break;
                case (short)eNETWORK_ID.NETID_UART:
                    sTempOutput = "UART";
                    break;
                case (short)eNETWORK_ID.NETID_UART2:
                    sTempOutput = "UART2";
                    break;
                case (short)eNETWORK_ID.NETID_UART3:
                    sTempOutput = "UART3";
                    break;
                case (short)eNETWORK_ID.NETID_UART4:
                    sTempOutput = "UART4";
                    break;
                case (short)eNETWORK_ID.NETID_SWCAN2:
                    sTempOutput = "SWCAN2";
                    break;
                case (short)eNETWORK_ID.NETID_ETHERNET_DAQ:
                    sTempOutput = "ETHERNE";
                    break;
                case (short)eNETWORK_ID.NETID_TEXTAPI_TO_HOST:
                    sTempOutput = "TEXTAP";
                    break;
                case (short)eNETWORK_ID.NETID_FLEXRAY1A:
                    sTempOutput = "FLEXRAY1A";
                    break;
                case (short)eNETWORK_ID.NETID_FLEXRAY1B:
                    sTempOutput = "FLEXRAY1B";
                    break;
                case (short)eNETWORK_ID.NETID_FLEXRAY2A:
                    sTempOutput = "FLEXRAY2A";
                    break;
                case (short)eNETWORK_ID.NETID_FLEXRAY2B:
                    sTempOutput = "FLEXRAY2B";
                    break;
                case (short)eNETWORK_ID.NETID_FLEXRAY:
                    sTempOutput = "FLEXRAY";
                    break;
                case (short)eNETWORK_ID.NETID_MOST25:
                    sTempOutput = "MOST25";
                    break;
                case (short)eNETWORK_ID.NETID_MOST50:
                    sTempOutput = "MOST50";
                    break;
                case (short)eNETWORK_ID.NETID_MOST150:
                    sTempOutput = "MOST150";
                    break;
                case (short)eNETWORK_ID.NETID_ETHERNET:
                    sTempOutput = "ETHERNET";
                    break;
                case (short)eNETWORK_ID.NETID_GMFSA:
                    sTempOutput = "GMFSA";
                    break;
                case (short)eNETWORK_ID.NETID_TCP:
                    sTempOutput = "TCP";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN6:
                    sTempOutput = "HSCAN6";
                    break;
                case (short)eNETWORK_ID.NETID_HSCAN7:
                    sTempOutput = "HSCAN7";
                    break;
                case (short)eNETWORK_ID.NETID_LIN6:
                    sTempOutput = "LIN6";
                    break;
                case (short)eNETWORK_ID.NETID_LSFTCAN2:
                    sTempOutput = "LSFTCAN2";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET1:
                    sTempOutput = "OP_ETHERNET1";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET2:
                    sTempOutput = "OP_ETHERNET2";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET3:
                    sTempOutput = "OP_ETHERNET3";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET4:
                    sTempOutput = "OP_ETHERNET4";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET5:
                    sTempOutput = "OP_ETHERNET5";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET6:
                    sTempOutput = "OP_ETHERNET6";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET7:
                    sTempOutput = "OP_ETHERNET7";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET8:
                    sTempOutput = "OP_ETHERNET8";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET9:
                    sTempOutput = "OP_ETHERNET9";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET10:
                    sTempOutput = "OP_ETHERNET10";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET11:
                    sTempOutput = "OP_ETHERNET11";
                    break;
                case (short)eNETWORK_ID.NETID_OP_ETHERNET12:
                    sTempOutput = "OP_ETHERNET12";
                    break;
                default:
                    sTempOutput = "Other";
                    break;
            }
            return sTempOutput;
        }

    }
}