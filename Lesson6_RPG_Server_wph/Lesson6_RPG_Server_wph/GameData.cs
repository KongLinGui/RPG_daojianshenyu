using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson6_RPG_Server_wph
{
    public static class GameData
    {
        //账号信息
        public static List<AccountInfo> AccountList = new List<AccountInfo>();
        //玩家信息 字典数据
        public static Dictionary<string, ServerPlayerData> PlayerDataDic = new Dictionary<string, ServerPlayerData>();

        //public static List<string> NameList = new List<string>();
        //角色战力
        public static List<RolePower> rolePowerList = new List<RolePower>();
    }
}
