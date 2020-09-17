using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lesson6_RPG_Server_wph
{
    /// <summary>
    /// 客户端对象
    /// </summary>
    class Client
    {
        /// <summary>
        /// 服务
        /// </summary>
        public Server server;
        //姓名
        public string Name;
        /// <summary>
        /// TcpClient 对象
        /// </summary>
        public TcpClient TcpClientObj;
        /// <summary>
        /// 服务线程
        /// </summary>
        public Thread ClientThread;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="tcp"></param>
        /// <param name="ser"></param>
        public Client(TcpClient tcp,Server ser)
        {
            this.server = ser;
            this.TcpClientObj = tcp;
            //开启线程
            this.ClientThread = new Thread(ClientService);
            ClientThread.Start();
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        public void Stop()
        {
            //1.关闭连接
            TcpClientObj.Close();
            //2.终止线程
            ClientThread.Interrupt();
            //3.从list中移除自身
            server.clientList.Remove(this);
            //4.广播 自己下线的通知
            //NetResponse resOut = new NetResponse();
            //resOut.Type = "Out";
            //resOut.Data = this.Name;
            //server.NotifyAllClient(resOut);
        }
        /// <summary>
        /// 向客户端发送数据
        /// </summary>
        /// <param name="response"></param>
        public void Send(NetResponse response)
        {
            try
            {
                NetworkStream stream = TcpClientObj.GetStream();
                byte[] sendData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(response)+"VR2End");
                stream.Write(sendData, 0, sendData.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("发送数据异常" + ex.Message);
                Console.WriteLine("发送数据异常" + ex.StackTrace);
            }
        }

        public string UserName;//账户名
        public string Password;//密码

        

        /// <summary>
        /// 客户端服务 (线程)
        /// </summary>
        void ClientService()
        {
            try
            {
                //3.收数据
                NetworkStream stream = TcpClientObj.GetStream();
                byte[] recBuffer = new byte[10240];
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    //3.1接收客户端数据
                    int recLength = stream.Read(recBuffer, 0, recBuffer.Length);
                    string rawMsg = Encoding.UTF8.GetString(recBuffer, 0, recLength);
                    //输出
                    Console.WriteLine("<收到客户端信息>" + 
                        TcpClientObj.Client.RemoteEndPoint + "" + rawMsg);

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
                                //解析成功
                                //输出
                                Console.WriteLine("<解析客户端数据> " + sb.ToString());
                                ParseNetRequest(JsonConvert.DeserializeObject<NetRequest>(sb.ToString()));
                                sb.Clear();
                                i += rnFixLength;
                            }
                        }
                        else
                        {
                            sb.Append(rawMsg[i]);
                            i++;
                        }
                    }

                    //ParseNetRequest(JsonConvert.DeserializeObject<NetRequest>(rawMsg));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("与客户端断开连接 Exception = " + ex.Message);
                Console.WriteLine("与客户端断开连接 Exception = " + ex.StackTrace);
                Stop();
            }
        }
       
        bool isOk = false;
        /// <summary>
        /// 解析从客户端发来的请求数据
        /// </summary>
        /// <param name="request"></param>
        void ParseNetRequest(NetRequest request)
        {
            if (request.Type=="Chat")//聊天
            {
                NetResponse response = new NetResponse();
                response.Type = request.Type;
                response.Code = 1;
                response.Data = request.Data;
                server.NotifyAllClient(response);//广播
            }
            else if (request.Type=="CreatChat")
            {
                NetResponse response = new NetResponse();
                response.Type = request.Type;
                response.Code = 1;
                // 存储
                this.Name = request.Data;
                // 发送数据
                Send(response);
                //广播
                NetResponse responseNotify = new NetResponse();
                //responseNotify.Type = "In";
                responseNotify.Data = this.Name;
                server.NotifyAllClient(responseNotify);

            }
            else if (request.Type == "PlayerData")
            {
                NetResponse res = new NetResponse();
                res.Type = request.Type;
                ServerPlayerData serverplayerdata = new ServerPlayerData();
                serverplayerdata.Account = new AccountInfo(UserName, Password);
                serverplayerdata.PlayerDataJson = request.Data;
                GameData.PlayerDataDic[UserName] = serverplayerdata;//###
                res.Code = 1;
                Send(res);
            }           
            else if (request.Type == "In")//进入
            {
                NetResponse response = new NetResponse();
                response.Type = request.Type;
                response.Data = request.Data;
                response.Code = 1;
                server.NotifyAllClient(response);
            }
            else if (request.Type == "Out")//退出
            {
                NetResponse response = new NetResponse();
                response.Type = request.Type;
                response.Data = request.Data;
                response.Code = 1;
                server.NotifyAllClient(response);
            }
            else if (request.Type == "UseHorn")//喇叭
            {
                Console.WriteLine("是不是没收到？");
                NetResponse response = new NetResponse();
                response.Type = request.Type;
                response.Data = request.Data;
                Send(response);
            }           
            else if (request.Type=="Register")//注册
            {
                NetResponse response = new NetResponse();//回应
                response.Type = request.Type;
                AccountInfo account = JsonConvert.DeserializeObject<AccountInfo>(request.Data);
                //密码长度
                if (account.Password.Length<3)
                {
                    response.Code = -1;
                }
                else
                {
                    if (GameData.AccountList.Count==0)
                    {
                        GameData.AccountList.Add(account);
                        response.Code = 1;
                    }
                    else
                    {
                        for (int i = 0; i < GameData.AccountList.Count; i++)
                        {
                            if (account.Username==GameData.AccountList[i].Username)
                            {
                                Console.WriteLine(account.Username);
                                Console.WriteLine(GameData.AccountList[i].Username);
                                response.Code = 0;
                                break;
                            }
                            else
                            {
                                response.Code = 1;
                                isOk = true;
                            }
                        }
                        if (isOk)
                        {
                            GameData.AccountList.Add(account);
                        }
                    }
                }
                Send(response);
            }          
            else if (request.Type=="Login")//登录
            {
                AccountInfo account = JsonConvert.DeserializeObject<AccountInfo>(request.Data);               
                //和服务器中存储的账号信息进行比对              
                for (int i = 0; i < GameData.AccountList.Count; i++)
                {
                    if (GameData.AccountList[i].Username==account.Username&&
                        GameData.AccountList[i].Password==account.Password)
                    {
                        isOk = true;
                        break;
                    }
                }
                NetResponse res = new NetResponse();
                res.Type = request.Type;
                //isOk = true;//测试代码
                //发送回应
                if (isOk)
                {
                    res.Code = 1;
                    // 通过账户名，找到对应的玩家数据，判断是否为Null;
                    if (GameData.PlayerDataDic.ContainsKey(account.Username))
                    {
                        res.Data = GameData.PlayerDataDic[account.Username].PlayerDataJson;
                    }
                    UserName = account.Username;
                    Password = account.Password;
                    //for (int i = 0; i < GameData.AccountList.Count; i++)
                    //{
                    //    // 判断密码的长度是否大于3
                    //    if (GameData.AccountList[i].Password.Length < 3)
                    //    {
                    //        res.Code = -1;
                    //    }

                    //    else
                    //    {
                    //        res.Code = 1;
                    //        // 通过账户名，找到对应的玩家数据，判断是否为Null;

                    //        if (GameData.PlayerDataDic.ContainsKey(account.Username))
                    //        {
                    //            res.Data = GameData.PlayerDataDic[account.Username].PlayerDataJson;
                    //        }
                    //        UserName = account.Username;
                    //    }

                    //}
                }
                else
                {
                    res.Code = 0;
                }
                Send(res);
            }
            else if (request.Type == "CreateRole")//创建角色
            {
                ////拿到名字是否重名
                //bool isOk = true;
                //NetResponse response = new NetResponse();
                //response.Type = request.Type;
                //for (int i = 0; i < GameData.NameList.Count; i++)
                //{
                //    if (GameData.NameList[i] == request.Data)
                //    {
                //        isOk = false;
                //        break;
                //    }
                //}

                //// 如果不重名
                //if (isOk)
                //{
                //    response.Code = 1;
                //    // 存储
                //    this.Name = request.Data;
                //    GameData.NameList.Add(Name);
                //    // 发送数据
                //    Send(response);
                //}
                //else// 如果重名
                //{
                //    response.Code = 0;
                //    //发送数据                   
                //    Send(response);
                //}

                NetResponse res = new NetResponse();
                res.Type = request.Type;
                string name = request.Data;
                for (int i = 0; i < GameData.AccountList.Count; i++)
                {
                    if (GameData.PlayerDataDic.ContainsKey(GameData.AccountList[i].Username))
                    {
                        ServerPlayerData player = GameData.PlayerDataDic[GameData.AccountList[i].Username];
                        JObject playerdataObj = JObject.Parse(player.PlayerDataJson);
                        if (playerdataObj["Name"].ToString() == request.Data)
                        {
                            res.Code = 0;
                            Send(res);
                            return;
                        }
                    }
                }
                res.Code = 1;
                Send(res);

            }
            else if (request.Type=="Rank")//排行榜
            {
                RolePower playerpower = JsonConvert.DeserializeObject<RolePower>(request.Data);
                Console.WriteLine(GameData.rolePowerList.Count);
                if (GameData.rolePowerList.Count==0)
                {
                    GameData.rolePowerList.Add(playerpower);
                }
                else
                {
                    bool isok = false;
                    for (int i = 0; i < GameData.rolePowerList.Count; i++)
                    {
                        if (playerpower.Name==GameData.rolePowerList[i].Name)
                        {
                            GameData.rolePowerList[i].Power = playerpower.Power;
                            isok = true;
                            break;
                        }
                    }
                    if (!isok)
                    {
                        GameData.rolePowerList.Add(playerpower);
                        isok = false;
                    }
                }
                NetResponse response = new NetResponse();
                response.Type = request.Type;               
                Console.WriteLine(GameData.rolePowerList.Count);
                //遍历战力链表
                List<RolePower> NowPowerList = new List<RolePower>();
                for (int i = 0; i <GameData.rolePowerList.Count; i++)
                {
                    Console.WriteLine("发送战力信息");
                    NowPowerList.Add(GameData.rolePowerList[i]);
                }
                response.Data = JsonConvert.SerializeObject(NowPowerList);
                Send(response);         
            }
        }
    }
}
