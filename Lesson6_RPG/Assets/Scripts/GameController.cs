using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
public class GameController : MonoBehaviour {

    public string UserName;
    public string PassWord;

    public static GameController Instance;

    public UIController uiController;
    public GameData gameData;

    private void Awake()
    {
        Instance = this;
        uiController = FindObjectOfType<UIController>();
        gameData = FindObjectOfType<GameData>();


        //确定加载场景的时候，脚本或物体不被删除
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(uiController);
        DontDestroyOnLoad(FindObjectOfType<EventSystem>());

    }

    public Vector3 InputJoystick;//移动位移向量
    public Player player;//玩家引用
    public PlayerData playerData;//玩家数据
    public int currentLevel; //当前进入副本

    void Start ()
    {
        //1. 初始化
        gameData.DataInit();

        uiController.Init();

       
    }

    //创建新人物
    public void CreateNewPlayer(string name)
    {
        playerData = new PlayerData(name);
        //将任务添加到人物的任务列表中
        for (int i = 0; i < gameData.questDataList.Count; i++)
        {
            if (gameData.questDataList[i].PreId == 0)//初始任务
            {
                //将任务数据克隆之后 放入玩家list中
                playerData.QuestList.Add(gameData.questDataList[i].Clone());
            }
        }

        //将技能数据，添加到玩家的技能数据里
        for (int i = 0; i < gameData.skillDataList.Count; i++)
        {
           //将任务数据克隆之后 放入玩家list中
           playerData.SkillList.Add(gameData.skillDataList[i].Clone());
            
        }


    }
    //初始化 关卡 数据 参数： 当前第几关
    public void InitLevel(int levelIndex)
    {
        //0.存储下 当前进入关卡
        currentLevel = levelIndex;
        //1.删除其他UI界面
        uiController.DestroyAllPanel();
        //2.加载 虚拟摇杆界面
        Transform panelJoy = uiController.CreatePanel("PanelJoystick");
        //2.1 初始化 虚拟摇杆界面
        panelJoy.GetComponent<UIPanelJoystick>().Init();
        //3.初始化 玩家
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        player.Init();

        //玩家相机初始化
        FindObjectOfType<PlayerCameraController>().Init(player.transform);
        //4.初始化怪物

    }


    /// <summary>
    /// 更新玩家击杀任务进度
    /// </summary>
    /// <param name="contentId">任务内容id</param>
    /// <param name="num">任务内容完成 数量</param>
    public void UpdatePlayerkillQuest(int contentId,int num)
    {
        for (int i = 0; i < playerData.QuestList.Count; i++)
        {
            //找到对应任务
            if (playerData.QuestList[i].ContentId == contentId)
            {
                //更新任务进度
                playerData.QuestList[i].CurrentProgress += num;
                if (playerData.QuestList[i].CurrentProgress>= playerData.QuestList[i].TotalProgress)
                {
                    UIPanelTip.Instance.Init(string.Format("任务 <color=#FF3636FF>{0}</color> 完成！", playerData.QuestList[i].Name));
                }
            }
        }
    }

    public void PlayerFindTarget()
    {
        player.FindTarget();
    }

    //通知玩家开始释放技能
    public void PlayerStartSkill(int skillid)
    {
        player.StartSkill(skillid);
    }

    //玩家通知UI界面，开始冷却
    public void UISkillStartCd(int skillid)
    {
        //1.通知UIPaneljoystick 
        FindObjectOfType<UIPanelJoystick>().UISkillStartCd(skillid);
    }


 
    //存储功能
    public bool GetData(string name, string word)
    {

        string p = PlayerPrefs.GetString(name + word);
        // 如果不是字符串，就是存储了
        if (p != "")
        {

            playerData = JsonConvert.DeserializeObject<PlayerData>(p);

            return true;
        }
        else
        {
            return false;
        }

    }

    public void SaveData()
    {
        PlayerPrefs.SetString(UserName + PassWord, JsonConvert.SerializeObject(playerData));
    }
    void OnDestroy()
    {
        //print("hhahahahahahha");
        NetClient.Instance.Stop();
    }
}
