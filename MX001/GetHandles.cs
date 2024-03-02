using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MerryKing
{
    static class GetHandles
    {
        #region 参数及引用区
        public static IntPtr[] headsethandle = new IntPtr[4];
        public static IntPtr[] donglehandle = new IntPtr[4];
        public static string[] headsetpath = new string[4];
        public static string[] donglepath = new string[4];
        public static IntPtr Handle = IntPtr.Zero;
        public static string Path = "";
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern int CloseHandle(IntPtr hFile);
        [DllImport("hid.dll")]//获得GUID
        private static extern void HidD_GetHidGuid(ref Guid hidGuid);
        [DllImport("setupapi.dll", SetLastError = true)]//过滤设备，获取需要的设备
        private static extern IntPtr SetupDiGetClassDevs(ref Guid gClass, [MarshalAs(UnmanagedType.LPStr)] string strEnumerator, IntPtr hParent, Digcf nFlags);
        public enum Digcf  //3
        {
            DigcfDefault = 0x1,//返回与系统默认设备相关的设备
            DigcfPresent = 0x2,//返回当前存在的设备
            DigcfAllclasses = 0x4,//返回所有安装的设备
            DigcfProfile = 0x8,//只返回当前硬件配置文件的设备
            DigcfDeviceinterface = 0x10//返回所有支持的设备
        }
        protected struct SpDeviceInterfaceData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }
        protected struct SpDeviceInfoData
        {
            public int Size;
            public Guid InterfaceClassGuid;
            public int Flags;
            public int Reserved;
        }
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern Boolean SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, uint devInfo, ref Guid interfaceClassGuid, uint memberIndex, ref SpDeviceInterfaceData deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, ref SpDeviceInterfaceData deviceInterfaceData, IntPtr deviceInterfaceDetailData,
                                                                 uint deviceInterfaceDetailDataSize, ref uint requiredSize, IntPtr deviceInfoData);
        [StructLayout(LayoutKind.Sequential, Pack = 2)]//2
        internal struct SpDeviceInterfaceDetailData
        {
            public int Size;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DevicePath;
        }
        private const uint GENERIC_READ = 0x80000000;
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint FILE_SHARE_WRITE = 0x2;
        private const uint FILE_SHARE_READ = 0x1;
        private const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        private const uint OPEN_EXISTING = 3;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile([MarshalAs(UnmanagedType.LPStr)] string strName, uint nAccess, uint nShareMode, IntPtr lpSecurity, uint nCreationFlags, uint nAttributes, IntPtr lpTemplate);
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr lpDeviceInfoSet, ref SpDeviceInterfaceData oInterfaceData, ref SpDeviceInterfaceDetailData oDetailData, uint nDeviceInterfaceDetailDataSize, ref uint nRequiredSize, IntPtr lpDeviceInfoData);
        [DllImport("setupapi.dll", SetLastError = true)]
        internal static extern IntPtr SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiEnumDeviceInfo(IntPtr hDevInfo, uint Widx,ref SpDeviceInfoData deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiRemoveDevice(IntPtr hDevInfo,  ref SpDeviceInfoData deviceInterfaceData);
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern bool SetupDiGetDeviceRegistryProperty(IntPtr hDevInfo, ref SpDeviceInfoData deviceInterfaceData, SPDRP OPTIONAL, uint PropertyRegDataType, StringBuilder PropertyBuffer, uint PropertyBufferSize, uint RequiredSize);
        public enum SPDRP
        {
            SPDRP_DEVICEDESC = 0,
            SPDRP_HARDWAREID = 0x1,
            SPDRP_COMPATIBLEIDS = 0x2,
            SPDRP_UNUSED0 = 0x3,
            SPDRP_SERVICE = 0x4,
            SPDRP_UNUSED1 = 0x5,
            SPDRP_UNUSED2 = 0x6,
            SPDRP_CLASS = 0x7,
            SPDRP_CLASSGUID = 0x8,
            SPDRP_DRIVER = 0x9,
            SPDRP_CONFIGFLAGS = 0xA,
            SPDRP_MFG = 0xB,
            SPDRP_FRIENDLYNAME = 0xC,
            SPDRP_LOCATION_INFORMATION = 0xD,
            SPDRP_PHYSICAL_DEVICE_OBJECT_NAME = 0xE,
            SPDRP_CAPABILITIES = 0xF,
            SPDRP_UI_NUMBER = 0x10,
            SPDRP_UPPERFILTERS = 0x11,
            SPDRP_LOWERFILTERS = 0x12,
            SPDRP_BUSTYPEGUID = 0x13,
            SPDRP_LEGACYBUSTYPE = 0x14,
            SPDRP_BUSNUMBER = 0x15,
            SPDRP_ENUMERATOR_NAME = 0x16,
            SPDRP_SECURITY = 0x17,
            SPDRP_SECURITY_SDS = 0x18,
            SPDRP_DEVTYPE = 0x19,
            SPDRP_EXCLUSIVE = 0x1A,
            SPDRP_CHARACTERISTICS = 0x1B,
            SPDRP_ADDRESS = 0x1C,
            SPDRP_UI_NUMBER_DESC_FORMAT = 0x1E,
            SPDRP_MAXIMUM_PROPERTY = 0x1F
        }

        #endregion
        #region 获取装置路径
        private static bool GetHidDevicePath(string headsetPID,string headsetVID,string donglePID,string dongleVID)
        {
            Guid hidGuid = Guid.Empty;
            Regex TXregPid = new Regex(donglePID.ToLower());
            Regex TXregVid = new Regex(dongleVID.ToLower());
            Regex RXregPid = new Regex(headsetPID.ToLower());
            Regex RXregVid = new Regex(headsetVID.ToLower());
            Regex[] ExternAgs = { new Regex(""), new Regex("col02"), new Regex("col03"), new Regex("col04") };
            bool result = true;
            bool resultflag= false;
            IntPtr hidHandle = IntPtr.Zero;
            UInt32 deviceSerialNumber = 0;
            HidD_GetHidGuid(ref hidGuid);

            IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfDeviceinterface);//过滤设备，获取需要的设备
            try
            {
                SpDeviceInterfaceData deviceInterfaceData = new SpDeviceInterfaceData();
                deviceInterfaceData.Size = Marshal.SizeOf(deviceInterfaceData);
                while (result)
                {
                  
                    result = SetupDiEnumDeviceInterfaces(hDevInfo, 0, ref hidGuid, deviceSerialNumber, ref deviceInterfaceData);//获取设备，true获取到                   
                    if (result)
                    {
                        uint nRequiredSize = 0;
                        SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero);

                        SpDeviceInterfaceDetailData detailData = new SpDeviceInterfaceDetailData();
                        detailData.Size = 5;	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
                        result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, ref detailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero);//获取接口的详细信息，必须调用两次，一次返回长度，二次获取数据

                        if (result)
                        {
                            if (detailData.DevicePath != null)
                            {
                                Match MatchTXPid = TXregPid.Match(detailData.DevicePath);
                                Match MatchRxPid = RXregPid.Match(detailData.DevicePath);
                                Match MatchTXVID = TXregVid.Match(detailData.DevicePath);
                                Match MatchRXVID = RXregVid.Match(detailData.DevicePath);
                                Match[] mathExternAgs = new Match[4];
                                for (int i = 0; i < 4; i++)
                                {
                                    mathExternAgs[i]= ExternAgs[i].Match(detailData.DevicePath);
                                }
                                if (MatchTXPid.Success && MatchTXVID.Success)
                                {
                                    for(int i = 0; i < 4; i++)
                                    {
                                        if (mathExternAgs[i].Success)
                                        {
                                            donglepath[i]= detailData.DevicePath;
                                            resultflag = true;
                                        }
                                    }                                  
                                }
                                else if (MatchRxPid.Success && MatchRXVID.Success)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (mathExternAgs[i].Success)
                                        {
                                            headsetpath[i] = detailData.DevicePath;
                                            resultflag = true;
                                        }
                                    }                                   
                                }
                            }
                            deviceSerialNumber++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                resultflag = false;
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            return resultflag;
        }
        #endregion
        #region 获取装置路径1
        private static bool GetHidDevicePath( string PID, string VID,string col)
        {
            Guid hidGuid = Guid.Empty;
            Regex TXregPid = new Regex(PID.ToLower());
            Regex TXregVid = new Regex(VID.ToLower());
            Regex ExternAgs = new Regex(col.ToLower());
            bool result = true;
            IntPtr hidHandle = IntPtr.Zero;
            uint deviceSerialNumber = 0;
            HidD_GetHidGuid(ref hidGuid);

            IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, Digcf.DigcfPresent | Digcf.DigcfDeviceinterface);//过滤设备，获取需要的设备
            try
            {
                SpDeviceInterfaceData deviceInterfaceData = new SpDeviceInterfaceData();
                deviceInterfaceData.Size =Marshal.SizeOf(deviceInterfaceData);
                while (result)
                {

                    result = SetupDiEnumDeviceInterfaces(hDevInfo, 0, ref hidGuid, deviceSerialNumber, ref deviceInterfaceData);//获取设备，true获取到                   
                    if (result)
                    {
                        uint nRequiredSize = 0;
                        SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, IntPtr.Zero, 0, ref nRequiredSize, IntPtr.Zero);

                        SpDeviceInterfaceDetailData detailData = new SpDeviceInterfaceDetailData();
                        detailData.Size = 5;	// hardcoded to 5! Sorry, but this works and trying more future proof versions by setting the size to the struct sizeof failed miserably. If you manage to sort it, mail me! Thx
                        result = SetupDiGetDeviceInterfaceDetail(hDevInfo, ref deviceInterfaceData, ref detailData, nRequiredSize, ref nRequiredSize, IntPtr.Zero);//获取接口的详细信息，必须调用两次，一次返回长度，二次获取数据

                        if (result)
                        {
                            if (detailData.DevicePath != null)
                            {
                                Match MatchPid = TXregPid.Match(detailData.DevicePath);
                                Match MatchVID = TXregVid.Match(detailData.DevicePath);
                                Match mathExternAgs= ExternAgs.Match(detailData.DevicePath);
                                
                                if (MatchPid.Success && MatchVID.Success&&mathExternAgs.Success)
                                {                                  
                                            Path = detailData.DevicePath;
                                            return true;                                                                    
                                }                             
                            }
                            deviceSerialNumber++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            return false;
        }
        #endregion
        #region 对外方法
        public static bool gethandle(string headsetPID, string headsetVID, string donglePID, string dongleVID)
        {
            bool flag = true;
            try
            {

                flag=GetHidDevicePath(headsetPID, headsetVID, donglePID, dongleVID);
               for(int i=0;i<4; i++)
                {
                    headsethandle[i] = GetHidDeviceHandle(headsetpath[i]);
                }
                for (int i = 0; i < 4; i++)
                {
                    donglehandle[i] = GetHidDeviceHandle(donglepath[i]);
                }

            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }
        #endregion
        #region 对外方法
        public static bool gethandle(string PID, string VID, string col)
        {
            bool flag = true;
            try
            {
                flag = GetHidDevicePath(PID, VID,col);
                Handle = GetHidDeviceHandle(Path);            
            }
            catch (Exception ex)
            {
                flag = false;
            }
            return flag;
        }
        #endregion
        #region 将路径转换成句柄
        private static IntPtr GetHidDeviceHandle(string HidDevicePath)
        {
            IntPtr _HIDWriteHandle = IntPtr.Zero;
            if (!String.IsNullOrEmpty(HidDevicePath))
            {

                _HIDWriteHandle = CreateFile(HidDevicePath, GENERIC_WRITE | GENERIC_READ, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            }
            return _HIDWriteHandle;
        }
        #endregion
        #region 释放句柄
        private static void CloseHandle()
        {
            try
            {
                foreach(IntPtr handle in headsethandle)
                {
                    if (handle != IntPtr.Zero) CloseHandle(handle);
                }
                foreach (IntPtr handle in donglehandle)
                {
                    if (handle != IntPtr.Zero) CloseHandle(handle);
                }               
            }
            catch
            {

            }
        }
        #endregion
        #region 删除驱动
        public static int deletedriver(List<string> PID)
        {
            Guid hidGuid = Guid.Empty;        
            bool result = true;
            bool resultflag = false;
            IntPtr hidHandle = IntPtr.Zero;
            UInt32 deviceSerialNumber = 0;
            HidD_GetHidGuid(ref hidGuid);
            int s=0;
            IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, Digcf.DigcfAllclasses | Digcf.DigcfDeviceinterface);//过滤设备，获取需要的设备
            try
            {
                SpDeviceInfoData devi = new SpDeviceInfoData();
                devi.Size= Marshal.SizeOf(devi);
                StringBuilder by = new StringBuilder();
               
                uint zzz = 0;
                while (result)
                {
                    result = SetupDiEnumDeviceInfo(hDevInfo, deviceSerialNumber,ref devi);                   
                    if (result)
                    {
                        resultflag = SetupDiGetDeviceRegistryProperty(hDevInfo, ref devi, SPDRP.SPDRP_DRIVER,
                          0, by, 2048, zzz);
                        if (!PID.Contains(by.ToString()))
                        {
                            resultflag = SetupDiRemoveDevice(hDevInfo, ref devi);
                            s++;
                        }
                    }
                    deviceSerialNumber++;
                }
            }
            catch (Exception ex)
            {

                resultflag = false;
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            return s;
        }
        #endregion
        #region 记住驱动
        public static List<string> remenberdriver()
        {
            Guid hidGuid = Guid.Empty;       
            bool result = true;
            bool resultflag = false;
            IntPtr hidHandle = IntPtr.Zero;
            UInt32 deviceSerialNumber = 0;
            HidD_GetHidGuid(ref hidGuid);
            List<string> s1 = new List<string>();
            int s = 0;
            string z = "";
            IntPtr hDevInfo = SetupDiGetClassDevs(ref hidGuid, null, IntPtr.Zero, Digcf.DigcfProfile | Digcf.DigcfDeviceinterface);//过滤设备，获取需要的设备
            try
            {
                SpDeviceInfoData devi = new SpDeviceInfoData();
                devi.Size = Marshal.SizeOf(devi);
                StringBuilder by = new StringBuilder();

                uint zzz = 0;
                while (result)
                {
                    result = SetupDiEnumDeviceInfo(hDevInfo, deviceSerialNumber, ref devi);
                    //result = SetupDiEnumDeviceInterfaces(hDevInfo, 0, ref hidGuid, deviceSerialNumber, ref deviceInterfaceData);//获取设备，true获取到        
                    if (result)
                    {
                        resultflag = SetupDiGetDeviceRegistryProperty(hDevInfo, ref devi, SPDRP.SPDRP_DRIVER,
                          0, by, 2048, zzz);
                        s1.Add(by.ToString());
                    }
                    deviceSerialNumber++;
                }
            }
            catch (Exception ex)
            {

                resultflag = false;
            }
            finally
            {
                SetupDiDestroyDeviceInfoList(hDevInfo);
            }
            return s1;
        }
        #endregion

    }
}
