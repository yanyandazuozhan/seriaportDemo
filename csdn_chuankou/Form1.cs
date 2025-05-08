using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace csdn_chuankou
{
    public partial class Form1: Form
    {
        public Form1()
        {
            InitializeComponent();
       
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            text_Serial_port();
            initial();
        }
        /// <summary>
        /// 获取可用串口--目前没有虚拟串口
        /// </summary>
        private void text_Serial_port()
        {
            string[] ports = System.IO.Ports.SerialPort.GetPortNames(); //获得可用的串口
            for (int i = 0; i < ports.Length; i++)
            {
                comboBox1.Items.Add(ports[i]);
            }
            comboBox1.SelectedIndex = comboBox1.Items.Count > 0 ? 0 : -1;//如果里面有数据,显示第0个

        }
        /// <summary>
        /// 串口初始化
        /// </summary>
        private void initial()
        {
            comboBox2.Text = "9600";//波特率设置为9600
            comboBox3.Text = "1";//数据位设置为1
            comboBox4.Text = "8";//停止位设置为8
            comboBox5.Text = "无";//校验位设置为无

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "打开串口")
            {
                try
                {
                    serialPort1.PortName = comboBox1.Text;//获取要打开的串口
                    serialPort1.BaudRate = int.Parse(comboBox2.Text);//获得波特率
                    serialPort1.DataBits = int.Parse(comboBox4.Text);//获得数据位
                    //设置停止位
                    if (comboBox3.Text == "1")
                    {
                        serialPort1.StopBits = StopBits.One;
                    }
                    else if (comboBox3.Text == "1.5")
                    {
                        serialPort1.StopBits = StopBits.OnePointFive;
                    }
                    else if (comboBox3.Text == "2")
                    {
                        serialPort1.StopBits = StopBits.Two;
                    }
                    //设置校验位
                    if (comboBox5.Text == "无")
                    {
                        serialPort1.Parity = Parity.None;
                    }
                    else if (comboBox5.Text == "奇校验")
                    {
                        serialPort1.Parity = Parity.Odd;
                    }
                    else if (comboBox5.Text == "偶校验")
                    {
                        serialPort1.Parity = Parity.Even;
                    }
                    serialPort1.Open();//打开串口
                    button1.Text = "关闭串口";
                }
                catch (Exception err)
                {
                    MessageBox.Show("打开失败" + err.ToString(), "提示！");
                }
            }
            else
            {
                //关闭串口
                try
                {
                    serialPort1.Close();//关闭串口
                }
                catch (Exception) { }
                button1.Text = "打开串口"; //按钮显示打开
            }
        }

        //字符串转换16进制显示方法
        private string byteToHexstr(byte[] buff)
        {
            string str = "";
            try
            {
                if (buff != null)
                {

                    for (int i = 0; i < buff.Length; i++)
                    {
                        //char a = (char)buff[i];
                        //str += a.ToString();
                        str += buff[i].ToString("x2");
                        str += " ";//两个之间用空格
                    }
                    //str = new string(buff);
                    return str;
                }
            }
            catch (Exception)
            {
                return str;
            }
            return str;
        }

        //读取并显示接收的信息
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            int len = serialPort1.BytesToRead; //获取可以读取的字节数

            byte[] buff = new byte[len];
            serialPort1.Read(buff, 0, len);//把数据读取到数组中
            string reslut = Encoding.Default.GetString(buff);//将byte值根据为ASCII值转为string
            Invoke((new Action(() =>
            {
                if (checkBox1.Checked)//16进制显示选择按钮为真，转化为16进制显示
                {
                    textBox1.AppendText(" " + byteToHexstr(buff) + Environment.NewLine);//16进制显示并换行
                }
                else
                {
                    textBox1.AppendText(" " + reslut + Environment.NewLine);//字符串显示在界面上并换行
                }
            }
            )));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();//清除数据显示区
        }

        //字符串转16进制
        private byte[] strToHexbytes(string str)
        {
            str = str.Replace(" ", "");//清除空格
            byte[] buff;
            if ((str.Length % 2) != 0)
            {
                buff = new byte[(str.Length + 1) / 2];
                try
                {
                    for (int i = 0; i < buff.Length; i++)
                    {
                        buff[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
                    }
                    buff[buff.Length - 1] = Convert.ToByte(str.Substring(str.Length - 1, 1).PadLeft(2, '0'), 16);
                    return buff;
                }
                catch (Exception err)
                {
                    MessageBox.Show("含有f非16进制的字符", "提示");
                    return null;
                }
            }
            else
            {
                buff = new byte[str.Length / 2];
                try
                {
                    for (int i = 0; i < buff.Length; i++)
                    {
                        buff[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
                    }
                    //buff[buff.Length - 1] = Convert.ToByte(str.Substring(str.Length - 1, 1).PadLeft(2, '0'), 16);
                }
                catch (Exception err)
                {
                    {
                        MessageBox.Show("含有非16进制的字符", "提示");
                        return null;
                    }
                }
            }

            return buff;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //创建发送数据
            Task.Run(() => {
                send_();
            });
        }

        //发送数据
        string data_;
        private void send_()
        {
            data_ = textBox2.Text.ToString();
            try
            {
                if (data_.Length != 0)
                {
                    data_ += " ";
                    if (checkBox2.Checked) //16进制发送
                    {
                        serialPort1.Write(Encoding.Default.GetString(strToHexbytes(data_)));
                    }
                    else
                    {
                        serialPort1.Write(data_);
                        //byte[] byteArray = Encoding.Default.GetBytes(shuju);//Str 转为 Byte值
                        //serialPort1.Write(byteArray, 0, byteArray.Length, 0, null, null); //发送数据         
                    }
                }

            }
            catch (Exception)
            {

            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Clear();//清除发送数据区
        }
    
    }
}
