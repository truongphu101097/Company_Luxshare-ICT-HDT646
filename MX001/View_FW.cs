using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MX001
{
    public partial class View_FW : Form
    {
        public string i;
        public View_FW()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(0, 0);
        }
       
        private void View_FW_Load(object sender, EventArgs e)
        {
            View();
            
        }

        private void View()
        {
            if (i == "V2.3")
            {
                label1.Text = "机型：HDT646-003\nFW:V2.3";
               

            }
            if (i == "V2.11")
            {
                label1.Text = "机型：HDT646-004\nFW: V2.11";
               
            }
          
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
