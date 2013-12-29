﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net; //IP,IPAdress(port)
using System.Net.Sockets;
using System.Threading;

namespace MyChatRoomServer
{
    public partial class FChatServer : Form
    {
        public FChatServer()
        {
            InitializeComponent();
            //关闭对文本框跨线程操作的检查
            TextBox.CheckForIllegalCrossThreadCalls = false;
        }

        Thread threadWatch = null;//负责监听客户端请求的线程
        Socket socketWatch = null;//负责监听服务端的套接字


        //Socket socketConnection = null;//负责和客户端通信的套接字
        //保存了服务器端所有和客户端通信的套接字
        Dictionary<string, Socket> dict = new Dictionary<string, Socket>();

        //保存了服务器端所有负责调用通信套接字的Receive方法的线程
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();

        //开启服务
        private void btnBeginListen_Click(object sender, EventArgs e)
        {
            //创建负责监听的套接字，参数使用IP4寻址协议，使用流式连接，使用TCP协议传输数据
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //获得文本框中的IP地址对象
            IPAddress address = IPAddress.Parse(txtIP.Text.Trim());
            //创建包含IP和port的网络节点对象
            IPEndPoint endpoint = new IPEndPoint(address, int.Parse(txtPort.Text.Trim()));
            //将负责监听的套接字绑定到唯一的IP和端口上
            socketWatch.Bind(endpoint);
            //设置监听队列的长度
            socketWatch.Listen(10);

            //创建负责监听的线程，并传入监听方法
            threadWatch = new Thread(WatchConnection);
            threadWatch.IsBackground = true;//设置为后台线程
            threadWatch.Start();//开启线程

            ShowMsg("服务端启动监听成功~");

        }

        //发送消息到客户端
        private void btnSend_Click(object sender, EventArgs e)
        {
            string strMsg = txtMsgSend.Text.Trim();
            //将要发送的字符串转成UTF-8对应的字节数组
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);

            //获得列表中选中的远程IP的Key
            string strClientKey = lbOnline.Text;
            //通过key找到字典集合中对应的某个客户端通信的套接字，用Send方法发送数据给对方
            dict[strClientKey].Send(arrMsg);

            //socketConnection.Send(arrMsg);
            ShowMsg(string.Format("对 {0} 说： {1}", strClientKey, strMsg));
        }

        //群发消息给每个客户端
        private void btnSendToAll_Click(object sender, EventArgs e)
        {
            string strMsg = txtMsgSend.Text.Trim();
            //将要发送的消息转成utf8对应的字节数组
            byte[] arrMsg = Encoding.UTF8.GetBytes(strMsg);
            foreach (Socket s in dict.Values)
            {
                s.Send(arrMsg);
            }
            ShowMsg("群发完毕~ :)");
        }

        /// <summary>
        /// 监听客户端请求的方法
        /// </summary>
        void WatchConnection()
        {
            //持续不断的监听客户端的新的连接请求
            while (true)
            {
                //开始监听请求，返回一个新的负责连接的套接字，负责和该客户端通信
                //注意：Accept方法会阻断当前线程！
                Socket socketConnection = socketWatch.Accept();

                //当有新的socket连接到服务端时就将IP添加到在线列表中,作为客户端的唯一标识                
                lbOnline.Items.Add(socketConnection.RemoteEndPoint.ToString());

                //将每个新产生的套接字存起来，装到键值对Dict集合中，以客户端IP:端口作为key
                dict.Add(socketConnection.RemoteEndPoint.ToString(), socketConnection);

                //为每个服务端通信套接字创建一个单独的通信线程，负责调用通信套接字的Receive方法，监听客户端发来的数据
                //创建通信线程
                Thread threadCommunicate = new Thread(ReceiveMsg);
                threadCommunicate.IsBackground = true;
                threadCommunicate.Start(socketConnection);//

                dictThread.Add(socketConnection.RemoteEndPoint.ToString(), threadCommunicate);
                


                //ShowMsg("客户端连接成功！" + socketConnection.RemoteEndPoint.ToString());
                ShowMsg(string.Format("{0} 上线了. ", socketConnection.RemoteEndPoint.ToString()));
            }
        }

        /// <summary>
        /// 服务端监听客户端发来的数据
        /// </summary>
        void ReceiveMsg(object socketClientPara)
        {
            Socket socketClient = socketClientPara as Socket;
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

        void ShowMsg(string msg)
        {
            txtMsg.AppendText(msg + "\r\n");
        }

        

        
    }
}
