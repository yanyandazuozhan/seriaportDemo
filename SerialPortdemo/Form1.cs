using SerialPortMethod;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 串口demo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PublicMethod.ReceiveMessageEvent += PublicMethod_ReceiveMessageEvent;
            PublicMethod.ReceiveICEvent += PublicMethod_ReceiveICEvent;
            PublicMethod.SerialPortWriteEvent += PublicMethod_SerialPortWriteEvent;
        }

        private void PublicMethod_ReceiveICEvent(string ICType, string ICNum)
        {
            textBox1.Text += string.Format("卡号(Ex)：{0}\r\n", ICNum);

            this.textBox1.Focus();//获取焦点
            this.textBox1.Select(this.textBox1.TextLength, 0);//光标定位到文本最后
            this.textBox1.ScrollToCaret();//滚动到光标处
        }

        private void PublicMethod_SerialPortWriteEvent(byte[] values, string CodeMessage)
        {

        }

        private void PublicMethod_ReceiveMessageEvent(string code, object message)
        {
            textBox1.Text += string.Format("———————错误信息：{0}----------------\r\n", code + ":" + message);

            this.textBox1.Focus();//获取焦点
            this.textBox1.Select(this.textBox1.TextLength, 0);//光标定位到文本最后
            this.textBox1.ScrollToCaret();//滚动到光标处
        }

        SerialPortToIC serialPortToIC = new SerialPortToIC();

        private void btnPort_Click(object sender, EventArgs e)
        {
            if (PublicMethod.SerialPortToICMethodV1_3("0", ref serialPortToIC))
                textBox1.Text += "———————打开端口----------------\r\n";
            else
                textBox1.Text += "———————打开失败----------------\r\n";
        }

        private void btnGetCardNo_Click(object sender, EventArgs e)
        {
            PublicMethod.ReadICNumberV1_3(serialPortToIC);
        }

        private void btn_monitor_card_Click(object sender, EventArgs e)
        {
            PublicMethod.MonitorICNumberOpenV1_3(serialPortToIC);
            textBox1.Text += "———————开启监控----------------\r\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PublicMethod.SerialPortCloseV1_3(serialPortToIC);
            textBox1.Text += "———————关闭端口----------------\r\n";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PublicMethod.MonitorICNumberCloseV1_3(serialPortToIC);
            textBox1.Text += "———————停止监控----------------\r\n";
        }

       
    }
}
