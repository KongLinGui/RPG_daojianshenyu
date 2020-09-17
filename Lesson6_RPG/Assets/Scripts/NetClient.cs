using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

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

    public NetRequest(string type, string data)
    {
        Type = type;
        Data = data;
    }
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
    ///  数据
    /// </summary>
    public string Data;
    /// <summary>
    /// 回应数
    /// </summary>
    public int Code;

}
/// <summary>
/// 回应消息委托
/// </summary>
/// <param name="res"></param>
public delegate void NetResponseHandler(NetResponse res);

/// <summary>
///  网络控制脚本
/// </summary>
public class NetClient
{
    //单例
    private static NetClient instance;
    public static NetClient Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetClient();
            }
            return instance;
        }
    }



    /// <summary>
    /// 服务器的ip地址
    /// </summary>
    private string ip;
    /// <summary>
    /// 服务器的端口号
    /// </summary>
    private int port;
    /// <summary>
    /// 服务线程
    /// </summary>
    private Thread serviceThread;
    /// <summary>
    /// 是否连接到服务器
    /// </summary>
    public bool connected = false;

    /// <summary>
    /// 连接服务器
    /// </summary>
    /// <param name="ip">ip</param>
    /// <param name="port">端口</param>
    public void Start(string ip, int port)
    {
        this.ip = ip;
        this.port = port;
        //开启线程
        serviceThread = new Thread(ServiceThread);
        serviceThread.Start();
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public void Stop()
    {
        if (connected)
        {
            connected = false;
            tcpClient.Close();
            serviceThread.Interrupt();
        }
    }

    private TcpClient tcpClient;

    /// <summary>
    /// 异步登录的回调函数
    /// </summary>
    /// <param name="ar">方法</param>
    private void ConnectCallback(IAsyncResult ar)
    {
        TcpClient tempTcpClient = (TcpClient)ar.AsyncState;
        if (tempTcpClient.Connected)
        {
            //函数运行到这里就说明连接成功
            tempTcpClient.EndConnect(ar);
            connected = true;
        }
    }

    /// <summary>
    /// 请求队列
    /// </summary>
    Queue<NetRequest> requestQueue = new Queue<NetRequest>();
    /// <summary>
    /// 回应队列
    /// </summary>
    Queue<NetResponse> responseQueue = new Queue<NetResponse>();

  

    /// <summary>
    /// 服务线程
    /// </summary>
    private void ServiceThread()
    {
        try
        {
            //1.连接
            tcpClient = new TcpClient();
            //异步连接
            tcpClient.BeginConnect(ip, port, ConnectCallback, tcpClient);
            //计时
            float connectTime = 0;
            while (!connected)
            {
                Thread.Sleep(100);
                connectTime += 0.1f;
                if (connectTime > 3f)
                {
                    throw new Exception("与服务器连接超时。。。");
                }
            }
            //连接成功
            Debug.Log("成功连接服务器！！！ ");

            //2.1 获得网络数据流对象
            NetworkStream stream = tcpClient.GetStream();

            //2.2 开启一个发数据的线程
            Thread sendThread = null;
            sendThread = new Thread(SendThread);
            sendThread.Start(stream);

            //3.收数据
            byte[] buffer = new byte[10240];
            StringBuilder sb = new StringBuilder();
            while (connected)
            {
                //收
                int recLength = stream.Read(buffer, 0, buffer.Length);
                string rawMsg = Encoding.UTF8.GetString(buffer, 0, recLength);

                Debug.Log("收到服务器数据: " + rawMsg);

                int rnFixLength = "VR2End".Length;
                for (int i = 0; i < rawMsg.Length;)//遍历接收到的整个buffer文本
                {
                    if (i <= rawMsg.Length - rnFixLength)
                    {
                        if (rawMsg.Substring(i, rnFixLength) != "VR2End")//非消息结束符，则加入sb
                        {
                            sb.Append(rawMsg[i]);
                            i++;
                        }
                        else
                        {
                            //输出
                            Console.WriteLine("<解析客户端数据> " + sb.ToString());
                            //将数据放入回应队列
                            responseQueue.Enqueue(JsonUtility.FromJson<NetResponse>(sb.ToString()));
                            sb = new StringBuilder();
                            i += rnFixLength;
                        }
                    }
                    else
                    {
                        sb.Append(rawMsg[i]);
                        i++;
                    }

                }

                //if (recLength > 0)
                //{
                //    string recStr = Encoding.UTF8.GetString(buffer, 0, recLength);

                //    Debug.Log("收到服务器数据: " + recStr);
                //    //将数据放入 回应队列
                //    responseQueue.Enqueue(JsonUtility.FromJson<NetResponse>(recStr));

                //}
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log(ex.StackTrace);
        }
        finally
        {
            Debug.Log("与服务器断开连接！");
        }
    }
    //回应消息委托
    public NetResponseHandler netResponseHandler;

    /// <summary>
    /// 处理回应数据的协程
    /// </summary>
    /// <returns></returns>
    public IEnumerator NetResponseMov()
    {

        while (true)
        {
            //Debug.Log(responseQueue.Count + " responseQueue");
            //从回应队列中拿数据
            if (responseQueue.Count > 0)
            {
                NetResponse response = responseQueue.Dequeue();
                //Debug.Log("333333333333");
                //将数据传入 委托方法 ，执行委托方法
                if (response != null && netResponseHandler != null)
                {
                    netResponseHandler(response);
                }
            }
            yield return null;
        }
    }


    /// <summary>
    /// 发数据
    /// </summary>
    /// <param name="message">请求</param>
    public void SendRequest(NetRequest request)
    {
        //将请求放入队列
        requestQueue.Enqueue(request);
    }

    /// <summary>
    /// 发送数据线程
    /// </summary>
    /// <param name="obj"></param>
    void SendThread(object obj)
    {
        try
        {
            NetworkStream stream = (NetworkStream)obj;
            while (connected)
            {
                //从请求队列里取数据
                if (requestQueue.Count > 0)
                {
                    string message = JsonUtility.ToJson(requestQueue.Dequeue());
                    Debug.Log("<向服务器发送数据> " + message+ "VR2End");
                    byte[] sendData = Encoding.UTF8.GetBytes(message+ "VR2End");
                    stream.Write(sendData, 0, sendData.Length);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("发送数据线程 Exception = " + ex.Message);
            Debug.Log("发送数据线程 Exception = " + ex.StackTrace);
        }
    }
}
