using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lesson6_RPG_Server_wph
{
    class Server
    {
        //程序入口
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
        }

        private TcpListener listener;

        /// <summary>
        /// 开始
        /// </summary>
        void Start()
        {
            try
            {
                //1.启动
                listener = new TcpListener(IPAddress.Any, 8886);
                listener.Start();
                Console.WriteLine("服务器开始监听~~~~~~~~");
                //2.开启监听服务
                Thread listenerService = new Thread(ListenerService);
                listenerService.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// 客户端List
        /// </summary>
        public List<Client> clientList = new List<Client>();

        /// <summary>
        /// 监听服务
        /// </summary>
        void ListenerService()
        {
            try
            {
                while (true)
                {
                    if (listener.Pending())//当接受到一个客户端请求时，确认连接
                    {
                        //1.拿到连接
                        TcpClient tcpClient = listener.AcceptTcpClient();
                        Console.WriteLine("<客户端连接成功>" + tcpClient.Client.RemoteEndPoint);
                        //2.创建一个Client对象
                        Client client = new Client(tcpClient, this);
                        //3.将对象 放入List
                        clientList.Add(client);
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("监听服务出现异常 ：" + ex.StackTrace);
                Console.WriteLine("监听服务出现异常 ：" + ex.Message);
            }
        }

        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="response"></param>
        public void NotifyAllClient(NetResponse response)
        {
            for (int i = 0; i <clientList.Count; i++)
            {
                clientList[i].Send(response);
            }
        }
    }
}

/// <summary>
/// 网络请求
/// </summary>
public class NetRequest
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public string Type;
    /// <summary>
    /// 数据
    /// </summary>
    public string Data;
}

/// <summary>
/// 网络回应
/// </summary>
public class NetResponse
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public string Type;
    /// <summary>
    /// 数据
    /// </summary>
    public string Data;
    /// <summary>
    /// 回应数
    /// </summary>
    public int Code;
}