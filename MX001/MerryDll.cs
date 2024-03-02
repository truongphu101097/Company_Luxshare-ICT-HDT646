using MerryTest.testitem;
using MerryTestFramework.testitem.Computer;
//using MerryTestFramework.testitem.Headset;
//using SwATE_Net;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MerryKing;
using BlueNinjaSoftware.HIDLib;
using static MerryKing.GetHandles;
using System.Runtime.InteropServices;
using static MerryTest.testitem.GetHandle;
using MX001;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Runtime.InteropServices.ComTypes;

namespace MerryDllFramework
{
    public class MerryDll : IMerryDll
    {
        // 天线通道（经示波器调测，通道要加1才能正确显示）
        public static string CH1 = "1";  // 原值为 "1"
        public static string CH20 = "20"; // 原值为 "20"
        public static string CH38 = "38"; // 原值为 "38"
        // 天线
        public static string ANT1 = "0"; // 天线1
        public static string ANT2 = "1"; // 天线2
        public string[] GetDllInfo()
        {
            string dllname = "DLL 名称       ：HDT646";
            string dllfunction = "Dll功能说明 ：HDT646";
            string dllHistoryVersion = "历史Dll版本：0.0.0.2-1";
            string dllHistoryVersion3 = "                     ：MP 23.5.20.1";
            string dllVersion = "当前Dll版本：MP 23.5.20.1";
            string dllChangeInfo = "Dll改动信息：";
            string dllChangeInfo3 = "MP 23.5.20.1：第一版开发程序";
            string dllChangeInfo2 = "MP 23.5.20.1-1：使管理项目代码 003 和项目代码 004 之间的 FW 变得容易";
            string[] info = { dllname,
                dllfunction,
                dllHistoryVersion, dllHistoryVersion3,
                dllVersion,
                dllChangeInfo, dllChangeInfo3,dllChangeInfo2
       };
            return info;
        }
       private check check = new check();
        private Data data = new Data();
        private uint handle = 0;
        private uint Donglehandle = 0;
        private MessageBox messagebox = new MessageBox();
        private Command commend = new Command();
        SerialPort com1 = new SerialPort();
        SerialPort com3 = new SerialPort();
        readonly VolumeTest VolumeTestPlan = new VolumeTest();
       
        FW FW;
       
        //private SwATE mesVN = new SwATE();
        /// <summary>
        /// 平台程序共享的参数
        /// </summary>
        Dictionary<string, object> Config = new Dictionary<string, object>();
        /// <summary>
        /// 连扳程序特有的单线程平台共享参数
        /// </summary>
        Dictionary<string, object> OnceConfig = new Dictionary<string, object>();
        /// <summary>
        /// 用于判断是连扳还是单板程序
        /// </summary>
        bool MoreTestFlag;
        public object Interface(Dictionary<string, object> Config) => this.Config = Config;

        struct dev
        {
            public string path;
            public int len;
            public IntPtr handle;
        }
        dev devinfo = new dev();
        dev devinfo_D = new dev();
        dev devinfo_DF = new dev();
        private void init()
        {
            data.OpenHandel(data.RXPID, data.RXVID, data.TXPID, data.TXVID);
        }

