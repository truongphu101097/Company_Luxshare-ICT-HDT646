using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SwATE_Net;
using MESDLL;
using MerryTest.Entity;

namespace MX001
{
    public partial class FW : Form
    {
        private UIAdaptiveSize uias;
        public FW()
        {
            InitializeComponent();
        }
        public string i;
        private void FW_Load(object sender, EventArgs e)
        {

            uias = new UIAdaptiveSize
            {
                Width = Width,
                Height = Height,
                FormsName = this.Text,
                X = Width,
            };
            uias.SetInitSize(this);
            flag1 = true;
        }
        bool flag1 = false;
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (!flag1) return;
            var newx = Width;
            uias.UpdateSize(Width, Height, this);
            uias.X = newx;
        }   
     
        private void button1_Click(object sender, EventArgs e)
        {
           
            i = "V2.3";
            this.Close();
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            i = "V2.11";
            this.Close();
          

        }

        private void FW_FormClosed(object sender, FormClosedEventArgs e)
        {/*
            View_FW View_FW = new View_FW();
            View_FW.i = this.i;
            View_FW.ShowDialog();*/
        }
    }
}
