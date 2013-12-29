using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;

namespace WinFormApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSingleThread_Click(object sender, EventArgs e)
        {
            CountNum();
            


        }

        void CountNum()
        {
            DateTime beginTime = DateTime.Now;

            for (int i = 0; i < 999999999; i++)
            {
                //故意留空
            }

            TimeSpan ts = DateTime.Now.Subtract(beginTime);
            MessageBox.Show("循环执行完毕，用时:" + ts.TotalMilliseconds);
        }

        private void btnMultiThread_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(CountNum);
            thread.IsBackground = true; //标记该线程为后台线程（前台线程结束后停止运行）
            thread.Start();
        }
    }
}