        public string Run(string message)
        {         
            GetHandles.Path = "";
            GetHandles.gethandle("046d", "0abb", "col01");
            if (GetHandles.Path == "")
            {
                GetHandles.gethandle("046d", "0aba", "col01");
            }
            devinfo.handle = GetHandles.Handle;
            devinfo.path = GetHandles.Path;
            devinfo.len = 20;
            GetHandles.gethandle("046d", "0aba", "col02");
            devinfo_D.handle = GetHandles.Handle;
            devinfo_D.path = GetHandles.Path;
            devinfo_D.len = 20;
            GetHandles.gethandle("046d", "0aba", "col03");
            devinfo_DF.handle = GetHandles.Handle;
            devinfo_DF.path = GetHandles.Path;
            devinfo_DF.len = 64;
            this.init();
            Thread.Sleep(50);
            try
            {
                
                switch (message)
                {
                    /*  case "ReadPIDVID": return ReadPIDVID();
                      case "ReadDongleName": return ReadDongleName();
                      case "ReadHeadSetName": return ReadHeadSetName();
                      case "ReadNewFWName": return ReadNewFWName();*/
                    case "HeadsetLEDON": return HeadsetLEDON(devinfo).ToString();
                    case "HeadsetLEDOff": return HeadsetLEDOff(devinfo).ToString();                 
                    case "ReadDongleFW": return ReadDongleFW();
                    case "ReadHeadSetFW": return ReadHeadSetFW();
                    case "DongleAndHeadsetPair": return DongleAndHeadsetPair();
                    case "GetdongleHidDevic": return GetdongleHidDevic();
                    case "GetHeadsetHidDevic": return GetHeadsetHidDevic();
                    case "DongleID": return DongleID();
                    case "GetVoltage": return GetVoltage().ToString();
                    case "DeciveHeadsetName": return DeciveHeadsetName();
                    case "VolUp": return VolUp().ToString();
                    case "VolDown": return VolDown().ToString();
                    case "PairingTest2": return PairingTest2(devinfo).ToString();
                    case "GetTXVidPid":return GetTXVidPid();
                    /* 耳机天线选择 ant1 ant2 */
                    /* 耳机通道选择 ch1 ch20 ch38 */
                    case "headsetAnt1Ch1": return SetRFChannel(data.RXVID, data.RXPID, ANT1, CH1).ToString();
                    case "headsetAnt1Ch20": return SetRFChannel(data.RXVID, data.RXPID, ANT1, CH20).ToString();
                    case "headsetAnt1Ch38": return SetRFChannel(data.RXVID, data.RXPID, ANT1, CH38).ToString();
                    case "headsetAnt2Ch1": return SetRFChannel(data.RXVID, data.RXPID, ANT2, CH1).ToString();
                    case "headsetAnt2Ch20": return SetRFChannel(data.RXVID, data.RXPID, ANT2, CH20).ToString();
                    case "headsetAnt2Ch38": return SetRFChannel(data.RXVID, data.RXPID, ANT2, CH38).ToString();
                    /* dongle天线选择 ant1 ant2 */
                    /* dongle通道选择 ch1 ch20 ch38 */
                    case "dongleAnt1Ch1": return SetRFChannel(data.TXVID, data.TXPID, ANT1, CH1).ToString();
                    case "dongleAnt1Ch20": return SetRFChannel(data.TXVID, data.TXPID, ANT1, CH20).ToString();
                    case "dongleAnt1Ch38": return SetRFChannel(data.TXVID, data.TXPID, ANT1, CH38).ToString();
                    case "dongleAnt2Ch1": return SetRFChannel(data.TXVID, data.TXPID, ANT2, CH1).ToString();
                    case "dongleAnt2Ch20": return SetRFChannel(data.TXVID, data.TXPID, ANT2, CH20).ToString();
                    case "dongleAnt2Ch38": return SetRFChannel(data.TXVID, data.TXPID, ANT2, CH38).ToString();
                    // case "GetDllFirmwareVersion": return "MP 22.3.9.1";
                    default: return " False";
                }
            }
            catch (Exception e)
            {
                messagebox.JudgeBox("电脑未读取到装置/Máy tính không tìm thấy sản phẩm");
                return "False";
            }
        }
        private string GetTXVidPid()
        {
            bool check = GetHandles.gethandle(data.TXPID, data.TXVID, "col01");
            if (!check)
            {
                return false.ToString();
            }
            else
            {
                return ("VID:" + data.TXVID+ "|PID:" + data.TXPID );
            }                             
        }
        // 天线选择
        public static bool SetRFChannel(string vid, string pid, string ch, string channel)
        {
            return SCPI.avnera.Main(new string[] { vid, pid, ch, channel }) == 1;
        }
        private object HeadsetLEDOff(dev dev)
        {
            string cmd = "06 88 0A 00";
            return commend.WriteSend(cmd, 20, dev.handle);
        }
        private object HeadsetLEDON(dev dev)
        {
            string cmd = "06 88 0A 01";
            return commend.WriteSend(cmd, 20, dev.handle);
        }
        OldButtonTest o = new OldButtonTest();

