using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using MerryTestFramework.testitem;
using MerryTestFramework.testitem.Computer;

namespace MerryTestFramework.testitem.Forms
{
    /// <summary>
    /// 弹窗
    /// </summary>
    internal partial class MessageBoxs : Form
    {
        /// <summary>
        /// 窗体名
        /// </summary>
        internal string names = "";
        /// <summary>
        /// 窗体信息
        /// </summary>
        internal string message = "";
        /// <summary>
        /// 窗体条码框长度
        /// </summary>
        internal int length = 0;
        /// <summary>
        /// 条码框输入值
        /// </summary>
        internal string barcode = "";
        /// <summary>
        /// 窗体颜色
        /// </summary>
        internal Color color = Color.Transparent;
        /// <summary>
        /// 不良SN
        /// </summary>
        internal string SN = "";
        internal bool uploadmes = false;
        /// <summary>
        /// 构造函数
        /// </summary>
        internal MessageBoxs()
        {
            InitializeComponent();
        }
        private void messageBox_Load(object sender, EventArgs e)
        {
            Text = names;
            Message_label.Text = message;
            TopLevel = true;
            TopMost = true;
            var mc = new MouseControl();
            mc.MouseMove(Location, 10, 145);
            mc.MouseLeftClick();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (length > 0) if (BarCode_textBox.Text.Length != length)
                {
                    System.Windows.Forms.MessageBox.Show("条码长度不正确");
                    return;
                }
            barcode = BarCode_textBox.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void text_SN_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (text_SN.Visible)
            {
                if (text_SN.Text.Length != 4)
                {
                    System.Windows.Forms.MessageBox.Show("不良SN码必须4位");
                    return;
                }

                SN = text_SN.Text;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void MessageBoxs_KeyDown(object sender, KeyEventArgs e)
        {
            if (uploadmes) return;
            if (e.KeyCode == Keys.N)
            {
                button2_Click(null, null);
                return;
            }
            if (e.KeyCode == Keys.Y)
            {
                button1_Click(null, null);
                return;
            }

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MessageBoxs
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "MessageBoxs";
            this.Load += new System.EventHandler(this.MessageBoxs_Load);
            this.ResumeLayout(false);

        }

        private void MessageBoxs_Load(object sender, EventArgs e)
        {

        }
    }
}

