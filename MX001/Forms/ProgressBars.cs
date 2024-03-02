using MX001;
using NAudio.CoreAudioApi;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MerryTestFramework.testitem.Forms
{
    internal partial class ProgressBars : Form
    {
        FW FW;
        public ProgressBars(string name)
        {
            InitializeComponent();
            this.Text = name;
            label1.Text = name;
          
        }
        private void ProgressBars_Load(object sender, EventArgs e)
        {
            i = 0;
            timer1.Interval = 500;
            timer1.Enabled = true;
            
        }
        private void SetSysVolume(int volumeLevel)
        {
            try
            {
                var mMDeviceEnumerator = new MMDeviceEnumerator();
                var defaultAudioEndpoint = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                defaultAudioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar = volumeLevel / 100f;
            }
            catch (Exception)
            {

            }
        }
        private int GetSysVolume()
        {
            int result = 0;
            try
            {
                MMDeviceEnumerator mMDeviceEnumerator = new MMDeviceEnumerator();
                MMDevice defaultAudioEndpoint = mMDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
                result = (int)(defaultAudioEndpoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100f);
                //DisplaylistboxMSG("Current Volume-->" + result);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }
        public void VolumeUpTest()
        {
            timer1.Enabled = false;
            Thread.Sleep(500);
            SetSysVolume(1);
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            int i = 1;
            while (true)
            {
                double msec = sWatch.ElapsedMilliseconds / 1000.0;
                if (msec > 15.0)
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                }
                int volumeAfter = GetSysVolume() + 1;
                if (i > volumeAfter)
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                }
                i = volumeAfter;

                if (volumeAfter >= 20)
                {
                    DialogResult = DialogResult.Yes;
                    this.Close();
                    break;
                }
                progressBar1.Value = volumeAfter * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }
        }

        public void VolumeDownTest()
        {
            timer1.Enabled = false;
            SetSysVolume(99);
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            int i = 99;
            while (true)
            {
                double msec = sWatch.ElapsedMilliseconds / 1000.0;
                if (msec > 15.0)
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                }
                int volumeAfter = GetSysVolume();
                if (i < volumeAfter)
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                }
                i = volumeAfter;
                if ((100 - volumeAfter) >= 20)
                {
                    DialogResult = DialogResult.Yes;
                    this.Close();
                    break;
                }
                progressBar1.Value = (100 - volumeAfter) * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }
        }
        public void VolumeUpTestNO()
        {
            timer1.Enabled = false;
            Thread.Sleep(500);
            SetSysVolume(1);
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            int i = 1;
            while (true)
            {
                double msec = sWatch.ElapsedMilliseconds / 1000.0;
                if (msec > 15.0)
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                }
                int volumeAfter = GetSysVolume() + 1;

                i = volumeAfter;

                if (volumeAfter >= 20)
                {
                    DialogResult = DialogResult.Yes;
                    this.Close();
                    break;
                }
                progressBar1.Value = volumeAfter * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }
        }
        public void VolumeDownTestNO()
        {
            timer1.Enabled = false;
            SetSysVolume(99);
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            int i = 99;
            while (true)
            {
                double msec = sWatch.ElapsedMilliseconds / 1000.0;
                if (msec > 15.0)
                {
                    DialogResult = DialogResult.No;
                    this.Close();
                    break;
                }
                int volumeAfter = GetSysVolume();
                i = volumeAfter;
                if ((100 - volumeAfter) >= 20)
                {
                    DialogResult = DialogResult.Yes;
                    this.Close();
                    break;
                }
                progressBar1.Value = (100 - volumeAfter) * 5;
                Application.DoEvents();
                Thread.Sleep(20);
            }
        }
        int i = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            i = i + 5;
            progressBar1.Value = i;
            //如果執行時間超過，則this.DialogResult = DialogResult.No;
            if (i >= 100)
            {
                DialogResult = DialogResult.No;
                timer1.Enabled = false;

                this.Close();
            }
        }
    }
}