        private bool PairingTest2(dev dev)
        {
            string cmd = "06 88 0c 01";// 06 88 0c 01 //06 88 0C AA
            bool a = commend.WriteSend(cmd, 20, dev.handle);
            return o.Buttontest("AA 01", "3 4", "检查配对按键/Kiểm tra nút nhấn ghép đôi ", devinfo.path, 1000);
            //o.Buttontest("06 88 0C AA 01", "0 1 2 3 4","pairing test",devinfo.path,20);
        }
        private bool VolDown()
        {
            return VolumeTestPlan.volumetest(false, "下调音量/ Vui lòng vặn giảm âm lượng");
        }
        private bool VolUp()
        {
            return VolumeTestPlan.volumetest(true, "上调音量/Vui lòng vặn tăng âm lượng");
        }
        private double GetVoltage()
        {
            string cmd = "11 FF 06";
            string returndata = "11 FF 06";
            string dataindex = "4 5 6";
            if (commend.WriteReturn(cmd, 20, returndata, dataindex, devinfo_D.path, devinfo_D.handle))
            {
                string[] value = commend.ReturnValue.Split(new char[3] {' ',' ',' '});
                int str1 = Convert.ToInt32(  value[0],16);
                int str2 = Convert.ToInt32(value[1], 16);
                int str3 = Convert.ToInt32(value[2], 16);
                double num2 = str1 * 256 + str2;
                double num = num2 / 1000.0;
                return  num;
            }
            return 0;
        }
        bool ErrorFlag = true;
        string DongleID1 = "";
        private string DongleAndHeadsetPair()
        {
            string cmd = "FF 0A 00 FD 04 00 00 05 81 D9 F3 04 00";
            string cmd1 = "FF 05 00 39";
            string dataindex = "11 12 13 14";
            string bbq;
            commend.ReturnValue = "";
            if (commend.SetFeatureSend(cmd, 64, devinfo_DF.handle))
            {
                Thread.Sleep(500);
                if (commend.GetFeatureReturn(cmd, 64, devinfo_DF.handle, dataindex))
                {
                    bbq = commend.ReturnValue;
                    string[] str = bbq.Split(new char[4] { ' ', ' ', ' ', ' ' });
                    string str1 = str[0];
                    string str2 = str[1];
                    string str3 = str[2];
                    string str4 = str[3];
                    string cmd2 = "06 " + "88 " + "06 " + str1 + " " + str2 + " " + str3 + " " + str4;
                    DongleID1 = String.Format(bbq);
                    if (!commend.SetFeatureSend(cmd1, 64, devinfo_DF.handle)) ErrorFlag = false;
                    Thread.Sleep(0);
                    if (commend.WriteSend(cmd2, 20, devinfo.handle))
                    {
                        string datalength = commend.ReturnValue;
                        if (datalength == DongleID1)
                        {
                            return true.ToString();
                        }
                        return "False "+datalength;
                    }
                }
            }
            return false.ToString();
        }

        private string DongleID()
        {
            string cmd = "FF 0A 00 FD 04 00 00 05 81 D9 F3 04";;
            string dataindex = "11 12 13 14";
            string ads = "";
            string str1 = "";
            if (commend.SetFeatureSend(cmd, 64, devinfo_DF.handle))
            {
                if (commend.GetFeatureReturn(cmd, 64, devinfo_DF.handle, dataindex))
                {
                    ads = commend.ReturnValue;
                    if (ads != null)
                    {                       
                        string datalength = ads;
                        string[] type1 = datalength.Split(new char[4] { ' ', ' ', ' ', ' ' });//分解字符串
                        string type2 = type1[0];
                        string type3 = type1[1];
                        string type4 = type1[2];
                        string type5 = type1[3];
                        str1 = type2 + " " + type3 + " " + type4 + " " + type5;
                        if(str1=="78 56 34 12")
                        {
                            messagebox.JudgeBox("Dongle报错NG/Dongle sai lầm NG");
                            return "False";
                        }
                    }
                    return str1;
                }
            }
            return "False";
        }
        private string GetHeadsetHidDevic()
        {
            bool check = GetHandles.gethandle(data.RXPID, data.RXVID, "col01");           
            if (!check)
            {
                return false.ToString();
            }
            else
            {
                return data.RXPID + data.RXVID;
            }
        }
        private string GetdongleHidDevic()
        {
           bool check= GetHandles.gethandle(data.TXPID, data.TXVID, "col01");       
            if(!check)
            {
                return false.ToString();
            }
            else
            {
                return data.TXPID + data.TXVID;
            }
        }
    
