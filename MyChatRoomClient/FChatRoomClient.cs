using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace MyChatRoomClient
{
    public partial class FChatRoomClient : Form
    {
        //客户端负责接收服务端发来的数据消息的线程
        Thread threadClient = null;
        //创建客户端套接字，负责连接服务器
        Socket socketClient = null;

        public FChatRoomClient()
        {
            InitializeComponent();
            //关闭对文本框跨线程操作的检查
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        //连接服务器
        private void btnConnect_Click(object sender, EventArgs e)
        {
            //获得文本框中的IP地址对象
            IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
            //创建包含IP和端口的网络节点对象
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
            //创建客户端套接字，负责连接服务器
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //客户端连接到服务器
                socketClient.Connect(endpoint);
            }
            catch (SocketException ex)
            {
                ShowMsg("客户端连接服务器发生异常：" + ex.Message);
            }
            catch (Exception ex)
            {
                ShowMsg("客户端连接服务器发生异常：" + ex.Message);
            }

            threadClient = new Thread(ReceiveMsg);
            threadClient.IsBackground = true;
            threadClient.Start();
        }

        //向服务器发送文本消息
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            string strMsg = txtMsgSend.Text.Trim();
            //将字符串转成方便网络传送的二进制数组
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            byte[] arrMsgSend = new byte[arrMsg.Length + 1];
            arrMsgSend[0] = 0;//设置标识位，0代表发送的是文字
            Buffer.BlockCopy(arrMsg, 0, arrMsgSend, 1, arrMsg.Length);
            try
            {
                socketClient.Send(arrMsgSend);

                ShowMsg(string.Format("我对 {0} 说：{1}", socketClient.RemoteEndPoint.ToString(), strMsg));
                //清空发送消息文本框中的消息
                this.txtMsgSend.Text = "";
            }
            catch (SocketException ex)
            {
                ShowMsg("客户端发送消息时发生异常：" + ex.Message);
            }
            catch (Exception ex)
            {
                ShowMsg("客户端发送消息时发生异常：" + ex.Message);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            //选择要发送的文件
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtFilePath.Text = ofd.FileName;
            }
        }

        //客户端向服务器发送文件
        private void btnSendFile_Click(object sender, EventArgs e)
        {
            //用文件流打开用户选择的文件
            using (FileStream fs = new FileStream(txtFilePath.Text, FileMode.Open))
            {
                //定义一个4M的数组（缓冲区）
                byte[] arrFile = new byte[1024 * 1024 * 2];
                //将文件数据读到数组arrFile中，并获取文件的真实长度
                int length = fs.Read(arrFile, 0, arrFile.Length);
                //用于发送真实数据的数组，多了一位标识位
                byte[] arrFileSend = new byte[length + 1];
                //第一位是协议位，1：文件 0：文字
                arrFileSend[0] = 1;
                //for (int i = 0; i < length; i++) //将数据拷贝到真实数组中
                //{
                //    arrFileSend[i + 1] = arrFile[i];
                //}
                //2.直接拷贝,不能指定其实元素位置offset
                //arrFile.CopyTo(arrFileSend, length);
                //3.
                Buffer.BlockCopy(arrFile, 0, arrFileSend, 1, length);
                //发送包含了标识位的新数据数组到服务端
                socketClient.Send(arrFileSend);
            }
        }

        void ShowMsg(string msg)
        {
            txtMsg.AppendText(msg + "\r\n");
        }

        /// <summary>
        /// 监听服务端发来的消息
        /// </summary>
        void ReceiveMsg()
        {
            while (true)
            {
                //定义一个接收消息用的字节数组缓冲区（2M大小）
                byte[] arrMsgRev = new byte[1024 * 1024 * 2];
                //将接收到的数据存入arrMsgRev,并返回真正接收到数据的长度
                int length = -1;
                try
                {
                    length = socketClient.Receive(arrMsgRev);
                }
                catch (SocketException ex)
                {
                    ShowMsg("客户端接收消息时发生异常：" + ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    ShowMsg("客户端接收消息时发生异常：" + ex.Message);
                    break;
                }

                //此时是将数组的所有元素（每个字节）都转成字符串，而真正接收到只有服务端发来的几个字符
                string strMsgReceive = Encoding.UTF8.GetString(arrMsgRev, 0, length);
                ShowMsg(string.Format("{0} 对我说：{1}", socketClient.RemoteEndPoint.ToString(), strMsgReceive));
            }
        }






    }
}
