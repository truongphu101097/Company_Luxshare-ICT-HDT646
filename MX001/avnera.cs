using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SCPI
{

    internal class avnera
    {
        private static RfAntennaDiversity Antenna;
        private static byte Dut_Channel = 0;
        private static int dut_handle = 0;
        private static int dut_index = -1;
        private static uint dut_pid = 0;
        private static uint dut_vid = 0;
        private static bool ErrorFlag = true;
        const string path = @".\hostapi_vmi.dll";

        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        private static extern Status AvDllCloseHandle(int handle);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        private static extern Status AvDllFindDevice(DeviceMatchBits bitmask, DeviceMatchData pMatch, int pDevFound);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        private static extern Status AvDllInitialize();
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        private static extern Status AvDllListDevices(DeviceType dev, int count, ref ushort devices, ref uint unique_ids);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        private static extern Status AvDllOpenDevice(DeviceType dev, int index, ref int pId);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        private static extern Status AvDllShutdown();
        private static void Initiallize()
        {
            if (AvDllInitialize() != Status.Status_SUCCESS)
            {
                ErrorFlag = false;
            }
        }

        public static int Main(string[] args)
        {
            uint result = 0;
            uint num2 = 0;
            uint num3 = 0;
            byte num4 = 0;
            if ((args.Length == 4) && uint.TryParse(args[0], NumberStyles.HexNumber, null, out result))
            {
                dut_vid = result;
            }
            else
            {
                return 0;
            }
            if (uint.TryParse(args[1], NumberStyles.HexNumber, null, out num2))
            {
                dut_pid = num2;
            }
            else
            {
                return 0;
            }
            if (uint.TryParse(args[2], out num3))
            {
                switch (num3)
                {
                    case 0:
                        Antenna = RfAntennaDiversity.RfAntennaDiversity_Lock_0;
                        goto Label_007C;

                    case 1:
                        Antenna = RfAntennaDiversity.RfAntennaDiversity_Lock_1;
                        goto Label_007C;
                }
            }
            return 0;
        Label_007C:
            if (byte.TryParse(args[3], out num4))
            {
                Dut_Channel = num4;
            }
            else
            {
                return 0;
            }
            Initiallize();
            if (!ErrorFlag)
            {
                return 0;
            }
            MatchTargetDevice();
            if (!ErrorFlag)
            {
                return 0;
            }
            OpenDUT();
            if (!ErrorFlag)
            {
                return 0;
            }
            rfTest();
            if (!ErrorFlag)
            {
                return 0;
            }
            Console.WriteLine("OK");
            return 1;
        }

        private static void MatchTargetDevice()
        {
            if (ErrorFlag)
            {
                ushort[] numArray = new ushort[5];
                uint[] numArray2 = new uint[5];
                uint num = 0;
                if (ErrorFlag)
                {
                    num = (uint)AvDllListDevices(DeviceType.DeviceType_Internal, 5, ref numArray[0], ref numArray2[0]);
                    if (num == 0)
                    {
                        ErrorFlag = false;
                    }
                    else
                    {
                        for (int i = 0; i < num; i++)
                        {
                            uint num2 = (numArray2[i] >> 0x10) & 0xffff;
                            uint num3 = numArray2[i] & 0xffff;
                            if ((num2 == dut_vid) && (num3 == dut_pid))
                            {
                                dut_index = i;
                                break;
                            }
                        }
                        if (dut_index == -1)
                        {
                            ErrorFlag = false;
                        }
                    }
                }
            }
        }

        private static void OpenDUT()
        {
            if (ErrorFlag)
            {
                if (AvDllOpenDevice(DeviceType.DeviceType_Internal, dut_index, ref dut_handle) == Status.Status_SUCCESS)
                {
                    if (dut_handle == 0)
                    {
                        ErrorFlag = false;
                    }
                }
                else
                {
                    ErrorFlag = false;
                }
            }
        }

        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        public static extern Status rfAntennaDiversitySetup(int handle, RfAntennaDiversity control);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        public static extern Status rfContinuousTransmitEnable(int handle, byte modulation);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        public static extern Status rfDynamicChannelSetup(int handle);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        public static extern Status rfFixedChannelSetup(int handle, byte channel);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        public static extern Status rfRevertToFlashSettings(int handle);
        [DllImport(path, CallingConvention = CallingConvention.StdCall)]
        public static extern Status rfSetTxPower(int handle, byte power);
        private static void rfTest()
        {
            if (ErrorFlag)
            {
                if (rfAntennaDiversitySetup(dut_handle, Antenna) == Status.Status_SUCCESS)
                {
                    Console.WriteLine(string.Format("rfAntennaDiversitySetup OK,Antenal={0}", Antenna));
                    if (rfFixedChannelSetup(dut_handle, Dut_Channel) != Status.Status_SUCCESS)
                    {
                        ErrorFlag = false;
                    }
                    else
                    {
                        Console.WriteLine(string.Format("rfFixedChannelSetup OK,Channel={0}", Dut_Channel));
                        if (rfContinuousTransmitEnable(dut_handle, 0) != Status.Status_SUCCESS)
                        {
                            ErrorFlag = false;
                        }
                    }
                }
                else
                {
                    ErrorFlag = false;
                }
            }
        }

        public enum ChipType
        {
            ChipType_Falcon = 0,
            ChipType_None = 0xff,
            ChipType_Raptor = 1,
            ChipType_Swift = 2
        }

        public enum DeviceMatchBits
        {
            DeviceMatchBits_DeviceId = 2,
            DeviceMatchBits_Query = 2,
            DeviceMatchBits_Revision = 4,
            DeviceMatchBits_Serial = 1,
            DeviceMatchBits_Type = 8
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DeviceMatchData
        {
            public avnera.DeviceType devtype;
            public uint serial;
            public uint deviceId;
            public uint revision;
            public avnera.ChipType chiptype;
        }

        public enum DeviceType
        {
            DeviceType_Internal,
            DeviceType_Anteater,
            DeviceType_Aardvark
        }

        public enum RfAntennaDiversity
        {
            RfAntennaDiversity_Dynamic,
            RfAntennaDiversity_Lock_0,
            RfAntennaDiversity_Lock_1
        }

        public enum Status
        {
            Status_ARGUMENT_ERROR = -4,
            Status_BAD_CHIP_ID = -6,
            Status_FAILURE = -1,
            Status_INVALID_HANDLE = -3,
            Status_SPI_ASSERT = -5,
            Status_SUCCESS = 0,
            Status_UNIMPLEMENTED = -2
        }
    }

}