        private string ReadDongleFW()
        {
            string cmd = "11 FF 02 1F";
            string returndata = "11 FF 02 1F 00 55 31 20 12";
            string dataindex = "10 11";
            string str111 = "";
            // data.CloseHandel();
            Thread.Sleep(0);
            Thread.Sleep(10);
            //if (commend.WriteSend(cmd,20,data.donglehandel2))
            // if (commend.WriteReturn(cmd, 20, returndata, dataindex, data.donglepath2, data.donglehandel2))
            if (commend.WriteReturn(cmd, 20, returndata, dataindex, devinfo_D.path, devinfo_D.handle))
            {
                string datalength = commend.ReturnValue;
                string[] type1 = datalength.Split(new char[2] { ' ', ' ' });//分解字符串
                var type2 = Convert.ToInt32(type1[0]);
                var type3 = Convert.ToInt32(type1[1]);
                str111 = "V" + type2.ToString() + "." + type3.ToString();
                if (str111 == FW.i)
                {
                    return str111;
                }              
            }
            return "False: "+ str111;
        }
        private string ReadHeadSetFW()
        {
           
            string cmd = "06 88 04 00";
            string returndata = "06 88 04";
            string dataindex = "03 04";
            string str11 = "";
            if (commend.WriteReturn(cmd, devinfo.len, returndata, dataindex, devinfo.path, devinfo.handle))
            {
                string datalength = commend.ReturnValue;
                string[] type1 = datalength.Split(new char[2] { ' ', ' ' });//分解字符串
                var type2 = Convert.ToInt32(type1[0]);
                var type3 = Convert.ToInt32(type1[1]);
                str11 = "V" + type2.ToString() + "." + type3.ToString();
               
                if (str11 == FW.i)
                {
                    return str11;
                }
            }
            return "False: "+ str11;
        }
        private string DeciveHeadsetName()
        {
            IList<HIDDevice> devList = HIDManagement.GetDevices(0x046d, 0x0abb, true);
            var HidDevice = devList[0];
            return HidDevice.ProductName;
        }
        private string ReadNewFWName()
        {
            string cmd = "04 20 02 00 30 00";
            string returndata = "05 60 20";
            string dataindex = "01 02";
            string str1 = "";
            if (commend.WriteReturn(cmd, devinfo.len, returndata, dataindex, devinfo.path, devinfo.handle))
            {
                string datalength = commend.ReturnValue;
                string[] type1 = datalength.Split(new char[2] { ' ', ' ' });//分解字符串
                string type2 = type1[0];
                string type3 = type1[1];
                str1 = "21-0B" + type2 + "_Rev_" + type3 + type2 + "_NVM - ANDR_dp_00.02.05.00.ptc";
            }
            return str1;
        }

        private string ReadHeadSetName()
        {

            IList<BlueNinjaSoftware.HIDLib.HIDDevice> devList = HIDManagement.GetDevices(0x046d, 0x0a9c, true);
            BlueNinjaSoftware.HIDLib.HIDDevice HidDevice = devList[0];
            return HidDevice.ProductName;
        }

        private string ReadDongleName()
        {
            IList<BlueNinjaSoftware.HIDLib.HIDDevice> devList = HIDManagement.GetDevices(0x046d, 0x0a9c, true);
            BlueNinjaSoftware.HIDLib.HIDDevice HidDevice = devList[0];
            return HidDevice.Manufacturer;
        }

        private string ReadPIDVID()
        {
            GetHandles.gethandle(data.TXPID, data.TXVID, "col01");
            if (GetHandles.Path == "")
            {
                return "false";
            }
            else
            {
                return data.TXPID + data.TXVID;
            }

        }
        bool flag = false;
        public bool Start(List<string> formsData, IntPtr _handel)
        {
            /*
                程序启动是触发方法
                写下你的代码 
                Console.WriteLine("Hello Word");
             */
            //料号
            string OrderNumberInformation = (string)Config["OrderNumberInformation"];
            //工单
            string OrderNumber = (string)Config["Works"];
            //根据料号索引的后台的参数
           if (check.Check == "1")
          {
                FW = new FW();
                FW.ShowDialog();
                Thread t = new Thread(ShowInfo);
                t.Start();
                Dictionary<string, string> PartNumberInfos = (Dictionary<string, string>)Config["PartNumberInfos"];
                flag = true;

           }

            return true;

        }
        private void ShowInfo()
        {
            while (true)
            {
                if (flag) 
                {
                    flag = false;
                    View_FW View_FW = new View_FW();
                    View_FW.i = FW.i;
                    View_FW.ShowDialog();
                }
                Thread.Sleep(1000);
            }
        }
        public bool StartRun()
        {
            /*
               单板开始测试是触发方法
               写下你的代码 
            */
            MoreTestFlag = false;
          
            return true;
        }

        public bool StartTest(Dictionary<string, object> OnceConfig)
        {
            /*
               连扳程序当开始测试后触发方法  OnceConfig是线程独立参数
               写下你的代码 
               Console.WriteLine("Hello Word");
            */
            this.OnceConfig = OnceConfig;
            MoreTestFlag = true;
           
            return true;
        }
        public void TestsEnd(object obj)
        {
            /*
                连扳程序当所有线程测试结束后触发方法
                写下你的代码 
                Console.WriteLine("Hello Word");
           */
        }
    }
}
