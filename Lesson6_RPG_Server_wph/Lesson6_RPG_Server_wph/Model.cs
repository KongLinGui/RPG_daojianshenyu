using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson6_RPG_Server_wph
{
    class Model
    {
    }

    public class RolePower//角色战力
    {
        public string Name;
        public int Power;
    }

    public class AccountInfo//账户信息
    {
        public string Username;
        public string Password;

        public AccountInfo(string user,string pass)
        {
            Username = user;
            Password = pass;
        }
    }

    /// <summary>
    /// 玩家信息
    /// </summary>
    public class ServerPlayerData
    {
        /// <summary>
        /// 账户信息
        /// </summary>
        public AccountInfo Account;
        /// <summary>
        /// 玩家数据
        /// </summary>
        public string PlayerDataJson;
    }
}
