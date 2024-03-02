using NAudio.CoreAudioApi;
using System;
using System.Management;

namespace MerryTestFramework.testitem.Computer
{
    /// <summary>
    /// 调节音量类
    /// </summary>
    public class AdjustVolume
    {

        /// <summary>
        /// 设置音量
        /// </summary>
        /// <param name="value">音量值</param>
        /// <param name="Name">装置名称MIC或SPK</param>
        public void SetVolume(int value, string Name)
        {

            try
            {
                value = value < 0 ? 0 : value;
                value = value > 100 ? 100 : value;
                var OSName = GetOSFriendlyName();
                if (OSName.Contains("Windows XP"))
                {
                    double newVolume = ushort.MaxValue * value / 100.0;
                    uint v = ((uint)newVolume) & 0xffff;
                    ushort vAll = (ushort)(v | (v << 16));
                    PC_VolumeControl.VolumeControl.SetVolume(vAll);
                }
                else
                {
                    var DevEnum = new MMDeviceEnumerator();
                    
                    var device =
                        Name == "MIC"
                        ? DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Communications)
                        : DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);


                    device.AudioEndpointVolume.MasterVolumeLevelScalar = value / 100.0f;
                }
            }
            catch (Exception ex)
            {
                new MessageBox().JudgeBox($"调节音量出现异常，异常信息为：{ex}");
            }


        }
        /// <summary>
        /// 获取当前音量
        /// </summary>
        /// <returns>系统当前音量</returns>
        public int GetVolume()
        {
            var result = 0;
            try
            {
                var OSName = GetOSFriendlyName();
                if (OSName.Contains("Windows XP"))
                {
                    var v = PC_VolumeControl.VolumeControl.GetVolume();
                    var vleft = (uint)v & 0xffff;
                    var vright = ((uint)v & 0xffff0000) >> 16;
                    result = ((int)vleft | (int)vright) * 100 / 0xffff;
                }
                else
                {
                    var DevEnum = new MMDeviceEnumerator();
                    var Volumes = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).AudioEndpointVolume.MasterVolumeLevelScalar;
                    result = (int)(Volumes * 100);
                }
            }
            catch
            {
                return -1;
            }
            return result;
        }


        /// <summary>
        /// 获取当前电脑操作系统名
        /// </summary>
        /// <returns></returns>
        private string GetOSFriendlyName()
        {
            string result = string.Empty;
            ManagementClass mc = new ManagementClass("Win32_OperatingSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                result = mo["Caption"].ToString();
            }
            return result;
        }
    }

}
