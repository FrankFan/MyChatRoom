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
            //客户端连接到服务器
            socketClient.Connect(endpoint);

            threadClient = new Thread(ReceiveMsg);
            threadClient.IsBackground = true;
            threadClient.Start();
        }

        //向服务器发送数据
        private void btnSend_Click(object sender, EventArgs e)
        {
            string strMsg = txtMsgSend.Text.Trim();
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);//将字符串转成方便网络传送的二进制数组
            socketClient.Send(arrMsg);
            ShowMsg(string.Format("我说：{0}", strMsg));

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
                int length = socketClient.Receive(arrMsgRev);
                //此时是将数组的所有元素（每个字节）都转成字符串，而真正接收到只有服务端发来的几个字符
                string strMsgReceive = Encoding.UTF8.GetString(arrMsgRev, 0, length);
                ShowMsg(strMsgReceive);
            }
        }


    }
}
